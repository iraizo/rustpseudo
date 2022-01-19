using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class HotAirBalloon : BaseCombatEntity, SamSite.ISamSiteTarget
{
	protected const Flags Flag_HasFuel = Flags.Reserved6;

	protected const Flags Flag_HalfInflated = Flags.Reserved1;

	protected const Flags Flag_FullInflated = Flags.Reserved2;

	public Transform centerOfMass;

	public Rigidbody myRigidbody;

	public Transform buoyancyPoint;

	public float liftAmount = 10f;

	public Transform windSock;

	public Transform[] windFlags;

	public GameObject staticBalloonDeflated;

	public GameObject staticBalloon;

	public GameObject animatedBalloon;

	public Animator balloonAnimator;

	public Transform groundSample;

	public float inflationLevel;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public float fuelPerSec = 0.25f;

	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	public EntityRef<StorageContainer> storageUnitInstance;

	[Header("Damage")]
	public DamageRenderer damageRenderer;

	public Transform engineHeight;

	public GameObject[] killTriggers;

	private EntityFuelSystem fuelSystem;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	[ServerVar(Help = "How long before a HAB loses all its health while outside")]
	public static float outsidedecayminutes = 180f;

	public float windForce = 30000f;

	public Vector3 currentWindVec = Vector3.get_zero();

	public Bounds collapsedBounds;

	public Bounds raisedBounds;

	public GameObject[] balloonColliders;

	[ServerVar]
	public static float serviceCeiling = 200f;

	private float currentBuoyancy;

	private float lastBlastTime;

	private float avgTerrainHeight;

	protected bool grounded;

	public bool IsFullyInflated => inflationLevel >= 1f;

	public SamSite.SamTargetType SAMTargetType => SamSite.targetTypeVehicle;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("HotAirBalloon.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 578721460 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - EngineSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("EngineSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(578721460u, "EngineSwitch", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							EngineSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in EngineSwitch");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1851540757 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenFuel "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenFuel", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg3 = rPCMessage;
						RPC_OpenFuel(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in RPC_OpenFuel");
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

	public override void InitShared()
	{
		fuelSystem = new EntityFuelSystem(base.isServer, fuelStoragePrefab, children);
	}

	public override void Load(LoadInfo info)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.hotAirBalloon != null)
		{
			inflationLevel = info.msg.hotAirBalloon.inflationAmount;
			if (info.fromDisk && Object.op_Implicit((Object)(object)myRigidbody))
			{
				myRigidbody.set_velocity(info.msg.hotAirBalloon.velocity);
			}
		}
		if (info.msg.motorBoat != null)
		{
			fuelSystem.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	public bool WaterLogged()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return WaterLevel.Test(engineHeight.get_position(), waves: true, this);
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (isSpawned)
			{
				fuelSystem.CheckNewChild(child);
			}
			if (child.prefabID == storageUnitPrefab.GetEntity().prefabID)
			{
				storageUnitInstance.Set((StorageContainer)child);
			}
		}
	}

	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot && storageUnitInstance.IsValid(base.isServer))
		{
			storageUnitInstance.Get(base.isServer).DropItems();
		}
		base.DoServerDestroy();
	}

	public bool IsValidSAMTarget(bool staticRespawn)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (staticRespawn)
		{
			return IsFullyInflated;
		}
		if (IsFullyInflated)
		{
			return !BaseVehicle.InSafeZone(triggers, ((Component)this).get_transform().get_position());
		}
		return false;
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.On, b: false);
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			fuelSystem.LootFuel(player);
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.hotAirBalloon = Pool.Get<HotAirBalloon>();
		info.msg.hotAirBalloon.inflationAmount = inflationLevel;
		if (info.forDisk && Object.op_Implicit((Object)(object)myRigidbody))
		{
			info.msg.hotAirBalloon.velocity = myRigidbody.get_velocity();
		}
		info.msg.motorBoat = Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = fuelSystem.fuelStorageInstance.uid;
	}

	public override void ServerInit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		myRigidbody.set_centerOfMass(centerOfMass.get_localPosition());
		myRigidbody.set_isKinematic(false);
		avgTerrainHeight = TerrainMeta.HeightMap.GetHeight(((Component)this).get_transform().get_position());
		base.ServerInit();
		bounds = collapsedBounds;
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayTick, Random.Range(30f, 60f), 60f, 6f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateIsGrounded, 0f, 3f, 0.2f);
	}

	public void DecayTick()
	{
		if (base.healthFraction != 0f && !IsFullyInflated && !(Time.get_time() < lastBlastTime + 600f))
		{
			float num = 1f / outsidedecayminutes;
			if (IsOutside())
			{
				Hurt(MaxHealth() * num, DamageType.Decay, this, useProtection: false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void EngineSwitch(RPCMessage msg)
	{
		bool b = msg.read.Bit();
		SetFlag(Flags.On, b);
		if (IsOn())
		{
			((FacepunchBehaviour)this).Invoke((Action)ScheduleOff, 60f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ScheduleOff);
		}
	}

	public void ScheduleOff()
	{
		SetFlag(Flags.On, b: false);
	}

	public void UpdateIsGrounded()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!(lastBlastTime + 5f > Time.get_time()))
		{
			List<Collider> list = Pool.GetList<Collider>();
			GamePhysics.OverlapSphere(((Component)groundSample).get_transform().get_position(), 1.25f, list, 1218511105, (QueryTriggerInteraction)1);
			grounded = list.Count > 0;
			Pool.FreeList<Collider>(ref list);
		}
	}

	protected void FixedUpdate()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		if (!isSpawned || base.isClient)
		{
			return;
		}
		if (!fuelSystem.HasFuel() || WaterLogged())
		{
			SetFlag(Flags.On, b: false);
		}
		if (IsOn())
		{
			fuelSystem.TryUseFuel(Time.get_fixedDeltaTime(), fuelPerSec);
		}
		SetFlag(Flags.Reserved6, fuelSystem.HasFuel());
		bool flag = (IsFullyInflated && myRigidbody.get_velocity().y < 0f) || myRigidbody.get_velocity().y < 0.75f;
		GameObject[] array = killTriggers;
		foreach (GameObject val in array)
		{
			if (val.get_activeSelf() != flag)
			{
				val.SetActive(flag);
			}
		}
		float num = inflationLevel;
		if (IsOn() && !IsFullyInflated)
		{
			inflationLevel = Mathf.Clamp01(inflationLevel + Time.get_fixedDeltaTime() / 10f);
		}
		else if (grounded && inflationLevel > 0f && !IsOn() && (Time.get_time() > lastBlastTime + 30f || WaterLogged()))
		{
			inflationLevel = Mathf.Clamp01(inflationLevel - Time.get_fixedDeltaTime() / 10f);
		}
		if (num != inflationLevel)
		{
			if (IsFullyInflated)
			{
				bounds = raisedBounds;
			}
			else if (inflationLevel == 0f)
			{
				bounds = collapsedBounds;
			}
			SetFlag(Flags.Reserved1, inflationLevel > 0.3f);
			SetFlag(Flags.Reserved2, inflationLevel >= 1f);
			SendNetworkUpdate();
			_ = inflationLevel;
		}
		bool flag2 = !myRigidbody.IsSleeping() || inflationLevel > 0f;
		array = balloonColliders;
		foreach (GameObject val2 in array)
		{
			if (val2.get_activeSelf() != flag2)
			{
				val2.SetActive(flag2);
			}
		}
		if (IsOn())
		{
			if (IsFullyInflated)
			{
				currentBuoyancy += Time.get_fixedDeltaTime() * 0.2f;
				lastBlastTime = Time.get_time();
			}
		}
		else
		{
			currentBuoyancy -= Time.get_fixedDeltaTime() * 0.1f;
		}
		currentBuoyancy = Mathf.Clamp(currentBuoyancy, 0f, 0.8f + 0.2f * base.healthFraction);
		if (inflationLevel > 0f)
		{
			avgTerrainHeight = Mathf.Lerp(avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(((Component)this).get_transform().get_position()), Time.get_deltaTime());
			float num2 = 1f - Mathf.InverseLerp(avgTerrainHeight + serviceCeiling - 20f, avgTerrainHeight + serviceCeiling, buoyancyPoint.get_position().y);
			myRigidbody.AddForceAtPosition(Vector3.get_up() * (0f - Physics.get_gravity().y) * myRigidbody.get_mass() * 0.5f * inflationLevel, buoyancyPoint.get_position(), (ForceMode)0);
			myRigidbody.AddForceAtPosition(Vector3.get_up() * liftAmount * currentBuoyancy * num2, buoyancyPoint.get_position(), (ForceMode)0);
			Vector3 windAtPos = GetWindAtPos(buoyancyPoint.get_position());
			((Vector3)(ref windAtPos)).get_magnitude();
			float num3 = 1f;
			float num4 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(buoyancyPoint.get_position()), TerrainMeta.WaterMap.GetHeight(buoyancyPoint.get_position()));
			float num5 = Mathf.InverseLerp(num4 + 20f, num4 + 60f, buoyancyPoint.get_position().y);
			float num6 = 1f;
			RaycastHit val3 = default(RaycastHit);
			if (Physics.SphereCast(new Ray(((Component)this).get_transform().get_position() + Vector3.get_up() * 2f, Vector3.get_down()), 1.5f, ref val3, 5f, 1218511105))
			{
				num6 = Mathf.Clamp01(((RaycastHit)(ref val3)).get_distance() / 5f);
			}
			num3 *= num5 * num2 * num6;
			num3 *= 0.2f + 0.8f * base.healthFraction;
			Vector3 val4 = ((Vector3)(ref windAtPos)).get_normalized() * num3 * windForce;
			currentWindVec = Vector3.Lerp(currentWindVec, val4, Time.get_fixedDeltaTime() * 0.25f);
			myRigidbody.AddForceAtPosition(val4 * 0.1f, buoyancyPoint.get_position(), (ForceMode)0);
			myRigidbody.AddForce(val4 * 0.9f, (ForceMode)0);
		}
	}

	public Vector3 GetWindAtPos(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = pos.y * 6f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Sin(num * ((float)Math.PI / 180f)), 0f, Mathf.Cos(num * ((float)Math.PI / 180f)));
		return ((Vector3)(ref val)).get_normalized() * 1f;
	}

	public override bool SupportsChildDeployables()
	{
		return false;
	}
}
