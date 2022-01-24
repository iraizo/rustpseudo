using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;

public class TrainEngine : BaseTrain, IEngineControllerUser, IEntity
{
	public enum EngineSpeeds
	{
		Rev_Hi,
		Rev_Med,
		Rev_Lo,
		Zero,
		Fwd_Lo,
		Fwd_Med,
		Fwd_Hi
	}

	private float buttonHoldTime;

	public const float HAZARD_CHECK_EVERY = 1f;

	public const float HAZARD_DIST_MAX = 325f;

	public const float HAZARD_DIST_MIN = 20f;

	public const float HAZARD_SPEED_MIN = 4.5f;

	private static readonly EngineSpeeds MaxThrottle = EngineSpeeds.Fwd_Hi;

	private static readonly EngineSpeeds MinThrottle = EngineSpeeds.Rev_Hi;

	public float decayDuration = 1200f;

	private float decayTickSpacing = 60f;

	private float lastDecayTick;

	private float decayingFor;

	private EngineDamageOverTime engineDamage;

	private Vector3 spawnOrigin;

	private Vector3 engineLocalOffset;

	[Header("Train Engine")]
	[SerializeField]
	private Transform leftHandLever;

	[SerializeField]
	private Transform rightHandLever;

	[SerializeField]
	private Transform leftHandGrip;

	[SerializeField]
	private Transform rightHandGrip;

	[SerializeField]
	private Canvas monitorCanvas;

	[SerializeField]
	private RustText monitorText;

	[SerializeField]
	private float engineForce = 50000f;

	[SerializeField]
	private float maxSpeed = 12f;

	[SerializeField]
	private float engineStartupTime = 1f;

	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	[SerializeField]
	private float idleFuelPerSec = 0.05f;

	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	[SerializeField]
	private GameObject detailedCabinColliderObj;

	[SerializeField]
	private TriggerParent platformParentTrigger;

	[SerializeField]
	private ProtectionProperties driverProtection;

	[SerializeField]
	private float wheelRadius = 0.615f;

	[SerializeField]
	private Transform[] wheelVisuals;

	[SerializeField]
	private VehicleLight[] lights;

	[SerializeField]
	private ParticleSystemContainer fxLightDamage;

	[SerializeField]
	private ParticleSystemContainer fxMediumDamage;

	[SerializeField]
	private ParticleSystemContainer fxHeavyDamage;

	[SerializeField]
	private ParticleSystemContainer fxEngineTrouble;

	[SerializeField]
	private BoxCollider engineWorldCol;

	[SerializeField]
	private GameObjectRef fxFinalExplosion;

	[SerializeField]
	private float engineDamageToSlow = 150f;

	[SerializeField]
	private float engineDamageTimeframe = 10f;

	[SerializeField]
	private float engineSlowedTime = 10f;

	[SerializeField]
	private float engineSlowedMaxVel = 4f;

	[SerializeField]
	private TrainEngineAudio trainAudio;

	public const Flags Flag_HazardAhead = Flags.Reserved6;

	public const Flags Flag_AltColor = Flags.Reserved9;

	public const Flags Flag_EngineSlowed = Flags.Reserved10;

	private VehicleEngineController<TrainEngine> engineController;

	public bool LightsAreOn => HasFlag(Flags.Reserved5);

	public bool CloseToHazard => HasFlag(Flags.Reserved6);

	public bool EngineIsSlowed => HasFlag(Flags.Reserved10);

	public EngineSpeeds CurThrottleSetting { get; private set; } = EngineSpeeds.Zero;


	public bool IsMovingOrOn
	{
		get
		{
			if (!IsMoving())
			{
				return IsOn();
			}
			return true;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TrainEngine.OnRpcMessage", 0);
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

	public override void ServerInit()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayTick, Random.Range(20f, 40f), decayTickSpacing, decayTickSpacing * 0.1f);
		spawnOrigin = ((Component)this).get_transform().get_position();
		engineDamage = new EngineDamageOverTime(engineDamageToSlow, engineDamageTimeframe, OnEngineTookHeavyDamage);
		engineLocalOffset = ((Component)this).get_transform().InverseTransformPoint(((Component)engineWorldCol).get_transform().get_position() + ((Component)engineWorldCol).get_transform().get_rotation() * engineWorldCol.get_center());
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && isSpawned)
		{
			GetFuelSystem().CheckNewChild(child);
		}
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		engineController.CheckEngineState();
		if (engineController.IsOn)
		{
			float fuelPerSecond = Mathf.Lerp(idleFuelPerSec, maxFuelPerSec, Mathf.Abs(GetThrottleFraction()));
			if (engineController.TickFuel(fuelPerSecond) > 0)
			{
				ClientRPC(null, "SetFuelAmount", GetFuelAmount());
			}
		}
		else if (LightsAreOn && !HasDriver())
		{
			SetFlag(Flags.Reserved5, b: false);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.trainEngine = Pool.Get<TrainEngine>();
		info.msg.trainEngine.throttleSetting = (int)CurThrottleSetting;
		info.msg.trainEngine.fuelStorageID = engineController.FuelSystem.fuelStorageInstance.uid;
		info.msg.trainEngine.fuelAmount = GetFuelAmount();
	}

