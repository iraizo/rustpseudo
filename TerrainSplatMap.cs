using System;
using UnityEngine;

public class TerrainSplatMap : TerrainMap<byte>
{
	public Texture2D SplatTexture0;

	public Texture2D SplatTexture1;

	internal int num;

	public override void Setup()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		res = terrain.get_terrainData().get_alphamapResolution();
		this.num = config.Splats.Length;
		src = (dst = new byte[this.num * res * res]);
		if ((Object)(object)SplatTexture0 != (Object)null)
		{
			if (((Texture)SplatTexture0).get_width() == ((Texture)SplatTexture0).get_height() && ((Texture)SplatTexture0).get_width() == res)
			{
				Color32[] pixels = SplatTexture0.GetPixels32();
				int i = 0;
				int num = 0;
				for (; i < res; i++)
				{
					int num2 = 0;
					while (num2 < res)
					{
						Color32 val = pixels[num];
						if (this.num > 0)
						{
							byte[] array = dst;
							_ = res;
							array[(0 + i) * res + num2] = val.r;
						}
						if (this.num > 1)
						{
							dst[(res + i) * res + num2] = val.g;
						}
						if (this.num > 2)
						{
							dst[(2 * res + i) * res + num2] = val.b;
						}
						if (this.num > 3)
						{
							dst[(3 * res + i) * res + num2] = val.a;
						}
						num2++;
						num++;
					}
				}
			}
			else
			{
				Debug.LogError((object)("Invalid splat texture: " + ((Object)SplatTexture0).get_name()), (Object)(object)SplatTexture0);
			}
		}
		if (!((Object)(object)SplatTexture1 != (Object)null))
		{
			return;
		}
		if (((Texture)SplatTexture1).get_width() == ((Texture)SplatTexture1).get_height() && ((Texture)SplatTexture1).get_width() == res && this.num > 5)
		{
			Color32[] pixels2 = SplatTexture1.GetPixels32();
			int j = 0;
			int num3 = 0;
			for (; j < res; j++)
			{
				int num4 = 0;
				while (num4 < res)
				{
					Color32 val2 = pixels2[num3];
					if (this.num > 4)
					{
						dst[(4 * res + j) * res + num4] = val2.r;
					}
					if (this.num > 5)
					{
						dst[(5 * res + j) * res + num4] = val2.g;
					}
					if (this.num > 6)
					{
						dst[(6 * res + j) * res + num4] = val2.b;
					}
					if (this.num > 7)
					{
						dst[(7 * res + j) * res + num4] = val2.a;
					}
					num4++;
					num3++;
				}
			}
		}
		else
		{
			Debug.LogError((object)("Invalid splat texture: " + ((Object)SplatTexture1).get_name()), (Object)(object)SplatTexture1);
		}
	}

	public void GenerateTextures()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		SplatTexture0 = new Texture2D(res, res, (TextureFormat)4, false, true);
		((Object)SplatTexture0).set_name("SplatTexture0");
		((Texture)SplatTexture0).set_wrapMode((TextureWrapMode)1);
		Color32[] cols = (Color32[])(object)new Color32[res * res];
		Parallel.For(0, res, (Action<int>)delegate(int z)
		{
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			for (int j = 0; j < res; j++)
			{
				int num;
				if (this.num <= 0)
				{
					num = 0;
				}
				else
				{
					byte[] array = src;
					_ = res;
					num = array[(0 + z) * res + j];
				}
				byte b5 = (byte)num;
				byte b6 = (byte)((this.num > 1) ? src[(res + z) * res + j] : 0);
				byte b7 = (byte)((this.num > 2) ? src[(2 * res + z) * res + j] : 0);
				byte b8 = (byte)((this.num > 3) ? src[(3 * res + z) * res + j] : 0);
				cols[z * res + j] = new Color32(b5, b6, b7, b8);
			}
		});
		SplatTexture0.SetPixels32(cols);
		SplatTexture1 = new Texture2D(res, res, (TextureFormat)4, false, true);
		((Object)SplatTexture1).set_name("SplatTexture1");
		((Texture)SplatTexture1).set_wrapMode((TextureWrapMode)1);
		Color32[] cols2 = (Color32[])(object)new Color32[res * res];
		Parallel.For(0, res, (Action<int>)delegate(int z)
		{
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < res; i++)
			{
				byte b = (byte)((num > 4) ? src[(4 * res + z) * res + i] : 0);
				byte b2 = (byte)((num > 5) ? src[(5 * res + z) * res + i] : 0);
				byte b3 = (byte)((num > 6) ? src[(6 * res + z) * res + i] : 0);
				byte b4 = (byte)((num > 7) ? src[(7 * res + z) * res + i] : 0);
				cols2[z * res + i] = new Color32(b, b2, b3, b4);
			}
		});
		SplatTexture1.SetPixels32(cols2);
	}

	public void ApplyTextures()
	{
		SplatTexture0.Apply(true, true);
		SplatTexture1.Apply(true, true);
	}

	public float GetSplatMax(Vector3 worldPos, int mask = -1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetSplatMax(normX, normZ, mask);
	}

	public float GetSplatMax(float normX, float normZ, int mask = -1)
	{
		int x = Index(normX);
		int z = Index(normZ);
		return GetSplatMax(x, z, mask);
	}

	public float GetSplatMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				byte b2 = src[(i * res + z) * res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return BitUtility.Byte2Float((int)b);
	}

	public int GetSplatMaxIndex(Vector3 worldPos, int mask = -1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetSplatMaxIndex(normX, normZ, mask);
	}

	public int GetSplatMaxIndex(float normX, float normZ, int mask = -1)
	{
		int x = Index(normX);
		int z = Index(normZ);
		return GetSplatMaxIndex(x, z, mask);
	}

	public int GetSplatMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int result = 0;
		for (int i = 0; i < num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				byte b2 = src[(i * res + z) * res + x];
				if (b2 >= b)
				{
					b = b2;
					result = i;
				}
			}
		}
		return result;
	}

	public int GetSplatMaxType(Vector3 worldPos, int mask = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TerrainSplat.IndexToType(GetSplatMaxIndex(worldPos, mask));
	}

	public int GetSplatMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainSplat.IndexToType(GetSplatMaxIndex(normX, normZ, mask));
	}

	public int GetSplatMaxType(int x, int z, int mask = -1)
	{
		return TerrainSplat.IndexToType(GetSplatMaxIndex(x, z, mask));
	}

	public float GetSplat(Vector3 worldPos, int mask)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetSplat(normX, normZ, mask);
	}

	public float GetSplat(float normX, float normZ, int mask)
	{
		int num = res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float num6 = Mathf.Lerp(GetSplat(num4, num5, mask), GetSplat(x, num5, mask), num2 - (float)num4);
		float num7 = Mathf.Lerp(GetSplat(num4, z, mask), GetSplat(x, z, mask), num2 - (float)num4);
		return Mathf.Lerp(num6, num7, num3 - (float)num5);
	}

	public float GetSplat(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)src[(TerrainSplat.TypeToIndex(mask) * res + z) * res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				num += src[(i * res + z) * res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	public void SetSplat(Vector3 worldPos, int id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		SetSplat(normX, normZ, id);
	}

	public void SetSplat(float normX, float normZ, int id)
	{
		int x = Index(normX);
		int z = Index(normZ);
		SetSplat(x, z, id);
	}

	public void SetSplat(int x, int z, int id)
	{
		int num = TerrainSplat.TypeToIndex(id);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				dst[(i * res + z) * res + x] = byte.MaxValue;
			}
			else
			{
				dst[(i * res + z) * res + x] = 0;
			}
		}
	}

	public void SetSplat(Vector3 worldPos, int id, float v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		SetSplat(normX, normZ, id, v);
	}

	public void SetSplat(float normX, float normZ, int id, float v)
	{
		int x = Index(normX);
		int z = Index(normZ);
		SetSplat(x, z, id, v);
	}

	public void SetSplat(int x, int z, int id, float v)
	{
		SetSplat(x, z, id, GetSplat(x, z, id), v);
	}

	public void SetSplatRaw(int x, int z, Vector4 v1, Vector4 v2, float opacity)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v1.x + v1.y + v1.z + v1.w + v2.x + v2.y + v2.z + v2.w);
		if (num != 0f)
		{
			float num2 = 1f - opacity * num;
			if (num2 == 0f && opacity == 1f)
			{
				byte[] array = dst;
				_ = res;
				array[(0 + z) * res + x] = BitUtility.Float2Byte(v1.x);
				dst[(res + z) * res + x] = BitUtility.Float2Byte(v1.y);
				dst[(2 * res + z) * res + x] = BitUtility.Float2Byte(v1.z);
				dst[(3 * res + z) * res + x] = BitUtility.Float2Byte(v1.w);
				dst[(4 * res + z) * res + x] = BitUtility.Float2Byte(v2.x);
				dst[(5 * res + z) * res + x] = BitUtility.Float2Byte(v2.y);
				dst[(6 * res + z) * res + x] = BitUtility.Float2Byte(v2.z);
				dst[(7 * res + z) * res + x] = BitUtility.Float2Byte(v2.w);
			}
			else
			{
				byte[] array2 = dst;
				_ = res;
				int num3 = (0 + z) * res + x;
				byte[] array3 = src;
				_ = res;
				array2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)array3[(0 + z) * res + x]) * num2 + v1.x * opacity);
				dst[(res + z) * res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)src[(res + z) * res + x]) * num2 + v1.y * opacity);
				dst[(2 * res + z) * res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)src[(2 * res + z) * res + x]) * num2 + v1.z * opacity);
				dst[(3 * res + z) * res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)src[(3 * res + z) * res + x]) * num2 + v1.w * opacity);
				dst[(4 * res + z) * res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)src[(4 * res + z) * res + x]) * num2 + v2.x * opacity);
				dst[(5 * res + z) * res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)src[(5 * res + z) * res + x]) * num2 + v2.y * opacity);
				dst[(6 * res + z) * res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)src[(6 * res + z) * res + x]) * num2 + v2.z * opacity);
				dst[(7 * res + z) * res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)src[(7 * res + z) * res + x]) * num2 + v2.w * opacity);
			}
		}
	}

	public void SetSplat(Vector3 worldPos, int id, float opacity, float radius, float fade = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		SetSplat(normX, normZ, id, opacity, radius, fade);
	}

	public void SetSplat(float normX, float normZ, int id, float opacity, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = BitUtility.Byte2Float((int)dst[(idx * res + z) * res + x]);
				float new_val = Mathf.Lerp(num, 1f, lerp * opacity);
				SetSplat(x, z, id, num, new_val);
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void AddSplat(Vector3 worldPos, int id, float delta, float radius, float fade = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		AddSplat(normX, normZ, id, delta, radius, fade);
	}

	public void AddSplat(float normX, float normZ, int id, float delta, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = BitUtility.Byte2Float((int)dst[(idx * res + z) * res + x]);
				float new_val = Mathf.Clamp01(num + lerp * delta);
				SetSplat(x, z, id, num, new_val);
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}

	private void SetSplat(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainSplat.TypeToIndex(id);
		if (old_val >= 1f)
		{
			return;
		}
		float num2 = (1f - new_val) / (1f - old_val);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				dst[(i * res + z) * res + x] = BitUtility.Float2Byte(new_val);
			}
			else
			{
				dst[(i * res + z) * res + x] = BitUtility.Float2Byte(num2 * BitUtility.Byte2Float((int)dst[(i * res + z) * res + x]));
			}
		}
	}
}
