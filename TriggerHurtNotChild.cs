using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

public class TriggerHurtNotChild : TriggerBase, IServerComponent, IHurtTrigger
{
	public interface IHurtTriggerUser
	{
		BasePlayer GetPlayerDamageInitiator();

		float GetPlayerDamageMultiplier();

		void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal);
	}

	public float DamagePerSecond = 1f;

	public float DamageTickRate = 4f;

	public float DamageDelay;

	public DamageType damageType;

	public bool ignoreNPC = true;

	public float npcMultiplier = 1f;

	public float resourceMultiplier = 1f;

	public bool triggerHitImpacts = true;

	public bool RequireUpAxis;

	public BaseEntity SourceEntity;

	public bool UseSourceEntityDamageMultiplier = true;

	public bool ignoreAllVehicleMounted;

	private Dictionary<BaseEntity, float> entryTimes;

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
		if (ignoreNPC && baseEntity.IsNpc)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	internal override void OnObjects()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)OnTick, 0f, 1f / DamageTickRate);
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if ((Object)(object)ent != (Object)null && DamageDelay > 0f)
		{
			if (entryTimes == null)
			{
				entryTimes = new Dictionary<BaseEntity, float>();
			}
			entryTimes.Add(ent, Time.get_time());
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		if ((Object)(object)ent != (Object)null && entryTimes != null)
		{
			entryTimes.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	internal override void OnEmpty()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
	}

	public new void OnDisable()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
		base.OnDisable();
	}

	private bool IsInterested(BaseEntity ent)
	{
		BasePlayer basePlayer = ent.ToPlayer();
		if ((Object)(object)basePlayer != (Object)null)
		{
			if (basePlayer.isMounted)
			{
				BaseVehicle mountedVehicle = basePlayer.GetMountedVehicle();
				if ((Object)(object)SourceEntity != (Object)null && (Object)(object)mountedVehicle == (Object)(object)SourceEntity)
				{
					return false;
				}
				if (ignoreAllVehicleMounted && (Object)(object)mountedVehicle != (Object)null)
				{
					return false;
				}
			}
			if ((Object)(object)SourceEntity != (Object)null && basePlayer.HasEntityInParents(SourceEntity))
			{
				return false;
			}
		}
		return true;
	}

	private void OnTick()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		if (((ICollection<BaseEntity>)entityContents).IsNullOrEmpty())
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		list.AddRange((IEnumerable<BaseEntity>)entityContents);
		IHurtTriggerUser hurtTriggerUser = SourceEntity as IHurtTriggerUser;
		foreach (BaseEntity item in list)
		{
			if (item.IsValid() && IsInterested(item) && (!(DamageDelay > 0f) || entryTimes == null || !entryTimes.TryGetValue(item, out var value) || !(value + DamageDelay > Time.get_time())) && (!RequireUpAxis || !(Vector3.Dot(((Component)item).get_transform().get_up(), ((Component)this).get_transform().get_up()) < 0f)))
			{
				float num = DamagePerSecond * 1f / DamageTickRate;
				if (UseSourceEntityDamageMultiplier && hurtTriggerUser != null)
				{
					num *= hurtTriggerUser.GetPlayerDamageMultiplier();
				}
				if (item.IsNpc)
				{
					num *= npcMultiplier;
				}
				if (item is ResourceEntity)
				{
					num *= resourceMultiplier;
				}
				Vector3 val = ((Component)item).get_transform().get_position() + Vector3.get_up() * 1f;
				HitInfo hitInfo = new HitInfo
				{
					DoHitEffects = true,
					HitEntity = item,
					HitPositionWorld = val,
					HitPositionLocal = ((Component)item).get_transform().InverseTransformPoint(val),
					HitNormalWorld = Vector3.get_up(),
					HitMaterial = ((item is BaseCombatEntity) ? StringPool.Get("Flesh") : 0u),
					Initiator = hurtTriggerUser?.GetPlayerDamageInitiator()
				};
				hitInfo.damageTypes = new DamageTypeList();
				hitInfo.damageTypes.Set(damageType, num);
				item.OnAttacked(hitInfo);
				hurtTriggerUser?.OnHurtTriggerOccupant(item, damageType, num);
				if (triggerHitImpacts)
				{
					Effect.server.ImpactEffect(hitInfo);
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		RemoveInvalidEntities();
	}
}
