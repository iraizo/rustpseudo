using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class DoorCloser : BaseEntity
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	public float delay = 3f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("DoorCloser.OnRpcMessage", 0);
		try
		{
			if (rpc == 342802563 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Take "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Take", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(342802563u, "RPC_Take", this, player, 3f))
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
							RPC_Take(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Take");
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

	public override float BoundsPadding()
	{
		return 1f;
	}

	public void Think()
	{
		((FacepunchBehaviour)this).Invoke((Action)SendClose, delay);
	}

	public void SendClose()
	{
		BaseEntity baseEntity = GetParentEntity();
		if (children != null)
		{
			foreach (BaseEntity child in children)
			{
				if ((Object)(object)child != (Object)null)
				{
					((FacepunchBehaviour)this).Invoke((Action)SendClose, delay);
					return;
				}
			}
		}
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			((Component)baseEntity).SendMessage("CloseRequest");
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Take(RPCMessage rpc)
	{
		if (!rpc.player.CanInteract() || !rpc.player.CanBuild())
		{
			return;
		}
		Door door = GetDoor();
		if (!((Object)(object)door == (Object)null) && door.GetPlayerLockPermission(rpc.player))
		{
			Item item = ItemManager.Create(itemType, 1, skinID);
			if (item != null)
			{
				rpc.player.GiveItem(item);
			}
			Kill();
		}
	}

	public Door GetDoor()
	{
		return GetParentEntity() as Door;
	}
}
