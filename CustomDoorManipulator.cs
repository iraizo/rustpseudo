using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class CustomDoorManipulator : DoorManipulator
{
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CustomDoorManipulator.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1224330484 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoPair "));
				}
				TimeWarning val2 = TimeWarning.New("DoPair", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1224330484u, "DoPair", this, player, 3f))
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
							DoPair(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoPair");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3800726972u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerActionChange "));
				}
				TimeWarning val2 = TimeWarning.New("ServerActionChange", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3800726972u, "ServerActionChange", this, player, 3f))
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
							ServerActionChange(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ServerActionChange");
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

	public override bool PairWithLockedDoors()
	{
		return false;
	}

	public bool CanPlayerAdmin(BasePlayer player)
	{
		if ((Object)(object)player != (Object)null && player.CanBuild())
		{
			return !IsOn();
		}
		return false;
	}

	public bool IsPaired()
	{
		return (Object)(object)targetDoor != (Object)null;
	}

	public void RefreshDoor()
	{
		SetTargetDoor(targetDoor);
	}

	private void OnPhysicsNeighbourChanged()
	{
		SetTargetDoor(targetDoor);
		((FacepunchBehaviour)this).Invoke((Action)RefreshDoor, 0.1f);
	}

	public override void SetupInitialDoorConnection()
	{
		if (entityRef.IsValid(serverside: true) && (Object)(object)targetDoor == (Object)null)
		{
			SetTargetDoor(((Component)entityRef.Get(serverside: true)).GetComponent<Door>());
		}
	}

	public override void DoActionDoorMissing()
	{
		SetTargetDoor(null);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void DoPair(RPCMessage msg)
	{
		Door door = targetDoor;
		Door door2 = FindDoor(PairWithLockedDoors());
		if ((Object)(object)door2 != (Object)(object)door)
		{
			SetTargetDoor(door2);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ServerActionChange(RPCMessage msg)
	{
	}
}
