using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class Detonator : HeldEntity, IRFObject
{
	public int frequency = 55;

	private float timeSinceDeploy;

	public GameObjectRef frequencyPanelPrefab;

	public GameObjectRef attackEffect;

	public GameObjectRef unAttackEffect;

	private float nextChangeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Detonator.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
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
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
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
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1106698135 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetPressed "));
				}
				TimeWarning val2 = TimeWarning.New("SetPressed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage pressed = rPCMessage;
						SetPressed(pressed);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in SetPressed");
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
	public void SetPressed(RPCMessage msg)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)msg.player == (Object)null) && !((Object)(object)msg.player != (Object)(object)GetOwnerPlayer()))
		{
			bool num = HasFlag(Flags.On);
			bool flag = msg.read.Bit();
			InternalSetPressed(flag);
			if (num != flag)
			{
				Effect.server.Run(flag ? attackEffect.resourcePath : unAttackEffect.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
			}
		}
	}

	internal void InternalSetPressed(bool pressed)
	{
		SetFlag(Flags.On, pressed);
		if (pressed)
		{
			RFManager.AddBroadcaster(frequency, this);
		}
		else
		{
			RFManager.RemoveBroadcaster(frequency, this);
		}
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

	public void RFSignalUpdate(bool on)
	{
	}

	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			InternalSetPressed(pressed: false);
		}
		base.SetHeld(bHeld);
	}

	[RPC_Server]
	public void ServerSetFrequency(RPCMessage msg)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		if ((Object)(object)msg.player == (Object)null || !msg.player.CanBuild() || (Object)(object)GetOwnerPlayer() != (Object)(object)msg.player || Time.get_time() < nextChangeTime)
		{
			return;
		}
		nextChangeTime = Time.get_time() + 2f;
		int newFrequency = msg.read.Int32();
		if (RFManager.IsReserved(newFrequency))
		{
			RFManager.ReserveErrorPrint(msg.player);
			return;
		}
		RFManager.ChangeFrequency(frequency, newFrequency, this, isListener: false, IsOn());
		frequency = newFrequency;
		SendNetworkUpdate();
		Item item = GetItem();
		if (item != null)
		{
			if (item.instanceData == null)
			{
				item.instanceData = new InstanceData();
				item.instanceData.ShouldPool = false;
			}
			item.instanceData.dataInt = frequency;
			item.MarkDirty();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<IOEntity>();
		}
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

	public int GetFrequency()
	{
		return frequency;
	}
}
