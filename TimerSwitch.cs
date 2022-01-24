using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class TimerSwitch : IOEntity
{
	public float timerLength = 10f;

	public Transform timerDrum;

	private float timePassed = -1f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TimerSwitch.OnRpcMessage", 0);
		try
		{
			if (rpc == 4167839872u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SVSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("SVSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4167839872u, "SVSwitch", this, player, 3f))
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
							SVSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
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
		if (((FacepunchBehaviour)this).IsInvoking((Action)AdvanceTime))
		{
			EndTimer();
		}
	}

	public override bool WantsPassthroughPower()
	{
		if (IsPowered())
		{
			return IsOn();
		}
		return false;
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!IsPowered() || !IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount();
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			SetFlag(Flags.Reserved8, inputAmount > 0, recursive: false, networkupdate: false);
		}
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		switch (inputSlot)
		{
		case 0:
			base.UpdateFromInput(inputAmount, inputSlot);
			if (!IsPowered() && ((FacepunchBehaviour)this).IsInvoking((Action)AdvanceTime))
			{
				EndTimer();
			}
			else if (timePassed != -1f)
			{
				SetFlag(Flags.On, b: false, recursive: false, networkupdate: false);
				SwitchPressed();
			}
			break;
		case 1:
			if (inputAmount > 0)
			{
				SwitchPressed();
			}
			break;
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SVSwitch(RPCMessage msg)
	{
		SwitchPressed();
	}

	public void SwitchPressed()
	{
		if (!IsOn() && IsPowered())
		{
			SetFlag(Flags.On, b: true);
			MarkDirty();
			((FacepunchBehaviour)this).InvokeRepeating((Action)AdvanceTime, 0f, 0.1f);
			SendNetworkUpdateImmediate();
		}
	}

	public void AdvanceTime()
	{
		if (timePassed < 0f)
		{
			timePassed = 0f;
		}
		timePassed += 0.1f;
		if (timePassed >= timerLength)
		{
			EndTimer();
		}
		else
		{
			SendNetworkUpdate();
		}
	}

	public void EndTimer()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)AdvanceTime);
		timePassed = -1f;
		SetFlag(Flags.On, b: false);
		SendNetworkUpdateImmediate();
		MarkDirty();
	}

	public float GetPassedTime()
	{
		return timePassed;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (timePassed == -1f)
		{
			if (IsOn())
			{
				SetFlag(Flags.On, b: false);
			}
		}
		else
		{
			SwitchPressed();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = GetPassedTime();
		info.msg.ioEntity.genericFloat2 = timerLength;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			timerLength = info.msg.ioEntity.genericFloat2;
			timePassed = info.msg.ioEntity.genericFloat1;
		}
	}
}
