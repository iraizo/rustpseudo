using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class PagerEntity : BaseEntity, IRFObject
{
	public static Flags Flag_Silent = Flags.Reserved1;

	private int frequency = 55;

	public float beepRepeat = 2f;

	public GameObjectRef pagerEffect;

	public GameObjectRef silentEffect;

	private float nextChangeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PagerEntity.OnRpcMessage", 0);
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

	public override void SwitchParent(BaseEntity ent)
	{
		SetParent(ent, worldPositionStays: false, sendImmediate: true);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		RFManager.AddListener(frequency, this);
	}

	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(frequency, this);
		base.DoServerDestroy();
	}

	public Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position();
	}

	public float GetMaxRange()
	{
		return float.PositiveInfinity;
	}

	public void RFSignalUpdate(bool on)
	{
		if (!base.IsDestroyed)
		{
			bool flag = IsOn();
			if (on != flag)
			{
				SetFlag(Flags.On, on);
				SendNetworkUpdate();
			}
		}
	}

	public void SetSilentMode(bool wantsSilent)
	{
		SetFlag(Flag_Silent, wantsSilent);
	}

	public void SetOff()
	{
		SetFlag(Flags.On, b: false);
	}

	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(frequency, newFreq, this, isListener: true);
		frequency = newFreq;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(RPCMessage msg)
	{
		if (!((Object)(object)msg.player == (Object)null) && msg.player.CanBuild() && !(Time.get_time() < nextChangeTime))
		{
			nextChangeTime = Time.get_time() + 2f;
			int newFrequency = msg.read.Int32();
			RFManager.ChangeFrequency(frequency, newFrequency, this, isListener: true);
			frequency = newFrequency;
			SendNetworkUpdateImmediate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.genericInt1 = frequency;
	}

	internal override void OnParentRemoved()
	{
		SetParent(null, worldPositionStays: false, sendImmediate: true);
	}

	public void OnParentDestroying()
	{
		if (base.isServer)
		{
			((Component)this).get_transform().set_parent((Transform)null);
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			frequency = info.msg.ioEntity.genericInt1;
		}
		if (base.isServer && info.fromDisk)
		{
			ChangeFrequency(frequency);
		}
	}
}
