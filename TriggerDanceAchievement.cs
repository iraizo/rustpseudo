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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents == null)
		{
			return;
		}
		int num = 0;
		Enumerator<BaseEntity> enumerator = entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.get_Current();
				if ((Object)(object)current.ToPlayer() != (Object)null && current.ToPlayer().CurrentGestureIsDance)
				{
					num++;
					if (num >= RequiredPlayerCount)
					{
						break;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (num < RequiredPlayerCount)
		{
			return;
		}
		enumerator = entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current2 = enumerator.get_Current();
				if (!triggeredPlayers.Contains(current2.net.ID) && (Object)(object)current2.ToPlayer() != (Object)null)
				{
					current2.ToPlayer().GiveAchievement(AchievementName);
					triggeredPlayers.Add(current2.net.ID);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}
}
