using System;
using System.Collections;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class BaseNpc : BaseCombatEntity
{
	[Flags]
	public enum AiFlags
	{
		Sitting = 0x2,
		Chasing = 0x4,
		Sleeping = 0x8
	}

	public enum Facts
	{
		HasEnemy,
		EnemyRange,
		CanTargetEnemies,
		Health,
		Speed,
		IsTired,
		IsSleeping,
		IsAttackReady,
		IsRoamReady,
		IsAggro,
		WantsToFlee,
		IsHungry,
		FoodRange,
		AttackedLately,
		LoudNoiseNearby,
		CanTargetFood,
		IsMoving,
		IsFleeing,
		IsEating,
		IsAfraid,
		AfraidRange,
		IsUnderHealthThreshold,
		CanNotMove,
		PathToTargetStatus
	}

	public enum EnemyRangeEnum : byte
	{
		AttackRange,
		AggroRange,
		AwareRange,
		OutOfRange
	}

	public enum FoodRangeEnum : byte
	{
		EatRange,
		AwareRange,
		OutOfRange
	}

	public enum AfraidRangeEnum : byte
	{
		InAfraidRange,
		OutOfRange
	}

	public enum HealthEnum : byte
	{
		Fine,
		Medium,
		Low
	}

	public enum SpeedEnum : byte
	{
		StandStill,
		Walk,
		Run
	}

	[Serializable]
	public struct AiStatistics
	{
		public enum FamilyEnum
		{
			Bear,
			Wolf,
			Deer,
			Boar,
			Chicken,
			Horse,
			Zombie,
			Scientist,
			Murderer,
			Player
		}

		[Tooltip("Ai will be less likely to fight animals that are larger than them, and more likely to flee from them.")]
		[Range(0f, 1f)]
		public float Size;

		[Tooltip("How fast we can move")]
		public float Speed;

		[Tooltip("How fast can we accelerate")]
		public float Acceleration;

		[Tooltip("How fast can we turn around")]
		public float TurnSpeed;

		[Tooltip("Determines things like how near we'll allow other species to get")]
		[Range(0f, 1f)]
		public float Tolerance;

		[Tooltip("How far this NPC can see")]
		public float VisionRange;

		[Tooltip("Our vision cone for dot product - a value of -1 means we can see all around us, 0 = only infront ")]
		public float VisionCone;

		[Tooltip("NPCs use distance visibility to basically make closer enemies easier to detect than enemies further away")]
		public AnimationCurve DistanceVisibility;

		[Tooltip("How likely are we to be offensive without being threatened")]
		public float Hostility;

		[Tooltip("How likely are we to defend ourselves when attacked")]
		public float Defensiveness;

		[Tooltip("The range at which we will engage targets")]
		public float AggressionRange;

		[Tooltip("The range at which an aggrified npc will disengage it's current target")]
		public float DeaggroRange;

		[Tooltip("For how long will we chase a target until we give up")]
		public float DeaggroChaseTime;

		[Tooltip("When we deaggro, how long do we wait until we can aggro again.")]
		public float DeaggroCooldown;

		[Tooltip("The threshold of our health fraction where there's a chance that we want to flee")]
		public float HealthThresholdForFleeing;

		[Tooltip("The chance that we will flee when our health threshold is triggered")]
		public float HealthThresholdFleeChance;

		[Tooltip("When we flee, what is the minimum distance we should flee?")]
		public float MinFleeRange;

		[Tooltip("When we flee, what is the maximum distance we should flee?")]
		public float MaxFleeRange;

		[Tooltip("When we flee, what is the maximum time that can pass until we stop?")]
		public float MaxFleeTime;

		[Tooltip("At what range we are afraid of a target that is in our Is Afraid Of list.")]
		public float AfraidRange;

		[Tooltip("The family this npc belong to. Npcs in the same family will not attack each other.")]
		public FamilyEnum Family;

		[Tooltip("List of the types of Npc that we are afraid of.")]
		public FamilyEnum[] IsAfraidOf;

		[Tooltip("The minimum distance this npc will wander when idle.")]
		public float MinRoamRange;

		[Tooltip("The maximum distance this npc will wander when idle.")]
		public float MaxRoamRange;

		[Tooltip("The minimum amount of time between each time we seek a new roam destination (when idle)")]
		public float MinRoamDelay;

		[Tooltip("The maximum amount of time between each time we seek a new roam destination (when idle)")]
		public float MaxRoamDelay;

		[Tooltip("If an npc is mobile, they are allowed to move when idle.")]
		public bool IsMobile;

		[Tooltip("In the range between min and max roam delay, we evaluate the random value through this curve")]
		public AnimationCurve RoamDelayDistribution;

		[Tooltip("For how long do we remember that someone attacked us")]
		public float AttackedMemoryTime;

		[Tooltip("How long should we block movement to make the wakeup animation not look whack?")]
		public float WakeupBlockMoveTime;

		[Tooltip("The maximum water depth this npc willingly will walk into.")]
		public float MaxWaterDepth;

		[Tooltip("The water depth at which they will start swimming.")]
		public float WaterLevelNeck;

		public float WaterLevelNeckOffset;

		[Tooltip("The range we consider using close range weapons.")]
		public float CloseRange;

		[Tooltip("The range we consider using medium range weapons.")]
		public float MediumRange;

		[Tooltip("The range we consider using long range weapons.")]
		public float LongRange;

		[Tooltip("How long can we be out of range of our spawn point before we time out and make our way back home (when idle).")]
		public float OutOfRangeOfSpawnPointTimeout;

		[Tooltip("If this is set to true, then a target must hold special markers (like IsHostile) for the target to be considered for aggressive action.")]
		public bool OnlyAggroMarkedTargets;
	}

	public enum Behaviour
	{
		Idle,
		Wander,
		Attack,
		Flee,
		Eat,
		Sleep,
		RetreatingToCover
	}

	[NonSerialized]
	public Transform ChaseTransform;

	public int agentTypeIndex;

	public bool NewAI;

	public bool LegacyNavigation = true;

	private Vector3 stepDirection;

	private float maxFleeTime;

	private float fleeHealthThresholdPercentage = 1f;

	private float blockEnemyTargetingTimeout = float.NegativeInfinity;

	private float blockFoodTargetingTimeout = float.NegativeInfinity;

	private float aggroTimeout = float.NegativeInfinity;

	private float lastAggroChanceResult;

	private float lastAggroChanceCalcTime;

	private const float aggroChanceRecalcTimeout = 5f;

	private float eatTimeout = float.NegativeInfinity;

	private float wakeUpBlockMoveTimeout = float.NegativeInfinity;

	private BaseEntity blockTargetingThisEnemy;

	[NonSerialized]
	public float waterDepth;

	[NonSerialized]
	public bool swimming;

	[NonSerialized]
	public bool wasSwimming;

	private static readonly AnimationCurve speedFractionResponse = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	private bool _traversingNavMeshLink;

	private OffMeshLinkData _currentNavMeshLink;

	private string _currentNavMeshLinkName;

	private float _currentNavMeshLinkTraversalTime;

	private float _currentNavMeshLinkTraversalTimeDelta;

	private Quaternion _currentNavMeshLinkOrientation;

	private Vector3 _currentNavMeshLinkEndPos;

	private float nextAttackTime;

	[SerializeField]
	[InspectorFlags]
	public Enum topologyPreference = (Enum)96;

	[InspectorFlags]
	public AiFlags aiFlags;

	[NonSerialized]
	public byte[] CurrentFacts = new byte[Enum.GetValues(typeof(Facts)).Length];

	[Header("NPC Senses")]
	public int ForgetUnseenEntityTime = 10;

	public float SensesTickRate = 0.5f;

	[NonSerialized]
	public BaseEntity[] SensesResults = new BaseEntity[64];

	private float lastTickTime;

	private float playerTargetDecisionStartTime;

	private float animalTargetDecisionStartTime;

	private bool isAlreadyCheckingPathPending;

	private int numPathPendingAttempts;

	private float accumPathPendingDelay;

	public const float TickRate = 0.1f;

	private Vector3 lastStuckPos;

	private float nextFlinchTime;

	private float _lastHeardGunshotTime = float.NegativeInfinity;

	[Header("BaseNpc")]
	public GameObjectRef CorpsePrefab;

	public AiStatistics Stats;

	public Vector3 AttackOffset;

	public float AttackDamage = 20f;

	public DamageType AttackDamageType = DamageType.Bite;

	[Tooltip("Stamina to use per attack")]
	public float AttackCost = 0.1f;

	[Tooltip("How often can we attack")]
	public float AttackRate = 1f;

	[Tooltip("Maximum Distance for an attack")]
	public float AttackRange = 1f;

	public NavMeshAgent NavAgent;

	public LayerMask movementMask = LayerMask.op_Implicit(429990145);

	public float stuckDuration;

	public float lastStuckTime;

	public float idleDuration;

	private bool _isDormant;

	private float lastSetDestinationTime;

	[NonSerialized]
	public StateTimer BusyTimer;

	[NonSerialized]
	public float Sleep;

	[NonSerialized]
	public VitalLevel Stamina;

	[NonSerialized]
	public VitalLevel Energy;

	[NonSerialized]
	public VitalLevel Hydration;

	public int AgentTypeIndex
	{
		get
		{
			return agentTypeIndex;
		}
		set
		{
			agentTypeIndex = value;
		}
	}

	public bool IsStuck { get; set; }

	public bool AgencyUpdateRequired { get; set; }

	public bool IsOnOffmeshLinkAndReachedNewCoord { get; set; }

	public float GetAttackRate => AttackRate;

	public bool IsSitting
	{
		get
		{
			return HasAiFlag(AiFlags.Sitting);
		}
		set
		{
			SetAiFlag(AiFlags.Sitting, value);
		}
	}

	public bool IsChasing
	{
		get
		{
			return HasAiFlag(AiFlags.Chasing);
		}
		set
		{
			SetAiFlag(AiFlags.Chasing, value);
		}
	}

	public bool IsSleeping
	{
		get
		{
			return HasAiFlag(AiFlags.Sleeping);
		}
		set
		{
			SetAiFlag(AiFlags.Sleeping, value);
		}
	}

	public float SecondsSinceLastHeardGunshot => Time.get_time() - _lastHeardGunshotTime;

	public Vector3 LastHeardGunshotDirection { get; set; }

	public float TargetSpeed { get; set; }

	public override bool IsNpc => true;

	public bool IsDormant
	{
		get
		{
			return _isDormant;
		}
		set
		{
			_isDormant = value;
			if (_isDormant)
			{
				StopMoving();
				Pause();
			}
			else if ((Object)(object)GetNavAgent == (Object)null || AiManager.nav_disable)
			{
				IsDormant = true;
			}
			else
			{
				Resume();
			}
		}
	}

	public float SecondsSinceLastSetDestination => Time.get_time() - lastSetDestinationTime;

	public float LastSetDestinationTime => lastSetDestinationTime;

	public Vector3 Destination
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (IsNavRunning())
			{
				return GetNavAgent.get_destination();
			}
			return Entity.ServerPosition;
		}
		set
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (IsNavRunning())
			{
				GetNavAgent.set_destination(value);
				lastSetDestinationTime = Time.get_time();
			}
		}
	}

	public bool IsStopped
	{
		get
		{
			if (IsNavRunning())
			{
				return GetNavAgent.get_isStopped();
			}
			return true;
		}
		set
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (IsNavRunning())
			{
				if (value)
				{
					GetNavAgent.set_destination(ServerPosition);
				}
				GetNavAgent.set_isStopped(value);
			}
		}
	}

	public bool AutoBraking
	{
		get
		{
			if (IsNavRunning())
			{
				return GetNavAgent.get_autoBraking();
			}
			return false;
		}
		set
		{
			if (IsNavRunning())
			{
				GetNavAgent.set_autoBraking(value);
			}
		}
	}

	public bool HasPath
	{
		get
		{
			if (IsNavRunning())
			{
				return GetNavAgent.get_hasPath();
			}
			return false;
		}
	}

	public BaseEntity AttackTarget { get; set; }

	public Memory.SeenInfo AttackTargetMemory { get; set; }

	public BaseEntity FoodTarget { get; set; }

	public BaseCombatEntity CombatTarget => AttackTarget as BaseCombatEntity;

	public Vector3 SpawnPosition { get; set; }

	public float AttackTargetVisibleFor => 0f;

	public float TimeAtDestination => 0f;

	public BaseCombatEntity Entity => this;

	public NavMeshAgent GetNavAgent
	{
		get
		{
			if (base.isClient)
			{
				return null;
			}
			if ((Object)(object)NavAgent == (Object)null)
			{
				NavAgent = ((Component)this).GetComponent<NavMeshAgent>();
				if ((Object)(object)NavAgent == (Object)null)
				{
					Debug.LogErrorFormat("{0} has no nav agent!", new object[1] { ((Object)this).get_name() });
				}
			}
			return NavAgent;
		}
	}

	public AiStatistics GetStats => Stats;

	public float GetAttackRange => AttackRange;

	public Vector3 GetAttackOffset => AttackOffset;

	public float GetStamina => Stamina.Level;

	public float GetEnergy => Energy.Level;

	public float GetAttackCost => AttackCost;

	public float GetSleep => Sleep;

	public Vector3 CurrentAimAngles => ((Component)this).get_transform().get_forward();

	public float GetStuckDuration => stuckDuration;

	public float GetLastStuckTime => lastStuckTime;

	public Vector3 AttackPosition => ServerPosition + ((Component)this).get_transform().TransformDirection(AttackOffset);

	public Vector3 CrouchedAttackPosition => AttackPosition;

	public float currentBehaviorDuration => 0f;

	public Behaviour CurrentBehaviour { get; set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseNpc.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void UpdateDestination(Vector3 position)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (IsStopped)
		{
			IsStopped = false;
		}
		Vector3 val = Destination - position;
		if (((Vector3)(ref val)).get_sqrMagnitude() > 0.010000001f)
		{
			Destination = position;
		}
		ChaseTransform = null;
	}

	public void UpdateDestination(Transform tx)
	{
		IsStopped = false;
		ChaseTransform = tx;
	}

	public void StopMoving()
	{
		IsStopped = true;
		ChaseTransform = null;
		SetFact(Facts.PathToTargetStatus, 0);
	}

	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		ServerPosition = GetNewNavPosWithVelocity(this, velocity);
	}

	public static Vector3 GetNewNavPosWithVelocity(BaseEntity ent, Vector3 velocity)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = ent.GetParentEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			velocity = ((Component)baseEntity).get_transform().InverseTransformDirection(velocity);
		}
		Vector3 val = ent.ServerPosition + velocity * Time.get_fixedDeltaTime();
		NavMeshHit val2 = default(NavMeshHit);
		NavMesh.Raycast(ent.ServerPosition, val, ref val2, -1);
		if (!Vector3Ex.IsNaNOrInfinity(((NavMeshHit)(ref val2)).get_position()))
		{
			return ((NavMeshHit)(ref val2)).get_position();
		}
		return ent.ServerPosition;
	}

	public override string DebugText()
	{
		return string.Concat(string.Concat(string.Concat(base.DebugText() + $"\nBehaviour: {CurrentBehaviour}", $"\nAttackTarget: {AttackTarget}"), $"\nFoodTarget: {FoodTarget}"), $"\nSleep: {Sleep:0.00}");
	}

	public void TickAi()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.think)
		{
			return;
		}
		if ((Object)(object)TerrainMeta.WaterMap != (Object)null)
		{
			waterDepth = TerrainMeta.WaterMap.GetDepth(ServerPosition);
			wasSwimming = swimming;
			swimming = waterDepth > Stats.WaterLevelNeck * 0.25f;
		}
		else
		{
			wasSwimming = false;
			swimming = false;
			waterDepth = 0f;
		}
		TimeWarning val = TimeWarning.New("TickNavigation", 0);
		try
		{
			TickNavigation();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (!AiManager.ai_dormant || ((Behaviour)GetNavAgent).get_enabled() || CurrentBehaviour == Behaviour.Sleep || NewAI)
		{
			val = TimeWarning.New("TickMetabolism", 0);
			try
			{
				TickSleep();
				TickMetabolism();
				TickSpeed();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private void TickSpeed()
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (!LegacyNavigation)
		{
			return;
		}
		float speed = Stats.Speed;
		if (NewAI)
		{
			speed = (swimming ? ToSpeed(SpeedEnum.Walk) : TargetSpeed);
			speed *= 0.5f + base.healthFraction * 0.5f;
			NavAgent.set_speed(Mathf.Lerp(NavAgent.get_speed(), speed, 0.5f));
			NavAgent.set_angularSpeed(Stats.TurnSpeed);
			NavAgent.set_acceleration(Stats.Acceleration);
			return;
		}
		speed *= 0.5f + base.healthFraction * 0.5f;
		if (CurrentBehaviour == Behaviour.Idle)
		{
			speed *= 0.2f;
		}
		if (CurrentBehaviour == Behaviour.Eat)
		{
			speed *= 0.3f;
		}
		float num = Mathf.Min(NavAgent.get_speed() / Stats.Speed, 1f);
		num = speedFractionResponse.Evaluate(num);
		Vector3 forward = ((Component)this).get_transform().get_forward();
		Vector3 val = NavAgent.get_nextPosition() - ServerPosition;
		float num2 = 1f - 0.9f * Vector3.Angle(forward, ((Vector3)(ref val)).get_normalized()) / 180f * num * num;
		speed *= num2;
		NavAgent.set_speed(Mathf.Lerp(NavAgent.get_speed(), speed, 0.5f));
		NavAgent.set_angularSpeed(Stats.TurnSpeed * (1.1f - num));
		NavAgent.set_acceleration(Stats.Acceleration);
	}

	protected virtual void TickMetabolism()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.00016666666f;
		if (CurrentBehaviour == Behaviour.Sleep)
		{
			num *= 0.01f;
		}
		Vector3 desiredVelocity = NavAgent.get_desiredVelocity();
		if (((Vector3)(ref desiredVelocity)).get_sqrMagnitude() > 0.1f)
		{
			num *= 2f;
		}
		Energy.Add(num * 0.1f * -1f);
		if (Stamina.TimeSinceUsed > 5f)
		{
			float num2 = 71f / (339f * (float)Math.PI);
			Stamina.Add(0.1f * num2);
		}
		_ = base.SecondsSinceAttacked;
		_ = 60f;
	}

	public virtual bool WantsToEat(BaseEntity best)
	{
		if (!best.HasTrait(TraitFlag.Food))
		{
			return false;
		}
		if (best.HasTrait(TraitFlag.Alive))
		{
			return false;
		}
		return true;
	}

	public virtual float FearLevel(BaseEntity ent)
	{
		float num = 0f;
		BaseNpc baseNpc = ent as BaseNpc;
		if ((Object)(object)baseNpc != (Object)null && baseNpc.Stats.Size > Stats.Size)
		{
			if (baseNpc.WantsToAttack(this) > 0.25f)
			{
				num += 0.2f;
			}
			if ((Object)(object)baseNpc.AttackTarget == (Object)(object)this)
			{
				num += 0.3f;
			}
			if (baseNpc.CurrentBehaviour == Behaviour.Attack)
			{
				num *= 1.5f;
			}
			if (baseNpc.CurrentBehaviour == Behaviour.Sleep)
			{
				num *= 0.1f;
			}
		}
		if ((Object)(object)(ent as BasePlayer) != (Object)null)
		{
			num += 1f;
		}
		return num;
	}

	public virtual float HateLevel(BaseEntity ent)
	{
		return 0f;
	}

	protected virtual void TickSleep()
	{
		if (CurrentBehaviour == Behaviour.Sleep)
		{
			IsSleeping = true;
			Sleep += 0.00033333336f;
		}
		else
		{
			IsSleeping = false;
			Sleep -= 2.7777778E-05f;
		}
		Sleep = Mathf.Clamp01(Sleep);
	}

	public void TickNavigationWater()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!LegacyNavigation || !AI.move || !IsNavRunning())
		{
			return;
		}
		if (IsDormant || !syncPosition)
		{
			StopMoving();
			return;
		}
		Vector3 moveToPosition = ((Component)this).get_transform().get_position();
		stepDirection = Vector3.get_zero();
		if (Object.op_Implicit((Object)(object)ChaseTransform))
		{
			TickChase();
		}
		if (NavAgent.get_isOnOffMeshLink())
		{
			HandleNavMeshLinkTraversal(0.1f, ref moveToPosition);
		}
		else if (NavAgent.get_hasPath())
		{
			TickFollowPath(ref moveToPosition);
		}
		if (ValidateNextPosition(ref moveToPosition))
		{
			moveToPosition.y = 0f - Stats.WaterLevelNeck;
			UpdatePositionAndRotation(moveToPosition);
			TickIdle();
			TickStuck();
		}
	}

	public void TickNavigation()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!LegacyNavigation || !AI.move || !IsNavRunning())
		{
			return;
		}
		if (IsDormant || !syncPosition)
		{
			StopMoving();
			return;
		}
		Vector3 moveToPosition = ((Component)this).get_transform().get_position();
		stepDirection = Vector3.get_zero();
		if (Object.op_Implicit((Object)(object)ChaseTransform))
		{
			TickChase();
		}
		if (NavAgent.get_isOnOffMeshLink())
		{
			HandleNavMeshLinkTraversal(0.1f, ref moveToPosition);
		}
		else if (NavAgent.get_hasPath())
		{
			TickFollowPath(ref moveToPosition);
		}
		if (ValidateNextPosition(ref moveToPosition))
		{
			UpdatePositionAndRotation(moveToPosition);
			TickIdle();
			TickStuck();
		}
	}

	private void TickChase()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ChaseTransform.get_position();
		Vector3 val2 = ((Component)this).get_transform().get_position() - val;
		if ((double)((Vector3)(ref val2)).get_magnitude() < 5.0)
		{
			val += ((Vector3)(ref val2)).get_normalized() * AttackOffset.z;
		}
		Vector3 val3 = NavAgent.get_destination() - val;
		if (((Vector3)(ref val3)).get_sqrMagnitude() > 0.010000001f)
		{
			NavAgent.SetDestination(val);
		}
	}

	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (_traversingNavMeshLink || HandleNavMeshLinkTraversalStart(delta))
		{
			HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
			if (!IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
			{
				_currentNavMeshLinkTraversalTimeDelta += delta;
			}
		}
	}

	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		OffMeshLinkData currentOffMeshLinkData = NavAgent.get_currentOffMeshLinkData();
		if (!((OffMeshLinkData)(ref currentOffMeshLinkData)).get_valid() || !((OffMeshLinkData)(ref currentOffMeshLinkData)).get_activated() || (Object)(object)((OffMeshLinkData)(ref currentOffMeshLinkData)).get_offMeshLink() == (Object)null)
		{
			return false;
		}
		Vector3 val = ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos() - ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos();
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		normalized.y = 0f;
		Vector3 desiredVelocity = NavAgent.get_desiredVelocity();
		desiredVelocity.y = 0f;
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
		{
			CompleteNavMeshLink();
			return false;
		}
		_currentNavMeshLink = currentOffMeshLinkData;
		OffMeshLinkType linkType = ((OffMeshLinkData)(ref _currentNavMeshLink)).get_linkType();
		_currentNavMeshLinkName = ((object)(OffMeshLinkType)(ref linkType)).ToString();
		if (((OffMeshLinkData)(ref currentOffMeshLinkData)).get_offMeshLink().get_biDirectional())
		{
			val = ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos() - ServerPosition;
			if (((Vector3)(ref val)).get_sqrMagnitude() < 0.05f)
			{
				_currentNavMeshLinkEndPos = ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos();
				_currentNavMeshLinkOrientation = Quaternion.LookRotation(((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos() + Vector3.get_up() * (((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos().y - ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos().y) - ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos());
			}
			else
			{
				_currentNavMeshLinkEndPos = ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos();
				_currentNavMeshLinkOrientation = Quaternion.LookRotation(((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos() + Vector3.get_up() * (((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos().y - ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos().y) - ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos());
			}
		}
		else
		{
			_currentNavMeshLinkEndPos = ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos();
			_currentNavMeshLinkOrientation = Quaternion.LookRotation(((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos() + Vector3.get_up() * (((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos().y - ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_endPos().y) - ((OffMeshLinkData)(ref currentOffMeshLinkData)).get_startPos());
		}
		_traversingNavMeshLink = true;
		NavAgent.ActivateCurrentOffMeshLink(false);
		NavAgent.set_obstacleAvoidanceType((ObstacleAvoidanceType)0);
		float num = Mathf.Max(NavAgent.get_speed(), 2.8f);
		Vector3 val2 = ((OffMeshLinkData)(ref _currentNavMeshLink)).get_startPos() - ((OffMeshLinkData)(ref _currentNavMeshLink)).get_endPos();
		float magnitude = ((Vector3)(ref val2)).get_magnitude();
		_currentNavMeshLinkTraversalTime = magnitude / num;
		_currentNavMeshLinkTraversalTimeDelta = 0f;
		if (!(_currentNavMeshLinkName == "OpenDoorLink") && !(_currentNavMeshLinkName == "JumpRockLink"))
		{
			_ = _currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (_currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.Lerp(((OffMeshLinkData)(ref _currentNavMeshLink)).get_startPos(), ((OffMeshLinkData)(ref _currentNavMeshLink)).get_endPos(), _currentNavMeshLinkTraversalTimeDelta);
		}
		else if (_currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.Lerp(((OffMeshLinkData)(ref _currentNavMeshLink)).get_startPos(), ((OffMeshLinkData)(ref _currentNavMeshLink)).get_endPos(), _currentNavMeshLinkTraversalTimeDelta);
		}
		else if (_currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.Lerp(((OffMeshLinkData)(ref _currentNavMeshLink)).get_startPos(), ((OffMeshLinkData)(ref _currentNavMeshLink)).get_endPos(), _currentNavMeshLinkTraversalTimeDelta);
		}
		else
		{
			moveToPosition = Vector3.Lerp(((OffMeshLinkData)(ref _currentNavMeshLink)).get_startPos(), ((OffMeshLinkData)(ref _currentNavMeshLink)).get_endPos(), _currentNavMeshLinkTraversalTimeDelta);
		}
	}

	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (_currentNavMeshLinkTraversalTimeDelta >= _currentNavMeshLinkTraversalTime)
		{
			moveToPosition = ((OffMeshLinkData)(ref _currentNavMeshLink)).get_endPos();
			_traversingNavMeshLink = false;
			_currentNavMeshLink = default(OffMeshLinkData);
			_currentNavMeshLinkTraversalTime = 0f;
			_currentNavMeshLinkTraversalTimeDelta = 0f;
			_currentNavMeshLinkName = string.Empty;
			_currentNavMeshLinkOrientation = Quaternion.get_identity();
			CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	private void CompleteNavMeshLink()
	{
		NavAgent.ActivateCurrentOffMeshLink(true);
		NavAgent.CompleteOffMeshLink();
		NavAgent.set_isStopped(false);
		NavAgent.set_obstacleAvoidanceType((ObstacleAvoidanceType)4);
	}

	private void TickFollowPath(ref Vector3 moveToPosition)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		moveToPosition = NavAgent.get_nextPosition();
		Vector3 desiredVelocity = NavAgent.get_desiredVelocity();
		stepDirection = ((Vector3)(ref desiredVelocity)).get_normalized();
	}

	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!ValidBounds.Test(moveToPosition) && (Object)(object)((Component)this).get_transform() != (Object)null && !base.IsDestroyed)
		{
			Debug.Log((object)string.Concat("Invalid NavAgent Position: ", this, " ", moveToPosition, " (destroying)"));
			Kill();
			return false;
		}
		return true;
	}

	private void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ServerPosition = moveToPosition;
		UpdateAiRotation();
	}

	private void TickIdle()
	{
		if (CurrentBehaviour == Behaviour.Idle)
		{
			idleDuration += 0.1f;
		}
		else
		{
			idleDuration = 0f;
		}
	}

	public void TickStuck()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (IsNavRunning() && !NavAgent.get_isStopped())
		{
			Vector3 val = lastStuckPos - ServerPosition;
			if (((Vector3)(ref val)).get_sqrMagnitude() < 0.0625f && AttackReady())
			{
				stuckDuration += 0.1f;
				if (stuckDuration >= 5f && Mathf.Approximately(lastStuckTime, 0f))
				{
					lastStuckTime = Time.get_time();
					OnBecomeStuck();
				}
				return;
			}
		}
		stuckDuration = 0f;
		lastStuckPos = ServerPosition;
		if (Time.get_time() - lastStuckTime > 5f)
		{
			lastStuckTime = 0f;
			OnBecomeUnStuck();
		}
	}

	public void OnBecomeStuck()
	{
		IsStuck = true;
	}

	public void OnBecomeUnStuck()
	{
		IsStuck = false;
	}

	public void UpdateAiRotation()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		if (!IsNavRunning() || CurrentBehaviour == Behaviour.Sleep)
		{
			return;
		}
		if (_traversingNavMeshLink)
		{
			Vector3 val = (((Object)(object)ChaseTransform != (Object)null) ? (ChaseTransform.get_localPosition() - ServerPosition) : ((!((Object)(object)AttackTarget != (Object)null)) ? (NavAgent.get_destination() - ServerPosition) : (AttackTarget.ServerPosition - ServerPosition)));
			if (((Vector3)(ref val)).get_sqrMagnitude() > 1f)
			{
				val = _currentNavMeshLinkEndPos - ServerPosition;
			}
			if (((Vector3)(ref val)).get_sqrMagnitude() > 0.001f)
			{
				ServerRotation = _currentNavMeshLinkOrientation;
				return;
			}
		}
		else
		{
			Vector3 val2 = NavAgent.get_destination() - ServerPosition;
			if (((Vector3)(ref val2)).get_sqrMagnitude() > 1f)
			{
				Vector3 val3 = stepDirection;
				if (((Vector3)(ref val3)).get_sqrMagnitude() > 0.001f)
				{
					ServerRotation = Quaternion.LookRotation(val3);
					return;
				}
			}
		}
		if (Object.op_Implicit((Object)(object)ChaseTransform) && CurrentBehaviour == Behaviour.Attack)
		{
			Vector3 val4 = ChaseTransform.get_localPosition() - ServerPosition;
			float sqrMagnitude = ((Vector3)(ref val4)).get_sqrMagnitude();
			if (sqrMagnitude < 9f && sqrMagnitude > 0.001f)
			{
				ServerRotation = Quaternion.LookRotation(((Vector3)(ref val4)).get_normalized());
			}
		}
		else if (Object.op_Implicit((Object)(object)AttackTarget) && CurrentBehaviour == Behaviour.Attack)
		{
			Vector3 val5 = AttackTarget.ServerPosition - ServerPosition;
			float sqrMagnitude2 = ((Vector3)(ref val5)).get_sqrMagnitude();
			if (sqrMagnitude2 < 9f && sqrMagnitude2 > 0.001f)
			{
				ServerRotation = Quaternion.LookRotation(((Vector3)(ref val5)).get_normalized());
			}
		}
	}

	public bool AttackReady()
	{
		return Time.get_realtimeSinceStartup() >= nextAttackTime;
	}

	public virtual void StartAttack()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)AttackTarget) || !AttackReady())
		{
			return;
		}
		Vector3 val = AttackTarget.ServerPosition - ServerPosition;
		if (!(((Vector3)(ref val)).get_magnitude() > AttackRange))
		{
			nextAttackTime = Time.get_realtimeSinceStartup() + AttackRate;
			BaseCombatEntity combatTarget = CombatTarget;
			if (Object.op_Implicit((Object)(object)combatTarget))
			{
				combatTarget.Hurt(AttackDamage, AttackDamageType, this);
				Stamina.Use(AttackCost);
				BusyTimer.Activate(0.5f);
				SignalBroadcast(Signal.Attack);
				ClientRPC<Vector3>(null, "Attack", AttackTarget.ServerPosition);
			}
		}
	}

	public void Attack(BaseCombatEntity target)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)target == (Object)null))
		{
			Vector3 val = target.ServerPosition - ServerPosition;
			if (((Vector3)(ref val)).get_magnitude() > 0.001f)
			{
				ServerRotation = Quaternion.LookRotation(((Vector3)(ref val)).get_normalized());
			}
			nextAttackTime = Time.get_realtimeSinceStartup() + AttackRate;
			target.Hurt(AttackDamage, AttackDamageType, this);
			Stamina.Use(AttackCost);
			SignalBroadcast(Signal.Attack);
			ClientRPC<Vector3>(null, "Attack", target.ServerPosition);
		}
	}

	public virtual void Eat()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)FoodTarget))
		{
			BusyTimer.Activate(0.5f);
			FoodTarget.Eat(this, 0.5f);
			StartEating(Random.get_value() * 5f + 0.5f);
			ClientRPC<Vector3>(null, "Eat", ((Component)FoodTarget).get_transform().get_position());
		}
	}

	public virtual void AddCalories(float amount)
	{
		Energy.Add(amount / 1000f);
	}

	public virtual void Startled()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ClientRPC<Vector3>(null, "Startled", ((Component)this).get_transform().get_position());
	}

	private bool IsAfraid()
	{
		SetFact(Facts.IsAfraid, 0);
		return false;
	}

	protected bool IsAfraidOf(AiStatistics.FamilyEnum family)
	{
		AiStatistics.FamilyEnum[] isAfraidOf = Stats.IsAfraidOf;
		foreach (AiStatistics.FamilyEnum familyEnum in isAfraidOf)
		{
			if (family == familyEnum)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckHealthThresholdToFlee()
	{
		if (base.healthFraction > Stats.HealthThresholdForFleeing)
		{
			if (Stats.HealthThresholdForFleeing < 1f)
			{
				SetFact(Facts.IsUnderHealthThreshold, 0);
				return false;
			}
			if (GetFact(Facts.HasEnemy) == 1)
			{
				SetFact(Facts.IsUnderHealthThreshold, 0);
				return false;
			}
		}
		bool flag = Random.get_value() < Stats.HealthThresholdFleeChance;
		SetFact(Facts.IsUnderHealthThreshold, (byte)(flag ? 1u : 0u));
		return flag;
	}

	private void TickBehaviourState()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (GetFact(Facts.WantsToFlee) == 1 && IsNavRunning() && (int)NavAgent.get_pathStatus() == 0 && Time.get_realtimeSinceStartup() - (maxFleeTime - Stats.MaxFleeTime) > 0.5f)
		{
			TickFlee();
		}
		if (GetFact(Facts.CanTargetEnemies) == 0)
		{
			TickBlockEnemyTargeting();
		}
		if (GetFact(Facts.CanTargetFood) == 0)
		{
			TickBlockFoodTargeting();
		}
		if (GetFact(Facts.IsAggro) == 1)
		{
			TickAggro();
		}
		if (GetFact(Facts.IsEating) == 1)
		{
			TickEating();
		}
		if (GetFact(Facts.CanNotMove) == 1)
		{
			TickWakeUpBlockMove();
		}
	}

	private void WantsToFlee()
	{
		if (GetFact(Facts.WantsToFlee) != 1 && IsNavRunning())
		{
			SetFact(Facts.WantsToFlee, 1);
			maxFleeTime = Time.get_realtimeSinceStartup() + Stats.MaxFleeTime;
		}
	}

	private void TickFlee()
	{
	}

	public bool BlockEnemyTargeting(float timeout)
	{
		if (GetFact(Facts.CanTargetEnemies) == 0)
		{
			return false;
		}
		SetFact(Facts.CanTargetEnemies, 0);
		blockEnemyTargetingTimeout = Time.get_realtimeSinceStartup() + timeout;
		blockTargetingThisEnemy = AttackTarget;
		return true;
	}

	private void TickBlockEnemyTargeting()
	{
		if (GetFact(Facts.CanTargetEnemies) != 1 && Time.get_realtimeSinceStartup() > blockEnemyTargetingTimeout)
		{
			SetFact(Facts.CanTargetEnemies, 1);
		}
	}

	public bool BlockFoodTargeting(float timeout)
	{
		if (GetFact(Facts.CanTargetFood) == 0)
		{
			return false;
		}
		SetFact(Facts.CanTargetFood, 0);
		blockFoodTargetingTimeout = Time.get_realtimeSinceStartup() + timeout;
		return true;
	}

	private void TickBlockFoodTargeting()
	{
		if (GetFact(Facts.CanTargetFood) != 1 && Time.get_realtimeSinceStartup() > blockFoodTargetingTimeout)
		{
			SetFact(Facts.CanTargetFood, 1);
		}
	}

	public bool TryAggro(EnemyRangeEnum range)
	{
		if (Mathf.Approximately(Stats.Hostility, 0f) && Mathf.Approximately(Stats.Defensiveness, 0f))
		{
			return false;
		}
		if (GetFact(Facts.IsAggro) == 0 && (range == EnemyRangeEnum.AggroRange || range == EnemyRangeEnum.AttackRange))
		{
			float num = ((range == EnemyRangeEnum.AttackRange) ? 1f : Stats.Defensiveness);
			num = Mathf.Max(num, Stats.Hostility);
			if (Time.get_realtimeSinceStartup() > lastAggroChanceCalcTime + 5f)
			{
				lastAggroChanceResult = Random.get_value();
				lastAggroChanceCalcTime = Time.get_realtimeSinceStartup();
			}
			if (lastAggroChanceResult < num)
			{
				return StartAggro(Stats.DeaggroChaseTime);
			}
		}
		return false;
	}

	public bool StartAggro(float timeout)
	{
		if (GetFact(Facts.IsAggro) == 1)
		{
			return false;
		}
		SetFact(Facts.IsAggro, 1);
		aggroTimeout = Time.get_realtimeSinceStartup() + timeout;
		return true;
	}

	private void TickAggro()
	{
	}

	public bool StartEating(float timeout)
	{
		if (GetFact(Facts.IsEating) == 1)
		{
			return false;
		}
		SetFact(Facts.IsEating, 1);
		eatTimeout = Time.get_realtimeSinceStartup() + timeout;
		return true;
	}

	private void TickEating()
	{
		if (GetFact(Facts.IsEating) != 0 && Time.get_realtimeSinceStartup() > eatTimeout)
		{
			SetFact(Facts.IsEating, 0);
		}
	}

	public bool WakeUpBlockMove(float timeout)
	{
		if (GetFact(Facts.CanNotMove) == 1)
		{
			return false;
		}
		SetFact(Facts.CanNotMove, 1);
		wakeUpBlockMoveTimeout = Time.get_realtimeSinceStartup() + timeout;
		return true;
	}

	private void TickWakeUpBlockMove()
	{
		if (GetFact(Facts.CanNotMove) != 0 && Time.get_realtimeSinceStartup() > wakeUpBlockMoveTimeout)
		{
			SetFact(Facts.CanNotMove, 0);
		}
	}

	private void OnFactChanged(Facts fact, byte oldValue, byte newValue)
	{
		switch (fact)
		{
		case Facts.IsSleeping:
			if (newValue > 0)
			{
				CurrentBehaviour = Behaviour.Sleep;
				SetFact(Facts.CanTargetEnemies, 0, triggerCallback: false);
				SetFact(Facts.CanTargetFood, 0);
			}
			else
			{
				CurrentBehaviour = Behaviour.Idle;
				SetFact(Facts.CanTargetEnemies, 1);
				SetFact(Facts.CanTargetFood, 1);
				WakeUpBlockMove(Stats.WakeupBlockMoveTime);
				TickSenses();
			}
			break;
		case Facts.IsAggro:
			if (newValue > 0)
			{
				CurrentBehaviour = Behaviour.Attack;
			}
			else
			{
				BlockEnemyTargeting(Stats.DeaggroCooldown);
			}
			break;
		case Facts.FoodRange:
			if (newValue == 0)
			{
				CurrentBehaviour = Behaviour.Eat;
			}
			break;
		case Facts.Speed:
			switch (newValue)
			{
			case 0:
				StopMoving();
				CurrentBehaviour = Behaviour.Idle;
				break;
			case 1:
				IsStopped = false;
				CurrentBehaviour = Behaviour.Wander;
				break;
			default:
				IsStopped = false;
				break;
			}
			break;
		case Facts.IsEating:
			if (newValue == 0)
			{
				FoodTarget = null;
			}
			break;
		case Facts.CanTargetEnemies:
			if (newValue == 1)
			{
				blockTargetingThisEnemy = null;
			}
			break;
		}
	}

	public int TopologyPreference()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected I4, but got Unknown
		return (int)topologyPreference;
	}

	public bool HasAiFlag(AiFlags f)
	{
		return (aiFlags & f) == f;
	}

	public void SetAiFlag(AiFlags f, bool set)
	{
		AiFlags num = aiFlags;
		if (set)
		{
			aiFlags |= f;
		}
		else
		{
			aiFlags &= ~f;
		}
		if (num != aiFlags && base.isServer)
		{
			SendNetworkUpdate();
		}
	}

	public void InitFacts()
	{
		SetFact(Facts.CanTargetEnemies, 1);
		SetFact(Facts.CanTargetFood, 1);
	}

	public byte GetFact(Facts fact)
	{
		return CurrentFacts[(int)fact];
	}

	public void SetFact(Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
		byte b = CurrentFacts[(int)fact];
		CurrentFacts[(int)fact] = value;
		if (triggerCallback && value != b)
		{
			OnFactChanged(fact, b, value);
		}
	}

	public EnemyRangeEnum ToEnemyRangeEnum(float range)
	{
		if (range <= AttackRange)
		{
			return EnemyRangeEnum.AttackRange;
		}
		if (range <= Stats.AggressionRange)
		{
			return EnemyRangeEnum.AggroRange;
		}
		if (range >= Stats.DeaggroRange && GetFact(Facts.IsAggro) > 0)
		{
			return EnemyRangeEnum.OutOfRange;
		}
		if (range <= Stats.VisionRange)
		{
			return EnemyRangeEnum.AwareRange;
		}
		return EnemyRangeEnum.OutOfRange;
	}

	public float GetActiveAggressionRangeSqr()
	{
		if (GetFact(Facts.IsAggro) == 1)
		{
			return Stats.DeaggroRange * Stats.DeaggroRange;
		}
		return Stats.AggressionRange * Stats.AggressionRange;
	}

	public FoodRangeEnum ToFoodRangeEnum(float range)
	{
		if (range <= 0.5f)
		{
			return FoodRangeEnum.EatRange;
		}
		if (range <= Stats.VisionRange)
		{
			return FoodRangeEnum.AwareRange;
		}
		return FoodRangeEnum.OutOfRange;
	}

	public AfraidRangeEnum ToAfraidRangeEnum(float range)
	{
		if (range <= Stats.AfraidRange)
		{
			return AfraidRangeEnum.InAfraidRange;
		}
		return AfraidRangeEnum.OutOfRange;
	}

	public HealthEnum ToHealthEnum(float healthNormalized)
	{
		if (healthNormalized >= 0.75f)
		{
			return HealthEnum.Fine;
		}
		if (healthNormalized >= 0.25f)
		{
			return HealthEnum.Medium;
		}
		return HealthEnum.Low;
	}

	public byte ToIsTired(float energyNormalized)
	{
		bool flag = GetFact(Facts.IsSleeping) == 1;
		if (!flag && energyNormalized < 0.1f)
		{
			return 1;
		}
		if (flag && energyNormalized < 0.5f)
		{
			return 1;
		}
		return 0;
	}

	public SpeedEnum ToSpeedEnum(float speed)
	{
		if (speed <= 0.01f)
		{
			return SpeedEnum.StandStill;
		}
		if (speed <= 0.18f)
		{
			return SpeedEnum.Walk;
		}
		return SpeedEnum.Run;
	}

	public float ToSpeed(SpeedEnum speed)
	{
		return speed switch
		{
			SpeedEnum.StandStill => 0f, 
			SpeedEnum.Walk => 0.18f * Stats.Speed, 
			_ => Stats.Speed, 
		};
	}

	public byte GetPathStatus()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!IsNavRunning())
		{
			return 2;
		}
		return (byte)NavAgent.get_pathStatus();
	}

	public NavMeshPathStatus ToPathStatus(byte value)
	{
		return (NavMeshPathStatus)value;
	}

	private void TickSenses()
	{
		if (Query.Server != null && !IsDormant)
		{
			if (Time.get_realtimeSinceStartup() > lastTickTime + SensesTickRate)
			{
				TickHearing();
				TickSmell();
				lastTickTime = Time.get_realtimeSinceStartup();
			}
			if (!AI.animal_ignore_food)
			{
				TickFoodAwareness();
			}
			UpdateSelfFacts();
		}
	}

	private void TickHearing()
	{
		SetFact(Facts.LoudNoiseNearby, 0);
	}

	private void TickSmell()
	{
	}

	private float DecisionMomentumPlayerTarget()
	{
		float num = Time.get_time() - playerTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	private float DecisionMomentumAnimalTarget()
	{
		float num = Time.get_time() - animalTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	private void TickFoodAwareness()
	{
		if (GetFact(Facts.CanTargetFood) == 0)
		{
			FoodTarget = null;
			SetFact(Facts.FoodRange, 2);
		}
		else
		{
			SelectFood();
		}
	}

	private void SelectFood()
	{
	}

	private void SelectClosestFood()
	{
	}

	private void UpdateSelfFacts()
	{
	}

	private byte IsMoving()
	{
		return (byte)((IsNavRunning() && NavAgent.get_hasPath() && NavAgent.get_remainingDistance() > NavAgent.get_stoppingDistance() && !IsStuck && GetFact(Facts.Speed) != 0) ? 1u : 0u);
	}

	private static bool AiCaresAbout(BaseEntity ent)
	{
		if (ent is BasePlayer)
		{
			return true;
		}
		if (ent is BaseNpc)
		{
			return true;
		}
		if (!AI.animal_ignore_food)
		{
			if (ent is WorldItem)
			{
				return true;
			}
			if (ent is BaseCorpse)
			{
				return true;
			}
			if (ent is CollectibleEntity)
			{
				return true;
			}
		}
		return false;
	}

	private static bool WithinVisionCone(BaseNpc npc, BaseEntity other)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
		{
			return true;
		}
		Vector3 val = other.ServerPosition - npc.ServerPosition;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		if (Vector3.Dot(((Component)npc).get_transform().get_forward(), normalized) < npc.Stats.VisionCone)
		{
			return false;
		}
		return true;
	}

	public void SetTargetPathStatus(float pendingDelay = 0.05f)
	{
		if (!isAlreadyCheckingPathPending)
		{
			if (NavAgent.get_pathPending() && numPathPendingAttempts < 10)
			{
				isAlreadyCheckingPathPending = true;
				((FacepunchBehaviour)this).Invoke((Action)DelayedTargetPathStatus, pendingDelay);
			}
			else
			{
				numPathPendingAttempts = 0;
				accumPathPendingDelay = 0f;
				SetFact(Facts.PathToTargetStatus, GetPathStatus());
			}
		}
	}

	private void DelayedTargetPathStatus()
	{
		accumPathPendingDelay += 0.1f;
		isAlreadyCheckingPathPending = false;
		SetTargetPathStatus(accumPathPendingDelay);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if ((Object)(object)NavAgent == (Object)null)
		{
			NavAgent = ((Component)this).GetComponent<NavMeshAgent>();
		}
		if ((Object)(object)NavAgent != (Object)null)
		{
			NavAgent.set_updateRotation(false);
			NavAgent.set_updatePosition(false);
			if (!LegacyNavigation)
			{
				((Component)((Component)this).get_transform()).get_gameObject().GetComponent<BaseNavigator>().Init(this, NavAgent);
			}
		}
		IsStuck = false;
		AgencyUpdateRequired = false;
		IsOnOffmeshLinkAndReachedNewCoord = false;
		((FacepunchBehaviour)this).InvokeRandomized((Action)TickAi, 0.1f, 0.1f, 0.0050000004f);
		Sleep = Random.Range(0.5f, 1f);
		Stamina.Level = Random.Range(0.1f, 1f);
		Energy.Level = Random.Range(0.5f, 1f);
		Hydration.Level = Random.Range(0.5f, 1f);
		if (NewAI)
		{
			InitFacts();
			fleeHealthThresholdPercentage = Stats.HealthThresholdForFleeing;
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
	}

	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = DropCorpse(CorpsePrefab.resourcePath);
		if (Object.op_Implicit((Object)(object)baseCorpse))
		{
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.5f);
	}

	public override void OnSensation(Sensation sensation)
	{
	}

	protected virtual void OnSenseGunshot(Sensation sensation)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		_lastHeardGunshotTime = Time.get_time();
		Vector3 val = sensation.Position - ((Component)this).get_transform().get_localPosition();
		LastHeardGunshotDirection = ((Vector3)(ref val)).get_normalized();
		if (CurrentBehaviour != Behaviour.Attack)
		{
			CurrentBehaviour = Behaviour.Flee;
		}
	}

	public bool IsNavRunning()
	{
		if ((Object)(object)GetNavAgent != (Object)null && ((Behaviour)GetNavAgent).get_enabled())
		{
			return GetNavAgent.get_isOnNavMesh();
		}
		return false;
	}

	public void Pause()
	{
		if ((Object)(object)GetNavAgent != (Object)null && ((Behaviour)GetNavAgent).get_enabled())
		{
			((Behaviour)GetNavAgent).set_enabled(false);
		}
	}

	public void Resume()
	{
		if (!GetNavAgent.get_isOnNavMesh())
		{
			((MonoBehaviour)this).StartCoroutine(TryForceToNavmesh());
		}
		else
		{
			((Behaviour)GetNavAgent).set_enabled(true);
		}
	}

	private IEnumerator TryForceToNavmesh()
	{
		yield return null;
		int numTries = 0;
		float waitForRetryTime2 = 1f;
		float maxDistanceMultiplier = 2f;
		if ((Object)(object)SingletonComponent<DynamicNavMesh>.Instance != (Object)null)
		{
			while (SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
			{
				yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime2);
				waitForRetryTime2 += 0.5f;
			}
		}
		waitForRetryTime2 = 1f;
		NavMeshHit val = default(NavMeshHit);
		for (; numTries < 4; numTries++)
		{
			if (!GetNavAgent.get_isOnNavMesh())
			{
				if (NavMesh.SamplePosition(ServerPosition, ref val, GetNavAgent.get_height() * maxDistanceMultiplier, GetNavAgent.get_areaMask()))
				{
					ServerPosition = ((NavMeshHit)(ref val)).get_position();
					GetNavAgent.Warp(ServerPosition);
					((Behaviour)GetNavAgent).set_enabled(true);
					yield break;
				}
				yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime2);
				maxDistanceMultiplier *= 1.5f;
				waitForRetryTime2 *= 1.5f;
				continue;
			}
			((Behaviour)GetNavAgent).set_enabled(true);
			yield break;
		}
		Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[1] { ((Object)this).get_name() });
		DieInstantly();
	}

	public float GetWantsToAttack(BaseEntity target)
	{
		return WantsToAttack(target);
	}

	public bool BusyTimerActive()
	{
		return BusyTimer.IsActive;
	}

	public void SetBusyFor(float dur)
	{
		BusyTimer.Activate(dur);
	}

	internal float WantsToAttack(BaseEntity target)
	{
		if ((Object)(object)target == (Object)null)
		{
			return 0f;
		}
		if (CurrentBehaviour == Behaviour.Sleep)
		{
			return 0f;
		}
		if (!target.HasAnyTrait(TraitFlag.Animal | TraitFlag.Human))
		{
			return 0f;
		}
		if (((object)target).GetType() == ((object)this).GetType())
		{
			return 1f - Stats.Tolerance;
		}
		return 1f;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.baseNPC = Pool.Get<BaseNPC>();
		info.msg.baseNPC.flags = (int)aiFlags;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseNPC != null)
		{
			aiFlags = (AiFlags)info.msg.baseNPC.flags;
		}
	}

	public override float MaxVelocity()
	{
		return Stats.Speed;
	}
}
