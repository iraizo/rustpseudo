using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class KeyLock : BaseLock
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition keyItemType;

	private int keyCode;

	private bool firstKeyCreated;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("KeyLock.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 4135414453u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_CreateKey "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_CreateKey", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4135414453u, "RPC_CreateKey", this, player, 3f))
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
							RPC_CreateKey(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_CreateKey");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 954115386 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Lock "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Lock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(954115386u, "RPC_Lock", this, player, 3f))
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
							RPC_Lock(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Lock");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1663222372 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Unlock "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Unlock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1663222372u, "RPC_Unlock", this, player, 3f))
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
							RPC_Unlock(rpc4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Unlock");
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

	public override bool HasLockPermission(BasePlayer player)
	{
		if (player.IsDead())
		{
			return false;
		}
		if (player.userID == base.OwnerID)
		{
			return true;
		}
		foreach (Item item in player.inventory.FindItemIDs(keyItemType.itemid))
		{
			if (CanKeyUnlockUs(item))
			{
				return true;
			}
		}
		return false;
	}

	private bool CanKeyUnlockUs(Item key)
	{
		if (key.instanceData == null)
		{
			return false;
		}
		if (key.instanceData.dataInt != keyCode)
		{
			return false;
		}
		return true;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.keyLock != null)
		{
			keyCode = info.msg.keyLock.code;
		}
	}

	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.OwnerID == 0L && Object.op_Implicit((Object)(object)GetParentEntity()))
		{
			base.OwnerID = GetParentEntity().OwnerID;
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.keyLock = Pool.Get<KeyLock>();
			info.msg.keyLock.code = keyCode;
		}
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		keyCode = Random.Range(1, 100000);
		Lock(deployedBy);
	}

	public override bool OnTryToOpen(BasePlayer player)
	{
		if (HasLockPermission(player))
		{
			return true;
		}
		return !IsLocked();
	}

	public override bool OnTryToClose(BasePlayer player)
	{
		if (HasLockPermission(player))
		{
			return true;
		}
		return !IsLocked();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Unlock(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && IsLocked() && HasLockPermission(rpc.player))
		{
			SetFlag(Flags.Locked, b: false);
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Lock(RPCMessage rpc)
	{
		Lock(rpc.player);
	}

	private void Lock(BasePlayer player)
	{
		if (!((Object)(object)player == (Object)null) && player.CanInteract() && !IsLocked() && HasLockPermission(player))
		{
			LockLock(player);
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_CreateKey(RPCMessage rpc)
	{
		if (!rpc.player.CanInteract() || (IsLocked() && !HasLockPermission(rpc.player)))
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(keyItemType.itemid);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			Debug.LogWarning((object)("RPC_CreateKey: Itemdef is missing! " + keyItemType));
			return;
		}
		ItemBlueprint bp = ItemManager.FindBlueprint(itemDefinition);
		if (rpc.player.inventory.crafting.CanCraft(bp))
		{
			InstanceData val = Pool.Get<InstanceData>();
			val.dataInt = keyCode;
			rpc.player.inventory.crafting.CraftItem(bp, rpc.player, val);
			if (!firstKeyCreated)
			{
				LockLock(rpc.player);
				SendNetworkUpdate();
				firstKeyCreated = true;
			}
		}
	}

	public void LockLock(BasePlayer player)
	{
		SetFlag(Flags.Locked, b: true);
		if (player.IsValid())
		{
			player.GiveAchievement("LOCK_LOCK");
		}
	}
}
