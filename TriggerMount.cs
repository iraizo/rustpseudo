using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMount : TriggerBase, IServerComponent
{
	private class EntryInfo
	{
		public float entryTime;

		public Vector3 entryPos;

		public EntryInfo(float entryTime, Vector3 entryPos)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}

		public void Set(float entryTime, Vector3 entryPos)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}
	}

	private const float MOUNT_DELAY = 3.5f;

	private const float MAX_MOVE = 0.5f;

	private Dictionary<BaseEntity, EntryInfo> entryInfo;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		BasePlayer basePlayer = baseEntity.ToPlayer();
		if ((Object)(object)basePlayer == (Object)null || basePlayer.IsNpc)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.OnEntityEnter(ent);
		if (entryInfo == null)
		{
			entryInfo = new Dictionary<BaseEntity, EntryInfo>();
		}
		entryInfo.Add(ent, new EntryInfo(Time.get_time(), ((Component)ent).get_transform().get_position()));
		((FacepunchBehaviour)this).Invoke((Action)CheckForMount, 3.6f);
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		if ((Object)(object)ent != (Object)null && entryInfo != null)
		{
			entryInfo.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	private void CheckForMount()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents == null || entryInfo == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseEntity, EntryInfo> item in entryInfo)
		{
			BaseEntity key = item.Key;
			if (!key.IsValid())
			{
				continue;
			}
			EntryInfo value = item.Value;
			BasePlayer basePlayer = key.ToPlayer();
			bool flag = (basePlayer.IsAdmin || basePlayer.IsDeveloper) && basePlayer.IsFlying;
			if (!((Object)(object)basePlayer != (Object)null) || !basePlayer.IsAlive() || flag)
			{
				continue;
			}
			bool flag2 = false;
			if (!basePlayer.isMounted && !basePlayer.IsSleeping() && value.entryTime + 3.5f < Time.get_time() && Vector3.Distance(((Component)key).get_transform().get_position(), value.entryPos) < 0.5f)
			{
				BaseVehicle componentInParent = ((Component)this).GetComponentInParent<BaseVehicle>();
				if ((Object)(object)componentInParent != (Object)null && !componentInParent.IsDead())
				{
					componentInParent.AttemptMount(basePlayer);
					flag2 = true;
				}
			}
			if (!flag2)
			{
				value.Set(Time.get_time(), ((Component)key).get_transform().get_position());
				((FacepunchBehaviour)this).Invoke((Action)CheckForMount, 3.6f);
			}
		}
	}
}
