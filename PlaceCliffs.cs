using System.Linq;
using UnityEngine;

public class PlaceCliffs : ProceduralComponent
{
	private class CliffPlacement
	{
		public int count;

		public int score;

		public Prefab prefab;

		public Vector3 pos = Vector3.get_zero();

		public Quaternion rot = Quaternion.get_identity();

		public Vector3 scale = Vector3.get_one();

		public CliffPlacement next;
	}

	public SpawnFilter Filter;

	public string ResourceFolder = string.Empty;

	public int RetryMultiplier = 1;

	public int CutoffSlope = 10;

	public float MinScale = 1f;

	public float MaxScale = 2f;

	private static int target_count = 4;

	private static int target_length = 0;

	private static float min_scale_delta = 0.1f;

	private static int max_scale_attempts = 10;

	private static int min_rotation = rotation_delta;

	private static int max_rotation = 60;

	private static int rotation_delta = 10;

	private static float offset_c = 0f;

	private static float offset_l = -0.75f;

	private static float offset_r = 0.75f;

	private static Vector3[] offsets = (Vector3[])(object)new Vector3[5]
	{
		new Vector3(offset_c, offset_c, offset_c),
		new Vector3(offset_l, offset_c, offset_c),
		new Vector3(offset_r, offset_c, offset_c),
		new Vector3(offset_c, offset_c, offset_l),
		new Vector3(offset_c, offset_c, offset_r)
	};

