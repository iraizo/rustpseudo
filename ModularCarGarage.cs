using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ModularCarGarage : ContainerIOEntity
{
	[Serializable]
	public class ChassisBuildOption
	{
		public GameObjectRef prefab;

		public ItemDefinition itemDef;
	}

	public enum OccupantLock
	{
		CannotHaveLock,
		NoLock,
		HasLock
	}

	private enum VehicleLiftState
	{
		Down,
		Up
	}

	private ModularCar lockedOccupant;

	private readonly HashSet<BasePlayer> lootingPlayers = new HashSet<BasePlayer>();

	private MagnetSnap magnetSnap;

	[SerializeField]
	private Transform vehicleLift;

	[SerializeField]
	private Animation vehicleLiftAnim;

	[SerializeField]
	private string animName = "LiftUp";

	[SerializeField]
	private VehicleLiftOccupantTrigger occupantTrigger;

	[SerializeField]
	private float liftMoveTime = 1f;

	[SerializeField]
	private EmissionToggle poweredLight;

	[SerializeField]
	private EmissionToggle inUseLight;

	[SerializeField]
	private Transform vehicleLiftPos;

	[SerializeField]
	[Range(0f, 1f)]
	private float recycleEfficiency = 0.5f;

	[SerializeField]
	private Transform recycleDropPos;

	[SerializeField]
	private bool needsElectricity;

	[SerializeField]
	private SoundDefinition liftStartSoundDef;

	[SerializeField]
	private SoundDefinition liftStopSoundDef;

	[SerializeField]
	private SoundDefinition liftStopDownSoundDef;

	[SerializeField]
	private SoundDefinition liftLoopSoundDef;

	public SoundDefinition liftOpenSoundDef;

	public SoundDefinition liftCloseSoundDef;

	public ChassisBuildOption[] chassisBuildOptions;

	public ItemAmount lockResourceCost;

	public ItemDefinition carKeyDefinition;

	private VehicleLiftState vehicleLiftState;

	private Sound liftLoopSound;

	private Vector3 downPos;

	public const Flags DestroyingChassis = Flags.Reserved6;

	public const float TimeToDestroyChassis = 10f;

	private ModularCar carOccupant
	{
		get
		{
			if (!((Object)(object)lockedOccupant != (Object)null))
			{
				return occupantTrigger.carOccupant;
			}
			return lockedOccupant;
		}
	}

	private bool HasOccupant
	{
		get
		{
			if ((Object)(object)carOccupant != (Object)null)
			{
				return carOccupant.IsFullySpawned();
			}
			return false;
		}
	}

	public bool PlatformIsOccupied { get; private set; }

	public bool HasEditableOccupant { get; private set; }

	public bool HasDriveableOccupant { get; private set; }

	public OccupantLock OccupantLockState { get; private set; }

	public int OccupantLockID { get; private set; }

	private bool LiftIsUp => vehicleLiftState == VehicleLiftState.Up;

	private bool LiftIsMoving => vehicleLiftAnim.get_isPlaying();

	private bool LiftIsDown => vehicleLiftState == VehicleLiftState.Down;

	public bool IsDestroyingChassis => HasFlag(Flags.Reserved6);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ModularCarGarage.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 554177909 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_DeselectedLootItem "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_DeselectedLootItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(554177909u, "RPC_DeselectedLootItem", this, player, 3f))
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
							RPC_DeselectedLootItem(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_DeselectedLootItem");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3659332720u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenEditing "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenEditing", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3659332720u, "RPC_OpenEditing", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3659332720u, "RPC_OpenEditing", this, player, 3f))
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
							RPC_OpenEditing(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_OpenEditing");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1582295101 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RepairItem "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RepairItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1582295101u, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1582295101u, "RPC_RepairItem", this, player, 3f))
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
							RPC_RepairItem(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_RepairItem");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3710764312u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestAddLock "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RequestAddLock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3710764312u, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3710764312u, "RPC_RequestAddLock", this, player, 3f))
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
							RPC_RequestAddLock(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_RequestAddLock");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1151989253 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestCarKey "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RequestCarKey", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1151989253u, "RPC_RequestCarKey", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1151989253u, "RPC_RequestCarKey", this, player, 3f))
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
							RPCMessage msg6 = rPCMessage;
							RPC_RequestCarKey(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_RequestCarKey");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1046853419 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestRemoveLock "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RequestRemoveLock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1046853419u, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1046853419u, "RPC_RequestRemoveLock", this, player, 3f))
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
							RPCMessage msg7 = rPCMessage;
							RPC_RequestRemoveLock(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_RequestRemoveLock");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4033916654u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_SelectedLootItem "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_SelectedLootItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4033916654u, "RPC_SelectedLootItem", this, player, 3f))
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
							RPCMessage msg8 = rPCMessage;
							RPC_SelectedLootItem(msg8);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in RPC_SelectedLootItem");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2974124904u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StartDestroyingChassis "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_StartDestroyingChassis", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2974124904u, "RPC_StartDestroyingChassis", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2974124904u, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2974124904u, "RPC_StartDestroyingChassis", this, player, 3f))
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
							RPCMessage msg9 = rPCMessage;
							RPC_StartDestroyingChassis(msg9);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in RPC_StartDestroyingChassis");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3830531963u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StopDestroyingChassis "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_StopDestroyingChassis", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3830531963u, "RPC_StopDestroyingChassis", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3830531963u, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3830531963u, "RPC_StopDestroyingChassis", this, player, 3f))
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
							RPCMessage msg10 = rPCMessage;
							RPC_StopDestroyingChassis(msg10);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in RPC_StopDestroyingChassis");
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

	protected void FixedUpdate()
	{
		if (!base.isServer || magnetSnap == null)
		{
			return;
		}
		UpdateCarOccupant();
		if (HasOccupant && carOccupant.CouldBeEdited() && carOccupant.GetSpeed() <= 1f)
		{
			if (IsOn() || !carOccupant.IsComplete())
			{
				if ((Object)(object)lockedOccupant == (Object)null)
				{
					GrabOccupant(occupantTrigger.carOccupant);
				}
				magnetSnap.FixedUpdate(((Component)carOccupant).get_transform());
			}
			if (carOccupant.carLock.HasALock && !carOccupant.carLock.CanHaveALock())
			{
				carOccupant.carLock.RemoveLock();
			}
		}
		else if (HasOccupant && carOccupant.rigidBody.get_isKinematic())
		{
			ReleaseOccupant();
		}
		if (HasOccupant && IsDestroyingChassis && carOccupant.HasAnyModules)
		{
			StopChassisDestroy();
		}
	}

	internal override void DoServerDestroy()
	{
		if (HasOccupant)
		{
			ReleaseOccupant();
			if (!HasDriveableOccupant)
			{
				carOccupant.Kill(DestroyMode.Gib);
			}
		}
		base.DoServerDestroy();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		magnetSnap = new MagnetSnap(vehicleLiftPos);
		RefreshOnOffState();
		SetOccupantState(hasOccupant: false, editableOccupant: false, driveableOccupant: false, OccupantLock.CannotHaveLock, 0, forced: true);
		RefreshLiftState(forced: true);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleLift = Pool.Get<VehicleLift>();
		info.msg.vehicleLift.platformIsOccupied = PlatformIsOccupied;
		info.msg.vehicleLift.editableOccupant = HasEditableOccupant;
		info.msg.vehicleLift.driveableOccupant = HasDriveableOccupant;
		info.msg.vehicleLift.occupantLockState = (int)OccupantLockState;
		info.msg.vehicleLift.occupantLockID = OccupantLockID;
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen);
		if (!flag)
		{
			return false;
		}
		if (HasEditableOccupant)
		{
			player.inventory.loot.AddContainer(carOccupant.Inventory.ModuleContainer);
			player.inventory.loot.AddContainer(carOccupant.Inventory.ChassisContainer);
			player.inventory.loot.SendImmediate();
		}
		lootingPlayers.Add(player);
		RefreshLiftState();
		return flag;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		lootingPlayers.Remove(player);
		base.PlayerStoppedLooting(player);
		RefreshLiftState();
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		RefreshOnOffState();
	}

	public bool TryGetModuleForItem(Item item, out BaseVehicleModule result)
	{
		if (!HasOccupant)
		{
			result = null;
			return false;
		}
		result = carOccupant.GetModuleForItem(item);
		return (Object)(object)result != (Object)null;
	}

	private void RefreshOnOffState()
	{
		bool flag = !needsElectricity || currentEnergy >= ConsumptionAmount();
		if (flag != IsOn())
		{
			SetFlag(Flags.On, flag);
		}
	}

	private void UpdateCarOccupant()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			if (HasOccupant)
			{
				bool editableOccupant = Vector3.SqrMagnitude(((Component)carOccupant).get_transform().get_position() - vehicleLiftPos.get_position()) < 1f && carOccupant.CouldBeEdited();
				bool driveableOccupant = carOccupant.IsComplete();
				OccupantLock occupantLockState = (carOccupant.carLock.CanHaveALock() ? ((!carOccupant.carLock.HasALock) ? OccupantLock.NoLock : OccupantLock.HasLock) : OccupantLock.CannotHaveLock);
				int lockID = carOccupant.carLock.LockID;
				SetOccupantState(HasOccupant, editableOccupant, driveableOccupant, occupantLockState, lockID);
			}
			else
			{
				SetOccupantState(hasOccupant: false, editableOccupant: false, driveableOccupant: false, OccupantLock.CannotHaveLock, 0);
			}
		}
	}

	private void UpdateOccupantMode()
	{
		if (HasOccupant)
		{
			carOccupant.inEditableLocation = HasEditableOccupant && LiftIsUp;
			carOccupant.immuneToDecay = IsOn();
		}
	}

	private void WakeNearbyRigidbodies()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(((Component)this).get_transform().get_position(), 7f, list, 34816, (QueryTriggerInteraction)2);
		foreach (Collider item in list)
		{
			Rigidbody attachedRigidbody = item.get_attachedRigidbody();
			if ((Object)(object)attachedRigidbody != (Object)null && attachedRigidbody.IsSleeping())
			{
				attachedRigidbody.WakeUp();
			}
			BaseEntity baseEntity = item.ToBaseEntity();
			BaseRidableAnimal baseRidableAnimal;
			if ((Object)(object)baseEntity != (Object)null && (baseRidableAnimal = baseEntity as BaseRidableAnimal) != null && baseRidableAnimal.isServer)
			{
				baseRidableAnimal.UpdateDropToGroundForDuration(2f);
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	private void EditableOccupantEntered()
	{
		RefreshLoot();
	}

	private void EditableOccupantLeft()
	{
		RefreshLoot();
	}

	private void RefreshLoot()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		list.AddRange((IEnumerable<BasePlayer>)lootingPlayers);
		foreach (BasePlayer item in list)
		{
			item.inventory.loot.Clear();
			PlayerOpenLoot(item);
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	private void GrabOccupant(ModularCar occupant)
	{
		if (!((Object)(object)occupant == (Object)null))
		{
			lockedOccupant = occupant;
			lockedOccupant.DisablePhysics();
		}
	}

	private void ReleaseOccupant()
	{
		carOccupant.inEditableLocation = false;
		carOccupant.immuneToDecay = false;
		if ((Object)(object)lockedOccupant != (Object)null)
		{
			lockedOccupant.EnablePhysics();
			lockedOccupant = null;
		}
	}

	private void StopChassisDestroy()
	{
		if (((FacepunchBehaviour)this).IsInvoking((Action)FinishDestroyingChassis))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FinishDestroyingChassis);
		}
		SetFlag(Flags.Reserved6, b: false);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RepairItem(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		uint num = msg.read.UInt32();
		if (!((Object)(object)player == (Object)null))
		{
			Item vehicleItem = carOccupant.GetVehicleItem(num);
			if (vehicleItem != null)
			{
				RepairBench.RepairAnItem(vehicleItem, player, this, 0f, mustKnowBlueprint: false);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": Couldn't get item to repair, with ID: " + num));
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_OpenEditing(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && !LiftIsMoving)
		{
			PlayerOpenLoot(player);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_SelectedLootItem(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		uint itemUID = msg.read.UInt32();
		if ((Object)(object)player == (Object)null || !player.inventory.loot.IsLooting() || (Object)(object)player.inventory.loot.entitySource != (Object)(object)this)
		{
			return;
		}
		Item vehicleItem = carOccupant.GetVehicleItem(itemUID);
		if (vehicleItem == null)
		{
			return;
		}
		bool flag = player.inventory.loot.RemoveContainerAt(3);
		if (TryGetModuleForItem(vehicleItem, out var result))
		{
			VehicleModuleStorage vehicleModuleStorage;
			VehicleModuleCamper vehicleModuleCamper;
			if ((vehicleModuleStorage = result as VehicleModuleStorage) != null)
			{
				IItemContainerEntity container = vehicleModuleStorage.GetContainer();
				if (!container.IsUnityNull())
				{
					player.inventory.loot.AddContainer(container.inventory);
					flag = true;
				}
			}
			else if ((vehicleModuleCamper = result as VehicleModuleCamper) != null)
			{
				IItemContainerEntity container2 = vehicleModuleCamper.GetContainer();
				if (!container2.IsUnityNull())
				{
					player.inventory.loot.AddContainer(container2.inventory);
					flag = true;
				}
			}
		}
		if (flag)
		{
			player.inventory.loot.SendImmediate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_DeselectedLootItem(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (player.inventory.loot.IsLooting() && !((Object)(object)player.inventory.loot.entitySource != (Object)(object)this) && player.inventory.loot.RemoveContainerAt(3))
		{
			player.inventory.loot.SendImmediate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RequestAddLock(RPCMessage msg)
	{
		if (!HasOccupant || carOccupant.carLock.HasALock)
		{
			return;
		}
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			ItemAmount itemAmount = lockResourceCost;
			if ((float)player.inventory.GetAmount(itemAmount.itemDef.itemid) >= itemAmount.amount && carOccupant.carLock.CanCraftAKey(player, free: true))
			{
				player.inventory.Take(null, itemAmount.itemDef.itemid, Mathf.CeilToInt(itemAmount.amount));
				carOccupant.carLock.AddALock();
				carOccupant.carLock.TryCraftAKey(player, free: true);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RequestRemoveLock(RPCMessage msg)
	{
		if (HasOccupant && carOccupant.carLock.HasALock)
		{
			carOccupant.carLock.RemoveLock();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RequestCarKey(RPCMessage msg)
	{
		if (HasOccupant && carOccupant.carLock.HasALock)
		{
			BasePlayer player = msg.player;
			if (!((Object)(object)player == (Object)null))
			{
				carOccupant.carLock.TryCraftAKey(player, free: false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void RPC_StartDestroyingChassis(RPCMessage msg)
	{
		if (!carOccupant.HasAnyModules)
		{
			((FacepunchBehaviour)this).Invoke((Action)FinishDestroyingChassis, 10f);
			SetFlag(Flags.Reserved6, b: true);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void RPC_StopDestroyingChassis(RPCMessage msg)
	{
		StopChassisDestroy();
	}

	private void FinishDestroyingChassis()
	{
		if (HasOccupant && !carOccupant.HasAnyModules)
		{
			carOccupant.Kill(DestroyMode.Gib);
			SetFlag(Flags.Reserved6, b: false);
		}
	}

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		downPos = ((Component)vehicleLift).get_transform().get_position();
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			UpdateOccupantMode();
		}
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		return IsOn();
	}

	public override int ConsumptionAmount()
	{
		return 5;
	}

	private void SetOccupantState(bool hasOccupant, bool editableOccupant, bool driveableOccupant, OccupantLock occupantLockState, int occupantLockID, bool forced = false)
	{
		if (PlatformIsOccupied == hasOccupant && HasEditableOccupant == editableOccupant && HasDriveableOccupant == driveableOccupant && OccupantLockState == occupantLockState && OccupantLockID == occupantLockID && !forced)
		{
			return;
		}
		bool hasEditableOccupant = HasEditableOccupant;
		PlatformIsOccupied = hasOccupant;
		HasEditableOccupant = editableOccupant;
		HasDriveableOccupant = driveableOccupant;
		OccupantLockState = occupantLockState;
		OccupantLockID = occupantLockID;
		if (base.isServer)
		{
			UpdateOccupantMode();
			SendNetworkUpdate();
			if (hasEditableOccupant && !editableOccupant)
			{
				EditableOccupantLeft();
			}
			else if (editableOccupant && !hasEditableOccupant)
			{
				EditableOccupantEntered();
			}
		}
		RefreshLiftState();
	}

	private void RefreshLiftState(bool forced = false)
	{
		VehicleLiftState desiredLiftState = ((IsOpen() || (HasEditableOccupant && !HasDriveableOccupant)) ? VehicleLiftState.Up : VehicleLiftState.Down);
		MoveLift(desiredLiftState, 0f, forced);
	}

	private void MoveLift(VehicleLiftState desiredLiftState, float startDelay = 0f, bool forced = false)
	{
		if (vehicleLiftState != desiredLiftState || forced)
		{
			_ = vehicleLiftState;
			vehicleLiftState = desiredLiftState;
			if (base.isServer)
			{
				UpdateOccupantMode();
				WakeNearbyRigidbodies();
			}
			if (!((Component)this).get_gameObject().get_activeSelf())
			{
				vehicleLiftAnim.get_Item(animName).set_time((desiredLiftState == VehicleLiftState.Up) ? 1f : 0f);
				vehicleLiftAnim.Play();
			}
			else if (desiredLiftState == VehicleLiftState.Up)
			{
				((FacepunchBehaviour)this).Invoke((Action)MoveLiftUp, startDelay);
			}
			else
			{
				((FacepunchBehaviour)this).Invoke((Action)MoveLiftDown, startDelay);
			}
		}
	}

	private void MoveLiftUp()
	{
		AnimationState obj = vehicleLiftAnim.get_Item(animName);
		obj.set_speed(obj.get_length() / liftMoveTime);
		vehicleLiftAnim.Play();
	}

	private void MoveLiftDown()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		AnimationState val = vehicleLiftAnim.get_Item(animName);
		val.set_speed(val.get_length() / liftMoveTime);
		if (!vehicleLiftAnim.get_isPlaying() && Vector3.Distance(((Component)vehicleLift).get_transform().get_position(), downPos) > 0.01f)
		{
			val.set_time(1f);
		}
		val.set_speed(val.get_speed() * -1f);
		vehicleLiftAnim.Play();
	}
}
