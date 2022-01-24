using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class BaseVehicle : BaseMountable
{
	[Serializable]
	public class MountPointInfo
	{
		public bool isDriver;

		public Vector3 pos;

		public Vector3 rot;

		public string bone = "";

		public GameObjectRef prefab;

		[HideInInspector]
		public BaseMountable mountable;
	}

	public readonly struct Enumerable : IEnumerable<MountPointInfo>, IEnumerable
	{
		private readonly BaseVehicle _vehicle;

		public Enumerable(BaseVehicle vehicle)
		{
			if ((Object)(object)vehicle == (Object)null)
			{
				throw new ArgumentNullException("vehicle");
			}
			_vehicle = vehicle;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(_vehicle);
		}

		IEnumerator<MountPointInfo> IEnumerable<MountPointInfo>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public struct Enumerator : IEnumerator<MountPointInfo>, IEnumerator, IDisposable
	{
		private enum State
		{
			Direct,
			EnterChild,
			EnumerateChild,
			Finished
		}

		private class Box : IPooled
		{
			public Enumerator Value;

			public void EnterPool()
			{
				Value = default(Enumerator);
			}

			public void LeavePool()
			{
				Value = default(Enumerator);
			}
		}

		private readonly BaseVehicle _vehicle;

		private State _state;

		private int _index;

		private int _childIndex;

		private Box _enumerator;

		public MountPointInfo Current { get; private set; }

		object IEnumerator.Current => Current;

		public Enumerator(BaseVehicle vehicle)
		{
			if ((Object)(object)vehicle == (Object)null)
			{
				throw new ArgumentNullException("vehicle");
			}
			_vehicle = vehicle;
			_state = State.Direct;
			_index = -1;
			_childIndex = -1;
			_enumerator = null;
			Current = null;
		}

		public bool MoveNext()
		{
			Current = null;
			switch (_state)
			{
			case State.Direct:
				_index++;
				if (_index >= _vehicle.mountPoints.Count)
				{
					_state = State.EnterChild;
					goto case State.EnterChild;
				}
				Current = _vehicle.mountPoints[_index];
				return true;
			case State.EnterChild:
				do
				{
					_childIndex++;
				}
				while (_childIndex < _vehicle.childVehicles.Count && (Object)(object)_vehicle.childVehicles[_childIndex] == (Object)null);
				if (_childIndex >= _vehicle.childVehicles.Count)
				{
					_state = State.Finished;
					return false;
				}
				_enumerator = Pool.Get<Box>();
				_enumerator.Value = _vehicle.childVehicles[_childIndex].allMountPoints.GetEnumerator();
				_state = State.EnumerateChild;
				goto case State.EnumerateChild;
			case State.EnumerateChild:
				if (_enumerator.Value.MoveNext())
				{
					Current = _enumerator.Value.Current;
					return true;
				}
				_enumerator.Value.Dispose();
				Pool.Free<Box>(ref _enumerator);
				_state = State.EnterChild;
				goto case State.EnterChild;
			case State.Finished:
				return false;
			default:
				throw new NotSupportedException();
			}
		}

		public void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Value.Dispose();
				Pool.Free<Box>(ref _enumerator);
			}
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	private const float MIN_TIME_BETWEEN_PUSHES = 1f;

	public TimeSince timeSinceLastPush;

	private BaseVehicle pendingLoad;

	private Queue<BasePlayer> recentDrivers = new Queue<BasePlayer>();

	private Action clearRecentDriverAction;

	private float safeAreaRadius;

	private Vector3 safeAreaOrigin;

	private float spawnTime = -1f;

	[Tooltip("Allow players to mount other mountables/ladders from this vehicle")]
	public bool mountChaining = true;

	[FormerlySerializedAs("seatClipCheck")]
	public bool continuousClippingCheck;

	public bool shouldShowHudHealth;

	public bool ignoreDamageFromOutside;

	[Header("Rigidbody (Optional)")]
	public Rigidbody rigidBody;

	[Header("Mount Points")]
	public List<MountPointInfo> mountPoints;

	public bool doClippingAndVisChecks = true;

	[Header("Damage")]
	public DamageRenderer damageRenderer;

	[FormerlySerializedAs("explosionDamageMultiplier")]
	public float explosionForceMultiplier = 400f;

	public float explosionForceMax = 75000f;

	public const Flags Flag_OnlyOwnerEntry = Flags.Locked;

	public const Flags Flag_Headlights = Flags.Reserved5;

	public const Flags Flag_Stationary = Flags.Reserved7;

	public const Flags Flag_SeatsFull = Flags.Reserved11;

	private readonly List<BaseVehicle> childVehicles = new List<BaseVehicle>(0);

	public virtual bool AlwaysAllowBradleyTargeting => false;

	protected bool RecentlyPushed => TimeSince.op_Implicit(timeSinceLastPush) < 1f;

	protected override bool PositionTickFixedTime => true;

	protected virtual bool CanSwapSeats => true;

	public override float RealisticMass => rigidBody.get_mass();

	public Enumerable allMountPoints => new Enumerable(this);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseVehicle.OnRpcMessage", 0);
		try
		{
			if (rpc == 2115395408 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_WantsPush "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsPush", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2115395408u, "RPC_WantsPush", this, player, 5f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							RPC_WantsPush(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_WantsPush");
					}
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

	public override void OnAttacked(HitInfo info)
	{
		if (IsSafe() && !info.damageTypes.Has(DamageType.Decay))
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.OnAttacked(info);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		ClearOwnerEntry();
		CheckAndSpawnMountPoints();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!base.isServer || !info.forDisk)
		{
			return;
		}
		info.msg.baseVehicle = Pool.Get<BaseVehicle>();
		info.msg.baseVehicle.mountPoints = Pool.GetList<MountPoint>();
		for (int i = 0; i < mountPoints.Count; i++)
		{
			MountPointInfo mountPointInfo = mountPoints[i];
			if (!((Object)(object)mountPointInfo.mountable == (Object)null))
			{
				MountPoint val = Pool.Get<MountPoint>();
				val.index = i;
				val.mountableId = mountPointInfo.mountable.net.ID;
				info.msg.baseVehicle.mountPoints.Add(val);
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.baseVehicle != null)
		{
			BaseVehicle obj = pendingLoad;
			if (obj != null)
			{
				obj.Dispose();
			}
			pendingLoad = info.msg.baseVehicle;
			info.msg.baseVehicle = null;
		}
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	public bool AnyMounted()
	{
		return (float)NumMounted() > 0f;
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (continuousClippingCheck && AnyMounted())
		{
			Vector3 val = ((Component)this).get_transform().TransformPoint(((Bounds)(ref bounds)).get_center());
			int num = (IsFlipped() ? 1218511105 : 1210122497);
			if (Physics.OverlapBox(val, ((Bounds)(ref bounds)).get_extents(), ((Component)this).get_transform().get_rotation(), num).Length != 0)
			{
				CheckSeatsForClipping(num);
			}
		}
		if (Object.op_Implicit((Object)(object)rigidBody))
		{
			SetFlag(Flags.Reserved7, rigidBody.IsSleeping() && !AnyMounted());
		}
		if (OnlyOwnerAccessible() && safeAreaRadius != -1f && Vector3.Distance(((Component)this).get_transform().get_position(), safeAreaOrigin) > safeAreaRadius)
		{
			ClearOwnerEntry();
		}
	}

	public virtual int StartingFuelUnits()
	{
		return -1;
	}

	public bool InSafeZone()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return InSafeZone(triggers, ((Component)this).get_transform().get_position());
	}

	public static bool InSafeZone(List<TriggerBase> triggers, Vector3 position)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = triggers[i] as TriggerSafeZone;
				if (!((Object)(object)triggerSafeZone == (Object)null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(position);
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
		}
		return num > 0f;
	}

	public virtual bool IsSeatVisible(BaseMountable mountable, Vector3 eyePos, int mask = 1218511105)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!doClippingAndVisChecks)
		{
			return true;
		}
		if ((Object)(object)mountable == (Object)null)
		{
			return false;
		}
		Vector3 p = ((Component)mountable).get_transform().get_position() + ((Component)this).get_transform().get_up() * 0.15f;
		return GamePhysics.LineOfSight(eyePos, p, mask);
	}

	public virtual bool IsSeatClipping(BaseMountable mountable, int mask = 1218511105)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!doClippingAndVisChecks)
		{
			return false;
		}
		if ((Object)(object)mountable == (Object)null)
		{
			return false;
		}
		Vector3 position = ((Component)mountable).get_transform().get_position();
		Vector3 position2 = ((Component)mountable.eyePositionOverride).get_transform().get_position();
		Vector3 val = position2 - position;
		float num = 0.4f;
		if (mountable.modifiesPlayerCollider)
		{
			num = Mathf.Min(num, mountable.customPlayerCollider.radius);
		}
		Vector3 start = position2 - val * (num - 0.15f);
		Vector3 end = position + val * (num + 0.05f);
		return GamePhysics.CheckCapsule(start, end, num, mask, (QueryTriggerInteraction)1);
	}

	public virtual void CheckSeatsForClipping(int mask)
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			BaseMountable mountable = mountPoint.mountable;
			if (!((Object)(object)mountable == (Object)null) && mountable.IsMounted() && IsSeatClipping(mountable, mask))
			{
				SeatClippedWorld(mountable);
			}
		}
	}

	public virtual void SeatClippedWorld(BaseMountable mountable)
	{
		mountable.DismountPlayer(mountable.GetMounted());
	}

	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
	}

	public override void DismountAllPlayers()
	{
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null)
			{
				allMountPoint.mountable.DismountAllPlayers();
			}
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		clearRecentDriverAction = ClearRecentDriver;
	}

	public virtual void SpawnSubEntities()
	{
		CheckAndSpawnMountPoints();
	}

	private void CheckAndSpawnMountPoints()
	{
		if (pendingLoad?.mountPoints != null)
		{
			foreach (MountPoint mountPoint in pendingLoad.mountPoints)
			{
				EntityRef<BaseMountable> entityRef = new EntityRef<BaseMountable>(mountPoint.mountableId);
				if (!entityRef.IsValid(serverside: true))
				{
					Debug.LogError((object)$"Loaded a mountpoint which doesn't exist: {mountPoint.index}", (Object)(object)this);
					continue;
				}
				if (mountPoint.index < 0 || mountPoint.index >= mountPoints.Count)
				{
					Debug.LogError((object)$"Loaded a mountpoint which has no info: {mountPoint.index}", (Object)(object)this);
					entityRef.Get(serverside: true).Kill();
					continue;
				}
				MountPointInfo mountPointInfo = mountPoints[mountPoint.index];
				if ((Object)(object)mountPointInfo.mountable != (Object)null)
				{
					Debug.LogError((object)$"Loading a mountpoint after one was already set: {mountPoint.index}", (Object)(object)this);
					mountPointInfo.mountable.Kill();
				}
				mountPointInfo.mountable = entityRef.Get(serverside: true);
				if (!mountPointInfo.mountable.enableSaving)
				{
					mountPointInfo.mountable.EnableSaving(wants: true);
				}
			}
		}
		BaseVehicle obj = pendingLoad;
		if (obj != null)
		{
			obj.Dispose();
		}
		pendingLoad = null;
		for (int i = 0; i < mountPoints.Count; i++)
		{
			SpawnMountPoint(mountPoints[i], model);
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer && !Application.isLoadingSave)
		{
			SpawnSubEntities();
		}
	}

	public override void Hurt(HitInfo info)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDead() && (Object)(object)rigidBody != (Object)null && !rigidBody.get_isKinematic())
		{
			float num = info.damageTypes.Get(DamageType.Explosion) + info.damageTypes.Get(DamageType.AntiVehicle);
			if (num > 3f)
			{
				float num2 = Mathf.Min(num * explosionForceMultiplier, explosionForceMax);
				rigidBody.AddExplosionForce(num2, info.HitPositionWorld, 1f, 2.5f);
			}
		}
		base.Hurt(info);
	}

	public int NumMounted()
	{
		if (!HasMountPoints())
		{
			if (!IsMounted())
			{
				return 0;
			}
			return 1;
		}
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null && (Object)(object)allMountPoint.mountable.GetMounted() != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public int MaxMounted()
	{
		if (!HasMountPoints())
		{
			return 1;
		}
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public bool HasDriver()
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver && allMountPoint.mountable.IsMounted())
				{
					return true;
				}
			}
			return false;
		}
		return base.IsMounted();
	}

	public bool IsDriver(BasePlayer player)
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver)
				{
					BasePlayer mounted = allMountPoint.mountable.GetMounted();
					if ((Object)(object)mounted != (Object)null && (Object)(object)mounted == (Object)(object)player)
					{
						return true;
					}
				}
			}
		}
		else if ((Object)(object)_mounted != (Object)null)
		{
			return (Object)(object)_mounted == (Object)(object)player;
		}
		return false;
	}

	public BasePlayer GetDriver()
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver)
				{
					BasePlayer mounted = allMountPoint.mountable.GetMounted();
					if ((Object)(object)mounted != (Object)null)
					{
						return mounted;
					}
				}
			}
		}
		else if ((Object)(object)_mounted != (Object)null)
		{
			return _mounted;
		}
		return null;
	}

	public void GetDrivers(List<BasePlayer> drivers)
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver)
				{
					BasePlayer mounted = allMountPoint.mountable.GetMounted();
					if ((Object)(object)mounted != (Object)null)
					{
						drivers.Add(mounted);
					}
				}
			}
		}
		else if ((Object)(object)_mounted != (Object)null)
		{
			drivers.Add(_mounted);
		}
	}

	public BasePlayer GetPlayerDamageInitiator()
	{
		if (HasDriver())
		{
			return GetDriver();
		}
		if (recentDrivers.get_Count() <= 0)
		{
			return null;
		}
		return recentDrivers.Peek();
	}

	public int GetPlayerSeat(BasePlayer player)
	{
		if (!HasMountPoints() && (Object)(object)GetMounted() == (Object)(object)player)
		{
			return 0;
		}
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null && (Object)(object)allMountPoint.mountable.GetMounted() == (Object)(object)player)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public MountPointInfo GetPlayerSeatInfo(BasePlayer player)
	{
		if (!HasMountPoints())
		{
			return null;
		}
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null && (Object)(object)allMountPoint.mountable.GetMounted() == (Object)(object)player)
			{
				return allMountPoint;
			}
		}
		return null;
	}

	public void SwapSeats(BasePlayer player, int targetSeat = 0)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!HasMountPoints() || !CanSwapSeats)
		{
			return;
		}
		int playerSeat = GetPlayerSeat(player);
		if (playerSeat == -1)
		{
			return;
		}
		BaseMountable mountable = GetMountPoint(playerSeat).mountable;
		int num = playerSeat;
		BaseMountable baseMountable = null;
		if ((Object)(object)baseMountable == (Object)null)
		{
			int num2 = MaxMounted();
			for (int i = 0; i < num2; i++)
			{
				num++;
				if (num >= num2)
				{
					num = 0;
				}
				MountPointInfo mountPoint = GetMountPoint(num);
				if ((Object)(object)mountPoint?.mountable != (Object)null && !mountPoint.mountable.IsMounted() && mountPoint.mountable.CanSwapToThis(player) && !IsSeatClipping(mountPoint.mountable) && IsSeatVisible(mountPoint.mountable, player.eyes.position))
				{
					baseMountable = mountPoint.mountable;
					break;
				}
			}
		}
		if ((Object)(object)baseMountable != (Object)null && (Object)(object)baseMountable != (Object)(object)mountable)
		{
			mountable.DismountPlayer(player, lite: true);
			baseMountable.MountPlayer(player);
			player.MarkSwapSeat();
		}
	}

	public bool HasDriverMountPoints()
	{
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if (allMountPoint.isDriver)
			{
				return true;
			}
		}
		return false;
	}

	public bool OnlyOwnerAccessible()
	{
		return HasFlag(Flags.Locked);
	}

	public bool IsDespawnEligable()
	{
		if (spawnTime != -1f)
		{
			return spawnTime + 300f < Time.get_realtimeSinceStartup();
		}
		return true;
	}

	public void SetupOwner(BasePlayer owner, Vector3 newSafeAreaOrigin, float newSafeAreaRadius)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)owner != (Object)null)
		{
			creatorEntity = owner;
			SetFlag(Flags.Locked, b: true);
			safeAreaRadius = newSafeAreaRadius;
			safeAreaOrigin = newSafeAreaOrigin;
			spawnTime = Time.get_realtimeSinceStartup();
		}
	}

	public void ClearOwnerEntry()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		creatorEntity = null;
		SetFlag(Flags.Locked, b: false);
		safeAreaRadius = -1f;
		safeAreaOrigin = Vector3.get_zero();
	}

	public virtual EntityFuelSystem GetFuelSystem()
	{
		return null;
	}

	public bool IsSafe()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (OnlyOwnerAccessible())
		{
			return Vector3.Distance(safeAreaOrigin, ((Component)this).get_transform().get_position()) <= safeAreaRadius;
		}
		return false;
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		if (IsSafe())
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.ScaleDamageForPlayer(player, info);
	}

	public BaseMountable GetIdealMountPoint(Vector3 eyePos, Vector3 pos, BasePlayer playerFor = null)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (!HasMountPoints())
		{
			return this;
		}
		BasePlayer basePlayer = creatorEntity as BasePlayer;
		bool flag = (Object)(object)basePlayer != (Object)null;
		bool flag2 = flag && basePlayer.Team != null;
		bool flag3 = flag && (Object)(object)playerFor == (Object)(object)basePlayer;
		if (!flag3 && flag && OnlyOwnerAccessible() && (Object)(object)playerFor != (Object)null && (playerFor.Team == null || !playerFor.Team.members.Contains(basePlayer.userID)))
		{
			return null;
		}
		BaseMountable result = null;
		float num = float.PositiveInfinity;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if (allMountPoint.mountable.IsMounted())
			{
				continue;
			}
			float num2 = Vector3.Distance(allMountPoint.mountable.mountAnchor.get_position(), pos);
			if (num2 > num)
			{
				continue;
			}
			if (IsSeatClipping(allMountPoint.mountable))
			{
				if (Application.get_isEditor())
				{
					Debug.Log((object)$"Skipping seat {allMountPoint.mountable} - it's clipping");
				}
			}
			else if (!IsSeatVisible(allMountPoint.mountable, eyePos))
			{
				if (Application.get_isEditor())
				{
					Debug.Log((object)$"Skipping seat {allMountPoint.mountable} - it's not visible");
				}
			}
			else if (!(OnlyOwnerAccessible() && flag3) || flag2 || allMountPoint.isDriver)
			{
				result = allMountPoint.mountable;
				num = num2;
			}
		}
		return result;
	}

	public virtual bool MountEligable(BasePlayer player)
	{
		if ((Object)(object)creatorEntity != (Object)null && OnlyOwnerAccessible() && (Object)(object)player != (Object)(object)creatorEntity)
		{
			BasePlayer basePlayer = creatorEntity as BasePlayer;
			if ((Object)(object)basePlayer != (Object)null && basePlayer.Team != null && !basePlayer.Team.members.Contains(player.userID))
			{
				return false;
			}
		}
		return true;
	}

	public int GetIndexFromSeat(BaseMountable seat)
	{
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable == (Object)(object)seat)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public virtual void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		UpdateFullFlag();
	}

	public virtual void PrePlayerDismount(BasePlayer player, BaseMountable seat)
	{
	}

	public virtual void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		recentDrivers.Enqueue(player);
		if (!((FacepunchBehaviour)this).IsInvoking(clearRecentDriverAction))
		{
			((FacepunchBehaviour)this).Invoke(clearRecentDriverAction, 3f);
		}
		UpdateFullFlag();
	}

	private void UpdateFullFlag()
	{
		bool b = NumMounted() == MaxMounted();
		SetFlag(Flags.Reserved11, b);
	}

	private void ClearRecentDriver()
	{
		if (recentDrivers.get_Count() > 0)
		{
			recentDrivers.Dequeue();
		}
		if (recentDrivers.get_Count() > 0)
		{
			((FacepunchBehaviour)this).Invoke(clearRecentDriverAction, 3f);
		}
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if ((Object)(object)_mounted != (Object)null || !MountEligable(player))
		{
			return;
		}
		BaseMountable idealMountPointFor = GetIdealMountPointFor(player);
		if (!((Object)(object)idealMountPointFor == (Object)null))
		{
			if ((Object)(object)idealMountPointFor == (Object)(object)this)
			{
				base.AttemptMount(player, doMountChecks);
			}
			else
			{
				idealMountPointFor.AttemptMount(player, doMountChecks);
			}
			if ((Object)(object)player.GetMountedVehicle() == (Object)(object)this)
			{
				PlayerMounted(player, idealMountPointFor);
			}
		}
	}

	protected BaseMountable GetIdealMountPointFor(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return GetIdealMountPoint(player.eyes.position, player.eyes.position + player.eyes.HeadForward() * 1f, player);
	}

	public override bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		List<Vector3> list = Pool.GetList<Vector3>();
		Vector3 visualCheckOrigin = player.TriggerPoint();
		Transform[] array = dismountPositions;
		foreach (Transform val in array)
		{
			if (ValidDismountPosition(((Component)val).get_transform().get_position(), visualCheckOrigin))
			{
				list.Add(((Component)val).get_transform().get_position());
			}
		}
		if (list.Count == 0)
		{
			Debug.LogWarning((object)("Failed to find dismount position for player :" + player.displayName + " / " + player.userID + " on obj : " + ((Object)((Component)this).get_gameObject()).get_name()));
			Pool.FreeList<Vector3>(ref list);
			res = ((Component)player).get_transform().get_position();
			return false;
		}
		Vector3 pos = ((Component)player).get_transform().get_position();
		list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
		res = list[0];
		Pool.FreeList<Vector3>(ref list);
		return true;
	}

	private BaseMountable SpawnMountPoint(MountPointInfo mountToSpawn, Model model)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mountToSpawn.mountable != (Object)null)
		{
			return mountToSpawn.mountable;
		}
		Vector3 val = Quaternion.Euler(mountToSpawn.rot) * Vector3.get_forward();
		Vector3 pos = mountToSpawn.pos;
		Vector3 up = Vector3.get_up();
		if (mountToSpawn.bone != "")
		{
			pos = ((Component)model.FindBone(mountToSpawn.bone)).get_transform().get_position() + ((Component)this).get_transform().TransformDirection(mountToSpawn.pos);
			val = ((Component)this).get_transform().TransformDirection(val);
			up = ((Component)this).get_transform().get_up();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mountToSpawn.prefab.resourcePath, pos, Quaternion.LookRotation(val, up));
		BaseMountable baseMountable = baseEntity as BaseMountable;
		if ((Object)(object)baseMountable != (Object)null)
		{
			if (!baseMountable.enableSaving)
			{
				baseMountable.EnableSaving(wants: true);
			}
			if (mountToSpawn.bone != "")
			{
				baseMountable.SetParent(this, mountToSpawn.bone, worldPositionStays: true, sendImmediate: true);
			}
			else
			{
				baseMountable.SetParent(this);
			}
			baseMountable.Spawn();
			mountToSpawn.mountable = baseMountable;
		}
		else
		{
			Debug.LogError((object)"MountPointInfo prefab is not a BaseMountable. Cannot spawn mount point.");
			if ((Object)(object)baseEntity != (Object)null)
			{
				baseEntity.Kill();
			}
		}
		return baseMountable;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(5f)]
	public void RPC_WantsPush(RPCMessage msg)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!player.isMounted && !RecentlyPushed && CanPushNow(player) && (!OnlyOwnerAccessible() || !((Object)(object)player != (Object)(object)creatorEntity)))
		{
			player.metabolism.calories.Subtract(3f);
			player.metabolism.SendChangesToClient();
			if (rigidBody.IsSleeping())
			{
				rigidBody.WakeUp();
			}
			DoPushAction(player);
			timeSinceLastPush = TimeSince.op_Implicit(0f);
		}
	}

	protected virtual void DoPushAction(BasePlayer player)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (IsFlipped())
		{
			float num = rigidBody.get_mass() * 8f;
			Vector3 val = Vector3.get_forward() * num;
			if (Vector3.Dot(((Component)this).get_transform().InverseTransformVector(((Component)this).get_transform().get_position() - ((Component)player).get_transform().get_position()), Vector3.get_right()) > 0f)
			{
				val *= -1f;
			}
			if (((Component)this).get_transform().get_up().y < 0f)
			{
				val *= -1f;
			}
			rigidBody.AddRelativeTorque(val, (ForceMode)1);
		}
		else
		{
			Vector3 val2 = Vector3.ProjectOnPlane(((Component)this).get_transform().get_position() - player.eyes.position, ((Component)this).get_transform().get_up());
			Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
			float num2 = rigidBody.get_mass() * 4f;
			rigidBody.AddForce(normalized * num2, (ForceMode)1);
		}
	}

	public bool IsStationary()
	{
		return HasFlag(Flags.Reserved7);
	}

	public bool IsMoving()
	{
		return !HasFlag(Flags.Reserved7);
	}

	public override bool PlayerIsMounted(BasePlayer player)
	{
		if (player.IsValid())
		{
			return (Object)(object)player.GetMountedVehicle() == (Object)(object)this;
		}
		return false;
	}

	protected virtual bool CanPushNow(BasePlayer pusher)
	{
		return !IsOn();
	}

	public bool HasMountPoints()
	{
		if (mountPoints.Count > 0)
		{
			return true;
		}
		using (Enumerator enumerator = allMountPoints.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				return true;
			}
		}
		return false;
	}

	public override bool IsMounted()
	{
		if (base.isServer)
		{
			return HasDriver();
		}
		throw new NotImplementedException("Please don't call BaseVehicle IsMounted on the client.");
	}

	public override bool SupportsChildDeployables()
	{
		return false;
	}

	public bool IsFlipped()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Dot(Vector3.get_up(), ((Component)this).get_transform().get_up()) <= 0f;
	}

	public virtual bool IsVehicleRoot()
	{
		return true;
	}

	public override bool DirectlyMountable()
	{
		return IsVehicleRoot();
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		BaseVehicle baseVehicle;
		if ((baseVehicle = child as BaseVehicle) != null && !baseVehicle.IsVehicleRoot() && !childVehicles.Contains(baseVehicle))
		{
			childVehicles.Add(baseVehicle);
		}
	}

	protected override void OnChildRemoved(BaseEntity child)
	{
		base.OnChildRemoved(child);
		BaseVehicle baseVehicle;
		if ((baseVehicle = child as BaseVehicle) != null && !baseVehicle.IsVehicleRoot())
		{
			childVehicles.Remove(baseVehicle);
		}
	}

	public MountPointInfo GetMountPoint(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index < mountPoints.Count)
		{
			return mountPoints[index];
		}
		index -= mountPoints.Count;
		int num = 0;
		foreach (BaseVehicle childVehicle in childVehicles)
		{
			if ((Object)(object)childVehicle == (Object)null)
			{
				continue;
			}
			foreach (MountPointInfo allMountPoint in childVehicle.allMountPoints)
			{
				if (num == index)
				{
					return allMountPoint;
				}
				num++;
			}
		}
		return null;
	}
}
