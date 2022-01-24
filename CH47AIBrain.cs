using UnityEngine;

public class CH47AIBrain : BaseAIBrain<CH47HelicopterAIController>
{
	public class DropCrate : BasicAIState
	{
		private float nextDropTime;

		public DropCrate()
			: base(AIState.DropCrate)
		{
		}

		public override bool CanInterrupt()
		{
			if (base.CanInterrupt())
			{
				return !CanDrop();
			}
			return false;
		}

		public bool CanDrop()
		{
			if (Time.get_time() > nextDropTime)
			{
				return brain.GetEntity().CanDropCrate();
			}
			return false;
		}

		public override float GetWeight()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if (!CanDrop())
			{
				return 0f;
			}
			if (IsInState())
			{
				return 10000f;
			}
			if (brain.CurrentState != null && brain.CurrentState.StateType == AIState.Orbit && brain.CurrentState.TimeInState > 60f)
			{
				CH47DropZone closest = CH47DropZone.GetClosest(brain.mainInterestPoint);
				if (Object.op_Implicit((Object)(object)closest) && Vector3Ex.Distance2D(((Component)closest).get_transform().get_position(), brain.mainInterestPoint) < 200f)
				{
					CH47AIBrain component = ((Component)brain).GetComponent<CH47AIBrain>();
					if ((Object)(object)component != (Object)null)
					{
						float num = Mathf.InverseLerp(300f, 600f, component.Age);
						return 1000f * num;
					}
				}
			}
			return 0f;
		}

