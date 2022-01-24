using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

public class DirectionalDamageTrigger : TriggerBase
{
	public float repeatRate = 1f;

	public List<DamageTypeEntry> damageType;

	public GameObjectRef attackEffect;

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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	internal override void OnObjects()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)OnTick, repeatRate, repeatRate);
	}

	internal override void OnEmpty()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
	}

	private void OnTick()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (attackEffect.isValid)
		{
			Effect.server.Run(attackEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
		}
		if (entityContents == null)
		{
			return;
		}
		BaseEntity[] array = Enumerable.ToArray<BaseEntity>((IEnumerable<BaseEntity>)entityContents);
		foreach (BaseEntity baseEntity in array)
		{
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!((Object)(object)baseCombatEntity == (Object)null))
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(damageType);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.PointStart = ((Component)this).get_transform().get_position();
					hitInfo.PointEnd = ((Component)baseCombatEntity).get_transform().get_position();
					baseCombatEntity.Hurt(hitInfo);
				}
			}
		}
	}
}
