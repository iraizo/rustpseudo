using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class MedicalTool : AttackEntity
{
	public float healDurationSelf = 4f;

	public float healDurationOther = 4f;

	public float healDurationOtherWounded = 7f;

	public float maxDistanceOther = 2f;

	public bool canUseOnOther = true;

	public bool canRevive = true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MedicalTool.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 789049461 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UseOther "));
				}
				TimeWarning val2 = TimeWarning.New("UseOther", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(789049461u, "UseOther", this, player))
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
							UseOther(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in UseOther");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2918424470u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UseSelf "));
				}
				TimeWarning val2 = TimeWarning.New("UseSelf", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(2918424470u, "UseSelf", this, player))
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
							UseSelf(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in UseSelf");
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
	[RPC_Server.IsActiveItem]
	private void UseOther(RPCMessage msg)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
		}
		else if (player.CanInteract() && HasItemAmount() && canUseOnOther)
		{
			BasePlayer basePlayer = BaseNetworkable.serverEntities.Find(msg.read.UInt32()) as BasePlayer;
			if ((Object)(object)basePlayer != (Object)null && Vector3.Distance(((Component)basePlayer).get_transform().get_position(), ((Component)player).get_transform().get_position()) < 4f)
			{
				ClientRPCPlayer(null, player, "Reset");
				GiveEffectsTo(basePlayer);
				UseItemAmount(1);
				StartAttackCooldown(repeatDelay);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void UseSelf(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
		}
		else if (player.CanInteract() && HasItemAmount())
		{
			ClientRPCPlayer(null, player, "Reset");
			GiveEffectsTo(player);
			UseItemAmount(1);
			StartAttackCooldown(repeatDelay);
		}
	}

	public override void ServerUse()
	{
		if (!base.isClient)
		{
			BasePlayer ownerPlayer = GetOwnerPlayer();
			if (!((Object)(object)ownerPlayer == (Object)null) && ownerPlayer.CanInteract() && HasItemAmount())
			{
				GiveEffectsTo(ownerPlayer);
				UseItemAmount(1);
				StartAttackCooldown(repeatDelay);
				SignalBroadcast(Signal.Attack, string.Empty);
			}
		}
	}

	private void GiveEffectsTo(BasePlayer player)
	{
		if (!Object.op_Implicit((Object)(object)player))
		{
			return;
		}
		ItemModConsumable component = ((Component)GetOwnerItemDefinition()).GetComponent<ItemModConsumable>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			Debug.LogWarning((object)("No consumable for medicaltool :" + ((Object)this).get_name()));
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)player != (Object)(object)ownerPlayer && player.IsWounded() && canRevive)
		{
			player.StopWounded(ownerPlayer);
		}
		foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
		{
			if (effect.type == MetabolismAttribute.Type.Health)
			{
				player.health += effect.amount;
			}
			else
			{
				player.metabolism.ApplyChange(effect.type, effect.amount, effect.time);
			}
		}
		if (player is BasePet)
		{
			player.SendNetworkUpdateImmediate();
		}
	}
}