		public override void StateEnter()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			brain.GetEntity().SetDropDoorOpen(open: true);
			brain.GetEntity().EnableFacingOverride(enabled: false);
			CH47DropZone closest = CH47DropZone.GetClosest(((Component)brain.GetEntity()).get_transform().get_position());
			if ((Object)(object)closest == (Object)null)
			{
				nextDropTime = Time.get_time() + 60f;
			}
			brain.mainInterestPoint = ((Component)closest).get_transform().get_position();
			brain.GetEntity().SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			if (CanDrop() && Vector3Ex.Distance2D(brain.mainInterestPoint, ((Component)brain.GetEntity()).get_transform().get_position()) < 5f)
			{
				Vector3 velocity = brain.GetEntity().rigidBody.get_velocity();
				if (((Vector3)(ref velocity)).get_magnitude() < 5f)
				{
					brain.GetEntity().DropCrate();
					nextDropTime = Time.get_time() + 120f;
				}
			}
			return StateStatus.Running;
		}

		public override void StateLeave()
		{
			brain.GetEntity().SetDropDoorOpen(open: false);
			nextDropTime = Time.get_time() + 60f;
			base.StateLeave();
		}
	}

	public class EgressState : BasicAIState
	{
		private bool killing;

		private bool egressAltitueAchieved;

		public EgressState()
			: base(AIState.Egress)
		{
		}

		public override bool CanInterrupt()
		{
			return false;
		}

		public override float GetWeight()
		{
			if (brain.GetEntity().OutOfCrates() && !brain.GetEntity().ShouldLand())
			{
				return 10000f;
			}
			CH47AIBrain component = ((Component)brain).GetComponent<CH47AIBrain>();
			if ((Object)(object)component != (Object)null)
			{
				if (!(component.Age > 1800f))
				{
					return 0f;
				}
				return 10000f;
			}
			return 0f;
		}

		public override void StateEnter()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			brain.GetEntity().EnableFacingOverride(enabled: false);
			Transform transform = ((Component)brain.GetEntity()).get_transform();
			Rigidbody rigidBody = brain.GetEntity().rigidBody;
			Vector3 velocity = rigidBody.get_velocity();
			Vector3 val;
			if (!(((Vector3)(ref velocity)).get_magnitude() < 0.1f))
			{
				velocity = rigidBody.get_velocity();
				val = ((Vector3)(ref velocity)).get_normalized();
			}
			else
			{
				val = transform.get_forward();
			}
			Vector3 val2 = val;
			Vector3 val3 = Vector3.Cross(Vector3.Cross(transform.get_up(), val2), Vector3.get_up());
			brain.mainInterestPoint = transform.get_position() + val3 * 8000f;
			brain.mainInterestPoint.y = 100f;
			brain.GetEntity().SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			if (killing)
			{
				return StateStatus.Running;
			}
			Vector3 position = ((Component)brain.GetEntity()).get_transform().get_position();
			if (position.y < 85f && !egressAltitueAchieved)
			{
				CH47LandingZone closest = CH47LandingZone.GetClosest(position);
				if ((Object)(object)closest != (Object)null && Vector3Ex.Distance2D(((Component)closest).get_transform().get_position(), position) < 20f)
				{
					float num = 0f;
					if ((Object)(object)TerrainMeta.HeightMap != (Object)null && (Object)(object)TerrainMeta.WaterMap != (Object)null)
					{
						num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(position), TerrainMeta.HeightMap.GetHeight(position));
					}
					num += 100f;
					Vector3 moveTarget = position;
					moveTarget.y = num;
					brain.GetEntity().SetMoveTarget(moveTarget);
					return StateStatus.Running;
				}
			}
			egressAltitueAchieved = true;
			brain.GetEntity().SetMoveTarget(brain.mainInterestPoint);
			if (base.TimeInState > 300f)
			{
				((MonoBehaviour)brain.GetEntity()).Invoke("DelayedKill", 2f);
				killing = true;
			}
			return StateStatus.Running;
		}

		public override void StateLeave()
		{
			base.StateLeave();
		}
	}

	public class IdleState : BaseIdleState
	{
		public override float GetWeight()
		{
			return 0.1f;
		}

		public override void StateEnter()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			CH47HelicopterAIController entity = brain.GetEntity();
			Vector3 position = brain.GetEntity().GetPosition();
			Vector3 velocity = brain.GetEntity().rigidBody.get_velocity();
			entity.SetMoveTarget(position + ((Vector3)(ref velocity)).get_normalized() * 10f);
			base.StateEnter();
		}
	}

	public class LandState : BasicAIState
	{
		private float landedForSeconds;

		private float lastLandtime;

		private float landingHeight = 20f;

		private float nextDismountTime;

		public LandState()
			: base(AIState.Land)
		{
		}

		public override float GetWeight()
		{
			if (!GetEntity().ShouldLand())
			{
				return 0f;
			}
			float num = Time.get_time() - lastLandtime;
			if (IsInState() && landedForSeconds < 12f)
			{
				return 1000f;
			}
			if (!IsInState() && num > 10f)
			{
				return 9000f;
			}
			return 0f;
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			Vector3 position = ((Component)brain.GetEntity()).get_transform().get_position();
			((Component)brain.GetEntity()).get_transform().get_forward();
			CH47LandingZone closest = CH47LandingZone.GetClosest(brain.GetEntity().landingTarget);
			if (!Object.op_Implicit((Object)(object)closest))
			{
				return StateStatus.Error;
			}
			Vector3 velocity = brain.GetEntity().rigidBody.get_velocity();
			float magnitude = ((Vector3)(ref velocity)).get_magnitude();
			float num = Vector3Ex.Distance2D(((Component)closest).get_transform().get_position(), position);
			bool enabled = num < 40f;
			bool altitudeProtection = num > 15f && position.y < ((Component)closest).get_transform().get_position().y + 10f;
			brain.GetEntity().EnableFacingOverride(enabled);
			brain.GetEntity().SetAltitudeProtection(altitudeProtection);
			bool num2 = Mathf.Abs(((Component)closest).get_transform().get_position().y - position.y) < 3f && num <= 5f && magnitude < 1f;
			if (num2)
			{
				landedForSeconds += delta;
				if (lastLandtime == 0f)
				{
					lastLandtime = Time.get_time();
				}
			}
			float num3 = 1f - Mathf.InverseLerp(0f, 7f, num);
			landingHeight -= 4f * num3 * Time.get_deltaTime();
			if (landingHeight < -5f)
			{
				landingHeight = -5f;
			}
			brain.GetEntity().SetAimDirection(((Component)closest).get_transform().get_forward());
			Vector3 moveTarget = brain.mainInterestPoint + new Vector3(0f, landingHeight, 0f);
			if (num < 100f && num > 15f)
			{
				Vector3 val = Vector3Ex.Direction2D(((Component)closest).get_transform().get_position(), position);
				RaycastHit val2 = default(RaycastHit);
				if (Physics.SphereCast(position, 15f, val, ref val2, num, 1218511105))
				{
					Vector3 val3 = Vector3.Cross(val, Vector3.get_up());
					moveTarget = ((RaycastHit)(ref val2)).get_point() + val3 * 50f;
				}
			}
			brain.GetEntity().SetMoveTarget(moveTarget);
			if (num2)
			{
				if (landedForSeconds > 1f && Time.get_time() > nextDismountTime)
				{
					foreach (BaseVehicle.MountPointInfo mountPoint in brain.GetEntity().mountPoints)
					{
						if (Object.op_Implicit((Object)(object)mountPoint.mountable) && mountPoint.mountable.IsMounted())
						{
							nextDismountTime = Time.get_time() + 0.5f;
							mountPoint.mountable.DismountAllPlayers();
							break;
						}
					}
				}
				if (landedForSeconds > 8f)
				{
					((Component)brain).GetComponent<CH47AIBrain>().ForceSetAge(float.PositiveInfinity);
				}
			}
			return StateStatus.Running;
		}

		public override void StateEnter()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			brain.mainInterestPoint = GetEntity().landingTarget;
			landingHeight = 15f;
			base.StateEnter();
		}

		public override void StateLeave()
		{
			brain.GetEntity().EnableFacingOverride(enabled: false);
			brain.GetEntity().SetAltitudeProtection(on: true);
			brain.GetEntity().SetMinHoverHeight(30f);
			landedForSeconds = 0f;
			base.StateLeave();
		}

		public override bool CanInterrupt()
		{
			return true;
		}
	}

	public class OrbitState : BasicAIState
	{
		public OrbitState()
			: base(AIState.Orbit)
		{
		}

		public Vector3 GetOrbitCenter()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return brain.mainInterestPoint;
		}

		public override float GetWeight()
		{
			if (IsInState())
			{
				float num = 1f - Mathf.InverseLerp(120f, 180f, base.TimeInState);
				return 5f * num;
			}
			if (brain.CurrentState != null && brain.CurrentState.StateType == AIState.Patrol)
			{
				PatrolState patrolState = brain.CurrentState as PatrolState;
				if (patrolState != null && patrolState.AtPatrolDestination())
				{
					return 5f;
				}
			}
			return 0f;
		}

		public override void StateEnter()
		{
			brain.GetEntity().EnableFacingOverride(enabled: true);
			brain.GetEntity().InitiateAnger();
			base.StateEnter();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			Vector3 orbitCenter = GetOrbitCenter();
			CH47HelicopterAIController entity = brain.GetEntity();
			Vector3 position = entity.GetPosition();
			Vector3 val = Vector3Ex.Direction2D(orbitCenter, position);
			Vector3 val2 = Vector3.Cross(Vector3.get_up(), val);
			float num = ((Vector3.Dot(Vector3.Cross(((Component)entity).get_transform().get_right(), Vector3.get_up()), val2) < 0f) ? (-1f) : 1f);
			float num2 = 75f;
			Vector3 val3 = -val + val2 * num * 0.6f;
			Vector3 normalized = ((Vector3)(ref val3)).get_normalized();
			Vector3 val4 = orbitCenter + normalized * num2;
			entity.SetMoveTarget(val4);
			entity.SetAimDirection(Vector3Ex.Direction2D(val4, position));
			base.StateThink(delta);
			return StateStatus.Running;
		}

		public override void StateLeave()
		{
			brain.GetEntity().EnableFacingOverride(enabled: false);
			brain.GetEntity().CancelAnger();
			base.StateLeave();
		}
	}

	public class PatrolState : BasePatrolState
	{
		protected float patrolApproachDist = 75f;

		public override void StateEnter()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter();
			brain.mainInterestPoint = brain.PathFinder.GetRandomPatrolPoint();
		}

		public override StateStatus StateThink(float delta)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta);
			brain.GetEntity().SetMoveTarget(brain.mainInterestPoint);
			return StateStatus.Running;
		}

		public bool AtPatrolDestination()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return Vector3Ex.Distance2D(GetDestination(), ((Component)brain.GetEntity()).get_transform().get_position()) < patrolApproachDist;
		}

		public Vector3 GetDestination()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return brain.mainInterestPoint;
		}

		public override bool CanInterrupt()
		{
			if (base.CanInterrupt())
			{
				return AtPatrolDestination();
			}
			return false;
		}

		public override float GetWeight()
		{
			if (IsInState())
			{
				if (AtPatrolDestination() && base.TimeInState > 2f)
				{
					return 0f;
				}
				return 3f;
			}
			float num = Mathf.InverseLerp(70f, 120f, TimeSinceState()) * 5f;
			return 1f + num;
		}
	}

	public override void AddStates()
	{
		base.AddStates();
		AddState(new IdleState());
		AddState(new PatrolState());
		AddState(new OrbitState());
		AddState(new EgressState());
		AddState(new DropCrate());
		AddState(new LandState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.FixedUpdate;
		base.PathFinder = new CH47PathFinder();
	}

	public void FixedUpdate()
	{
		if (!((Object)(object)base.baseEntity == (Object)null) && !base.baseEntity.isClient)
		{
			Think(Time.get_fixedDeltaTime());
		}
	}
}
