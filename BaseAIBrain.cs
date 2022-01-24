using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConVar;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseAIBrain<T> : EntityComponent<T>, IPet, IAISleepable, IAIDesign, IAIGroupable, IAIEventListener where T : BaseEntity
{
	public class BaseAttackState : BasicAIState
	{
		private IAIAttack attack;

		public BaseAttackState()
			: base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			attack = GetEntity() as IAIAttack;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity != (Object)null)
			{
				Vector3 aimDirection = GetAimDirection(((Component)brain.Navigator).get_transform().get_position(), ((Component)baseEntity).get_transform().get_position());
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
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
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
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				BasePlayer basePlayer = baseEntity as BasePlayer;
				if ((Object)(object)basePlayer != (Object)null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			if (!brain.Navigator.SetDestination(((Component)baseEntity).get_transform().get_position(), BaseNavigator.NavigationSpeed.Fast, 0.25f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = GetAimDirection(((Component)brain.Navigator).get_transform().get_position(), ((Component)baseEntity).get_transform().get_position());
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

		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return Vector3Ex.Direction2D(target, from);
		}

		private void StartAttacking(BaseEntity entity)
		{
			attack.StartAttacking(entity);
		}
	}

	public class BaseChaseState : BasicAIState
	{
		public BaseChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
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
			if (!brain.Navigator.SetDestination(((Component)baseEntity).get_transform().get_position(), BaseNavigator.NavigationSpeed.Fast, 0.25f))
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

	public class BaseCooldownState : BasicAIState
	{
		public BaseCooldownState()
			: base(AIState.Cooldown)
		{
		}
	}

	public class BaseDismountedState : BasicAIState
	{
		public BaseDismountedState()
			: base(AIState.Dismounted)
		{
		}
	}

	public class BaseFleeState : BasicAIState
	{
		private float nextInterval = 2f;

		private float stopFleeDistance;

		public BaseFleeState()
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

	public class BaseFollowPathState : BasicAIState
	{
		private AIMovePointPath path;

		private StateStatus status;

		private AIMovePoint currentTargetPoint;

		private float currentWaitTime;

		private AIMovePointPath.PathDirection pathDirection;

		private int currentNodeIndex;

		public BaseFollowPathState()
			: base(AIState.FollowPath)
		{
		}

		public override void StateEnter()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			status = StateStatus.Error;
			brain.Navigator.SetBrakingEnabled(flag: false);
			path = brain.Navigator.Path;
			T entity = GetEntity();
			if ((Object)(object)path == (Object)null)
			{
				AIInformationZone forPoint = AIInformationZone.GetForPoint(entity.ServerPosition);
				if ((Object)(object)forPoint == (Object)null)
				{
					return;
				}
				path = forPoint.GetNearestPath(entity.ServerPosition);
				if ((Object)(object)path == (Object)null)
				{
					return;
				}
			}
			currentNodeIndex = path.FindNearestPointIndex(entity.ServerPosition);
			currentTargetPoint = path.FindNearestPoint(entity.ServerPosition);
			if (!((Object)(object)currentTargetPoint == (Object)null))
			{
				status = StateStatus.Running;
				currentWaitTime = 0f;
				brain.Navigator.SetDestination(((Component)currentTargetPoint).get_transform().get_position(), BaseNavigator.NavigationSpeed.Slow);
			}
		}

		public override void StateLeave()
		{
			base.StateLeave();
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.SetBrakingEnabled(flag: true);
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			if (status == StateStatus.Error)
			{
				return status;
			}
			if (!brain.Navigator.Moving)
			{
				if (currentWaitTime <= 0f && currentTargetPoint.HasLookAtPoints())
				{
					Transform randomLookAtPoint = currentTargetPoint.GetRandomLookAtPoint();
					if ((Object)(object)randomLookAtPoint != (Object)null)
					{
						brain.Navigator.SetFacingDirectionOverride(Vector3Ex.Direction2D(((Component)randomLookAtPoint).get_transform().get_position(), GetEntity().ServerPosition));
					}
				}
				if (currentTargetPoint.WaitTime > 0f)
				{
					currentWaitTime += delta;
				}
				if (currentTargetPoint.WaitTime <= 0f || currentWaitTime >= currentTargetPoint.WaitTime)
				{
					brain.Navigator.ClearFacingDirectionOverride();
					currentWaitTime = 0f;
					int num = currentNodeIndex;
					currentNodeIndex = path.GetNextPointIndex(currentNodeIndex, ref pathDirection);
					currentTargetPoint = path.GetPointAtIndex(currentNodeIndex);
					if ((!((Object)(object)currentTargetPoint != (Object)null) || currentNodeIndex != num) && ((Object)(object)currentTargetPoint == (Object)null || !brain.Navigator.SetDestination(((Component)currentTargetPoint).get_transform().get_position(), BaseNavigator.NavigationSpeed.Slow)))
					{
						return StateStatus.Error;
					}
				}
			}
			return StateStatus.Running;
		}
	}

	public class BaseIdleState : BasicAIState
	{
		public BaseIdleState()
			: base(AIState.Idle)
		{
		}
	}

	public class BaseMountedState : BasicAIState
	{
		public BaseMountedState()
			: base(AIState.Mounted)
		{
		}

		public override void StateEnter()
		{
			base.StateEnter();
			brain.Navigator.Stop();
		}
	}

	public class BaseMoveTorwardsState : BasicAIState
	{
		public BaseMoveTorwardsState()
			: base(AIState.MoveTowards)
		{
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
			if (!brain.Navigator.SetDestination(((Component)baseEntity).get_transform().get_position(), BaseNavigator.NavigationSpeed.Normal, 0.25f))
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

	public class BaseNavigateHomeState : BasicAIState
	{
		private StateStatus status;

		public BaseNavigateHomeState()
			: base(AIState.NavigateHome)
		{
		}

		public override void StateEnter()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			Vector3 pos = brain.Events.Memory.Position.Get(4);
			status = StateStatus.Running;
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal))
			{
				status = StateStatus.Error;
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
			base.StateThink(delta);
			if (status == StateStatus.Error)
			{
				return status;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	public class BasePatrolState : BasicAIState
	{
		public BasePatrolState()
			: base(AIState.Patrol)
		{
		}
	}

	public class BaseRoamState : BasicAIState
	{
		private float nextRoamPositionTime = -1f;

		private float lastDestinationTime;

		public BaseRoamState()
			: base(AIState.Roam)
		{
		}

		public override float GetWeight()
		{
			return 0f;
		}

		public override void StateEnter()
		{
			base.StateEnter();
			nextRoamPositionTime = -1f;
			lastDestinationTime = Time.get_time();
		}

		public virtual Vector3 GetDestination()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.get_zero();
		}

		public virtual Vector3 GetForwardDirection()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.get_forward();
		}

		public virtual void SetDestination(Vector3 destination)
		{
		}

		public override void DrawGizmos()
		{
			base.DrawGizmos();
			brain.PathFinder.DebugDraw();
		}

		public virtual Vector3 GetRoamAnchorPosition()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (brain.Navigator.MaxRoamDistanceFromHome > -1f)
			{
				return brain.Events.Memory.Position.Get(4);
			}
			return ((Component)GetEntity()).get_transform().get_position();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			bool flag = Time.get_time() - lastDestinationTime > 25f;
			T entity = GetEntity();
			if ((Vector3.Distance(GetDestination(), ((Component)entity).get_transform().get_position()) < 2f || flag) && nextRoamPositionTime == -1f)
			{
				nextRoamPositionTime = Time.get_time() + Random.Range(5f, 10f);
			}
			if (nextRoamPositionTime != -1f && Time.get_time() > nextRoamPositionTime)
			{
				AIMovePoint bestRoamPoint = brain.PathFinder.GetBestRoamPoint(GetRoamAnchorPosition(), entity.ServerPosition, GetForwardDirection(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
				if (Object.op_Implicit((Object)(object)bestRoamPoint))
				{
					float num = Vector3.Distance(((Component)bestRoamPoint).get_transform().get_position(), ((Component)entity).get_transform().get_position()) / 1.5f;
					bestRoamPoint.SetUsedBy(entity, num + 11f);
				}
				lastDestinationTime = Time.get_time();
				Vector3 insideUnitSphere = Random.get_insideUnitSphere();
				insideUnitSphere.y = 0f;
				((Vector3)(ref insideUnitSphere)).Normalize();
				Vector3 destination = (((Object)(object)bestRoamPoint == (Object)null) ? ((Component)entity).get_transform().get_position() : (((Component)bestRoamPoint).get_transform().get_position() + insideUnitSphere * bestRoamPoint.radius));
				SetDestination(destination);
				nextRoamPositionTime = -1f;
			}
			return StateStatus.Running;
		}
	}

	public class BaseSleepState : BasicAIState
	{
		private StateStatus status = StateStatus.Error;

		public BaseSleepState()
			: base(AIState.Sleep)
		{
		}

		public override void StateEnter()
		{
			base.StateEnter();
			status = StateStatus.Error;
			IAISleep iAISleep;
			if ((iAISleep = GetEntity() as IAISleep) != null)
			{
				iAISleep.StartSleeping();
				status = StateStatus.Running;
			}
		}

		public override void StateLeave()
		{
			base.StateLeave();
			IAISleep iAISleep;
			if ((iAISleep = GetEntity() as IAISleep) != null)
			{
				iAISleep.StopSleeping();
			}
		}

		public override StateStatus StateThink(float delta)
		{
			base.StateThink(delta);
			return status;
		}
	}

	public class BasicAIState
	{
		public BaseAIBrain<T> brain;

		protected float _lastStateExitTime;

		public AIState StateType { get; private set; }

		public float TimeInState { get; private set; }

		public bool AgrresiveState { get; protected set; }

		public virtual void StateEnter()
		{
			TimeInState = 0f;
		}

		public virtual StateStatus StateThink(float delta)
		{
			TimeInState += delta;
			return StateStatus.Running;
		}

		public virtual void StateLeave()
		{
			TimeInState = 0f;
			_lastStateExitTime = Time.get_time();
		}

		public virtual bool CanInterrupt()
		{
			return true;
		}

		public virtual bool CanEnter()
		{
			return true;
		}

		public virtual bool CanLeave()
		{
			return CanInterrupt();
		}

		public virtual float GetWeight()
		{
			return 0f;
		}

		public float TimeSinceState()
		{
			return Time.get_time() - _lastStateExitTime;
		}

		public BasicAIState(AIState state)
		{
			StateType = state;
		}

		public void Reset()
		{
			TimeInState = 0f;
		}

		public bool IsInState()
		{
			if ((Object)(object)brain != (Object)null && brain.CurrentState != null)
			{
				return brain.CurrentState == this;
			}
			return false;
		}

		public virtual void DrawGizmos()
		{
		}

		public T GetEntity()
		{
			return brain.GetEntity();
		}
	}

	public bool UseQueuedMovementUpdates;

	public bool AllowedToSleep = true;

	public AIDesignSO DefaultDesignSO;

	public List<AIDesignSO> Designs = new List<AIDesignSO>();

	public AIDesign InstanceSpecificDesign;

	public float SenseRange = 10f;

	public float AttackRangeMultiplier = 1f;

	public float TargetLostRange = 40f;

	public float VisionCone = -0.8f;

	public bool CheckVisionCone;

	public bool CheckLOS;

	public bool IgnoreNonVisionSneakers = true;

	public float ListenRange;

	public EntityType SenseTypes;

	public bool HostileTargetsOnly;

	public bool IgnoreSafeZonePlayers;

	public int MaxGroupSize;

	public float MemoryDuration = 10f;

	public bool RefreshKnownLOS;

	public Vector3 mainInterestPoint;

	public bool UseAIDesign;

	public bool Pet;

	private List<IAIGroupable> groupMembers = new List<IAIGroupable>();

	protected int loadedDesignIndex;

	private int currentStateContainerID = -1;

	private float lastMovementTickTime;

	private bool sleeping;

	private bool disabled;

	protected Dictionary<AIState, BasicAIState> states;

	protected float thinkRate = 0.25f;

	protected float lastThinkTime;

	public BasicAIState CurrentState { get; private set; }

	public AIThinkMode ThinkMode { get; protected set; } = AIThinkMode.Interval;


	public float Age { get; private set; }

	public AIBrainSenses Senses { get; private set; } = new AIBrainSenses();


	public BasePathFinder PathFinder { get; protected set; }

	public AIEvents Events { get; private set; }

	public AIDesign AIDesign { get; private set; }

	public BasePlayer DesigningPlayer { get; private set; }

	public BasePlayer OwningPlayer { get; private set; }

	public bool IsGroupLeader { get; private set; }

	public bool IsGrouped { get; private set; }

	public IAIGroupable GroupLeader { get; private set; }

	public BaseNavigator Navigator { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseAIBrain<T>.OnRpcMessage", 0);
		try
		{
			BaseEntity.RPCMessage rPCMessage;
			if (rpc == 66191493 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestAIDesign "));
				}
				TimeWarning val2 = TimeWarning.New("RequestAIDesign", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(BaseEntity.RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						BaseEntity.RPCMessage msg2 = rPCMessage;
						RequestAIDesign(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RequestAIDesign");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2122228512 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StopAIDesign "));
				}
				TimeWarning val2 = TimeWarning.New("StopAIDesign", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(BaseEntity.RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						BaseEntity.RPCMessage msg3 = rPCMessage;
						StopAIDesign(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in StopAIDesign");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 657290375 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SubmitAIDesign "));
				}
				TimeWarning val2 = TimeWarning.New("SubmitAIDesign", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(BaseEntity.RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						BaseEntity.RPCMessage msg4 = rPCMessage;
						SubmitAIDesign(msg4);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex3)
				{
					Debug.LogException(ex3);
					player.Kick("RPC Error in SubmitAIDesign");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool IsPet()
	{
		return Pet;
	}

	public void SetPetOwner(BasePlayer player)
	{
		T val = (T)(player.PetEntity = GetEntity());
		GetEntity().OwnerID = player.userID;
		BasePet.ActivePetByOwnerID[player.userID] = val as BasePet;
	}

	public bool IsOwnedBy(BasePlayer player)
	{
		if ((Object)(object)OwningPlayer == (Object)null)
		{
			return false;
		}
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (this == null)
		{
			return false;
		}
		return (Object)(object)OwningPlayer == (Object)(object)player;
	}

	public bool IssuePetCommand(PetCommandType cmd, int param, Ray? ray)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (ray.HasValue)
		{
			int num = 10551296;
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(ray.Value, ref val, 75f, num))
			{
				Events.Memory.Position.Set(((RaycastHit)(ref val)).get_point(), 6);
			}
			else
			{
				Events.Memory.Position.Set(((Component)this).get_transform().get_position(), 6);
			}
		}
		switch (cmd)
		{
		case PetCommandType.LoadDesign:
			if (param < 0 || param >= Designs.Count)
			{
				return false;
			}
			LoadAIDesign(AIDesigns.GetByNameOrInstance(Designs[param].Filename, InstanceSpecificDesign), null, param);
			return true;
		case PetCommandType.SetState:
		{
			AIStateContainer stateContainerByID = AIDesign.GetStateContainerByID(param);
			if (stateContainerByID == null)
			{
				return false;
			}
			return SwitchToState(stateContainerByID.State, param);
		}
		case PetCommandType.Destroy:
			GetEntity().Kill();
			return true;
		default:
			return false;
		}
	}

	public void ForceSetAge(float age)
	{
		Age = age;
	}

	public int LoadedDesignIndex()
	{
		return loadedDesignIndex;
	}

	public void SetEnabled(bool flag)
	{
		disabled = !flag;
	}

	bool IAIDesign.CanPlayerDesignAI(BasePlayer player)
	{
		return PlayerCanDesignAI(player);
	}

	private bool PlayerCanDesignAI(BasePlayer player)
	{
		if (!AI.allowdesigning)
		{
			return false;
		}
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (!UseAIDesign)
		{
			return false;
		}
		if ((Object)(object)DesigningPlayer != (Object)null)
		{
			return false;
		}
		if (!player.IsDeveloper)
		{
			return false;
		}
		return true;
	}

	[BaseEntity.RPC_Server]
	private void RequestAIDesign(BaseEntity.RPCMessage msg)
	{
		if (UseAIDesign && !((Object)(object)msg.player == (Object)null) && AIDesign != null && PlayerCanDesignAI(msg.player))
		{
			msg.player.designingAIEntity = GetEntity();
			msg.player.ClientRPCPlayer<AIDesign>(null, msg.player, "StartDesigningAI", AIDesign.ToProto(currentStateContainerID));
			DesigningPlayer = msg.player;
			SetOwningPlayer(msg.player);
		}
	}

	[BaseEntity.RPC_Server]
	private void SubmitAIDesign(BaseEntity.RPCMessage msg)
	{
		AIDesign val = AIDesign.Deserialize((Stream)(object)msg.read);
		if (!LoadAIDesign(val, msg.player, loadedDesignIndex))
		{
			return;
		}
		SaveDesign();
		if (val.scope == 2)
		{
			return;
		}
		T entity = GetEntity();
		BaseEntity[] array = BaseEntity.Util.FindTargets(entity.ShortPrefabName, onlyPlayers: false);
		if (array == null || array.Length == 0)
		{
			return;
		}
		BaseEntity[] array2 = array;
		foreach (BaseEntity baseEntity in array2)
		{
			if ((Object)(object)baseEntity == (Object)null || (Object)(object)baseEntity == (Object)(object)entity)
			{
				continue;
			}
			EntityComponentBase[] components = baseEntity.Components;
			if (components == null)
			{
				continue;
			}
			EntityComponentBase[] array3 = components;
			for (int j = 0; j < array3.Length; j++)
			{
				IAIDesign iAIDesign;
				if ((iAIDesign = array3[j] as IAIDesign) != null)
				{
					iAIDesign.LoadAIDesign(val, null);
					break;
				}
			}
		}
	}

	void IAIDesign.StopDesigning()
	{
		ClearDesigningPlayer();
	}

	void IAIDesign.LoadAIDesign(AIDesign design, BasePlayer player)
	{
		LoadAIDesign(design, player, loadedDesignIndex);
	}

	public bool LoadDefaultAIDesign()
	{
		if (loadedDesignIndex == 0)
		{
			return true;
		}
		return LoadAIDesignAtIndex(0);
	}

	public bool LoadAIDesignAtIndex(int index)
	{
		if (Designs == null)
		{
			return false;
		}
		if (index < 0 || index >= Designs.Count)
		{
			return false;
		}
		return LoadAIDesign(AIDesigns.GetByNameOrInstance(Designs[index].Filename, InstanceSpecificDesign), null, index);
	}

	public virtual void OnAIDesignLoadedAtIndex(int index)
	{
	}

	protected bool LoadAIDesign(AIDesign design, BasePlayer player, int index)
	{
		if (design == null)
		{
			Debug.LogError((object)(((Object)((Component)GetEntity()).get_gameObject()).get_name() + " failed to load AI design!"));
			return false;
		}
		if ((Object)(object)player != (Object)null)
		{
			AIDesignScope scope = (AIDesignScope)design.scope;
			if (scope == AIDesignScope.Default && !player.IsDeveloper)
			{
				return false;
			}
			if (scope == AIDesignScope.EntityServerWide && !player.IsDeveloper && !player.IsAdmin)
			{
				return false;
			}
		}
		if (AIDesign == null)
		{
			return false;
		}
		AIDesign.Load(design, base.baseEntity);
		AIStateContainer defaultStateContainer = AIDesign.GetDefaultStateContainer();
		if (defaultStateContainer != null)
		{
			SwitchToState(defaultStateContainer.State, defaultStateContainer.ID);
		}
		loadedDesignIndex = index;
		OnAIDesignLoadedAtIndex(loadedDesignIndex);
		return true;
	}

	public void SaveDesign()
	{
		if (AIDesign == null)
		{
			return;
		}
		AIDesign val = AIDesign.ToProto(currentStateContainerID);
		string text = "cfg/ai/";
		string filename = Designs[loadedDesignIndex].Filename;
		switch (AIDesign.Scope)
		{
		case AIDesignScope.Default:
			text += filename;
			try
			{
				using (FileStream fileStream2 = File.Create(text))
				{
					AIDesign.Serialize((Stream)fileStream2, val);
				}
				AIDesigns.RefreshCache(filename, val);
			}
			catch (Exception)
			{
				Debug.LogWarning((object)("Error trying to save default AI Design: " + text));
			}
			break;
		case AIDesignScope.EntityServerWide:
			filename += "_custom";
			text += filename;
			try
			{
				using (FileStream fileStream = File.Create(text))
				{
					AIDesign.Serialize((Stream)fileStream, val);
				}
				AIDesigns.RefreshCache(filename, val);
			}
			catch (Exception)
			{
				Debug.LogWarning((object)("Error trying to save server-wide AI Design: " + text));
			}
			break;
		case AIDesignScope.EntityInstance:
			break;
		}
	}

	[BaseEntity.RPC_Server]
	private void StopAIDesign(BaseEntity.RPCMessage msg)
	{
		if ((Object)(object)msg.player == (Object)(object)DesigningPlayer)
		{
			ClearDesigningPlayer();
		}
	}

	private void ClearDesigningPlayer()
	{
		DesigningPlayer = null;
	}

	public void SetOwningPlayer(BasePlayer owner)
	{
		OwningPlayer = owner;
		Events.Memory.Entity.Set(OwningPlayer, 5);
		IPet pet;
		if ((pet = this) != null && pet.IsPet())
		{
			pet.SetPetOwner(owner);
			owner.Pet = pet;
		}
	}

	public virtual bool ShouldServerThink()
	{
		if (ThinkMode == AIThinkMode.Interval && Time.get_time() > lastThinkTime + thinkRate)
		{
			return true;
		}
		return false;
	}

	public virtual void DoThink()
	{
		float delta = Time.get_time() - lastThinkTime;
		Think(delta);
	}

	public List<AIState> GetStateList()
	{
		return Enumerable.ToList<AIState>((IEnumerable<AIState>)states.Keys);
	}

	public T GetEntity()
	{
		return base.baseEntity;
	}

	public void Start()
	{
		AddStates();
		InitializeAI();
	}

	public virtual void AddStates()
	{
		states = new Dictionary<AIState, BasicAIState>();
	}

	public virtual void InitializeAI()
	{
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		T entity = GetEntity();
		entity.HasBrain = true;
		Navigator = ((Component)this).GetComponent<BaseNavigator>();
		if (UseAIDesign)
		{
			AIDesign = new AIDesign();
			AIDesign.SetAvailableStates(GetStateList());
			if (Events == null)
			{
				Events = new AIEvents();
			}
			bool senseFriendlies = MaxGroupSize > 0;
			Senses.Init(entity, MemoryDuration, SenseRange, TargetLostRange, VisionCone, CheckVisionCone, CheckLOS, IgnoreNonVisionSneakers, ListenRange, HostileTargetsOnly, senseFriendlies, IgnoreSafeZonePlayers, SenseTypes, RefreshKnownLOS);
			if (DefaultDesignSO == null && Designs.Count == 0)
			{
				Debug.LogWarning((object)("Brain on " + ((Object)((Component)this).get_gameObject()).get_name() + " is trying to load a null AI design!"));
				return;
			}
			Events.Memory.Position.Set(((Component)this).get_transform().get_position(), 4);
			if (Designs.Count == 0)
			{
				Designs.Add(DefaultDesignSO);
			}
			loadedDesignIndex = 0;
			LoadAIDesign(AIDesigns.GetByNameOrInstance(Designs[loadedDesignIndex].Filename, InstanceSpecificDesign), null, loadedDesignIndex);
			AIInformationZone forPoint = AIInformationZone.GetForPoint(((Component)this).get_transform().get_position(), fallBackToNearest: false);
			if ((Object)(object)forPoint != (Object)null)
			{
				forPoint.RegisterSleepableEntity(this);
			}
		}
		BaseEntity.Query.Server.AddBrain(entity);
		StartMovementTick();
	}

	public virtual void OnDestroy()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isQuitting)
		{
			T entity = GetEntity();
			if (entity.isServer)
			{
				BaseEntity.Query.Server.RemoveBrain(entity);
			}
			AIInformationZone aIInformationZone = null;
			HumanNPC humanNPC;
			if ((humanNPC = entity as HumanNPC) != null)
			{
				aIInformationZone = humanNPC.VirtualInfoZone;
			}
			if ((Object)(object)aIInformationZone == (Object)null)
			{
				aIInformationZone = AIInformationZone.GetForPoint(((Component)this).get_transform().get_position());
			}
			if ((Object)(object)aIInformationZone != (Object)null)
			{
				aIInformationZone.UnregisterSleepableEntity(this);
			}
			LeaveGroup();
		}
	}

	private void StartMovementTick()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)TickMovement);
		((FacepunchBehaviour)this).InvokeRandomized((Action)TickMovement, 1f, 0.1f, 0.010000001f);
	}

	private void StopMovementTick()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)TickMovement);
	}

	public void TickMovement()
	{
		if (BasePet.queuedMovementsAllowed && UseQueuedMovementUpdates && (Object)(object)Navigator != (Object)null)
		{
			if (BasePet.onlyQueueBaseNavMovements && Navigator.CurrentNavigationType != BaseNavigator.NavigationType.Base)
			{
				DoMovementTick();
				return;
			}
			BasePet basePet = GetEntity() as BasePet;
			if ((Object)(object)basePet != (Object)null && !basePet.inQueue)
			{
				BasePet._movementProcessQueue.Enqueue(basePet);
				basePet.inQueue = true;
			}
		}
		else
		{
			DoMovementTick();
		}
	}

	public void DoMovementTick()
	{
		float delta = Time.get_realtimeSinceStartup() - lastMovementTickTime;
		lastMovementTickTime = Time.get_realtimeSinceStartup();
		if ((Object)(object)Navigator != (Object)null)
		{
			Navigator.Think(delta);
		}
	}

	public void AddState(BasicAIState newState)
	{
		if (states.ContainsKey(newState.StateType))
		{
			Debug.LogWarning((object)("Trying to add duplicate state: " + newState.StateType.ToString() + " to " + GetBaseEntity().PrefabName));
			return;
		}
		newState.brain = this;
		newState.Reset();
		states.Add(newState.StateType, newState);
	}

	protected bool SwitchToState(AIState newState, int stateContainerID = -1)
	{
		if (states.ContainsKey(newState))
		{
			bool num = SwitchToState(states[newState], stateContainerID);
			if (num)
			{
				OnStateChanged();
			}
			return num;
		}
		return false;
	}

	private bool SwitchToState(BasicAIState newState, int stateContainerID = -1)
	{
		if (newState == null || !newState.CanEnter())
		{
			return false;
		}
		if (CurrentState != null)
		{
			if (!CurrentState.CanLeave())
			{
				return false;
			}
			if (CurrentState == newState && !UseAIDesign)
			{
				return false;
			}
			CurrentState.StateLeave();
		}
		AddEvents(stateContainerID);
		CurrentState = newState;
		CurrentState.StateEnter();
		currentStateContainerID = stateContainerID;
		return true;
	}

	protected virtual void OnStateChanged()
	{
	}

	private void AddEvents(int stateContainerID)
	{
		if (UseAIDesign && AIDesign != null)
		{
			Events.Init(this, AIDesign.GetStateContainerByID(stateContainerID), base.baseEntity, Senses);
		}
	}

	public virtual void Think(float delta)
	{
		if (!AI.think)
		{
			return;
		}
		lastThinkTime = Time.get_time();
		if (sleeping || disabled)
		{
			return;
		}
		Age += delta;
		if (UseAIDesign)
		{
			Senses.Update();
			UpdateGroup();
		}
		if (CurrentState != null)
		{
			UpdateAgressionTimer(delta);
			StateStatus stateStatus = CurrentState.StateThink(delta);
			if (Events != null)
			{
				Events.Tick(delta, stateStatus);
			}
		}
		if (UseAIDesign || (CurrentState != null && !CurrentState.CanLeave()))
		{
			return;
		}
		float num = 0f;
		BasicAIState basicAIState = null;
		foreach (BasicAIState value in states.Values)
		{
			if (value != null && value.CanEnter())
			{
				float weight = value.GetWeight();
				if (weight > num)
				{
					num = weight;
					basicAIState = value;
				}
			}
		}
		if (basicAIState != CurrentState)
		{
			SwitchToState(basicAIState);
		}
	}

	private void UpdateAgressionTimer(float delta)
	{
		if (CurrentState == null)
		{
			Senses.TimeInAgressiveState = 0f;
		}
		else if (CurrentState.AgrresiveState)
		{
			Senses.TimeInAgressiveState += delta;
		}
		else
		{
			Senses.TimeInAgressiveState = 0f;
		}
	}

	bool IAISleepable.AllowedToSleep()
	{
		return AllowedToSleep;
	}

	void IAISleepable.SleepAI()
	{
		if (!sleeping)
		{
			sleeping = true;
			if ((Object)(object)Navigator != (Object)null)
			{
				Navigator.Pause();
			}
			StopMovementTick();
		}
	}

	void IAISleepable.WakeAI()
	{
		if (sleeping)
		{
			sleeping = false;
			if ((Object)(object)Navigator != (Object)null)
			{
				Navigator.Resume();
			}
			StartMovementTick();
		}
	}

	private void UpdateGroup()
	{
		if (!AI.groups || MaxGroupSize <= 0 || InGroup() || Senses.Memory.Friendlies.Count <= 0)
		{
			return;
		}
		IAIGroupable iAIGroupable = null;
		foreach (BaseEntity friendly in Senses.Memory.Friendlies)
		{
			if ((Object)(object)friendly == (Object)null)
			{
				continue;
			}
			IAIGroupable component = ((Component)friendly).GetComponent<IAIGroupable>();
			if (component != null)
			{
				if (component.InGroup() && component.AddMember(this))
				{
					break;
				}
				if (iAIGroupable == null && !component.InGroup())
				{
					iAIGroupable = component;
				}
			}
		}
		if (!InGroup() && iAIGroupable != null)
		{
			AddMember(iAIGroupable);
		}
	}

	public bool AddMember(IAIGroupable member)
	{
		if (InGroup() && !IsGroupLeader)
		{
			return GroupLeader.AddMember(member);
		}
		if (MaxGroupSize <= 0)
		{
			return false;
		}
		if (groupMembers.Contains(member))
		{
			return true;
		}
		if (groupMembers.Count + 1 >= MaxGroupSize)
		{
			return false;
		}
		groupMembers.Add(member);
		IsGrouped = true;
		IsGroupLeader = true;
		GroupLeader = this;
		T entity = GetEntity();
		Events.Memory.Entity.Set(entity, 6);
		member.JoinGroup(this, entity);
		return true;
	}

	public void JoinGroup(IAIGroupable leader, BaseEntity leaderEntity)
	{
		Events.Memory.Entity.Set(leaderEntity, 6);
		GroupLeader = leader;
		IsGroupLeader = false;
		IsGrouped = true;
	}

	public void SetGroupRoamRootPosition(Vector3 rootPos)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (IsGroupLeader)
		{
			foreach (IAIGroupable groupMember in groupMembers)
			{
				groupMember.SetGroupRoamRootPosition(rootPos);
			}
		}
		Events.Memory.Position.Set(rootPos, 5);
	}

	public bool InGroup()
	{
		return IsGrouped;
	}

	public void LeaveGroup()
	{
		if (!InGroup())
		{
			return;
		}
		if (IsGroupLeader)
		{
			if (groupMembers.Count == 0)
			{
				return;
			}
			IAIGroupable iAIGroupable = groupMembers[0];
			if (iAIGroupable == null)
			{
				return;
			}
			RemoveMember(iAIGroupable);
			for (int num = groupMembers.Count - 1; num >= 0; num--)
			{
				IAIGroupable iAIGroupable2 = groupMembers[num];
				if (iAIGroupable2 != null && iAIGroupable2 != iAIGroupable)
				{
					RemoveMember(iAIGroupable2);
					iAIGroupable.AddMember(iAIGroupable2);
				}
			}
			groupMembers.Clear();
		}
		else if (GroupLeader != null)
		{
			GroupLeader.RemoveMember(((Component)this).GetComponent<IAIGroupable>());
		}
	}

	public void RemoveMember(IAIGroupable member)
	{
		if (member != null && IsGroupLeader && groupMembers.Contains(member))
		{
			groupMembers.Remove(member);
			member.SetUngrouped();
			if (groupMembers.Count == 0)
			{
				SetUngrouped();
			}
		}
	}

	public void SetUngrouped()
	{
		IsGrouped = false;
		IsGroupLeader = false;
		GroupLeader = null;
	}

	private void SendStateChangeEvent(int previousStateID, int newStateID, int sourceEventID)
	{
		if ((Object)(object)DesigningPlayer != (Object)null)
		{
			DesigningPlayer.ClientRPCPlayer(null, DesigningPlayer, "OnDebugAIEventTriggeredStateChange", previousStateID, newStateID, sourceEventID);
		}
	}

	public void EventTriggeredStateChange(int newStateContainerID, int sourceEventID)
	{
		if (AIDesign != null && newStateContainerID != -1)
		{
			AIStateContainer stateContainerByID = AIDesign.GetStateContainerByID(newStateContainerID);
			int previousStateID = currentStateContainerID;
			SwitchToState(stateContainerByID.State, newStateContainerID);
			SendStateChangeEvent(previousStateID, currentStateContainerID, sourceEventID);
		}
	}
}
