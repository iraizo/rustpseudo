using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseTrain : BaseVehicle, TriggerHurtNotChild.IHurtTriggerUser, TrainTrackSpline.ITrainTrackUser, ITrainCollidable
{
	private enum StaticCollisionState
	{
		Free,
		StaticColliding,
		StayingStill
	}

	private StaticCollisionState staticCollidingAtFront;

	private StaticCollisionState staticCollidingAtRear;

	private const float MIN_COLLISION_FORCE = 70000f;

	private float nextCollisionFXTime;

	private const float MIN_TIME_BETWEEN_COLLISION_FX = 0.5f;

	private Dictionary<Rigidbody, float> prevTrackSpeeds = new Dictionary<Rigidbody, float>();

	protected bool trainDebug;

	private TrainTrackSpline _frontTrackSection;

	private float lastMovingTime = float.MinValue;

	private const float SLEEP_SPEED = 0.25f;

	private const float SLEEP_DELAY = 10f;

	private float distFrontToBackWheel;

	private float initialSpawnTime;

	[Header("Base Train")]
	[SerializeField]
	private float corpseSeconds = 60f;

	[SerializeField]
	private TriggerTrainCollisions frontCollisionTrigger;

	[SerializeField]
	private TriggerTrainCollisions rearCollisionTrigger;

	[Tooltip("How much impact energy is retained on collisions. 1.0 = 100% retained, 0.0 = 100% loss of energy")]
	[SerializeField]
	private float impactEnergyFraction = 0.75f;

	[SerializeField]
	private float collisionDamageDivide = 100000f;

	[SerializeField]
	private float derailCollisionForce = 130000f;

	[SerializeField]
	private GameObjectRef collisionEffect;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	[SerializeField]
	private CapsuleCollider frontWheelWorldCol;

	[SerializeField]
	private CapsuleCollider rearWheelWorldCol;

	[SerializeField]
	private Transform centreOfMassTransform;

	[SerializeField]
	private ParticleSystemContainer[] sparks;

	[FormerlySerializedAs("brakeSparkLights")]
	[SerializeField]
	private Light[] sparkLights;

	protected TrainTrackSpline.TrackSelection curTrackSelection;

	public float TrackSpeed { get; private set; }

	public Vector3 Position => ((Component)this).get_transform().get_position();

	public float FrontWheelSplineDist { get; private set; }

	protected TrainTrackSpline FrontTrackSection
	{
		get
		{
			return _frontTrackSection;
		}
		set
		{
			if ((Object)(object)_frontTrackSection != (Object)(object)value)
			{
				if ((Object)(object)_frontTrackSection != (Object)null)
				{
					_frontTrackSection.DeregisterTrackUser(this);
				}
				_frontTrackSection = value;
				if ((Object)(object)_frontTrackSection != (Object)null)
				{
					_frontTrackSection.RegisterTrackUser(this);
				}
			}
		}
	}

	protected TrainTrackSpline RearTrackSection { get; private set; }

	protected bool IsAtAStation
	{
		get
		{
			if ((Object)(object)FrontTrackSection != (Object)null)
			{
				return FrontTrackSection.isStation;
			}
			return false;
		}
	}

	private bool RecentlySpawned => Time.get_time() < initialSpawnTime + 2f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseTrain.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void ReduceSpeedBy(float velChange)
	{
		if (TrackSpeed > 0f)
		{
			TrackSpeed = Mathf.Max(0f, TrackSpeed - velChange);
		}
		else if (TrackSpeed < 0f)
		{
			TrackSpeed = Mathf.Min(0f, TrackSpeed + velChange);
		}
	}

	public float GetTotalPushingForces(Vector3 pushDirection, List<BaseTrain> prevTrains = null)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (prevTrains == null)
		{
			prevTrains = Pool.GetList<BaseTrain>();
		}
		else if (prevTrains.Contains(this))
		{
			Debug.LogWarning((object)"GetTotalPushingForces: Recursive loop detected. Bailing out.");
			Pool.FreeList<BaseTrain>(ref prevTrains);
			return 0f;
		}
		prevTrains.Add(this);
		bool num = Vector3.Dot(((Component)this).get_transform().get_forward(), pushDirection) >= 0f;
		TriggerTrainCollisions triggerTrainCollisions = (num ? frontCollisionTrigger : rearCollisionTrigger);
		float num2 = GetEngineForces();
		if (!num)
		{
			num2 *= -1f;
		}
		Enumerator<BaseTrain> enumerator = triggerTrainCollisions.trainContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseTrain current = enumerator.get_Current();
				if (!((Object)(object)current == (Object)(object)this))
				{
					num2 += current.GetTotalPushingForces(pushDirection, prevTrains);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		Pool.FreeList<BaseTrain>(ref prevTrains);
		return num2;
	}

	public void FreeStaticCollision()
	{
		staticCollidingAtFront = StaticCollisionState.Free;
		staticCollidingAtRear = StaticCollisionState.Free;
	}

	private float ApplyCollisionsToTrackSpeed(float trackSpeed, float deltaTime)
	{
		trackSpeed = ApplyCollisions(trackSpeed, atOurFront: true, frontCollisionTrigger, ref staticCollidingAtFront, deltaTime);
		trackSpeed = ApplyCollisions(trackSpeed, atOurFront: false, rearCollisionTrigger, ref staticCollidingAtRear, deltaTime);
		Rigidbody val = null;
		foreach (KeyValuePair<Rigidbody, float> prevTrackSpeed in prevTrackSpeeds)
		{
			if ((Object)(object)prevTrackSpeed.Key == (Object)null || (!frontCollisionTrigger.otherRigidbodyContents.Contains(prevTrackSpeed.Key) && !rearCollisionTrigger.otherRigidbodyContents.Contains(prevTrackSpeed.Key)))
			{
				val = prevTrackSpeed.Key;
				break;
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			prevTrackSpeeds.Remove(val);
		}
		return trackSpeed;
	}

	private float ApplyCollisions(float trackSpeed, bool atOurFront, TriggerTrainCollisions trigger, ref StaticCollisionState wasStaticColliding, float deltaTime)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		bool hasAnyStaticContents = trigger.HasAnyStaticContents;
		float num;
		if (!hasAnyStaticContents)
		{
			num = 0f;
		}
		else
		{
			Vector3 velocity = rigidBody.get_velocity();
			num = ((Vector3)(ref velocity)).get_magnitude() * rigidBody.get_mass();
		}
		float num2 = num;
		trackSpeed = HandleStaticCollisions(hasAnyStaticContents, atOurFront, trackSpeed, ref wasStaticColliding);
		if (!hasAnyStaticContents)
		{
			Enumerator<BaseTrain> enumerator = trigger.trainContents.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BaseTrain current = enumerator.get_Current();
					trackSpeed = HandleTrainCollision(atOurFront, trackSpeed, current, deltaTime, ref wasStaticColliding);
					num2 += Vector3.Magnitude(current.rigidBody.get_velocity() - rigidBody.get_velocity()) * current.rigidBody.get_mass();
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			Enumerator<Rigidbody> enumerator2 = trigger.otherRigidbodyContents.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Rigidbody current2 = enumerator2.get_Current();
					trackSpeed = HandleRigidbodyCollision(atOurFront, trackSpeed, current2, current2.get_mass(), deltaTime, calcSecondaryForces: true);
					num2 += Vector3.Magnitude(current2.get_velocity() - rigidBody.get_velocity()) * current2.get_mass();
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
		}
		if (ApplyCollisionDamage(num2) > 5f && collisionEffect.isValid && Time.get_time() > nextCollisionFXTime)
		{
			Enumerator<Collider> enumerator3 = trigger.colliderContents.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					Collider current3 = enumerator3.get_Current();
					Effect.server.Run(collisionEffect.resourcePath, current3.ClosestPointOnBounds(((Component)this).get_transform().get_position()), ((Component)this).get_transform().get_up());
				}
			}
			finally
			{
				((IDisposable)enumerator3).Dispose();
			}
			nextCollisionFXTime = Time.get_time() + 0.5f;
		}
		return trackSpeed;
	}

	private float HandleStaticCollisions(bool staticColliding, bool front, float trackSpeed, ref StaticCollisionState wasStaticColliding)
	{
		float num = (front ? (-5f) : 5f);
		if (staticColliding && (front ? (trackSpeed > num) : (trackSpeed < num)))
		{
			trackSpeed = num;
			wasStaticColliding = StaticCollisionState.StaticColliding;
		}
		else if (wasStaticColliding == StaticCollisionState.StaticColliding)
		{
			trackSpeed = 0f;
			wasStaticColliding = StaticCollisionState.StayingStill;
		}
		else if (wasStaticColliding == StaticCollisionState.StayingStill)
		{
			if (front ? (trackSpeed > 0.01f) : (trackSpeed < -0.01f))
			{
				trackSpeed = 0f;
			}
			else
			{
				wasStaticColliding = StaticCollisionState.Free;
			}
		}
		return trackSpeed;
	}

	private float HandleTrainCollision(bool front, float trackSpeed, BaseTrain theirTrain, float deltaTime, ref StaticCollisionState wasStaticColliding)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pushDirection = (front ? ((Component)this).get_transform().get_forward() : (-((Component)this).get_transform().get_forward()));
		float num = Vector3.Angle(((Component)this).get_transform().get_forward(), ((Component)theirTrain).get_transform().get_forward());
		Vector3 val = ((Component)theirTrain).get_transform().get_position() - ((Component)this).get_transform().get_position();
		float num2 = Vector3.Dot(((Component)this).get_transform().get_forward(), ((Vector3)(ref val)).get_normalized());
		if ((num > 30f && num < 150f) || Mathf.Abs(num2) < 0.975f)
		{
			trackSpeed = (front ? (-0.5f) : 0.5f);
		}
		else
		{
			float totalPushingMass = theirTrain.GetTotalPushingMass(pushDirection);
			trackSpeed = ((!(totalPushingMass < 0f)) ? HandleRigidbodyCollision(front, trackSpeed, theirTrain.rigidBody, totalPushingMass, deltaTime, calcSecondaryForces: false) : HandleStaticCollisions(staticColliding: true, front, trackSpeed, ref wasStaticColliding));
			float num3 = theirTrain.GetTotalPushingForces(pushDirection);
			if (!front)
			{
				num3 *= -1f;
			}
			trackSpeed += num3 / rigidBody.get_mass() * deltaTime;
		}
		return trackSpeed;
	}

	private float HandleRigidbodyCollision(bool atOurFront, float trackSpeed, Rigidbody theirRB, float theirTotalMass, float deltaTime, bool calcSecondaryForces)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(((Component)this).get_transform().get_forward(), theirRB.get_velocity());
		float num2 = trackSpeed - num;
		if ((atOurFront && num2 <= 0f) || (!atOurFront && num2 >= 0f))
		{
			return trackSpeed;
		}
		float num3 = num2 / deltaTime * theirTotalMass * impactEnergyFraction;
		if (calcSecondaryForces)
		{
			if (prevTrackSpeeds.ContainsKey(theirRB))
			{
				float num4 = num2 / deltaTime * rigidBody.get_mass() * impactEnergyFraction / theirTotalMass * deltaTime;
				float num5 = prevTrackSpeeds[theirRB] - num;
				num3 -= Mathf.Clamp((num5 - num4) * rigidBody.get_mass(), 0f, 1000000f);
				prevTrackSpeeds[theirRB] = num;
			}
			else
			{
				prevTrackSpeeds.Add(theirRB, num);
			}
		}
		float num6 = num3 / rigidBody.get_mass() * deltaTime;
		trackSpeed -= num6;
		return trackSpeed;
	}

	private float ApplyCollisionDamage(float forceMagnitude)
	{
		if (forceMagnitude < 70000f)
		{
			return 0f;
		}
		float num = ((!(forceMagnitude > derailCollisionForce)) ? (Mathf.Pow(forceMagnitude, 1.4f) / collisionDamageDivide) : float.MaxValue);
		Hurt(num, DamageType.Collision, this, useProtection: false);
		return num;
	}

	protected bool HasAnyCollisions()
	{
		if (!frontCollisionTrigger.HasAnyContents)
		{
			return rearCollisionTrigger.HasAnyContents;
		}
		return true;
	}

	private float GetTotalPushingMass(Vector3 pushDirection, List<BaseTrain> prevTrains = null)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (prevTrains == null)
		{
			prevTrains = Pool.GetList<BaseTrain>();
		}
		else if (prevTrains.Contains(this))
		{
			Debug.LogWarning((object)"GetTotalPushingMass: Recursive loop detected. Bailing out.");
			Pool.FreeList<BaseTrain>(ref prevTrains);
			return 0f;
		}
		prevTrains.Add(this);
		bool flag = Vector3.Dot(((Component)this).get_transform().get_forward(), pushDirection) >= 0f;
		if ((flag ? staticCollidingAtFront : staticCollidingAtRear) != 0)
		{
			Pool.FreeList<BaseTrain>(ref prevTrains);
			return -1f;
		}
		TriggerTrainCollisions triggerTrainCollisions = (flag ? frontCollisionTrigger : rearCollisionTrigger);
		float num = rigidBody.get_mass();
		Enumerator<BaseTrain> enumerator = triggerTrainCollisions.trainContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseTrain current = enumerator.get_Current();
				if (!((Object)(object)current == (Object)(object)this))
				{
					float totalPushingMass = current.GetTotalPushingMass(pushDirection, prevTrains);
					if (totalPushingMass < 0f)
					{
						Pool.FreeList<BaseTrain>(ref prevTrains);
						return -1f;
					}
					num += totalPushingMass;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		Enumerator<Rigidbody> enumerator2 = triggerTrainCollisions.otherRigidbodyContents.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				Rigidbody current2 = enumerator2.get_Current();
				num += current2.get_mass();
			}
		}
		finally
		{
			((IDisposable)enumerator2).Dispose();
		}
		Pool.FreeList<BaseTrain>(ref prevTrains);
		return num;
	}

	public override void ServerInit()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		distFrontToBackWheel = Vector3.Distance(GetFrontWheelPos(), GetRearWheelPos());
		lastMovingTime = Time.get_time();
		rigidBody.set_centerOfMass(centreOfMassTransform.get_localPosition());
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.health <= 0f)
		{
			ActualDeath();
		}
	}

	public override void Spawn()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.Spawn();
		initialSpawnTime = Time.get_time();
		if (TrainTrackSpline.TryFindTrackNearby(GetFrontWheelPos(), 2f, out var splineResult, out var distResult) && splineResult.HasClearTrackSpaceNear(this))
		{
			FrontWheelSplineDist = distResult;
			SetTheRestFromFrontWheelData(ref splineResult, splineResult.GetPosition(FrontWheelSplineDist));
			FrontTrackSection = splineResult;
		}
		else
		{
			Kill();
		}
	}

	public override void Hurt(HitInfo info)
	{
		if (!RecentlySpawned)
		{
			base.Hurt(info);
		}
	}

	public override void OnKilled(HitInfo info)
	{
		float num = info.damageTypes.Get(DamageType.AntiVehicle);
		float num2 = info.damageTypes.Get(DamageType.Explosion);
		float num3 = info.damageTypes.Total();
		if ((num + num2) / num3 > 0.5f || vehicle.cinematictrains)
		{
			if (HasDriver())
			{
				GetDriver().Hurt(float.MaxValue);
			}
			base.OnKilled(info);
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)ActualDeath, corpseSeconds);
		}
	}

	public void ActualDeath()
	{
		Kill(DestroyMode.Gib);
	}

	public override void DoRepair(BasePlayer player)
	{
		base.DoRepair(player);
		if (IsDead() && Health() > 0f)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ActualDeath);
			lifestate = LifeState.Alive;
		}
	}

	public float GetPlayerDamageMultiplier()
	{
		return Mathf.Abs(TrackSpeed) * 1f;
	}

	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	internal override void DoServerDestroy()
	{
		if ((Object)(object)FrontTrackSection != (Object)null)
		{
			FrontTrackSection.DeregisterTrackUser(this);
		}
		base.DoServerDestroy();
	}

	public override bool MountEligable(BasePlayer player)
	{
		if (IsDead())
		{
			return false;
		}
		return base.MountEligable(player);
	}

	public override float MaxVelocity()
	{
		return 25f;
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_forward() * TrackSpeed;
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float num = 0f;
		if (!rigidBody.IsSleeping() || IsOn() || HasAnyCollisions())
		{
			num = FixedUpdateMoveTrain(Time.get_fixedDeltaTime());
		}
		((Component)hurtTriggerFront).get_gameObject().SetActive(num > hurtTriggerMinSpeed);
		((Component)hurtTriggerRear).get_gameObject().SetActive(num < 0f - hurtTriggerMinSpeed);
	}

	private float FixedUpdateMoveTrain(float deltaTime)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!IsFullySpawned())
		{
			return 0f;
		}
		if (rigidBody.IsSleeping())
		{
			rigidBody.WakeUp();
			lastMovingTime = Time.get_time();
		}
		float num = FixedUpdateTrainOnTrack(deltaTime);
		if (!(Mathf.Abs(num) > 0.25f))
		{
			Vector3 angularVelocity = rigidBody.get_angularVelocity();
			if (!(Mathf.Abs(((Vector3)(ref angularVelocity)).get_magnitude()) > 0.25f))
			{
				goto IL_006f;
			}
		}
		lastMovingTime = Time.get_time();
		goto IL_006f;
		IL_006f:
		if (!HasDriver() && !HasAnyCollisions() && Time.get_time() > lastMovingTime + 10f)
		{
			rigidBody.Sleep();
		}
		return num;
	}

	public Vector3 GetFrontOfTrainPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_rotation() * (((Bounds)(ref bounds)).get_center() + Vector3.get_forward() * ((Bounds)(ref bounds)).get_extents().z);
	}

	public Vector3 GetRearOfTrainPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_rotation() * (((Bounds)(ref bounds)).get_center() - Vector3.get_forward() * ((Bounds)(ref bounds)).get_extents().z);
	}

	private Vector3 GetFrontWheelPos()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)frontWheelWorldCol).get_transform().get_position() + ((Component)frontWheelWorldCol).get_transform().get_rotation() * frontWheelWorldCol.get_center();
	}

	private Vector3 GetRearWheelPos()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)rearWheelWorldCol).get_transform().get_position() + ((Component)rearWheelWorldCol).get_transform().get_rotation() * rearWheelWorldCol.get_center();
	}

	private float FixedUpdateTrainOnTrack(float deltaTime)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		float engineForces = GetEngineForces();
		TrackSpeed += engineForces / rigidBody.get_mass() * deltaTime;
		if (TrackSpeed > 0f)
		{
			TrackSpeed -= rigidBody.get_drag() * 5f * deltaTime;
			if (TrackSpeed < 0f)
			{
				TrackSpeed = 0f;
			}
		}
		else if (TrackSpeed < 0f)
		{
			TrackSpeed += rigidBody.get_drag() * 5f * deltaTime;
			if (TrackSpeed > 0f)
			{
				TrackSpeed = 0f;
			}
		}
		float num = ((Component)this).get_transform().get_localEulerAngles().x;
		if (num > 180f)
		{
			num -= 360f;
		}
		float num2 = num / 90f * Physics.get_gravity().y;
		TrackSpeed += num2 * deltaTime;
		TrackSpeed = ApplyCollisionsToTrackSpeed(TrackSpeed, deltaTime);
		float distMoved = TrackSpeed * deltaTime;
		TrainTrackSpline preferredAltTrack = (((Object)(object)RearTrackSection != (Object)(object)FrontTrackSection) ? RearTrackSection : null);
		FrontWheelSplineDist = FrontTrackSection.GetSplineDistAfterMove(FrontWheelSplineDist, ((Component)this).get_transform().get_forward(), distMoved, curTrackSelection, out var onSpline, out var _, preferredAltTrack);
		Vector3 position = onSpline.GetPosition(FrontWheelSplineDist);
		SetTheRestFromFrontWheelData(ref onSpline, position);
		FrontTrackSection = onSpline;
		return TrackSpeed;
	}

	private void SetTheRestFromFrontWheelData(ref TrainTrackSpline frontTS, Vector3 targetFrontWheelPos)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		TrainTrackSpline onSpline;
		bool atEndOfLine;
		float splineDistAfterMove = frontTS.GetSplineDistAfterMove(FrontWheelSplineDist, ((Component)this).get_transform().get_forward(), 0f - distFrontToBackWheel, curTrackSelection, out onSpline, out atEndOfLine, RearTrackSection);
		Vector3 position = onSpline.GetPosition(splineDistAfterMove);
		if (atEndOfLine)
		{
			FrontWheelSplineDist = onSpline.GetSplineDistAfterMove(splineDistAfterMove, ((Component)this).get_transform().get_forward(), distFrontToBackWheel, curTrackSelection, out frontTS, out var _, onSpline);
		}
		RearTrackSection = onSpline;
		Vector3 val = targetFrontWheelPos - position;
		Vector3 val2 = targetFrontWheelPos - val * 0.5f;
		rigidBody.MovePosition(val2);
		if (((Vector3)(ref val)).get_magnitude() == 0f)
		{
			rigidBody.MoveRotation(Quaternion.get_identity());
		}
		else
		{
			rigidBody.MoveRotation(Quaternion.LookRotation(val));
		}
		if (Application.get_isEditor())
		{
			Debug.DrawLine(targetFrontWheelPos, position, Color.get_magenta(), 0.2f);
			Debug.DrawLine(rigidBody.get_position(), val2, Color.get_yellow(), 0.2f);
			Debug.DrawRay(val2, Vector3.get_up(), Color.get_yellow(), 0.2f);
		}
	}

	public virtual float GetEngineForces()
	{
		return 0f;
	}

	public bool CustomCollision(BaseTrain train, TriggerTrainCollisions trainTrigger)
	{
		return false;
	}

	public override float InheritedVelocityScale()
	{
		return 0.5f;
	}

	protected virtual void SetTrackSelection(TrainTrackSpline.TrackSelection trackSelection)
	{
		if (curTrackSelection != trackSelection)
		{
			curTrackSelection = trackSelection;
			if (base.isServer)
			{
				ClientRPC(null, "SetTrackSelection", (sbyte)curTrackSelection);
			}
		}
	}
}
