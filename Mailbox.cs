using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class Mailbox : StorageContainer
{
	public string ownerPanel;

	public GameObjectRef mailDropSound;

	public ItemDefinition[] allowedItems;

	public bool autoSubmitWhenClosed;

	public bool shouldMarkAsFull;

	public int mailInputSlot => inventorySlots - 1;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Mailbox.OnRpcMessage", 0);
		try
		{
			if (rpc == 131727457 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Submit "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Submit", 0);
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
						RPC_Submit(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RPC_Submit");
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

	public virtual bool PlayerIsOwner(BasePlayer player)
	{
		return player.CanBuild();
	}

	public bool IsFull()
	{
		if (shouldMarkAsFull)
		{
			return HasFlag(Flags.Reserved1);
		}
		return false;
	}

	public void MarkFull(bool full)
	{
		SetFlag(Flags.Reserved1, shouldMarkAsFull && full);
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return base.PlayerOpenLoot(player, PlayerIsOwner(player) ? ownerPanel : panelToOpen);
	}

	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (panelName == ownerPanel)
		{
			if (PlayerIsOwner(player))
			{
				return base.CanOpenLootPanel(player, panelName);
			}
			return false;
		}
		if (!HasFreeSpace())
		{
			return !shouldMarkAsFull;
		}
		return true;
	}

	private bool HasFreeSpace()
	{
		return GetFreeSlot() != -1;
	}

	private int GetFreeSlot()
	{
		for (int i = 0; i < mailInputSlot; i++)
		{
			if (base.inventory.GetSlot(i) == null)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual bool MoveItemToStorage(Item item)
	{
		item.RemoveFromContainer();
		if (!item.MoveToContainer(base.inventory))
		{
			return false;
		}
		return true;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (autoSubmitWhenClosed)
		{
			SubmitInputItems(player);
		}
		if (IsFull())
		{
			base.inventory.GetSlot(mailInputSlot)?.Drop(GetDropPosition(), GetDropVelocity());
		}
		base.PlayerStoppedLooting(player);
		if (PlayerIsOwner(player))
		{
			SetFlag(Flags.On, b: false);
		}
	}

	[RPC_Server]
	public void RPC_Submit(RPCMessage msg)
	{
		if (!IsFull())
		{
			BasePlayer player = msg.player;
			SubmitInputItems(player);
		}
	}

	public void SubmitInputItems(BasePlayer fromPlayer)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Item slot = base.inventory.GetSlot(mailInputSlot);
		if (IsFull() || slot == null)
		{
			return;
		}
		if (MoveItemToStorage(slot))
		{
			if (slot.position != mailInputSlot)
			{
				Effect.server.Run(mailDropSound.resourcePath, GetDropPosition());
				if ((Object)(object)fromPlayer != (Object)null && !PlayerIsOwner(fromPlayer))
				{
					SetFlag(Flags.On, b: true);
				}
			}
		}
		else
		{
			slot.Drop(GetDropPosition(), GetDropVelocity());
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		MarkFull(!HasFreeSpace());
		base.OnItemAddedOrRemoved(item, added);
	}

	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		bool flag = PlayerIsOwner(player);
		if (!flag)
		{
			flag = item == base.inventory.GetSlot(mailInputSlot);
		}
		if (flag)
		{
			return base.CanMoveFrom(player, item);
		}
		return false;
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (allowedItems == null || allowedItems.Length == 0)
		{
			return base.ItemFilter(item, targetSlot);
		}
		ItemDefinition[] array = allowedItems;
		foreach (ItemDefinition itemDefinition in array)
		{
			if ((Object)(object)item.info == (Object)(object)itemDefinition)
			{
				return true;
			}
		}
		return false;
	}
}
