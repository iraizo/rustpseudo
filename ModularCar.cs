using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

public class ModularCar : BaseModularVehicle, TriggerHurtNotChild.IHurtTriggerUser, TakeCollisionDamage.ICanRestoreVelocity, IVehicleLockUser, IEngineControllerUser, IEntity
{
	private class DriverSeatInputs
	{
		public float steerInput;

		public bool steerMod;

		public float brakeInput;

		public float throttleInput;
	}

	[Serializable]
	public class Wheel
	{
		public WheelCollider wheelCollider;

		public Transform visualWheel;

		public Transform visualWheelSteering;

		public bool steerWheel;

		public bool brakeWheel = true;

		public bool powerWheel = true;

		public bool visualPowerWheel = true;

		public ParticleSystem snowFX;

		public ParticleSystem sandFX;

		public ParticleSystem dirtFX;

		public ParticleSystem waterFX;

		public ParticleSystem snowSpinFX;

		public ParticleSystem sandSpinFX;

		public ParticleSystem dirtSpinFX;

		public ParticleSystem asphaltSpinFX;
	}

	[Serializable]
	public class SpawnSettings
	{
		[Tooltip("Must be true to use any of these settings.")]
		public bool useSpawnSettings;

		[Tooltip("Specify a list of possible module configurations that'll automatically spawn with this vehicle.")]
		public ModularCarPresetConfig[] configurationOptions;

		[Tooltip("Min health % at spawn for any modules that spawn with this chassis.")]
		public float minStartHealthPercent = 0.15f;

		[Tooltip("Max health  % at spawn for any modules that spawn with this chassis.")]
		public float maxStartHealthPercent = 0.5f;
	}

	public static HashSet<ModularCar> allCarsList = new HashSet<ModularCar>();

	private readonly ListDictionary<BaseMountable, DriverSeatInputs> driverSeatInputs = new ListDictionary<BaseMountable, DriverSeatInputs>();

	private ModularCarPhysics carPhysics;

	private VehicleTerrainHandler serverTerrainHandler;

	private float nextCollisionFXTime;

	private float nextCollisionDamageTime;

	private float lastEngineOnTime;

	private const float DECAY_TICK_TIME = 60f;

	private const float INSIDE_DECAY_MULTIPLIER = 0.1f;

	private const float CORPSE_DECAY_MINUTES = 5f;

	private Vector3 prevPosition;

	private Quaternion prevRotation;

	private float deathDamageCounter;

	private const float DAMAGE_TO_GIB = 600f;

	private TimeSince timeSinceDeath;

	private const float IMMUNE_TIME = 1f;

	private Dictionary<BaseEntity, float> damageSinceLastTick = new Dictionary<BaseEntity, float>();

	protected readonly Vector3 groundedCOMMultiplier = new Vector3(0.25f, 0.3f, 0.25f);

	protected readonly Vector3 airbourneCOMMultiplier = new Vector3(0.25f, 0.75f, 0.25f);

	private Vector3 prevCOMMultiplier;

	[Header("Modular Car")]
	public ModularCarChassisVisuals chassisVisuals;

	public Wheel wheelFL;

	public Wheel wheelFR;

	public Wheel wheelRL;

	public Wheel wheelRR;

	public ItemDefinition carKeyDefinition;

	[SerializeField]
	private ModularCarSettings carSettings;

	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	[SerializeField]
	private ProtectionProperties immortalProtection;

	[SerializeField]
	private ProtectionProperties mortalProtection;

	[Header("Spawn")]
	[SerializeField]
	private SpawnSettings spawnSettings;

	[Header("Fuel")]
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	[SerializeField]
	private Transform fuelStoragePoint;

	[Header("Audio/FX")]
	[SerializeField]
	private ModularCarAudio carAudio;

	[SerializeField]
	private GameObjectRef collisionEffect;

	[SerializeField]
	[HideInInspector]
	private MeshRenderer[] damageShowingRenderers;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 3f;

