using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerForce : TriggerBase, IServerComponent
{
	public BoxCollider triggerCollider;

	public float pushVelocity = 5f;

	public bool requireUpAxis;

	private const float HACK_DISABLE_TIME = 4f;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			return ((Component)baseEntity).get_gameObject();
		}
		return null;
	}

	internal override void OnObjects()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)HackDisableTick, 0f, 3.75f);
	}

	internal override void OnEmpty()
	{
		base.OnEmpty();
		((FacepunchBehaviour)this).CancelInvoke((Action)HackDisableTick);
	}

	protected override void OnDisable()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)HackDisableTick);
		base.OnDisable();
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.get_zero());
	}

	private void HackDisableTick()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents == null || !((Behaviour)this).get_enabled())
		{
			return;
		}
		Enumerator<BaseEntity> enumerator = entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.get_Current();
				if (IsInterested(current))
				{
					BasePlayer basePlayer = current.ToPlayer();
					if ((Object)(object)basePlayer != (Object)null && !basePlayer.IsNpc)
					{
						basePlayer.PauseVehicleNoClipDetection(4f);
						basePlayer.PauseSpeedHackDetection(4f);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	protected void FixedUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents == null)
		{
			return;
		}
		Enumerator<BaseEntity> enumerator = entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.get_Current();
				if ((!requireUpAxis || !(Vector3.Dot(((Component)current).get_transform().get_up(), ((Component)this).get_transform().get_up()) < 0f)) && IsInterested(current))
				{
					Vector3 velocity = GetPushVelocity(((Component)current).get_gameObject());
					current.ApplyInheritedVelocity(velocity);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	private Vector3 GetPushVelocity(GameObject obj)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = -(((Component)this).get_transform().get_position() + triggerCollider.get_center() - obj.get_transform().get_position());
		((Vector3)(ref val)).Normalize();
		val.y = 0.2f;
		((Vector3)(ref val)).Normalize();
		return val * pushVelocity;
	}

	private bool IsInterested(BaseEntity entity)
	{
		if ((Object)(object)entity == (Object)null || entity.isClient)
		{
			return false;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		if ((Object)(object)basePlayer != (Object)null)
		{
			if ((basePlayer.IsAdmin || basePlayer.IsDeveloper) && basePlayer.IsFlying)
			{
				return false;
			}
			if ((Object)(object)basePlayer != (Object)null && basePlayer.IsAlive())
			{
				return !basePlayer.isMounted;
			}
			return false;
		}
		return true;
	}
}
