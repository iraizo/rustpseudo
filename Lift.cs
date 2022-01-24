using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class Lift : AnimatedBuildingBlock
{
	public GameObjectRef triggerPrefab;

	public string triggerBone;

	public float resetDelay = 5f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Lift.OnRpcMessage", 0);
		try
		{
			if (rpc == 2657791441u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_UseLift "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_UseLift", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2657791441u, "RPC_UseLift", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							RPC_UseLift(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_UseLift");
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
	[RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(RPCMessage rpc)
	{
		if (rpc.player.CanInteract())
		{
			MoveUp();
		}
	}

	private void MoveUp()
	{
		if (!IsOpen() && !IsBusy())
		{
			SetFlag(Flags.Open, b: true);
			SendNetworkUpdateImmediate();
		}
	}

	private void MoveDown()
	{
		if (IsOpen() && !IsBusy())
		{
			SetFlag(Flags.Open, b: false);
			SendNetworkUpdateImmediate();
		}
	}

	protected override void OnAnimatorDisabled()
	{
		if (base.isServer && IsOpen())
		{
			((FacepunchBehaviour)this).Invoke((Action)MoveDown, resetDelay);
		}
	}

	public override void Spawn()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(triggerPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity());
			baseEntity.Spawn();
			baseEntity.SetParent(this, triggerBone);
		}
	}
}