	[ServerVar(Help = "How many minutes before a ModularCar loses all its health while outside")]
	public static float outsidedecayminutes = 216f;

	public const BUTTON MouseSteerButton = BUTTON.DUCK;

	public const BUTTON RapidSteerButton = BUTTON.SPRINT;

	public ModularCarLock carLock;

	private VehicleEngineController<ModularCar> engineController;

	private VehicleEngineController<ModularCar>.EngineState lastSetEngineState;

	private float cachedFuelFraction;

	public override bool AlwaysAllowBradleyTargeting => true;

	public VehicleTerrainHandler.Surface OnSurface
	{
		get
		{
			if (serverTerrainHandler == null)
			{
				return VehicleTerrainHandler.Surface.Default;
			}
			return serverTerrainHandler.OnSurface;
		}
	}

	public float DriveWheelVelocity
	{
		get
		{
			if (base.isServer)
			{
				return carPhysics.DriveWheelVelocity;
			}
			return 0f;
		}
	}

	public float DriveWheelSlip
	{
		get
		{
			if (base.isServer)
			{
				return carPhysics.DriveWheelSlip;
			}
			return 0f;
		}
	}

	public float SteerAngle
	{
		get
		{
			if (base.isServer)
			{
				return carPhysics.SteerAngle;
			}
			return 0f;
		}
	}

	public ItemDefinition AssociatedItemDef => repair.itemTarget;

	public float MaxSteerAngle => carSettings.maxSteerAngle;

	public override bool IsLockable => carLock.HasALock;

