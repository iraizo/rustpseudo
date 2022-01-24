using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class SurveyCrater : BaseCombatEntity
{
	private ResourceDispenser resourceDispenser;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SurveyCrater.OnRpcMessage", 0);
		try
		{
			if (rpc == 3491246334u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AnalysisComplete "));
				}
				TimeWarning val2 = TimeWarning.New("AnalysisComplete", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg2 = rPCMessage;
						AnalysisComplete(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in AnalysisComplete");
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
		((FacepunchBehaviour)this).Invoke((Action)RemoveMe, 1800f);
	}

	public override void OnAttacked(HitInfo info)
	{
		_ = base.isServer;
		base.OnAttacked(info);
	}

	public void RemoveMe()
	{
		Kill();
	}

	[RPC_Server]
	public void AnalysisComplete(RPCMessage msg)
	{
	}

	public override float BoundsPadding()
	{
		return 2f;
	}
}
