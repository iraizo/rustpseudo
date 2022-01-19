using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class SprayCanSpray : BaseEntity, ISplashable
{
	public float Duration = 30f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SprayCanSpray.OnRpcMessage", 0);
		try
		{
			if (rpc == 2774110739u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestWaterClear "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestWaterClear", 0);
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
						Server_RequestWaterClear(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in Server_RequestWaterClear");
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
		((FacepunchBehaviour)this).Invoke((Action)Timeout, Duration);
	}

	private void Timeout()
	{
		Kill();
	}

	private void RainCheck()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (Climate.GetRain(((Component)this).get_transform().get_position()) > 0f && IsOutside())
		{
			Kill();
		}
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return amount > 0;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed)
		{
			Kill();
		}
		return 1;
	}

	[RPC_Server]
	private void Server_RequestWaterClear(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && Menu_WaterClear_ShowIf(player))
		{
			Kill();
		}
	}

	private bool Menu_WaterClear_ShowIf(BasePlayer player)
	{
		BaseLiquidVessel baseLiquidVessel;
		if ((Object)(object)player.GetHeldEntity() != (Object)null && (baseLiquidVessel = player.GetHeldEntity() as BaseLiquidVessel) != null)
		{
			return baseLiquidVessel.AmountHeld() > 0;
		}
		return false;
	}
}
