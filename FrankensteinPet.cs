using System;
using System.Collections;
using Network;
using Rust;
using UnityEngine;

public class FrankensteinPet : BasePet, IAISenses, IAIAttack
{
	[Header("Frankenstein")]
	[ServerVar(Help = "How long before a Frankenstein Pet dies un controlled and not asleep on table")]
	public static float decayminutes = 180f;

	[Header("Audio")]
	public SoundDefinition AttackVocalSFX;

	private float nextAttackTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FrankensteinPet.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!base.isClient)
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)TickDecay, Random.Range(30f, 60f), 60f, 6f);
		}
	}

	public IEnumerator DelayEquipWeapon(ItemDefinition item, float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		if (!((Object)(object)inventory == (Object)null) && inventory.containerBelt != null && !((Object)(object)item == (Object)null))
		{
			inventory.GiveItem(ItemManager.Create(item, 1, 0uL), inventory.containerBelt);
			EquipWeapon();
		}
	}

	private void TickDecay()
	{
		BasePlayer basePlayer = BasePlayer.FindByID(base.OwnerID);
		if ((!((Object)(object)basePlayer != (Object)null) || basePlayer.IsSleeping()) && !(base.healthFraction <= 0f) && !base.IsDestroyed)
		{
			float num = 1f / decayminutes;
			float amount = MaxHealth() * num;
			Hurt(amount, DamageType.Decay, this, useProtection: false);
		}
	}

	public float EngagementRange()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (Object.op_Implicit((Object)(object)attackEntity))
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * base.Brain.AttackRangeMultiplier;
		}
		return base.Brain.SenseRange;
	}

	public bool IsThreat(BaseEntity entity)
	{
		return IsTarget(entity);
	}

	public bool IsTarget(BaseEntity entity)
	{
		if (entity is BasePlayer)
		{
			return !entity.IsNpc;
		}
		return false;
	}

	public bool IsFriendly(BaseEntity entity)
	{
		return false;
	}

	public bool CanAttack(BaseEntity entity)
	{
		if ((Object)(object)entity == (Object)null)
		{
			return false;
		}
		if (((Component)entity).get_gameObject().get_layer() == 21 || ((Component)entity).get_gameObject().get_layer() == 8)
		{
			return false;
		}
		if (NeedsToReload())
		{
			return false;
		}
		if (IsOnCooldown())
		{
			return false;
		}
		if (!IsTargetInRange(entity, out var _))
		{
			return false;
		}
		BasePlayer basePlayer;
		if (InSafeZone() || ((basePlayer = entity as BasePlayer) != null && basePlayer.InSafeZone()))
		{
			return false;
		}
		if (!CanSeeTarget(entity))
		{
			return false;
		}
		return true;
	}

	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		dist = Vector3.Distance(((Component)entity).get_transform().get_position(), ((Component)this).get_transform().get_position());
		return dist <= EngagementRange();
	}

	public bool CanSeeTarget(BaseEntity entity)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)entity == (Object)null)
		{
			return false;
		}
		return entity.IsVisible(GetEntity().CenterPoint(), entity.CenterPoint());
	}

	public bool NeedsToReload()
	{
		return false;
	}

	public bool Reload()
	{
		return true;
	}

	public float CooldownDuration()
	{
		return BaseAttackRate;
	}

	public bool IsOnCooldown()
	{
		return Time.get_realtimeSinceStartup() < nextAttackTime;
	}

	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if ((Object)(object)baseCombatEntity == (Object)null)
		{
			return false;
		}
		Attack(baseCombatEntity);
		return true;
	}

	private void Attack(BaseCombatEntity target)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)target == (Object)null))
		{
			Vector3 val = target.ServerPosition - ServerPosition;
			if (((Vector3)(ref val)).get_magnitude() > 0.001f)
			{
				ServerRotation = Quaternion.LookRotation(((Vector3)(ref val)).get_normalized());
			}
			target.Hurt(BaseAttackDamge, AttackDamageType, this);
			SignalBroadcast(Signal.Attack);
			ClientRPC(null, "OnAttack");
			nextAttackTime = Time.get_realtimeSinceStartup() + CooldownDuration();
		}
	}

	public void StopAttacking()
	{
	}

	public float GetAmmoFraction()
	{
		return AmmoFractionRemaining();
	}

	public BaseEntity GetBestTarget()
	{
		return null;
	}

	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public override BaseCorpse CreateCorpse()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Create corpse", 0);
		try
		{
			NPCPlayerCorpse nPCPlayerCorpse = DropCorpse("assets/rust.ai/agents/NPCPlayer/pet/frankensteinpet_corpse.prefab") as NPCPlayerCorpse;
			if (Object.op_Implicit((Object)(object)nPCPlayerCorpse))
			{
				((Component)nPCPlayerCorpse).get_transform().set_position(((Component)nPCPlayerCorpse).get_transform().get_position() + Vector3.get_down() * NavAgent.get_baseOffset());
				nPCPlayerCorpse.SetLootableIn(2f);
				nPCPlayerCorpse.SetFlag(Flags.Reserved5, HasPlayerFlag(PlayerFlags.DisplaySash));
				nPCPlayerCorpse.SetFlag(Flags.Reserved2, b: true);
				nPCPlayerCorpse.TakeFrom(inventory.containerMain, inventory.containerWear, inventory.containerBelt);
				nPCPlayerCorpse.playerName = OverrideCorpseName();
				nPCPlayerCorpse.playerSteamID = userID;
				nPCPlayerCorpse.Spawn();
				ItemContainer[] containers = nPCPlayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
			}
			return nPCPlayerCorpse;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	protected virtual string OverrideCorpseName()
	{
		return "Frankenstein";
	}
}
