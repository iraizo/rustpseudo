using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class Candle : BaseCombatEntity, ISplashable, IIgniteable
{
	private float lifeTimeSeconds = 7200f;

	private float burnRate = 10f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Candle.OnRpcMessage", 0);
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

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetWantsOn(RPCMessage msg)
	{
		bool b = msg.read.Bit();
		SetFlag(Flags.On, b);
		UpdateInvokes();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		UpdateInvokes();
	}

	public void UpdateInvokes()
	{
		if (IsOn())
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)Burn, burnRate, burnRate, 1f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)Burn);
		}
	}

	public void Burn()
	{
		float num = burnRate / lifeTimeSeconds;
		Hurt(num * MaxHealth(), DamageType.Decay, this, useProtection: false);
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.damageTypes.Get(DamageType.Heat) > 0f && !IsOn())
		{
			SetFlag(Flags.On, b: true);
			UpdateInvokes();
		}
		base.OnAttacked(info);
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed && amount > 1)
		{
			return IsOn();
		}
		return false;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (amount > 1)
		{
			SetFlag(Flags.On, b: false);
			UpdateInvokes();
			amount--;
		}
		return amount;
	}

	public void Ignite()
	{
		SetFlag(Flags.On, b: true);
		UpdateInvokes();
	}

	public bool CanIgnite()
	{
		return !IsOn();
	}
}
