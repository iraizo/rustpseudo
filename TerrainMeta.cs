using UnityEngine;

[ExecuteInEditMode]
public class TerrainMeta : MonoBehaviour
{
	public enum PaintMode
	{
		None,
		Splats,
		Biomes,
		Alpha,
		Blend,
		Field,
		Cliff,
		Summit,
		Beachside,
		Beach,
		Forest,
		Forestside,
		Ocean,
		Oceanside,
		Decor,
		Monument,
		Road,
		Roadside,
		Bridge,
		River,
		Riverside,
		Lake,
		Lakeside,
		Offshore,
		Powerline,
		Plain,
		Building,
		Cliffside,
		Mountain,
		Clutter,
		Alt,
		Tier0,
		Tier1,
		Tier2,
		Mainland,
		Hilltop
	}

	public Terrain terrain;

	public TerrainConfig config;

	public PaintMode paint;

	[HideInInspector]
	public PaintMode currentPaintMode;

	public static TerrainConfig Config { get; private set; }

	public static Terrain Terrain { get; private set; }

	public static Transform Transform { get; private set; }

	public static Vector3 Position { get; private set; }

	public static Vector3 Size { get; private set; }

	public static Vector3 Center => Position + Size * 0.5f;

	public static Vector3 OneOverSize { get; private set; }

	public static Vector3 HighestPoint { get; set; }

	public static Vector3 LowestPoint { get; set; }

	public static float LootAxisAngle { get; private set; }

	public static float BiomeAxisAngle { get; private set; }

	public static TerrainData Data { get; private set; }

	public static TerrainCollider Collider { get; private set; }

	public static TerrainCollision Collision { get; private set; }

	public static TerrainPhysics Physics { get; private set; }

	public static TerrainColors Colors { get; private set; }

	public static TerrainQuality Quality { get; private set; }

	public static TerrainPath Path { get; private set; }

	public static TerrainBiomeMap BiomeMap { get; private set; }

	public static TerrainAlphaMap AlphaMap { get; private set; }

	public static TerrainBlendMap BlendMap { get; private set; }

	public static TerrainHeightMap HeightMap { get; private set; }

	public static TerrainSplatMap SplatMap { get; private set; }

	public static TerrainTopologyMap TopologyMap { get; private set; }

	public static TerrainWaterMap WaterMap { get; private set; }

	public static TerrainDistanceMap DistanceMap { get; private set; }

	public static TerrainPlacementMap PlacementMap { get; private set; }

	public static TerrainTexturing Texturing { get; private set; }

	public static bool OutOfBounds(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (worldPos.x < Position.x)
		{
			return true;
		}
		if (worldPos.z < Position.z)
		{
			return true;
		}
		if (worldPos.x > Position.x + Size.x)
		{
			return true;
		}
		if (worldPos.z > Position.z + Size.z)
		{
			return true;
		}
		return false;
	}

	public static bool OutOfMargin(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (worldPos.x < Position.x - Size.x)
		{
			return true;
		}
		if (worldPos.z < Position.z - Size.z)
		{
			return true;
		}
		if (worldPos.x > Position.x + Size.x + Size.x)
		{
			return true;
		}
		if (worldPos.z > Position.z + Size.z + Size.z)
		{
			return true;
		}
		return false;
	}

