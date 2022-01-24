using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class SpookySpeaker : BaseCombatEntity
{
	public SoundPlayer soundPlayer;

	public float soundSpacing = 12f;

	public float soundSpacingRand = 5f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SpookySpeaker.OnRpcMessage", 0);
		try
		{
			if (rpc == 2523893445u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetWantsOn "));
				}
				TimeWarning val2 = TimeWarning.New("SetWantsOn", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2523893445u, "SetWantsOn", this, player, 3f))
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
							RPCMessage wantsOn = rPCMessage;
							SetWantsOn(wantsOn);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetWantsOn");
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

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		UpdateInvokes();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetWantsOn(RPCMessage msg)
	{
		bool b = msg.read.Bit();
		SetFlag(Flags.On, b);
		UpdateInvokes();
	}

	public void UpdateInvokes()
	{
		if (IsOn())
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)SendPlaySound, soundSpacing, soundSpacing, soundSpacingRand);
			((FacepunchBehaviour)this).Invoke((Action)DelayedOff, 7200f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)SendPlaySound);
			((FacepunchBehaviour)this).CancelInvoke((Action)DelayedOff);
		}
	}

	public void SendPlaySound()
	{
		ClientRPC(null, "PlaySpookySound");
	}

	public void DelayedOff()
	{
		SetFlag(Flags.On, b: false);
	}
}