	public VehicleEngineController<ModularCar>.EngineState CurEngineState => engineController.CurEngineState;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ModularCar.OnRpcMessage", 0);
		try
		{
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
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg2 = rPCMessage;
						RPC_OpenFuel(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
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

	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			ProcessCollision(collision);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		carPhysics = new ModularCarPhysics(this, ((Component)this).get_transform(), rigidBody, carSettings);
		serverTerrainHandler = new VehicleTerrainHandler(this);
		if (!Application.isLoadingSave)
		{
			SpawnPreassignedModules();
		}
		lastEngineOnTime = Time.get_realtimeSinceStartup();
		allCarsList.Add(this);
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateClients, 0f, 0.15f, 0.02f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayTick, Random.Range(30f, 60f), 60f, 6f);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		allCarsList.Remove(this);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		carLock.EnableCentralLockingIfNoDriver();
		if (IsDead())
		{
			Kill();
		}
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && isSpawned)
		{
			GetFuelSystem().CheckNewChild(child);
		}
	}

	public override EntityFuelSystem GetFuelSystem()
	{
		return engineController.FuelSystem;
	}

	public float GetPlayerDamageMultiplier()
	{
		return Mathf.Abs(GetSpeed()) * 1f;
	}

	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient && !hurtEntity.IsDestroyed)
		{
			Vector3 val = hurtEntity.GetLocalVelocity() - base.Velocity;
			Vector3 val2 = ClosestPoint(((Component)hurtEntity).get_transform().get_position());
			Vector3 val3 = hurtEntity.RealisticMass * val;
			rigidBody.AddForceAtPosition(val3 * 1.25f, val2, (ForceMode)1);
			QueueCollisionDamage(this, ((Vector3)(ref val3)).get_magnitude() * 0.75f / Time.get_deltaTime());
			carPhysics.SetTempDrag(2.25f, 1f);
		}
	}

	public float GetSteerInput()
	{
		float num = 0f;
		BufferList<DriverSeatInputs> values = driverSeatInputs.get_Values();
		for (int i = 0; i < values.get_Count(); i++)
		{
			num += values.get_Item(i).steerInput;
		}
		return Mathf.Clamp(num, -1f, 1f);
	}

	public bool GetSteerModInput()
	{
		BufferList<DriverSeatInputs> values = driverSeatInputs.get_Values();
		for (int i = 0; i < values.get_Count(); i++)
		{
			if (values.get_Item(i).steerMod)
			{
				return true;
			}
		}
		return false;
	}

	public override void VehicleFixedUpdate()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		float speed = GetSpeed();
		carPhysics.FixedUpdate(Time.get_fixedDeltaTime(), speed);
		engineController.CheckEngineState();
		((Component)hurtTriggerFront).get_gameObject().SetActive(speed > hurtTriggerMinSpeed);
		((Component)hurtTriggerRear).get_gameObject().SetActive(speed < 0f - hurtTriggerMinSpeed);
		if (serverTerrainHandler != null)
		{
			serverTerrainHandler.FixedUpdate();
		}
		SetFlag(Flags.Reserved7, rigidBody.get_position() == prevPosition && rigidBody.get_rotation() == prevRotation);
		prevPosition = rigidBody.get_position();
		prevRotation = rigidBody.get_rotation();
		if (IsMoving())
		{
			Vector3 cOMMultiplier = GetCOMMultiplier();
			if (cOMMultiplier != prevCOMMultiplier)
			{
				rigidBody.set_centerOfMass(Vector3.Scale(realLocalCOM, cOMMultiplier));
				prevCOMMultiplier = cOMMultiplier;
			}
		}
		if (!(Time.get_time() >= nextCollisionDamageTime))
		{
			return;
		}
		nextCollisionDamageTime = Time.get_time() + 0.33f;
		foreach (KeyValuePair<BaseEntity, float> item in damageSinceLastTick)
		{
			DoCollisionDamage(item.Key, item.Value);
		}
		damageSinceLastTick.Clear();
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		MountPointInfo playerSeatInfo = GetPlayerSeatInfo(player);
		if (playerSeatInfo == null || !playerSeatInfo.isDriver)
		{
			return;
		}
		if (!this.driverSeatInputs.Contains(playerSeatInfo.mountable))
		{
			this.driverSeatInputs.Add(playerSeatInfo.mountable, new DriverSeatInputs());
		}
		DriverSeatInputs driverSeatInputs = this.driverSeatInputs.get_Item(playerSeatInfo.mountable);
		if (inputState.IsDown(BUTTON.DUCK))
		{
			driverSeatInputs.steerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			driverSeatInputs.steerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				driverSeatInputs.steerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				driverSeatInputs.steerInput = 1f;
			}
		}
		driverSeatInputs.steerMod = inputState.IsDown(BUTTON.SPRINT);
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		driverSeatInputs.throttleInput = 0f;
		driverSeatInputs.brakeInput = 0f;
		if (GetSpeed() > 3f && num < -0.1f)
		{
			driverSeatInputs.throttleInput = 0f;
			driverSeatInputs.brakeInput = 0f - num;
		}
		else
		{
			driverSeatInputs.throttleInput = num;
			driverSeatInputs.brakeInput = 0f;
		}
		for (int i = 0; i < base.NumAttachedModules; i++)
		{
			base.AttachedModuleEntities[i].PlayerServerInput(inputState, player);
		}
		if (engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			engineController.TryStartEngine(player);
		}
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		DriverSeatInputs driverSeatInputs = default(DriverSeatInputs);
		if (this.driverSeatInputs.TryGetValue(seat, ref driverSeatInputs))
		{
			this.driverSeatInputs.Remove(seat);
		}
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if ((Object)(object)attachedModuleEntity != (Object)null)
			{
				attachedModuleEntity.OnPlayerDismountedVehicle(player);
			}
		}
		carLock.EnableCentralLockingIfNoDriver();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.modularCar = Pool.Get<ModularCar>();
		info.msg.modularCar.steerAngle = SteerAngle;
		info.msg.modularCar.driveWheelVel = DriveWheelVelocity;
		info.msg.modularCar.throttleInput = GetThrottleInput();
		info.msg.modularCar.brakeInput = GetBrakeInput();
		info.msg.modularCar.fuelStorageID = engineController.FuelSystem.fuelStorageInstance.uid;
		info.msg.modularCar.fuelFraction = GetFuelFraction();
		info.msg.modularCar.lockID = carLock.LockID;
	}

	public override void Hurt(HitInfo info)
	{
		if (!IsDead() && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			PropagateDamageToModules(info, 0.5f / (float)base.NumAttachedModules, 0.9f / (float)base.NumAttachedModules, null);
		}
		base.Hurt(info);
	}

	public void TickFuel(float fuelUsedPerSecond)
	{
		engineController.TickFuel(fuelUsedPerSecond);
	}

	public override bool MountEligable(BasePlayer player)
	{
		if (!base.MountEligable(player))
		{
			return false;
		}
		ModularCarSeat modularCarSeat = GetIdealMountPointFor(player) as ModularCarSeat;
		if ((Object)(object)modularCarSeat != (Object)null && !modularCarSeat.associatedSeatingModule.DoorsAreLockable)
		{
			return true;
		}
		return PlayerCanUseThis(player, ModularCarLock.LockType.Door);
	}

	public override bool IsComplete()
	{
		if (HasAnyEngines() && HasDriverMountPoints())
		{
			return !IsDead();
		}
		return false;
	}

	public void DoDecayDamage(float damage)
	{
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if (!attachedModuleEntity.IsDestroyed)
			{
				attachedModuleEntity.Hurt(damage, DamageType.Decay);
			}
		}
		if (!base.HasAnyModules)
		{
			Hurt(damage, DamageType.Decay);
		}
	}

	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetAdjustedDriveForce(absSpeed, topSpeed);
		}
		return RollOffDriveForce(num);
	}

	public bool HasAnyEngines()
	{
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			if (base.AttachedModuleEntities[i].HasAnEngine)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyWorkingEngines()
	{
		return GetMaxDriveForce() > 0f;
	}

	public bool MeetsEngineRequirements()
	{
		if (HasAnyWorkingEngines())
		{
			return HasDriver();
		}
		return false;
	}

	public void OnEngineStartFailed()
	{
		bool arg = !HasAnyWorkingEngines() || engineController.IsWaterlogged();
		ClientRPC(null, "EngineStartFailed", arg);
	}

	public bool AdminFixUp(int tier)
	{
		if (IsDead())
		{
			return false;
		}
		engineController.FuelSystem.AdminFillFuel();
		SetHealth(MaxHealth());
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			attachedModuleEntity.AdminFixUp(tier);
		}
		SendNetworkUpdate();
		return true;
	}

	public override void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead())
		{
			if (TimeSince.op_Implicit(timeSinceDeath) > 1f)
			{
				for (int i = 0; i < info.damageTypes.types.Length; i++)
				{
					deathDamageCounter += info.damageTypes.types[i];
				}
			}
			if (deathDamageCounter > 600f && !base.IsDestroyed)
			{
				Kill(DestroyMode.Gib);
			}
		}
		else if (hurtModule.PropagateDamage && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			PropagateDamageToModules(info, 0.15f, 0.4f, hurtModule);
		}
	}

	private void PropagateDamageToModules(HitInfo info, float minPropagationPercent, float maxPropagationPercent, BaseVehicleModule ignoreModule)
	{
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if ((Object)(object)attachedModuleEntity == (Object)(object)ignoreModule || attachedModuleEntity.Health() <= 0f)
			{
				continue;
			}
			if (IsDead())
			{
				break;
			}
			float num = Random.Range(minPropagationPercent, maxPropagationPercent);
			for (int i = 0; i < info.damageTypes.types.Length; i++)
			{
				float num2 = info.damageTypes.types[i];
				if (num2 > 0f)
				{
					attachedModuleEntity.AcceptPropagatedDamage(num2 * num, (DamageType)i, info.Initiator, info.UseProtection);
				}
				if (IsDead())
				{
					break;
				}
			}
		}
	}

	public override void ModuleReachedZeroHealth()
	{
		if (IsDead())
		{
			return;
		}
		bool flag = true;
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if (attachedModuleEntity.health > 0f)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			Die();
		}
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		DismountAllPlayers();
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			attachedModuleEntity.repair.enabled = false;
		}
		if (carLock != null)
		{
			carLock.RemoveLock();
		}
		timeSinceDeath = TimeSince.op_Implicit(0f);
		if (vehicle.carwrecks)
		{
			if (!base.HasAnyModules)
			{
				Kill(DestroyMode.Gib);
			}
			else
			{
				SendNetworkUpdate();
			}
		}
		else
		{
			Kill(DestroyMode.Gib);
		}
	}

	public void RemoveLock()
	{
		carLock.RemoveLock();
	}

	public void RestoreVelocity(Vector3 vel)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = rigidBody.get_velocity();
		if (((Vector3)(ref velocity)).get_sqrMagnitude() < ((Vector3)(ref vel)).get_sqrMagnitude())
		{
			vel.y = rigidBody.get_velocity().y;
			rigidBody.set_velocity(vel);
		}
	}

	protected override Vector3 GetCOMMultiplier()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (carPhysics == null || !carPhysics.IsGrounded() || !IsOn())
		{
			return airbourneCOMMultiplier;
		}
		return groundedCOMMultiplier;
	}

	private void UpdateClients()
	{
		if (HasDriver())
		{
			byte num = (byte)((GetThrottleInput() + 1f) * 7f);
			byte b = (byte)(GetBrakeInput() * 15f);
			byte arg = (byte)(num + (b << 4));
			byte arg2 = (byte)(GetFuelFraction() * 255f);
			ClientRPC(null, "ModularCarUpdate", SteerAngle, arg, DriveWheelVelocity, arg2);
		}
	}

	private void DecayTick()
	{
		if (base.IsDestroyed || IsOn() || immuneToDecay || Time.get_time() < lastEngineOnTime + 600f)
		{
			return;
		}
		float num = 1f;
		if (IsDead())
		{
			int num2 = Mathf.Max(1, base.AttachedModuleEntities.Count);
			num /= 5f * (float)num2;
			DoDecayDamage(600f * num);
			return;
		}
		num /= outsidedecayminutes;
		if (!IsOutside())
		{
			num *= 0.1f;
		}
		float num3 = (base.HasAnyModules ? Enumerable.Max<BaseVehicleModule>((IEnumerable<BaseVehicleModule>)base.AttachedModuleEntities, (Func<BaseVehicleModule, float>)((BaseVehicleModule module) => module.MaxHealth())) : MaxHealth());
		DoDecayDamage(num3 * num);
	}

	private void ProcessCollision(Collision collision)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient || collision == null || (Object)(object)collision.get_gameObject() == (Object)null || (Object)(object)collision.get_gameObject() == (Object)null)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		BaseEntity baseEntity = null;
		if ((Object)(object)((ContactPoint)(ref contact)).get_otherCollider().get_attachedRigidbody() == (Object)(object)rigidBody)
		{
			baseEntity = ((ContactPoint)(ref contact)).get_otherCollider().ToBaseEntity();
		}
		else if ((Object)(object)((ContactPoint)(ref contact)).get_thisCollider().get_attachedRigidbody() == (Object)(object)rigidBody)
		{
			baseEntity = ((ContactPoint)(ref contact)).get_thisCollider().ToBaseEntity();
		}
		if ((Object)(object)baseEntity != (Object)null)
		{
			Vector3 impulse = collision.get_impulse();
			float forceMagnitude = ((Vector3)(ref impulse)).get_magnitude() / Time.get_fixedDeltaTime();
			if (QueueCollisionDamage(baseEntity, forceMagnitude) > 0f)
			{
				ShowCollisionFX(collision);
			}
		}
	}

	private float QueueCollisionDamage(BaseEntity hitEntity, float forceMagnitude)
	{
		float num = Mathf.InverseLerp(20000f, 2500000f, forceMagnitude);
		if (num > 0f)
		{
			float num2 = Mathf.Lerp(1f, 200f, num);
			if (damageSinceLastTick.TryGetValue(hitEntity, out var value))
			{
				if (value < num2)
				{
					damageSinceLastTick[hitEntity] = num2;
				}
			}
			else
			{
				damageSinceLastTick[hitEntity] = num2;
			}
		}
		return num;
	}

	private void DoCollisionDamage(BaseEntity hitEntity, float damage)
	{
		BaseVehicleModule baseVehicleModule;
		if ((baseVehicleModule = hitEntity as BaseVehicleModule) != null)
		{
			baseVehicleModule.Hurt(damage, DamageType.Collision, this, useProtection: false);
		}
		else
		{
			if (!((Object)(object)hitEntity == (Object)(object)this))
			{
				return;
			}
			if (!base.HasAnyModules)
			{
				Hurt(damage, DamageType.Collision, this, useProtection: false);
				return;
			}
			float amount = damage / (float)base.NumAttachedModules;
			foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
			{
				attachedModuleEntity.AcceptPropagatedDamage(amount, DamageType.Collision, this, useProtection: false);
			}
		}
	}

	private void ShowCollisionFX(Collision collision)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.get_time() < nextCollisionFXTime))
		{
			nextCollisionFXTime = Time.get_time() + 0.25f;
			if (collisionEffect.isValid)
			{
				ContactPoint contact = collision.GetContact(0);
				Vector3 point = ((ContactPoint)(ref contact)).get_point();
				point += (((Component)this).get_transform().get_position() - point) * 0.25f;
				Effect.server.Run(collisionEffect.resourcePath, point, ((Component)this).get_transform().get_up());
			}
		}
	}

	private void SpawnPreassignedModules()
	{
		if (!spawnSettings.useSpawnSettings || spawnSettings.configurationOptions.IsNullOrEmpty())
		{
			return;
		}
		ModularCarPresetConfig modularCarPresetConfig = spawnSettings.configurationOptions[Random.Range(0, spawnSettings.configurationOptions.Length)];
		for (int i = 0; i < modularCarPresetConfig.socketItemDefs.Length; i++)
		{
			ItemModVehicleModule itemModVehicleModule = modularCarPresetConfig.socketItemDefs[i];
			if ((Object)(object)itemModVehicleModule != (Object)null && base.Inventory.SocketsAreFree(i, itemModVehicleModule.socketsTaken))
			{
				itemModVehicleModule.doNonUserSpawn = true;
				Item item = ItemManager.Create(((Component)itemModVehicleModule).GetComponent<ItemDefinition>(), 1, 0uL);
				float num = Random.Range(spawnSettings.minStartHealthPercent, spawnSettings.maxStartHealthPercent);
				item.condition = item.maxCondition * num;
				if (!TryAddModule(item))
				{
					item.Remove();
				}
			}
		}
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			engineController.FuelSystem.LootFuel(player);
		}
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			VehicleModuleSeating vehicleModuleSeating;
			if (attachedModuleEntity.HasSeating && (vehicleModuleSeating = attachedModuleEntity as VehicleModuleSeating) != null && vehicleModuleSeating.IsOnThisModule(player))
			{
				attachedModuleEntity.ScaleDamageForPlayer(player, info);
			}
		}
	}

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		damageShowingRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
	}

	public override void InitShared()
	{
		base.InitShared();
		engineController = new VehicleEngineController<ModularCar>(this, base.isServer, carSettings.engineStartupTime, fuelStoragePrefab, waterSample);
		carLock = new ModularCarLock(this, base.isServer);
	}

	public override float MaxHealth()
	{
		return AssociatedItemDef.condition.max;
	}

	public override float StartHealth()
	{
		return AssociatedItemDef.condition.max;
	}

	public float TotalHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].Health();
		}
		return num;
	}

	public float TotalMaxHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].MaxHealth();
		}
		return num;
	}

	public override float GetMaxForwardSpeed()
	{
		float num = GetMaxDriveForce() / base.TotalMass * 30f;
		return Mathf.Pow(0.9945f, num) * num;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.modularCar != null)
		{
			carLock.LockID = info.msg.modularCar.lockID;
			engineController.FuelSystem.fuelStorageInstance.uid = info.msg.modularCar.fuelStorageID;
			cachedFuelFraction = info.msg.modularCar.fuelFraction;
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old != next)
		{
			RefreshEngineState();
		}
	}

	public float GetThrottleInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<DriverSeatInputs> values = driverSeatInputs.get_Values();
			for (int i = 0; i < values.get_Count(); i++)
			{
				num += values.get_Item(i).throttleInput;
			}
			return Mathf.Clamp(num, -1f, 1f);
		}
		return 0f;
	}

	public float GetBrakeInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<DriverSeatInputs> values = driverSeatInputs.get_Values();
			for (int i = 0; i < values.get_Count(); i++)
			{
				num += values.get_Item(i).brakeInput;
			}
			return Mathf.Clamp01(num);
		}
		return 0f;
	}

	public float GetMaxDriveForce()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetMaxDriveForce();
		}
		return RollOffDriveForce(num);
	}

	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return engineController.FuelSystem.GetFuelFraction();
		}
		return cachedFuelFraction;
	}

	public bool PlayerHasUnlockPermission(BasePlayer player)
	{
		return carLock.PlayerHasUnlockPermission(player);
	}

	public override bool PlayerCanUseThis(BasePlayer player, ModularCarLock.LockType lockType)
	{
		return carLock.PlayerCanUseThis(player, lockType);
	}

	public bool PlayerCanDestroyLock(BasePlayer player, BaseVehicleModule viaModule)
	{
		return carLock.PlayerCanDestroyLock(viaModule);
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (!PlayerCanUseThis(player, ModularCarLock.LockType.General))
		{
			return false;
		}
		return !IsOn();
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		if (!base.CanPushNow(pusher))
		{
			return false;
		}
		if (pusher.isMounted || pusher.IsSwimming())
		{
			return false;
		}
		if (pusher.InSafeZone() && !carLock.PlayerHasUnlockPermission(pusher))
		{
			return false;
		}
		return !pusher.IsStandingOnEntity(this, 8192);
	}

	protected bool RefreshEngineState()
	{
		if (lastSetEngineState == CurEngineState)
		{
			return false;
		}
		if (base.isServer && CurEngineState == VehicleEngineController<ModularCar>.EngineState.Off)
		{
			lastEngineOnTime = Time.get_time();
		}
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			attachedModuleEntity.OnEngineStateChanged(lastSetEngineState, CurEngineState);
		}
		lastSetEngineState = CurEngineState;
		return true;
	}

	private float RollOffDriveForce(float driveForce)
	{
		return Mathf.Pow(0.9999175f, driveForce) * driveForce;
	}

	private void RefreshChassisProtectionState()
	{
		if (base.HasAnyModules)
		{
			baseProtection = immortalProtection;
			if (base.isServer)
			{
				SetHealth(MaxHealth());
			}
		}
		else
		{
			baseProtection = mortalProtection;
		}
	}

	protected override void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		base.ModuleEntityAdded(addedModule);
		RefreshChassisProtectionState();
	}

	protected override void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		base.ModuleEntityRemoved(removedModule);
		RefreshChassisProtectionState();
	}

	void IEngineControllerUser.Invoke(Action action, float time)
	{
		((FacepunchBehaviour)this).Invoke(action, time);
	}

	void IEngineControllerUser.CancelInvoke(Action action)
	{
		((FacepunchBehaviour)this).CancelInvoke(action);
	}
}
