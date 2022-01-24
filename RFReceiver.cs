using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class RFReceiver : IOEntity, IRFObject
{
	public int frequency;

	public GameObjectRef frequencyPanelPrefab;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RFReceiver.OnRpcMessage", 0);
		try
		{
			if (rpc == 2778616053u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerSetFrequency "));
				}
				TimeWarning val2 = TimeWarning.New("ServerSetFrequency", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2778616053u, "ServerSetFrequency", this, player, 3f))
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
							ServerSetFrequency(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerSetFrequency");
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

	public int GetFrequency()
	{
		return frequency;
	}

	public override bool WantsPower()
	{
		return IsOn();
	}

	public override void ResetIOState()
	{
		SetFlag(Flags.On, b: false);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!IsOn())
		{
			return 0;
		}
		return GetCurrentEnergy();
	}

	public Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position();
	}

	public float GetMaxRange()
	{
		return 100000f;
	}

	public override void Init()
	{
		base.Init();
		RFManager.AddListener(frequency, this);
	}

	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(frequency, this);
		base.DoServerDestroy();
	}

	public void RFSignalUpdate(bool on)
	{
		if (!base.IsDestroyed && IsOn() != on)
		{
			SetFlag(Flags.On, on);
			SendNetworkUpdateImmediate();
			MarkDirty();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(RPCMessage msg)
	{
		if (!((Object)(object)msg.player == (Object)null) && msg.player.CanBuild())
		{
			int newFrequency = msg.read.Int32();
			RFManager.ChangeFrequency(frequency, newFrequency, this, isListener: true);
			frequency = newFrequency;
			MarkDirty();
			SendNetworkUpdate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = frequency;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			frequency = info.msg.ioEntity.genericInt1;
		}
	}
}
