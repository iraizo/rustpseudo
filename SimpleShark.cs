using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

public class SimpleShark : BaseCombatEntity
{
	public class SimpleState
	{
		public SimpleShark entity;

		private float stateEnterTime;

		public SimpleState(SimpleShark owner)
		{
			entity = owner;
		}

		public virtual float State_Weight()
		{
			return 0f;
		}

		public virtual void State_Enter()
		{
			stateEnterTime = Time.get_realtimeSinceStartup();
		}

		public virtual void State_Think(float delta)
		{
		}

		public virtual void State_Exit()
		{
		}

		public virtual bool CanInterrupt()
		{
			return true;
		}

		public virtual float TimeInState()
		{
			return Time.get_realtimeSinceStartup() - stateEnterTime;
		}
	}

	public class IdleState : SimpleState
	{
		private int patrolTargetIndex;

		public IdleState(SimpleShark owner)
			: base(owner)
		{
		}

		public Vector3 GetTargetPatrolPosition()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return entity.patrolPath[patrolTargetIndex];
		}

		public override float State_Weight()
		{
			return 1f;
		}

		public override void State_Enter()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < entity.patrolPath.Count; i++)
			{
				float num3 = Vector3.Distance(entity.patrolPath[i], ((Component)entity).get_transform().get_position());
				if (num3 < num)
				{
					num2 = i;
					num = num3;
				}
			}
			patrolTargetIndex = num2;
			base.State_Enter();
		}

		public override void State_Think(float delta)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			if (Vector3.Distance(GetTargetPatrolPosition(), ((Component)entity).get_transform().get_position()) < entity.stoppingDistance)
			{
				patrolTargetIndex++;
				if (patrolTargetIndex >= entity.patrolPath.Count)
				{
					patrolTargetIndex = 0;
				}
			}
			if (entity.TimeSinceAttacked() >= 120f && entity.healthFraction < 1f)
			{
				entity.health = entity.MaxHealth();
			}
			entity.destination = entity.WaterClamp(GetTargetPatrolPosition());
		}

		public override void State_Exit()
		{
			base.State_Exit();
		}

		public override bool CanInterrupt()
		{
			return true;
		}
	}

	public class AttackState : SimpleState
	{
		public AttackState(SimpleShark owner)
			: base(owner)
		{
		}

		public override float State_Weight()
		{
			if (!entity.HasTarget() || !entity.CanAttack())
			{
				return 0f;
			}
			return 10f;
		}

		public override void State_Enter()
		{
			base.State_Enter();
		}

		public override void State_Think(float delta)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer target = entity.GetTarget();
			if ((Object)(object)target == (Object)null)
			{
				return;
			}
			if (TimeInState() >= 10f)
			{
				entity.nextAttackTime = Time.get_realtimeSinceStartup() + 4f;
				entity.Startle();
				return;
			}
			if (entity.CanAttack())
			{
				entity.Startle();
			}
			float num = Vector3.Distance(entity.GetTarget().eyes.position, ((Component)entity).get_transform().get_position());
			bool num2 = num < 4f;
			if (entity.CanAttack() && num <= 2f)
			{
				entity.DoAttack();
			}
			if (!num2)
			{
				Vector3 val = Vector3Ex.Direction(entity.GetTarget().eyes.position, ((Component)entity).get_transform().get_position());
				Vector3 point = target.eyes.position + val * 10f;
				point = entity.WaterClamp(point);
				entity.destination = point;
			}
		}

		public override void State_Exit()
		{
			base.State_Exit();
		}

		public override bool CanInterrupt()
		{
			return true;
		}
	}

	public Vector3 destination;

	public float minSpeed;

	public float maxSpeed;

	public float idealDepth;

	public float minTurnSpeed = 0.25f;

	public float maxTurnSpeed = 2f;

	public float attackCooldown = 7f;

	public float aggroRange = 15f;

	public float obstacleDetectionRadius = 1f;

	public Animator animator;

	public GameObjectRef bloodCloud;

	public GameObjectRef corpsePrefab;

	[ServerVar]
	public static float forceSurfaceAmount = 0f;

	[ServerVar]
	public static bool disable = false;

	private Vector3 spawnPos;

	private float stoppingDistance = 3f;

	private float currentSpeed;

	private float lastStartleTime;

	private float startleDuration = 1f;

	private SimpleState[] states;

	private SimpleState _currentState;

	private bool sleeping;

	public List<Vector3> patrolPath = new List<Vector3>();

	private BasePlayer target;

	private float lastSeenTargetTime;

	private float nextTargetSearchTime;

	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	private float minFloorDist = 2f;

	private float minSurfaceDist = 1f;

	private float lastTimeAttacked;

	private float nextAttackTime;

	private Vector3 cachedObstacleNormal;

	private float cachedObstacleDistance;

	private float obstacleAvoidanceScale;

	private float obstacleDetectionRange = 5f;

	private float timeSinceLastObstacleCheck;

	private void GenerateIdlePoints(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int num3 = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		RaycastHit val3 = default(RaycastHit);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = 1f;
			Vector3 val = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 val2 = Vector3Ex.Direction(val, center);
			val = ((!Physics.SphereCast(center, obstacleDetectionRadius, val2, ref val3, radius + staggerOffset, num3)) ? (center + val2 * radius) : (center + val2 * (((RaycastHit)(ref val3)).get_distance() - 6f)));
			if (staggerOffset != 0f)
			{
				val += val2 * Random.Range(0f - staggerOffset, staggerOffset);
			}
			val.y += Random.Range(0f - heightOffset, heightOffset);
			val.y = Mathf.Clamp(val.y, height2 + 3f, height - 3f);
			patrolPath.Add(val);
		}
	}

	private void GenerateIdlePoints_Shrinkwrap(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int num3 = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		RaycastHit val3 = default(RaycastHit);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = radius * 2f;
			Vector3 val = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 val2 = Vector3Ex.Direction(center, val);
			val = ((!Physics.SphereCast(val, obstacleDetectionRadius, val2, ref val3, radius + staggerOffset, num3)) ? (val + val2 * radius) : (((RaycastHit)(ref val3)).get_point() - val2 * 6f));
			if (staggerOffset != 0f)
			{
				val += val2 * Random.Range(0f - staggerOffset, staggerOffset);
			}
			val.y += Random.Range(0f - heightOffset, heightOffset);
			val.y = Mathf.Clamp(val.y, height2 + 3f, height - 3f);
			patrolPath.Add(val);
		}
	}

	public override void ServerInit()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (disable)
		{
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.01f);
			return;
		}
		((Component)this).get_transform().set_position(WaterClamp(((Component)this).get_transform().get_position()));
		Init();
		((FacepunchBehaviour)this).InvokeRandomized((Action)CheckSleepState, 0f, 1f, 0.5f);
	}

	public void CheckSleepState()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		bool flag = BaseNetworkable.HasCloseConnections(((Component)this).get_transform().get_position(), 100f);
		sleeping = !flag;
	}

	public void Init()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		GenerateIdlePoints_Shrinkwrap(((Component)this).get_transform().get_position(), 20f, 2f, 3f);
		states = new SimpleState[2];
		states[0] = new IdleState(this);
		states[1] = new AttackState(this);
		((Component)this).get_transform().set_position(patrolPath[0]);
	}

	private void Think(float delta)
	{
		if (states == null)
		{
			return;
		}
		if (disable)
		{
			if (!((FacepunchBehaviour)this).IsInvoking((Action)base.KillMessage))
			{
				((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.01f);
			}
		}
		else
		{
			if (sleeping)
			{
				return;
			}
			SimpleState simpleState = null;
			float num = -1f;
			SimpleState[] array = states;
			foreach (SimpleState simpleState2 in array)
			{
				float num2 = simpleState2.State_Weight();
				if (num2 > num)
				{
					simpleState = simpleState2;
					num = num2;
				}
			}
			if (simpleState != _currentState && (_currentState == null || _currentState.CanInterrupt()))
			{
				if (_currentState != null)
				{
					_currentState.State_Exit();
				}
				simpleState.State_Enter();
				_currentState = simpleState;
			}
			UpdateTarget(delta);
			_currentState.State_Think(delta);
			UpdateObstacleAvoidance(delta);
			UpdateDirection(delta);
			UpdateSpeed(delta);
			UpdatePosition(delta);
			SetFlag(Flags.Open, HasTarget() && CanAttack());
		}
	}

	public Vector3 WaterClamp(Vector3 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float height = WaterSystem.GetHeight(point);
		float num = TerrainMeta.HeightMap.GetHeight(point) + minFloorDist;
		float num2 = height - minSurfaceDist;
		if (forceSurfaceAmount != 0f)
		{
			num = (num2 = WaterSystem.GetHeight(point) + forceSurfaceAmount);
		}
		point.y = Mathf.Clamp(point.y, num, num2);
		return point;
	}

	public bool ValidTarget(BasePlayer newTarget)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(newTarget.eyes.position, ((Component)this).get_transform().get_position());
		Vector3 val = Vector3Ex.Direction(newTarget.eyes.position, ((Component)this).get_transform().get_position());
		int num2 = 10551552;
		if (Physics.Raycast(((Component)this).get_transform().get_position(), val, num, num2))
		{
			return false;
		}
		if (newTarget.isMounted)
		{
			if (Object.op_Implicit((Object)(object)newTarget.GetMountedVehicle()))
			{
				return false;
			}
			if (!((Behaviour)((Component)newTarget.GetMounted()).GetComponent<WaterInflatable>().buoyancy).get_enabled())
			{
				return false;
			}
		}
		else if (!WaterLevel.Test(newTarget.CenterPoint(), waves: true, newTarget))
		{
			return false;
		}
		return true;
	}

	public void ClearTarget()
	{
		target = null;
		lastSeenTargetTime = 0f;
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (base.isServer)
		{
			BaseCorpse baseCorpse = DropCorpse(corpsePrefab.resourcePath);
			if (Object.op_Implicit((Object)(object)baseCorpse))
			{
				baseCorpse.Spawn();
				baseCorpse.TakeChildren(this);
			}
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.5f);
		}
		base.OnKilled(hitInfo);
	}

	public void UpdateTarget(float delta)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)target != (Object)null)
		{
			bool flag = Vector3.Distance(target.eyes.position, ((Component)this).get_transform().get_position()) > aggroRange * 2f;
			bool flag2 = Time.get_realtimeSinceStartup() > lastSeenTargetTime + 4f;
			if (!ValidTarget(target) || flag || flag2)
			{
				ClearTarget();
			}
			else
			{
				lastSeenTargetTime = Time.get_realtimeSinceStartup();
			}
		}
		if (Time.get_realtimeSinceStartup() < nextTargetSearchTime || !((Object)(object)target == (Object)null))
		{
			return;
		}
		nextTargetSearchTime = Time.get_realtimeSinceStartup() + 1f;
		if (!BaseNetworkable.HasCloseConnections(((Component)this).get_transform().get_position(), aggroRange))
		{
			return;
		}
		int playersInSphere = Query.Server.GetPlayersInSphere(((Component)this).get_transform().get_position(), aggroRange, playerQueryResults);
		for (int i = 0; i < playersInSphere; i++)
		{
			BasePlayer basePlayer = playerQueryResults[i];
			if (!basePlayer.isClient && ValidTarget(basePlayer))
			{
				target = basePlayer;
				lastSeenTargetTime = Time.get_realtimeSinceStartup();
				break;
			}
		}
	}

	public float TimeSinceAttacked()
	{
		return Time.get_realtimeSinceStartup() - lastTimeAttacked;
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		lastTimeAttacked = Time.get_realtimeSinceStartup();
		if (info.damageTypes.Total() > 20f)
		{
			Startle();
		}
		if ((Object)(object)info.InitiatorPlayer != (Object)null && (Object)(object)target == (Object)null && ValidTarget(info.InitiatorPlayer))
		{
			target = info.InitiatorPlayer;
			lastSeenTargetTime = Time.get_realtimeSinceStartup();
		}
	}

	public bool HasTarget()
	{
		return (Object)(object)target != (Object)null;
	}

	public BasePlayer GetTarget()
	{
		return target;
	}

	public bool CanAttack()
	{
		return Time.get_realtimeSinceStartup() > nextAttackTime;
	}

	public void DoAttack()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (HasTarget())
		{
			GetTarget().Hurt(Random.Range(30f, 70f), DamageType.Bite, this);
			Vector3 posWorld = WaterClamp(GetTarget().CenterPoint());
			Effect.server.Run(bloodCloud.resourcePath, posWorld, Vector3.get_forward());
			nextAttackTime = Time.get_realtimeSinceStartup() + attackCooldown;
		}
	}

	public void Startle()
	{
		lastStartleTime = Time.get_realtimeSinceStartup();
	}

	public bool IsStartled()
	{
		return lastStartleTime + startleDuration > Time.get_realtimeSinceStartup();
	}

	private float GetDesiredSpeed()
	{
		if (!IsStartled())
		{
			return minSpeed;
		}
		return maxSpeed;
	}

	public float GetTurnSpeed()
	{
		if (IsStartled())
		{
			return maxTurnSpeed;
		}
		if (obstacleAvoidanceScale != 0f)
		{
			return Mathf.Lerp(minTurnSpeed, maxTurnSpeed, obstacleAvoidanceScale);
		}
		return minTurnSpeed;
	}

	private float GetCurrentSpeed()
	{
		return currentSpeed;
	}

	private void UpdateObstacleAvoidance(float delta)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		timeSinceLastObstacleCheck += delta;
		if (timeSinceLastObstacleCheck < 0.5f)
		{
			return;
		}
		Vector3 forward = ((Component)this).get_transform().get_forward();
		Vector3 position = ((Component)this).get_transform().get_position();
		int num = 1503764737;
		RaycastHit val = default(RaycastHit);
		if (Physics.SphereCast(position, obstacleDetectionRadius, forward, ref val, obstacleDetectionRange, num))
		{
			Vector3 point = ((RaycastHit)(ref val)).get_point();
			Vector3 val2 = Vector3.get_zero();
			Vector3 val3 = Vector3.get_zero();
			RaycastHit val4 = default(RaycastHit);
			if (Physics.SphereCast(position + Vector3.get_down() * 0.25f + ((Component)this).get_transform().get_right() * 0.25f, obstacleDetectionRadius, forward, ref val4, obstacleDetectionRange, num))
			{
				val2 = ((RaycastHit)(ref val4)).get_point();
			}
			RaycastHit val5 = default(RaycastHit);
			if (Physics.SphereCast(position + Vector3.get_down() * 0.25f - ((Component)this).get_transform().get_right() * 0.25f, obstacleDetectionRadius, forward, ref val5, obstacleDetectionRange, num))
			{
				val3 = ((RaycastHit)(ref val5)).get_point();
			}
			if (val2 != Vector3.get_zero() && val3 != Vector3.get_zero())
			{
				Plane val6 = default(Plane);
				((Plane)(ref val6))._002Ector(point, val2, val3);
				Vector3 normal = ((Plane)(ref val6)).get_normal();
				if (normal != Vector3.get_zero())
				{
					((RaycastHit)(ref val)).set_normal(normal);
				}
			}
			cachedObstacleNormal = ((RaycastHit)(ref val)).get_normal();
			cachedObstacleDistance = ((RaycastHit)(ref val)).get_distance();
			obstacleAvoidanceScale = 1f - Mathf.InverseLerp(2f, obstacleDetectionRange * 0.75f, ((RaycastHit)(ref val)).get_distance());
		}
		else
		{
			obstacleAvoidanceScale = Mathf.MoveTowards(obstacleAvoidanceScale, 0f, timeSinceLastObstacleCheck * 2f);
			if (obstacleAvoidanceScale == 0f)
			{
				cachedObstacleDistance = 0f;
			}
		}
		timeSinceLastObstacleCheck = 0f;
	}

	private void UpdateDirection(float delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().get_forward();
		Vector3 val = Vector3Ex.Direction(WaterClamp(destination), ((Component)this).get_transform().get_position());
		if (obstacleAvoidanceScale != 0f)
		{
			Vector3 val3;
			if (cachedObstacleNormal != Vector3.get_zero())
			{
				Vector3 val2 = QuaternionEx.LookRotationForcedUp(cachedObstacleNormal, Vector3.get_up()) * Vector3.get_forward();
				val3 = ((!(Vector3.Dot(val2, ((Component)this).get_transform().get_right()) > Vector3.Dot(val2, -((Component)this).get_transform().get_right()))) ? (-((Component)this).get_transform().get_right()) : ((Component)this).get_transform().get_right());
			}
			else
			{
				val3 = ((Component)this).get_transform().get_right();
			}
			val = val3 * obstacleAvoidanceScale;
			((Vector3)(ref val)).Normalize();
		}
		if (val != Vector3.get_zero())
		{
			Quaternion val4 = Quaternion.LookRotation(val, Vector3.get_up());
			((Component)this).get_transform().set_rotation(Quaternion.Lerp(((Component)this).get_transform().get_rotation(), val4, delta * GetTurnSpeed()));
		}
	}

	private void UpdatePosition(float delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)this).get_transform().get_forward();
		Vector3 point = ((Component)this).get_transform().get_position() + forward * GetCurrentSpeed() * delta;
		point = WaterClamp(point);
		((Component)this).get_transform().set_position(point);
	}

	private void UpdateSpeed(float delta)
	{
		currentSpeed = Mathf.Lerp(currentSpeed, GetDesiredSpeed(), delta * 4f);
	}

	public void Update()
	{
		if (base.isServer)
		{
			Think(Time.get_deltaTime());
		}
	}
}
