using UnityEngine;

public class AnimalBrain : BaseAIBrain<BaseAnimalNPC>
{
	public class AttackState : BasicAIState
	{
		private IAIAttack attack;

		public AttackState()
			: base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			attack = GetEntity();
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if ((Object)(object)basePlayer != (Object)null && basePlayer.IsDead())
			{
				StopAttacking();
			}
			else if ((Object)(object)baseEntity != (Object)null && baseEntity.Health() > 0f)
			{
				BaseCombatEntity target = baseEntity as BaseCombatEntity;
				Vector3 aimDirection = GetAimDirection(GetEntity(), target);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (attack.CanAttack(baseEntity))
				{
					StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(((Component)baseEntity).get_transform().get_position(), BaseNavigator.NavigationSpeed.Fast);
			}
		}

		public override void StateLeave()
		{
			base.StateLeave();
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			StopAttacking();
		}

		private void StopAttacking()
		{
			attack.StopAttacking();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (attack == null)
			{
				return StateStatus.Error;
			}
			if ((Object)(object)baseEntity == (Object)null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				StopAttacking();
				return StateStatus.Finished;
			}
			if (baseEntity.Health() <= 0f)
			{
				StopAttacking();
				return StateStatus.Finished;
			}
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if ((Object)(object)basePlayer != (Object)null && basePlayer.IsDead())
			{
				StopAttacking();
				return StateStatus.Finished;
			}
			BaseVehicle baseVehicle = (((Object)(object)basePlayer != (Object)null) ? basePlayer.GetMountedVehicle() : null);
			if ((Object)(object)baseVehicle != (Object)null && baseVehicle is BaseModularVehicle)
			{
				StopAttacking();
				return StateStatus.Error;
			}
			if (brain.Senses.ignoreSafeZonePlayers && (Object)(object)basePlayer != (Object)null && basePlayer.InSafeZone())
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(((Component)baseEntity).get_transform().get_position(), BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && attack != null) ? attack.EngagementRange() : 0f))
			{
				return StateStatus.Error;
			}
			BaseCombatEntity target = baseEntity as BaseCombatEntity;
			Vector3 aimDirection = GetAimDirection(GetEntity(), target);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (attack.CanAttack(baseEntity))
			{
				StartAttacking(baseEntity);
			}
			else
			{
				StopAttacking();
			}
			return StateStatus.Running;
		}

		private static Vector3 GetAimDirection(BaseCombatEntity from, BaseCombatEntity target)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)from == (Object)null || (Object)(object)target == (Object)null)
			{
				if (!((Object)(object)from != (Object)null))
				{
					return Vector3.get_forward();
				}
				return ((Component)from).get_transform().get_forward();
			}
			return Vector3Ex.Direction2D(((Component)target).get_transform().get_position(), ((Component)from).get_transform().get_position());
		}

		private void StartAttacking(BaseEntity entity)
		{
			attack.StartAttacking(entity);
		}
	}

	public class ChaseState : BasicAIState
	{
		private IAIAttack attack;

		public ChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter()
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			attack = GetEntity();
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity != (Object)null)
			{
				brain.Navigator.SetDestination(((Component)baseEntity).get_transform().get_position(), BaseNavigator.NavigationSpeed.Fast);
			}
		}

		public override void StateLeave()
		{
			base.StateLeave();
			Stop();
		}

		private void Stop()
		{
			brain.Navigator.Stop();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(((Component)baseEntity).get_transform().get_position(), BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && attack != null) ? attack.EngagementRange() : 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	public class FleeState : BasicAIState
	{
		private float nextInterval = 2f;

		private float stopFleeDistance;

		public FleeState()
			: base(AIState.Flee)
		{
		}

		public override void StateEnter()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity != (Object)null)
			{
				stopFleeDistance = Random.Range(80f, 100f) + Mathf.Clamp(Vector3Ex.Distance2D(((Component)brain.Navigator).get_transform().get_position(), ((Component)baseEntity).get_transform().get_position()), 0f, 50f);
			}
			FleeFrom(brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot), GetEntity());
		}

		public override void StateLeave()
		{
			base.StateLeave();
			Stop();
		}

		private void Stop()
		{
			brain.Navigator.Stop();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				return StateStatus.Finished;
			}
			if (Vector3Ex.Distance2D(((Component)brain.Navigator).get_transform().get_position(), ((Component)baseEntity).get_transform().get_position()) >= stopFleeDistance)
			{
				return StateStatus.Finished;
			}
			if ((brain.Navigator.UpdateIntervalElapsed(nextInterval) || !brain.Navigator.Moving) && !FleeFrom(baseEntity, GetEntity()))
			{
				return StateStatus.Error;
			}
			return StateStatus.Running;
		}

		private bool FleeFrom(BaseEntity fleeFromEntity, BaseEntity thisEntity)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)thisEntity == (Object)null || (Object)(object)fleeFromEntity == (Object)null)
			{
				return false;
			}
			nextInterval = Random.Range(3f, 6f);
			if (!brain.PathFinder.GetBestFleePosition(brain.Navigator, brain.Senses, fleeFromEntity, brain.Events.Memory.Position.Get(4), 50f, 100f, out var result))
			{
				return false;
			}
			bool num = brain.Navigator.SetDestination(result, BaseNavigator.NavigationSpeed.Fast);
			if (!num)
			{
				Stop();
			}
			return num;
		}
	}

	public class IdleState : BaseIdleState
	{
		private float nextTurnTime;

		private float minTurnTime = 10f;

		private float maxTurnTime = 20f;

		private int turnChance = 33;

		public override void StateEnter()
		{
			base.StateEnter();
			FaceNewDirection();
		}

		public override void StateLeave()
		{
			base.StateLeave();
			brain.Navigator.ClearFacingDirectionOverride();
		}

		private void FaceNewDirection()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (Random.Range(0, 100) <= turnChance)
			{
				Vector3 position = ((Component)GetEntity()).get_transform().get_position();
				Vector3 val = BasePathFinder.GetPointOnCircle(position, 1f, Random.Range(0f, 594f)) - position;
				Vector3 normalized = ((Vector3)(ref val)).get_normalized();
				brain.Navigator.SetFacingDirectionOverride(normalized);
			}
			nextTurnTime = Time.get_realtimeSinceStartup() + Random.Range(minTurnTime, maxTurnTime);
		}

		public override StateStatus StateThink(float delta)
		{
			base.StateThink(delta);
			if (Time.get_realtimeSinceStartup() >= nextTurnTime)
			{
				FaceNewDirection();
			}
			return StateStatus.Running;
		}
	}

	public class MoveTowardsState : BaseMoveTorwardsState
	{
	}

	public class RoamState : BasicAIState
	{
		private StateStatus status = StateStatus.Error;

		public RoamState()
			: base(AIState.Roam)
		{
		}

		public override void StateLeave()
		{
			base.StateLeave();
			Stop();
		}

		public override void StateEnter()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			Vector3 center;
			if (brain.InGroup() && !brain.IsGroupLeader)
			{
				center = brain.Events.Memory.Position.Get(5);
				center = BasePathFinder.GetPointOnCircle(center, Random.Range(2f, 7f), Random.Range(0f, 359f));
			}
			else
			{
				center = brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 20f, 100f);
			}
			if (brain.Navigator.SetDestination(center, BaseNavigator.NavigationSpeed.Slow))
			{
				if (brain.InGroup() && brain.IsGroupLeader)
				{
					brain.SetGroupRoamRootPosition(center);
				}
				status = StateStatus.Running;
			}
			else
			{
				status = StateStatus.Error;
			}
		}

		private void Stop()
		{
			brain.Navigator.Stop();
		}

		public override StateStatus StateThink(float delta)
		{
			base.StateThink(delta);
			if (status == StateStatus.Error)
			{
				return status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}
	}

	public static int Count;

	public override void AddStates()
	{
		base.AddStates();
		AddState(new IdleState());
		AddState(new MoveTowardsState());
		AddState(new FleeState());
		AddState(new RoamState());
		AddState(new AttackState());
		AddState(new BaseSleepState());
		AddState(new ChaseState());
		AddState(new BaseCooldownState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		thinkRate = 0.25f;
		base.PathFinder = new BasePathFinder();
		Count++;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Count--;
	}
}
