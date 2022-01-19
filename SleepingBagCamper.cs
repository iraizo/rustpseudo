using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SleepingBagCamper : SleepingBag
{
	public EntityRef<BaseVehicleSeat> AssociatedSeat;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SleepingBagCamper.OnRpcMessage", 0);
		try
		{
			if (rpc == 2177887503u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerClearBed "));
				}
				TimeWarning val2 = TimeWarning.New("ServerClearBed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2177887503u, "ServerClearBed", this, player, 3f))
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
							ServerClearBed(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerClearBed");
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
		SetFlag(Flags.Reserved3, b: true);
	}

	protected override void PostPlayerSpawn(BasePlayer p)
	{
		base.PostPlayerSpawn(p);
		BaseVehicleSeat baseVehicleSeat = AssociatedSeat.Get(base.isServer);
		if ((Object)(object)baseVehicleSeat != (Object)null)
		{
			if (p.IsConnected)
			{
				p.EndSleeping();
			}
			baseVehicleSeat.MountPlayer(p);
		}
	}

	public void SetSeat(BaseVehicleSeat seat, bool sendNetworkUpdate = false)
	{
		AssociatedSeat.Set(seat);
		if (sendNetworkUpdate)
		{
			SendNetworkUpdate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.sleepingBagCamper = Pool.Get<SleepingBagCamper>();
			info.msg.sleepingBagCamper.seatID = AssociatedSeat.uid;
		}
	}

	public override bool IsOccupied(ulong userID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (WaterLevel.Test(((Component)this).get_transform().get_position()))
		{
			return true;
		}
		if (AssociatedSeat.IsValid(base.isServer))
		{
			BasePlayer mounted = AssociatedSeat.Get(base.isServer).GetMounted();
			if ((Object)(object)mounted != (Object)null)
			{
				return mounted.userID != userID;
			}
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void ServerClearBed(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && AssociatedSeat.IsValid(base.isServer) && !((Object)(object)AssociatedSeat.Get(base.isServer).GetMounted() != (Object)(object)player))
		{
			deployerUserID = 0uL;
			SendNetworkUpdate();
		}
	}
}
