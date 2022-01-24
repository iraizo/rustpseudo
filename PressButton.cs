using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class PressButton : IOEntity
{
	public float pressDuration = 5f;

	public float pressPowerTime = 0.5f;

	public int pressPowerAmount = 2;

	public const Flags Flag_EmittingPower = Flags.Reserved3;

	public bool smallBurst;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PressButton.OnRpcMessage", 0);
		try
		{
			if (rpc == 3778543711u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Press "));
				}
				TimeWarning val2 = TimeWarning.New("Press", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3778543711u, "Press", this, player, 3f))
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
							Press(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Press");
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

	public override void ResetIOState()
	{
		base.ResetIOState();
		SetFlag(Flags.On, b: false);
		SetFlag(Flags.Reserved3, b: false);
		((FacepunchBehaviour)this).CancelInvoke((Action)Unpress);
		((FacepunchBehaviour)this).CancelInvoke((Action)UnpowerTime);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (IsOn())
		{
			if (HasFlag(Flags.Reserved3) && ((Object)(object)sourceItem != (Object)null || smallBurst))
			{
				return pressPowerAmount;
			}
			return base.GetPassthroughAmount();
		}
		return 0;
	}

	public void UnpowerTime()
	{
		SetFlag(Flags.Reserved3, b: false);
		MarkDirty();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.On, b: false);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void Press(RPCMessage msg)
	{
		if (!IsOn())
		{
			SetFlag(Flags.On, b: true);
			((FacepunchBehaviour)this).Invoke((Action)UnpowerTime, pressPowerTime);
			SetFlag(Flags.Reserved3, b: true);
			SendNetworkUpdateImmediate();
			MarkDirty();
			((FacepunchBehaviour)this).Invoke((Action)Unpress, pressDuration);
		}
	}

	public void Unpress()
	{
		SetFlag(Flags.On, b: false);
		MarkDirty();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = pressDuration;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			pressDuration = info.msg.ioEntity.genericFloat1;
		}
	}
}
