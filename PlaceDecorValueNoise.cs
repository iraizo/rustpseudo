using UnityEngine;

public class PlaceDecorValueNoise : ProceduralComponent
{
	public SpawnFilter Filter;

	public string ResourceFolder = string.Empty;

	public NoiseParameters Cluster = new NoiseParameters(2, 0.5f, 1f, 0f);

	public float ObjectDensity = 100f;

	public override void Process(uint seed)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
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
		int num = Mathf.RoundToInt(ObjectDensity * size.x * size.z * 1E-06f);
		float x = position.x;
		float z = position.z;
		float num2 = position.x + size.x;
		float num3 = position.z + size.z;
		float num4 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		float num5 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		int octaves = Cluster.Octaves;
		float offset = Cluster.Offset;
		float frequency = Cluster.Frequency * 0.01f;
		float amplitude = Cluster.Amplitude;
		Vector3 pos = default(Vector3);
		for (int i = 0; i < num; i++)
		{
			float num6 = SeedRandom.Range(ref seed, x, num2);
			float num7 = SeedRandom.Range(ref seed, z, num3);
			float normX = TerrainMeta.NormalizeX(num6);
			float normZ = TerrainMeta.NormalizeZ(num7);
			float num8 = SeedRandom.Value(ref seed);
			float factor = Filter.GetFactor(normX, normZ);
			Prefab random = array.GetRandom(ref seed);
			if (!(factor <= 0f) && !((offset + Noise.Turbulence(num4 + num6, num5 + num7, octaves, frequency, amplitude)) * factor * factor < num8))
			{
				float height = heightMap.GetHeight(normX, normZ);
				((Vector3)(ref pos))._002Ector(num6, height, num7);
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
