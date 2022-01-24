using UnityEngine;

public class AITraversalArea : TriggerBase
{
	public Transform entryPoint1;

	public Transform entryPoint2;

	public AITraversalWaitPoint[] waitPoints;

	public Bounds movementArea;

	public Transform activeEntryPoint;

	public float nextFreeTime;

	public void OnValidate()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Bounds)(ref movementArea)).set_center(((Component)this).get_transform().get_position());
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!baseEntity.IsNpc)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	public bool CanTraverse(BaseEntity ent)
	{
		return Time.get_time() > nextFreeTime;
	}

	public Transform GetClosestEntry(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(position, entryPoint1.get_position());
		float num2 = Vector3.Distance(position, entryPoint2.get_position());
		if (num < num2)
		{
			return entryPoint1;
		}
		return entryPoint2;
	}

	public Transform GetFarthestEntry(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(position, entryPoint1.get_position());
		float num2 = Vector3.Distance(position, entryPoint2.get_position());
		if (num > num2)
		{
			return entryPoint1;
		}
		return entryPoint2;
	}

	public void SetBusyFor(float dur = 1f)
	{
		nextFreeTime = Time.get_time() + dur;
	}

	public bool CanUse(Vector3 dirFrom)
	{
		return Time.get_time() > nextFreeTime;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
	}

	public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = GetClosestEntry(pos).get_position();
		Vector3 position2 = GetFarthestEntry(pos).get_position();
		_ = new BaseEntity[1];
		AITraversalWaitPoint result = null;
		float num = 0f;
		AITraversalWaitPoint[] array = waitPoints;
		foreach (AITraversalWaitPoint aITraversalWaitPoint in array)
		{
			if (aITraversalWaitPoint.Occupied())
			{
				continue;
			}
			Vector3 position3 = ((Component)aITraversalWaitPoint).get_transform().get_position();
			float num2 = Vector3.Distance(position, position3);
			if (!(Vector3.Distance(position2, position3) < num2))
			{
				float num3 = Vector3.Distance(position3, pos);
				float num4 = (1f - Mathf.InverseLerp(0f, 20f, num3)) * 100f;
				if (num4 > num)
				{
					num = num4;
					result = aITraversalWaitPoint;
				}
			}
		}
		return result;
	}

	public bool EntityFilter(BaseEntity ent)
	{
		if (ent.IsNpc)
		{
			return ent.isServer;
		}
		return false;
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
	}

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_magenta());
		Gizmos.DrawCube(entryPoint1.get_position() + Vector3.get_up() * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawCube(entryPoint2.get_position() + Vector3.get_up() * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.set_color(new Color(0.2f, 1f, 0.2f, 0.5f));
		Gizmos.DrawCube(((Bounds)(ref movementArea)).get_center(), ((Bounds)(ref movementArea)).get_size());
		Gizmos.set_color(Color.get_magenta());
		AITraversalWaitPoint[] array = waitPoints;
		for (int i = 0; i < array.Length; i++)
		{
			GizmosUtil.DrawCircleY(((Component)array[i]).get_transform().get_position(), 0.5f);
		}
	}
}
