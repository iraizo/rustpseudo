using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class LootableCorpse : BaseCorpse, LootPanel.IHasLootPanel
{
	public string lootPanelName = "generic";

	[NonSerialized]
	public ulong playerSteamID;

	[NonSerialized]
	public string _playerName;

	[NonSerialized]
	public ItemContainer[] containers;

	public virtual string playerName
	{
		get
		{
			return _playerName;
		}
		set
		{
			_playerName = value;
		}
	}

	public Phrase LootPanelTitle => Phrase.op_Implicit(playerName);

	public Phrase LootPanelName => Phrase.op_Implicit("N/A");

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("LootableCorpse.OnRpcMessage", 0);
		try
		{
			if (rpc == 2278459738u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_LootCorpse "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_LootCorpse", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2278459738u, "RPC_LootCorpse", this, player, 3f))
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
							RPC_LootCorpse(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_LootCorpse");
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
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		DropItems();
		if (containers != null)
		{
			ItemContainer[] array = containers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill();
			}
		}
		containers = null;
	}

	public void TakeFrom(params ItemContainer[] source)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(containers == null, "Initializing Twice");
		TimeWarning val = TimeWarning.New("Corpse.TakeFrom", 0);
		try
		{
			containers = new ItemContainer[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				containers[i] = new ItemContainer();
				containers[i].ServerInitialize(null, source[i].capacity);
				containers[i].GiveUID();
				containers[i].entityOwner = this;
				Item[] array = source[i].itemList.ToArray();
				foreach (Item item in array)
				{
					if (!item.MoveToContainer(containers[i]))
					{
						item.DropAndTossUpwards(((Component)this).get_transform().get_position());
					}
				}
			}
			ResetRemovalTime();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override bool CanRemove()
	{
		return !IsOpen();
	}

	public virtual bool CanLoot()
	{
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_LootCorpse(RPCMessage rpc)
	{
		BasePlayer player = rpc.player;
		if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && CanLoot() && containers != null && player.inventory.loot.StartLootingEntity(this))
		{
			SetFlag(Flags.Open, b: true);
			ItemContainer[] array = containers;
			foreach (ItemContainer container in array)
			{
				player.inventory.loot.AddContainer(container);
			}
			player.inventory.loot.SendImmediate();
			ClientRPCPlayer(null, player, "RPC_ClientLootCorpse");
			SendNetworkUpdate();
		}
	}

	public void PlayerStoppedLooting(BasePlayer player)
	{
		ResetRemovalTime();
		SetFlag(Flags.Open, b: false);
		SendNetworkUpdate();
	}

	public void DropItems()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (containers != null)
		{
			DroppedItemContainer droppedItemContainer = ItemContainer.Drop("assets/prefabs/misc/item drop/item_drop_backpack.prefab", ((Component)this).get_transform().get_position(), Quaternion.get_identity(), containers);
			if ((Object)(object)droppedItemContainer != (Object)null)
			{
				droppedItemContainer.playerName = playerName;
				droppedItemContainer.playerSteamID = playerSteamID;
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Pool.Get<LootableCorpse>();
		info.msg.lootableCorpse.playerName = playerName;
		info.msg.lootableCorpse.playerID = playerSteamID;
		if (!info.forDisk || containers == null)
		{
			return;
		}
		info.msg.lootableCorpse.privateData = Pool.Get<Private>();
		info.msg.lootableCorpse.privateData.container = Pool.GetList<ItemContainer>();
		ItemContainer[] array = containers;
		foreach (ItemContainer itemContainer in array)
		{
			if (itemContainer != null)
			{
				ItemContainer val = itemContainer.Save();
				if (val != null)
				{
					info.msg.lootableCorpse.privateData.container.Add(val);
				}
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse == null)
		{
			return;
		}
		playerName = info.msg.lootableCorpse.playerName;
		playerSteamID = info.msg.lootableCorpse.playerID;
		if (info.fromDisk && info.msg.lootableCorpse.privateData != null && info.msg.lootableCorpse.privateData.container != null)
		{
			int count = info.msg.lootableCorpse.privateData.container.Count;
			containers = new ItemContainer[count];
			for (int i = 0; i < count; i++)
			{
				containers[i] = new ItemContainer();
				containers[i].ServerInitialize(null, info.msg.lootableCorpse.privateData.container[i].slots);
				containers[i].GiveUID();
				containers[i].entityOwner = this;
				containers[i].Load(info.msg.lootableCorpse.privateData.container[i]);
			}
		}
	}
}