	public override void Process(uint seed)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
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
		Prefab[] array2 = array.Where((Prefab prefab) => (bool)prefab.Attribute.Find<DecorSocketMale>(prefab.ID) && (bool)prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)).ToArray();
		if (array2 == null || array2.Length == 0)
		{
			return;
		}
		Prefab[] array3 = array.Where((Prefab prefab) => prefab.Attribute.Find<DecorSocketMale>(prefab.ID)).ToArray();
		if (array3 == null || array3.Length == 0)
		{
			return;
		}
		Prefab[] array4 = array.Where((Prefab prefab) => prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)).ToArray();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		int num3 = Mathf.RoundToInt(size.x * size.z * 0.001f * (float)RetryMultiplier);
		Vector3 val = default(Vector3);
		for (int i = 0; i < num3; i++)
		{
			float num4 = SeedRandom.Range(ref seed, x, num);
			float num5 = SeedRandom.Range(ref seed, z, num2);
			float normX = TerrainMeta.NormalizeX(num4);
			float normZ = TerrainMeta.NormalizeZ(num5);
			float num6 = SeedRandom.Value(ref seed);
			float factor = Filter.GetFactor(normX, normZ);
			Prefab random = array2.GetRandom(ref seed);
			if (factor * factor < num6)
			{
				continue;
			}
			Vector3 normal = TerrainMeta.HeightMap.GetNormal(normX, normZ);
			if (Vector3.Angle(Vector3.get_up(), normal) < (float)CutoffSlope)
			{
				continue;
			}
			float height = heightMap.GetHeight(normX, normZ);
			((Vector3)(ref val))._002Ector(num4, height, num5);
			Quaternion val2 = QuaternionEx.LookRotationForcedUp(normal, Vector3.get_up());
			float num7 = Mathf.Max((MaxScale - MinScale) / (float)max_scale_attempts, min_scale_delta);
			for (float num8 = MaxScale; num8 >= MinScale; num8 -= num7)
			{
				Vector3 pos = val;
				Quaternion val3 = val2 * random.Object.get_transform().get_localRotation();
				Vector3 val4 = num8 * random.Object.get_transform().get_localScale();
				if (random.ApplyTerrainAnchors(ref pos, val3, val4) && random.ApplyTerrainChecks(pos, val3, val4) && random.ApplyTerrainFilters(pos, val3, val4) && random.ApplyWaterChecks(pos, val3, val4))
				{
					CliffPlacement cliffPlacement = PlaceMale(array3, ref seed, random, pos, val3, val4);
					CliffPlacement cliffPlacement2 = PlaceFemale(array4, ref seed, random, pos, val3, val4);
					World.AddPrefab("Decor", random, pos, val3, val4);
					while (cliffPlacement != null && cliffPlacement.prefab != null)
					{
						World.AddPrefab("Decor", cliffPlacement.prefab, cliffPlacement.pos, cliffPlacement.rot, cliffPlacement.scale);
						cliffPlacement = cliffPlacement.next;
						i++;
					}
					while (cliffPlacement2 != null && cliffPlacement2.prefab != null)
					{
						World.AddPrefab("Decor", cliffPlacement2.prefab, cliffPlacement2.pos, cliffPlacement2.rot, cliffPlacement2.scale);
						cliffPlacement2 = cliffPlacement2.next;
						i++;
					}
					break;
				}
			}
		}
	}

	private CliffPlacement PlaceMale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Place<DecorSocketFemale, DecorSocketMale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale);
	}

	private CliffPlacement PlaceFemale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Place<DecorSocketMale, DecorSocketFemale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale);
	}

	private CliffPlacement Place<ParentSocketType, ChildSocketType>(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale, int parentAngle = 0, int parentCount = 0, int parentScore = 0) where ParentSocketType : PrefabAttribute where ChildSocketType : PrefabAttribute
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		CliffPlacement cliffPlacement = null;
		if (parentAngle > 160 || parentAngle < -160)
		{
			return cliffPlacement;
		}
		int num = SeedRandom.Range(ref seed, 0, prefabs.Length);
		ParentSocketType val = parentPrefab.Attribute.Find<ParentSocketType>(parentPrefab.ID);
		Vector3 val2 = parentPos + parentRot * Vector3.Scale(val.worldPosition, parentScale);
		float num2 = Mathf.Max((MaxScale - MinScale) / (float)max_scale_attempts, min_scale_delta);
		for (int i = 0; i < prefabs.Length; i++)
		{
			Prefab prefab = prefabs[(num + i) % prefabs.Length];
			if (prefab == parentPrefab)
			{
				continue;
			}
			ParentSocketType val3 = prefab.Attribute.Find<ParentSocketType>(prefab.ID);
			ChildSocketType val4 = prefab.Attribute.Find<ChildSocketType>(prefab.ID);
			bool flag = (PrefabAttribute)val3 != (PrefabAttribute)null;
			if (cliffPlacement != null && cliffPlacement.count > target_count && cliffPlacement.score > target_length && flag)
			{
				continue;
			}
			float num3 = MaxScale;
			while (num3 >= MinScale)
			{
				int j;
				Vector3 val6;
				Quaternion val7;
				Vector3 pos;
				for (j = min_rotation; j <= max_rotation; j += rotation_delta)
				{
					for (int k = -1; k <= 1; k += 2)
					{
						Vector3[] array = offsets;
						int num4 = 0;
						while (num4 < array.Length)
						{
							Vector3 val5 = array[num4];
							val6 = parentScale * num3;
							val7 = Quaternion.Euler(0f, (float)(k * j), 0f) * parentRot;
							pos = val2 - val7 * (Vector3.Scale(val4.worldPosition, val6) + val5);
							if (Filter.GetFactor(pos) < 0.5f || !prefab.ApplyTerrainAnchors(ref pos, val7, val6) || !prefab.ApplyTerrainChecks(pos, val7, val6) || !prefab.ApplyTerrainFilters(pos, val7, val6) || !prefab.ApplyWaterChecks(pos, val7, val6))
							{
								num4++;
								continue;
							}
							goto IL_01dd;
						}
					}
				}
				num3 -= num2;
				continue;
				IL_01dd:
				int parentAngle2 = parentAngle + j;
				int num5 = parentCount + 1;
				int num6 = parentScore + Mathf.CeilToInt(Vector3Ex.Distance2D(parentPos, pos));
				CliffPlacement cliffPlacement2 = null;
				if (flag)
				{
					cliffPlacement2 = Place<ParentSocketType, ChildSocketType>(prefabs, ref seed, prefab, pos, val7, val6, parentAngle2, num5, num6);
					if (cliffPlacement2 != null)
					{
						num5 = cliffPlacement2.count;
						num6 = cliffPlacement2.score;
					}
				}
				else
				{
					num6 *= 2;
				}
				if (cliffPlacement == null)
				{
					cliffPlacement = new CliffPlacement();
				}
				if (cliffPlacement.score < num6)
				{
					cliffPlacement.next = cliffPlacement2;
					cliffPlacement.count = num5;
					cliffPlacement.score = num6;
					cliffPlacement.prefab = prefab;
					cliffPlacement.pos = pos;
					cliffPlacement.rot = val7;
					cliffPlacement.scale = val6;
				}
				break;
			}
		}
		return cliffPlacement;
	}
}
