using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class DieselEngine : StorageContainer
{
	public GameObjectRef rumbleEffect;

	public Transform rumbleOrigin;

	public const Flags Flag_HasFuel = Flags.Reserved3;

	public float runningTimePerFuelUnit = 120f;

	private float cachedFuelTime;

	private const float rumbleMaxDistSq = 100f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("DieselEngine.OnRpcMessage", 0);
		try
		{
			if (rpc == 578721460 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - EngineSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("EngineSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(578721460u, "EngineSwitch", this, player, 6f))
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
							EngineSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in EngineSwitch");
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

	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		return base.CanOpenLootPanel(player, panelName);
	}

	public void FixedUpdate()
	{
		if (!base.isClient && IsOn())
		{
			if (cachedFuelTime <= Time.get_fixedDeltaTime() && ConsumeFuelItem())
			{
				cachedFuelTime += runningTimePerFuelUnit;
			}
			cachedFuelTime -= Time.get_fixedDeltaTime();
			if (cachedFuelTime <= 0f)
			{
				cachedFuelTime = 0f;
				EngineOff();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(6f)]
	public void EngineSwitch(RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			if (GetFuelAmount() > 0)
			{
				EngineOn();
			}
		}
		else
		{
			EngineOff();
		}
	}

	public void TimedShutdown()
	{
		EngineOff();
	}

	public bool ConsumeFuelItem(int amount = 1)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < amount)
		{
			return false;
		}
		slot.UseItem(amount);
		UpdateHasFuelFlag();
		return true;
	}

	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	public void UpdateHasFuelFlag()
	{
		SetFlag(Flags.Reserved3, GetFuelAmount() > 0);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		UpdateHasFuelFlag();
	}

	public void EngineOff()
	{
		SetFlag(Flags.On, b: false);
		BroadcastEntityMessage("DieselEngineOff");
	}

	public void EngineOn()
	{
		SetFlag(Flags.On, b: true);
		BroadcastEntityMessage("DieselEngineOn");
	}

	public void RescheduleEngineShutdown()
	{
		float num = 120f;
		((FacepunchBehaviour)this).Invoke((Action)TimedShutdown, num);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (IsOn())
		{
			BroadcastEntityMessage("DieselEngineOn");
		}
		else
		{
			BroadcastEntityMessage("DieselEngineOff");
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.genericFloat1 = cachedFuelTime;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			cachedFuelTime = info.msg.ioEntity.genericFloat1;
		}
	}

	public bool HasFuel()
	{
		return HasFlag(Flags.Reserved3);
	}
}
