using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceMonumentsOffshore : ProceduralComponent
{
	private struct SpawnInfo
	{
		public Prefab prefab;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}

	public string ResourceFolder = string.Empty;

	public int TargetCount;

	public int MinDistanceFromTerrain = 100;

	public int MaxDistanceFromTerrain = 500;

	public int DistanceBetweenMonuments = 500;

	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	private const int Candidates = 10;

	private const int Attempts = 10000;

	public override void Process(uint seed)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		string[] array = (from folder in ResourceFolder.Split(',')
			select "assets/bundled/prefabs/autospawn/" + folder + "/").ToArray();
		if (World.Networked)
		{
			World.Spawn("Monument", array);
		}
		else
		{
			if (World.Size < MinWorldSize)
			{
				return;
			}
			TerrainHeightMap heightMap = TerrainMeta.HeightMap;
			List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
				array3.Shuffle(ref seed);
				list.AddRange(array3);
			}
			Prefab<MonumentInfo>[] array4 = list.ToArray();
			if (array4 == null || array4.Length == 0)
			{
				return;
			}
			array4.BubbleSort();
			Vector3 position = TerrainMeta.Position;
			Vector3 size = TerrainMeta.Size;
			float num = position.x - (float)MaxDistanceFromTerrain;
			float num2 = position.x - (float)MinDistanceFromTerrain;
			float num3 = position.x + size.x + (float)MinDistanceFromTerrain;
			float num4 = position.x + size.x + (float)MaxDistanceFromTerrain;
			float num5 = position.z - (float)MaxDistanceFromTerrain;
			_ = MinDistanceFromTerrain;
			float num6 = position.z + size.z + (float)MinDistanceFromTerrain;
			float num7 = position.z + size.z + (float)MaxDistanceFromTerrain;
			int num8 = 0;
			List<SpawnInfo> list2 = new List<SpawnInfo>();
			int num9 = 0;
			List<SpawnInfo> list3 = new List<SpawnInfo>();
			Vector3 pos = default(Vector3);
			for (int j = 0; j < 10; j++)
			{
				num8 = 0;
				list2.Clear();
				Prefab<MonumentInfo>[] array5 = array4;
				foreach (Prefab<MonumentInfo> prefab in array5)
				{
					int num10 = (int)((!Object.op_Implicit((Object)(object)prefab.Parameters)) ? PrefabPriority.Low : (prefab.Parameters.Priority + 1));
					int num11 = num10 * num10 * num10 * num10;
					for (int k = 0; k < 10000; k++)
					{
						float num12 = 0f;
						float num13 = 0f;
						switch (seed % 4u)
						{
						case 0u:
							num12 = SeedRandom.Range(ref seed, num, num2);
							num13 = SeedRandom.Range(ref seed, num5, num7);
							break;
						case 1u:
							num12 = SeedRandom.Range(ref seed, num3, num4);
							num13 = SeedRandom.Range(ref seed, num5, num7);
							break;
						case 2u:
							num12 = SeedRandom.Range(ref seed, num, num4);
							num13 = SeedRandom.Range(ref seed, num5, num5);
							break;
						case 3u:
							num12 = SeedRandom.Range(ref seed, num, num4);
							num13 = SeedRandom.Range(ref seed, num6, num7);
							break;
						}
						float normX = TerrainMeta.NormalizeX(num12);
						float normZ = TerrainMeta.NormalizeZ(num13);
						float height = heightMap.GetHeight(normX, normZ);
						((Vector3)(ref pos))._002Ector(num12, height, num13);
						Quaternion rot = prefab.Object.get_transform().get_localRotation();
						Vector3 scale = prefab.Object.get_transform().get_localScale();
						if (!CheckRadius(list2, pos, DistanceBetweenMonuments))
						{
							prefab.ApplyDecorComponents(ref pos, ref rot, ref scale);
							if ((!Object.op_Implicit((Object)(object)prefab.Component) || prefab.Component.CheckPlacement(pos, rot, scale)) && !prefab.CheckEnvironmentVolumes(pos, rot, scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
							{
								SpawnInfo item = default(SpawnInfo);
								item.prefab = prefab;
								item.position = pos;
								item.rotation = rot;
								item.scale = scale;
								list2.Add(item);
								num8 += num11;
								break;
							}
						}
					}
					if (TargetCount > 0 && list2.Count >= TargetCount)
					{
						break;
					}
				}
				if (num8 > num9)
				{
					num9 = num8;
					GenericsUtil.Swap<List<SpawnInfo>>(ref list2, ref list3);
				}
			}
			foreach (SpawnInfo item2 in list3)
			{
				World.AddPrefab("Monument", item2.prefab, item2.position, item2.rotation, item2.scale);
			}
		}
	}

	private bool CheckRadius(List<SpawnInfo> spawns, Vector3 pos, float radius)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		float num = radius * radius;
		foreach (SpawnInfo spawn in spawns)
		{
			Vector3 val = spawn.position - pos;
			if (((Vector3)(ref val)).get_sqrMagnitude() < num)
			{
				return true;
			}
		}
		return false;
	}
}
