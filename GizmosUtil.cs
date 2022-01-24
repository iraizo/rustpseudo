using System;
using UnityEngine;

public static class GizmosUtil
{
	public static void DrawWireCircleX(Vector3 pos, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Gizmos.get_matrix() * Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(0f, 1f, 1f)));
		Gizmos.DrawWireSphere(Vector3.get_zero(), radius);
		Gizmos.set_matrix(matrix);
	}

	public static void DrawWireCircleY(Vector3 pos, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Gizmos.get_matrix() * Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 0f, 1f)));
		Gizmos.DrawWireSphere(Vector3.get_zero(), radius);
		Gizmos.set_matrix(matrix);
	}

	public static void DrawWireCircleZ(Vector3 pos, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Gizmos.get_matrix() * Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 1f, 0f)));
		Gizmos.DrawWireSphere(Vector3.get_zero(), radius);
		Gizmos.set_matrix(matrix);
	}

	public static void DrawCircleX(Vector3 pos, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Gizmos.get_matrix() * Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(0f, 1f, 1f)));
		Gizmos.DrawSphere(Vector3.get_zero(), radius);
		Gizmos.set_matrix(matrix);
	}

	public static void DrawCircleY(Vector3 pos, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Gizmos.get_matrix() * Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 0f, 1f)));
		Gizmos.DrawSphere(Vector3.get_zero(), radius);
		Gizmos.set_matrix(matrix);
	}

	public static void DrawCircleZ(Vector3 pos, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Gizmos.get_matrix() * Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 1f, 0f)));
		Gizmos.DrawSphere(Vector3.get_zero(), radius);
		Gizmos.set_matrix(matrix);
	}

	public static void DrawWireCylinderX(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		DrawWireCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		DrawWireCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	public static void DrawWireCylinderY(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		DrawWireCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		DrawWireCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	public static void DrawWireCylinderZ(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		DrawWireCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		DrawWireCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	public static void DrawCylinderX(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		DrawCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		DrawCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	public static void DrawCylinderY(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		DrawCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		DrawCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	public static void DrawCylinderZ(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		DrawCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		DrawCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	public static void DrawWireCapsuleX(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - new Vector3(0.5f * height, 0f, 0f) + Vector3.get_right() * radius;
		Vector3 val2 = pos + new Vector3(0.5f * height, 0f, 0f) - Vector3.get_right() * radius;
		Gizmos.DrawWireSphere(val, radius);
		Gizmos.DrawWireSphere(val2, radius);
		Gizmos.DrawLine(val + Vector3.get_forward() * radius, val2 + Vector3.get_forward() * radius);
		Gizmos.DrawLine(val + Vector3.get_up() * radius, val2 + Vector3.get_up() * radius);
		Gizmos.DrawLine(val + Vector3.get_back() * radius, val2 + Vector3.get_back() * radius);
		Gizmos.DrawLine(val + Vector3.get_down() * radius, val2 + Vector3.get_down() * radius);
	}

	public static void DrawWireCapsuleY(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - new Vector3(0f, 0.5f * height, 0f) + Vector3.get_up() * radius;
		Vector3 val2 = pos + new Vector3(0f, 0.5f * height, 0f) - Vector3.get_up() * radius;
		Gizmos.DrawWireSphere(val, radius);
		Gizmos.DrawWireSphere(val2, radius);
		Gizmos.DrawLine(val + Vector3.get_forward() * radius, val2 + Vector3.get_forward() * radius);
		Gizmos.DrawLine(val + Vector3.get_right() * radius, val2 + Vector3.get_right() * radius);
		Gizmos.DrawLine(val + Vector3.get_back() * radius, val2 + Vector3.get_back() * radius);
		Gizmos.DrawLine(val + Vector3.get_left() * radius, val2 + Vector3.get_left() * radius);
	}

	public static void DrawWireCapsuleZ(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - new Vector3(0f, 0f, 0.5f * height) + Vector3.get_forward() * radius;
		Vector3 val2 = pos + new Vector3(0f, 0f, 0.5f * height) - Vector3.get_forward() * radius;
		Gizmos.DrawWireSphere(val, radius);
		Gizmos.DrawWireSphere(val2, radius);
		Gizmos.DrawLine(val + Vector3.get_up() * radius, val2 + Vector3.get_up() * radius);
		Gizmos.DrawLine(val + Vector3.get_right() * radius, val2 + Vector3.get_right() * radius);
		Gizmos.DrawLine(val + Vector3.get_down() * radius, val2 + Vector3.get_down() * radius);
		Gizmos.DrawLine(val + Vector3.get_left() * radius, val2 + Vector3.get_left() * radius);
	}

	public static void DrawCapsuleX(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - new Vector3(0.5f * height, 0f, 0f);
		Vector3 val2 = pos + new Vector3(0.5f * height, 0f, 0f);
		Gizmos.DrawSphere(val, radius);
		Gizmos.DrawSphere(val2, radius);
	}

	public static void DrawCapsuleY(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - new Vector3(0f, 0.5f * height, 0f);
		Vector3 val2 = pos + new Vector3(0f, 0.5f * height, 0f);
		Gizmos.DrawSphere(val, radius);
		Gizmos.DrawSphere(val2, radius);
	}

	public static void DrawCapsuleZ(Vector3 pos, float radius, float height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - new Vector3(0f, 0f, 0.5f * height);
		Vector3 val2 = pos + new Vector3(0f, 0f, 0.5f * height);
		Gizmos.DrawSphere(val, radius);
		Gizmos.DrawSphere(val2, radius);
	}

	public static void DrawWireCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Matrix4x4.TRS(pos, rot, size));
		Gizmos.DrawWireCube(Vector3.get_zero(), Vector3.get_one());
		Gizmos.set_matrix(matrix);
	}

	public static void DrawCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 matrix = Gizmos.get_matrix();
		Gizmos.set_matrix(Matrix4x4.TRS(pos, rot, size));
		Gizmos.DrawCube(Vector3.get_zero(), Vector3.get_one());
		Gizmos.set_matrix(matrix);
	}

	public static void DrawWirePath(Vector3 a, Vector3 b, float thickness)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		DrawWireCircleY(a, thickness);
		DrawWireCircleY(b, thickness);
		Vector3 val = b - a;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		Vector3 val2 = Quaternion.Euler(0f, 90f, 0f) * normalized;
		Gizmos.DrawLine(b + val2 * thickness, a + val2 * thickness);
		Gizmos.DrawLine(b - val2 * thickness, a - val2 * thickness);
	}

	public static void DrawSemiCircle(float radius)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		float num = radius * ((float)Math.PI / 180f) * 0.5f;
		Vector3 val = Mathf.Cos(num) * Vector3.get_forward() + Mathf.Sin(num) * Vector3.get_right();
		Gizmos.DrawLine(Vector3.get_zero(), val);
		Vector3 val2 = Mathf.Cos(0f - num) * Vector3.get_forward() + Mathf.Sin(0f - num) * Vector3.get_right();
		Gizmos.DrawLine(Vector3.get_zero(), val2);
		float num2 = Mathf.Clamp(radius / 16f, 4f, 64f);
		float num3 = num / num2;
		for (float num4 = num; num4 > 0f; num4 -= num3)
		{
			Vector3 val3 = Mathf.Cos(num4) * Vector3.get_forward() + Mathf.Sin(num4) * Vector3.get_right();
			Gizmos.DrawLine(Vector3.get_zero(), val3);
			if (val != Vector3.get_zero())
			{
				Gizmos.DrawLine(val3, val);
			}
			val = val3;
			Vector3 val4 = Mathf.Cos(0f - num4) * Vector3.get_forward() + Mathf.Sin(0f - num4) * Vector3.get_right();
			Gizmos.DrawLine(Vector3.get_zero(), val4);
			if (val2 != Vector3.get_zero())
			{
				Gizmos.DrawLine(val4, val2);
			}
			val2 = val4;
		}
		Gizmos.DrawLine(val, val2);
	}

	public static void DrawMeshes(Transform transform)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer[] componentsInChildren = ((Component)transform).GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer val in componentsInChildren)
		{
			if (!((Renderer)val).get_enabled())
			{
				continue;
			}
			MeshFilter component = ((Component)val).GetComponent<MeshFilter>();
			if (Object.op_Implicit((Object)(object)component))
			{
				Transform transform2 = ((Component)val).get_transform();
				if ((Object)(object)transform2 != (Object)null && (Object)(object)component != (Object)null && (Object)(object)component.get_sharedMesh() != (Object)null && component.get_sharedMesh().get_normals() != null && component.get_sharedMesh().get_normals().Length != 0)
				{
					Gizmos.DrawMesh(component.get_sharedMesh(), transform2.get_position(), transform2.get_rotation(), transform2.get_lossyScale());
				}
			}
		}
	}

	public static void DrawBounds(Transform transform)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = transform.GetBounds(includeRenderers: true, includeColliders: false);
		Vector3 lossyScale = transform.get_lossyScale();
		Quaternion rotation = transform.get_rotation();
		Vector3 pos = transform.get_position() + rotation * Vector3.Scale(lossyScale, ((Bounds)(ref bounds)).get_center());
		Vector3 size = Vector3.Scale(lossyScale, ((Bounds)(ref bounds)).get_size());
		DrawCube(pos, size, rotation);
		DrawWireCube(pos, size, rotation);
	}
}
