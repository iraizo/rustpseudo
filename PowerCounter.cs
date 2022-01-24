using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PowerCounter : IOEntity
{
	private int counterNumber;

	private int targetCounterNumber = 10;

	public Canvas canvas;

	public CanvasGroup screenAlpha;

	public Text screenText;

	public const Flags Flag_ShowPassthrough = Flags.Reserved2;

	public GameObjectRef counterConfigPanel;

	public Color passthroughColor;

	public Color counterColor;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PowerCounter.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3554226761u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_SetTarget "));
				}
				TimeWarning val2 = TimeWarning.New("SERVER_SetTarget", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3554226761u, "SERVER_SetTarget", this, player, 3f))
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
							SERVER_SetTarget(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SERVER_SetTarget");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3222475159u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ToggleDisplayMode "));
				}
				TimeWarning val2 = TimeWarning.New("ToggleDisplayMode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3222475159u, "ToggleDisplayMode", this, player, 3f))
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
							ToggleDisplayMode(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ToggleDisplayMode");
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

	public bool DisplayPassthrough()
	{
		return HasFlag(Flags.Reserved2);
	}

	public bool DisplayCounter()
	{
		return !DisplayPassthrough();
	}

	public bool CanPlayerAdmin(BasePlayer player)
	{
		if ((Object)(object)player != (Object)null)
		{
			return player.CanBuild();
		}
		return false;
	}

	public int GetTarget()
	{
		return targetCounterNumber;
	}

	public override void ResetState()
	{
		base.ResetState();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SERVER_SetTarget(RPCMessage msg)
	{
		if (CanPlayerAdmin(msg.player))
		{
			targetCounterNumber = msg.read.Int32();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ToggleDisplayMode(RPCMessage msg)
	{
		if (msg.player.CanBuild())
		{
			SetFlag(Flags.Reserved2, msg.read.Bit(), recursive: false, networkupdate: false);
			MarkDirty();
			SendNetworkUpdate();
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (DisplayPassthrough() || counterNumber >= targetCounterNumber)
		{
			return base.GetPassthroughAmount(outputSlot);
		}
		return 0;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = counterNumber;
		info.msg.ioEntity.genericInt2 = GetPassthroughAmount();
		info.msg.ioEntity.genericInt3 = GetTarget();
	}

	public void SetCounterNumber(int newNumber)
	{
		counterNumber = newNumber;
	}

	public override void SendIONetworkUpdate()
	{
		SendNetworkUpdate();
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (DisplayCounter() && inputAmount > 0 && inputSlot != 0)
		{
			int num = counterNumber;
			switch (inputSlot)
			{
			case 1:
				counterNumber++;
				break;
			case 2:
				counterNumber--;
				if (counterNumber < 0)
				{
					counterNumber = 0;
				}
				break;
			case 3:
				counterNumber = 0;
				break;
			}
			counterNumber = Mathf.Clamp(counterNumber, 0, 100);
			if (num != counterNumber)
			{
				MarkDirty();
				SendNetworkUpdate();
			}
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			if (base.isServer)
			{
				counterNumber = info.msg.ioEntity.genericInt1;
			}
			targetCounterNumber = info.msg.ioEntity.genericInt3;
		}
	}
}
