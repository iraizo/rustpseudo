using System;
using UnityEngine;

public static class MapImageRenderer
{
	private readonly struct Array2D<T>
	{
		private readonly T[] _items;

		private readonly int _width;

		private readonly int _height;

		public ref T this[int x, int y]
		{
			get
			{
				int num = Mathf.Clamp(x, 0, _width - 1);
				int num2 = Mathf.Clamp(y, 0, _height - 1);
				return ref _items[num2 * _width + num];
			}
		}

		public Array2D(T[] items, int width, int height)
		{
			_items = items;
			_width = width;
			_height = height;
		}
	}

	private static readonly Vector3 StartColor = new Vector3(73f / 255f, 23f / 85f, 0.24705884f);

	private static readonly Vector4 WaterColor = new Vector4(0.16941601f, 0.31755757f, 0.36200002f, 1f);

	private static readonly Vector4 GravelColor = new Vector4(0.25f, 37f / 152f, 0.22039475f, 1f);

	private static readonly Vector4 DirtColor = new Vector4(0.6f, 0.47959462f, 0.33f, 1f);

	private static readonly Vector4 SandColor = new Vector4(0.7f, 0.65968585f, 504f / 955f, 1f);

	private static readonly Vector4 GrassColor = new Vector4(0.35486364f, 0.37f, 0.2035f, 1f);

	private static readonly Vector4 ForestColor = new Vector4(0.24843751f, 0.3f, 9f / 128f, 1f);

	private static readonly Vector4 RockColor = new Vector4(0.4f, 254f / 645f, 0.37519377f, 1f);

	private static readonly Vector4 SnowColor = new Vector4(0.86274517f, 0.9294118f, 0.94117653f, 1f);

	private static readonly Vector4 PebbleColor = new Vector4(7f / 51f, 71f / 255f, 0.2761563f, 1f);

	private static readonly Vector4 OffShoreColor = new Vector4(0.04090196f, 0.22060032f, 14f / 51f, 1f);

	private static readonly Vector3 SunDirection = Vector3.Normalize(new Vector3(0.95f, 2.87f, 2.37f));

	private const float SunPower = 0.65f;

	private const float Brightness = 1.05f;

	private const float Contrast = 0.94f;

	private const float OceanWaterLevel = 0f;

	private static readonly Vector3 Half = new Vector3(0.5f, 0.5f, 0.5f);

	public static byte[] Render(out int imageWidth, out int imageHeight, out Color background, float scale = 0.5f, bool lossy = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		imageWidth = 0;
		imageHeight = 0;
		background = Color.op_Implicit(OffShoreColor);
		TerrainTexturing instance = TerrainTexturing.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			return null;
		}
		Terrain component = ((Component)instance).GetComponent<Terrain>();
		TerrainMeta component2 = ((Component)instance).GetComponent<TerrainMeta>();
		TerrainHeightMap terrainHeightMap = ((Component)instance).GetComponent<TerrainHeightMap>();
		TerrainSplatMap terrainSplatMap = ((Component)instance).GetComponent<TerrainSplatMap>();
		if ((Object)(object)component == (Object)null || (Object)(object)component2 == (Object)null || (Object)(object)terrainHeightMap == (Object)null || (Object)(object)terrainSplatMap == (Object)null)
		{
			return null;
		}
		int mapRes = (int)((float)World.Size * Mathf.Clamp(scale, 0.1f, 4f));
		float invMapRes = 1f / (float)mapRes;
		if (mapRes <= 0)
		{
			return null;
		}
		imageWidth = mapRes + 1000;
		imageHeight = mapRes + 1000;
		Color[] array = (Color[])(object)new Color[imageWidth * imageHeight];
		Array2D<Color> output = new Array2D<Color>(array, imageWidth, imageHeight);
		Parallel.For(0, imageHeight, (Action<int>)delegate(int y)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			y -= 500;
			float y2 = (float)y * invMapRes;
			int num = mapRes + 500;
			for (int i = -500; i < num; i++)
			{
				float x2 = (float)i * invMapRes;
				Vector3 startColor = StartColor;
				float height = GetHeight(x2, y2);
				float num2 = Math.Max(Vector3.Dot(GetNormal(x2, y2), SunDirection), 0f);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(GravelColor), GetSplat(x2, y2, 128) * GravelColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(PebbleColor), GetSplat(x2, y2, 64) * PebbleColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(RockColor), GetSplat(x2, y2, 8) * RockColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(DirtColor), GetSplat(x2, y2, 1) * DirtColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(GrassColor), GetSplat(x2, y2, 16) * GrassColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(ForestColor), GetSplat(x2, y2, 32) * ForestColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(SandColor), GetSplat(x2, y2, 4) * SandColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(SnowColor), GetSplat(x2, y2, 2) * SnowColor.w);
				float num3 = 0f - height;
				if (num3 > 0f)
				{
					startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(WaterColor), Mathf.Clamp(0.5f + num3 / 5f, 0f, 1f));
					startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(OffShoreColor), Mathf.Clamp(num3 / 50f, 0f, 1f));
					num2 = 0.5f;
				}
				startColor += (num2 - 0.5f) * 0.65f * startColor;
				startColor = (startColor - Half) * 0.94f + Half;
				startColor *= 1.05f;
				output[i + 500, y + 500] = new Color(startColor.x, startColor.y, startColor.z);
			}
		});
		background = output[0, 0];
		return EncodeToFile(imageWidth, imageHeight, array, lossy);
		float GetHeight(float x, float y)
		{
			return terrainHeightMap.GetHeight(x, y);
		}
		Vector3 GetNormal(float x, float y)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return terrainHeightMap.GetNormal(x, y);
		}
		float GetSplat(float x, float y, int mask)
		{
			return terrainSplatMap.GetSplat(x, y, mask);
		}
	}

	private static byte[] EncodeToFile(int width, int height, Color[] pixels, bool lossy)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		Texture2D val = null;
		try
		{
			val = new Texture2D(width, height);
			val.SetPixels(pixels);
			val.Apply();
			return lossy ? ImageConversion.EncodeToJPG(val, 85) : ImageConversion.EncodeToPNG(val);
		}
		finally
		{
			if ((Object)(object)val != (Object)null)
			{
				Object.Destroy((Object)(object)val);
			}
		}
	}

	private static Vector3 UnpackNormal(Vector4 value)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		value.x *= value.w;
		Vector3 val = default(Vector3);
		val.x = value.x * 2f - 1f;
		val.y = value.y * 2f - 1f;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(val.x, val.y);
		val.z = Mathf.Sqrt(1f - Mathf.Clamp(Vector2.Dot(val2, val2), 0f, 1f));
		return val;
	}
}
