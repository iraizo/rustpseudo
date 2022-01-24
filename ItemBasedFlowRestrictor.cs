using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemBasedFlowRestrictor : IOEntity
{
	public ItemDefinition passthroughItem;

	public ItemContainer.ContentsType allowedContents = ItemContainer.ContentsType.Generic;

	public int maxStackSize = 1;

	public int numSlots;

	public string lootPanelName = "generic";

	public const Flags HasPassthrough = Flags.Reserved1;

	public const Flags Sparks = Flags.Reserved2;

	public float passthroughItemConditionLossPerSec = 1f;

	private ItemContainer inventory;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ItemBasedFlowRestrictor.OnRpcMessage", 0);
		try
		{
			if (rpc == 331989034 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenLoot "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenLoot", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(331989034u, "RPC_OpenLoot", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							RPC_OpenLoot(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenLoot");
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

	public override void ResetIOState()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		SetFlag(Flags.On, b: false);
		if (inventory != null)
		{
			inventory.GetSlot(0)?.Drop(((Component)debugOrigin).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 0.5f, GetInheritedDropVelocity() + ((Component)this).get_transform().get_forward() * 2f);
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!HasFlag(Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		SetFlag(Flags.On, IsPowered());
		SetFlag(Flags.Reserved1, HasPassthroughItem());
		SetFlag(Flags.Reserved2, IsOn() && !HasFlag(Flags.Reserved1));
	}

	public virtual bool HasPassthroughItem()
	{
		if (inventory.itemList.Count <= 0)
		{
			return false;
		}
		Item slot = inventory.GetSlot(0);
		if (slot == null)
		{
			return false;
		}
		if (passthroughItemConditionLossPerSec > 0f && slot.hasCondition && slot.conditionNormalized <= 0f)
		{
			return false;
		}
		if ((Object)(object)slot.info == (Object)(object)passthroughItem)
		{
			return true;
		}
		return false;
	}

	public virtual void TickPassthroughItem()
	{
		if (inventory.itemList.Count > 0 && HasFlag(Flags.On))
		{
			Item slot = inventory.GetSlot(0);
			if (slot != null && slot.hasCondition)
			{
				slot.LoseCondition(1f);
			}
		}
	}

	public override void ServerInit()
	{
		if (inventory == null)
		{
			CreateInventory(giveUID: true);
			OnInventoryFirstCreated(inventory);
		}
		((FacepunchBehaviour)this).InvokeRandomized((Action)TickPassthroughItem, 1f, 1f, 0.015f);
		base.ServerInit();
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		CreateInventory(giveUID: false);
	}

	public void CreateInventory(bool giveUID)
	{
		inventory = new ItemContainer();
		inventory.entityOwner = this;
		inventory.allowedContents = ((allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : allowedContents);
		inventory.SetOnlyAllowedItem(passthroughItem);
		inventory.maxStackSize = maxStackSize;
		inventory.ServerInitialize(null, numSlots);
		if (giveUID)
		{
			inventory.GiveUID();
		}
		inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (inventory != null)
			{
				info.msg.storageBox = Pool.Get<StorageBox>();
				info.msg.storageBox.contents = inventory.Save();
			}
			else
			{
				Debug.LogWarning((object)("Storage container without inventory: " + ((object)this).ToString()));
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
		{
			if (inventory != null)
			{
				inventory.Load(info.msg.storageBox.contents);
				inventory.capacity = numSlots;
			}
			else
			{
				Debug.LogWarning((object)("Storage container without inventory: " + ((object)this).ToString()));
			}
		}
	}

	public virtual void OnInventoryFirstCreated(ItemContainer container)
	{
	}

	public virtual void OnItemAddedOrRemoved(Item item, bool added)
	{
		SetFlag(Flags.Reserved1, HasPassthroughItem());
		MarkDirty();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage rpc)
	{
		if (inventory != null)
		{
			BasePlayer player = rpc.player;
			if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && player.inventory.loot.StartLootingEntity(this))
			{
				SetFlag(Flags.Open, b: true);
				player.inventory.loot.AddContainer(inventory);
				player.inventory.loot.SendImmediate();
				player.ClientRPCPlayer(null, player, "RPC_OpenLootPanel", lootPanelName);
				SendNetworkUpdate();
			}
		}
	}

	public void PlayerStoppedLooting(BasePlayer player)
	{
	}
}
