using System;
using UnityEngine;

[Serializable]
public class WaterMesh
{
	private Mesh borderMesh;

	private Mesh centerPatch;

	private int borderRingCount;

	private float borderRingSpacingFalloff;

	private int resolution;

	private Vector3[] borderVerticesLocal;

	private Vector3[] borderVerticesWorld;

	private bool initialized;

	public Mesh BorderMesh => borderMesh;

	public Mesh CenterPatch => centerPatch;

	public bool IsInitialized => initialized;

	public void Initialize(int patchResolution, float patchSizeInWorld, int borderRingCount, float borderRingSpacingFalloff)
	{
		if (!Mathf.IsPowerOfTwo(patchResolution))
		{
			Debug.LogError((object)"[Water] Patch resolution must be a power-of-two number.");
			return;
		}
		this.borderRingCount = borderRingCount;
		this.borderRingSpacingFalloff = borderRingSpacingFalloff;
		borderMesh = CreateSortedBorderPatch(patchResolution, borderRingCount, patchSizeInWorld);
		centerPatch = CreateSortedCenterPatch(patchResolution, patchSizeInWorld, borderOnly: false);
		resolution = patchResolution;
		borderVerticesLocal = (Vector3[])(object)new Vector3[borderMesh.get_vertexCount()];
		borderVerticesWorld = (Vector3[])(object)new Vector3[borderMesh.get_vertexCount()];
		Array.Copy(borderMesh.get_vertices(), borderVerticesLocal, borderMesh.get_vertexCount());
		initialized = true;
	}

	public void Destroy()
	{
		if (initialized)
		{
			Object.DestroyImmediate((Object)(object)borderMesh);
			Object.DestroyImmediate((Object)(object)centerPatch);
			initialized = false;
		}
	}

