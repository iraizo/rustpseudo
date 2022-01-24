using System;
using UnityEngine;

public class TerrainWaterMap : TerrainMap<short>
{
	public Texture2D WaterTexture;

	private float normY;

	public override void Setup()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)WaterTexture != (Object)null)
		{
			if (((Texture)WaterTexture).get_width() == ((Texture)WaterTexture).get_height())
			{
				res = ((Texture)WaterTexture).get_width();
				src = (dst = new short[res * res]);
				Color32[] pixels = WaterTexture.GetPixels32();
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
				Debug.LogError((object)("Invalid water texture: " + ((Object)WaterTexture).get_name()));
			}
		}
		else
		{
			res = terrain.get_terrainData().get_heightmapResolution();
			src = (dst = new short[res * res]);
		}
		normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)res;
	}

	public void GenerateTextures()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		Color32[] heights = (Color32[])(object)new Color32[res * res];
		Parallel.For(0, res, (Action<int>)delegate(int z)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < res; i++)
			{
				heights[z * res + i] = BitUtility.EncodeShort(src[z * res + i]);
			}
		});
		WaterTexture = new Texture2D(res, res, (TextureFormat)4, true, true);
		((Object)WaterTexture).set_name("WaterTexture");
		((Texture)WaterTexture).set_wrapMode((TextureWrapMode)1);
		WaterTexture.SetPixels32(heights);
	}

	public void ApplyTextures()
	{
		WaterTexture.Apply(true, true);
	}

	public float GetHeight(Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return Math.Max(TerrainMeta.Position.y + GetHeight01(worldPos) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	public float GetHeight(float normX, float normZ)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Math.Max(TerrainMeta.Position.y + GetHeight01(normX, normZ) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
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
		return Math.Max(TerrainMeta.Position.y + num19 * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	public float GetHeight(int x, int z)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Math.Max(TerrainMeta.Position.y + GetHeight01(x, z) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
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
		float num6 = Mathf.Lerp(GetHeight01(num4, num5), GetHeight01(x, num5), num2 - (float)num4);
		float num7 = Mathf.Lerp(GetHeight01(num4, z), GetHeight01(x, z), num2 - (float)num4);
		return Mathf.Lerp(num6, num7, num3 - (float)num5);
	}

	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)src[z * res + x]);
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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		int num = res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float num6 = GetHeight01(x, num5) - GetHeight01(num4, num5);
		float num7 = GetHeight01(num4, z) - GetHeight01(num4, num5);
		Vector3 val = new Vector3(0f - num6, normY, 0f - num7);
		return ((Vector3)(ref val)).get_normalized();
	}

	public Vector3 GetNormalFast(Vector2 uv)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		int num = res - 1;
		int num2 = (int)(uv.x * (float)num);
		int num3 = (int)(uv.y * (float)num);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		int num4 = ((num2 < num) ? 1 : 0);
		int num5 = ((num3 < num) ? res : 0);
		int num6 = num3 * res + num2;
		int num7 = num6 + num4;
		int num8 = num6 + num5;
		short num9 = src[num6];
		short num10 = src[num7];
		short num11 = src[num8];
		float num12 = (float)(num10 - num9) * 3.051944E-05f;
		float num13 = (float)(num11 - num9) * 3.051944E-05f;
		return new Vector3(0f - num12, normY, 0f - num13);
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

	public float GetDepth(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetHeight(worldPos) - TerrainMeta.HeightMap.GetHeight(worldPos);
	}

	public float GetDepth(float normX, float normZ)
	{
		return GetHeight(normX, normZ) - TerrainMeta.HeightMap.GetHeight(normX, normZ);
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
}
