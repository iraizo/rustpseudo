using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class AutoTurret : ContainerIOEntity, IRemoteControllable
{
	public static class TurretFlags
	{
		public const Flags Peacekeeper = Flags.Reserved1;
	}

	public class UpdateAutoTurretScanQueue : ObjectWorkQueue<AutoTurret>
	{
		protected override void RunJob(AutoTurret entity)
		{
			if (((ObjectWorkQueue<AutoTurret>)this).ShouldAdd(entity))
			{
				entity.TargetScan();
			}
		}

		protected override bool ShouldAdd(AutoTurret entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public float bulletSpeed = 200f;

	public AmbienceEmitter ambienceEmitter;

	public GameObject assignDialog;

	public static UpdateAutoTurretScanQueue updateAutoTurretScanQueue = new UpdateAutoTurretScanQueue();

	private BasePlayer playerController;

	private string rcIdentifier = "TURRET";

	private Vector3 initialAimDir;

	public float rcTurnSensitivity = 4f;

	public Transform RCEyes;

	public TargetTrigger targetTrigger;

	public Transform socketTransform;

	private float nextShotTime;

	private float lastShotTime;

	private float nextVisCheck;

	private float lastTargetSeenTime;

	private bool targetVisible = true;

	private bool booting;

	private float nextIdleAimTime;

	private Vector3 targetAimDir = Vector3.get_forward();

	private const float bulletDamage = 15f;

	private float nextForcedAimTime;

	private Vector3 lastSentAimDir = Vector3.get_zero();

	private static float[] visibilityOffsets = new float[3] { 0f, 0.15f, -0.15f };

	private int peekIndex;

	[NonSerialized]
	private int numConsecutiveMisses;

	[NonSerialized]
	private int totalAmmo;

	private float nextAmmoCheckTime;

	private bool totalAmmoDirty = true;

	private float currentAmmoGravity;

	private float currentAmmoVelocity;

	private HeldEntity AttachedWeapon;

	public float attachedWeaponZOffsetScale = -0.5f;

	public BaseCombatEntity target;

	public Transform eyePos;

	public Transform muzzlePos;

	public Vector3 aimDir;

	public Transform gun_yaw;

	public Transform gun_pitch;

	public float sightRange = 30f;

	public SoundDefinition turnLoopDef;

	public SoundDefinition movementChangeDef;

	public SoundDefinition ambientLoopDef;

	public SoundDefinition focusCameraDef;

	public float focusSoundFreqMin = 2.5f;

	public float focusSoundFreqMax = 7f;

	public GameObjectRef peacekeeperToggleSound;

	public GameObjectRef onlineSound;

	public GameObjectRef offlineSound;

	public GameObjectRef targetAcquiredEffect;

	public GameObjectRef targetLostEffect;

	public GameObjectRef reloadEffect;

	public float aimCone;

	public const Flags Flag_Equipped = Flags.Reserved3;

	public const Flags Flag_MaxAuths = Flags.Reserved4;

	[NonSerialized]
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	public virtual bool RequiresMouse => false;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AutoTurret.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1092560690 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AddSelfAuthorize "));
				}
				TimeWarning val2 = TimeWarning.New("AddSelfAuthorize", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1092560690u, "AddSelfAuthorize", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							AddSelfAuthorize(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3057055788u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AssignToFriend "));
				}
				TimeWarning val2 = TimeWarning.New("AssignToFriend", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3057055788u, "AssignToFriend", this, player, 3f))
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
							AssignToFriend(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in AssignToFriend");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 253307592 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ClearList "));
				}
				TimeWarning val2 = TimeWarning.New("ClearList", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(253307592u, "ClearList", this, player, 3f))
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
							RPCMessage rpc3 = rPCMessage;
							ClearList(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in ClearList");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1500257773 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - FlipAim "));
				}
				TimeWarning val2 = TimeWarning.New("FlipAim", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1500257773u, "FlipAim", this, player, 3f))
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
							RPCMessage rpc4 = rPCMessage;
							FlipAim(rpc4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in FlipAim");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3617985969u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RemoveSelfAuthorize "));
				}
				TimeWarning val2 = TimeWarning.New("RemoveSelfAuthorize", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3617985969u, "RemoveSelfAuthorize", this, player, 3f))
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
							RPCMessage rpc5 = rPCMessage;
							RemoveSelfAuthorize(rpc5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1770263114 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_AttackAll "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_AttackAll", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1770263114u, "SERVER_AttackAll", this, player, 3f))
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
							RPCMessage rpc6 = rPCMessage;
							SERVER_AttackAll(rpc6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in SERVER_AttackAll");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3265538831u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_Peacekeeper "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_Peacekeeper", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3265538831u, "SERVER_Peacekeeper", this, player, 3f))
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
							RPCMessage rpc7 = rPCMessage;
							SERVER_Peacekeeper(rpc7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in SERVER_Peacekeeper");
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

	public bool PeacekeeperMode()
	{
		return HasFlag(Flags.Reserved1);
	}

	public bool IsBeingRemoteControlled()
	{
		return (Object)(object)playerController != (Object)null;
	}

	public Transform GetEyes()
	{
		return RCEyes;
	}

	public bool Occupied()
	{
		return false;
	}

	public BaseEntity GetEnt()
	{
		return this;
	}

	public virtual bool CanControl()
	{
		return false;
	}

	public void UserInput(InputState inputState, BasePlayer player)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp(0f - inputState.current.mouseDelta.y, -1f, 1f) * rcTurnSensitivity;
		float num2 = Mathf.Clamp(inputState.current.mouseDelta.x, -1f, 1f) * rcTurnSensitivity;
		Quaternion val = Quaternion.LookRotation(aimDir, ((Component)this).get_transform().get_up());
		Quaternion val2 = Quaternion.Euler(num, num2, 0f);
		Quaternion val3 = val * val2;
		aimDir = val3 * Vector3.get_forward();
		if (inputState.IsDown(BUTTON.RELOAD))
		{
			Reload();
		}
		bool flag = inputState.IsDown(BUTTON.FIRE_PRIMARY);
		EnsureReloaded();
		if (!(Time.get_time() >= nextShotTime && flag))
		{
			return;
		}
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if (Object.op_Implicit((Object)(object)attachedWeapon))
		{
			if (attachedWeapon.primaryMagazine.contents > 0)
			{
				FireAttachedGun(Vector3.get_zero(), aimCone);
				float delay = (attachedWeapon.isSemiAuto ? (attachedWeapon.repeatDelay * 1.5f) : attachedWeapon.repeatDelay);
				delay = attachedWeapon.ScaleRepeatDelay(delay);
				nextShotTime = Time.get_time() + delay;
			}
			else
			{
				nextShotTime = Time.get_time() + 5f;
			}
		}
		else if (HasGenericFireable())
		{
			AttachedWeapon.ServerUse();
			nextShotTime = Time.get_time() + 0.115f;
		}
		else
		{
			nextShotTime = Time.get_time() + 1f;
		}
	}

	public void InitializeControl(BasePlayer controller)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		playerController = controller;
		SetTarget(null);
		initialAimDir = aimDir;
	}

	public void StopControl()
	{
		playerController = null;
	}

	public void RCSetup()
	{
	}

	public void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		rcIdentifier = newID;
	}

	public string GetIdentifier()
	{
		return rcIdentifier;
	}

	public override int ConsumptionAmount()
	{
		return 10;
	}

	public void SetOnline()
	{
		SetIsOnline(online: true);
	}

	public void SetIsOnline(bool online)
	{
		if (online != HasFlag(Flags.On))
		{
			SetFlag(Flags.On, online);
			booting = false;
			SendNetworkUpdate();
			if (IsOffline())
			{
				SetTarget(null);
				isLootable = true;
			}
			else
			{
				isLootable = false;
			}
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int result = Mathf.Min(1, GetCurrentEnergy());
		switch (outputSlot)
		{
		case 0:
			if (!HasTarget())
			{
				return 0;
			}
			return result;
		case 1:
			if (totalAmmo > 50)
			{
				return 0;
			}
			return result;
		case 2:
			if (totalAmmo != 0)
			{
				return 0;
			}
			return result;
		default:
			return 0;
		}
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (IsPowered() && !IsOn())
		{
			InitiateStartup();
		}
		else if ((!IsPowered() && IsOn()) || booting)
		{
			InitiateShutdown();
		}
	}

	public void InitiateShutdown()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOffline() || booting)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)SetOnline);
			booting = false;
			Effect.server.Run(offlineSound.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
			SetIsOnline(online: false);
		}
	}

	public void InitiateStartup()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOnline() && !booting)
		{
			Effect.server.Run(onlineSound.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
			((FacepunchBehaviour)this).Invoke((Action)SetOnline, 2f);
			booting = true;
		}
	}

	public void SetPeacekeepermode(bool isOn)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (PeacekeeperMode() != isOn)
		{
			SetFlag(Flags.Reserved1, isOn);
			Effect.server.Run(peacekeeperToggleSound.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
		}
	}

	public bool IsValidWeapon(Item item)
	{
		ItemDefinition info = item.info;
		if (item.isBroken)
		{
			return false;
		}
		ItemModEntity component = ((Component)info).GetComponent<ItemModEntity>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		HeldEntity component2 = component.entityPrefab.Get().GetComponent<HeldEntity>();
		if ((Object)(object)component2 == (Object)null)
		{
			return false;
		}
		if (!component2.IsUsableByTurret)
		{
			return false;
		}
		return true;
	}

	public bool CanAcceptItem(Item item, int targetSlot)
	{
		Item slot = base.inventory.GetSlot(0);
		if (IsValidWeapon(item) && targetSlot == 0)
		{
			return true;
		}
		if (item.info.category == ItemCategory.Ammunition)
		{
			if (slot == null || !Object.op_Implicit((Object)(object)GetAttachedWeapon()))
			{
				return false;
			}
			if (targetSlot == 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public bool AtMaxAuthCapacity()
	{
		return HasFlag(Flags.Reserved4);
	}

	public void UpdateMaxAuthCapacity()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (Object.op_Implicit((Object)(object)activeGameMode) && activeGameMode.limitTeamAuths)
		{
			SetFlag(Flags.Reserved4, authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize());
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void FlipAim(RPCMessage rpc)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOnline() && IsAuthed(rpc.player) && !booting)
		{
			((Component)this).get_transform().set_rotation(Quaternion.LookRotation(-((Component)this).get_transform().get_forward(), ((Component)this).get_transform().get_up()));
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void AddSelfAuthorize(RPCMessage rpc)
	{
		AddSelfAuthorize(rpc.player);
	}

	private void AddSelfAuthorize(BasePlayer player)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		if (!IsOnline() && player.CanBuild() && !AtMaxAuthCapacity())
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
			PlayerNameID val = new PlayerNameID();
			val.userid = player.userID;
			val.username = player.displayName;
			authorizedPlayers.Add(val);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RemoveSelfAuthorize(RPCMessage rpc)
	{
		if (!booting && !IsOnline() && IsAuthed(rpc.player))
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void ClearList(RPCMessage rpc)
	{
		if (!booting && !IsOnline() && IsAuthed(rpc.player))
		{
			authorizedPlayers.Clear();
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void AssignToFriend(RPCMessage msg)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		if (!AtMaxAuthCapacity() && !((Object)(object)msg.player == (Object)null) && msg.player.CanInteract() && CanChangeSettings(msg.player))
		{
			ulong num = msg.read.UInt64();
			if (num != 0L && !IsAuthed(num))
			{
				string username = BasePlayer.SanitizePlayerNameString(msg.read.String(256), num);
				PlayerNameID val = new PlayerNameID();
				val.userid = num;
				val.username = username;
				authorizedPlayers.Add(val);
				UpdateMaxAuthCapacity();
				SendNetworkUpdate();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void SERVER_Peacekeeper(RPCMessage rpc)
	{
		if (IsAuthed(rpc.player))
		{
			SetPeacekeepermode(isOn: true);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void SERVER_AttackAll(RPCMessage rpc)
	{
		if (IsAuthed(rpc.player))
		{
			SetPeacekeepermode(isOn: false);
		}
	}

	public virtual float TargetScanRate()
	{
		return 1f;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
		((FacepunchBehaviour)this).InvokeRepeating((Action)ServerTick, Random.Range(0f, 1f), 0.015f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)SendAimDir, Random.Range(0f, 1f), 0.2f, 0.05f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)ScheduleForTargetScan, Random.Range(0f, 1f), TargetScanRate(), 0.2f);
		((Component)targetTrigger).GetComponent<SphereCollider>().set_radius(sightRange);
	}

	public void SendAimDir()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_realtimeSinceStartup() > nextForcedAimTime || HasTarget() || Vector3.Angle(lastSentAimDir, aimDir) > 0.03f)
		{
			lastSentAimDir = aimDir;
			ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", aimDir);
			nextForcedAimTime = Time.get_realtimeSinceStartup() + 2f;
		}
	}

	public void SetTarget(BaseCombatEntity targ)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)targ != (Object)(object)target)
		{
			Effect.server.Run(((Object)(object)targ == (Object)null) ? targetLostEffect.resourcePath : targetAcquiredEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
			MarkDirtyForceUpdateOutputs();
			nextShotTime += 0.1f;
		}
		target = targ;
	}

	public virtual bool CheckPeekers()
	{
		return true;
	}

	public bool ObjectVisible(BaseCombatEntity obj)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		Vector3 position = ((Component)eyePos).get_transform().get_position();
		if (GamePhysics.CheckSphere(position, 0.1f, 2097152, (QueryTriggerInteraction)0))
		{
			return false;
		}
		Vector3 val = AimOffset(obj);
		float num = Vector3.Distance(val, position);
		Vector3 val2 = val - position;
		Vector3 val3 = Vector3.Cross(((Vector3)(ref val2)).get_normalized(), Vector3.get_up());
		for (int i = 0; (float)i < (CheckPeekers() ? 3f : 1f); i++)
		{
			val2 = val + val3 * visibilityOffsets[i] - position;
			Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
			list.Clear();
			GamePhysics.TraceAll(new Ray(position, normalized), 0f, list, num * 1.1f, 1218652417, (QueryTriggerInteraction)0);
			for (int j = 0; j < list.Count; j++)
			{
				BaseEntity entity = list[j].GetEntity();
				if ((!((Object)(object)entity != (Object)null) || !entity.isClient) && (!((Object)(object)entity != (Object)null) || !((Object)(object)entity.ToPlayer() != (Object)null) || entity.EqualNetID(obj)) && (!((Object)(object)entity != (Object)null) || !entity.EqualNetID(this)))
				{
					if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)obj || entity.EqualNetID(obj)))
					{
						Pool.FreeList<RaycastHit>(ref list);
						peekIndex = i;
						return true;
					}
					if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	public virtual void FireAttachedGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if (!((Object)(object)attachedWeapon == (Object)null) && !IsOffline())
		{
			attachedWeapon.ServerUse(1f, gun_pitch);
		}
	}

	public virtual void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		if (IsOffline())
		{
			return;
		}
		if ((Object)(object)muzzleToUse == (Object)null)
		{
			muzzleToUse = muzzlePos;
		}
		Vector3 val = ((Component)GetCenterMuzzle()).get_transform().get_position() - GetCenterMuzzle().get_forward() * 0.25f;
		Vector3 val2 = ((Component)GetCenterMuzzle()).get_transform().get_forward();
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, val2);
		targetPos = val + modifiedAimConeDirection * 300f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(val, modifiedAimConeDirection), 0f, list, 300f, 1219701521, (QueryTriggerInteraction)0);
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if (((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID(this))) || (PeacekeeperMode() && (Object)(object)target != (Object)null && (Object)(object)entity != (Object)null && (Object)(object)((Component)entity).GetComponent<BasePlayer>() != (Object)null && !entity.EqualNetID(target)))
			{
				continue;
			}
			BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
			if ((Object)(object)baseCombatEntity != (Object)null)
			{
				ApplyDamage(baseCombatEntity, ((RaycastHit)(ref hit)).get_point(), modifiedAimConeDirection);
				if (baseCombatEntity.EqualNetID(target))
				{
					flag = true;
				}
			}
			if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
			{
				targetPos = ((RaycastHit)(ref hit)).get_point();
				Vector3 val3 = targetPos - val;
				val2 = ((Vector3)(ref val3)).get_normalized();
				break;
			}
		}
		int num = 2;
		if (!flag)
		{
			numConsecutiveMisses++;
		}
		else
		{
			numConsecutiveMisses = 0;
		}
		if ((Object)(object)target != (Object)null && targetVisible && numConsecutiveMisses > num)
		{
			ApplyDamage(target, ((Component)target).get_transform().get_position() - val2 * 0.25f, val2);
			numConsecutiveMisses = 0;
		}
		ClientRPC<uint, Vector3>(null, "CLIENT_FireGun", StringPool.Get(((Object)((Component)muzzleToUse).get_gameObject()).get_name()), targetPos);
		Pool.FreeList<RaycastHit>(ref list);
	}

	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		float num = 15f * Random.Range(0.9f, 1.1f);
		if (entity is BasePlayer && (Object)(object)entity != (Object)(object)target)
		{
			num *= 0.5f;
		}
		if (PeacekeeperMode() && (Object)(object)entity == (Object)(object)target)
		{
			target.MarkHostileFor(300f);
		}
		HitInfo info = new HitInfo(this, entity, DamageType.Bullet, num, point);
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

	public void IdleTick()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_realtimeSinceStartup() > nextIdleAimTime)
		{
			nextIdleAimTime = Time.get_realtimeSinceStartup() + Random.Range(4f, 5f);
			Quaternion val = Quaternion.LookRotation(((Component)this).get_transform().get_forward(), Vector3.get_up());
			val *= Quaternion.AngleAxis(Random.Range(-45f, 45f), Vector3.get_up());
			targetAimDir = val * Vector3.get_forward();
		}
		if (!HasTarget())
		{
			aimDir = Vector3.Lerp(aimDir, targetAimDir, Time.get_deltaTime() * 2f);
		}
	}

	public virtual bool HasClipAmmo()
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return false;
		}
		return attachedWeapon.primaryMagazine.contents > 0;
	}

	public virtual bool HasReserveAmmo()
	{
		return totalAmmo > 0;
	}

	public int GetTotalAmmo()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return num;
		}
		List<Item> list = Pool.GetList<Item>();
		base.inventory.FindAmmo(list, attachedWeapon.primaryMagazine.definition.ammoTypes);
		for (int i = 0; i < list.Count; i++)
		{
			num += list[i].amount;
		}
		Pool.FreeList<Item>(ref list);
		return num;
	}

	public AmmoTypes GetValidAmmoTypes()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return (AmmoTypes)2;
		}
		return attachedWeapon.primaryMagazine.definition.ammoTypes;
	}

	public ItemDefinition GetDesiredAmmo()
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return null;
		}
		return attachedWeapon.primaryMagazine.ammoType;
	}

	public void Reload()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return;
		}
		nextShotTime = Mathf.Max(nextShotTime, Time.get_time() + Mathf.Min(attachedWeapon.GetReloadDuration() * 0.5f, 2f));
		AmmoTypes ammoTypes = attachedWeapon.primaryMagazine.definition.ammoTypes;
		if (attachedWeapon.primaryMagazine.contents > 0)
		{
			bool flag = false;
			if (base.inventory.capacity > base.inventory.itemList.Count)
			{
				flag = true;
			}
			else
			{
				int num = 0;
				foreach (Item item in base.inventory.itemList)
				{
					if ((Object)(object)item.info == (Object)(object)attachedWeapon.primaryMagazine.ammoType)
					{
						num += item.info.stackable - item.amount;
					}
				}
				flag = num >= attachedWeapon.primaryMagazine.contents;
			}
			if (!flag)
			{
				return;
			}
			base.inventory.AddItem(attachedWeapon.primaryMagazine.ammoType, attachedWeapon.primaryMagazine.contents, 0uL);
			attachedWeapon.primaryMagazine.contents = 0;
		}
		List<Item> list = Pool.GetList<Item>();
		base.inventory.FindAmmo(list, ammoTypes);
		if (list.Count > 0)
		{
			Effect.server.Run(reloadEffect.resourcePath, this, StringPool.Get("WeaponAttachmentPoint"), Vector3.get_zero(), Vector3.get_zero());
			totalAmmoDirty = true;
			attachedWeapon.primaryMagazine.ammoType = list[0].info;
			int num2 = 0;
			while (attachedWeapon.primaryMagazine.contents < attachedWeapon.primaryMagazine.capacity && num2 < list.Count)
			{
				if ((Object)(object)list[num2].info == (Object)(object)attachedWeapon.primaryMagazine.ammoType)
				{
					int num3 = attachedWeapon.primaryMagazine.capacity - attachedWeapon.primaryMagazine.contents;
					num3 = Mathf.Min(list[num2].amount, num3);
					list[num2].UseItem(num3);
					attachedWeapon.primaryMagazine.contents += num3;
				}
				num2++;
			}
		}
		ItemDefinition ammoType = attachedWeapon.primaryMagazine.ammoType;
		if (Object.op_Implicit((Object)(object)ammoType))
		{
			ItemModProjectile component = ((Component)ammoType).GetComponent<ItemModProjectile>();
			GameObject val = component.projectileObject.Get();
			if (Object.op_Implicit((Object)(object)val))
			{
				if (Object.op_Implicit((Object)(object)val.GetComponent<Projectile>()))
				{
					currentAmmoGravity = 0f;
					currentAmmoVelocity = component.GetMaxVelocity();
				}
				else
				{
					ServerProjectile component2 = val.GetComponent<ServerProjectile>();
					if (Object.op_Implicit((Object)(object)component2))
					{
						currentAmmoGravity = component2.gravityModifier;
						currentAmmoVelocity = component2.speed;
					}
				}
			}
		}
		Pool.FreeList<Item>(ref list);
		attachedWeapon.SendNetworkUpdate();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		totalAmmoDirty = true;
		Reload();
	}

	public void UpdateTotalAmmo()
	{
		int num = totalAmmo;
		totalAmmo = GetTotalAmmo();
		if (num != totalAmmo)
		{
			MarkDirtyForceUpdateOutputs();
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModEntity>()))
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)UpdateAttachedWeapon))
			{
				UpdateAttachedWeapon();
			}
			((FacepunchBehaviour)this).Invoke((Action)UpdateAttachedWeapon, 0.5f);
		}
	}

	public void EnsureReloaded(bool onlyReloadIfEmpty = true)
	{
		bool flag = HasReserveAmmo();
		if (onlyReloadIfEmpty)
		{
			if (flag && !HasClipAmmo())
			{
				Reload();
			}
		}
		else if (flag)
		{
			Reload();
		}
	}

	public BaseProjectile GetAttachedWeapon()
	{
		return AttachedWeapon as BaseProjectile;
	}

	public virtual bool HasFallbackWeapon()
	{
		return false;
	}

	private bool HasGenericFireable()
	{
		if ((Object)(object)AttachedWeapon != (Object)null)
		{
			return AttachedWeapon.IsInstrument();
		}
		return false;
	}

	public void UpdateAttachedWeapon()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		Item slot = base.inventory.GetSlot(0);
		HeldEntity heldEntity = null;
		if (slot != null && (slot.info.category == ItemCategory.Weapon || slot.info.category == ItemCategory.Fun))
		{
			BaseEntity heldEntity2 = slot.GetHeldEntity();
			if ((Object)(object)heldEntity2 != (Object)null)
			{
				HeldEntity component = ((Component)heldEntity2).GetComponent<HeldEntity>();
				if ((Object)(object)component != (Object)null && component.IsUsableByTurret)
				{
					heldEntity = component;
				}
			}
		}
		SetFlag(Flags.Reserved3, (Object)(object)heldEntity != (Object)null);
		if ((Object)(object)heldEntity == (Object)null)
		{
			if (Object.op_Implicit((Object)(object)GetAttachedWeapon()))
			{
				GetAttachedWeapon().SetGenericVisible(wantsVis: false);
				GetAttachedWeapon().SetLightsOn(isOn: false);
			}
			AttachedWeapon = null;
			return;
		}
		heldEntity.SetLightsOn(isOn: true);
		Transform transform = ((Component)heldEntity).get_transform();
		Transform muzzleTransform = heldEntity.MuzzleTransform;
		heldEntity.SetParent(null);
		transform.set_localPosition(Vector3.get_zero());
		transform.set_localRotation(Quaternion.get_identity());
		Quaternion val = transform.get_rotation() * Quaternion.Inverse(muzzleTransform.get_rotation());
		heldEntity.limitNetworking = false;
		heldEntity.SetFlag(Flags.Disabled, b: false);
		heldEntity.SetParent(this, StringPool.Get(((Object)socketTransform).get_name()));
		transform.set_localPosition(Vector3.get_zero());
		transform.set_localRotation(Quaternion.get_identity());
		transform.set_rotation(transform.get_rotation() * val);
		Vector3 val2 = socketTransform.InverseTransformPoint(muzzleTransform.get_position());
		transform.set_localPosition(Vector3.get_left() * val2.x);
		float num = Vector3.Distance(muzzleTransform.get_position(), transform.get_position());
		transform.set_localPosition(transform.get_localPosition() + Vector3.get_forward() * num * attachedWeaponZOffsetScale);
		heldEntity.SetGenericVisible(wantsVis: true);
		AttachedWeapon = heldEntity;
		totalAmmoDirty = true;
		Reload();
		UpdateTotalAmmo();
	}

	public override void OnKilled(HitInfo info)
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon != (Object)null)
		{
			attachedWeapon.SetGenericVisible(wantsVis: false);
			attachedWeapon.SetLightsOn(isOn: false);
		}
		AttachedWeapon = null;
		base.OnKilled(info);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		UpdateTotalAmmo();
		EnsureReloaded(onlyReloadIfEmpty: false);
		UpdateTotalAmmo();
		nextShotTime = Time.get_time();
	}

	public virtual float GetMaxAngleForEngagement()
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		float result = (((Object)(object)attachedWeapon == (Object)null) ? 1f : ((1f - Mathf.InverseLerp(0.2f, 1f, attachedWeapon.repeatDelay)) * 7f));
		if (Time.get_time() - lastShotTime > 1f)
		{
			result = 1f;
		}
		return result;
	}

	public void TargetTick()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_realtimeSinceStartup() >= nextVisCheck)
		{
			nextVisCheck = Time.get_realtimeSinceStartup() + Random.Range(0.2f, 0.3f);
			targetVisible = ObjectVisible(target);
			if (targetVisible)
			{
				lastTargetSeenTime = Time.get_realtimeSinceStartup();
			}
		}
		EnsureReloaded();
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if (Time.get_time() >= nextShotTime && targetVisible && Mathf.Abs(AngleToTarget(target, currentAmmoGravity != 0f)) < GetMaxAngleForEngagement())
		{
			if (Object.op_Implicit((Object)(object)attachedWeapon))
			{
				if (attachedWeapon.primaryMagazine.contents > 0)
				{
					FireAttachedGun(AimOffset(target), aimCone, null, PeacekeeperMode() ? target : null);
					float delay = (attachedWeapon.isSemiAuto ? (attachedWeapon.repeatDelay * 1.5f) : attachedWeapon.repeatDelay);
					delay = attachedWeapon.ScaleRepeatDelay(delay);
					nextShotTime = Time.get_time() + delay;
				}
				else
				{
					nextShotTime = Time.get_time() + 5f;
				}
			}
			else if (HasFallbackWeapon())
			{
				FireGun(AimOffset(target), aimCone, null, target);
				nextShotTime = Time.get_time() + 0.115f;
			}
			else if (HasGenericFireable())
			{
				AttachedWeapon.ServerUse();
				nextShotTime = Time.get_time() + 0.115f;
			}
			else
			{
				nextShotTime = Time.get_time() + 1f;
			}
		}
		if ((Object)(object)target == (Object)null || target.IsDead() || Time.get_realtimeSinceStartup() - lastTargetSeenTime > 3f || Vector3.Distance(((Component)this).get_transform().get_position(), ((Component)target).get_transform().get_position()) > sightRange || (PeacekeeperMode() && !IsEntityHostile(target)))
		{
			SetTarget(null);
		}
	}

	public bool HasTarget()
	{
		if ((Object)(object)target != (Object)null)
		{
			return target.IsAlive();
		}
		return false;
	}

	public void OfflineTick()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		aimDir = Vector3.get_up();
	}

	public virtual bool IsEntityHostile(BaseCombatEntity ent)
	{
		BasePet basePet;
		if ((basePet = ent as BasePet) != null && (Object)(object)basePet.Brain.OwningPlayer != (Object)null)
		{
			if (!basePet.Brain.OwningPlayer.IsHostile())
			{
				return ent.IsHostile();
			}
			return true;
		}
		return ent.IsHostile();
	}

	public bool ShouldTarget(BaseCombatEntity targ)
	{
		if (targ is AutoTurret)
		{
			return false;
		}
		if (targ is RidableHorse)
		{
			return false;
		}
		BasePet basePet;
		if ((basePet = targ as BasePet) != null && (Object)(object)basePet.Brain.OwningPlayer != (Object)null && IsAuthed(basePet.Brain.OwningPlayer))
		{
			return false;
		}
		return true;
	}

	private void ScheduleForTargetScan()
	{
		((ObjectWorkQueue<AutoTurret>)updateAutoTurretScanQueue).Add(this);
	}

	public void TargetScan()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (HasTarget() || IsOffline() || IsBeingRemoteControlled() || targetTrigger.entityContents == null)
		{
			return;
		}
		Enumerator<BaseEntity> enumerator = targetTrigger.entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.get_Current();
				if ((Object)(object)current == (Object)null)
				{
					continue;
				}
				BaseCombatEntity component = ((Component)current).GetComponent<BaseCombatEntity>();
				if ((Object)(object)component == (Object)null || !component.IsAlive() || !InFiringArc(component) || !ObjectVisible(component))
				{
					continue;
				}
				if (!Sentry.targetall)
				{
					BasePlayer basePlayer = component as BasePlayer;
					if (Object.op_Implicit((Object)(object)basePlayer) && (IsAuthed(basePlayer) || Ignore(basePlayer)))
					{
						continue;
					}
				}
				if (!ShouldTarget(component))
				{
					continue;
				}
				if (PeacekeeperMode())
				{
					if (!IsEntityHostile(component))
					{
						continue;
					}
					if ((Object)(object)target == (Object)null)
					{
						nextShotTime = Time.get_time() + 1f;
					}
				}
				SetTarget(component);
				break;
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	protected virtual bool Ignore(BasePlayer player)
	{
		return false;
	}

	public void ServerTick()
	{
		if (base.isClient || base.IsDestroyed)
		{
			return;
		}
		if (!IsOnline())
		{
			OfflineTick();
		}
		else if (!IsBeingRemoteControlled())
		{
			if (HasTarget())
			{
				TargetTick();
			}
			else
			{
				IdleTick();
			}
		}
		UpdateFacingToTarget();
		if (totalAmmoDirty && Time.get_time() > nextAmmoCheckTime)
		{
			UpdateTotalAmmo();
			totalAmmoDirty = false;
			nextAmmoCheckTime = Time.get_time() + 0.5f;
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (((IsOnline() && !HasTarget()) || !targetVisible) && !((Object)(object)(info.Initiator as AutoTurret) != (Object)null) && !((Object)(object)(info.Initiator as SamSite) != (Object)null) && !((Object)(object)(info.Initiator as GunTrap) != (Object)null))
		{
			BasePlayer basePlayer = info.Initiator as BasePlayer;
			if (!Object.op_Implicit((Object)(object)basePlayer) || !IsAuthed(basePlayer))
			{
				SetTarget(info.Initiator as BaseCombatEntity);
			}
		}
	}

	public void UpdateFacingToTarget()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)target != (Object)null && targetVisible)
		{
			Vector3 val = AimOffset(target);
			Vector3 val2;
			if (peekIndex != 0)
			{
				Vector3 position = ((Component)eyePos).get_transform().get_position();
				Vector3.Distance(val, position);
				val2 = val - position;
				Vector3 val3 = Vector3.Cross(((Vector3)(ref val2)).get_normalized(), Vector3.get_up());
				val += val3 * visibilityOffsets[peekIndex];
			}
			val2 = val - ((Component)eyePos).get_transform().get_position();
			Vector3 val4 = ((Vector3)(ref val2)).get_normalized();
			if (currentAmmoGravity != 0f)
			{
				float num = 0.2f;
				if (target is BasePlayer)
				{
					float num2 = Mathf.Clamp01(target.WaterFactor()) * 1.8f;
					if (num2 > num)
					{
						num = num2;
					}
				}
				val = ((Component)target).get_transform().get_position() + Vector3.get_up() * num;
				float angle = GetAngle(((Component)eyePos).get_transform().get_position(), val, currentAmmoVelocity, currentAmmoGravity);
				Vector3 val5 = Vector3Ex.XZ3D(val) - Vector3Ex.XZ3D(((Component)eyePos).get_transform().get_position());
				val5 = ((Vector3)(ref val5)).get_normalized();
				val4 = Quaternion.LookRotation(val5) * Quaternion.Euler(angle, 0f, 0f) * Vector3.get_forward();
			}
			aimDir = val4;
		}
		UpdateAiming();
	}

	private float GetAngle(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravityScale)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		float num = Physics.get_gravity().y * gravityScale;
		float num2 = Vector3.Distance(Vector3Ex.XZ3D(launchPosition), Vector3Ex.XZ3D(targetPosition));
		float num3 = launchPosition.y - targetPosition.y;
		float num4 = Mathf.Pow(launchVelocity, 2f);
		float num5 = Mathf.Pow(launchVelocity, 4f);
		float num6 = Mathf.Atan((num4 + Mathf.Sqrt(num5 - num * (num * Mathf.Pow(num2, 2f) + 2f * num3 * num4))) / (num * num2)) * 57.29578f;
		float num7 = Mathf.Atan((num4 - Mathf.Sqrt(num5 - num * (num * Mathf.Pow(num2, 2f) + 2f * num3 * num4))) / (num * num2)) * 57.29578f;
		if (float.IsNaN(num6) && float.IsNaN(num7))
		{
			return -45f;
		}
		if (float.IsNaN(num6))
		{
			return num7;
		}
		if (!(num6 > num7))
		{
			return num7;
		}
		return num6;
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		AddSelfAuthorize(deployedBy);
	}

	public bool IsOnline()
	{
		return IsOn();
	}

	public bool IsOffline()
	{
		return !IsOnline();
	}

	public override void ResetState()
	{
		base.ResetState();
	}

	public virtual Transform GetCenterMuzzle()
	{
		return gun_pitch;
	}

	public float AngleToTarget(BaseCombatEntity potentialtarget, bool use2D = false)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		use2D = true;
		Transform centerMuzzle = GetCenterMuzzle();
		Vector3 position = centerMuzzle.get_position();
		Vector3 val = AimOffset(potentialtarget);
		Vector3 zero = Vector3.get_zero();
		Vector3 val2;
		if (use2D)
		{
			zero = Vector3Ex.Direction2D(val, position);
		}
		else
		{
			val2 = val - position;
			zero = ((Vector3)(ref val2)).get_normalized();
		}
		Vector3 val3;
		if (!use2D)
		{
			val3 = centerMuzzle.get_forward();
		}
		else
		{
			val2 = Vector3Ex.XZ3D(centerMuzzle.get_forward());
			val3 = ((Vector3)(ref val2)).get_normalized();
		}
		return Vector3.Angle(val3, zero);
	}

	public virtual bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return Mathf.Abs(AngleToTarget(potentialtarget)) <= 90f;
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player) && IsOffline())
		{
			return IsAuthed(player);
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = Pool.Get<AutoTurret>();
		info.msg.autoturret.users = authorizedPlayers;
		info.msg.rcEntity = Pool.Get<RCEntity>();
		info.msg.rcEntity.identifier = GetIdentifier();
	}

	public override void PostSave(SaveInfo info)
	{
		base.PostSave(info);
		info.msg.autoturret.users = null;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			authorizedPlayers = info.msg.autoturret.users;
			info.msg.autoturret.users = null;
		}
		if (info.msg.rcEntity != null)
		{
			UpdateIdentifier(info.msg.rcEntity.identifier);
		}
	}

	public Vector3 AimOffset(BaseCombatEntity aimat)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = aimat as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			if (basePlayer.IsSleeping())
			{
				return ((Component)basePlayer).get_transform().get_position() + Vector3.get_up() * 0.1f;
			}
			if (basePlayer.IsWounded())
			{
				return ((Component)basePlayer).get_transform().get_position() + Vector3.get_up() * 0.25f;
			}
			return basePlayer.eyes.position;
		}
		return aimat.CenterPoint();
	}

	public float GetAimSpeed()
	{
		if (HasTarget())
		{
			return 5f;
		}
		return 1f;
	}

	public void UpdateAiming()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (!(aimDir == Vector3.get_zero()))
		{
			float speed = 5f;
			if (base.isServer)
			{
				speed = ((!HasTarget()) ? 15f : 35f);
			}
			Quaternion val = Quaternion.LookRotation(aimDir);
			Quaternion val2 = Quaternion.Euler(0f, ((Quaternion)(ref val)).get_eulerAngles().y, 0f);
			Quaternion val3 = Quaternion.Euler(((Quaternion)(ref val)).get_eulerAngles().x, 0f, 0f);
			if (((Component)gun_yaw).get_transform().get_rotation() != val2)
			{
				((Component)gun_yaw).get_transform().set_rotation(Lerp(((Component)gun_yaw).get_transform().get_rotation(), val2, speed));
			}
			if (((Component)gun_pitch).get_transform().get_localRotation() != val3)
			{
				((Component)gun_pitch).get_transform().set_localRotation(Lerp(((Component)gun_pitch).get_transform().get_localRotation(), val3, speed));
			}
		}
	}

	private static Quaternion Lerp(Quaternion from, Quaternion to, float speed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Lerp(to, from, Mathf.Pow(2f, (0f - speed) * Time.get_deltaTime()));
	}

	public bool IsAuthed(ulong id)
	{
		return Enumerable.Any<PlayerNameID>((IEnumerable<PlayerNameID>)authorizedPlayers, (Func<PlayerNameID, bool>)((PlayerNameID x) => x.userid == id));
	}

	public bool IsAuthed(BasePlayer player)
	{
		return Enumerable.Any<PlayerNameID>((IEnumerable<PlayerNameID>)authorizedPlayers, (Func<PlayerNameID, bool>)((PlayerNameID x) => x.userid == player.userID));
	}

	public bool AnyAuthed()
	{
		return authorizedPlayers.Count > 0;
	}

	public virtual bool CanChangeSettings(BasePlayer player)
	{
		if (IsAuthed(player) && IsOffline())
		{
			return player.CanBuild();
		}
		return false;
	}
}
