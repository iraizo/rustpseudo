using UnityEngine;

public class PlaceDecorUniform : ProceduralComponent
{
	public SpawnFilter Filter;

	public string ResourceFolder = string.Empty;

	public float ObjectDistance = 10f;

	public float ObjectDithering = 5f;

	public override void Process(uint seed)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			World.Spawn("Decor", "assets/bundled/prefabs/autospawn/" + ResourceFolder + "/");
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + ResourceFolder);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		Vector3 pos = default(Vector3);
		for (float num3 = z; num3 < num2; num3 += ObjectDistance)
		{
			for (float num4 = x; num4 < num; num4 += ObjectDistance)
			{
				float num5 = num4 + SeedRandom.Range(ref seed, 0f - ObjectDithering, ObjectDithering);
				float num6 = num3 + SeedRandom.Range(ref seed, 0f - ObjectDithering, ObjectDithering);
				float normX = TerrainMeta.NormalizeX(num5);
				float normZ = TerrainMeta.NormalizeZ(num6);
				float num7 = SeedRandom.Value(ref seed);
				float factor = Filter.GetFactor(normX, normZ);
				Prefab random = array.GetRandom(ref seed);
				if (!(factor * factor < num7))
				{
					float height = heightMap.GetHeight(normX, normZ);
					((Vector3)(ref pos))._002Ector(num5, height, num6);
					Quaternion rot = random.Object.get_transform().get_localRotation();
					Vector3 scale = random.Object.get_transform().get_localScale();
					random.ApplyDecorComponents(ref pos, ref rot, ref scale);
					if (random.ApplyTerrainAnchors(ref pos, rot, scale, Filter) && random.ApplyTerrainChecks(pos, rot, scale, Filter) && random.ApplyTerrainFilters(pos, rot, scale) && random.ApplyWaterChecks(pos, rot, scale))
					{
						World.AddPrefab("Decor", random, pos, rot, scale);
					}
				}
			}
		}
	}
}
