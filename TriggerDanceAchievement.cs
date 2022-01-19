using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDanceAchievement : TriggerBase
{
	public int RequiredPlayerCount = 3;

	public string AchievementName;

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
		if (!(baseEntity is BasePlayer))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	public void NotifyDanceStarted()
	{
		if (entityContents == null)
		{
			return;
		}
		int num = 0;
		foreach (BaseEntity entityContent in entityContents)
		{
			if ((Object)(object)entityContent.ToPlayer() != (Object)null && entityContent.ToPlayer().CurrentGestureIsDance)
			{
				num++;
				if (num >= RequiredPlayerCount)
				{
					break;
				}
			}
		}
		if (num < RequiredPlayerCount)
		{
			return;
		}
		foreach (BaseEntity entityContent2 in entityContents)
		{
			if (!triggeredPlayers.Contains(entityContent2.net.ID) && (Object)(object)entityContent2.ToPlayer() != (Object)null)
			{
				entityContent2.ToPlayer().GiveAchievement(AchievementName);
				triggeredPlayers.Add(entityContent2.net.ID);
			}
		}
	}
}
