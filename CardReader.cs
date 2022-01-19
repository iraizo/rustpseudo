using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class CardReader : IOEntity
{
	public float accessDuration = 10f;

	public int accessLevel;

	public GameObjectRef accessGrantedEffect;

	public GameObjectRef accessDeniedEffect;

	public GameObjectRef swipeEffect;

	public Transform audioPosition;

	public Flags AccessLevel1 = Flags.Reserved1;

	public Flags AccessLevel2 = Flags.Reserved2;

	public Flags AccessLevel3 = Flags.Reserved3;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CardReader.OnRpcMessage", 0);
		try
		{
			if (rpc == 979061374 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerCardSwiped "));
				}
				TimeWarning val2 = TimeWarning.New("ServerCardSwiped", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(979061374u, "ServerCardSwiped", this, player, 3f))
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
							ServerCardSwiped(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerCardSwiped");
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
		((FacepunchBehaviour)this).CancelInvoke((Action)GrantCard);
		((FacepunchBehaviour)this).CancelInvoke((Action)CancelAccess);
		CancelAccess();
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	public void CancelAccess()
	{
		SetFlag(Flags.On, b: false);
		MarkDirty();
	}

	public void FailCard()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(accessDeniedEffect.resourcePath, audioPosition.get_position(), Vector3.get_up());
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetFlag(AccessLevel1, accessLevel == 1);
		SetFlag(AccessLevel2, accessLevel == 2);
		SetFlag(AccessLevel3, accessLevel == 3);
	}

	public void GrantCard()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		SetFlag(Flags.On, b: true);
		MarkDirty();
		Effect.server.Run(accessGrantedEffect.resourcePath, audioPosition.get_position(), Vector3.get_up());
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ServerCardSwiped(RPCMessage msg)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!IsPowered() || Vector3Ex.Distance2D(((Component)msg.player).get_transform().get_position(), ((Component)this).get_transform().get_position()) > 1f || ((FacepunchBehaviour)this).IsInvoking((Action)GrantCard) || ((FacepunchBehaviour)this).IsInvoking((Action)FailCard))
		{
			return;
		}
		uint uid = msg.read.UInt32();
		Keycard keycard = BaseNetworkable.serverEntities.Find(uid) as Keycard;
		Effect.server.Run(swipeEffect.resourcePath, audioPosition.get_position(), Vector3.get_up(), msg.player.net.get_connection());
		if ((Object)(object)keycard != (Object)null)
		{
			Item item = keycard.GetItem();
			if (item != null && keycard.accessLevel == accessLevel && item.conditionNormalized > 0f)
			{
				((FacepunchBehaviour)this).Invoke((Action)GrantCard, 0.5f);
				item.LoseCondition(1f);
			}
			else
			{
				((FacepunchBehaviour)this).Invoke((Action)FailCard, 0.5f);
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = accessLevel;
		info.msg.ioEntity.genericFloat1 = accessDuration;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			accessLevel = info.msg.ioEntity.genericInt1;
			accessDuration = info.msg.ioEntity.genericFloat1;
		}
	}
}
