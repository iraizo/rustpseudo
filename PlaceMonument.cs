using UnityEngine;

public class PlaceMonument : ProceduralComponent
{
	private struct SpawnInfo
	{
		public Prefab prefab;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}

	public SpawnFilter Filter;

	public GameObjectRef Monument;

	private const int Attempts = 10000;

	public override void Process(uint seed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		SpawnInfo spawnInfo = default(SpawnInfo);
		int num3 = int.MinValue;
		Prefab<MonumentInfo> prefab = Prefab.Load<MonumentInfo>(Monument.resourceID, (GameManager)null, (PrefabAttribute.Library)null);
		Vector3 pos = default(Vector3);
		for (int i = 0; i < 10000; i++)
		{
			float num4 = SeedRandom.Range(ref seed, x, num);
			float num5 = SeedRandom.Range(ref seed, z, num2);
			float normX = TerrainMeta.NormalizeX(num4);
			float normZ = TerrainMeta.NormalizeZ(num5);
			float num6 = SeedRandom.Value(ref seed);
			float factor = Filter.GetFactor(normX, normZ);
			if (factor * factor < num6)
			{
				continue;
			}
			float height = heightMap.GetHeight(normX, normZ);
			((Vector3)(ref pos))._002Ector(num4, height, num5);
			Quaternion rot = prefab.Object.get_transform().get_localRotation();
			Vector3 scale = prefab.Object.get_transform().get_localScale();
			prefab.ApplyDecorComponents(ref pos, ref rot, ref scale);
			if ((!Object.op_Implicit((Object)(object)prefab.Component) || prefab.Component.CheckPlacement(pos, rot, scale)) && prefab.ApplyTerrainAnchors(ref pos, rot, scale, Filter) && prefab.ApplyTerrainChecks(pos, rot, scale, Filter) && prefab.ApplyTerrainFilters(pos, rot, scale) && prefab.ApplyWaterChecks(pos, rot, scale) && !prefab.CheckEnvironmentVolumes(pos, rot, scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
			{
				SpawnInfo spawnInfo2 = default(SpawnInfo);
				spawnInfo2.prefab = prefab;
				spawnInfo2.position = pos;
				spawnInfo2.rotation = rot;
				spawnInfo2.scale = scale;
				int num7 = -Mathf.RoundToInt(Vector3Ex.Magnitude2D(pos));
				if (num7 > num3)
				{
					num3 = num7;
					spawnInfo = spawnInfo2;
				}
			}
		}
		if (num3 != int.MinValue)
		{
			World.AddPrefab("Monument", spawnInfo.prefab, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
		}
	}
}
