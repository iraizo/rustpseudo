using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWakeAIZ : TriggerBase, IServerComponent
{
	public float SleepDelaySeconds = 30f;

	public List<AIInformationZone> zones = new List<AIInformationZone>();

	private AIInformationZone aiz;

	public void Init(AIInformationZone zone = null)
	{
		if ((Object)(object)zone != (Object)null)
		{
			aiz = zone;
		}
		else if (zones == null || zones.Count == 0)
		{
			Transform val = ((Component)this).get_transform().get_parent();
			if ((Object)(object)val == (Object)null)
			{
				val = ((Component)this).get_transform();
			}
			aiz = ((Component)val).GetComponentInChildren<AIInformationZone>();
		}
		SetZonesSleeping(flag: true);
	}

	private void Awake()
	{
		Init();
	}

	private void SetZonesSleeping(bool flag)
	{
		if ((Object)(object)aiz != (Object)null)
		{
			if (flag)
			{
				aiz.SleepAI();
			}
			else
			{
				aiz.WakeAI();
			}
		}
		if (zones == null || zones.Count <= 0)
		{
			return;
		}
		foreach (AIInformationZone zone in zones)
		{
			if ((Object)(object)zone != (Object)null)
			{
				if (flag)
				{
					zone.SleepAI();
				}
				else
				{
					zone.WakeAI();
				}
			}
		}
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
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null && basePlayer.IsNpc)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (!((Object)(object)aiz == (Object)null) || (zones != null && zones.Count != 0))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)SleepAI);
			SetZonesSleeping(flag: false);
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if ((!((Object)(object)aiz == (Object)null) || (zones != null && zones.Count != 0)) && (entityContents == null || entityContents.Count == 0))
		{
			DelayedSleepAI();
		}
	}

	private void DelayedSleepAI()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)SleepAI);
		((FacepunchBehaviour)this).Invoke((Action)SleepAI, SleepDelaySeconds);
	}

	private void SleepAI()
	{
		SetZonesSleeping(flag: true);
	}
}
