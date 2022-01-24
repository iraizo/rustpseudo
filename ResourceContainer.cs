using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class ResourceContainer : EntityComponent<BaseEntity>
{
	public bool lootable = true;

	[NonSerialized]
	public ItemContainer container;

	[NonSerialized]
	public float lastAccessTime;

	public int accessedSecondsAgo => (int)(Time.get_realtimeSinceStartup() - lastAccessTime);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ResourceContainer.OnRpcMessage", 0);
		try
		{
			if (rpc == 548378753 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StartLootingContainer "));
				}
				TimeWarning val2 = TimeWarning.New("StartLootingContainer", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(548378753u, "StartLootingContainer", GetBaseEntity(), player, 3f))
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
							StartLootingContainer(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in StartLootingContainer");
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

	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void StartLootingContainer(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && lootable && player.inventory.loot.StartLootingEntity(base.baseEntity))
		{
			lastAccessTime = Time.get_realtimeSinceStartup();
			player.inventory.loot.AddContainer(container);
		}
	}
}
