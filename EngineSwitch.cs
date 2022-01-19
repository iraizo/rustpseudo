using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class EngineSwitch : BaseEntity
{
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("EngineSwitch.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1249530220 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StartEngine "));
				}
				TimeWarning val2 = TimeWarning.New("StartEngine", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1249530220u, "StartEngine", this, player, 3f))
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
							StartEngine(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in StartEngine");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1739656243 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StopEngine "));
				}
				TimeWarning val2 = TimeWarning.New("StopEngine", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1739656243u, "StopEngine", this, player, 3f))
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
							StopEngine(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StopEngine");
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

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void StopEngine(RPCMessage msg)
	{
		MiningQuarry miningQuarry = GetParentEntity() as MiningQuarry;
		if (Object.op_Implicit((Object)(object)miningQuarry))
		{
			miningQuarry.EngineSwitch(isOn: false);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void StartEngine(RPCMessage msg)
	{
		MiningQuarry miningQuarry = GetParentEntity() as MiningQuarry;
		if (Object.op_Implicit((Object)(object)miningQuarry))
		{
			miningQuarry.EngineSwitch(isOn: true);
		}
	}
}
