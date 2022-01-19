using System;
using Rust;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainTexturing : TerrainExtension
{
	private const int ShoreVectorDownscale = 3;

	private const int ShoreVectorBlurPasses = 0;

	private float terrainSize;

	private int shoreMapSize;

	private float[] shoreDistances;

	private Vector3[] shoreVectors;

	public bool debugFoliageDisplacement;

	private bool initialized;

	private static TerrainTexturing instance;

	public int ShoreMapSize => shoreMapSize;

	public Vector3[] ShoreMap => shoreVectors;

	public static TerrainTexturing Instance => instance;

	private void InitializeBasePyramid()
	{
	}

	private void ReleaseBasePyramid()
	{
	}

	private void UpdateBasePyramid()
	{
	}

	private void InitializeCoarseHeightSlope()
	{
	}

	private void ReleaseCoarseHeightSlope()
	{
	}

	private void UpdateCoarseHeightSlope()
	{
	}

	private void InitializeShoreVector()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.ClosestPowerOfTwo(terrain.get_terrainData().get_heightmapResolution()) >> 3;
		int num2 = num * num;
		terrainSize = Mathf.Max(terrain.get_terrainData().get_size().x, terrain.get_terrainData().get_size().z);
		shoreMapSize = num;
		shoreDistances = new float[num * num];
		shoreVectors = (Vector3[])(object)new Vector3[num * num];
		for (int i = 0; i < num2; i++)
		{
			shoreDistances[i] = 10000f;
			shoreVectors[i] = Vector3.get_one();
		}
	}

	private void GenerateShoreVector()
	{
		TimeWarning val = TimeWarning.New("GenerateShoreVector", 500);
		try
		{
			GenerateShoreVector(out shoreDistances, out shoreVectors);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ReleaseShoreVector()
	{
		shoreDistances = null;
		shoreVectors = null;
	}

	private void GenerateShoreVector(out float[] distances, out Vector3[] vectors)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		float num = terrainSize / (float)shoreMapSize;
		Vector3 position = terrain.GetPosition();
		int num2 = LayerMask.NameToLayer("Terrain");
		NativeArray<RaycastHit> val = default(NativeArray<RaycastHit>);
		val._002Ector(shoreMapSize * shoreMapSize, (Allocator)3, (NativeArrayOptions)1);
		NativeArray<RaycastCommand> val2 = default(NativeArray<RaycastCommand>);
		val2._002Ector(shoreMapSize * shoreMapSize, (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < shoreMapSize; i++)
		{
			for (int j = 0; j < shoreMapSize; j++)
			{
				float num3 = ((float)j + 0.5f) * num;
				float num4 = ((float)i + 0.5f) * num;
				Vector3 val3 = new Vector3(position.x, 0f, position.z) + new Vector3(num3, 1000f, num4);
				Vector3 down = Vector3.get_down();
				val2.set_Item(i * shoreMapSize + j, new RaycastCommand(val3, down, float.MaxValue, -5, 1));
			}
		}
		JobHandle val4 = RaycastCommand.ScheduleBatch(val2, val, 1, default(JobHandle));
		((JobHandle)(ref val4)).Complete();
		byte[] image = new byte[shoreMapSize * shoreMapSize];
		distances = new float[shoreMapSize * shoreMapSize];
		vectors = (Vector3[])(object)new Vector3[shoreMapSize * shoreMapSize];
		int k = 0;
		int num5 = 0;
		for (; k < shoreMapSize; k++)
		{
			int num6 = 0;
			while (num6 < shoreMapSize)
			{
				RaycastHit val5 = val.get_Item(k * shoreMapSize + num6);
				bool flag = ((Component)((RaycastHit)(ref val5)).get_collider()).get_gameObject().get_layer() == num2;
				image[num5] = (byte)(flag ? 255u : 0u);
				distances[num5] = (flag ? 256 : 0);
				num6++;
				num5++;
			}
		}
		ref int size = ref shoreMapSize;
		byte threshold = 127;
		DistanceField.Generate(in size, in threshold, in image, ref distances);
		DistanceField.ApplyGaussianBlur(shoreMapSize, distances, 0);
		DistanceField.GenerateVectors(in shoreMapSize, in distances, ref vectors);
		val.Dispose();
		val2.Dispose();
	}

	public float GetCoarseDistanceToShore(Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Vector2 uv = default(Vector2);
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return GetCoarseDistanceToShore(uv);
	}

	public float GetCoarseDistanceToShore(Vector2 uv)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		int num = shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = ((num3 < (float)num2) ? 1 : 0);
		int num10 = ((num4 < (float)num2) ? num : 0);
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		float num15 = shoreDistances[num11];
		float num16 = shoreDistances[num12];
		float num17 = shoreDistances[num13];
		float num18 = shoreDistances[num14];
		float num19 = (num16 - num15) * num7 + num15;
		return ((num18 - num17) * num7 + num17 - num19) * num8 + num19;
	}

	public Vector3 GetCoarseVectorToShore(Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector2 uv = default(Vector2);
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return GetCoarseVectorToShore(uv);
	}

	public Vector3 GetCoarseVectorToShore(Vector2 uv)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		int num = shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = ((num3 < (float)num2) ? 1 : 0);
		int num10 = ((num4 < (float)num2) ? num : 0);
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		Vector3 val = shoreVectors[num11];
		Vector3 val2 = shoreVectors[num12];
		Vector3 val3 = shoreVectors[num13];
		Vector3 val4 = shoreVectors[num14];
		Vector3 val5 = default(Vector3);
		val5.x = (val2.x - val.x) * num7 + val.x;
		val5.y = (val2.y - val.y) * num7 + val.y;
		val5.z = (val2.z - val.z) * num7 + val.z;
		Vector3 val6 = default(Vector3);
		val6.x = (val4.x - val3.x) * num7 + val3.x;
		val6.y = (val4.y - val3.y) * num7 + val3.y;
		val6.z = (val4.z - val3.z) * num7 + val3.z;
		float num15 = (val6.x - val5.x) * num8 + val5.x;
		float num16 = (val6.y - val5.y) * num8 + val5.y;
		float num17 = (val6.z - val5.z) * num8 + val5.z;
		return new Vector3(num15, num16, num17);
	}

	private void CheckInstance()
	{
		instance = (((Object)(object)instance != (Object)null) ? instance : this);
	}

	private void Awake()
	{
		CheckInstance();
	}

	public override void Setup()
	{
		InitializeShoreVector();
	}

	public override void PostSetup()
	{
		TerrainMeta component = ((Component)this).GetComponent<TerrainMeta>();
		if ((Object)(object)component == (Object)null || (Object)(object)component.config == (Object)null)
		{
			Debug.LogError((object)"[TerrainTexturing] Missing TerrainMeta or TerrainConfig not assigned.");
			return;
		}
		Shutdown();
		InitializeCoarseHeightSlope();
		GenerateShoreVector();
		initialized = true;
	}

	private void Shutdown()
	{
		ReleaseBasePyramid();
		ReleaseCoarseHeightSlope();
		ReleaseShoreVector();
		initialized = false;
	}

	private void OnEnable()
	{
		CheckInstance();
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			Shutdown();
		}
	}

	private void Update()
	{
		if (initialized)
		{
			UpdateBasePyramid();
			UpdateCoarseHeightSlope();
		}
	}
}
