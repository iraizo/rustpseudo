using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class FileSystem_Warmup : MonoBehaviour
{
	private static bool run = true;

	private static bool running = false;

	public static string[] ExcludeFilter = new string[11]
	{
		"/bundled/prefabs/autospawn/monument", "/bundled/prefabs/autospawn/mountain", "/bundled/prefabs/autospawn/canyon", "/bundled/prefabs/autospawn/decor", "/bundled/prefabs/navmesh", "/content/ui/", "/prefabs/ui/", "/prefabs/world/", "/prefabs/system/", "/standard assets/",
		"/third party/"
	};

	public static void Run()
	{
		if (run && !running)
		{
			running = true;
			string[] assetList = GetAssetList();
			for (int i = 0; i < assetList.Length; i++)
			{
				PrefabWarmup(assetList[i]);
			}
			running = (run = false);
		}
	}

	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		if (!run || running)
		{
			yield break;
		}
		running = true;
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
		running = (run = false);
	}

	private static bool ShouldIgnore(string path)
	{
		for (int i = 0; i < ExcludeFilter.Length; i++)
		{
			if (StringEx.Contains(path, ExcludeFilter[i], CompareOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static string[] GetAssetList()
	{
		return Enumerable.ToArray<string>(Enumerable.Where<string>(Enumerable.Select<GameManifest.PrefabProperties, string>((IEnumerable<GameManifest.PrefabProperties>)GameManifest.Current.prefabProperties, (Func<GameManifest.PrefabProperties, string>)((GameManifest.PrefabProperties x) => x.name)), (Func<string, bool>)((string x) => !ShouldIgnore(x))));
	}

	private static void PrefabWarmup(string path)
	{
		GameManager.server.FindPrefab(path);
	}

	public FileSystem_Warmup()
		: this()
	{
	}
}