	public void UpdateBorderMesh(Matrix4x4 centerLocalToWorld, Matrix4x4 borderLocalToWorld, bool collapseCenter)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		int num = resolution * 4;
		int num2 = 0;
		int num3 = num;
		int num4 = borderMesh.get_vertexCount() - num;
		int vertexCount = borderMesh.get_vertexCount();
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(float.MinValue, float.MinValue, float.MinValue);
		Bounds bounds = default(Bounds);
		for (int i = num2; i < num3; i++)
		{
			Vector3 val3 = ((Matrix4x4)(ref borderLocalToWorld)).MultiplyPoint3x4(borderVerticesLocal[i]);
			val = Vector3.Min(val, val3);
			val2 = Vector3.Max(val2, val3);
			borderVerticesWorld[i] = val3;
		}
		((Bounds)(ref bounds)).SetMinMax(val, val2);
		if (!collapseCenter)
		{
			for (int j = num4; j < vertexCount; j++)
			{
				borderVerticesWorld[j] = ((Matrix4x4)(ref centerLocalToWorld)).MultiplyPoint3x4(borderVerticesLocal[j]);
			}
		}
		else
		{
			for (int k = num4; k < vertexCount; k++)
			{
				borderVerticesWorld[k] = ((Matrix4x4)(ref centerLocalToWorld)).MultiplyPoint3x4(Vector3.get_zero());
			}
		}
		int l = 1;
		int num5 = num3;
		for (; l < borderRingCount; l++)
		{
			float num6 = (float)l / (float)borderRingCount;
			num6 = Mathf.Clamp01(Mathf.Pow(num6, borderRingSpacingFalloff));
			int num7 = 0;
			while (num7 < num)
			{
				Vector3 val4 = borderVerticesWorld[num2 + num7];
				Vector3 val5 = borderVerticesWorld[num4 + num7];
				borderVerticesWorld[num5].x = val4.x + (val5.x - val4.x) * num6;
				borderVerticesWorld[num5].y = val4.y + (val5.y - val4.y) * num6;
				borderVerticesWorld[num5].z = val4.z + (val5.z - val4.z) * num6;
				num7++;
				num5++;
			}
		}
		borderMesh.set_vertices(borderVerticesWorld);
		borderMesh.set_bounds(bounds);
	}

	private Mesh CreateSortedBorderPatch(int resolution, int ringCount, float sizeInWorld)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
		float num = sizeInWorld / (float)resolution;
		int num2 = resolution * 4 * (ringCount + 1);
		int num3 = resolution * 4 * ringCount * 6;
		Vector3[] array = (Vector3[])(object)new Vector3[num2];
		Vector3[] array2 = (Vector3[])(object)new Vector3[num2];
		Color[] array3 = (Color[])(object)new Color[num2];
		int[] array4 = new int[num3];
		for (int i = 0; i < num2; i++)
		{
			array2[i] = Vector3.get_up();
		}
		for (int j = 0; j < num2; j++)
		{
			array3[j] = Color.get_clear();
		}
		int num4 = resolution * 4;
		float num5 = (float)resolution * num;
		Vector3 val = new Vector3(sizeInWorld, 0f, sizeInWorld) * 0.5f;
		int k = 0;
		int num6 = 0;
		for (; k < ringCount + 1; k++)
		{
			Vector3 val2 = -val;
			for (int l = 0; l < resolution; l++)
			{
				array[num6++] = val2 + new Vector3((float)l * num, 0f, 0f);
			}
			for (int m = 0; m < resolution; m++)
			{
				array[num6++] = val2 + new Vector3(num5, 0f, (float)m * num);
			}
			for (int num7 = resolution; num7 > 0; num7--)
			{
				array[num6++] = val2 + new Vector3((float)num7 * num, 0f, num5);
			}
			for (int num8 = resolution; num8 > 0; num8--)
			{
				array[num6++] = val2 + new Vector3(0f, 0f, (float)num8 * num);
			}
		}
		int n = 0;
		int num9 = 0;
		for (; n < ringCount; n++)
		{
			int num10 = n * num4;
			int num11 = num10 + num4;
			int num12 = num11;
			int num13 = num10 + num4 * 2;
			int num14 = num10;
			int num15 = num10 + num4 + 1;
			int num16 = num10 + num4;
			int num17 = num10 + 1;
			int num18 = num10 + num4 + 1;
			int num19 = num10;
			for (int num20 = 0; num20 < num4; num20++)
			{
				bool num21 = num20 % resolution == 0;
				int num22 = num14;
				int num23 = (num21 ? (num15 - num4) : num15);
				int num24 = num16;
				int num25 = num17;
				int num26 = num18;
				int num27 = (num21 ? (num19 + num4) : num19);
				if (num23 >= num13)
				{
					num23 = num12;
				}
				if (num25 >= num11)
				{
					num25 = num10;
				}
				if (num26 >= num13)
				{
					num26 = num12;
				}
				array4[num9++] = num24;
				array4[num9++] = num23;
				array4[num9++] = num22;
				array4[num9++] = num27;
				array4[num9++] = num26;
				array4[num9++] = num25;
				num14++;
				num15++;
				num16++;
				num17++;
				num18++;
				num19++;
			}
		}
		Mesh val3 = new Mesh();
		((Object)val3).set_hideFlags((HideFlags)52);
		val3.set_vertices(array);
		val3.set_normals(array2);
		val3.set_colors(array3);
		val3.set_triangles(array4);
		val3.RecalculateBounds();
		return val3;
	}

	private Mesh CreateSortedCenterPatch(int resolution, float sizeInWorld, bool borderOnly)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Expected O, but got Unknown
		float num = sizeInWorld / (float)resolution;
		int num2 = resolution + 1;
		int num3;
		int num4;
		if (borderOnly)
		{
			num3 = resolution * 8 - 8;
			num4 = (resolution - 1) * 24;
		}
		else
		{
			num3 = num2 * num2;
			num4 = resolution * resolution * 6;
		}
		Vector3[] array = (Vector3[])(object)new Vector3[num3];
		Vector3[] array2 = (Vector3[])(object)new Vector3[num3];
		Color[] array3 = (Color[])(object)new Color[num3];
		int[] array4 = new int[num4];
		for (int i = 0; i < num3; i++)
		{
			array2[i] = Vector3.get_up();
		}
		int num5 = resolution / 2;
		int num6 = num5 - 1;
		int num7 = resolution;
		int num8 = resolution * 4;
		Vector3 val = new Vector3(sizeInWorld, 0f, sizeInWorld) * 0.5f;
		if (borderOnly)
		{
			for (int j = 0; j < num3; j++)
			{
				array3[j] = Color.get_clear();
			}
		}
		else
		{
			for (int k = 0; k < num3; k++)
			{
				if (k >= num8)
				{
					array3[k] = Color.get_white();
				}
				else
				{
					array3[k] = Color.get_clear();
				}
			}
		}
		int l = 0;
		int num9 = 0;
		Vector3 val2 = default(Vector3);
		for (; l < num5 + 1; l++)
		{
			val2.x = (float)l * num;
			val2.y = 0f;
			val2.z = val2.x;
			val2 -= val;
			float num10 = (float)num7 * num;
			if (l <= num6)
			{
				for (int m = 0; m < num7; m++)
				{
					array[num9++] = val2 + new Vector3((float)m * num, 0f, 0f);
				}
				for (int n = 0; n < num7; n++)
				{
					array[num9++] = val2 + new Vector3(num10, 0f, (float)n * num);
				}
				for (int num11 = num7; num11 > 0; num11--)
				{
					array[num9++] = val2 + new Vector3((float)num11 * num, 0f, num10);
				}
				for (int num12 = num7; num12 > 0; num12--)
				{
					array[num9++] = val2 + new Vector3(0f, 0f, (float)num12 * num);
				}
			}
			else
			{
				array[num9++] = val2;
			}
			num7 -= 2;
			if (borderOnly && l >= 1)
			{
				break;
			}
		}
		int num13 = resolution;
		int num14 = resolution - 2;
		int num15 = resolution * 4;
		int num16 = num15 - 8;
		int num17 = (resolution - 1) * 4;
		int num18 = num17 - 8;
		int num19 = 0;
		int num20 = num15;
		int num21 = 0;
		int num22 = 0;
		for (; num21 < num5; num21++)
		{
			if (num21 < num6)
			{
				int num23 = num20;
				int num24 = num20 - 1;
				int num25 = num19;
				int num26 = 0;
				bool flag = true;
				for (int num27 = 0; num27 < num17; num27++)
				{
					int num28 = num23;
					int num29 = num24;
					int num30 = num25;
					num24 = num25;
					num25++;
					int num31 = num23;
					int num32 = num24;
					int num33 = num25;
					bool flag2 = (num26 & 1) == 0;
					if (flag2 || (borderOnly && flag && !flag2))
					{
						array4[num22++] = num33;
						array4[num22++] = num32;
						array4[num22++] = num31;
						array4[num22++] = num30;
						array4[num22++] = num29;
						array4[num22++] = num28;
					}
					else
					{
						array4[num22++] = num30;
						array4[num22++] = num29;
						array4[num22++] = num33;
						array4[num22++] = num33;
						array4[num22++] = num29;
						array4[num22++] = num28;
					}
					flag = (num27 + 1) % (num13 - 1) == 0;
					if (flag)
					{
						num24++;
						num25++;
						num26++;
					}
					else
					{
						num24 = num23;
						num23 = ((num23 + 1 < num20 + num16) ? (num23 + 1) : num20);
					}
				}
				num17 -= 8;
				num18 -= 8;
				num15 -= 8;
				num16 -= 8;
				num13 -= 2;
				num14 -= 2;
				num19 = num20;
				num20 += num15;
			}
			else
			{
				int num34 = num20;
				int num35 = num20 - 1;
				int num36 = num19;
				int num37 = 0;
				for (int num38 = 0; num38 < num17; num38++)
				{
					int num39 = num34;
					int num40 = num35;
					int num41 = num36;
					num35 = num36;
					num36++;
					int num42 = num34;
					int num43 = num35;
					int num44 = num36;
					num35++;
					num36++;
					if ((num37 & 1) == 0)
					{
						array4[num22++] = num44;
						array4[num22++] = num43;
						array4[num22++] = num42;
						array4[num22++] = num41;
						array4[num22++] = num40;
						array4[num22++] = num39;
					}
					else
					{
						array4[num22++] = num41;
						array4[num22++] = num40;
						array4[num22++] = num44;
						array4[num22++] = num44;
						array4[num22++] = num40;
						array4[num22++] = num39;
					}
					num37++;
				}
			}
			if (borderOnly)
			{
				break;
			}
		}
		Mesh val3 = new Mesh();
		((Object)val3).set_hideFlags((HideFlags)52);
		val3.set_vertices(array);
		val3.set_normals(array2);
		val3.set_colors(array3);
		val3.set_triangles(array4);
		val3.RecalculateBounds();
		return val3;
	}
}
