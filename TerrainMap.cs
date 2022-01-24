using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class TerrainMap : TerrainExtension
{
	internal int res;

	public void ApplyFilter(float normX, float normZ, float radius, float fade, Action<int, int, float> action)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.OneOverSize.x * radius;
		float num2 = TerrainMeta.OneOverSize.x * fade;
		float num3 = (float)res * (num - num2);
		float num4 = (float)res * num;
		float num5 = normX * (float)res;
		float num6 = normZ * (float)res;
		int num7 = Index(normX - num);
		int num8 = Index(normX + num);
		int num9 = Index(normZ - num);
		int num10 = Index(normZ + num);
		Vector2 val;
		if (num3 != num4)
		{
			for (int i = num9; i <= num10; i++)
			{
				for (int j = num7; j <= num8; j++)
				{
					val = new Vector2((float)j + 0.5f - num5, (float)i + 0.5f - num6);
					float magnitude = ((Vector2)(ref val)).get_magnitude();
					float arg = Mathf.InverseLerp(num4, num3, magnitude);
					action(j, i, arg);
				}
			}
			return;
		}
		for (int k = num9; k <= num10; k++)
		{
			for (int l = num7; l <= num8; l++)
			{
				val = new Vector2((float)l + 0.5f - num5, (float)k + 0.5f - num6);
				float arg2 = ((((Vector2)(ref val)).get_magnitude() < num4) ? 1 : 0);
				action(l, k, arg2);
			}
		}
	}

	public void ForEach(Vector3 worldPos, float radius, Action<int, int> action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int num = Index(TerrainMeta.NormalizeX(worldPos.x - radius));
		int num2 = Index(TerrainMeta.NormalizeX(worldPos.x + radius));
		int num3 = Index(TerrainMeta.NormalizeZ(worldPos.z - radius));
		int num4 = Index(TerrainMeta.NormalizeZ(worldPos.z + radius));
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				action(j, i);
			}
		}
	}

	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v3 = default(Vector2i);
		((Vector2i)(ref v3))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		ForEachParallel(v3, v4, v5, action);
	}

	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathx.Min(v0.x, v1.x, v2.x);
		int num2 = Mathx.Max(v0.x, v1.x, v2.x);
		int num3 = Mathx.Min(v0.y, v1.y, v2.y);
		int num4 = Mathx.Max(v0.y, v1.y, v2.y);
		Vector2i base_min = new Vector2i(num, num3);
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(num2, num4);
		Vector2i base_count = val - base_min + Vector2i.one;
		Parallel.Call((Action<int, int>)delegate(int thread_id, int thread_count)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Vector2i min = base_min + base_count * thread_id / thread_count;
			Vector2i max = base_min + base_count * (thread_id + 1) / thread_count - Vector2i.one;
			ForEachInternal(v0, v1, v2, action, min, max);
		});
	}

	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v3 = default(Vector2i);
		((Vector2i)(ref v3))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		ForEach(v3, v4, v5, action);
	}

	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector2i min = default(Vector2i);
		((Vector2i)(ref min))._002Ector(int.MinValue, int.MinValue);
		Vector2i max = default(Vector2i);
		((Vector2i)(ref max))._002Ector(int.MaxValue, int.MaxValue);
		ForEachInternal(v0, v1, v2, action, min, max);
	}

	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action, Vector2i min, Vector2i max)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x));
		int num2 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x));
		int num3 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y));
		int num4 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y));
		int num5 = v0.y - v1.y;
		int num6 = v1.x - v0.x;
		int num7 = v1.y - v2.y;
		int num8 = v2.x - v1.x;
		int num9 = v2.y - v0.y;
		int num10 = v0.x - v2.x;
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(num, num3);
		int num11 = (v2.x - v1.x) * (val.y - v1.y) - (v2.y - v1.y) * (val.x - v1.x);
		int num12 = (v0.x - v2.x) * (val.y - v2.y) - (v0.y - v2.y) * (val.x - v2.x);
		int num13 = (v1.x - v0.x) * (val.y - v0.y) - (v1.y - v0.y) * (val.x - v0.x);
		val.y = num3;
		while (val.y <= num4)
		{
			int num14 = num11;
			int num15 = num12;
			int num16 = num13;
			val.x = num;
			while (val.x <= num2)
			{
				if ((num14 | num15 | num16) >= 0)
				{
					action(val.x, val.y);
				}
				num14 += num7;
				num15 += num9;
				num16 += num5;
				val.x++;
			}
			num11 += num8;
			num12 += num10;
			num13 += num6;
			val.y++;
		}
	}

	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6 = default(Vector2i);
		((Vector2i)(ref v6))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7 = default(Vector2i);
		((Vector2i)(ref v7))._002Ector(Index(TerrainMeta.NormalizeX(v3.x)), Index(TerrainMeta.NormalizeZ(v3.z)));
		ForEachParallel(v4, v5, v6, v7, action);
	}

	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathx.Min(v0.x, v1.x, v2.x, v3.x);
		int num2 = Mathx.Max(v0.x, v1.x, v2.x, v3.x);
		int num3 = Mathx.Min(v0.y, v1.y, v2.y, v3.y);
		int num4 = Mathx.Max(v0.y, v1.y, v2.y, v3.y);
		Vector2i base_min = new Vector2i(num, num3);
		Vector2i val = new Vector2i(num2, num4) - base_min + Vector2i.one;
		Vector2i size_x = new Vector2i(val.x, 0);
		Vector2i size_y = new Vector2i(0, val.y);
		Parallel.Call((Action<int, int>)delegate(int thread_id, int thread_count)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			Vector2i min = base_min + size_y * thread_id / thread_count;
			Vector2i max = base_min + size_y * (thread_id + 1) / thread_count + size_x - Vector2i.one;
			ForEachInternal(v0, v1, v2, v3, action, min, max);
		});
	}

	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6 = default(Vector2i);
		((Vector2i)(ref v6))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7 = default(Vector2i);
		((Vector2i)(ref v7))._002Ector(Index(TerrainMeta.NormalizeX(v3.x)), Index(TerrainMeta.NormalizeZ(v3.z)));
		ForEach(v4, v5, v6, v7, action);
	}

	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector2i min = default(Vector2i);
		((Vector2i)(ref min))._002Ector(int.MinValue, int.MinValue);
		Vector2i max = default(Vector2i);
		((Vector2i)(ref max))._002Ector(int.MaxValue, int.MaxValue);
		ForEachInternal(v0, v1, v2, v3, action, min, max);
	}

	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action, Vector2i min, Vector2i max)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x, v3.x));
		int num2 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x, v3.x));
		int num3 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y, v3.y));
		int num4 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y, v3.y));
		int num5 = v0.y - v1.y;
		int num6 = v1.x - v0.x;
		int num7 = v1.y - v2.y;
		int num8 = v2.x - v1.x;
		int num9 = v2.y - v0.y;
		int num10 = v0.x - v2.x;
		int num11 = v3.y - v2.y;
		int num12 = v2.x - v3.x;
		int num13 = v2.y - v1.y;
		int num14 = v1.x - v2.x;
		int num15 = v1.y - v3.y;
		int num16 = v3.x - v1.x;
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(num, num3);
		int num17 = (v2.x - v1.x) * (val.y - v1.y) - (v2.y - v1.y) * (val.x - v1.x);
		int num18 = (v0.x - v2.x) * (val.y - v2.y) - (v0.y - v2.y) * (val.x - v2.x);
		int num19 = (v1.x - v0.x) * (val.y - v0.y) - (v1.y - v0.y) * (val.x - v0.x);
		int num20 = (v1.x - v2.x) * (val.y - v2.y) - (v1.y - v2.y) * (val.x - v2.x);
		int num21 = (v3.x - v1.x) * (val.y - v1.y) - (v3.y - v1.y) * (val.x - v1.x);
		int num22 = (v2.x - v3.x) * (val.y - v3.y) - (v2.y - v3.y) * (val.x - v3.x);
		val.y = num3;
		while (val.y <= num4)
		{
			int num23 = num17;
			int num24 = num18;
			int num25 = num19;
			int num26 = num20;
			int num27 = num21;
			int num28 = num22;
			val.x = num;
			while (val.x <= num2)
			{
				if ((num23 | num24 | num25) >= 0 || (num26 | num27 | num28) >= 0)
				{
					action(val.x, val.y);
				}
				num23 += num7;
				num24 += num9;
				num25 += num5;
				num26 += num13;
				num27 += num15;
				num28 += num11;
				val.x++;
			}
			num17 += num8;
			num18 += num10;
			num19 += num6;
			num20 += num14;
			num21 += num16;
			num22 += num12;
			val.y++;
		}
	}

	public void ForEach(int x_min, int x_max, int z_min, int z_max, Action<int, int> action)
	{
		for (int i = z_min; i <= z_max; i++)
		{
			for (int j = x_min; j <= x_max; j++)
			{
				action(j, i);
			}
		}
	}

	public void ForEach(Action<int, int> action)
	{
		for (int i = 0; i < res; i++)
		{
			for (int j = 0; j < res; j++)
			{
				action(j, i);
			}
		}
	}

	public int Index(float normalized)
	{
		int num = (int)(normalized * (float)res);
		if (num >= 0)
		{
			if (num <= res - 1)
			{
				return num;
			}
			return res - 1;
		}
		return 0;
	}

	public float Coordinate(int index)
	{
		return ((float)index + 0.5f) / (float)res;
	}
}
public abstract class TerrainMap<T> : TerrainMap where T : struct
{
	internal T[] src;

	internal T[] dst;

	public void Push()
	{
		if (src == dst)
		{
			dst = (T[])src.Clone();
		}
	}

	public void Pop()
	{
		if (src != dst)
		{
			Array.Copy(dst, src, src.Length);
			dst = src;
		}
	}

	public IEnumerable<T> ToEnumerable()
	{
		return Enumerable.Cast<T>((IEnumerable)src);
	}

	public int BytesPerElement()
	{
		return Marshal.SizeOf(typeof(T));
	}

	public long GetMemoryUsage()
	{
		return (long)BytesPerElement() * (long)src.Length;
	}

	public byte[] ToByteArray()
	{
		byte[] array = new byte[BytesPerElement() * src.Length];
		Buffer.BlockCopy(src, 0, array, 0, array.Length);
		return array;
	}

	public void FromByteArray(byte[] dat)
	{
		Buffer.BlockCopy(dat, 0, dst, 0, dat.Length);
	}
}
