using UnityEngine;

public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	public TerrainConfig config;

	private const float HeightMapRes = 1f;

	private const float SplatMapRes = 0.5f;

	private const float BaseMapRes = 0.01f;

	public static int GetHeightMapRes()
	{
		return Mathf.Min(4096, Mathf.ClosestPowerOfTwo((int)((float)World.Size * 1f))) + 1;
	}

	public static int GetSplatMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)((float)World.Size * 0.5f)));
	}

	public static int GetBaseMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)((float)World.Size * 0.01f)));
	}

	public GameObject CreateTerrain()
	{
		return CreateTerrain(GetHeightMapRes(), GetSplatMapRes());
	}

	public GameObject CreateTerrain(int heightmapResolution, int alphamapResolution)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		TerrainData val = new TerrainData();
		val.set_baseMapResolution(GetBaseMapRes());
		val.set_heightmapResolution(heightmapResolution);
		val.set_alphamapResolution(alphamapResolution);
		val.set_size(new Vector3((float)World.Size, 1000f, (float)World.Size));
		Terrain component = Terrain.CreateTerrainGameObject(val).GetComponent<Terrain>();
		((Component)component).get_transform().set_position(((Component)this).get_transform().get_position() + new Vector3((float)(0L - (long)World.Size) * 0.5f, 0f, (float)(0L - (long)World.Size) * 0.5f));
		component.set_drawInstanced(false);
		component.set_castShadows(config.CastShadows);
		component.set_materialType((MaterialType)3);
		component.set_materialTemplate(config.Material);
		((Component)component).get_gameObject().set_tag(((Component)this).get_gameObject().get_tag());
		((Component)component).get_gameObject().set_layer(((Component)this).get_gameObject().get_layer());
		((Collider)((Component)component).get_gameObject().GetComponent<TerrainCollider>()).set_sharedMaterial(config.GenericMaterial);
		TerrainMeta terrainMeta = ((Component)component).get_gameObject().AddComponent<TerrainMeta>();
		((Component)component).get_gameObject().AddComponent<TerrainPhysics>();
		((Component)component).get_gameObject().AddComponent<TerrainColors>();
		((Component)component).get_gameObject().AddComponent<TerrainCollision>();
		((Component)component).get_gameObject().AddComponent<TerrainBiomeMap>();
		((Component)component).get_gameObject().AddComponent<TerrainAlphaMap>();
		((Component)component).get_gameObject().AddComponent<TerrainHeightMap>();
		((Component)component).get_gameObject().AddComponent<TerrainSplatMap>();
		((Component)component).get_gameObject().AddComponent<TerrainTopologyMap>();
		((Component)component).get_gameObject().AddComponent<TerrainWaterMap>();
		((Component)component).get_gameObject().AddComponent<TerrainPlacementMap>();
		((Component)component).get_gameObject().AddComponent<TerrainPath>();
		((Component)component).get_gameObject().AddComponent<TerrainTexturing>();
		terrainMeta.terrain = component;
		terrainMeta.config = config;
		Object.DestroyImmediate((Object)(object)((Component)this).get_gameObject());
		return ((Component)component).get_gameObject();
	}
}
