using System.Collections.Generic;
using UnityEngine;

public class TriggerSafeZone : TriggerBase
{
	public static List<TriggerSafeZone> allSafeZones = new List<TriggerSafeZone>();

	public float maxDepth = 20f;

	public float maxAltitude = -1f;

	public Collider triggerCollider { get; private set; }

	protected void Awake()
	{
		triggerCollider = ((Component)this).GetComponent<Collider>();
	}

	protected void OnEnable()
	{
		allSafeZones.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		allSafeZones.Remove(this);
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
		return ((Component)baseEntity).get_gameObject();
	}

	public bool PassesHeightChecks(Vector3 entPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).get_transform().get_position();
		float num = Mathf.Abs(position.y - entPos.y);
		if (maxDepth != -1f && entPos.y < position.y && num > maxDepth)
		{
			return false;
		}
		if (maxAltitude != -1f && entPos.y > position.y && num > maxAltitude)
		{
			return false;
		}
		return true;
	}

	public float GetSafeLevel(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!PassesHeightChecks(pos))
		{
			return 0f;
		}
		return 1f;
	}
}
