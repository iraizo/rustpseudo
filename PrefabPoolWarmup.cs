using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConVar;
using Rust;
using UnityEngine;

public class PrefabPoolWarmup
{
	public static void Run()
	{
		if (!Application.isLoadingPrefabs)
		{
			Application.isLoadingPrefabs = true;
			string[] assetList = GetAssetList();
			for (int i = 0; i < assetList.Length; i++)
			{
				PrefabWarmup(assetList[i]);
			}
			Application.isLoadingPrefabs = false;
		}
	}

	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		if (Application.get_isEditor() || Application.isLoadingPrefabs || !Pool.prewarm)
		{
			yield break;
		}
		Application.isLoadingPrefabs = true;
		string[] prewarmAssets = GetAssetList();
		Stopwatch sw = Stopwatch.StartNew();
		for (int i = 0; i < prewarmAssets.Length; i++)
		{
			if (sw.get_Elapsed().TotalSeconds > (double)deltaTime || i == 0 || i == prewarmAssets.Length - 1)
			{
				statusFunction?.Invoke(string.Format((format != null) ? format : "{0}/{1}", i + 1, prewarmAssets.Length));
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			PrefabWarmup(prewarmAssets[i]);
		}
		Application.isLoadingPrefabs = false;
	}

	public static string[] GetAssetList()
	{
		return Enumerable.ToArray<string>(Enumerable.Select<GameManifest.PrefabProperties, string>(Enumerable.Where<GameManifest.PrefabProperties>((IEnumerable<GameManifest.PrefabProperties>)GameManifest.Current.prefabProperties, (Func<GameManifest.PrefabProperties, bool>)((GameManifest.PrefabProperties x) => x.pool)), (Func<GameManifest.PrefabProperties, string>)((GameManifest.PrefabProperties x) => x.name)));
	}

	private static void PrefabWarmup(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		GameObject val = GameManager.server.FindPrefab(path);
		if ((Object)(object)val != (Object)null && val.SupportsPooling())
		{
			int serverCount = val.GetComponent<Poolable>().ServerCount;
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < serverCount; i++)
			{
				list.Add(GameManager.server.CreatePrefab(path));
			}
			for (int j = 0; j < serverCount; j++)
			{
				GameManager.server.Retire(list[j]);
			}
		}
	}
}
