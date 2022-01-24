using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectableEasterEgg : BaseEntity
{
	public Transform artwork;

	public float bounceRange = 0.2f;

	public float bounceSpeed = 1f;

	public GameObjectRef pickupEffect;

	public ItemDefinition itemToGive;

	private float lastPickupStartTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CollectableEasterEgg.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 2436818324u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_PickUp "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_PickUp", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2436818324u, "RPC_PickUp", this, player, 3f))
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
							RPC_PickUp(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_PickUp");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2243088389u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StartPickUp "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_StartPickUp", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2243088389u, "RPC_StartPickUp", this, player, 3f))
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
							RPC_StartPickUp(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_StartPickUp");
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

	public override void ServerInit()
	{
		int num = Random.Range(0, 3);
		SetFlag(Flags.Reserved1, num == 0, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved2, num == 1, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved3, num == 2, recursive: false, networkupdate: false);
		base.ServerInit();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_StartPickUp(RPCMessage msg)
	{
		if (!((Object)(object)msg.player == (Object)null))
		{
			lastPickupStartTime = Time.get_realtimeSinceStartup();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_PickUp(RPCMessage msg)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)msg.player == (Object)null)
		{
			return;
		}
		float num = Time.get_realtimeSinceStartup() - lastPickupStartTime;
		if (!Object.op_Implicit((Object)(object)(msg.player.GetHeldEntity() as EasterBasket)) && (num > 2f || num < 0.8f))
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)EggHuntEvent.serverEvent))
		{
			if (!EggHuntEvent.serverEvent.IsEventActive())
			{
				return;
			}
			EggHuntEvent.serverEvent.EggCollected(msg.player);
			int iAmount = 1;
			msg.player.GiveItem(ItemManager.Create(itemToGive, iAmount, 0uL));
		}
		Effect.server.Run(pickupEffect.resourcePath, ((Component)this).get_transform().get_position() + Vector3.get_up() * 0.3f, Vector3.get_up());
		Kill();
	}
}
