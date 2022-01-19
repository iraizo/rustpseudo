using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public static class EnvironmentVolumeEx
{
	public static bool CheckEnvironmentVolumes(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		((Component)transform).GetComponentsInChildren<EnvironmentVolume>(true, list);
		OBB obb = default(OBB);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			((OBB)(ref obb))._002Ector(((Component)environmentVolume).get_transform(), new Bounds(environmentVolume.Center, environmentVolume.Size));
			((OBB)(ref obb)).Transform(pos, scale, rot);
			if (EnvironmentManager.Check(obb, type))
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return true;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return false;
	}

	public static bool CheckEnvironmentVolumes(this Transform transform, EnvironmentType type)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return transform.CheckEnvironmentVolumes(transform.get_position(), transform.get_rotation(), transform.get_lossyScale(), type);
	}

	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.HeightMap == (Object)null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		((Component)transform).GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		OBB val = default(OBB);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) == 0)
			{
				continue;
			}
			((OBB)(ref val))._002Ector(((Component)environmentVolume).get_transform(), new Bounds(environmentVolume.Center, environmentVolume.Size));
			((OBB)(ref val)).Transform(pos, scale, rot);
			Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
			Vector3 point2 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
			Vector3 point3 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
			Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
			Bounds val2 = ((OBB)(ref val)).ToBounds();
			float max = ((Bounds)(ref val2)).get_max().y + padding;
			bool fail = false;
			TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
			{
				if (TerrainMeta.HeightMap.GetHeight(x, z) <= max)
				{
					fail = true;
				}
			});
			if (fail)
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return false;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, EnvironmentType type)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return transform.CheckEnvironmentVolumesInsideTerrain(transform.get_position(), transform.get_rotation(), transform.get_lossyScale(), type);
	}

	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.HeightMap == (Object)null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		((Component)transform).GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		OBB val = default(OBB);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) == 0)
			{
				continue;
			}
			((OBB)(ref val))._002Ector(((Component)environmentVolume).get_transform(), new Bounds(environmentVolume.Center, environmentVolume.Size));
			((OBB)(ref val)).Transform(pos, scale, rot);
			Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
			Vector3 point2 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
			Vector3 point3 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
			Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
			Bounds val2 = ((OBB)(ref val)).ToBounds();
			float min = ((Bounds)(ref val2)).get_min().y - padding;
			bool fail = false;
			TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
			{
				if (TerrainMeta.HeightMap.GetHeight(x, z) >= min)
				{
					fail = true;
				}
			});
			if (fail)
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return false;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, EnvironmentType type)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return transform.CheckEnvironmentVolumesOutsideTerrain(transform.get_position(), transform.get_rotation(), transform.get_lossyScale(), type);
	}
}
