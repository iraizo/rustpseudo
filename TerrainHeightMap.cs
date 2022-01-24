using System;
using UnityEngine;

public class TerrainHeightMap : TerrainMap<short>
{
	public Texture2D HeightTexture;

	public Texture2D NormalTexture;

	private float normY;

	public override void Setup()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)HeightTexture != (Object)null)
		{
			if (((Texture)HeightTexture).get_width() == ((Texture)HeightTexture).get_height())
			{
				res = ((Texture)HeightTexture).get_width();
				src = (dst = new short[res * res]);
				Color32[] pixels = HeightTexture.GetPixels32();
				int i = 0;
				int num = 0;
				for (; i < res; i++)
				{
					int num2 = 0;
					while (num2 < res)
					{
						Color32 val = pixels[num];
						dst[i * res + num2] = BitUtility.DecodeShort(val);
						num2++;
						num++;
					}
				}
			}
			else
			{
				Debug.LogError((object)("Invalid height texture: " + ((Object)HeightTexture).get_name()));
			}
		}
		else
		{
			res = terrain.get_terrainData().get_heightmapResolution();
			src = (dst = new short[res * res]);
		}
		normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)res;
	}

	public void ApplyToTerrain()
	{
		float[,] heights = terrain.get_terrainData().GetHeights(0, 0, res, res);
		Parallel.For(0, res, (Action<int>)delegate(int z)
		{
			for (int i = 0; i < res; i++)
			{
				heights[z, i] = GetHeight01(i, z);
			}
		});
		terrain.get_terrainData().SetHeights(0, 0, heights);
		TerrainCollider component = ((Component)terrain).GetComponent<TerrainCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Collider)component).set_enabled(false);
			((Collider)component).set_enabled(true);
		}
	}

	public void GenerateTextures(bool heightTexture = true, bool normalTexture = true)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		if (heightTexture)
		{
			Color32[] heights = (Color32[])(object)new Color32[res * res];
			Parallel.For(0, res, (Action<int>)delegate(int z)
			{
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				for (int j = 0; j < res; j++)
				{
					heights[z * res + j] = BitUtility.EncodeShort(src[z * res + j]);
				}
			});
			HeightTexture = new Texture2D(res, res, (TextureFormat)4, true, true);
			((Object)HeightTexture).set_name("HeightTexture");
			((Texture)HeightTexture).set_wrapMode((TextureWrapMode)1);
			HeightTexture.SetPixels32(heights);
		}
		if (!normalTexture)
		{
			return;
		}
		int normalres = (res - 1) / 2;
		Color32[] normals = (Color32[])(object)new Color32[normalres * normalres];
		Parallel.For(0, normalres, (Action<int>)delegate(int z)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			float normZ = ((float)z + 0.5f) / (float)normalres;
			for (int i = 0; i < normalres; i++)
			{
				float normX = ((float)i + 0.5f) / (float)normalres;
				Vector3 normal = GetNormal(normX, normZ);
				float num = Vector3.Angle(Vector3.get_up(), normal);
				float num2 = Mathf.InverseLerp(50f, 70f, num);
				normal = Vector3.Slerp(normal, Vector3.get_up(), num2);
				normals[z * normalres + i] = Color32.op_Implicit(BitUtility.EncodeNormal(normal));
			}
		});
		NormalTexture = new Texture2D(normalres, normalres, (TextureFormat)4, false, true);
		((Object)NormalTexture).set_name("NormalTexture");
		((Texture)NormalTexture).set_wrapMode((TextureWrapMode)1);
		NormalTexture.SetPixels32(normals);
	}

	public void ApplyTextures()
	{
		HeightTexture.Apply(true, false);
		NormalTexture.Apply(true, false);
		NormalTexture.Compress(false);
		HeightTexture.Apply(false, true);
		NormalTexture.Apply(false, true);
	}

	public float GetHeight(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return TerrainMeta.Position.y + GetHeight01(worldPos) * TerrainMeta.Size.y;
	}

	public float GetHeight(float normX, float normZ)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return TerrainMeta.Position.y + GetHeight01(normX, normZ) * TerrainMeta.Size.y;
	}

	public float GetHeightFast(Vector2 uv)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		int num = res - 1;
		float num2 = uv.x * (float)num;
		float num3 = uv.y * (float)num;
		int num4 = (int)num2;
		int num5 = (int)num3;
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		num4 = ((num4 >= 0) ? num4 : 0);
		num5 = ((num5 >= 0) ? num5 : 0);
		num4 = ((num4 <= num) ? num4 : num);
		num5 = ((num5 <= num) ? num5 : num);
		int num8 = ((num2 < (float)num) ? 1 : 0);
		int num9 = ((num3 < (float)num) ? res : 0);
		int num10 = num5 * res + num4;
		int num11 = num10 + num8;
		int num12 = num10 + num9;
		int num13 = num12 + num8;
		float num14 = (float)src[num10] * 3.051944E-05f;
		float num15 = (float)src[num11] * 3.051944E-05f;
		float num16 = (float)src[num12] * 3.051944E-05f;
		float num17 = (float)src[num13] * 3.051944E-05f;
		float num18 = (num15 - num14) * num6 + num14;
		float num19 = ((num17 - num16) * num6 + num16 - num18) * num7 + num18;
		return TerrainMeta.Position.y + num19 * TerrainMeta.Size.y;
	}

	public float GetHeight(int x, int z)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return TerrainMeta.Position.y + GetHeight01(x, z) * TerrainMeta.Size.y;
	}

	public float GetHeight01(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetHeight01(normX, normZ);
	}

	public float GetHeight01(float normX, float normZ)
	{
		int num = res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float height = GetHeight01(num4, num5);
		float height2 = GetHeight01(x, num5);
		float height3 = GetHeight01(num4, z);
		float height4 = GetHeight01(x, z);
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		float num8 = Mathf.Lerp(height, height2, num6);
		float num9 = Mathf.Lerp(height3, height4, num6);
		return Mathf.Lerp(num8, num9, num7);
	}

	public float GetTriangulatedHeight01(float normX, float normZ)
	{
		int num = res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		float height = GetHeight01(num4, num5);
		float height2 = GetHeight01(x, z);
		if (num6 > num7)
		{
			float height3 = GetHeight01(x, num5);
			return height + (height3 - height) * num6 + (height2 - height3) * num7;
		}
		float height4 = GetHeight01(num4, z);
		return height + (height2 - height4) * num6 + (height4 - height) * num7;
	}

	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)src[z * res + x]);
	}

	private float GetSrcHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)src[z * res + x]);
	}

	private float GetDstHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)dst[z * res + x]);
	}

	public Vector3 GetNormal(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetNormal(normX, normZ);
	}

	public Vector3 GetNormal(float normX, float normZ)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		int num = res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		Vector3 normal = GetNormal(num4, num5);
		Vector3 normal2 = GetNormal(x, num5);
		Vector3 normal3 = GetNormal(num4, z);
		Vector3 normal4 = GetNormal(x, z);
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		Vector3 val = Vector3.Slerp(normal, normal2, num6);
		Vector3 val2 = Vector3.Slerp(normal3, normal4, num6);
		Vector3 val3 = Vector3.Slerp(val, val2, num7);
		return ((Vector3)(ref val3)).get_normalized();
	}

	public Vector3 GetNormal(int x, int z)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		int num = res - 1;
		int x2 = Mathf.Clamp(x - 1, 0, num);
		int z2 = Mathf.Clamp(z - 1, 0, num);
		int x3 = Mathf.Clamp(x + 1, 0, num);
		int z3 = Mathf.Clamp(z + 1, 0, num);
		float num2 = (GetHeight01(x3, z2) - GetHeight01(x2, z2)) * 0.5f;
		float num3 = (GetHeight01(x2, z3) - GetHeight01(x2, z2)) * 0.5f;
		Vector3 val = new Vector3(0f - num2, normY, 0f - num3);
		return ((Vector3)(ref val)).get_normalized();
	}

	private Vector3 GetNormalSobel(int x, int z)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		int num = res - 1;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(TerrainMeta.Size.x / (float)num, TerrainMeta.Size.y, TerrainMeta.Size.z / (float)num);
		int x2 = Mathf.Clamp(x - 1, 0, num);
		int z2 = Mathf.Clamp(z - 1, 0, num);
		int x3 = Mathf.Clamp(x + 1, 0, num);
		int z3 = Mathf.Clamp(z + 1, 0, num);
		float num2 = GetHeight01(x2, z2) * -1f;
		num2 += GetHeight01(x2, z) * -2f;
		num2 += GetHeight01(x2, z3) * -1f;
		num2 += GetHeight01(x3, z2) * 1f;
		num2 += GetHeight01(x3, z) * 2f;
		num2 += GetHeight01(x3, z3) * 1f;
		num2 *= val.y;
		num2 /= val.x;
		float num3 = GetHeight01(x2, z2) * -1f;
		num3 += GetHeight01(x, z2) * -2f;
		num3 += GetHeight01(x3, z2) * -1f;
		num3 += GetHeight01(x2, z3) * 1f;
		num3 += GetHeight01(x, z3) * 2f;
		num3 += GetHeight01(x3, z3) * 1f;
		num3 *= val.y;
		num3 /= val.z;
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(0f - num2, 8f, 0f - num3);
		return ((Vector3)(ref val2)).get_normalized();
	}

	public float GetSlope(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Angle(Vector3.get_up(), GetNormal(worldPos));
	}

	public float GetSlope(float normX, float normZ)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Angle(Vector3.get_up(), GetNormal(normX, normZ));
	}

	public float GetSlope(int x, int z)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Angle(Vector3.get_up(), GetNormal(x, z));
	}

	public float GetSlope01(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetSlope(worldPos) * 0.011111111f;
	}

	public float GetSlope01(float normX, float normZ)
	{
		return GetSlope(normX, normZ) * 0.011111111f;
	}

	public float GetSlope01(int x, int z)
	{
		return GetSlope(x, z) * 0.011111111f;
	}

	public void SetHeight(Vector3 worldPos, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		SetHeight(normX, normZ, height);
	}

	public void SetHeight(float normX, float normZ, float height)
	{
		int x = Index(normX);
		int z = Index(normZ);
		SetHeight(x, z, height);
	}

	public void SetHeight(int x, int z, float height)
	{
		dst[z * res + x] = BitUtility.Float2Short(height);
	}

	public void SetHeight(Vector3 worldPos, float height, float opacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		SetHeight(normX, normZ, height, opacity);
	}

	public void SetHeight(float normX, float normZ, float height, float opacity)
	{
		int x = Index(normX);
		int z = Index(normZ);
		SetHeight(x, z, height, opacity);
	}

	public void SetHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.SmoothStep(GetSrcHeight01(x, z), height, opacity);
		SetHeight(x, z, height2);
	}

	public void AddHeight(Vector3 worldPos, float delta)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		AddHeight(normX, normZ, delta);
	}

	public void AddHeight(float normX, float normZ, float delta)
	{
		int x = Index(normX);
		int z = Index(normZ);
		AddHeight(x, z, delta);
	}

	public void AddHeight(int x, int z, float delta)
	{
		float height = Mathf.Clamp01(GetDstHeight01(x, z) + delta);
		SetHeight(x, z, height);
	}

	public void LowerHeight(Vector3 worldPos, float height, float opacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		LowerHeight(normX, normZ, height, opacity);
	}

	public void LowerHeight(float normX, float normZ, float height, float opacity)
	{
		int x = Index(normX);
		int z = Index(normZ);
		LowerHeight(x, z, height, opacity);
	}

	public void LowerHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Min(GetDstHeight01(x, z), Mathf.SmoothStep(GetSrcHeight01(x, z), height, opacity));
		SetHeight(x, z, height2);
	}

	public void RaiseHeight(Vector3 worldPos, float height, float opacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		RaiseHeight(normX, normZ, height, opacity);
	}

	public void RaiseHeight(float normX, float normZ, float height, float opacity)
	{
		int x = Index(normX);
		int z = Index(normZ);
		RaiseHeight(x, z, height, opacity);
	}

	public void RaiseHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Max(GetDstHeight01(x, z), Mathf.SmoothStep(GetSrcHeight01(x, z), height, opacity));
		SetHeight(x, z, height2);
	}

	public void SetHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		SetHeight(normX, normZ, height, opacity, radius, fade);
	}

	public void SetHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				SetHeight(x, z, height, lerp * opacity);
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void LowerHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		LowerHeight(normX, normZ, height, opacity, radius, fade);
	}

	public void LowerHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				LowerHeight(x, z, height, lerp * opacity);
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void RaiseHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		RaiseHeight(normX, normZ, height, opacity, radius, fade);
	}

	public void RaiseHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				RaiseHeight(x, z, height, lerp * opacity);
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void AddHeight(Vector3 worldPos, float delta, float radius, float fade = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		AddHeight(normX, normZ, delta, radius, fade);
	}

	public void AddHeight(float normX, float normZ, float delta, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				AddHeight(x, z, lerp * delta);
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}
}
