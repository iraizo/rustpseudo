using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class DeployVolume : PrefabAttribute
{
	public enum EntityMode
	{
		ExcludeList,
		IncludeList
	}

	public LayerMask layers = LayerMask.op_Implicit(537001984);

	[InspectorFlags]
	public ColliderInfo.Flags ignore;

	public EntityMode entityMode;

	[FormerlySerializedAs("entities")]
	public BaseEntity[] entityList;

	protected override Type GetIndexedType()
	{
		return typeof(DeployVolume);
	}

	protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

	protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, int mask = -1)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, mask))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, OBB test, int mask = -1)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, test, mask))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckSphere(Vector3 pos, float radius, int layerMask, DeployVolume volume)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, DeployVolume volume)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckOBB(OBB obb, int layerMask, DeployVolume volume)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckBounds(Bounds bounds, int layerMask, DeployVolume volume)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	private static bool CheckFlags(List<Collider> list, DeployVolume volume)
	{
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = ((Component)list[i]).get_gameObject();
			if (gameObject.CompareTag("DeployVolumeIgnore"))
			{
				continue;
			}
			ColliderInfo component = gameObject.GetComponent<ColliderInfo>();
			if (!((Object)(object)component == (Object)null) && volume.ignore != 0 && component.HasFlag(volume.ignore))
			{
				continue;
			}
			if (volume.entityList.Length == 0)
			{
				return true;
			}
			BaseEntity baseEntity = list[i].ToBaseEntity();
			bool flag = false;
			if ((Object)(object)baseEntity != (Object)null)
			{
				BaseEntity[] array = volume.entityList;
				foreach (BaseEntity baseEntity2 in array)
				{
					if (baseEntity.prefabID == baseEntity2.prefabID)
					{
						flag = true;
						break;
					}
				}
			}
			if (volume.entityMode == EntityMode.IncludeList)
			{
				if (flag)
				{
					return true;
				}
			}
			else if (volume.entityMode == EntityMode.ExcludeList && !flag)
			{
				return true;
			}
		}
		return false;
	}
}
