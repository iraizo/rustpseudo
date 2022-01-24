using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ReclaimTerminal : StorageContainer
{
	public int itemCount;

	public static readonly Phrase DespawnToast = new Phrase("softcore.reclaimdespawn", "Items remaining in the reclaim terminal will despawn in two hours.");

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ReclaimTerminal.OnRpcMessage", 0);
		try
		{
			if (rpc == 2609933020u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_ReloadLoot "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReloadLoot", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2609933020u, "RPC_ReloadLoot", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2609933020u, "RPC_ReloadLoot", this, player, 3f))
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
							RPCMessage msg2 = rPCMessage;
							RPC_ReloadLoot(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_ReloadLoot");
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

	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, b: true);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void RPC_ReloadLoot(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && !((Object)(object)ReclaimManager.instance == (Object)null) && !((Object)(object)player.inventory.loot.entitySource != (Object)(object)this))
		{
			LoadReclaimLoot(player);
		}
	}

	public void LoadReclaimLoot(BasePlayer player)
	{
		if ((Object)(object)ReclaimManager.instance == (Object)null)
		{
			return;
		}
		List<ReclaimManager.PlayerReclaimEntry> list = Pool.GetList<ReclaimManager.PlayerReclaimEntry>();
		ReclaimManager.instance.GetReclaimsForPlayer(player.userID, ref list);
		itemCount = 0;
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			if (base.inventory.GetSlot(i) != null)
			{
				itemCount++;
			}
		}
		foreach (ReclaimManager.PlayerReclaimEntry item2 in list)
		{
			for (int num = item2.inventory.itemList.Count - 1; num >= 0; num--)
			{
				Item item = item2.inventory.itemList[num];
				itemCount++;
				item.MoveToContainer(base.inventory);
			}
		}
		Pool.FreeList<ReclaimManager.PlayerReclaimEntry>(ref list);
		SendNetworkUpdate();
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if ((Object)(object)ReclaimManager.instance == (Object)null)
		{
			return false;
		}
		LoadReclaimLoot(player);
		return base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (!((Object)(object)ReclaimManager.instance == (Object)null))
		{
			ReclaimManager.instance.DoCleanup();
			if (base.inventory.itemList.Count > 0)
			{
				ReclaimManager.instance.AddPlayerReclaim(player.userID, base.inventory.itemList, 0uL);
				player.ShowToast(2, DespawnToast);
			}
			base.PlayerStoppedLooting(player);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.reclaimTerminal = Pool.Get<ReclaimTerminal>();
			info.msg.reclaimTerminal.itemCount = itemCount;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.reclaimTerminal != null)
		{
			itemCount = info.msg.reclaimTerminal.itemCount;
		}
	}
}
