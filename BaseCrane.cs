using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseCrane : BaseVehicle, TriggerHurtNotChild.IHurtTriggerUser
{
	public float extensionArmState;

	public float raiseArmState;

	public float yawState = 5f;

	public Transform COM;

	public float extensionDirection;

	public float yawDirection;

	public float raiseArmDirection;

	public float arm1Speed = 0.01f;

	public float arm2Speed = 0.01f;

	public float turnYawSpeed = 0.01f;

	public Animator animator;

	public BaseMagnet Magnet;

	public Rigidbody myRigidbody;

	public WheelCollider[] leftWheels;

	public WheelCollider[] rightWheels;

	public float brakeStrength = 1000f;

	public float engineStrength = 1000f;

	public Transform[] collisionTestingPoints;

	public float maxDistanceFromOrigin;

	public GameObjectRef selfDamageEffect;

	public GameObjectRef explosionEffect;

	public Transform explosionPoint;

	public CapsuleCollider driverCollision;

	public Transform leftHandTarget;

	public Transform rightHandTarget;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public float fuelPerSec;

	protected EntityFuelSystem fuelSystem;

	public GameObject[] OnTriggers;

	public TriggerHurtEx magnetDamage;

	public static readonly Phrase ReturnMessage = new Phrase("junkyardcrane.return", "Return to the Junkyard. Excessive damage will occur.");

	private Vector3 spawnOrigin = Vector3.get_zero();

	public float nextInputTime;

	private float nextToggleTime;

	public float turnAmount;

	public float throttle;

	private float lastExtensionArmState;

	private float lastRaiseArmState;

	private float lastYawState;

	private bool handbrakeOn = true;

	private float nextSelfHealTime;

	private Vector3 lastDamagePos = Vector3.get_zero();

	private float lastDrivenTime;

	private float testPreviousYaw = 5f;

	public float GetPlayerDamageMultiplier()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localVelocity = GetLocalVelocity();
		return Mathf.Abs(((Vector3)(ref localVelocity)).get_magnitude()) * 60f;
	}

	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	public override void ServerInit()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateParams, 0f, 0.1f);
		animator.set_cullingMode((AnimatorCullingMode)0);
		animator.set_updateMode((AnimatorUpdateMode)1);
		myRigidbody.set_centerOfMass(COM.get_localPosition());
		SetMagnetEnabled(wantsOn: false);
		spawnOrigin = ((Component)this).get_transform().get_position();
		lastDrivenTime = Time.get_realtimeSinceStartup();
		GameObject[] onTriggers = OnTriggers;
		for (int i = 0; i < onTriggers.Length; i++)
		{
			onTriggers[i].SetActive(false);
		}
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && isSpawned)
		{
			fuelSystem.CheckNewChild(child);
		}
	}

	public void SetMagnetEnabled(bool wantsOn)
	{
		Magnet.SetMagnetEnabled(wantsOn);
		SetFlag(Flags.Reserved6, wantsOn);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (!EngineOn())
		{
			return;
		}
		bool num = inputState.IsDown(BUTTON.SPRINT);
		if (inputState.IsDown(BUTTON.RELOAD) && Time.get_realtimeSinceStartup() > nextToggleTime)
		{
			SetMagnetEnabled(!Magnet.IsMagnetOn());
			nextToggleTime = Time.get_realtimeSinceStartup() + 0.5f;
		}
		throttle = 0f;
		turnAmount = 0f;
		if (num)
		{
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				throttle = 1f;
			}
			if (inputState.IsDown(BUTTON.BACKWARD))
			{
				throttle = -1f;
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				turnAmount = -1f;
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				turnAmount = 1f;
			}
		}
		else if (Time.get_realtimeSinceStartup() >= nextInputTime)
		{
			if (inputState.IsDown(BUTTON.FIRE_PRIMARY))
			{
				extensionDirection = 1f;
			}
			if (inputState.IsDown(BUTTON.FIRE_SECONDARY))
			{
				extensionDirection = -1f;
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				yawDirection = -1f;
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				yawDirection = 1f;
			}
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				raiseArmDirection = 1f;
			}
			if (inputState.IsDown(BUTTON.BACKWARD))
			{
				raiseArmDirection = -1f;
			}
		}
		handbrakeOn = throttle == 0f && turnAmount == 0f;
	}

	public bool EngineOn()
	{
		return IsOn();
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		bool flag = EngineOn();
		if (EngineOn())
		{
			fuelSystem.TryUseFuel(Time.get_fixedDeltaTime(), fuelPerSec);
		}
		SetFlag(Flags.On, HasDriver() && GetFuelSystem().HasFuel());
		if (IsOn() != flag)
		{
			GameObject[] onTriggers = OnTriggers;
			for (int i = 0; i < onTriggers.Length; i++)
			{
				onTriggers[i].SetActive(IsOn());
			}
		}
		if (Vector3.Dot(((Component)this).get_transform().get_up(), Vector3.get_down()) >= 0.4f)
		{
			Kill(DestroyMode.Gib);
			return;
		}
		if (Time.get_realtimeSinceStartup() > lastDrivenTime + 14400f)
		{
			Kill(DestroyMode.Gib);
			return;
		}
		if (spawnOrigin != Vector3.get_zero() && maxDistanceFromOrigin != 0f)
		{
			if (Vector3Ex.Distance2D(((Component)this).get_transform().get_position(), spawnOrigin) > maxDistanceFromOrigin)
			{
				if (Vector3Ex.Distance2D(((Component)this).get_transform().get_position(), lastDamagePos) > 6f)
				{
					if ((Object)(object)GetDriver() != (Object)null)
					{
						GetDriver().ShowToast(1, ReturnMessage);
					}
					Hurt(MaxHealth() * 0.15f, DamageType.Generic, this, useProtection: false);
					lastDamagePos = ((Component)this).get_transform().get_position();
					nextSelfHealTime = Time.get_realtimeSinceStartup() + 3600f;
					Effect.server.Run(selfDamageEffect.resourcePath, ((Component)this).get_transform().get_position() + Vector3.get_up() * 2f, Vector3.get_up());
					return;
				}
			}
			else if (base.healthFraction < 1f && Time.get_realtimeSinceStartup() > nextSelfHealTime && base.SecondsSinceAttacked > 600f)
			{
				Heal(1000f);
			}
		}
		if (!HasDriver() || !EngineOn())
		{
			handbrakeOn = true;
			throttle = 0f;
			turnAmount = 0f;
			SetFlag(Flags.Reserved10, b: false);
			SetFlag(Flags.Reserved5, b: false);
			SetMagnetEnabled(wantsOn: false);
		}
		else
		{
			lastDrivenTime = Time.get_realtimeSinceStartup();
			if (Magnet.IsMagnetOn() && Magnet.HasConnectedObject() && GamePhysics.CheckOBB(Magnet.GetConnectedOBB(0.75f), 1084293121, (QueryTriggerInteraction)1))
			{
				SetMagnetEnabled(wantsOn: false);
				nextToggleTime = Time.get_realtimeSinceStartup() + 2f;
				Effect.server.Run(selfDamageEffect.resourcePath, ((Component)Magnet).get_transform().get_position(), Vector3.get_up());
			}
		}
		extensionDirection = Mathf.MoveTowards(extensionDirection, 0f, Time.get_fixedDeltaTime() * 3f);
		yawDirection = Mathf.MoveTowards(yawDirection, 0f, Time.get_fixedDeltaTime() * 3f);
		raiseArmDirection = Mathf.MoveTowards(raiseArmDirection, 0f, Time.get_fixedDeltaTime() * 3f);
		bool flag2 = extensionDirection != 0f || raiseArmDirection != 0f || yawDirection != 0f;
		SetFlag(Flags.Reserved7, flag2);
		magnetDamage.damageEnabled = IsOn() && flag2;
		extensionArmState += extensionDirection * arm1Speed * Time.get_fixedDeltaTime();
		raiseArmState += raiseArmDirection * arm2Speed * Time.get_fixedDeltaTime();
		yawState += yawDirection * turnYawSpeed * Time.get_fixedDeltaTime();
		extensionArmState = Mathf.Clamp(extensionArmState, -1f, 1f);
		raiseArmState = Mathf.Clamp(raiseArmState, -1f, 1f);
		UpdateAnimator(shouldLerp: false);
		Magnet.MagnetThink(Time.get_fixedDeltaTime());
		float num = throttle;
		float num2 = throttle;
		if (turnAmount == 1f)
		{
			num = -1f;
			num2 = 1f;
		}
		else if (turnAmount == -1f)
		{
			num = 1f;
			num2 = -1f;
		}
		UpdateMotorSpeed(num * engineStrength, num2 * engineStrength, handbrakeOn ? brakeStrength : 0f);
	}

	public void UpdateMotorSpeed(float speedLeft, float speedRight, float brakeSpeed)
	{
		WheelCollider[] array = leftWheels;
		foreach (WheelCollider obj in array)
		{
			obj.set_motorTorque(speedLeft);
			obj.set_brakeTorque(brakeSpeed);
		}
		array = rightWheels;
		foreach (WheelCollider obj2 in array)
		{
			obj2.set_motorTorque(speedRight);
			obj2.set_brakeTorque(brakeSpeed);
		}
		SetFlag(Flags.Reserved10, speedLeft != 0f && speedRight != 0f);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.crane = Pool.Get<Crane>();
		info.msg.crane.arm1 = extensionArmState;
		info.msg.crane.arm2 = raiseArmState;
		info.msg.crane.yaw = yawState;
	}

	public void UpdateParams()
	{
		SendNetworkUpdate();
	}

	public void LateUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (HasDriver() && DidCollide())
		{
			if (Time.get_realtimeSinceStartup() > nextInputTime)
			{
				nextInputTime = Time.get_realtimeSinceStartup() + 0.5f;
				extensionArmState = lastExtensionArmState;
				raiseArmState = lastRaiseArmState;
				yawState = lastYawState;
				extensionDirection = 0f - extensionDirection;
				yawDirection = 0f - yawDirection;
				raiseArmDirection = 0f - raiseArmDirection;
			}
			UpdateAnimator(shouldLerp: false);
		}
		else
		{
			lastExtensionArmState = extensionArmState;
			lastRaiseArmState = raiseArmState;
			lastYawState = yawState;
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			BasePlayer driver = GetDriver();
			if ((Object)(object)driver != (Object)null && info.damageTypes.Has(DamageType.Bullet))
			{
				Capsule val = default(Capsule);
				((Capsule)(ref val))._002Ector(((Component)driverCollision).get_transform().get_position(), driverCollision.get_radius(), driverCollision.get_height());
				float num = Vector3.Distance(info.PointStart, info.PointEnd);
				Ray val2 = default(Ray);
				((Ray)(ref val2))._002Ector(info.PointStart, Vector3Ex.Direction(info.PointEnd, info.PointStart));
				RaycastHit val3 = default(RaycastHit);
				if (((Capsule)(ref val)).Trace(val2, ref val3, 0.05f, num * 1.2f))
				{
					driver.Hurt(info.damageTypes.Total() * 0.15f, DamageType.Bullet, info.Initiator);
				}
			}
		}
		base.OnAttacked(info);
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (HasDriver())
		{
			GetDriver().Hurt(10000f, DamageType.Blunt, info.Initiator, useProtection: false);
		}
		if (explosionEffect.isValid)
		{
			Effect.server.Run(explosionEffect.resourcePath, explosionPoint.get_position(), Vector3.get_up());
		}
		base.OnKilled(info);
	}

	public override void LightToggle(BasePlayer player)
	{
		SetFlag(Flags.Reserved5, !HasFlag(Flags.Reserved5));
	}

	public bool DidCollide()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Transform[] array = collisionTestingPoints;
		foreach (Transform val in array)
		{
			if (((Component)val).get_gameObject().get_activeSelf())
			{
				Vector3 position = val.get_position();
				Quaternion rotation = val.get_rotation();
				if (GamePhysics.CheckOBB(new OBB(position, new Vector3(val.get_localScale().x, val.get_localScale().y, val.get_localScale().z), rotation), 1084293121, (QueryTriggerInteraction)1))
				{
					return true;
				}
			}
		}
		return false;
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && (!HasDriver() || IsDriver(player)))
		{
			fuelSystem.LootFuel(player);
		}
	}

	public override EntityFuelSystem GetFuelSystem()
	{
		return fuelSystem;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.crane != null)
		{
			yawState = info.msg.crane.yaw;
			extensionArmState = info.msg.crane.arm1;
			raiseArmState = info.msg.crane.arm2;
		}
	}

	public void UpdateAnimator(bool shouldLerp = true)
	{
		float @float = animator.GetFloat("Arm_01");
		float float2 = animator.GetFloat("Arm_02");
		_ = testPreviousYaw;
		animator.SetFloat("Arm_01", shouldLerp ? Mathf.Lerp(@float, extensionArmState, Time.get_deltaTime() * 6f) : extensionArmState);
		animator.SetFloat("Arm_02", shouldLerp ? Mathf.Lerp(float2, raiseArmState, Time.get_deltaTime() * 6f) : raiseArmState);
		float num = Mathf.Lerp(testPreviousYaw, yawState, Time.get_deltaTime() * 6f);
		if (num % 1f < 0f)
		{
			num += 1f;
		}
		if (yawState % 1f < 0f)
		{
			yawState += 1f;
		}
		animator.SetFloat("Yaw", (shouldLerp ? num : yawState) % 1f);
		testPreviousYaw = num;
	}

	public override void InitShared()
	{
		base.InitShared();
		fuelSystem = new EntityFuelSystem(base.isServer, fuelStoragePrefab, children);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseCrane.OnRpcMessage", 0);
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
}
