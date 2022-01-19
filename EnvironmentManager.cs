using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class EnvironmentManager : SingletonComponent<EnvironmentManager>
{
	public static EnvironmentType Get(OBB obb)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		EnvironmentType environmentType = (EnvironmentType)0;
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		GamePhysics.OverlapOBB<EnvironmentVolume>(obb, list, 262144, (QueryTriggerInteraction)2);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return environmentType;
	}

	public static EnvironmentType Get(Vector3 pos, ref List<EnvironmentVolume> list)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EnvironmentType environmentType = (EnvironmentType)0;
		GamePhysics.OverlapSphere<EnvironmentVolume>(pos, 0.01f, list, 262144, (QueryTriggerInteraction)2);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		return environmentType;
	}

	public static EnvironmentType Get(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		EnvironmentType result = Get(pos, ref list);
		Pool.FreeList<EnvironmentVolume>(ref list);
		return result;
	}

	public static bool Check(OBB obb, EnvironmentType type)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return (Get(obb) & type) != 0;
	}

	public static bool Check(Vector3 pos, EnvironmentType type)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return (Get(pos) & type) != 0;
	}
}
