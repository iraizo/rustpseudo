using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class MLRS : BaseMountable
{
	[Serializable]
	public class RocketTube
	{
		public Vector3 firingOffset;

		public Transform hinge;

		public Renderer rocket;
	}

	private struct TheoreticalProjectile
	{
		public Vector3 pos;

		public Vector3 forward;

		public float gravityMult;

		public TheoreticalProjectile(Vector3 pos, Vector3 forward, float gravityMult)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			this.pos = pos;
			this.forward = forward;
			this.gravityMult = gravityMult;
		}
	}

	private float leftRightInput;

	private float upDownInput;

	private Vector3 lastSentTargetHitPos;

	private Vector3 lastSentTrueHitPos;

	private int nextRocketIndex;

	private EntityRef rocketOwnerRef;

	private TimeSince timeSinceBroken;

	private int radiusModIndex;

	private float[] radiusMods = new float[4]
	{
		0.1f,
		0.2f,
		0.33333334f,
		2f / 3f
	};

	private Vector3 trueTargetHitPos;

	[Header("MLRS Components")]
	[SerializeField]
	private GameObjectRef rocketStoragePrefab;

	[SerializeField]
	private GameObjectRef dashboardStoragePrefab;

	[Header("MLRS Rotation")]
	[SerializeField]
	private Transform hRotator;

	[SerializeField]
	private float hRotSpeed = 25f;

	[SerializeField]
	private Transform vRotator;

	[SerializeField]
	private float vRotSpeed = 10f;

	[SerializeField]
	[Range(50f, 90f)]
	private float vRotMax = 85f;

	[SerializeField]
	private Transform hydraulics;

	[Header("MLRS Weaponry")]
	[Tooltip("Minimum distance from the MLRS to a targeted hit point. In metres.")]
	[SerializeField]
	public float minRange = 200f;

	[Tooltip("The size of the area that the rockets may hit, minus rocket damage radius.")]
	[SerializeField]
	public float targetAreaRadius = 30f;

	[SerializeField]
	private GameObjectRef mlrsRocket;

	[SerializeField]
	public Transform firingPoint;

	[SerializeField]
	private RocketTube[] rocketTubes;

	[Header("MLRS Dashboard/FX")]
	[SerializeField]
	private GameObject screensChild;

	[SerializeField]
	private Transform leftHandGrip;

	[SerializeField]
	private Transform leftJoystick;

	[SerializeField]
	private Transform rightHandGrip;

	[SerializeField]
	private Transform rightJoystick;

	[SerializeField]
	private Transform controlKnobHeight;

	[SerializeField]
	private Transform controlKnobAngle;

	[SerializeField]
	private GameObjectRef uiDialogPrefab;

	[SerializeField]
	private Light fireButtonLight;

	[SerializeField]
	private GameObject brokenDownEffect;

	[SerializeField]
	private ParticleSystem topScreenShutdown;

	[SerializeField]
	private ParticleSystem bottomScreenShutdown;

	[ServerVar(Help = "How many minutes before the MLRS recovers from use and can be used again")]
	public static float brokenDownMinutes = 10f;

	public const Flags FLAG_FIRING_ROCKETS = Flags.Reserved6;

	public const Flags FLAG_HAS_AIMING_MODULE = Flags.Reserved8;

	private EntityRef rocketStorageInstance;

	private EntityRef dashboardStorageInstance;

	private float rocketBaseGravity;

	private float rocketSpeed;

	private bool isInitialLoad = true;

	public Vector3 UserTargetHitPos { get; private set; }

	public Vector3 TrueHitPos { get; private set; }

	public bool HasAimingModule => HasFlag(Flags.Reserved8);

	private bool CanBeUsed
	{
		get
		{
			if (HasAimingModule)
			{
				return !IsBroken();
			}
			return false;
		}
	}

	private bool CanFire
	{
		get
		{
			if (CanBeUsed && RocketAmmoCount > 0 && !IsFiringRockets)
			{
				return !IsRealigning;
			}
			return false;
		}
	}

	private float HRotation
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return hRotator.get_eulerAngles().y;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 eulerAngles = hRotator.get_eulerAngles();
			eulerAngles.y = value;
			hRotator.set_eulerAngles(eulerAngles);
		}
	}

	private float VRotation
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return vRotator.get_localEulerAngles().x;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localEulerAngles = vRotator.get_localEulerAngles();
			if (value < 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, 0f - vRotMax, 0f);
			}
			else if (value > 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, 360f - vRotMax, 360f);
			}
			vRotator.set_localEulerAngles(localEulerAngles);
		}
	}

	public float CurGravityMultiplier { get; private set; }

	public int RocketAmmoCount { get; private set; }

	public bool IsRealigning { get; private set; }

	public bool IsFiringRockets => HasFlag(Flags.Reserved6);

	public float RocketDamageRadius { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MLRS.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 455279877 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Fire_Rockets "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Fire_Rockets", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(455279877u, "RPC_Fire_Rockets", this, player, 3f))
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
							RPC_Fire_Rockets(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Fire_Rockets");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 751446792 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Open_Dashboard "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Open_Dashboard", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(751446792u, "RPC_Open_Dashboard", this, player, 3f))
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
							RPCMessage msg3 = rPCMessage;
							RPC_Open_Dashboard(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Open_Dashboard");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1311007340 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Open_Rockets "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Open_Rockets", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1311007340u, "RPC_Open_Rockets", this, player, 3f))
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
							RPCMessage msg4 = rPCMessage;
							RPC_Open_Rockets(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Open_Rockets");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 858951307 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_SetTargetHitPos "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_SetTargetHitPos", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(858951307u, "RPC_SetTargetHitPos", this, player, 3f))
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
							RPCMessage msg5 = rPCMessage;
							RPC_SetTargetHitPos(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_SetTargetHitPos");
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

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (child.prefabID == rocketStoragePrefab.GetEntity().prefabID)
			{
				rocketStorageInstance.Set(child);
			}
			if (child.prefabID == dashboardStoragePrefab.GetEntity().prefabID)
			{
				dashboardStorageInstance.Set(child);
			}
		}
	}

	public override void VehicleFixedUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		Item item;
		if (IsBroken())
		{
			if (!(TimeSince.op_Implicit(timeSinceBroken) >= brokenDownMinutes * 60f))
			{
				SetFlag(Flags.Reserved8, TryGetAimingModule(out item));
				return;
			}
			SetRepaired();
		}
		bool b = TryGetAimingModule(out item);
		SetFlag(Flags.Reserved8, b);
		int rocketAmmoCount = RocketAmmoCount;
		RocketAmmoCount = GetRocketContainer().inventory.GetAmmoAmount((AmmoTypes)2048);
		if (CanBeUsed && IsMounted())
		{
			Vector3 userTargetHitPos = UserTargetHitPos;
			userTargetHitPos += Vector3.get_forward() * upDownInput * 75f * Time.get_fixedDeltaTime();
			userTargetHitPos += Vector3.get_right() * leftRightInput * 75f * Time.get_fixedDeltaTime();
			SetUserTargetHitPos(userTargetHitPos);
		}
		if (!IsFiringRockets)
		{
			HitPosToRotation(trueTargetHitPos, out var hRot, out var vRot, out var g);
			float num = g / (0f - Physics.get_gravity().y);
			IsRealigning = Mathf.Abs(Mathf.DeltaAngle(VRotation, vRot)) > 0.001f || Mathf.Abs(Mathf.DeltaAngle(HRotation, hRot)) > 0.001f || !Mathf.Approximately(CurGravityMultiplier, num);
			if (IsRealigning)
			{
				if (isInitialLoad)
				{
					VRotation = vRot;
					HRotation = hRot;
					isInitialLoad = false;
				}
				else
				{
					VRotation = Mathf.MoveTowardsAngle(VRotation, vRot, Time.get_deltaTime() * vRotSpeed);
					HRotation = Mathf.MoveTowardsAngle(HRotation, hRot, Time.get_deltaTime() * hRotSpeed);
				}
				CurGravityMultiplier = num;
				TrueHitPos = GetTrueHitPos();
			}
		}
		if (UserTargetHitPos != lastSentTargetHitPos || TrueHitPos != lastSentTrueHitPos || RocketAmmoCount != rocketAmmoCount)
		{
			SendNetworkUpdate();
		}
	}

	private Vector3 GetTrueHitPos()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = firingPoint.get_position();
		Vector3 forward = firingPoint.get_forward();
		TheoreticalProjectile projectile = new TheoreticalProjectile(position, ((Vector3)(ref forward)).get_normalized() * rocketSpeed, CurGravityMultiplier);
		int num = 0;
		float dt = ((projectile.forward.y > 0f) ? 2f : 0.66f);
		while (!NextRayHitSomething(ref projectile, dt) && (float)num < 128f)
		{
			num++;
		}
		return projectile.pos;
	}

	private bool NextRayHitSomething(ref TheoreticalProjectile projectile, float dt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		float num = Physics.get_gravity().y * projectile.gravityMult;
		Vector3 pos = projectile.pos;
		float num2 = Vector3Ex.MagnitudeXZ(projectile.forward) * dt;
		float num3 = projectile.forward.y * dt + num * dt * dt * 0.5f;
		Vector2 val = Vector3Ex.XZ2D(projectile.forward);
		Vector2 val2 = ((Vector2)(ref val)).get_normalized() * num2;
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(val2.x, num3, val2.y);
		ref Vector3 pos2 = ref projectile.pos;
		pos2 += val3;
		float y = projectile.forward.y + num * dt;
		projectile.forward.y = y;
		RaycastHit hit = default(RaycastHit);
		if (Physics.Linecast(pos, projectile.pos, ref hit, 1084293393, (QueryTriggerInteraction)1))
		{
			projectile.pos = ((RaycastHit)(ref hit)).get_point();
			BaseEntity entity = hit.GetEntity();
			bool num4 = (Object)(object)entity != (Object)null && entity.EqualNetID(this);
			if (num4)
			{
				ref Vector3 pos3 = ref projectile.pos;
				pos3 += projectile.forward * 1f;
			}
			return !num4;
		}
		return false;
	}

	private float GetSurfaceHeight(Vector3 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float height2 = TerrainMeta.WaterMap.GetHeight(pos);
		return Mathf.Max(height, height2);
	}

	private void SetRepaired()
	{
		SetFlag(Flags.Broken, b: false);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			upDownInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			upDownInput = -1f;
		}
		else
		{
			upDownInput = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			leftRightInput = -1f;
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			leftRightInput = 1f;
		}
		else
		{
			leftRightInput = 0f;
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.mlrs = Pool.Get<MLRS>();
		info.msg.mlrs.targetPos = UserTargetHitPos;
		info.msg.mlrs.curHitPos = TrueHitPos;
		info.msg.mlrs.rocketStorageID = rocketStorageInstance.uid;
		info.msg.mlrs.dashboardStorageID = dashboardStorageInstance.uid;
		info.msg.mlrs.ammoCount = (uint)RocketAmmoCount;
		lastSentTargetHitPos = UserTargetHitPos;
		lastSentTrueHitPos = TrueHitPos;
	}

	public bool AdminFixUp()
	{
		if (IsDead() || IsFiringRockets)
		{
			return false;
		}
		StorageContainer dashboardContainer = GetDashboardContainer();
		if (!HasAimingModule)
		{
			dashboardContainer.inventory.AddItem(ItemManager.FindItemDefinition("aiming.module.mlrs"), 1, 0uL);
		}
		StorageContainer rocketContainer = GetRocketContainer();
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("ammo.rocket.mlrs");
		if (RocketAmmoCount < rocketContainer.inventory.capacity * itemDefinition.stackable)
		{
			int num = itemDefinition.stackable * rocketContainer.inventory.capacity - RocketAmmoCount;
			while (num > 0)
			{
				int num2 = Mathf.Min(num, itemDefinition.stackable);
				rocketContainer.inventory.AddItem(itemDefinition, itemDefinition.stackable, 0uL);
				num -= num2;
			}
		}
		SetRepaired();
		SendNetworkUpdate();
		return true;
	}

	private void Fire(BasePlayer owner)
	{
		if (CanFire && !IsFiringRockets && !((Object)(object)_mounted == (Object)null))
		{
			nextRocketIndex = Mathf.Min(RocketAmmoCount - 1, rocketTubes.Length - 1);
			rocketOwnerRef.Set(owner);
			SetFlag(Flags.Reserved6, b: true);
			radiusModIndex = 0;
			((FacepunchBehaviour)this).InvokeRepeating((Action)FireNextRocket, 0f, 0.5f);
		}
	}

	private void EndFiring()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		((FacepunchBehaviour)this).CancelInvoke((Action)FireNextRocket);
		rocketOwnerRef.Set(null);
		if (TryGetAimingModule(out var item))
		{
			item.LoseCondition(1f);
		}
		SetFlag(Flags.Reserved6, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Broken, b: true, recursive: false, networkupdate: false);
		SendNetworkUpdate_Flags();
		timeSinceBroken = TimeSince.op_Implicit(0f);
	}

	private void FireNextRocket()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (nextRocketIndex < 0)
		{
			EndFiring();
			return;
		}
		StorageContainer rocketContainer = GetRocketContainer();
		Vector3 firingPos = firingPoint.get_position() + firingPoint.get_rotation() * rocketTubes[nextRocketIndex].firingOffset;
		float num = 1f;
		if (radiusModIndex < radiusMods.Length)
		{
			num = radiusMods[radiusModIndex];
		}
		radiusModIndex++;
		Vector2 val = Random.get_insideUnitCircle() * (targetAreaRadius - RocketDamageRadius) * num;
		Vector3 targetPos = TrueHitPos + new Vector3(val.x, 0f, val.y);
		float g;
		Vector3 aimToTarget = GetAimToTarget(targetPos, out g);
		if (BaseMountable.TryFireProjectile(rocketContainer, (AmmoTypes)2048, firingPos, aimToTarget, _mounted, 0f, 0f, out var projectile))
		{
			projectile.gravityModifier = g / (0f - Physics.get_gravity().y);
			nextRocketIndex--;
		}
		else
		{
			EndFiring();
		}
	}

	private bool TryGetAimingModule(out Item item)
	{
		ItemContainer inventory = GetDashboardContainer().inventory;
		if (!inventory.IsEmpty())
		{
			item = inventory.itemList[0];
			return true;
		}
		item = null;
		return false;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_SetTargetHitPos(RPCMessage msg)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (PlayerIsMounted(player))
		{
			SetUserTargetHitPos(msg.read.Vector3());
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Fire_Rockets(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (PlayerIsMounted(player))
		{
			Fire(player);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Rockets(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			IItemContainerEntity rocketContainer = GetRocketContainer();
			if (!rocketContainer.IsUnityNull())
			{
				rocketContainer.PlayerOpenLoot(player, "", doPositionChecks: false);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Dashboard(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			IItemContainerEntity dashboardContainer = GetDashboardContainer();
			if (!dashboardContainer.IsUnityNull())
			{
				dashboardContainer.PlayerOpenLoot(player);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
			}
		}
	}

	public override void InitShared()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.InitShared();
		GameObject obj = mlrsRocket.Get();
		ServerProjectile component = obj.GetComponent<ServerProjectile>();
		rocketBaseGravity = (0f - Physics.get_gravity().y) * component.gravityModifier;
		rocketSpeed = component.speed;
		TimedExplosive component2 = obj.GetComponent<TimedExplosive>();
		RocketDamageRadius = component2.explosionRadius;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.mlrs != null)
		{
			SetUserTargetHitPos(info.msg.mlrs.targetPos);
			TrueHitPos = info.msg.mlrs.curHitPos;
			HitPosToRotation(TrueHitPos, out var hRot, out var vRot, out var g);
			CurGravityMultiplier = g / (0f - Physics.get_gravity().y);
			if (base.isServer)
			{
				HRotation = hRot;
				VRotation = vRot;
			}
			rocketStorageInstance.uid = info.msg.mlrs.rocketStorageID;
			dashboardStorageInstance.uid = info.msg.mlrs.dashboardStorageID;
			RocketAmmoCount = (int)info.msg.mlrs.ammoCount;
		}
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		return !IsFiringRockets;
	}

	private void SetUserTargetHitPos(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		if (UserTargetHitPos == worldPos)
		{
			return;
		}
		if (base.isServer)
		{
			Vector3 position = TerrainMeta.Position;
			Vector3 val = position + TerrainMeta.Size;
			worldPos.x = Mathf.Clamp(worldPos.x, position.x, val.x);
			worldPos.z = Mathf.Clamp(worldPos.z, position.z, val.z);
			worldPos.y = GetSurfaceHeight(worldPos);
		}
		UserTargetHitPos = worldPos;
		if (!base.isServer)
		{
			return;
		}
		trueTargetHitPos = UserTargetHitPos;
		foreach (TriggerSafeZone allSafeZone in TriggerSafeZone.allSafeZones)
		{
			Vector3 val2 = ((Component)allSafeZone).get_transform().get_position() + allSafeZone.triggerCollider.GetLocalCentre();
			val2.y = 0f;
			float num = allSafeZone.triggerCollider.GetRadius(((Component)allSafeZone).get_transform().get_localScale()) + targetAreaRadius;
			trueTargetHitPos.y = 0f;
			if (Vector3.Distance(val2, trueTargetHitPos) < num)
			{
				Vector3 val3 = trueTargetHitPos - val2;
				trueTargetHitPos = val2 + ((Vector3)(ref val3)).get_normalized() * num;
				trueTargetHitPos.y = GetSurfaceHeight(trueTargetHitPos);
				break;
			}
		}
	}

	private StorageContainer GetRocketContainer()
	{
		BaseEntity baseEntity = rocketStorageInstance.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	private StorageContainer GetDashboardContainer()
	{
		BaseEntity baseEntity = dashboardStorageInstance.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	private void HitPosToRotation(Vector3 hitPos, out float hRot, out float vRot, out float g)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 aimToTarget = GetAimToTarget(hitPos, out g);
		Quaternion val = Quaternion.LookRotation(aimToTarget, Vector3.get_up());
		Vector3 eulerAngles = ((Quaternion)(ref val)).get_eulerAngles();
		vRot = eulerAngles.x - 360f;
		aimToTarget.y = 0f;
		hRot = eulerAngles.y;
	}

	private Vector3 GetAimToTarget(Vector3 targetPos, out float g)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		g = rocketBaseGravity;
		float num = rocketSpeed;
		Vector3 val = targetPos - firingPoint.get_position();
		float num2 = Vector3Ex.Magnitude2D(val);
		float y = val.y;
		float num3 = Mathf.Sqrt(num * num * num * num - g * (g * (num2 * num2) + 2f * y * num * num));
		float num4 = Mathf.Atan((num * num + num3) / (g * num2)) * 57.29578f;
		float num5 = Mathf.Clamp(num4, 0f, 90f);
		if (float.IsNaN(num4))
		{
			num5 = 45f;
			g = ProjectileDistToGravity(num2, y, num5, num);
		}
		else if (num4 > vRotMax)
		{
			num5 = vRotMax;
			g = ProjectileDistToGravity(Mathf.Max(num2, minRange), y, num5, num);
		}
		((Vector3)(ref val)).Normalize();
		val.y = 0f;
		Vector3 val2 = Vector3.Cross(val, Vector3.get_up());
		val = Quaternion.AngleAxis(num5, val2) * val;
		return val;
	}

	private static float ProjectileDistToSpeed(float x, float y, float angle, float g, float fallbackV)
	{
		float num = angle * ((float)Math.PI / 180f);
		float num2 = Mathf.Sqrt(x * x * g / (x * Mathf.Sin(2f * num) - 2f * y * Mathf.Cos(num) * Mathf.Cos(num)));
		if (float.IsNaN(num2) || num2 < 1f)
		{
			num2 = fallbackV;
		}
		return num2;
	}

	private static float ProjectileDistToGravity(float x, float y, float θ, float v)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		float num = θ * ((float)Math.PI / 180f);
		float num2 = (v * v * x * Mathf.Sin(2f * num) - 2f * v * v * y * Mathf.Cos(num) * Mathf.Cos(num)) / (x * x);
		if (float.IsNaN(num2) || num2 < 0.01f)
		{
			num2 = 0f - Physics.get_gravity().y;
		}
		return num2;
	}
}
