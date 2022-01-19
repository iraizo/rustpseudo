using System;
using System.Linq;
using System.Threading.Tasks;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class SteamInventory : EntityComponent<BasePlayer>
{
	private IPlayerItem[] Items;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SteamInventory.OnRpcMessage", 0);
		try
		{
			if (rpc == 643458331 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UpdateSteamInventory "));
				}
				TimeWarning val2 = TimeWarning.New("UpdateSteamInventory", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(643458331u, "UpdateSteamInventory", GetBaseEntity(), player))
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
							BaseEntity.RPCMessage rPCMessage = default(BaseEntity.RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							BaseEntity.RPCMessage msg2 = rPCMessage;
							UpdateSteamInventory(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in UpdateSteamInventory");
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

	public bool HasItem(int itemid)
	{
		if (Items == null)
		{
			return false;
		}
		IPlayerItem[] items = Items;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].get_DefinitionId() == itemid)
			{
				return true;
			}
		}
		return false;
	}

	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private async Task UpdateSteamInventory(BaseEntity.RPCMessage msg)
	{
		byte[] array = msg.read.BytesWithSize(10485760u);
		if (array == null)
		{
			Debug.LogWarning((object)"UpdateSteamInventory: Data is null");
			return;
		}
		IPlayerInventory val = await PlatformService.Instance.DeserializeInventory(array);
		if (val == null)
		{
			Debug.LogWarning((object)"UpdateSteamInventory: result is null");
		}
		else if ((Object)(object)base.baseEntity == (Object)null)
		{
			Debug.LogWarning((object)"UpdateSteamInventory: player is null");
		}
		else if (!val.BelongsTo(base.baseEntity.userID))
		{
			Debug.LogWarning((object)$"UpdateSteamPlayer: inventory belongs to someone else (userID={base.baseEntity.userID})");
		}
		else if (Object.op_Implicit((Object)(object)((Component)this).get_gameObject()))
		{
			Items = val.get_Items().ToArray();
			((IDisposable)val).Dispose();
		}
	}
}
