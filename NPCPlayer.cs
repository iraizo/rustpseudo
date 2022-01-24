using System;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlayer : BasePlayer
{
	public AIInformationZone VirtualInfoZone;

	public Vector3 finalDestination;

	[NonSerialized]
	private float randomOffset;

	[NonSerialized]
	private Vector3 spawnPos;

	public PlayerInventoryProperties[] loadouts;

	public LayerMask movementMask = LayerMask.op_Implicit(429990145);

	public bool LegacyNavigation = true;

	public NavMeshAgent NavAgent;

	public float damageScale = 1f;

	public float shortRange = 10f;

	public float attackLengthMaxShortRangeScale = 1f;

	private bool _isDormant;

	protected float lastGunShotTime;

	private float triggerEndTime;

	protected float nextTriggerTime;

	private float lastThinkTime;

	private float lastPositionUpdateTime;

	private float lastMovementTickTime;

	private Vector3 lastPos;

	public override bool IsNpc => true;

	public virtual bool IsDormant
	{
		get
		{
			return _isDormant;
		}
		set
		{
			_isDormant = value;
			_ = _isDormant;
		}
	}

	protected override float PositionTickRate => 0.1f;

	public virtual bool IsOnNavMeshLink
	{
		get
		{
			if (IsNavRunning())
			{
				return NavAgent.get_isOnOffMeshLink();
			}
			return false;
		}
	}

	public virtual bool HasPath
	{
		get
		{
			if (IsNavRunning())
			{
				return NavAgent.get_hasPath();
			}
			return false;
		}
	}

	public virtual bool IsLoadBalanced()
	{
		return false;
	}

	public override void ServerInit()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		spawnPos = GetPosition();
		randomOffset = Random.Range(0f, 1f);
		base.ServerInit();
		UpdateNetworkGroup();
		if (loadouts != null && loadouts.Length != 0)
		{
			loadouts[Random.Range(0, loadouts.Length)].GiveToPlayer(this);
		}
		if (!IsLoadBalanced())
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)ServerThink_Internal, 0f, 0.1f);
			lastThinkTime = Time.get_time();
		}
		((FacepunchBehaviour)this).Invoke((Action)EquipTest, 0.25f);
		finalDestination = ((Component)this).get_transform().get_position();
		if ((Object)(object)NavAgent == (Object)null)
		{
			NavAgent = ((Component)this).GetComponent<NavMeshAgent>();
		}
		if (Object.op_Implicit((Object)(object)NavAgent))
		{
			NavAgent.set_updateRotation(false);
			NavAgent.set_updatePosition(false);
			if (!LegacyNavigation)
			{
				((Component)((Component)this).get_transform()).get_gameObject().GetComponent<BaseNavigator>().Init(this, NavAgent);
			}
		}
		((FacepunchBehaviour)this).InvokeRandomized((Action)TickMovement, 1f, PositionTickRate, PositionTickRate * 0.1f);
	}

	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	public void RandomMove()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = 8f;
		Vector2 val = Random.get_insideUnitCircle() * num;
		SetDestination(spawnPos + new Vector3(val.x, 0f, val.y));
	}

	public virtual void SetDestination(Vector3 newDestination)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		finalDestination = newDestination;
	}

	public AttackEntity GetAttackEntity()
	{
		return GetHeldEntity() as AttackEntity;
	}

	public BaseProjectile GetGun()
	{
		AttackEntity attackEntity = GetHeldEntity() as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return null;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (Object.op_Implicit((Object)(object)baseProjectile))
		{
			return baseProjectile;
		}
		return null;
	}

	public virtual float AmmoFractionRemaining()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (Object.op_Implicit((Object)(object)attackEntity))
		{
			return attackEntity.AmmoFraction();
		}
		return 0f;
	}

	public virtual bool IsReloading()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (!Object.op_Implicit((Object)(object)attackEntity))
		{
			return false;
		}
		return attackEntity.ServerIsReloading();
	}

	public virtual void AttemptReload()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (!((Object)(object)attackEntity == (Object)null) && attackEntity.CanReload())
		{
			attackEntity.ServerReload();
		}
	}

	public virtual bool ShotTest(float targetDist)
	{
		AttackEntity attackEntity = GetHeldEntity() as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return false;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (Object.op_Implicit((Object)(object)baseProjectile))
		{
			if (baseProjectile.primaryMagazine.contents <= 0)
			{
				baseProjectile.ServerReload();
				return false;
			}
			if (baseProjectile.NextAttackTime > Time.get_time())
			{
				return false;
			}
		}
		if (!Mathf.Approximately(attackEntity.attackLengthMin, -1f))
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)TriggerDown))
			{
				return true;
			}
			if (Time.get_time() < nextTriggerTime)
			{
				return true;
			}
			((FacepunchBehaviour)this).InvokeRepeating((Action)TriggerDown, 0f, 0.01f);
			if (targetDist <= shortRange)
			{
				triggerEndTime = Time.get_time() + Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax * attackLengthMaxShortRangeScale);
			}
			else
			{
				triggerEndTime = Time.get_time() + Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax);
			}
			TriggerDown();
			return true;
		}
		attackEntity.ServerUse(damageScale);
		lastGunShotTime = Time.get_time();
		return true;
	}

	public virtual float GetAimConeScale()
	{
		return 1f;
	}

	public void CancelBurst(float delay = 0.2f)
	{
		if (triggerEndTime > Time.get_time() + delay)
		{
			triggerEndTime = Time.get_time() + delay;
		}
	}

	public bool MeleeAttack()
	{
		AttackEntity attackEntity = GetHeldEntity() as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return false;
		}
		BaseMelee baseMelee = attackEntity as BaseMelee;
		if ((Object)(object)baseMelee == (Object)null)
		{
			return false;
		}
		baseMelee.ServerUse(damageScale);
		return true;
	}

	public virtual void TriggerDown()
	{
		AttackEntity attackEntity = GetHeldEntity() as AttackEntity;
		if ((Object)(object)attackEntity != (Object)null)
		{
			attackEntity.ServerUse(damageScale);
		}
		lastGunShotTime = Time.get_time();
		if (Time.get_time() > triggerEndTime)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TriggerDown);
			nextTriggerTime = Time.get_time() + (((Object)(object)attackEntity != (Object)null) ? attackEntity.attackSpacing : 1f);
		}
	}

	public virtual void EquipWeapon()
	{
		if ((Object)(object)inventory == (Object)null || inventory.containerBelt == null)
		{
			return;
		}
		Item slot = inventory.containerBelt.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		UpdateActiveItem(inventory.containerBelt.GetSlot(0).uid);
		BaseEntity heldEntity = slot.GetHeldEntity();
		if ((Object)(object)heldEntity != (Object)null)
		{
			AttackEntity component = ((Component)heldEntity).GetComponent<AttackEntity>();
			if ((Object)(object)component != (Object)null)
			{
				component.TopUpAmmo();
			}
		}
	}

	public void EquipTest()
	{
		EquipWeapon();
	}

	internal void ServerThink_Internal()
	{
		float delta = Time.get_time() - lastThinkTime;
		ServerThink(delta);
		lastThinkTime = Time.get_time();
	}

	public virtual void ServerThink(float delta)
	{
		TickAi(delta);
	}

	public virtual void Resume()
	{
	}

	public virtual bool IsNavRunning()
	{
		return false;
	}

	public virtual void TickAi(float delta)
	{
	}

	public void TickMovement()
	{
		float delta = Time.get_realtimeSinceStartup() - lastMovementTickTime;
		lastMovementTickTime = Time.get_realtimeSinceStartup();
		MovementUpdate(delta);
	}

	public override float GetNetworkTime()
	{
		if (Time.get_realtimeSinceStartup() - lastPositionUpdateTime > PositionTickRate * 2f)
		{
			return Time.get_time();
		}
		return lastPositionUpdateTime;
	}

	public virtual void MovementUpdate(float delta)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!LegacyNavigation || base.isClient || !IsAlive() || IsWounded() || (!base.isMounted && !IsNavRunning()))
		{
			return;
		}
		if (IsDormant || !syncPosition)
		{
			if (IsNavRunning())
			{
				NavAgent.set_destination(ServerPosition);
			}
			return;
		}
		Vector3 moveToPosition = ((Component)this).get_transform().get_position();
		if (HasPath)
		{
			moveToPosition = NavAgent.get_nextPosition();
		}
		if (ValidateNextPosition(ref moveToPosition))
		{
			UpdateSpeed(delta);
			UpdatePositionAndRotation(moveToPosition);
		}
	}

	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!ValidBounds.Test(moveToPosition) && (Object)(object)((Component)this).get_transform() != (Object)null && !base.IsDestroyed)
		{
			Debug.Log((object)string.Concat("Invalid NavAgent Position: ", this, " ", ((object)(Vector3)(ref moveToPosition)).ToString(), " (destroying)"));
			Kill();
			return false;
		}
		return true;
	}

	private void UpdateSpeed(float delta)
	{
		float num = DesiredMoveSpeed();
		NavAgent.set_speed(Mathf.Lerp(NavAgent.get_speed(), num, delta * 8f));
	}

	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		lastPositionUpdateTime = Time.get_time();
		ServerPosition = moveToPosition;
		SetAimDirection(GetAimDirection());
	}

	public Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position();
	}

	public virtual float DesiredMoveSpeed()
	{
		float running = Mathf.Sin(Time.get_time() + randomOffset);
		return GetSpeed(running, 0f, 0f);
	}

	public override bool EligibleForWounding(HitInfo info)
	{
		return false;
	}

	public virtual Vector3 GetAimDirection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.Distance2D(finalDestination, GetPosition()) >= 1f)
		{
			Vector3 val = finalDestination - GetPosition();
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			return new Vector3(normalized.x, 0f, normalized.z);
		}
		return eyes.BodyForward();
	}

	public virtual void SetAimDirection(Vector3 newAim)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!(newAim == Vector3.get_zero()))
		{
			AttackEntity attackEntity = GetAttackEntity();
			if (Object.op_Implicit((Object)(object)attackEntity))
			{
				newAim = attackEntity.ModifyAIAim(newAim);
			}
			eyes.rotation = Quaternion.LookRotation(newAim, Vector3.get_up());
			Quaternion rotation = eyes.rotation;
			viewAngles = ((Quaternion)(ref rotation)).get_eulerAngles();
			ServerRotation = eyes.rotation;
			lastPositionUpdateTime = Time.get_time();
		}
	}
}
