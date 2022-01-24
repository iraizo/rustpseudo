using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

public class PatrolHelicopterAI : BaseMonoBehaviour
{
	public class targetinfo
	{
		public BasePlayer ply;

		public BaseEntity ent;

		public float lastSeenTime = float.PositiveInfinity;

		public float visibleFor;

		public float nextLOSCheck;

		public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
		{
			ply = initPly;
			ent = initEnt;
			lastSeenTime = float.PositiveInfinity;
			nextLOSCheck = Time.get_realtimeSinceStartup() + 1.5f;
		}

		public bool IsVisible()
		{
			return TimeSinceSeen() < 1.5f;
		}

		public float TimeSinceSeen()
		{
			return Time.get_realtimeSinceStartup() - lastSeenTime;
		}
	}

	public enum aiState
	{
		IDLE,
		MOVE,
		ORBIT,
		STRAFE,
		PATROL,
		GUARD,
		DEATH
	}

	public List<targetinfo> _targetList = new List<targetinfo>();

	public Vector3 interestZoneOrigin;

	public Vector3 destination;

	public bool hasInterestZone;

	public float moveSpeed;

	public float maxSpeed = 25f;

	public float courseAdjustLerpTime = 2f;

	public Quaternion targetRotation;

	public Vector3 windVec;

	public Vector3 targetWindVec;

	public float windForce = 5f;

	public float windFrequency = 1f;

	public float targetThrottleSpeed;

	public float throttleSpeed;

	public float maxRotationSpeed = 90f;

	public float rotationSpeed;

	public float terrainPushForce = 100f;

	public float obstaclePushForce = 100f;

	public HelicopterTurret leftGun;

	public HelicopterTurret rightGun;

	public static PatrolHelicopterAI heliInstance;

	public BaseHelicopter helicopterBase;

	public aiState _currentState;

	private Vector3 _aimTarget;

	private bool movementLockingAiming;

	private bool hasAimTarget;

	private bool aimDoorSide;

	private Vector3 pushVec = Vector3.get_zero();

	private Vector3 _lastPos;

	private Vector3 _lastMoveDir;

	private bool isDead;

	private bool isRetiring;

	private float spawnTime;

	private float lastDamageTime;

	private float deathTimeout;

	private float destination_min_dist = 2f;

	private float currentOrbitDistance;

	private float currentOrbitTime;

	private bool hasEnteredOrbit;

	private float orbitStartTime;

	private float maxOrbitDuration = 30f;

	private bool breakingOrbit;

	public List<MonumentInfo> _visitedMonuments;

	public float arrivalTime;

	public GameObjectRef rocketProjectile;

	public GameObjectRef rocketProjectile_Napalm;

	private bool leftTubeFiredLast;

	private float lastRocketTime;

	private float timeBetweenRockets = 0.2f;

	private int numRocketsLeft = 12;

	private const int maxRockets = 12;

	private Vector3 strafe_target_position;

	private bool puttingDistance;

	private const float strafe_approach_range = 175f;

	private const float strafe_firing_range = 150f;

	private bool useNapalm;

	[NonSerialized]
	private float lastNapalmTime = float.NegativeInfinity;

	[NonSerialized]
	private float lastStrafeTime = float.NegativeInfinity;

	private float _lastThinkTime;

