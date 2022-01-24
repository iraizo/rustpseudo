using System;
using UnityEngine;

public static class BoundsEx
{
	private static Vector3[] pts = (Vector3[])(object)new Vector3[8];

	public static Bounds XZ3D(this Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(Vector3Ex.XZ3D(((Bounds)(ref bounds)).get_center()), Vector3Ex.XZ3D(((Bounds)(ref bounds)).get_size()));
	}

	public static Bounds Transform(this Bounds bounds, Matrix4x4 matrix)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(((Bounds)(ref bounds)).get_center());
		Vector3 extents = ((Bounds)(ref bounds)).get_extents();
		Vector3 val = ((Matrix4x4)(ref matrix)).MultiplyVector(new Vector3(extents.x, 0f, 0f));
		Vector3 val2 = ((Matrix4x4)(ref matrix)).MultiplyVector(new Vector3(0f, extents.y, 0f));
		Vector3 val3 = ((Matrix4x4)(ref matrix)).MultiplyVector(new Vector3(0f, 0f, extents.z));
		extents.x = Mathf.Abs(val.x) + Mathf.Abs(val2.x) + Mathf.Abs(val3.x);
		extents.y = Mathf.Abs(val.y) + Mathf.Abs(val2.y) + Mathf.Abs(val3.y);
		extents.z = Mathf.Abs(val.z) + Mathf.Abs(val2.z) + Mathf.Abs(val3.z);
		Bounds result = default(Bounds);
		((Bounds)(ref result)).set_center(center);
		((Bounds)(ref result)).set_extents(extents);
		return result;
	}

	public static Rect ToScreenRect(this Bounds b, Camera cam)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Bounds.ToScreenRect", 0);
		try
		{
			pts[0] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x + ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y + ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z + ((Bounds)(ref b)).get_extents().z));
			pts[1] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x + ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y + ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z - ((Bounds)(ref b)).get_extents().z));
			pts[2] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x + ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y - ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z + ((Bounds)(ref b)).get_extents().z));
			pts[3] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x + ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y - ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z - ((Bounds)(ref b)).get_extents().z));
			pts[4] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x - ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y + ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z + ((Bounds)(ref b)).get_extents().z));
			pts[5] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x - ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y + ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z - ((Bounds)(ref b)).get_extents().z));
			pts[6] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x - ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y - ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z + ((Bounds)(ref b)).get_extents().z));
			pts[7] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).get_center().x - ((Bounds)(ref b)).get_extents().x, ((Bounds)(ref b)).get_center().y - ((Bounds)(ref b)).get_extents().y, ((Bounds)(ref b)).get_center().z - ((Bounds)(ref b)).get_extents().z));
			Vector3 val2 = pts[0];
			Vector3 val3 = pts[0];
			for (int i = 1; i < pts.Length; i++)
			{
				val2 = Vector3.Min(val2, pts[i]);
				val3 = Vector3.Max(val3, pts[i]);
			}
			return Rect.MinMaxRect(val2.x, val2.y, val3.x, val3.y);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static Rect ToCanvasRect(this Bounds b, RectTransform target, Camera cam)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Rect result = b.ToScreenRect(cam);
		((Rect)(ref result)).set_min(Vector2Ex.ToCanvas(((Rect)(ref result)).get_min(), target, (Camera)null));
		((Rect)(ref result)).set_max(Vector2Ex.ToCanvas(((Rect)(ref result)).get_max(), target, (Camera)null));
		return result;
	}
}
