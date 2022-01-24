using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class WaterDynamics : MonoBehaviour
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageDesc
	{
		public int width;

		public int height;

		public int maxWidth;

		public int maxHeight;

		public int widthShift;

		public ImageDesc(Texture2D tex)
		{
			width = ((Texture)tex).get_width();
			height = ((Texture)tex).get_height();
			maxWidth = ((Texture)tex).get_width() - 1;
			maxHeight = ((Texture)tex).get_height() - 1;
			widthShift = (int)Mathf.Log((float)((Texture)tex).get_width(), 2f);
		}

		public void Clear()
		{
			width = 0;
			height = 0;
			maxWidth = 0;
			maxHeight = 0;
			widthShift = 0;
		}
	}

	public class Image
	{
		public ImageDesc desc;

		public byte[] pixels;

		public Texture2D texture { get; private set; }

		public Image(Texture2D tex)
		{
			desc = new ImageDesc(tex);
			texture = tex;
			pixels = GetDisplacementPixelsFromTexture(tex);
		}

		public void Destroy()
		{
			desc.Clear();
			texture = null;
			pixels = null;
		}

		private byte[] GetDisplacementPixelsFromTexture(Texture2D tex)
		{
			Color32[] pixels = tex.GetPixels32();
			byte[] array = new byte[pixels.Length];
			for (int i = 0; i < pixels.Length; i++)
			{
				array[i] = pixels[i].b;
			}
			return array;
		}
	}

	private struct Point2D
	{
		public int x;

		public int y;

		public Point2D(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Point2D(float x, float y)
		{
			this.x = (int)x;
			this.y = (int)y;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TargetDesc
	{
		public int size;

		public int maxSize;

		public int maxSizeSubStep;

		public Vector2 areaOffset;

		public Vector2 areaToMapUV;

		public Vector2 areaToMapXY;

		public int tileSize;

		public int tileSizeShift;

		public int tileCount;

		public int tileMaxCount;

		public TargetDesc(Vector3 areaPosition, Vector3 areaSize)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			size = 512;
			maxSize = size - 1;
			maxSizeSubStep = maxSize * 256;
			areaOffset = new Vector2(areaPosition.x, areaPosition.z);
			areaToMapUV = new Vector2(1f / areaSize.x, 1f / areaSize.z);
			areaToMapXY = areaToMapUV * (float)size;
			tileSize = Mathf.NextPowerOfTwo(Mathf.Max(size, 4096)) / 256;
			tileSizeShift = (int)Mathf.Log((float)tileSize, 2f);
			tileCount = Mathf.CeilToInt((float)size / (float)tileSize);
			tileMaxCount = tileCount - 1;
		}

		public void Clear()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			areaOffset = Vector2.get_zero();
			areaToMapUV = Vector2.get_zero();
			areaToMapXY = Vector2.get_zero();
			size = 0;
			maxSize = 0;
			maxSizeSubStep = 0;
			tileSize = 0;
			tileSizeShift = 0;
			tileCount = 0;
			tileMaxCount = 0;
		}

		public ushort TileOffsetToXYOffset(ushort tileOffset, out int x, out int y, out int offset)
		{
			int num = (int)tileOffset % tileCount;
			int num2 = (int)tileOffset / tileCount;
			x = num * tileSize;
			y = num2 * tileSize;
			offset = y * size + x;
			return tileOffset;
		}

		public ushort TileOffsetToTileXYIndex(ushort tileOffset, out int tileX, out int tileY, out ushort tileIndex)
		{
			tileX = (int)tileOffset % tileCount;
			tileY = (int)tileOffset / tileCount;
			tileIndex = (ushort)(tileY * tileCount + tileX);
			return tileOffset;
		}

		public Vector2 WorldToRaster(Vector2 pos)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 result = default(Vector2);
			result.x = (pos.x - areaOffset.x) * areaToMapXY.x;
			result.y = (pos.y - areaOffset.y) * areaToMapXY.y;
			return result;
		}

		public Vector3 WorldToRaster(Vector3 pos)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			val.x = (pos.x - areaOffset.x) * areaToMapXY.x;
			val.y = (pos.z - areaOffset.y) * areaToMapXY.y;
			return Vector2.op_Implicit(val);
		}
	}

	public class Target
	{
		public WaterDynamics owner;

		public TargetDesc desc;

		private byte[] pixels;

		private byte[] clearTileTable;

		private SimpleList<ushort> clearTileList;

		private byte[] drawTileTable;

		private SimpleList<ushort> drawTileList;

		private const int MaxInteractionOffset = 100;

		private Vector3 prevCameraWorldPos;

		private Vector2i interactionOffset;

		public TargetDesc Desc => desc;

		public byte[] Pixels => pixels;

		public byte[] DrawTileTable => drawTileTable;

		public SimpleList<ushort> DrawTileList => drawTileList;

		public Target(WaterDynamics owner, Vector3 areaPosition, Vector3 areaSize)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			this.owner = owner;
			desc = new TargetDesc(areaPosition, areaSize);
		}

		public void Destroy()
		{
			desc.Clear();
		}

		private Texture2D CreateDynamicTexture(int size)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			Texture2D val = new Texture2D(size, size, (TextureFormat)5, false, true);
			((Texture)val).set_filterMode((FilterMode)1);
			((Texture)val).set_wrapMode((TextureWrapMode)1);
			return val;
		}

		private RenderTexture CreateRenderTexture(int size)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			RenderTextureFormat val = (RenderTextureFormat)(SystemInfoEx.SupportsRenderTextureFormat((RenderTextureFormat)15) ? 15 : 14);
			RenderTexture val2 = new RenderTexture(size, size, 0, val, (RenderTextureReadWrite)1);
			((Texture)val2).set_filterMode((FilterMode)1);
			((Texture)val2).set_wrapMode((TextureWrapMode)1);
			val2.Create();
			return val2;
		}

		public void ClearTiles()
		{
			for (int i = 0; i < clearTileList.Count; i++)
			{
				desc.TileOffsetToXYOffset(clearTileList[i], out var x, out var y, out var offset);
				int num = Mathf.Min(x + desc.tileSize, desc.size) - x;
				int num2 = Mathf.Min(y + desc.tileSize, desc.size) - y;
				if (owner.useNativePath)
				{
					RasterClearTile_Native(ref pixels[0], offset, desc.size, num, num2);
					continue;
				}
				for (int j = 0; j < num2; j++)
				{
					Array.Clear(pixels, offset, num);
					offset += desc.size;
				}
			}
		}

		public void ProcessTiles()
		{
			int tileX;
			int tileY;
			ushort tileIndex;
			for (int i = 0; i < clearTileList.Count; i++)
			{
				ushort num = desc.TileOffsetToTileXYIndex(clearTileList[i], out tileX, out tileY, out tileIndex);
				clearTileTable[num] = 0;
				clearTileList[i] = ushort.MaxValue;
			}
			clearTileList.Clear();
			for (int j = 0; j < drawTileList.Count; j++)
			{
				ushort num2 = desc.TileOffsetToTileXYIndex(drawTileList[j], out tileX, out tileY, out tileIndex);
				if (clearTileTable[tileIndex] == 0)
				{
					clearTileTable[tileIndex] = 1;
					clearTileList.Add(tileIndex);
				}
				drawTileTable[num2] = 0;
				drawTileList[j] = ushort.MaxValue;
			}
			drawTileList.Clear();
		}

		public void UpdateTiles()
		{
		}

		public void Prepare()
		{
		}

		public void Update()
		{
		}

		public void UpdateGlobalShaderProperties()
		{
		}
	}

	private const int maxRasterSize = 1024;

	private const int subStep = 256;

	private const int subShift = 8;

	private const int subMask = 255;

	private const float oneOverSubStep = 0.00390625f;

	private const float interp_subStep = 65536f;

	private const int interp_subShift = 16;

	private const int interp_subFracMask = 65535;

	private ImageDesc imageDesc;

	private byte[] imagePixels;

	private TargetDesc targetDesc;

	private byte[] targetPixels;

	private byte[] targetDrawTileTable;

	private SimpleList<ushort> targetDrawTileList;

	public bool ShowDebug;

	public bool ForceFallback;

	private Target target;

	private bool useNativePath;

	private static HashSet<WaterInteraction> interactions = new HashSet<WaterInteraction>();

	public bool IsInitialized { get; private set; }

	private void RasterBindImage(Image image)
	{
		imageDesc = image.desc;
		imagePixels = image.pixels;
	}

	private void RasterBindTarget(Target target)
	{
		targetDesc = target.Desc;
		targetPixels = target.Pixels;
		targetDrawTileTable = target.DrawTileTable;
		targetDrawTileList = target.DrawTileList;
	}

	private void RasterInteraction(Vector2 pos, Vector2 scale, float rotation, float disp, float dist)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = targetDesc.WorldToRaster(pos);
		float num = (0f - rotation) * ((float)Math.PI / 180f);
		float s = Mathf.Sin(num);
		float c = Mathf.Cos(num);
		float num2 = Mathf.Min((float)imageDesc.width * scale.x, 1024f) * 0.5f;
		float num3 = Mathf.Min((float)imageDesc.height * scale.y, 1024f) * 0.5f;
		Vector2 val2 = val + Rotate2D(new Vector2(0f - num2, 0f - num3), s, c);
		Vector2 val3 = val + Rotate2D(new Vector2(num2, 0f - num3), s, c);
		Vector2 val4 = val + Rotate2D(new Vector2(num2, num3), s, c);
		Vector2 val5 = val + Rotate2D(new Vector2(0f - num2, num3), s, c);
		Point2D p = new Point2D(val2.x * 256f, val2.y * 256f);
		Point2D p2 = new Point2D(val3.x * 256f, val3.y * 256f);
		Point2D point2D = new Point2D(val4.x * 256f, val4.y * 256f);
		Point2D p3 = new Point2D(val5.x * 256f, val5.y * 256f);
		Vector2 uv = default(Vector2);
		((Vector2)(ref uv))._002Ector(-0.5f, -0.5f);
		Vector2 uv2 = default(Vector2);
		((Vector2)(ref uv2))._002Ector((float)imageDesc.width - 0.5f, -0.5f);
		Vector2 val6 = default(Vector2);
		((Vector2)(ref val6))._002Ector((float)imageDesc.width - 0.5f, (float)imageDesc.height - 0.5f);
		Vector2 uv3 = default(Vector2);
		((Vector2)(ref uv3))._002Ector(-0.5f, (float)imageDesc.height - 0.5f);
		byte disp2 = (byte)(disp * 255f);
		byte dist2 = (byte)(dist * 255f);
		RasterizeTriangle(p, p2, point2D, uv, uv2, val6, disp2, dist2);
		RasterizeTriangle(p, point2D, p3, uv, val6, uv3, disp2, dist2);
	}

	private float Frac(float x)
	{
		return x - (float)(int)x;
	}

	private Vector2 Rotate2D(Vector2 v, float s, float c)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Vector2 result = default(Vector2);
		result.x = v.x * c - v.y * s;
		result.y = v.y * c + v.x * s;
		return result;
	}

	private int Min3(int a, int b, int c)
	{
		return Mathf.Min(a, Mathf.Min(b, c));
	}

	private int Max3(int a, int b, int c)
	{
		return Mathf.Max(a, Mathf.Max(b, c));
	}

	private int EdgeFunction(Point2D a, Point2D b, Point2D c)
	{
		return (int)(((long)(b.x - a.x) * (long)(c.y - a.y) >> 8) - ((long)(b.y - a.y) * (long)(c.x - a.x) >> 8));
	}

	private bool IsTopLeft(Point2D a, Point2D b)
	{
		if (a.y != b.y || a.x >= b.x)
		{
			return a.y > b.y;
		}
		return true;
	}

	private void RasterizeTriangle(Point2D p0, Point2D p1, Point2D p2, Vector2 uv0, Vector2 uv1, Vector2 uv2, byte disp, byte dist)
	{
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		int width = imageDesc.width;
		int widthShift = imageDesc.widthShift;
		int maxWidth = imageDesc.maxWidth;
		int maxHeight = imageDesc.maxHeight;
		int size = targetDesc.size;
		int tileCount = targetDesc.tileCount;
		int num = Mathf.Max(Min3(p0.x, p1.x, p2.x), 0);
		int num2 = Mathf.Max(Min3(p0.y, p1.y, p2.y), 0);
		int num3 = Mathf.Min(Max3(p0.x, p1.x, p2.x), targetDesc.maxSizeSubStep);
		int num4 = Mathf.Min(Max3(p0.y, p1.y, p2.y), targetDesc.maxSizeSubStep);
		int num5 = Mathf.Max(num >> 8 >> targetDesc.tileSizeShift, 0);
		int num6 = Mathf.Min(num3 >> 8 >> targetDesc.tileSizeShift, targetDesc.tileMaxCount);
		int num7 = Mathf.Max(num2 >> 8 >> targetDesc.tileSizeShift, 0);
		int num8 = Mathf.Min(num4 >> 8 >> targetDesc.tileSizeShift, targetDesc.tileMaxCount);
		for (int i = num7; i <= num8; i++)
		{
			int num9 = i * tileCount;
			for (int j = num5; j <= num6; j++)
			{
				int num10 = num9 + j;
				if (targetDrawTileTable[num10] == 0)
				{
					targetDrawTileTable[num10] = 1;
					targetDrawTileList.Add((ushort)num10);
				}
			}
		}
		num = (num + 255) & -256;
		num2 = (num2 + 255) & -256;
		int num11 = ((!IsTopLeft(p1, p2)) ? (-1) : 0);
		int num12 = ((!IsTopLeft(p2, p0)) ? (-1) : 0);
		int num13 = ((!IsTopLeft(p0, p1)) ? (-1) : 0);
		Point2D c = new Point2D(num, num2);
		int num14 = EdgeFunction(p1, p2, c) + num11;
		int num15 = EdgeFunction(p2, p0, c) + num12;
		int num16 = EdgeFunction(p0, p1, c) + num13;
		int num17 = p1.y - p2.y;
		int num18 = p2.y - p0.y;
		int num19 = p0.y - p1.y;
		int num20 = p2.x - p1.x;
		int num21 = p0.x - p2.x;
		int num22 = p1.x - p0.x;
		float num23 = 16777216f / (float)EdgeFunction(p0, p1, p2);
		float num24 = uv0.x * 65536f;
		float num25 = uv0.y * 65536f;
		float num26 = (uv1.x - uv0.x) * num23;
		float num27 = (uv1.y - uv0.y) * num23;
		float num28 = (uv2.x - uv0.x) * num23;
		float num29 = (uv2.y - uv0.y) * num23;
		int num30 = (int)((float)num18 * 0.00390625f * num26 + (float)num19 * 0.00390625f * num28);
		int num31 = (int)((float)num18 * 0.00390625f * num27 + (float)num19 * 0.00390625f * num29);
		for (int k = num2; k <= num4; k += 256)
		{
			int num32 = num14;
			int num33 = num15;
			int num34 = num16;
			int num35 = (int)(num24 + num26 * 0.00390625f * (float)num33 + num28 * 0.00390625f * (float)num34);
			int num36 = (int)(num25 + num27 * 0.00390625f * (float)num33 + num29 * 0.00390625f * (float)num34);
			for (int l = num; l <= num3; l += 256)
			{
				if ((num32 | num33 | num34) >= 0)
				{
					int num37 = ((num35 > 0) ? num35 : 0);
					int num38 = ((num36 > 0) ? num36 : 0);
					int num39 = num37 >> 16;
					int num40 = num38 >> 16;
					byte b = (byte)((num37 & 0xFFFF) >> 8);
					byte b2 = (byte)((num38 & 0xFFFF) >> 8);
					num39 = ((num39 > 0) ? num39 : 0);
					num40 = ((num40 > 0) ? num40 : 0);
					num39 = ((num39 < maxWidth) ? num39 : maxWidth);
					num40 = ((num40 < maxHeight) ? num40 : maxHeight);
					int num41 = ((num39 < maxWidth) ? 1 : 0);
					int num42 = ((num40 < maxHeight) ? width : 0);
					int num43 = (num40 << widthShift) + num39;
					int num44 = num43 + num41;
					int num45 = num43 + num42;
					int num46 = num45 + num41;
					byte b3 = imagePixels[num43];
					byte b4 = imagePixels[num44];
					byte b5 = imagePixels[num45];
					byte b6 = imagePixels[num46];
					int num47 = b3 + (b * (b4 - b3) >> 8);
					int num48 = b5 + (b * (b6 - b5) >> 8);
					int num49 = num47 + (b2 * (num48 - num47) >> 8);
					num49 = num49 * disp >> 8;
					int num50 = (k >> 8) * size + (l >> 8);
					num49 = targetPixels[num50] + num49;
					num49 = ((num49 < 255) ? num49 : 255);
					targetPixels[num50] = (byte)num49;
				}
				num32 += num17;
				num33 += num18;
				num34 += num19;
				num35 += num30;
				num36 += num31;
			}
			num14 += num20;
			num15 += num21;
			num16 += num22;
		}
	}

	[DllImport("RustNative", EntryPoint = "Water_RasterClearTile")]
	private static extern void RasterClearTile_Native(ref byte pixels, int offset, int stride, int width, int height);

	[DllImport("RustNative", EntryPoint = "Water_RasterBindImage")]
	private static extern void RasterBindImage_Native(ref ImageDesc desc, ref byte pixels);

	[DllImport("RustNative", EntryPoint = "Water_RasterBindTarget")]
	private static extern void RasterBindTarget_Native(ref TargetDesc desc, ref byte pixels, ref byte drawTileTable, ref ushort drawTileList, ref int drawTileCount);

	[DllImport("RustNative", EntryPoint = "Water_RasterInteraction")]
	private static extern void RasterInteraction_Native(Vector2 pos, Vector2 scale, float rotation, float disp, float dist);

	public static void SafeDestroy<T>(ref T obj) where T : Object
	{
		if ((Object)(object)obj != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)obj);
			obj = default(T);
		}
	}

	public static T SafeDestroy<T>(T obj) where T : Object
	{
		if ((Object)(object)obj != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)obj);
		}
		return default(T);
	}

	public static void SafeRelease<T>(ref T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = null;
		}
	}

	public static T SafeRelease<T>(T obj) where T : class, IDisposable
	{
		obj?.Dispose();
		return null;
	}

	public static void RegisterInteraction(WaterInteraction interaction)
	{
		interactions.Add(interaction);
	}

	public static void UnregisterInteraction(WaterInteraction interaction)
	{
		interactions.Remove(interaction);
	}

	private bool SupportsNativePath()
	{
		bool result = true;
		try
		{
			ImageDesc desc = default(ImageDesc);
			byte[] array = new byte[1];
			RasterBindImage_Native(ref desc, ref array[0]);
			return result;
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log((object)"[WaterDynamics] Fast native path not available. Reverting to managed fallback.");
			return false;
		}
	}

	public void Initialize(Vector3 areaPosition, Vector3 areaSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		target = new Target(this, areaPosition, areaSize);
		useNativePath = SupportsNativePath();
		IsInitialized = true;
	}

	public bool TryInitialize()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInitialized && (Object)(object)TerrainMeta.Data != (Object)null)
		{
			Initialize(TerrainMeta.Position, TerrainMeta.Data.get_size());
			return true;
		}
		return false;
	}

	public void Shutdown()
	{
		if (target != null)
		{
			target.Destroy();
			target = null;
		}
		IsInitialized = false;
	}

	public void OnEnable()
	{
		TryInitialize();
	}

	public void OnDisable()
	{
		Shutdown();
	}

	public void Update()
	{
		if (!((Object)(object)WaterSystem.Instance == (Object)null) && !IsInitialized)
		{
			TryInitialize();
		}
	}

	private void ProcessInteractions()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<WaterInteraction> enumerator = interactions.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				WaterInteraction current = enumerator.get_Current();
				if (!((Object)(object)current == (Object)null))
				{
					current.UpdateTransform();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public float SampleHeight(Vector3 pos)
	{
		return 0f;
	}

	public WaterDynamics()
		: this()
	{
	}
}
