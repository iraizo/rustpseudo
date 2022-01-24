using System;
using UnityEngine;

public static class WaterLevel
{
	public struct WaterInfo
	{
		public bool isValid;

		public float currentDepth;

		public float overallDepth;

		public float surfaceLevel;
	}

	public static float Factor(Vector3 start, Vector3 end, float radius, BaseEntity forEntity = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.Factor", 0);
		try
		{
			WaterInfo waterInfo = GetWaterInfo(start, end, radius, forEntity);
			return waterInfo.isValid ? Mathf.InverseLerp(Mathf.Min(start.y, end.y) - radius, Mathf.Max(start.y, end.y) + radius, waterInfo.surfaceLevel) : 0f;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static float Factor(Bounds bounds, BaseEntity forEntity = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.Factor", 0);
		try
		{
			if (((Bounds)(ref bounds)).get_size() == Vector3.get_zero())
			{
				((Bounds)(ref bounds)).set_size(new Vector3(0.1f, 0.1f, 0.1f));
			}
			WaterInfo waterInfo = GetWaterInfo(bounds, forEntity);
			return waterInfo.isValid ? Mathf.InverseLerp(((Bounds)(ref bounds)).get_min().y, ((Bounds)(ref bounds)).get_max().y, waterInfo.surfaceLevel) : 0f;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool Test(Vector3 pos, bool waves = true, BaseEntity forEntity = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.Test", 0);
		try
		{
			return GetWaterInfo(pos, waves, forEntity).isValid;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static float GetWaterDepth(Vector3 pos, bool waves = true, BaseEntity forEntity = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterDepth", 0);
		try
		{
			return GetWaterInfo(pos, waves, forEntity).currentDepth;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static float GetOverallWaterDepth(Vector3 pos, bool waves = true, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0);
		try
		{
			return GetWaterInfo(pos, waves, forEntity, noEarlyExit).overallDepth;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetBuoyancyWaterInfo(Vector3 pos, Vector2 posUV, float terrainHeight, float waterHeight, bool doDeepwaterChecks, BaseEntity forEntity = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo result = default(WaterInfo);
			if (pos.y > waterHeight)
			{
				return GetWaterInfoFromVolumes(pos, forEntity);
			}
			bool flag = pos.y < terrainHeight - 1f;
			if (flag)
			{
				waterHeight = 0f;
				if (pos.y > waterHeight)
				{
					return result;
				}
			}
			bool flag2 = doDeepwaterChecks && pos.y < waterHeight - 10f;
			int num = (Object.op_Implicit((Object)(object)TerrainMeta.TopologyMap) ? TerrainMeta.TopologyMap.GetTopologyFast(posUV) : 0);
			if ((flag || flag2 || (num & 0x3C180) == 0) && Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(pos))
			{
				return result;
			}
			RaycastHit val2 = default(RaycastHit);
			if (flag2 && Physics.Raycast(pos, Vector3.get_up(), ref val2, 5f, 16, (QueryTriggerInteraction)2))
			{
				waterHeight = Mathf.Min(waterHeight, ((RaycastHit)(ref val2)).get_point().y);
			}
			result.isValid = true;
			result.currentDepth = Mathf.Max(0f, waterHeight - pos.y);
			result.overallDepth = Mathf.Max(0f, waterHeight - terrainHeight);
			result.surfaceLevel = waterHeight;
			return result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetWaterInfo(Vector3 pos, bool waves = true, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo result = default(WaterInfo);
			float num = 0f;
			if (waves)
			{
				num = WaterSystem.GetHeight(pos);
			}
			else if (Object.op_Implicit((Object)(object)TerrainMeta.WaterMap))
			{
				num = TerrainMeta.WaterMap.GetHeight(pos);
			}
			if (pos.y > num)
			{
				if (!noEarlyExit)
				{
					return GetWaterInfoFromVolumes(pos, forEntity);
				}
				result = GetWaterInfoFromVolumes(pos, forEntity);
			}
			float num2 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(pos) : 0f);
			if (pos.y < num2 - 1f)
			{
				num = 0f;
				if (pos.y > num && !noEarlyExit)
				{
					return result;
				}
			}
			if (Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(pos))
			{
				return result;
			}
			result.isValid = true;
			result.currentDepth = Mathf.Max(0f, num - pos.y);
			result.overallDepth = Mathf.Max(0f, num - num2);
			result.surfaceLevel = num;
			return result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetWaterInfo(Bounds bounds, BaseEntity forEntity = null, bool waves = true)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo result = default(WaterInfo);
			float num = 0f;
			if (waves)
			{
				num = WaterSystem.GetHeight(((Bounds)(ref bounds)).get_center());
			}
			else if (Object.op_Implicit((Object)(object)TerrainMeta.WaterMap))
			{
				num = TerrainMeta.WaterMap.GetHeight(((Bounds)(ref bounds)).get_center());
			}
			if (((Bounds)(ref bounds)).get_min().y > num)
			{
				return GetWaterInfoFromVolumes(bounds, forEntity);
			}
			float num2 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(((Bounds)(ref bounds)).get_center()) : 0f);
			if (((Bounds)(ref bounds)).get_max().y < num2 - 1f)
			{
				num = 0f;
				if (((Bounds)(ref bounds)).get_min().y > num)
				{
					return result;
				}
			}
			if (Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(bounds))
			{
				return result;
			}
			result.isValid = true;
			result.currentDepth = Mathf.Max(0f, num - ((Bounds)(ref bounds)).get_min().y);
			result.overallDepth = Mathf.Max(0f, num - num2);
			result.surfaceLevel = num;
			return result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetWaterInfo(Vector3 start, Vector3 end, float radius, BaseEntity forEntity = null, bool waves = true)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo result = default(WaterInfo);
			float num = 0f;
			Vector3 val2 = (start + end) * 0.5f;
			float num2 = Mathf.Min(start.y, end.y) - radius;
			float num3 = Mathf.Max(start.y, end.y) + radius;
			if (waves)
			{
				num = WaterSystem.GetHeight(val2);
			}
			else if (Object.op_Implicit((Object)(object)TerrainMeta.WaterMap))
			{
				num = TerrainMeta.WaterMap.GetHeight(val2);
			}
			if (num2 > num)
			{
				return GetWaterInfoFromVolumes(start, end, radius, forEntity);
			}
			float num4 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(val2) : 0f);
			if (num3 < num4 - 1f)
			{
				num = 0f;
				if (num2 > num)
				{
					return result;
				}
			}
			if (Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(start, end, radius))
			{
				Vector3 val3 = Vector3Ex.WithY(val2, Mathf.Lerp(num2, num3, 0.75f));
				if (WaterSystem.Collision.GetIgnore(val3))
				{
					return result;
				}
				num = Mathf.Min(num, val3.y);
			}
			result.isValid = true;
			result.currentDepth = Mathf.Max(0f, num - num2);
			result.overallDepth = Mathf.Max(0f, num - num4);
			result.surfaceLevel = num;
			return result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static WaterInfo GetWaterInfoFromVolumes(Bounds bounds, BaseEntity forEntity)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		WaterInfo info = default(WaterInfo);
		if ((Object)(object)forEntity == (Object)null)
		{
			return info;
		}
		forEntity.WaterTestFromVolumes(bounds, out info);
		return info;
	}

	private static WaterInfo GetWaterInfoFromVolumes(Vector3 pos, BaseEntity forEntity)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		WaterInfo info = default(WaterInfo);
		if ((Object)(object)forEntity == (Object)null)
		{
			return info;
		}
		forEntity.WaterTestFromVolumes(pos, out info);
		return info;
	}

	private static WaterInfo GetWaterInfoFromVolumes(Vector3 start, Vector3 end, float radius, BaseEntity forEntity)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		WaterInfo info = default(WaterInfo);
		if ((Object)(object)forEntity == (Object)null)
		{
			return info;
		}
		forEntity.WaterTestFromVolumes(start, end, radius, out info);
		return info;
	}
}
