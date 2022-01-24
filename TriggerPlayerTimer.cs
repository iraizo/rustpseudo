using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

public class TriggerPlayerTimer : TriggerBase
{
	public BaseEntity TargetEntity;

	public float DamageAmount = 20f;

	public float TimeToDamage = 3f;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj != (Object)null)
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			BasePlayer basePlayer;
			if ((basePlayer = baseEntity as BasePlayer) != null && baseEntity.isServer && !basePlayer.isMounted)
			{
				return ((Component)baseEntity).get_gameObject();
			}
		}
		return obj;
	}

	internal override void OnObjects()
	{
		base.OnObjects();
		((FacepunchBehaviour)this).Invoke((Action)DamageTarget, TimeToDamage);
	}

	internal override void OnEmpty()
	{
		base.OnEmpty();
		((FacepunchBehaviour)this).CancelInvoke((Action)DamageTarget);
	}

	private void DamageTarget()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Enumerator<BaseEntity> enumerator = entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer basePlayer;
				if ((basePlayer = enumerator.get_Current() as BasePlayer) != null && !basePlayer.isMounted)
				{
					flag = true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (flag && (Object)(object)TargetEntity != (Object)null)
		{
			TargetEntity.OnAttacked(new HitInfo(null, TargetEntity, DamageType.Generic, DamageAmount));
		}
		((FacepunchBehaviour)this).Invoke((Action)DamageTarget, TimeToDamage);
	}
}