	public void UpdateTargetList()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		Vector3 strafePos = Vector3.get_zero();
		bool flag = false;
		bool shouldUseNapalm = false;
		for (int num = _targetList.Count - 1; num >= 0; num--)
		{
			targetinfo targetinfo = _targetList[num];
			if (targetinfo == null || (Object)(object)targetinfo.ent == (Object)null)
			{
				_targetList.Remove(targetinfo);
			}
			else
			{
				if (Time.get_realtimeSinceStartup() > targetinfo.nextLOSCheck)
				{
					targetinfo.nextLOSCheck = Time.get_realtimeSinceStartup() + 1f;
					if (PlayerVisible(targetinfo.ply))
					{
						targetinfo.lastSeenTime = Time.get_realtimeSinceStartup();
						targetinfo.visibleFor += 1f;
					}
					else
					{
						targetinfo.visibleFor = 0f;
					}
				}
				bool flag2 = (Object.op_Implicit((Object)(object)targetinfo.ply) ? targetinfo.ply.IsDead() : (targetinfo.ent.Health() <= 0f));
				if (targetinfo.TimeSinceSeen() >= 6f || flag2)
				{
					bool flag3 = Random.Range(0f, 1f) >= 0f;
					if ((CanStrafe() || CanUseNapalm()) && IsAlive() && !flag && !flag2 && ((Object)(object)targetinfo.ply == (Object)(object)leftGun._target || (Object)(object)targetinfo.ply == (Object)(object)rightGun._target) && flag3)
					{
						shouldUseNapalm = !ValidStrafeTarget(targetinfo.ply) || Random.Range(0f, 1f) > 0.75f;
						flag = true;
						strafePos = ((Component)targetinfo.ply).get_transform().get_position();
					}
					_targetList.Remove(targetinfo);
				}
			}
		}
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				if (current.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone) || Vector3Ex.Distance2D(((Component)this).get_transform().get_position(), ((Component)current).get_transform().get_position()) > 150f)
				{
					continue;
				}
				bool flag4 = false;
				foreach (targetinfo target in _targetList)
				{
					if ((Object)(object)target.ply == (Object)(object)current)
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4 && current.GetThreatLevel() > 0.5f && PlayerVisible(current))
				{
					_targetList.Add(new targetinfo(current, current));
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (flag)
		{
			ExitCurrentState();
			State_Strafe_Enter(strafePos, shouldUseNapalm);
		}
	}

	public bool PlayerVisible(BasePlayer ply)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ply.eyes.position;
		if (TOD_Sky.get_Instance().get_IsNight() && Vector3.Distance(position, interestZoneOrigin) > 40f)
		{
			return false;
		}
		Vector3 val = ((Component)this).get_transform().get_position() - Vector3.get_up() * 6f;
		float num = Vector3.Distance(position, val);
		Vector3 val2 = position - val;
		Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
		if (GamePhysics.Trace(new Ray(val + normalized * 5f, normalized), 0f, out var hitInfo, num * 1.1f, 1218652417, (QueryTriggerInteraction)0) && (Object)(object)((Component)((RaycastHit)(ref hitInfo)).get_collider()).get_gameObject().ToBaseEntity() == (Object)(object)ply)
		{
			return true;
		}
		return false;
	}

	public void WasAttacked(HitInfo info)
	{
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			_targetList.Add(new targetinfo(basePlayer, basePlayer));
		}
	}

	public void Awake()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (PatrolHelicopter.lifetimeMinutes == 0f)
		{
			((FacepunchBehaviour)this).Invoke((Action)DestroyMe, 1f);
			return;
		}
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateWind, 0f, 1f / windFrequency);
		_lastPos = ((Component)this).get_transform().get_position();
		spawnTime = Time.get_realtimeSinceStartup();
		InitializeAI();
	}

	public void SetInitialDestination(Vector3 dest, float mapScaleDistance = 0.25f)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		hasInterestZone = true;
		interestZoneOrigin = dest;
		float x = TerrainMeta.Size.x;
		float y = dest.y + 25f;
		Vector3 val = Vector3Ex.Range(-1f, 1f);
		val.y = 0f;
		((Vector3)(ref val)).Normalize();
		val *= x * mapScaleDistance;
		val.y = y;
		if (mapScaleDistance == 0f)
		{
			val = interestZoneOrigin + new Vector3(0f, 10f, 0f);
		}
		((Component)this).get_transform().set_position(val);
		ExitCurrentState();
		State_Move_Enter(dest);
	}

	public void Retire()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (!isRetiring)
		{
			isRetiring = true;
			((FacepunchBehaviour)this).Invoke((Action)DestroyMe, 240f);
			float x = TerrainMeta.Size.x;
			float y = 200f;
			Vector3 val = Vector3Ex.Range(-1f, 1f);
			val.y = 0f;
			((Vector3)(ref val)).Normalize();
			val *= x * 20f;
			val.y = y;
			ExitCurrentState();
			State_Move_Enter(val);
		}
	}

	public void SetIdealRotation(Quaternion newTargetRot, float rotationSpeedOverride = -1f)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		float num = ((rotationSpeedOverride == -1f) ? Mathf.Clamp01(moveSpeed / (maxSpeed * 0.5f)) : rotationSpeedOverride);
		rotationSpeed = num * maxRotationSpeed;
		targetRotation = newTargetRot;
	}

	public Quaternion GetYawRotationTo(Vector3 targetDest)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = targetDest;
		val.y = 0f;
		Vector3 position = ((Component)this).get_transform().get_position();
		position.y = 0f;
		Vector3 val2 = val - position;
		return Quaternion.LookRotation(((Vector3)(ref val2)).get_normalized());
	}

	public void SetTargetDestination(Vector3 targetDest, float minDist = 5f, float minDistForFacingRotation = 30f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		destination = targetDest;
		destination_min_dist = minDist;
		float num = Vector3.Distance(targetDest, ((Component)this).get_transform().get_position());
		if (num > minDistForFacingRotation && !IsTargeting())
		{
			SetIdealRotation(GetYawRotationTo(destination));
		}
		targetThrottleSpeed = GetThrottleForDistance(num);
	}

	public bool AtDestination()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Distance(((Component)this).get_transform().get_position(), destination) < destination_min_dist;
	}

	public void MoveToDestination()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 lastMoveDir = _lastMoveDir;
		Vector3 val = destination - ((Component)this).get_transform().get_position();
		Vector3 val2 = (_lastMoveDir = Vector3.Lerp(lastMoveDir, ((Vector3)(ref val)).get_normalized(), Time.get_deltaTime() / courseAdjustLerpTime));
		throttleSpeed = Mathf.Lerp(throttleSpeed, targetThrottleSpeed, Time.get_deltaTime() / 3f);
		float num = throttleSpeed * maxSpeed;
		TerrainPushback();
		Transform transform = ((Component)this).get_transform();
		transform.set_position(transform.get_position() + val2 * num * Time.get_deltaTime());
		windVec = Vector3.Lerp(windVec, targetWindVec, Time.get_deltaTime());
		Transform transform2 = ((Component)this).get_transform();
		transform2.set_position(transform2.get_position() + windVec * windForce * Time.get_deltaTime());
		moveSpeed = Mathf.Lerp(moveSpeed, Vector3.Distance(_lastPos, ((Component)this).get_transform().get_position()) / Time.get_deltaTime(), Time.get_deltaTime() * 2f);
		_lastPos = ((Component)this).get_transform().get_position();
	}

	public void TerrainPushback()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		if (_currentState != aiState.DEATH)
		{
			Vector3 val = ((Component)this).get_transform().get_position() + new Vector3(0f, 2f, 0f);
			Vector3 val2 = destination - val;
			Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
			float num = Vector3.Distance(destination, ((Component)this).get_transform().get_position());
			Ray val3 = default(Ray);
			((Ray)(ref val3))._002Ector(val, normalized);
			float num2 = 5f;
			float num3 = Mathf.Min(100f, num);
			int mask = LayerMask.GetMask(new string[3] { "Terrain", "World", "Construction" });
			Vector3 val4 = Vector3.get_zero();
			RaycastHit val5 = default(RaycastHit);
			if (Physics.SphereCast(val3, num2, ref val5, num3 - num2 * 0.5f, mask))
			{
				float num4 = 1f - ((RaycastHit)(ref val5)).get_distance() / num3;
				float num5 = terrainPushForce * num4;
				val4 = Vector3.get_up() * num5;
			}
			Ray val6 = new Ray(val, _lastMoveDir);
			float num6 = Mathf.Min(10f, num);
			RaycastHit val7 = default(RaycastHit);
			if (Physics.SphereCast(val6, num2, ref val7, num6 - num2 * 0.5f, mask))
			{
				float num7 = 1f - ((RaycastHit)(ref val7)).get_distance() / num6;
				float num8 = obstaclePushForce * num7;
				val4 += _lastMoveDir * num8 * -1f;
				val4 += Vector3.get_up() * num8;
			}
			pushVec = Vector3.Lerp(pushVec, val4, Time.get_deltaTime());
			Transform transform = ((Component)this).get_transform();
			transform.set_position(transform.get_position() + pushVec * Time.get_deltaTime());
		}
	}

	public void UpdateRotation()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (hasAimTarget)
		{
			Vector3 position = ((Component)this).get_transform().get_position();
			position.y = 0f;
			Vector3 aimTarget = _aimTarget;
			aimTarget.y = 0f;
			Vector3 val = aimTarget - position;
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			Vector3 val2 = Vector3.Cross(normalized, Vector3.get_up());
			float num = Vector3.Angle(normalized, ((Component)this).get_transform().get_right());
			float num2 = Vector3.Angle(normalized, -((Component)this).get_transform().get_right());
			if (aimDoorSide)
			{
				if (num < num2)
				{
					targetRotation = Quaternion.LookRotation(val2);
				}
				else
				{
					targetRotation = Quaternion.LookRotation(-val2);
				}
			}
			else
			{
				targetRotation = Quaternion.LookRotation(normalized);
			}
		}
		rotationSpeed = Mathf.Lerp(rotationSpeed, maxRotationSpeed, Time.get_deltaTime() / 2f);
		((Component)this).get_transform().set_rotation(Quaternion.Lerp(((Component)this).get_transform().get_rotation(), targetRotation, rotationSpeed * Time.get_deltaTime()));
	}

	public void UpdateSpotlight()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (hasInterestZone)
		{
			helicopterBase.spotlightTarget = new Vector3(interestZoneOrigin.x, TerrainMeta.HeightMap.GetHeight(interestZoneOrigin), interestZoneOrigin.z);
		}
		else
		{
			helicopterBase.spotlightTarget = Vector3.get_zero();
		}
	}

	public void Update()
	{
		if (helicopterBase.isClient)
		{
			return;
		}
		heliInstance = this;
		UpdateTargetList();
		MoveToDestination();
		UpdateRotation();
		UpdateSpotlight();
		AIThink();
		DoMachineGuns();
		if (!isRetiring)
		{
			float num = Mathf.Max(spawnTime + PatrolHelicopter.lifetimeMinutes * 60f, lastDamageTime + 120f);
			if (Time.get_realtimeSinceStartup() > num)
			{
				Retire();
			}
		}
	}

	public void WeakspotDamaged(BaseHelicopter.weakspot weak, HitInfo info)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_realtimeSinceStartup() - lastDamageTime;
		lastDamageTime = Time.get_realtimeSinceStartup();
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		bool num2 = ValidStrafeTarget(basePlayer);
		bool flag = num2 && CanStrafe();
		bool flag2 = !num2 && CanUseNapalm();
		if (num < 5f && (Object)(object)basePlayer != (Object)null && (flag || flag2))
		{
			ExitCurrentState();
			State_Strafe_Enter(((Component)info.Initiator).get_transform().get_position(), flag2);
		}
	}

	public void CriticalDamage()
	{
		isDead = true;
		ExitCurrentState();
		State_Death_Enter();
	}

	public void DoMachineGuns()
	{
		if (_targetList.Count > 0)
		{
			if (leftGun.NeedsNewTarget())
			{
				leftGun.UpdateTargetFromList(_targetList);
			}
			if (rightGun.NeedsNewTarget())
			{
				rightGun.UpdateTargetFromList(_targetList);
			}
		}
		leftGun.TurretThink();
		rightGun.TurretThink();
	}

	public void FireGun(Vector3 targetPos, float aimCone, bool left)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (PatrolHelicopter.guns == 0)
		{
			return;
		}
		Vector3 position = (left ? helicopterBase.left_gun_muzzle.get_transform() : helicopterBase.right_gun_muzzle.get_transform()).get_position();
		Vector3 val = targetPos - position;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		position += normalized * 2f;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized);
		if (GamePhysics.Trace(new Ray(position, modifiedAimConeDirection), 0f, out var hitInfo, 300f, 1219701521, (QueryTriggerInteraction)0))
		{
			targetPos = ((RaycastHit)(ref hitInfo)).get_point();
			if (Object.op_Implicit((Object)(object)((RaycastHit)(ref hitInfo)).get_collider()))
			{
				BaseEntity entity = hitInfo.GetEntity();
				if (Object.op_Implicit((Object)(object)entity) && (Object)(object)entity != (Object)(object)helicopterBase)
				{
					BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
					HitInfo info = new HitInfo(helicopterBase, entity, DamageType.Bullet, helicopterBase.bulletDamage * PatrolHelicopter.bulletDamageScale, ((RaycastHit)(ref hitInfo)).get_point());
					if (Object.op_Implicit((Object)(object)baseCombatEntity))
					{
						baseCombatEntity.OnAttacked(info);
						if (baseCombatEntity is BasePlayer)
						{
							Effect.server.ImpactEffect(new HitInfo
							{
								HitPositionWorld = ((RaycastHit)(ref hitInfo)).get_point() - modifiedAimConeDirection * 0.25f,
								HitNormalWorld = -modifiedAimConeDirection,
								HitMaterial = StringPool.Get("Flesh")
							});
						}
					}
					else
					{
						entity.OnAttacked(info);
					}
				}
			}
		}
		else
		{
			targetPos = position + modifiedAimConeDirection * 300f;
		}
		helicopterBase.ClientRPC<bool, Vector3>(null, "FireGun", left, targetPos);
	}

	public bool CanInterruptState()
	{
		if (_currentState != aiState.STRAFE)
		{
			return _currentState != aiState.DEATH;
		}
		return false;
	}

	public bool IsAlive()
	{
		return !isDead;
	}

	public void DestroyMe()
	{
		helicopterBase.Kill();
	}

	public Vector3 GetLastMoveDir()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return _lastMoveDir;
	}

	public Vector3 GetMoveDirection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = destination - ((Component)this).get_transform().get_position();
		return ((Vector3)(ref val)).get_normalized();
	}

	public float GetMoveSpeed()
	{
		return moveSpeed;
	}

	public float GetMaxRotationSpeed()
	{
		return maxRotationSpeed;
	}

	public bool IsTargeting()
	{
		return hasAimTarget;
	}

	public void UpdateWind()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		targetWindVec = Random.get_onUnitSphere();
	}

	public void SetAimTarget(Vector3 aimTarg, bool isDoorSide)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!movementLockingAiming)
		{
			hasAimTarget = true;
			_aimTarget = aimTarg;
			aimDoorSide = isDoorSide;
		}
	}

	public void ClearAimTarget()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		hasAimTarget = false;
		_aimTarget = Vector3.get_zero();
	}

	public void State_Death_Think(float timePassed)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_realtimeSinceStartup() * 0.25f;
		float num2 = Mathf.Sin((float)Math.PI * 2f * num) * 10f;
		float num3 = Mathf.Cos((float)Math.PI * 2f * num) * 10f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num2, 0f, num3);
		SetAimTarget(((Component)this).get_transform().get_position() + val, isDoorSide: true);
		Ray val2 = default(Ray);
		((Ray)(ref val2))._002Ector(((Component)this).get_transform().get_position(), GetLastMoveDir());
		int mask = LayerMask.GetMask(new string[4] { "Terrain", "World", "Construction", "Water" });
		RaycastHit val3 = default(RaycastHit);
		if (Physics.SphereCast(val2, 3f, ref val3, 5f, mask) || Time.get_realtimeSinceStartup() > deathTimeout)
		{
			helicopterBase.Hurt(helicopterBase.health * 2f, DamageType.Generic, null, useProtection: false);
		}
	}

	public void State_Death_Enter()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		maxRotationSpeed *= 8f;
		_currentState = aiState.DEATH;
		Vector3 randomOffset = GetRandomOffset(((Component)this).get_transform().get_position(), 20f, 60f);
		int num = 1236478737;
		TransformUtil.GetGroundInfo(randomOffset - Vector3.get_up() * 2f, out var pos, out var _, 500f, LayerMask.op_Implicit(num));
		SetTargetDestination(pos);
		targetThrottleSpeed = 0.5f;
		deathTimeout = Time.get_realtimeSinceStartup() + 10f;
	}

	public void State_Death_Leave()
	{
	}

	public void State_Idle_Think(float timePassed)
	{
		ExitCurrentState();
		State_Patrol_Enter();
	}

	public void State_Idle_Enter()
	{
		_currentState = aiState.IDLE;
	}

	public void State_Idle_Leave()
	{
	}

	public void State_Move_Think(float timePassed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float distToTarget = Vector3.Distance(((Component)this).get_transform().get_position(), destination);
		targetThrottleSpeed = GetThrottleForDistance(distToTarget);
		if (AtDestination())
		{
			ExitCurrentState();
			State_Idle_Enter();
		}
	}

	public void State_Move_Enter(Vector3 newPos)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_currentState = aiState.MOVE;
		destination_min_dist = 5f;
		SetTargetDestination(newPos);
		float distToTarget = Vector3.Distance(((Component)this).get_transform().get_position(), destination);
		targetThrottleSpeed = GetThrottleForDistance(distToTarget);
	}

	public void State_Move_Leave()
	{
	}

	public void State_Orbit_Think(float timePassed)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		if (breakingOrbit)
		{
			if (AtDestination())
			{
				ExitCurrentState();
				State_Idle_Enter();
			}
		}
		else
		{
			if (Vector3Ex.Distance2D(((Component)this).get_transform().get_position(), destination) > 15f)
			{
				return;
			}
			if (!hasEnteredOrbit)
			{
				hasEnteredOrbit = true;
				orbitStartTime = Time.get_realtimeSinceStartup();
			}
			float num = (float)Math.PI * 2f * currentOrbitDistance;
			float num2 = 0.5f * maxSpeed;
			float num3 = num / num2;
			currentOrbitTime += timePassed / (num3 * 1.01f);
			float rate = currentOrbitTime;
			Vector3 orbitPosition = GetOrbitPosition(rate);
			ClearAimTarget();
			SetTargetDestination(orbitPosition, 0f, 1f);
			targetThrottleSpeed = 0.5f;
		}
		if (Time.get_realtimeSinceStartup() - orbitStartTime > maxOrbitDuration && !breakingOrbit)
		{
			breakingOrbit = true;
			Vector3 appropriatePosition = GetAppropriatePosition(((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 75f, 40f, 50f);
			SetTargetDestination(appropriatePosition, 10f, 0f);
		}
	}

	public Vector3 GetOrbitPosition(float rate)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Sin((float)Math.PI * 2f * rate) * currentOrbitDistance;
		float num2 = Mathf.Cos((float)Math.PI * 2f * rate) * currentOrbitDistance;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num, 20f, num2);
		val = interestZoneOrigin + val;
		return val;
	}

	public void State_Orbit_Enter(float orbitDistance)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		_currentState = aiState.ORBIT;
		breakingOrbit = false;
		hasEnteredOrbit = false;
		orbitStartTime = Time.get_realtimeSinceStartup();
		Vector3 val = ((Component)this).get_transform().get_position() - interestZoneOrigin;
		currentOrbitTime = Mathf.Atan2(val.x, val.z);
		currentOrbitDistance = orbitDistance;
		ClearAimTarget();
		SetTargetDestination(GetOrbitPosition(currentOrbitTime), 20f, 0f);
	}

	public void State_Orbit_Leave()
	{
		breakingOrbit = false;
		hasEnteredOrbit = false;
		currentOrbitTime = 0f;
		ClearAimTarget();
	}

	public Vector3 GetRandomPatrolDestination()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_zero();
		if ((Object)(object)TerrainMeta.Path != (Object)null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
		{
			MonumentInfo monumentInfo = null;
			if (_visitedMonuments.Count > 0)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					bool flag = false;
					foreach (MonumentInfo visitedMonument in _visitedMonuments)
					{
						if ((Object)(object)monument == (Object)(object)visitedMonument)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						monumentInfo = monument;
						break;
					}
				}
			}
			if ((Object)(object)monumentInfo == (Object)null)
			{
				_visitedMonuments.Clear();
				monumentInfo = TerrainMeta.Path.Monuments[Random.Range(0, TerrainMeta.Path.Monuments.Count)];
			}
			if (Object.op_Implicit((Object)(object)monumentInfo))
			{
				val = ((Component)monumentInfo).get_transform().get_position();
				_visitedMonuments.Add(monumentInfo);
				val.y = TerrainMeta.HeightMap.GetHeight(val) + 200f;
				if (TransformUtil.GetGroundInfo(val, out var hitOut, 300f, LayerMask.op_Implicit(1235288065)))
				{
					val.y = ((RaycastHit)(ref hitOut)).get_point().y;
				}
				val.y += 30f;
			}
		}
		else
		{
			float x = TerrainMeta.Size.x;
			float y = 30f;
			val = Vector3Ex.Range(-1f, 1f);
			val.y = 0f;
			((Vector3)(ref val)).Normalize();
			val *= x * Random.Range(0f, 0.75f);
			val.y = y;
		}
		return val;
	}

	public void State_Patrol_Think(float timePassed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(((Component)this).get_transform().get_position(), destination);
		if (num <= 25f)
		{
			targetThrottleSpeed = GetThrottleForDistance(num);
		}
		else
		{
			targetThrottleSpeed = 0.5f;
		}
		if (AtDestination() && arrivalTime == 0f)
		{
			arrivalTime = Time.get_realtimeSinceStartup();
			ExitCurrentState();
			maxOrbitDuration = 20f;
			State_Orbit_Enter(75f);
		}
		if (_targetList.Count > 0)
		{
			interestZoneOrigin = ((Component)_targetList[0].ply).get_transform().get_position() + new Vector3(0f, 20f, 0f);
			ExitCurrentState();
			maxOrbitDuration = 10f;
			State_Orbit_Enter(75f);
		}
	}

	public void State_Patrol_Enter()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_currentState = aiState.PATROL;
		Vector3 randomPatrolDestination = GetRandomPatrolDestination();
		SetTargetDestination(randomPatrolDestination, 10f);
		interestZoneOrigin = randomPatrolDestination;
		arrivalTime = 0f;
	}

	public void State_Patrol_Leave()
	{
	}

	public int ClipRocketsLeft()
	{
		return numRocketsLeft;
	}

	public bool CanStrafe()
	{
		if (Time.get_realtimeSinceStartup() - lastStrafeTime >= 20f)
		{
			return CanInterruptState();
		}
		return false;
	}

	public bool CanUseNapalm()
	{
		return Time.get_realtimeSinceStartup() - lastNapalmTime >= 30f;
	}

	public void State_Strafe_Enter(Vector3 strafePos, bool shouldUseNapalm = false)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (CanUseNapalm() && shouldUseNapalm)
		{
			useNapalm = shouldUseNapalm;
			lastNapalmTime = Time.get_realtimeSinceStartup();
		}
		lastStrafeTime = Time.get_realtimeSinceStartup();
		_currentState = aiState.STRAFE;
		int mask = LayerMask.GetMask(new string[4] { "Terrain", "World", "Construction", "Water" });
		if (TransformUtil.GetGroundInfo(strafePos, out var pos, out var _, 100f, LayerMask.op_Implicit(mask), ((Component)this).get_transform()))
		{
			strafe_target_position = pos;
		}
		else
		{
			strafe_target_position = strafePos;
		}
		numRocketsLeft = 12;
		lastRocketTime = 0f;
		movementLockingAiming = true;
		Vector3 randomOffset = GetRandomOffset(strafePos, 175f, 192.5f);
		SetTargetDestination(randomOffset, 10f);
		SetIdealRotation(GetYawRotationTo(randomOffset));
		puttingDistance = true;
	}

	public void State_Strafe_Think(float timePassed)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		if (puttingDistance)
		{
			if (AtDestination())
			{
				puttingDistance = false;
				SetTargetDestination(strafe_target_position + new Vector3(0f, 40f, 0f), 10f);
				SetIdealRotation(GetYawRotationTo(strafe_target_position));
			}
			return;
		}
		SetIdealRotation(GetYawRotationTo(strafe_target_position));
		float num = Vector3Ex.Distance2D(strafe_target_position, ((Component)this).get_transform().get_position());
		if (num <= 150f && ClipRocketsLeft() > 0 && Time.get_realtimeSinceStartup() - lastRocketTime > timeBetweenRockets)
		{
			float num2 = Vector3.Distance(strafe_target_position, ((Component)this).get_transform().get_position()) - 10f;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			Vector3 position = ((Component)this).get_transform().get_position();
			Vector3 val = strafe_target_position - ((Component)this).get_transform().get_position();
			if (!Physics.Raycast(position, ((Vector3)(ref val)).get_normalized(), num2, LayerMask.GetMask(new string[2] { "Terrain", "World" })))
			{
				FireRocket();
			}
		}
		if (ClipRocketsLeft() <= 0 || num <= 15f)
		{
			ExitCurrentState();
			State_Move_Enter(GetAppropriatePosition(strafe_target_position + ((Component)this).get_transform().get_forward() * 120f));
		}
	}

	public bool ValidStrafeTarget(BasePlayer ply)
	{
		return !ply.IsNearEnemyBase();
	}

	public void State_Strafe_Leave()
	{
		lastStrafeTime = Time.get_realtimeSinceStartup();
		if (useNapalm)
		{
			lastNapalmTime = Time.get_realtimeSinceStartup();
		}
		useNapalm = false;
		movementLockingAiming = false;
	}

	public void FireRocket()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		numRocketsLeft--;
		lastRocketTime = Time.get_realtimeSinceStartup();
		float num = 4f;
		bool flag = leftTubeFiredLast;
		leftTubeFiredLast = !leftTubeFiredLast;
		Transform val = (flag ? helicopterBase.rocket_tube_left.get_transform() : helicopterBase.rocket_tube_right.get_transform());
		Vector3 val2 = val.get_position() + val.get_forward() * 1f;
		Vector3 val3 = strafe_target_position - val2;
		Vector3 val4 = ((Vector3)(ref val3)).get_normalized();
		if (num > 0f)
		{
			val4 = AimConeUtil.GetModifiedAimConeDirection(num, val4);
		}
		float num2 = 1f;
		RaycastHit val5 = default(RaycastHit);
		if (Physics.Raycast(val2, val4, ref val5, num2, 1236478737))
		{
			num2 = ((RaycastHit)(ref val5)).get_distance() - 0.1f;
		}
		Effect.server.Run(helicopterBase.rocket_fire_effect.resourcePath, helicopterBase, StringPool.Get(flag ? "rocket_tube_left" : "rocket_tube_right"), Vector3.get_zero(), Vector3.get_forward(), null, broadcast: true);
		BaseEntity baseEntity = GameManager.server.CreateEntity(useNapalm ? rocketProjectile_Napalm.resourcePath : rocketProjectile.resourcePath, val2);
		if (!((Object)(object)baseEntity == (Object)null))
		{
			ServerProjectile component = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.InitializeVelocity(val4 * component.speed);
			}
			baseEntity.Spawn();
		}
	}

	public void InitializeAI()
	{
		_lastThinkTime = Time.get_realtimeSinceStartup();
	}

	public void OnCurrentStateExit()
	{
		switch (_currentState)
		{
		default:
			State_Idle_Leave();
			break;
		case aiState.MOVE:
			State_Move_Leave();
			break;
		case aiState.STRAFE:
			State_Strafe_Leave();
			break;
		case aiState.ORBIT:
			State_Orbit_Leave();
			break;
		case aiState.PATROL:
			State_Patrol_Leave();
			break;
		}
	}

	public void ExitCurrentState()
	{
		OnCurrentStateExit();
		_currentState = aiState.IDLE;
	}

	public float GetTime()
	{
		return Time.get_realtimeSinceStartup();
	}

	public void AIThink()
	{
		float time = GetTime();
		float timePassed = time - _lastThinkTime;
		_lastThinkTime = time;
		switch (_currentState)
		{
		default:
			State_Idle_Think(timePassed);
			break;
		case aiState.MOVE:
			State_Move_Think(timePassed);
			break;
		case aiState.STRAFE:
			State_Strafe_Think(timePassed);
			break;
		case aiState.ORBIT:
			State_Orbit_Think(timePassed);
			break;
		case aiState.PATROL:
			State_Patrol_Think(timePassed);
			break;
		case aiState.DEATH:
			State_Death_Think(timePassed);
			break;
		}
	}

	public Vector3 GetRandomOffset(Vector3 origin, float minRange, float maxRange = 0f, float minHeight = 20f, float maxHeight = 30f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 onUnitSphere = Random.get_onUnitSphere();
		onUnitSphere.y = 0f;
		((Vector3)(ref onUnitSphere)).Normalize();
		maxRange = Mathf.Max(minRange, maxRange);
		Vector3 origin2 = origin + onUnitSphere * Random.Range(minRange, maxRange);
		return GetAppropriatePosition(origin2, minHeight, maxHeight);
	}

	public Vector3 GetAppropriatePosition(Vector3 origin, float minHeight = 20f, float maxHeight = 30f)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		float num = 100f;
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(origin + new Vector3(0f, num, 0f), Vector3.get_down());
		float num2 = 5f;
		int mask = LayerMask.GetMask(new string[4] { "Terrain", "World", "Construction", "Water" });
		RaycastHit val2 = default(RaycastHit);
		if (Physics.SphereCast(val, num2, ref val2, num * 2f - num2, mask))
		{
			origin = ((RaycastHit)(ref val2)).get_point();
		}
		origin.y += Random.Range(minHeight, maxHeight);
		return origin;
	}

	public float GetThrottleForDistance(float distToTarget)
	{
		float num = 0f;
		if (distToTarget >= 75f)
		{
			return 1f;
		}
		if (distToTarget >= 50f)
		{
			return 0.75f;
		}
		if (distToTarget >= 25f)
		{
			return 0.33f;
		}
		if (distToTarget >= 5f)
		{
			return 0.05f;
		}
		return 0.05f * (1f - distToTarget / 5f);
	}
}
