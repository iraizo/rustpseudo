using UnityEngine;

public class TriggerVehiclePush : TriggerBase, IServerComponent
{
	public BaseEntity thisEntity;

	public float maxPushVelocity = 10f;

	public float minRadius;

	public float maxRadius;

	public bool snapToAxis;

	public Vector3 axisToSnapTo = Vector3.get_right();

	public bool allowParentRigidbody;

	public bool useRigidbodyPosition;

	public bool useCentreOfMass;

	public int ContentsCount => entityContents?.Count ?? 0;

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
		if (baseEntity is BuildingBlock)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	public void FixedUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)thisEntity == (Object)null || entityContents == null)
		{
			return;
		}
		Vector3 position = ((Component)this).get_transform().get_position();
		foreach (BaseEntity entityContent in entityContents)
		{
			if (!entityContent.IsValid() || entityContent.EqualNetID(thisEntity))
			{
				continue;
			}
			Rigidbody val = ((Component)entityContent).GetComponent<Rigidbody>();
			if ((Object)(object)val == (Object)null && allowParentRigidbody)
			{
				val = ((Component)entityContent).GetComponentInParent<Rigidbody>();
			}
			if (Object.op_Implicit((Object)(object)val) && !val.get_isKinematic())
			{
				float num = Vector3Ex.Distance2D(useRigidbodyPosition ? ((Component)val).get_transform().get_position() : ((Component)entityContent).get_transform().get_position(), ((Component)this).get_transform().get_position());
				float num2 = 1f - Mathf.InverseLerp(minRadius, maxRadius, num);
				float num3 = 1f - Mathf.InverseLerp(minRadius - 1f, minRadius, num);
				Vector3 val2 = entityContent.ClosestPoint(position);
				Vector3 val3 = Vector3Ex.Direction2D(val2, position);
				val3 = Vector3Ex.Direction2D(useCentreOfMass ? val.get_worldCenterOfMass() : val2, position);
				if (snapToAxis)
				{
					Vector3 val4 = ((Component)this).get_transform().InverseTransformDirection(val3);
					val3 = ((!(Vector3.Angle(val4, axisToSnapTo) < Vector3.Angle(val4, -axisToSnapTo))) ? (-((Component)this).get_transform().TransformDirection(axisToSnapTo)) : ((Component)this).get_transform().TransformDirection(axisToSnapTo));
				}
				val.AddForceAtPosition(val3 * maxPushVelocity * num2, val2, (ForceMode)5);
				if (num3 > 0f)
				{
					val.AddForceAtPosition(val3 * 1f * num3, val2, (ForceMode)2);
				}
			}
		}
	}

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_red());
		Gizmos.DrawWireSphere(((Component)this).get_transform().get_position(), minRadius);
		Gizmos.set_color(new Color(0.5f, 0f, 0f, 1f));
		Gizmos.DrawWireSphere(((Component)this).get_transform().get_position(), maxRadius);
		if (snapToAxis)
		{
			Gizmos.set_color(Color.get_cyan());
			Vector3 val = ((Component)this).get_transform().TransformDirection(axisToSnapTo);
			Gizmos.DrawLine(((Component)this).get_transform().get_position() + val, ((Component)this).get_transform().get_position() - val);
		}
	}
}
