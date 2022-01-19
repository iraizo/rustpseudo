using UnityEngine;

public class TriggerForce : TriggerBase, IServerComponent
{
	public const float GravityMultiplier = 0.1f;

	public const float VelocityLerp = 10f;

	public const float AngularDrag = 10f;

	public Vector3 velocity = Vector3.get_forward();

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

	internal override void OnEntityEnter(BaseEntity ent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		base.OnEntityEnter(ent);
		Vector3 val = ((Component)this).get_transform().TransformDirection(velocity);
		ent.ApplyInheritedVelocity(val);
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.get_zero());
	}

	protected void FixedUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents == null)
		{
			return;
		}
		Vector3 val = ((Component)this).get_transform().TransformDirection(velocity);
		foreach (BaseEntity entityContent in entityContents)
		{
			if ((Object)(object)entityContent != (Object)null)
			{
				entityContent.ApplyInheritedVelocity(val);
			}
		}
	}
}
