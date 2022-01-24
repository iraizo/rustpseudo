using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class MissionPoint : MonoBehaviour
{
	public enum MissionPointEnum
	{
		EasyMonument = 1,
		MediumMonument = 2,
		HardMonument = 4,
		Item_Hidespot = 8,
		Underwater = 0x80
	}

	public bool dropToGround = true;

	public const int COUNT = 8;

	public const int EVERYTHING = -1;

	public const int NOTHING = 0;

	public const int EASY_MONUMENT = 1;

	public const int MED_MONUMENT = 2;

	public const int HARD_MONUMENT = 4;

	public const int ITEM_HIDESPOT = 8;

	public const int UNDERWATER = 128;

	public const int EASY_MONUMENT_IDX = 0;

	public const int MED_MONUMENT_IDX = 1;

	public const int HARD_MONUMENT_IDX = 2;

	public const int ITEM_HIDESPOT_IDX = 3;

	public const int FOREST_IDX = 4;

	public const int ROADSIDE_IDX = 5;

	public const int BEACH = 6;

	public const int UNDERWATER_IDX = 7;

	private static Dictionary<int, int> type2index = new Dictionary<int, int>
	{
		{ 1, 0 },
		{ 2, 1 },
		{ 4, 2 },
		{ 8, 3 },
		{ 128, 7 }
	};

	public static List<MissionPoint> all = new List<MissionPoint>();

	[InspectorFlags]
	public MissionPointEnum Flags = (MissionPointEnum)(-1);

	public static int TypeToIndex(int id)
	{
		return type2index[id];
	}

	public static int IndexToType(int idx)
	{
		return 1 << idx;
	}

	public void Awake()
	{
		if (dropToGround)
		{
			((Component)this).get_transform().DropToGround();
		}
		all.Add(this);
	}

	public void OnDisable()
	{
		if (all.Contains(this))
		{
			all.Remove(this);
		}
	}

	public virtual Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position();
	}

	public virtual Quaternion GetRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_rotation();
	}

	public static bool GetMissionPoints(ref List<MissionPoint> points, Vector3 near, float minDistance, float maxDistance, int flags, int exclusionFlags)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		List<MissionPoint> list = Pool.GetList<MissionPoint>();
		foreach (MissionPoint item in all)
		{
			if (((uint)item.Flags & (uint)flags) != (uint)flags || (exclusionFlags != 0 && ((uint)item.Flags & (uint)exclusionFlags) != 0))
			{
				continue;
			}
			float num = Vector3.Distance(((Component)item).get_transform().get_position(), near);
			if (!(num <= maxDistance) || !(num > minDistance))
			{
				continue;
			}
			if (BaseMission.blockedPoints.Count > 0)
			{
				bool flag = false;
				foreach (Vector3 blockedPoint in BaseMission.blockedPoints)
				{
					if (Vector3.Distance(blockedPoint, ((Component)item).get_transform().get_position()) < 5f)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			list.Add(item);
		}
		if (list.Count == 0)
		{
			return false;
		}
		foreach (MissionPoint item2 in list)
		{
			points.Add(item2);
		}
		Pool.FreeList<MissionPoint>(ref list);
		return true;
	}

	public MissionPoint()
		: this()
	{
	}
}
