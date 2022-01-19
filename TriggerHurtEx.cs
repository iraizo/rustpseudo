using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

public class TriggerHurtEx : TriggerBase, IServerComponent, IHurtTrigger
{
	public enum HurtType
	{
		Simple,
		IncludeBleedingAndScreenShake
	}

	public class EntityTriggerInfo
	{
		public Vector3 lastPosition;
	}

	public float repeatRate = 0.1f;

	[Header("On Enter")]
	public List<DamageTypeEntry> damageOnEnter;

	public GameObjectRef effectOnEnter;

	public HurtType hurtTypeOnEnter;

	[Header("On Timer (damage per second)")]
	public List<DamageTypeEntry> damageOnTimer;

	public GameObjectRef effectOnTimer;

	public HurtType hurtTypeOnTimer;

	[Header("On Move (damage per meter)")]
	public List<DamageTypeEntry> damageOnMove;

	public GameObjectRef effectOnMove;

	public HurtType hurtTypeOnMove;

	[Header("On Leave")]
	public List<DamageTypeEntry> damageOnLeave;

	public GameObjectRef effectOnLeave;

	public HurtType hurtTypeOnLeave;

	public bool damageEnabled = true;

	internal Dictionary<BaseEntity, EntityTriggerInfo> entityInfo;

	internal List<BaseEntity> entityAddList;

	internal List<BaseEntity> entityLeaveList;

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

	internal void DoDamage(BaseEntity ent, HurtType type, List<DamageTypeEntry> damage, GameObjectRef effect, float multiply = 1f)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (!damageEnabled)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("TriggerHurtEx.DoDamage", 0);
		try
		{
			if (damage != null && damage.Count > 0)
			{
				BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
				if (Object.op_Implicit((Object)(object)baseCombatEntity))
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(damage);
					hitInfo.damageTypes.ScaleAll(multiply);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.Initiator = ((Component)this).get_gameObject().ToBaseEntity();
					hitInfo.PointStart = ((Component)this).get_transform().get_position();
					hitInfo.PointEnd = ((Component)baseCombatEntity).get_transform().get_position();
					if (type == HurtType.Simple)
					{
						baseCombatEntity.Hurt(hitInfo);
					}
					else
					{
						baseCombatEntity.OnAttacked(hitInfo);
					}
				}
			}
			if (effect.isValid)
			{
				Effect.server.Run(effect.resourcePath, ent, StringPool.closest, ((Component)this).get_transform().get_position(), Vector3.get_up());
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (!((Object)(object)ent == (Object)null))
		{
			if (entityAddList == null)
			{
				entityAddList = new List<BaseEntity>();
			}
			entityAddList.Add(ent);
			((FacepunchBehaviour)this).Invoke((Action)ProcessQueues, 0.1f);
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (!((Object)(object)ent == (Object)null))
		{
			if (entityLeaveList == null)
			{
				entityLeaveList = new List<BaseEntity>();
			}
			entityLeaveList.Add(ent);
			((FacepunchBehaviour)this).Invoke((Action)ProcessQueues, 0.1f);
		}
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		ProcessQueues();
		if (entityInfo == null)
		{
			return;
		}
		KeyValuePair<BaseEntity, EntityTriggerInfo>[] array = entityInfo.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<BaseEntity, EntityTriggerInfo> keyValuePair = array[i];
			if (keyValuePair.Key.IsValid())
			{
				Vector3 position = ((Component)keyValuePair.Key).get_transform().get_position();
				Vector3 val = position - keyValuePair.Value.lastPosition;
				float magnitude = ((Vector3)(ref val)).get_magnitude();
				if (magnitude > 0.01f)
				{
					keyValuePair.Value.lastPosition = position;
					DoDamage(keyValuePair.Key, hurtTypeOnMove, damageOnMove, effectOnMove, magnitude);
				}
				DoDamage(keyValuePair.Key, hurtTypeOnTimer, damageOnTimer, effectOnTimer, repeatRate);
			}
		}
	}

	private void ProcessQueues()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (entityAddList != null)
		{
			foreach (BaseEntity entityAdd in entityAddList)
			{
				if (entityAdd.IsValid())
				{
					DoDamage(entityAdd, hurtTypeOnEnter, damageOnEnter, effectOnEnter);
					if (entityInfo == null)
					{
						entityInfo = new Dictionary<BaseEntity, EntityTriggerInfo>();
					}
					if (!entityInfo.ContainsKey(entityAdd))
					{
						entityInfo.Add(entityAdd, new EntityTriggerInfo
						{
							lastPosition = ((Component)entityAdd).get_transform().get_position()
						});
					}
				}
			}
			entityAddList = null;
		}
		if (entityLeaveList == null)
		{
			return;
		}
		foreach (BaseEntity entityLeave in entityLeaveList)
		{
			if (!entityLeave.IsValid())
			{
				continue;
			}
			DoDamage(entityLeave, hurtTypeOnLeave, damageOnLeave, effectOnLeave);
			if (entityInfo != null)
			{
				entityInfo.Remove(entityLeave);
				if (entityInfo.Count == 0)
				{
					entityInfo = null;
				}
			}
		}
		entityLeaveList.Clear();
	}
}