	public override EntityFuelSystem GetFuelSystem()
	{
		return engineController.FuelSystem;
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.OnKilled(info);
		if (base.IsDestroyed)
		{
			Effect.server.Run(fxFinalExplosion.resourcePath, ((Component)engineWorldCol).get_transform().get_position() + engineWorldCol.get_center(), Vector3.get_up(), null, broadcast: true);
		}
	}

	public override void LightToggle(BasePlayer player)
	{
		if (IsDriver(player))
		{
			SetFlag(Flags.Reserved5, !LightsAreOn);
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (!IsDriver(player))
		{
			return;
		}
		if (engineController.IsOff)
		{
			if ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD)))
			{
				engineController.TryStartEngine(player);
			}
		}
		else if (!ProcessThrottleInput(BUTTON.FORWARD, IncreaseThrottle))
		{
			ProcessThrottleInput(BUTTON.BACKWARD, DecreaseThrottle);
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			SetTrackSelection(TrainTrackSpline.TrackSelection.Left);
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			SetTrackSelection(TrainTrackSpline.TrackSelection.Right);
		}
		else
		{
			SetTrackSelection(TrainTrackSpline.TrackSelection.Default);
		}
		bool ProcessThrottleInput(BUTTON button, Action action)
		{
			if (inputState.IsDown(button))
			{
				if (!inputState.WasDown(button))
				{
					action();
					buttonHoldTime = 0f;
				}
				else
				{
					buttonHoldTime += player.clientTickInterval;
					if (buttonHoldTime > 0.55f)
					{
						action();
						buttonHoldTime = 0.4f;
					}
				}
				return true;
			}
			return false;
		}
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		driverProtection.Scale(info.damageTypes);
	}

	public bool MeetsEngineRequirements()
	{
		if (!HasDriver() && CurThrottleSetting == EngineSpeeds.Zero)
		{
			return false;
		}
		return AnyPlayersOnTrain();
	}

	public void OnEngineStartFailed()
	{
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (CanMount(player))
		{
			base.AttemptMount(player, doMountChecks);
		}
	}

	public override float GetEngineForces()
	{
		if (IsDead() || base.IsDestroyed)
		{
			return 0f;
		}
		float num = (engineController.IsOn ? GetThrottleFraction() : 0f);
		float num2 = maxSpeed * num;
		float curTopSpeed = GetCurTopSpeed();
		num2 = Mathf.Clamp(num2, 0f - curTopSpeed, curTopSpeed);
		if (num > 0f && base.TrackSpeed < num2)
		{
			return GetCurEngineForce();
		}
		if (num < 0f && base.TrackSpeed > num2)
		{
			return 0f - GetCurEngineForce();
		}
		return 0f;
	}

	public override void Hurt(HitInfo info)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (engineDamage != null && Vector3.SqrMagnitude(engineLocalOffset - info.HitPositionLocal) < 2f)
		{
			engineDamage.TakeDamage(info.damageTypes.Total());
		}
		base.Hurt(info);
	}

	private void IncreaseThrottle()
	{
		if (CurThrottleSetting != MaxThrottle)
		{
			SetThrottle(CurThrottleSetting + 1);
		}
	}

	private void DecreaseThrottle()
	{
		if (CurThrottleSetting != MinThrottle)
		{
			SetThrottle(CurThrottleSetting - 1);
		}
	}

	private void SetZeroThrottle()
	{
		SetThrottle(EngineSpeeds.Zero);
	}

	private void ServerFlagsChanged(Flags old, Flags next)
	{
		if (next.HasFlag(Flags.On) && !old.HasFlag(Flags.On))
		{
			SetFlag(Flags.Reserved5, b: true);
			((FacepunchBehaviour)this).InvokeRandomized((Action)CheckForHazards, 0f, 1f, 0.1f);
		}
		else if (!next.HasFlag(Flags.On) && old.HasFlag(Flags.On))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)CheckForHazards);
			SetFlag(Flags.Reserved6, b: false);
		}
	}

	private bool AnyPlayersOnTrain()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (AnyMounted())
		{
			return true;
		}
		if (platformParentTrigger.HasAnyEntityContents)
		{
			Enumerator<BaseEntity> enumerator = platformParentTrigger.entityContents.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if ((Object)(object)enumerator.get_Current().ToPlayer() != (Object)null)
					{
						return true;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		return false;
	}

	private bool AnyPlayersNearby(float maxDist)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities(((Component)this).get_transform().get_position(), maxDist, list, 131072, (QueryTriggerInteraction)2);
		bool result = false;
		foreach (BasePlayer item in list)
		{
			if (!item.IsSleeping() && item.IsAlive())
			{
				result = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return result;
	}

	private void CheckForHazards()
	{
		if (base.TrackSpeed > 4.5f || base.TrackSpeed < -4.5f)
		{
			float maxHazardDist = Mathf.Lerp(40f, 325f, Mathf.Abs(base.TrackSpeed) * 0.05f);
			SetFlag(Flags.Reserved6, base.FrontTrackSection.HasValidHazardWithin(this, base.FrontWheelSplineDist, 20f, maxHazardDist, curTrackSelection, base.RearTrackSection));
		}
		else
		{
			SetFlag(Flags.Reserved6, b: false);
		}
	}

	private void OnEngineTookHeavyDamage()
	{
		SetFlag(Flags.Reserved10, b: true);
		((FacepunchBehaviour)this).Invoke((Action)ResetEngineToNormal, engineSlowedTime);
	}

	private void ResetEngineToNormal()
	{
		SetFlag(Flags.Reserved10, b: false);
	}

	private float GetCurTopSpeed()
	{
		float num = maxSpeed * GetEnginePowerMultiplier(0.5f);
		if (EngineIsSlowed)
		{
			num = Mathf.Clamp(num, 0f - engineSlowedMaxVel, engineSlowedMaxVel);
		}
		return num;
	}

	private float GetCurEngineForce()
	{
		return engineForce * GetEnginePowerMultiplier(0.75f);
	}

	private void DecayTick()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		bool flag = HasDriver() || AnyPlayersOnTrain();
		bool num = base.IsAtAStation && Vector3.Distance(spawnOrigin, ((Component)this).get_transform().get_position()) < 50f;
		if (flag)
		{
			decayingFor = 0f;
		}
		bool num2 = !num && !flag && !AnyPlayersNearby(30f);
		float realtimeSinceStartup = Time.get_realtimeSinceStartup();
		float num3 = realtimeSinceStartup - lastDecayTick;
		lastDecayTick = realtimeSinceStartup;
		if (num2)
		{
			decayingFor += num3;
			if (decayingFor >= decayDuration)
			{
				ActualDeath();
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

	public override void InitShared()
	{
		base.InitShared();
		engineController = new VehicleEngineController<TrainEngine>(this, base.isServer, engineStartupTime, fuelStoragePrefab);
		if (base.isServer)
		{
			bool b = SeedRandom.Range(net.ID, 0, 2) == 0;
			SetFlag(Flags.Reserved9, b);
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.trainEngine != null)
		{
			engineController.FuelSystem.fuelStorageInstance.uid = info.msg.trainEngine.fuelStorageID;
			SetThrottle((EngineSpeeds)info.msg.trainEngine.throttleSetting);
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old != next && base.isServer)
		{
			ServerFlagsChanged(old, next);
		}
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (player.isMounted)
		{
			return false;
		}
		if (!PlayerIsInParentTrigger(player))
		{
			return false;
		}
		return true;
	}

	private float GetEnginePowerMultiplier(float minPercent)
	{
		if (base.healthFraction > 0.4f)
		{
			return 1f;
		}
		return Mathf.Lerp(minPercent, 1f, base.healthFraction / 0.4f);
	}

	public float GetThrottleFraction()
	{
		return CurThrottleSetting switch
		{
			EngineSpeeds.Rev_Hi => -1f, 
			EngineSpeeds.Rev_Med => -0.5f, 
			EngineSpeeds.Rev_Lo => -0.2f, 
			EngineSpeeds.Zero => 0f, 
			EngineSpeeds.Fwd_Lo => 0.2f, 
			EngineSpeeds.Fwd_Med => 0.5f, 
			EngineSpeeds.Fwd_Hi => 1f, 
			_ => 0f, 
		};
	}

	public bool IsNearDesiredSpeed(float leeway)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(((Component)this).get_transform().get_forward(), GetLocalVelocity());
		float num2 = maxSpeed * GetThrottleFraction();
		if (num2 < 0f)
		{
			return num - leeway <= num2;
		}
		return num + leeway >= num2;
	}

	protected override void SetTrackSelection(TrainTrackSpline.TrackSelection trackSelection)
	{
		base.SetTrackSelection(trackSelection);
	}

	private void SetThrottle(EngineSpeeds throttle)
	{
		if (CurThrottleSetting != throttle)
		{
			CurThrottleSetting = throttle;
			if (base.isServer)
			{
				ClientRPC(null, "SetThrottle", (sbyte)throttle);
			}
		}
	}

	private int GetFuelAmount()
	{
		if (base.isServer)
		{
			return engineController.FuelSystem.GetFuelAmount();
		}
		return 0;
	}

	private bool CanMount(BasePlayer player)
	{
		return PlayerIsInParentTrigger(player);
	}

	private bool PlayerIsInParentTrigger(BasePlayer player)
	{
		return (Object)(object)player.GetParentEntity() == (Object)(object)this;
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
