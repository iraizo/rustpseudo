using UnityEngine;

public class ScientistBrain : BaseAIBrain<HumanNPC>
{
	public class ChaseState : BasicAIState
	{
		private StateStatus status = StateStatus.Error;

		private float nextPositionUpdateTime;

		public ChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		public override void StateLeave()
		{
			base.StateLeave();
			Stop();
		}

		public override void StateEnter()
		{
			base.StateEnter();
			status = StateStatus.Error;
			if (brain.PathFinder != null)
			{
				status = StateStatus.Running;
				nextPositionUpdateTime = 0f;
			}
		}

		private void Stop()
		{
			brain.Navigator.Stop();
			brain.Navigator.ClearFacingDirectionOverride();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			if (status == StateStatus.Error)
			{
				return status;
			}
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				return StateStatus.Error;
			}
			HumanNPC entity = GetEntity();
			float num = Vector3.Distance(((Component)baseEntity).get_transform().get_position(), ((Component)entity).get_transform().get_position());
			if (brain.Senses.Memory.IsLOS(baseEntity) || num <= 10f)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			if (num <= 10f)
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Normal);
			}
			else
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Fast);
			}
			if (Time.get_time() > nextPositionUpdateTime)
			{
				nextPositionUpdateTime = Time.get_time() + Random.Range(0.5f, 1f);
				Vector3 pos = ((Component)GetEntity()).get_transform().get_position();
				AIInformationZone informationZone = entity.GetInformationZone(((Component)baseEntity).get_transform().get_position());
				bool flag = false;
				if ((Object)(object)informationZone != (Object)null)
				{
					AIMovePoint bestMovePointNear = informationZone.GetBestMovePointNear(((Component)baseEntity).get_transform().get_position(), ((Component)entity).get_transform().get_position(), 0f, brain.Navigator.BestMovementPointMaxDistance, checkLOS: true, entity, returnClosest: true);
					if (Object.op_Implicit((Object)(object)bestMovePointNear))
					{
						bestMovePointNear.SetUsedBy(entity, 5f);
						pos = brain.PathFinder.GetRandomPositionAround(((Component)bestMovePointNear).get_transform().get_position(), 0f, bestMovePointNear.radius - 0.3f);
						flag = true;
					}
				}
				if (!flag)
				{
					return StateStatus.Error;
				}
				if (num < 10f)
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal);
				}
				else
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast);
				}
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}
	}

	public class CombatState : BasicAIState
	{
		private float nextActionTime;

		private Vector3 combatStartPosition;

		public CombatState()
			: base(AIState.Combat)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			combatStartPosition = ((Component)GetEntity()).get_transform().get_position();
			FaceTarget();
		}

		public override void StateLeave()
		{
			base.StateLeave();
			GetEntity().SetDucked(flag: false);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			FaceTarget();
			if (Time.get_time() > nextActionTime)
			{
				HumanNPC entity = GetEntity();
				if (Random.Range(0, 3) == 1)
				{
					nextActionTime = Time.get_time() + Random.Range(1f, 2f);
					entity.SetDucked(flag: true);
					brain.Navigator.Stop();
				}
				else
				{
					nextActionTime = Time.get_time() + Random.Range(2f, 3f);
					entity.SetDucked(flag: false);
					brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(combatStartPosition, 1f), BaseNavigator.NavigationSpeed.Normal);
				}
			}
			return StateStatus.Running;
		}

		private void FaceTarget()
		{
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			else
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
		}
	}

	public class CombatStationaryState : BasicAIState
	{
		public CombatStationaryState()
			: base(AIState.CombatStationary)
		{
			base.AgrresiveState = true;
		}

		public override void StateLeave()
		{
			base.StateLeave();
			brain.Navigator.ClearFacingDirectionOverride();
		}

		public override StateStatus StateThink(float delta)
		{
			base.StateThink(delta);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity != (Object)null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			return StateStatus.Running;
		}
	}

	public class CoverState : BasicAIState
	{
		public CoverState()
			: base(AIState.Cover)
		{
		}

		public override void StateEnter()
		{
			base.StateEnter();
			HumanNPC entity = GetEntity();
			entity.SetDucked(flag: true);
			AIPoint aIPoint = brain.Events.Memory.AIPoint.Get(4);
			if ((Object)(object)aIPoint != (Object)null)
			{
				aIPoint.SetUsedBy(entity);
			}
		}

		public override void StateLeave()
		{
			base.StateLeave();
			HumanNPC entity = GetEntity();
			entity.SetDucked(flag: false);
			brain.Navigator.ClearFacingDirectionOverride();
			AIPoint aIPoint = brain.Events.Memory.AIPoint.Get(4);
			if ((Object)(object)aIPoint != (Object)null)
			{
				aIPoint.ClearIfUsedBy(entity);
			}
		}

		public override StateStatus StateThink(float delta)
		{
			base.StateThink(delta);
			HumanNPC entity = GetEntity();
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			float num = entity.AmmoFractionRemaining();
			if (num == 0f || ((Object)(object)baseEntity != (Object)null && !brain.Senses.Memory.IsLOS(baseEntity) && num < 0.25f))
			{
				entity.AttemptReload();
			}
			if ((Object)(object)baseEntity != (Object)null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			return StateStatus.Running;
		}
	}

	public class DismountedState : BaseDismountedState
	{
		private StateStatus status = StateStatus.Error;

		public override void StateEnter()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			status = StateStatus.Error;
			HumanNPC entity = GetEntity();
			if (brain.PathFinder == null)
			{
				return;
			}
			AIInformationZone informationZone = entity.GetInformationZone(((Component)entity).get_transform().get_position());
			if (!((Object)(object)informationZone == (Object)null))
			{
				AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(((Component)entity).get_transform().get_position(), ((Component)entity).get_transform().get_position(), 25f, 50f, entity);
				if (Object.op_Implicit((Object)(object)bestCoverPoint))
				{
					bestCoverPoint.SetUsedBy(entity, 10f);
				}
				Vector3 pos = (((Object)(object)bestCoverPoint == (Object)null) ? ((Component)entity).get_transform().get_position() : ((Component)bestCoverPoint).get_transform().get_position());
				if (brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast))
				{
					status = StateStatus.Running;
				}
			}
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

	public class IdleState : BaseIdleState
	{
	}

	public class MountedState : BaseMountedState
	{
	}

	public class RoamState : BaseRoamState
	{
		private StateStatus status = StateStatus.Error;

		private AIMovePoint roamPoint;

		public override void StateLeave()
		{
			base.StateLeave();
			Stop();
			ClearRoamPointUsage();
		}

		public override void StateEnter()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			status = StateStatus.Error;
			ClearRoamPointUsage();
			HumanNPC entity = GetEntity();
			if (brain.PathFinder == null)
			{
				return;
			}
			status = StateStatus.Error;
			roamPoint = brain.PathFinder.GetBestRoamPoint(GetRoamAnchorPosition(), ((Component)entity).get_transform().get_position(), entity.eyes.BodyForward(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
			if ((Object)(object)roamPoint != (Object)null)
			{
				if (brain.Navigator.SetDestination(((Component)roamPoint).get_transform().get_position(), BaseNavigator.NavigationSpeed.Slow))
				{
					roamPoint.SetUsedBy(GetEntity());
					status = StateStatus.Running;
				}
				else
				{
					roamPoint.SetUsedBy(entity, 600f);
				}
			}
		}

		private void ClearRoamPointUsage()
		{
			if ((Object)(object)roamPoint != (Object)null)
			{
				roamPoint.ClearIfUsedBy(GetEntity());
				roamPoint = null;
			}
		}

		private void Stop()
		{
			brain.Navigator.Stop();
		}

		public override StateStatus StateThink(float delta)
		{
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

	public class TakeCoverState : BasicAIState
	{
		private StateStatus status = StateStatus.Error;

		private BaseEntity coverFromEntity;

		public TakeCoverState()
			: base(AIState.TakeCover)
		{
		}

		public override void StateEnter()
		{
			base.StateEnter();
			status = StateStatus.Running;
			if (!StartMovingToCover())
			{
				status = StateStatus.Error;
			}
		}

		public override void StateLeave()
		{
			base.StateLeave();
			brain.Navigator.ClearFacingDirectionOverride();
			ClearCoverPointUsage();
		}

		private void ClearCoverPointUsage()
		{
			AIPoint aIPoint = brain.Events.Memory.AIPoint.Get(4);
			if ((Object)(object)aIPoint != (Object)null)
			{
				aIPoint.ClearIfUsedBy(GetEntity());
			}
		}

		private bool StartMovingToCover()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			coverFromEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)coverFromEntity == (Object)null)
			{
				return false;
			}
			HumanNPC entity = GetEntity();
			Vector3 hideFromPosition = (Object.op_Implicit((Object)(object)coverFromEntity) ? ((Component)coverFromEntity).get_transform().get_position() : (((Component)entity).get_transform().get_position() + entity.LastAttackedDir * 30f));
			AIInformationZone informationZone = entity.GetInformationZone(((Component)entity).get_transform().get_position());
			if ((Object)(object)informationZone == (Object)null)
			{
				return false;
			}
			float minRange = ((entity.SecondsSinceAttacked < 2f) ? 2f : 0f);
			float bestCoverPointMaxDistance = brain.Navigator.BestCoverPointMaxDistance;
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(((Component)entity).get_transform().get_position(), hideFromPosition, minRange, bestCoverPointMaxDistance, entity);
			if ((Object)(object)bestCoverPoint == (Object)null)
			{
				return false;
			}
			Vector3 position = ((Component)bestCoverPoint).get_transform().get_position();
			if (!brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Normal))
			{
				return false;
			}
			FaceCoverFromEntity();
			brain.Events.Memory.AIPoint.Set(bestCoverPoint, 4);
			bestCoverPoint.SetUsedBy(entity);
			return true;
		}

		public override void DrawGizmos()
		{
			base.DrawGizmos();
		}

		public override StateStatus StateThink(float delta)
		{
			base.StateThink(delta);
			FaceCoverFromEntity();
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

		private void FaceCoverFromEntity()
		{
			coverFromEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (!((Object)(object)coverFromEntity == (Object)null))
			{
				brain.Navigator.SetFacingDirectionEntity(coverFromEntity);
			}
		}
	}

	public static int Count;

	public override void AddStates()
	{
		base.AddStates();
		AddState(new BaseIdleState());
		AddState(new RoamState());
		AddState(new ChaseState());
		AddState(new CombatState());
		AddState(new TakeCoverState());
		AddState(new CoverState());
		AddState(new MountedState());
		AddState(new DismountedState());
		AddState(new BaseFollowPathState());
		AddState(new BaseNavigateHomeState());
		AddState(new CombatStationaryState());
		AddState(new BaseMoveTorwardsState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(GetEntity());
		Count++;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Count--;
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (base.CurrentState != null)
		{
			switch (base.CurrentState.StateType)
			{
			case AIState.Idle:
			case AIState.Roam:
			case AIState.Patrol:
			case AIState.FollowPath:
			case AIState.Cooldown:
				GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, b: true);
				break;
			default:
				GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, b: false);
				break;
			}
		}
	}
}
