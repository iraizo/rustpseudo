using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.AI;
using UnityEngine;
using UnityEngine.AI;

public class BradleyAPC : BaseCombatEntity
{
	[Serializable]
	public class TargetInfo : IPooled
	{
		public float damageReceivedFrom;

		public BaseEntity entity;

		public float lastSeenTime;

		public Vector3 lastSeenPosition;

		public void EnterPool()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			entity = null;
			lastSeenPosition = Vector3.get_zero();
			lastSeenTime = 0f;
		}

		public void Setup(BaseEntity ent, float time)
		{
			entity = ent;
			lastSeenTime = time;
		}

		public void LeavePool()
		{
		}

		public float GetPriorityScore(BradleyAPC apc)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = entity as BasePlayer;
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				float num = Vector3.Distance(((Component)entity).get_transform().get_position(), ((Component)apc).get_transform().get_position());
				float num2 = (1f - Mathf.InverseLerp(10f, 80f, num)) * 50f;
				float num3 = (((Object)(object)basePlayer.GetHeldEntity() == (Object)null) ? 0f : basePlayer.GetHeldEntity().hostileScore);
				float num4 = Mathf.InverseLerp(4f, 20f, num3) * 100f;
				float num5 = Mathf.InverseLerp(10f, 3f, Time.get_time() - lastSeenTime) * 100f;
				float num6 = Mathf.InverseLerp(0f, 100f, damageReceivedFrom) * 50f;
				return num2 + num4 + num6 + num5;
			}
			return 0f;
		}

		public bool IsVisible()
		{
			if (lastSeenTime != -1f)
			{
				return Time.get_time() - lastSeenTime < sightUpdateRate * 2f;
			}
			return false;
		}

		public bool IsValid()
		{
			return (Object)(object)entity != (Object)null;
		}
	}

	[Header("Sound")]
	public BlendedLoopEngineSound engineSound;

	public SoundDefinition treadLoopDef;

	public AnimationCurve treadGainCurve;

	public AnimationCurve treadPitchCurve;

	public AnimationCurve treadFreqCurve;

	private Sound treadLoop;

	private SoundModulation.Modulator treadGain;

	private SoundModulation.Modulator treadPitch;

	public SoundDefinition chasisLurchSoundDef;

	public float chasisLurchAngleDelta = 2f;

	public float chasisLurchSpeedDelta = 2f;

	private float lastAngle;

	private float lastSpeed;

	public SoundDefinition turretTurnLoopDef;

	public float turretLoopGainSpeed = 3f;

	public float turretLoopPitchSpeed = 3f;

	public float turretLoopMinAngleDelta;

	public float turretLoopMaxAngleDelta = 10f;

	public float turretLoopPitchMin = 0.5f;

	public float turretLoopPitchMax = 1f;

	public float turretLoopGainThreshold = 0.0001f;

	private Sound turretTurnLoop;

	private SoundModulation.Modulator turretTurnLoopGain;

	private SoundModulation.Modulator turretTurnLoopPitch;

	public float enginePitch = 0.9f;

	public float rpmMultiplier = 0.6f;

	private TreadAnimator treadAnimator;

	[Header("Pathing")]
	public List<Vector3> currentPath;

	public int currentPathIndex;

	public bool pathLooping;

	[Header("Targeting")]
	public float viewDistance = 100f;

	public float searchRange = 100f;

	public float searchFrequency = 2f;

	public float memoryDuration = 20f;

	public static float sightUpdateRate = 0.5f;

	public List<TargetInfo> targetList = new List<TargetInfo>();

	private BaseCombatEntity mainGunTarget;

	[Header("Coax")]
	public float coaxFireRate = 0.06667f;

	public int coaxBurstLength = 10;

	public float coaxAimCone = 3f;

	public float bulletDamage = 15f;

	[Header("TopTurret")]
	public float topTurretFireRate = 0.25f;

	private float nextCoaxTime;

	private int numCoaxBursted;

	private float nextTopTurretTime = 0.3f;

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	private float lastLateUpdate;

	[Header("Wheels")]
	public WheelCollider[] leftWheels;

	public WheelCollider[] rightWheels;

	[Header("Movement Config")]
	public float moveForceMax = 2000f;

	public float brakeForce = 100f;

	public float turnForce = 2000f;

	public float sideStiffnessMax = 1f;

	public float sideStiffnessMin = 0.5f;

	public Transform centerOfMass;

	public float stoppingDist = 5f;

	[Header("Control")]
	public float throttle = 1f;

	public float turning;

	public float rightThrottle;

	public float leftThrottle;

	public bool brake;

	[Header("Other")]
	public Rigidbody myRigidBody;

	public Collider myCollider;

	public Vector3 destination;

	private Vector3 finalDestination;

	public Transform followTest;

	public TriggerHurtEx impactDamager;

	[Header("Weapons")]
	public Transform mainTurretEyePos;

	public Transform mainTurret;

	public Transform CannonPitch;

	public Transform CannonMuzzle;

	public Transform coaxPitch;

	public Transform coaxMuzzle;

	public Transform topTurretEyePos;

	public Transform topTurretYaw;

	public Transform topTurretPitch;

	public Transform topTurretMuzzle;

	private Vector3 turretAimVector = Vector3.get_forward();

	private Vector3 desiredAimVector = Vector3.get_forward();

	private Vector3 topTurretAimVector = Vector3.get_forward();

	private Vector3 desiredTopTurretAimVector = Vector3.get_forward();

	[Header("Effects")]
	public GameObjectRef explosionEffect;

	public GameObjectRef servergibs;

	public GameObjectRef fireBall;

	public GameObjectRef crateToDrop;

	public GameObjectRef debrisFieldMarker;

	[Header("Loot")]
	public int maxCratesToSpawn;

	public int patrolPathIndex;

	public BasePath patrolPath;

	public bool DoAI = true;

	public GameObjectRef mainCannonMuzzleFlash;

	public GameObjectRef mainCannonProjectile;

	public float recoilScale = 200f;

	public NavMeshPath navMeshPath;

	public int navMeshPathIndex;

	private float nextFireTime = 10f;

	private int numBursted;

	private float nextPatrolTime;

	private float nextEngagementPathTime;

	private float currentSpeedZoneLimit;

	protected override float PositionTickRate => 0.1f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BradleyAPC.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool HasPath()
	{
		if (currentPath != null)
		{
			return currentPath.Count > 0;
		}
		return false;
	}

	public void ClearPath()
	{
		currentPath.Clear();
		currentPathIndex = -1;
	}

	public bool IndexValid(int index)
	{
		if (!HasPath())
		{
			return false;
		}
		if (index >= 0)
		{
			return index < currentPath.Count;
		}
		return false;
	}

	public Vector3 GetFinalDestination()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath())
		{
			return ((Component)this).get_transform().get_position();
		}
		return finalDestination;
	}

	public Vector3 GetCurrentPathDestination()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath())
		{
			return ((Component)this).get_transform().get_position();
		}
		return currentPath[currentPathIndex];
	}

	public bool PathComplete()
	{
		if (HasPath())
		{
			if (currentPathIndex == currentPath.Count - 1)
			{
				return AtCurrentPathNode();
			}
			return false;
		}
		return true;
	}

	public bool AtCurrentPathNode()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (currentPathIndex < 0 || currentPathIndex >= currentPath.Count)
		{
			return false;
		}
		return Vector3.Distance(((Component)this).get_transform().get_position(), currentPath[currentPathIndex]) <= stoppingDist;
	}

	public int GetLoopedIndex(int index)
	{
		if (!HasPath())
		{
			Debug.LogWarning((object)"Warning, GetLoopedIndex called without a path");
			return 0;
		}
		if (!pathLooping)
		{
			return Mathf.Clamp(index, 0, currentPath.Count - 1);
		}
		if (index >= currentPath.Count)
		{
			return index % currentPath.Count;
		}
		if (index < 0)
		{
			return currentPath.Count - Mathf.Abs(index % currentPath.Count);
		}
		return index;
	}

	public Vector3 PathDirection(int index)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath() || currentPath.Count <= 1)
		{
			return ((Component)this).get_transform().get_forward();
		}
		index = GetLoopedIndex(index);
		Vector3 zero = Vector3.get_zero();
		Vector3 zero2 = Vector3.get_zero();
		if (pathLooping)
		{
			int loopedIndex = GetLoopedIndex(index - 1);
			zero = currentPath[loopedIndex];
			zero2 = currentPath[GetLoopedIndex(index)];
		}
		else
		{
			zero = ((index - 1 >= 0) ? currentPath[index - 1] : ((Component)this).get_transform().get_position());
			zero2 = currentPath[index];
		}
		Vector3 val = zero2 - zero;
		return ((Vector3)(ref val)).get_normalized();
	}

	public Vector3 IdealPathPosition()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath())
		{
			return ((Component)this).get_transform().get_position();
		}
		int loopedIndex = GetLoopedIndex(currentPathIndex - 1);
		if (loopedIndex == currentPathIndex)
		{
			return currentPath[currentPathIndex];
		}
		return ClosestPointAlongPath(currentPath[loopedIndex], currentPath[currentPathIndex], ((Component)this).get_transform().get_position());
	}

	public void AdvancePathMovement()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (HasPath())
		{
			if (AtCurrentPathNode() || currentPathIndex == -1)
			{
				currentPathIndex = GetLoopedIndex(currentPathIndex + 1);
			}
			if (PathComplete())
			{
				ClearPath();
				return;
			}
			Vector3 val = IdealPathPosition();
			float num = Vector3.Distance(val, currentPath[currentPathIndex]);
			float num2 = Vector3.Distance(((Component)this).get_transform().get_position(), val);
			float num3 = Mathf.InverseLerp(8f, 0f, num2);
			val += Direction2D(currentPath[currentPathIndex], val) * Mathf.Min(num, num3 * 20f);
			SetDestination(val);
		}
	}

	public bool GetPathToClosestTurnableNode(BasePathNode start, Vector3 forward, ref List<BasePathNode> nodes)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		float num = float.NegativeInfinity;
		BasePathNode basePathNode = null;
		Vector3 val;
		foreach (BasePathNode item in start.linked)
		{
			val = ((Component)item).get_transform().get_position() - ((Component)start).get_transform().get_position();
			float num2 = Vector3.Dot(forward, ((Vector3)(ref val)).get_normalized());
			if (num2 > num)
			{
				num = num2;
				basePathNode = item;
			}
		}
		if ((Object)(object)basePathNode != (Object)null)
		{
			nodes.Add(basePathNode);
			if (!basePathNode.straightaway)
			{
				return true;
			}
			BasePathNode start2 = basePathNode;
			val = ((Component)basePathNode).get_transform().get_position() - ((Component)start).get_transform().get_position();
			return GetPathToClosestTurnableNode(start2, ((Vector3)(ref val)).get_normalized(), ref nodes);
		}
		return false;
	}

	public bool GetEngagementPath(ref List<BasePathNode> nodes)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		BasePathNode closestToPoint = patrolPath.GetClosestToPoint(((Component)this).get_transform().get_position());
		Vector3 val = ((Component)closestToPoint).get_transform().get_position() - ((Component)this).get_transform().get_position();
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		if (Vector3.Dot(((Component)this).get_transform().get_forward(), normalized) > 0f)
		{
			nodes.Add(closestToPoint);
			if (!closestToPoint.straightaway)
			{
				return true;
			}
		}
		return GetPathToClosestTurnableNode(closestToPoint, ((Component)this).get_transform().get_forward(), ref nodes);
	}

	public void AddOrUpdateTarget(BaseEntity ent, Vector3 pos, float damageFrom = 0f)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!(ent is BasePlayer))
		{
			return;
		}
		TargetInfo targetInfo = null;
		foreach (TargetInfo target in targetList)
		{
			if ((Object)(object)target.entity == (Object)(object)ent)
			{
				targetInfo = target;
				break;
			}
		}
		if (targetInfo == null)
		{
			targetInfo = Pool.Get<TargetInfo>();
			targetInfo.Setup(ent, Time.get_time() - 1f);
			targetList.Add(targetInfo);
		}
		targetInfo.lastSeenPosition = pos;
		targetInfo.damageReceivedFrom += damageFrom;
	}

	public void UpdateTargetList()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(((Component)this).get_transform().get_position(), searchRange, list, 133120, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (!(item is BasePlayer))
			{
				continue;
			}
			BasePlayer basePlayer = item as BasePlayer;
			if (basePlayer.IsDead() || basePlayer is HumanNPC || !VisibilityTest(item))
			{
				continue;
			}
			bool flag = false;
			foreach (TargetInfo target in targetList)
			{
				if ((Object)(object)target.entity == (Object)(object)item)
				{
					target.lastSeenTime = Time.get_time();
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				TargetInfo targetInfo = Pool.Get<TargetInfo>();
				targetInfo.Setup(item, Time.get_time());
				targetList.Add(targetInfo);
			}
		}
		for (int num = targetList.Count - 1; num >= 0; num--)
		{
			TargetInfo targetInfo2 = targetList[num];
			BasePlayer basePlayer2 = targetInfo2.entity as BasePlayer;
			if ((Object)(object)targetInfo2.entity == (Object)null || Time.get_time() - targetInfo2.lastSeenTime > memoryDuration || basePlayer2.IsDead())
			{
				targetList.Remove(targetInfo2);
				Pool.Free<TargetInfo>(ref targetInfo2);
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		targetList.Sort(new Comparison<TargetInfo>(SortTargets));
	}

	public int SortTargets(TargetInfo t1, TargetInfo t2)
	{
		return t2.GetPriorityScore(this).CompareTo(t1.GetPriorityScore(this));
	}

	public Vector3 GetAimPoint(BaseEntity ent)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = ent as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			return basePlayer.eyes.position;
		}
		return ent.CenterPoint();
	}

	public bool VisibilityTest(BaseEntity ent)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent == (Object)null)
		{
			return false;
		}
		if (!(Vector3.Distance(((Component)ent).get_transform().get_position(), ((Component)this).get_transform().get_position()) < viewDistance))
		{
			return false;
		}
		bool flag = false;
		if (ent is BasePlayer)
		{
			BasePlayer basePlayer = ent as BasePlayer;
			Vector3 position = ((Component)mainTurret).get_transform().get_position();
			flag = IsVisible(basePlayer.eyes.position, position) || IsVisible(((Component)basePlayer).get_transform().get_position() + Vector3.get_up() * 0.1f, position);
			if (!flag && basePlayer.isMounted && (Object)(object)basePlayer.GetMounted().VehicleParent() != (Object)null && basePlayer.GetMounted().VehicleParent().AlwaysAllowBradleyTargeting)
			{
				flag = IsVisible(((Bounds)(ref basePlayer.GetMounted().VehicleParent().bounds)).get_center(), position);
			}
			if (flag)
			{
				flag = !Physics.SphereCast(new Ray(position, Vector3Ex.Direction(basePlayer.eyes.position, position)), 0.05f, Vector3.Distance(basePlayer.eyes.position, position), 10551297);
			}
		}
		else
		{
			Debug.LogWarning((object)"Standard vis test!");
			flag = IsVisible(ent.CenterPoint());
		}
		return flag;
	}

	public void UpdateTargetVisibilities()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		foreach (TargetInfo target in targetList)
		{
			if (target.IsValid() && VisibilityTest(target.entity))
			{
				target.lastSeenTime = Time.get_time();
				target.lastSeenPosition = ((Component)target.entity).get_transform().get_position();
			}
		}
	}

	public void DoWeaponAiming()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized;
		Vector3 val;
		if (!((Object)(object)mainGunTarget != (Object)null))
		{
			normalized = desiredAimVector;
		}
		else
		{
			val = GetAimPoint(mainGunTarget) - ((Component)mainTurretEyePos).get_transform().get_position();
			normalized = ((Vector3)(ref val)).get_normalized();
		}
		desiredAimVector = normalized;
		BaseEntity baseEntity = null;
		if (targetList.Count > 0)
		{
			if (targetList.Count > 1 && targetList[1].IsValid() && targetList[1].IsVisible())
			{
				baseEntity = targetList[1].entity;
			}
			else if (targetList[0].IsValid() && targetList[0].IsVisible())
			{
				baseEntity = targetList[0].entity;
			}
		}
		Vector3 val2;
		if (!((Object)(object)baseEntity != (Object)null))
		{
			val2 = ((Component)this).get_transform().get_forward();
		}
		else
		{
			val = GetAimPoint(baseEntity) - ((Component)topTurretEyePos).get_transform().get_position();
			val2 = ((Vector3)(ref val)).get_normalized();
		}
		desiredTopTurretAimVector = val2;
	}

	public void DoWeapons()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mainGunTarget != (Object)null)
		{
			Vector3 val = turretAimVector;
			Vector3 val2 = GetAimPoint(mainGunTarget) - ((Component)mainTurretEyePos).get_transform().get_position();
			if (Vector3.Dot(val, ((Vector3)(ref val2)).get_normalized()) >= 0.99f)
			{
				bool flag = VisibilityTest(mainGunTarget);
				float num = Vector3.Distance(((Component)mainGunTarget).get_transform().get_position(), ((Component)this).get_transform().get_position());
				if (Time.get_time() > nextCoaxTime && flag && num <= 40f)
				{
					numCoaxBursted++;
					FireGun(GetAimPoint(mainGunTarget), 3f, isCoax: true);
					nextCoaxTime = Time.get_time() + coaxFireRate;
					if (numCoaxBursted >= coaxBurstLength)
					{
						nextCoaxTime = Time.get_time() + 1f;
						numCoaxBursted = 0;
					}
				}
				if (num >= 10f && flag)
				{
					FireGunTest();
				}
			}
		}
		if (targetList.Count > 1)
		{
			BaseEntity entity = targetList[1].entity;
			if ((Object)(object)entity != (Object)null && Time.get_time() > nextTopTurretTime && VisibilityTest(entity))
			{
				FireGun(GetAimPoint(targetList[1].entity), 3f, isCoax: false);
				nextTopTurretTime = Time.get_time() + topTurretFireRate;
			}
		}
	}

	public void FireGun(Vector3 targetPos, float aimCone, bool isCoax)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		Transform val = (isCoax ? coaxMuzzle : topTurretMuzzle);
		Vector3 val2 = ((Component)val).get_transform().get_position() - val.get_forward() * 0.25f;
		Vector3 val3 = targetPos - val2;
		Vector3 normalized = ((Vector3)(ref val3)).get_normalized();
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized);
		targetPos = val2 + modifiedAimConeDirection * 300f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(val2, modifiedAimConeDirection), 0f, list, 300f, 1219701521, (QueryTriggerInteraction)0);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if (!((Object)(object)entity != (Object)null) || (!((Object)(object)entity == (Object)(object)this) && !entity.EqualNetID(this)))
			{
				BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
				if ((Object)(object)baseCombatEntity != (Object)null)
				{
					ApplyDamage(baseCombatEntity, ((RaycastHit)(ref hit)).get_point(), modifiedAimConeDirection);
				}
				if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
				{
					targetPos = ((RaycastHit)(ref hit)).get_point();
					break;
				}
			}
		}
		ClientRPC<bool, Vector3>(null, "CLIENT_FireGun", isCoax, targetPos);
		Pool.FreeList<RaycastHit>(ref list);
	}

	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		float damageAmount = bulletDamage * Random.Range(0.9f, 1.1f);
		HitInfo info = new HitInfo(this, entity, DamageType.Bullet, damageAmount, point);
		entity.OnAttacked(info);
		if (entity is BasePlayer || entity is BaseNpc)
		{
			Effect.server.ImpactEffect(new HitInfo
			{
				HitPositionWorld = point,
				HitNormalWorld = -normal,
				HitMaterial = StringPool.Get("Flesh")
			});
		}
	}

	public void AimWeaponAt(Transform weaponYaw, Transform weaponPitch, Vector3 direction, float minPitch = -360f, float maxPitch = 360f, float maxYaw = 360f, Transform parentOverride = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = direction;
		val = weaponYaw.get_parent().InverseTransformDirection(val);
		Quaternion localRotation = Quaternion.LookRotation(val);
		Vector3 eulerAngles = ((Quaternion)(ref localRotation)).get_eulerAngles();
		for (int i = 0; i < 3; i++)
		{
			((Vector3)(ref eulerAngles)).set_Item(i, ((Vector3)(ref eulerAngles)).get_Item(i) - ((((Vector3)(ref eulerAngles)).get_Item(i) > 180f) ? 360f : 0f));
		}
		Quaternion localRotation2 = Quaternion.Euler(0f, Mathf.Clamp(eulerAngles.y, 0f - maxYaw, maxYaw), 0f);
		Quaternion localRotation3 = Quaternion.Euler(Mathf.Clamp(eulerAngles.x, minPitch, maxPitch), 0f, 0f);
		if ((Object)(object)weaponYaw == (Object)null && (Object)(object)weaponPitch != (Object)null)
		{
			((Component)weaponPitch).get_transform().set_localRotation(localRotation3);
			return;
		}
		if ((Object)(object)weaponPitch == (Object)null && (Object)(object)weaponYaw != (Object)null)
		{
			((Component)weaponYaw).get_transform().set_localRotation(localRotation);
			return;
		}
		((Component)weaponYaw).get_transform().set_localRotation(localRotation2);
		((Component)weaponPitch).get_transform().set_localRotation(localRotation3);
	}

	public void LateUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_time() - lastLateUpdate;
		lastLateUpdate = Time.get_time();
		if (base.isServer)
		{
			float num2 = (float)Math.PI * 2f / 3f;
			turretAimVector = Vector3.RotateTowards(turretAimVector, desiredAimVector, num2 * num, 0f);
		}
		else
		{
			turretAimVector = Vector3.Lerp(turretAimVector, desiredAimVector, Time.get_deltaTime() * 10f);
		}
		AimWeaponAt(mainTurret, coaxPitch, turretAimVector, -90f, 90f);
		AimWeaponAt(mainTurret, CannonPitch, turretAimVector, -90f, 7f);
		topTurretAimVector = Vector3.Lerp(topTurretAimVector, desiredTopTurretAimVector, Time.get_deltaTime() * 5f);
		AimWeaponAt(topTurretYaw, topTurretPitch, topTurretAimVector, -360f, 360f, 360f, mainTurret);
	}

	public override void Load(LoadInfo info)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.bradley != null && !info.fromDisk)
		{
			throttle = info.msg.bradley.engineThrottle;
			rightThrottle = info.msg.bradley.throttleRight;
			leftThrottle = info.msg.bradley.throttleLeft;
			desiredAimVector = info.msg.bradley.mainGunVec;
			desiredTopTurretAimVector = info.msg.bradley.topTurretVec;
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.bradley = Pool.Get<BradleyAPC>();
			info.msg.bradley.engineThrottle = throttle;
			info.msg.bradley.throttleLeft = leftThrottle;
			info.msg.bradley.throttleRight = rightThrottle;
			info.msg.bradley.mainGunVec = turretAimVector;
			info.msg.bradley.topTurretVec = topTurretAimVector;
		}
	}

	public void SetDestination(Vector3 dest)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		destination = dest;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Initialize();
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateTargetList, 0f, 2f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateTargetVisibilities, 0f, sightUpdateRate);
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
	}

	public void Initialize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		myRigidBody.set_centerOfMass(centerOfMass.get_localPosition());
		destination = ((Component)this).get_transform().get_position();
		finalDestination = ((Component)this).get_transform().get_position();
	}

	public BasePlayer FollowPlayer()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				if (current.IsAdmin && current.IsAlive() && !current.IsSleeping() && current.GetActiveItem() != null && current.GetActiveItem().info.shortname == "tool.binoculars")
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return null;
	}

	public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(aimAt.x, 0f, aimAt.z) - new Vector3(aimFrom.x, 0f, aimFrom.z);
		return ((Vector3)(ref val)).get_normalized();
	}

	public bool IsAtDestination()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3Ex.Distance2D(((Component)this).get_transform().get_position(), destination) <= stoppingDist;
	}

	public bool IsAtFinalDestination()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3Ex.Distance2D(((Component)this).get_transform().get_position(), finalDestination) <= stoppingDist;
	}

	public Vector3 ClosestPointAlongPath(Vector3 start, Vector3 end, Vector3 fromPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		Vector3 val2 = fromPos - start;
		float num = Vector3.Dot(val, val2);
		float num2 = Vector3.SqrMagnitude(end - start);
		float num3 = Mathf.Clamp01(num / num2);
		return start + val * num3;
	}

	public void FireGunTest()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_time() < nextFireTime)
		{
			return;
		}
		nextFireTime = Time.get_time() + 0.25f;
		numBursted++;
		if (numBursted >= 4)
		{
			nextFireTime = Time.get_time() + 5f;
			numBursted = 0;
		}
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(2f, CannonMuzzle.get_rotation() * Vector3.get_forward());
		Vector3 val = ((Component)CannonPitch).get_transform().get_rotation() * Vector3.get_back() + ((Component)this).get_transform().get_up() * -1f;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		myRigidBody.AddForceAtPosition(normalized * recoilScale, ((Component)CannonPitch).get_transform().get_position(), (ForceMode)1);
		Effect.server.Run(mainCannonMuzzleFlash.resourcePath, this, StringPool.Get(((Object)((Component)CannonMuzzle).get_gameObject()).get_name()), Vector3.get_zero(), Vector3.get_zero());
		BaseEntity baseEntity = GameManager.server.CreateEntity(mainCannonProjectile.resourcePath, ((Component)CannonMuzzle).get_transform().get_position(), Quaternion.LookRotation(modifiedAimConeDirection));
		if (!((Object)(object)baseEntity == (Object)null))
		{
			ServerProjectile component = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.InitializeVelocity(modifiedAimConeDirection * component.speed);
			}
			baseEntity.Spawn();
		}
	}

	public void InstallPatrolPath(BasePath path)
	{
		patrolPath = path;
		currentPath = new List<Vector3>();
		currentPathIndex = -1;
	}

	public void UpdateMovement_Patrol()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)patrolPath == (Object)null || Time.get_time() < nextPatrolTime)
		{
			return;
		}
		nextPatrolTime = Time.get_time() + 20f;
		if (HasPath() && !IsAtFinalDestination())
		{
			return;
		}
		PathInterestNode randomInterestNodeAwayFrom = patrolPath.GetRandomInterestNodeAwayFrom(((Component)this).get_transform().get_position());
		BasePathNode closestToPoint = patrolPath.GetClosestToPoint(((Component)randomInterestNodeAwayFrom).get_transform().get_position());
		BasePathNode basePathNode = null;
		bool flag = false;
		List<BasePathNode> nodes = Pool.GetList<BasePathNode>();
		if (GetEngagementPath(ref nodes))
		{
			flag = true;
			basePathNode = nodes[nodes.Count - 1];
		}
		else
		{
			basePathNode = patrolPath.GetClosestToPoint(((Component)this).get_transform().get_position());
		}
		if (!(Vector3.Distance(finalDestination, ((Component)closestToPoint).get_transform().get_position()) > 2f))
		{
			return;
		}
		if ((Object)(object)closestToPoint == (Object)(object)basePathNode)
		{
			currentPath.Clear();
			currentPath.Add(((Component)closestToPoint).get_transform().get_position());
			currentPathIndex = -1;
			pathLooping = false;
			finalDestination = ((Component)closestToPoint).get_transform().get_position();
		}
		else
		{
			if (!AStarPath.FindPath(basePathNode, closestToPoint, out var path, out var _))
			{
				return;
			}
			currentPath.Clear();
			if (flag)
			{
				for (int i = 0; i < nodes.Count - 1; i++)
				{
					currentPath.Add(((Component)nodes[i]).get_transform().get_position());
				}
			}
			Enumerator<BasePathNode> enumerator = path.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePathNode current = enumerator.get_Current();
					currentPath.Add(((Component)current).get_transform().get_position());
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			currentPathIndex = -1;
			pathLooping = false;
			finalDestination = ((Component)closestToPoint).get_transform().get_position();
		}
	}

	public void UpdateMovement_Hunt()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)patrolPath == (Object)null)
		{
			return;
		}
		TargetInfo targetInfo = targetList[0];
		if (!targetInfo.IsValid())
		{
			return;
		}
		if (HasPath() && targetInfo.IsVisible())
		{
			if (currentPath.Count > 1)
			{
				Vector3 item = currentPath[currentPathIndex];
				ClearPath();
				currentPath.Add(item);
				finalDestination = item;
				currentPathIndex = 0;
			}
		}
		else
		{
			if (!(Time.get_time() > nextEngagementPathTime) || HasPath() || targetInfo.IsVisible())
			{
				return;
			}
			bool flag = false;
			BasePathNode start = patrolPath.GetClosestToPoint(((Component)this).get_transform().get_position());
			List<BasePathNode> nodes = Pool.GetList<BasePathNode>();
			if (GetEngagementPath(ref nodes))
			{
				flag = true;
				start = nodes[nodes.Count - 1];
			}
			BasePathNode basePathNode = null;
			List<BasePathNode> nearNodes = Pool.GetList<BasePathNode>();
			patrolPath.GetNodesNear(targetInfo.lastSeenPosition, ref nearNodes, 30f);
			Stack<BasePathNode> val = null;
			float num = float.PositiveInfinity;
			float y = mainTurretEyePos.get_localPosition().y;
			foreach (BasePathNode item2 in nearNodes)
			{
				Stack<BasePathNode> path = new Stack<BasePathNode>();
				if (targetInfo.entity.IsVisible(((Component)item2).get_transform().get_position() + new Vector3(0f, y, 0f)) && AStarPath.FindPath(start, item2, out path, out var pathCost) && pathCost < num)
				{
					val = path;
					num = pathCost;
					basePathNode = item2;
				}
			}
			if (val == null && nearNodes.Count > 0)
			{
				Stack<BasePathNode> path2 = new Stack<BasePathNode>();
				BasePathNode basePathNode2 = nearNodes[Random.Range(0, nearNodes.Count)];
				if (AStarPath.FindPath(start, basePathNode2, out path2, out var pathCost2) && pathCost2 < num)
				{
					val = path2;
					basePathNode = basePathNode2;
				}
			}
			if (val != null)
			{
				currentPath.Clear();
				if (flag)
				{
					for (int i = 0; i < nodes.Count - 1; i++)
					{
						currentPath.Add(((Component)nodes[i]).get_transform().get_position());
					}
				}
				Enumerator<BasePathNode> enumerator2 = val.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						BasePathNode current2 = enumerator2.get_Current();
						currentPath.Add(((Component)current2).get_transform().get_position());
					}
				}
				finally
				{
					((IDisposable)enumerator2).Dispose();
				}
				currentPathIndex = -1;
				pathLooping = false;
				finalDestination = ((Component)basePathNode).get_transform().get_position();
			}
			Pool.FreeList<BasePathNode>(ref nearNodes);
			Pool.FreeList<BasePathNode>(ref nodes);
			nextEngagementPathTime = Time.get_time() + 5f;
		}
	}

	public void DoSimpleAI()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		SetFlag(Flags.Reserved5, TOD_Sky.get_Instance().get_IsNight());
		if (!DoAI)
		{
			return;
		}
		if (targetList.Count > 0)
		{
			if (targetList[0].IsValid() && targetList[0].IsVisible())
			{
				mainGunTarget = targetList[0].entity as BaseCombatEntity;
			}
			else
			{
				mainGunTarget = null;
			}
			UpdateMovement_Hunt();
		}
		else
		{
			mainGunTarget = null;
			UpdateMovement_Patrol();
		}
		AdvancePathMovement();
		float num = Vector3.Distance(((Component)this).get_transform().get_position(), destination);
		float num2 = Vector3.Distance(((Component)this).get_transform().get_position(), finalDestination);
		if (num > stoppingDist)
		{
			Vector3 val = Direction2D(destination, ((Component)this).get_transform().get_position());
			float num3 = Vector3.Dot(val, ((Component)this).get_transform().get_right());
			float num4 = Vector3.Dot(val, ((Component)this).get_transform().get_right());
			float num5 = Vector3.Dot(val, -((Component)this).get_transform().get_right());
			if (Vector3.Dot(val, -((Component)this).get_transform().get_forward()) > num3)
			{
				if (num4 >= num5)
				{
					turning = 1f;
				}
				else
				{
					turning = -1f;
				}
			}
			else
			{
				turning = Mathf.Clamp(num3 * 3f, -1f, 1f);
			}
			float num6 = 1f - Mathf.InverseLerp(0f, 0.3f, Mathf.Abs(turning));
			float num7 = Mathf.InverseLerp(0.1f, 0.4f, Vector3.Dot(((Component)this).get_transform().get_forward(), Vector3.get_up()));
			throttle = (0.1f + Mathf.InverseLerp(0f, 20f, num2) * 1f) * num6 + num7;
		}
		DoWeaponAiming();
		SendNetworkUpdate();
	}

	public void FixedUpdate()
	{
		DoSimpleAI();
		DoPhysicsMove();
		DoWeapons();
		DoHealing();
	}

	public void DoPhysicsMove()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		Vector3 velocity = myRigidBody.get_velocity();
		throttle = Mathf.Clamp(throttle, -1f, 1f);
		leftThrottle = throttle;
		rightThrottle = throttle;
		if (turning > 0f)
		{
			rightThrottle = 0f - turning;
			leftThrottle = turning;
		}
		else if (turning < 0f)
		{
			leftThrottle = turning;
			rightThrottle = turning * -1f;
		}
		Vector3.Distance(((Component)this).get_transform().get_position(), GetFinalDestination());
		float num = Vector3.Distance(((Component)this).get_transform().get_position(), GetCurrentPathDestination());
		float num2 = 15f;
		if (num < 20f)
		{
			float num3 = Vector3.Dot(PathDirection(currentPathIndex), PathDirection(currentPathIndex + 1));
			float num4 = Mathf.InverseLerp(2f, 10f, num);
			float num5 = Mathf.InverseLerp(0.5f, 0.8f, num3);
			num2 = 15f - 14f * ((1f - num5) * (1f - num4));
		}
		_ = 20f;
		if ((Object)(object)patrolPath != (Object)null)
		{
			float num6 = num2;
			foreach (PathSpeedZone speedZone in patrolPath.speedZones)
			{
				OBB val = speedZone.WorldSpaceBounds();
				if (((OBB)(ref val)).Contains(((Component)this).get_transform().get_position()))
				{
					num6 = Mathf.Min(num6, speedZone.GetMaxSpeed());
				}
			}
			currentSpeedZoneLimit = Mathf.Lerp(currentSpeedZoneLimit, num6, Time.get_deltaTime());
			num2 = Mathf.Min(num2, currentSpeedZoneLimit);
		}
		if (PathComplete())
		{
			num2 = 0f;
		}
		if (Global.developer > 1)
		{
			Debug.Log((object)("velocity:" + ((Vector3)(ref velocity)).get_magnitude() + "max : " + num2));
		}
		brake = ((Vector3)(ref velocity)).get_magnitude() >= num2;
		ApplyBrakes(brake ? 1f : 0f);
		float num7 = throttle;
		leftThrottle = Mathf.Clamp(leftThrottle + num7, -1f, 1f);
		rightThrottle = Mathf.Clamp(rightThrottle + num7, -1f, 1f);
		float num8 = Mathf.InverseLerp(2f, 1f, ((Vector3)(ref velocity)).get_magnitude() * Mathf.Abs(Vector3.Dot(((Vector3)(ref velocity)).get_normalized(), ((Component)this).get_transform().get_forward())));
		float torqueAmount = Mathf.Lerp(moveForceMax, turnForce, num8);
		float num9 = Mathf.InverseLerp(5f, 1.5f, ((Vector3)(ref velocity)).get_magnitude() * Mathf.Abs(Vector3.Dot(((Vector3)(ref velocity)).get_normalized(), ((Component)this).get_transform().get_forward())));
		ScaleSidewaysFriction(1f - num9);
		SetMotorTorque(leftThrottle, rightSide: false, torqueAmount);
		SetMotorTorque(rightThrottle, rightSide: true, torqueAmount);
		TriggerHurtEx triggerHurtEx = impactDamager;
		Vector3 velocity2 = myRigidBody.get_velocity();
		triggerHurtEx.damageEnabled = ((Vector3)(ref velocity2)).get_magnitude() > 2f;
	}

	public void ApplyBrakes(float amount)
	{
		ApplyBrakeTorque(amount, rightSide: true);
		ApplyBrakeTorque(amount, rightSide: false);
	}

	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		foreach (WheelCollider val in array)
		{
			num += val.get_motorTorque();
		}
		return num / (float)rightWheels.Length;
	}

	public void ScaleSidewaysFriction(float scale)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		float stiffness = 0.75f + 0.75f * scale;
		WheelCollider[] array = rightWheels;
		foreach (WheelCollider obj in array)
		{
			WheelFrictionCurve sidewaysFriction = obj.get_sidewaysFriction();
			((WheelFrictionCurve)(ref sidewaysFriction)).set_stiffness(stiffness);
			obj.set_sidewaysFriction(sidewaysFriction);
		}
		array = leftWheels;
		foreach (WheelCollider obj2 in array)
		{
			WheelFrictionCurve sidewaysFriction2 = obj2.get_sidewaysFriction();
			((WheelFrictionCurve)(ref sidewaysFriction2)).set_stiffness(stiffness);
			obj2.set_sidewaysFriction(sidewaysFriction2);
		}
	}

	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float num = torqueAmount * newThrottle;
		int num2 = (rightSide ? rightWheels.Length : leftWheels.Length);
		int num3 = 0;
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		WheelHit val = default(WheelHit);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetGroundHit(ref val))
			{
				num3++;
			}
		}
		float num4 = 1f;
		if (num3 > 0)
		{
			num4 = num2 / num3;
		}
		array = (rightSide ? rightWheels : leftWheels);
		WheelHit val3 = default(WheelHit);
		foreach (WheelCollider val2 in array)
		{
			if (val2.GetGroundHit(ref val3))
			{
				val2.set_motorTorque(num * num4);
			}
			else
			{
				val2.set_motorTorque(num);
			}
		}
	}

	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].set_brakeTorque(brakeForce * amount);
		}
	}

	public void CreateExplosionMarker(float durationMinutes)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(debrisFieldMarker.resourcePath, ((Component)this).get_transform().get_position(), Quaternion.get_identity());
		baseEntity.Spawn();
		((Component)baseEntity).SendMessage("SetDuration", (object)durationMinutes, (SendMessageOptions)1);
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		CreateExplosionMarker(10f);
		Effect.server.Run(explosionEffect.resourcePath, ((Component)mainTurretEyePos).get_transform().get_position(), Vector3.get_up(), null, broadcast: true);
		Vector3 zero = Vector3.get_zero();
		GameObject gibSource = servergibs.Get().GetComponent<ServerGib>()._gibSource;
		List<ServerGib> list = ServerGib.CreateGibs(servergibs.resourcePath, ((Component)this).get_gameObject(), gibSource, zero, 3f);
		for (int i = 0; i < 12 - maxCratesToSpawn; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation());
			if (!Object.op_Implicit((Object)(object)baseEntity))
			{
				continue;
			}
			float num = 3f;
			float num2 = 10f;
			Vector3 onUnitSphere = Random.get_onUnitSphere();
			((Component)baseEntity).get_transform().set_position(((Component)this).get_transform().get_position() + new Vector3(0f, 1.5f, 0f) + onUnitSphere * Random.Range(-4f, 4f));
			Collider component = ((Component)baseEntity).GetComponent<Collider>();
			baseEntity.Spawn();
			baseEntity.SetVelocity(zero + onUnitSphere * Random.Range(num, num2));
			foreach (ServerGib item in list)
			{
				Physics.IgnoreCollision(component, (Collider)(object)item.GetCollider(), true);
			}
		}
		for (int j = 0; j < maxCratesToSpawn; j++)
		{
			Vector3 onUnitSphere2 = Random.get_onUnitSphere();
			onUnitSphere2.y = 0f;
			((Vector3)(ref onUnitSphere2)).Normalize();
			Vector3 pos = ((Component)this).get_transform().get_position() + new Vector3(0f, 1.5f, 0f) + onUnitSphere2 * Random.Range(2f, 3f);
			BaseEntity baseEntity2 = GameManager.server.CreateEntity(crateToDrop.resourcePath, pos, Quaternion.LookRotation(onUnitSphere2));
			baseEntity2.Spawn();
			LootContainer lootContainer = baseEntity2 as LootContainer;
			if (Object.op_Implicit((Object)(object)lootContainer))
			{
				((FacepunchBehaviour)lootContainer).Invoke((Action)lootContainer.RemoveMe, 1800f);
			}
			Collider component2 = ((Component)baseEntity2).GetComponent<Collider>();
			Rigidbody val = ((Component)baseEntity2).get_gameObject().AddComponent<Rigidbody>();
			val.set_useGravity(true);
			val.set_collisionDetectionMode((CollisionDetectionMode)2);
			val.set_mass(2f);
			val.set_interpolation((RigidbodyInterpolation)1);
			val.set_velocity(zero + onUnitSphere2 * Random.Range(1f, 3f));
			val.set_angularVelocity(Vector3Ex.Range(-1.75f, 1.75f));
			val.set_drag(0.5f * (val.get_mass() / 5f));
			val.set_angularDrag(0.2f * (val.get_mass() / 5f));
			FireBall fireBall = GameManager.server.CreateEntity(this.fireBall.resourcePath) as FireBall;
			if (Object.op_Implicit((Object)(object)fireBall))
			{
				fireBall.SetParent(baseEntity2);
				fireBall.Spawn();
				((Component)fireBall).GetComponent<Rigidbody>().set_isKinematic(true);
				((Component)fireBall).GetComponent<Collider>().set_enabled(false);
			}
			((Component)baseEntity2).SendMessage("SetLockingEnt", (object)((Component)fireBall).get_gameObject(), (SendMessageOptions)1);
			foreach (ServerGib item2 in list)
			{
				Physics.IgnoreCollision(component2, (Collider)(object)item2.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.OnAttacked(info);
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			AddOrUpdateTarget(basePlayer, info.PointStart, info.damageTypes.Total());
		}
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (base.isServer)
		{
			SetFlag(Flags.Reserved2, base.healthFraction <= 0.75f);
			SetFlag(Flags.Reserved3, base.healthFraction < 0.4f);
		}
	}

	public void DoHealing()
	{
		if (!base.isClient && base.healthFraction < 1f && base.SecondsSinceAttacked > 600f)
		{
			float amount = MaxHealth() / 300f * Time.get_fixedDeltaTime();
			Heal(amount);
		}
	}
}