	public static Vector3 RandomPointOffshore()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		float num = Random.Range(-1f, 1f);
		float num2 = Random.Range(0f, 100f);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Min(Size.x, 4000f) - 100f, 0f, Mathf.Min(Size.z, 4000f) - 100f);
		if (num2 < 25f)
		{
			return Center + new Vector3(0f - val.x, 0f, num * val.z);
		}
		if (num2 < 50f)
		{
			return Center + new Vector3(val.x, 0f, num * val.z);
		}
		if (num2 < 75f)
		{
			return Center + new Vector3(num * val.x, 0f, 0f - val.z);
		}
		return Center + new Vector3(num * val.x, 0f, val.z);
	}

	public static Vector3 Normalize(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		float num = (worldPos.x - Position.x) * OneOverSize.x;
		float num2 = (worldPos.y - Position.y) * OneOverSize.y;
		float num3 = (worldPos.z - Position.z) * OneOverSize.z;
		return new Vector3(num, num2, num3);
	}

	public static float NormalizeX(float x)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return (x - Position.x) * OneOverSize.x;
	}

	public static float NormalizeY(float y)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return (y - Position.y) * OneOverSize.y;
	}

	public static float NormalizeZ(float z)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return (z - Position.z) * OneOverSize.z;
	}

	public static Vector3 Denormalize(Vector3 normPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		float num = Position.x + normPos.x * Size.x;
		float num2 = Position.y + normPos.y * Size.y;
		float num3 = Position.z + normPos.z * Size.z;
		return new Vector3(num, num2, num3);
	}

	public static float DenormalizeX(float normX)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Position.x + normX * Size.x;
	}

	public static float DenormalizeY(float normY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Position.y + normY * Size.y;
	}

	public static float DenormalizeZ(float normZ)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Position.z + normZ * Size.z;
	}

	protected void Awake()
	{
		if (Application.get_isPlaying())
		{
			Shader.DisableKeyword("TERRAIN_PAINTING");
		}
	}

	public void Init(Terrain terrainOverride = null, TerrainConfig configOverride = null)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)terrainOverride != (Object)null)
		{
			terrain = terrainOverride;
		}
		if ((Object)(object)configOverride != (Object)null)
		{
			config = configOverride;
		}
		Terrain = terrain;
		Config = config;
		Transform = ((Component)terrain).get_transform();
		Data = terrain.get_terrainData();
		Size = terrain.get_terrainData().get_size();
		OneOverSize = Vector3Ex.Inverse(Size);
		Position = terrain.GetPosition();
		Collider = ((Component)terrain).GetComponent<TerrainCollider>();
		Collision = ((Component)terrain).GetComponent<TerrainCollision>();
		Physics = ((Component)terrain).GetComponent<TerrainPhysics>();
		Colors = ((Component)terrain).GetComponent<TerrainColors>();
		Quality = ((Component)terrain).GetComponent<TerrainQuality>();
		Path = ((Component)terrain).GetComponent<TerrainPath>();
		BiomeMap = ((Component)terrain).GetComponent<TerrainBiomeMap>();
		AlphaMap = ((Component)terrain).GetComponent<TerrainAlphaMap>();
		BlendMap = ((Component)terrain).GetComponent<TerrainBlendMap>();
		HeightMap = ((Component)terrain).GetComponent<TerrainHeightMap>();
		SplatMap = ((Component)terrain).GetComponent<TerrainSplatMap>();
		TopologyMap = ((Component)terrain).GetComponent<TerrainTopologyMap>();
		WaterMap = ((Component)terrain).GetComponent<TerrainWaterMap>();
		DistanceMap = ((Component)terrain).GetComponent<TerrainDistanceMap>();
		PlacementMap = ((Component)terrain).GetComponent<TerrainPlacementMap>();
		Texturing = ((Component)terrain).GetComponent<TerrainTexturing>();
		terrain.set_drawInstanced(false);
		HighestPoint = new Vector3(Position.x, Position.y + Size.y, Position.z);
		LowestPoint = new Vector3(Position.x, Position.y, Position.z);
		TerrainExtension[] components = ((Component)this).GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Init(terrain, config);
		}
		uint seed = World.Seed;
		int num = SeedRandom.Range(ref seed, 0, 4) * 90;
		int num2 = SeedRandom.Range(ref seed, -45, 46);
		int num3 = SeedRandom.Sign(ref seed);
		LootAxisAngle = num;
		BiomeAxisAngle = num + num2 + num3 * 90;
	}

	public static void InitNoTerrain(bool createPath = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Size = new Vector3(4096f, 4096f, 4096f);
		OneOverSize = Vector3Ex.Inverse(Size);
		Position = -0.5f * Size;
	}

	public void SetupComponents()
	{
		TerrainExtension[] components = ((Component)this).GetComponents<TerrainExtension>();
		foreach (TerrainExtension obj in components)
		{
			obj.Setup();
			obj.isInitialized = true;
		}
	}

	public void PostSetupComponents()
	{
		TerrainExtension[] components = ((Component)this).GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].PostSetup();
		}
	}

	public void BindShaderProperties()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)config))
		{
			Shader.SetGlobalTexture("Terrain_AlbedoArray", config.AlbedoArray);
			Shader.SetGlobalTexture("Terrain_NormalArray", config.NormalArray);
			Shader.SetGlobalVector("Terrain_TexelSize", Vector4.op_Implicit(new Vector2(1f / config.GetMinSplatTiling(), 1f / config.GetMinSplatTiling())));
			Shader.SetGlobalVector("Terrain_TexelSize0", new Vector4(1f / config.Splats[0].SplatTiling, 1f / config.Splats[1].SplatTiling, 1f / config.Splats[2].SplatTiling, 1f / config.Splats[3].SplatTiling));
			Shader.SetGlobalVector("Terrain_TexelSize1", new Vector4(1f / config.Splats[4].SplatTiling, 1f / config.Splats[5].SplatTiling, 1f / config.Splats[6].SplatTiling, 1f / config.Splats[7].SplatTiling));
			Shader.SetGlobalVector("Splat0_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[0].UVMIXMult, config.Splats[0].UVMIXStart, 1f / config.Splats[0].UVMIXDist)));
			Shader.SetGlobalVector("Splat1_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[1].UVMIXMult, config.Splats[1].UVMIXStart, 1f / config.Splats[1].UVMIXDist)));
			Shader.SetGlobalVector("Splat2_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[2].UVMIXMult, config.Splats[2].UVMIXStart, 1f / config.Splats[2].UVMIXDist)));
			Shader.SetGlobalVector("Splat3_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[3].UVMIXMult, config.Splats[3].UVMIXStart, 1f / config.Splats[3].UVMIXDist)));
			Shader.SetGlobalVector("Splat4_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[4].UVMIXMult, config.Splats[4].UVMIXStart, 1f / config.Splats[4].UVMIXDist)));
			Shader.SetGlobalVector("Splat5_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[5].UVMIXMult, config.Splats[5].UVMIXStart, 1f / config.Splats[5].UVMIXDist)));
			Shader.SetGlobalVector("Splat6_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[6].UVMIXMult, config.Splats[6].UVMIXStart, 1f / config.Splats[6].UVMIXDist)));
			Shader.SetGlobalVector("Splat7_UVMIX", Vector4.op_Implicit(new Vector3(config.Splats[7].UVMIXMult, config.Splats[7].UVMIXStart, 1f / config.Splats[7].UVMIXDist)));
		}
		if (Object.op_Implicit((Object)(object)HeightMap))
		{
			Shader.SetGlobalTexture("Terrain_Normal", (Texture)(object)HeightMap.NormalTexture);
		}
		if (Object.op_Implicit((Object)(object)AlphaMap))
		{
			Shader.SetGlobalTexture("Terrain_Alpha", (Texture)(object)AlphaMap.AlphaTexture);
		}
		if (Object.op_Implicit((Object)(object)BiomeMap))
		{
			Shader.SetGlobalTexture("Terrain_Biome", (Texture)(object)BiomeMap.BiomeTexture);
		}
		if (Object.op_Implicit((Object)(object)SplatMap))
		{
			Shader.SetGlobalTexture("Terrain_Control0", (Texture)(object)SplatMap.SplatTexture0);
			Shader.SetGlobalTexture("Terrain_Control1", (Texture)(object)SplatMap.SplatTexture1);
		}
		Object.op_Implicit((Object)(object)WaterMap);
		if (Object.op_Implicit((Object)(object)DistanceMap))
		{
			Shader.SetGlobalTexture("Terrain_Distance", (Texture)(object)DistanceMap.DistanceTexture);
		}
		if (!Object.op_Implicit((Object)(object)terrain))
		{
			return;
		}
		Shader.SetGlobalVector("Terrain_Position", Vector4.op_Implicit(Position));
		Shader.SetGlobalVector("Terrain_Size", Vector4.op_Implicit(Size));
		Shader.SetGlobalVector("Terrain_RcpSize", Vector4.op_Implicit(OneOverSize));
		if (Object.op_Implicit((Object)(object)terrain.get_materialTemplate()))
		{
			if (terrain.get_materialTemplate().IsKeywordEnabled("_TERRAIN_BLEND_LINEAR"))
			{
				terrain.get_materialTemplate().DisableKeyword("_TERRAIN_BLEND_LINEAR");
			}
			if (terrain.get_materialTemplate().IsKeywordEnabled("_TERRAIN_VERTEX_NORMALS"))
			{
				terrain.get_materialTemplate().DisableKeyword("_TERRAIN_VERTEX_NORMALS");
			}
		}
	}

	public TerrainMeta()
		: this()
	{
	}
}
