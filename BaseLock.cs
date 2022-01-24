using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLock : BaseEntity
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseLock.OnRpcMessage", 0);
		try
		{
			if (rpc == 3572556655u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_TakeLock "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_TakeLock", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3572556655u, "RPC_TakeLock", this, player, 3f))
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
							RPC_TakeLock(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_TakeLock");
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

	public virtual bool GetPlayerLockPermission(BasePlayer player)
	{
		return OnTryToOpen(player);
	}

	public virtual bool OnTryToOpen(BasePlayer player)
	{
		return !IsLocked();
	}

	public virtual bool OnTryToClose(BasePlayer player)
	{
		return true;
	}

	public virtual bool HasLockPermission(BasePlayer player)
	{
		return true;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_TakeLock(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && !IsLocked())
		{
			Item item = ItemManager.Create(itemType, 1, skinID);
			if (item != null)
			{
				rpc.player.GiveItem(item);
			}
			Kill();
		}
	}

	public override float BoundsPadding()
	{
		return 2f;
	}
}
