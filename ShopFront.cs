using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class ShopFront : StorageContainer
{
	public static class ShopFrontFlags
	{
		public const Flags VendorAccepted = Flags.Reserved1;

		public const Flags CustomerAccepted = Flags.Reserved2;

		public const Flags Exchanging = Flags.Reserved3;
	}

	public float maxUseAngle = 27f;

	public BasePlayer vendorPlayer;

	public BasePlayer customerPlayer;

	public GameObjectRef transactionCompleteEffect;

	public ItemContainer customerInventory;

	private float AngleDotProduct => 1f - maxUseAngle / 90f;

	public ItemContainer vendorInventory => base.inventory;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ShopFront.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1159607245 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AcceptClicked "));
				}
				TimeWarning val2 = TimeWarning.New("AcceptClicked", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1159607245u, "AcceptClicked", this, player, 3f))
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
							AcceptClicked(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AcceptClicked");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3168107540u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - CancelClicked "));
				}
				TimeWarning val2 = TimeWarning.New("CancelClicked", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3168107540u, "CancelClicked", this, player, 3f))
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
							CancelClicked(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in CancelClicked");
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

	public bool TradeLocked()
	{
		return false;
	}

	public bool IsTradingPlayer(BasePlayer player)
	{
		if ((Object)(object)player != (Object)null)
		{
			if (!IsPlayerCustomer(player))
			{
				return IsPlayerVendor(player);
			}
			return true;
		}
		return false;
	}

	public bool IsPlayerCustomer(BasePlayer player)
	{
		return (Object)(object)player == (Object)(object)customerPlayer;
	}

	public bool IsPlayerVendor(BasePlayer player)
	{
		return (Object)(object)player == (Object)(object)vendorPlayer;
	}

	public bool PlayerInVendorPos(BasePlayer player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 right = ((Component)this).get_transform().get_right();
		Vector3 val = ((Component)player).get_transform().get_position() - ((Component)this).get_transform().get_position();
		return Vector3.Dot(right, ((Vector3)(ref val)).get_normalized()) <= 0f - AngleDotProduct;
	}

	public bool PlayerInCustomerPos(BasePlayer player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 right = ((Component)this).get_transform().get_right();
		Vector3 val = ((Component)player).get_transform().get_position() - ((Component)this).get_transform().get_position();
		return Vector3.Dot(right, ((Vector3)(ref val)).get_normalized()) >= AngleDotProduct;
	}

	public bool LootEligable(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (PlayerInVendorPos(player) && (Object)(object)vendorPlayer == (Object)null)
		{
			return true;
		}
		if (PlayerInCustomerPos(player) && (Object)(object)customerPlayer == (Object)null)
		{
			return true;
		}
		return false;
	}

	public void ResetTrade()
	{
		SetFlag(Flags.Reserved1, b: false);
		SetFlag(Flags.Reserved2, b: false);
		SetFlag(Flags.Reserved3, b: false);
		vendorInventory.SetLocked(isLocked: false);
		customerInventory.SetLocked(isLocked: false);
		((FacepunchBehaviour)this).CancelInvoke((Action)CompleteTrade);
	}

	public void CompleteTrade()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)vendorPlayer != (Object)null && (Object)(object)customerPlayer != (Object)null && HasFlag(Flags.Reserved1) && HasFlag(Flags.Reserved2))
		{
			for (int num = vendorInventory.capacity - 1; num >= 0; num--)
			{
				Item slot = vendorInventory.GetSlot(num);
				Item slot2 = customerInventory.GetSlot(num);
				if (Object.op_Implicit((Object)(object)customerPlayer) && slot != null)
				{
					customerPlayer.GiveItem(slot);
				}
				if (Object.op_Implicit((Object)(object)vendorPlayer) && slot2 != null)
				{
					vendorPlayer.GiveItem(slot2);
				}
			}
			Effect.server.Run(transactionCompleteEffect.resourcePath, this, 0u, new Vector3(0f, 1f, 0f), Vector3.get_zero());
		}
		ResetTrade();
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void AcceptClicked(RPCMessage msg)
	{
		if (IsTradingPlayer(msg.player) && !((Object)(object)vendorPlayer == (Object)null) && !((Object)(object)customerPlayer == (Object)null))
		{
			if (IsPlayerVendor(msg.player))
			{
				SetFlag(Flags.Reserved1, b: true);
				vendorInventory.SetLocked(isLocked: true);
			}
			else if (IsPlayerCustomer(msg.player))
			{
				SetFlag(Flags.Reserved2, b: true);
				customerInventory.SetLocked(isLocked: true);
			}
			if (HasFlag(Flags.Reserved1) && HasFlag(Flags.Reserved2))
			{
				SetFlag(Flags.Reserved3, b: true);
				((FacepunchBehaviour)this).Invoke((Action)CompleteTrade, 2f);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void CancelClicked(RPCMessage msg)
	{
		if (IsTradingPlayer(msg.player))
		{
			Object.op_Implicit((Object)(object)vendorPlayer);
			Object.op_Implicit((Object)(object)customerPlayer);
			ResetTrade();
		}
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = vendorInventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptVendorItem));
		if (customerInventory == null)
		{
			customerInventory = new ItemContainer();
			customerInventory.allowedContents = ((allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : allowedContents);
			customerInventory.SetOnlyAllowedItem(allowedItem);
			customerInventory.entityOwner = this;
			customerInventory.maxStackSize = maxStackSize;
			customerInventory.ServerInitialize(null, inventorySlots);
			customerInventory.GiveUID();
			customerInventory.onDirty += OnInventoryDirty;
			customerInventory.onItemAddedRemoved = OnItemAddedOrRemoved;
			ItemContainer itemContainer2 = customerInventory;
			itemContainer2.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer2.canAcceptItem, new Func<Item, int, bool>(CanAcceptCustomerItem));
			OnInventoryFirstCreated(customerInventory);
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		ResetTrade();
	}

	private bool CanAcceptVendorItem(Item item, int targetSlot)
	{
		if (((Object)(object)vendorPlayer != (Object)null && (Object)(object)item.GetOwnerPlayer() == (Object)(object)vendorPlayer) || vendorInventory.itemList.Contains(item) || item.parent == null)
		{
			return true;
		}
		return false;
	}

	private bool CanAcceptCustomerItem(Item item, int targetSlot)
	{
		if (((Object)(object)customerPlayer != (Object)null && (Object)(object)item.GetOwnerPlayer() == (Object)(object)customerPlayer) || customerInventory.itemList.Contains(item) || item.parent == null)
		{
			return true;
		}
		return false;
	}

	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		if (TradeLocked())
		{
			return false;
		}
		if (IsTradingPlayer(player))
		{
			if (IsPlayerCustomer(player) && customerInventory.itemList.Contains(item) && !customerInventory.IsLocked())
			{
				return true;
			}
			if (IsPlayerVendor(player) && vendorInventory.itemList.Contains(item) && !vendorInventory.IsLocked())
			{
				return true;
			}
		}
		return false;
	}

	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (base.CanOpenLootPanel(player, panelName))
		{
			return LootEligable(player);
		}
		return false;
	}

	public void ReturnPlayerItems(BasePlayer player)
	{
		if (!IsTradingPlayer(player))
		{
			return;
		}
		ItemContainer itemContainer = null;
		if (IsPlayerVendor(player))
		{
			itemContainer = vendorInventory;
		}
		else if (IsPlayerCustomer(player))
		{
			itemContainer = customerInventory;
		}
		if (itemContainer != null)
		{
			for (int num = itemContainer.itemList.Count - 1; num >= 0; num--)
			{
				Item item = itemContainer.itemList[num];
				player.GiveItem(item);
			}
		}
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (IsTradingPlayer(player))
		{
			ReturnPlayerItems(player);
			if ((Object)(object)player == (Object)(object)vendorPlayer)
			{
				vendorPlayer = null;
			}
			if ((Object)(object)player == (Object)(object)customerPlayer)
			{
				customerPlayer = null;
			}
			UpdatePlayers();
			ResetTrade();
			base.PlayerStoppedLooting(player);
		}
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		bool flag = base.PlayerOpenLoot(player, panelToOpen);
		if (flag)
		{
			player.inventory.loot.AddContainer(customerInventory);
			player.inventory.loot.SendImmediate();
		}
		if (PlayerInVendorPos(player) && (Object)(object)vendorPlayer == (Object)null)
		{
			vendorPlayer = player;
		}
		else
		{
			if (!PlayerInCustomerPos(player) || !((Object)(object)customerPlayer == (Object)null))
			{
				return false;
			}
			customerPlayer = player;
		}
		ResetTrade();
		UpdatePlayers();
		return flag;
	}

	public void UpdatePlayers()
	{
		ClientRPC(null, "CLIENT_ReceivePlayers", (!((Object)(object)vendorPlayer == (Object)null)) ? vendorPlayer.net.ID : 0u, (!((Object)(object)customerPlayer == (Object)null)) ? customerPlayer.net.ID : 0u);
	}
}
