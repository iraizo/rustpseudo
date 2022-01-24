using UnityEngine;

public class MonumentNode : MonoBehaviour
{
	public string ResourceFolder = string.Empty;

	protected void Awake()
	{
		if (!((Object)(object)SingletonComponent<WorldSetup>.Instance == (Object)null))
		{
			if (SingletonComponent<WorldSetup>.Instance.MonumentNodes == null)
			{
				Debug.LogError((object)"WorldSetup.Instance.MonumentNodes is null.", (Object)(object)this);
			}
			else
			{
				SingletonComponent<WorldSetup>.Instance.MonumentNodes.Add(this);
			}
		}
	}

	public void Process(ref uint seed)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			World.Spawn("Monument", "assets/bundled/prefabs/autospawn/" + ResourceFolder + "/");
			return;
		}
		Prefab<MonumentInfo>[] array = Prefab.Load<MonumentInfo>("assets/bundled/prefabs/autospawn/" + ResourceFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array != null && array.Length != 0)
		{
			Prefab<MonumentInfo> random = array.GetRandom(ref seed);
			float height = TerrainMeta.HeightMap.GetHeight(((Component)this).get_transform().get_position());
			Vector3 pos = default(Vector3);
			((Vector3)(ref pos))._002Ector(((Component)this).get_transform().get_position().x, height, ((Component)this).get_transform().get_position().z);
			Quaternion rot = random.Object.get_transform().get_localRotation();
			Vector3 scale = random.Object.get_transform().get_localScale();
			random.ApplyDecorComponents(ref pos, ref rot, ref scale);
			World.AddPrefab("Monument", random, pos, rot, scale);
		}
	}

	public MonumentNode()
		: this()
	{
	}
}
