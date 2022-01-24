using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class Megaphone : HeldEntity
{
	[Header("Megaphone")]
	public VoiceProcessor voiceProcessor;

	public float VoiceDamageMinFrequency = 2f;

	public float VoiceDamageAmount = 1f;

	public AudioSource VoiceSource;

	public SoundDefinition StartBroadcastingSfx;

	public SoundDefinition StopBroadcastingSfx;

	[ReplicatedVar(Default = "100")]
	public static float MegaphoneVoiceRange { get; set; } = 100f;


	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Megaphone.OnRpcMessage", 0);
		try
		{
			if (rpc == 4196056309u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_ToggleBroadcasting "));
				}
				TimeWarning val2 = TimeWarning.New("Server_ToggleBroadcasting", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(4196056309u, "Server_ToggleBroadcasting", this, player))
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
							Server_ToggleBroadcasting(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_ToggleBroadcasting");
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

	private void UpdateItemCondition()
	{
		Item ownerItem = GetOwnerItem();
		if (ownerItem != null && ownerItem.hasCondition)
		{
			ownerItem.LoseCondition(VoiceDamageAmount);
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void Server_ToggleBroadcasting(RPCMessage msg)
	{
		bool flag = msg.read.Int8() == 1;
		SetFlag(Flags.On, flag);
		if (flag)
		{
			if (!((FacepunchBehaviour)this).IsInvoking((Action)UpdateItemCondition))
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateItemCondition, 0f, VoiceDamageMinFrequency);
			}
		}
		else if (((FacepunchBehaviour)this).IsInvoking((Action)UpdateItemCondition))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)UpdateItemCondition);
		}
	}
}
