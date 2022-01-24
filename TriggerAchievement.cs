using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAchievement : TriggerBase
{
	public string statToIncrease = "";

	public string achievementOnEnter = "";

	public string requiredVehicleName = "";

	[Tooltip("Always set to true, clientside does not work, currently")]
	public bool serverSide = true;

	[NonSerialized]
	private List<ulong> triggeredPlayers = new List<ulong>();

	public void OnPuzzleReset()
	{
		Reset();
	}

	public void Reset()
	{
		triggeredPlayers.Clear();
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
		if (baseEntity.isClient && serverSide)
		{
			return null;
		}
		if (baseEntity.isServer && !serverSide)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if ((Object)(object)ent == (Object)null)
		{
			return;
		}
		BasePlayer component = ((Component)ent).GetComponent<BasePlayer>();
		if ((Object)(object)component == (Object)null || !component.IsAlive() || component.IsSleeping() || component.IsNpc || triggeredPlayers.Contains(component.userID))
		{
			return;
		}
		if (!string.IsNullOrEmpty(requiredVehicleName))
		{
			BaseVehicle mountedVehicle = component.GetMountedVehicle();
			if ((Object)(object)mountedVehicle == (Object)null || !mountedVehicle.ShortPrefabName.Contains(requiredVehicleName))
			{
				return;
			}
		}
		if (serverSide)
		{
			if (!string.IsNullOrEmpty(achievementOnEnter))
			{
				component.GiveAchievement(achievementOnEnter);
			}
			if (!string.IsNullOrEmpty(statToIncrease))
			{
				component.stats.Add(statToIncrease, 1);
				component.stats.Save();
			}
			triggeredPlayers.Add(component.userID);
		}
	}
}
