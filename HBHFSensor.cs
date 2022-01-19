using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class HBHFSensor : BaseDetector
{
	public GameObjectRef detectUp;

	public GameObjectRef detectDown;

	public const Flags Flag_IncludeOthers = Flags.Reserved2;

	public const Flags Flag_IncludeAuthed = Flags.Reserved3;

	private int detectedPlayers;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("HBHFSensor.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3206885720u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetIncludeAuth "));
				}
				TimeWarning val2 = TimeWarning.New("SetIncludeAuth", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3206885720u, "SetIncludeAuth", this, player, 3f))
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
							RPCMessage includeAuth = rPCMessage;
							SetIncludeAuth(includeAuth);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetIncludeAuth");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2223203375u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetIncludeOthers "));
				}
				TimeWarning val2 = TimeWarning.New("SetIncludeOthers", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2223203375u, "SetIncludeOthers", this, player, 3f))
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
							RPCMessage includeOthers = rPCMessage;
							SetIncludeOthers(includeOthers);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetIncludeOthers");
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

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Min(detectedPlayers, GetCurrentEnergy());
	}

	public override void OnObjects()
	{
		base.OnObjects();
		UpdatePassthroughAmount();
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdatePassthroughAmount, 0f, 1f, 0.1f);
	}

	public override void OnEmpty()
	{
		base.OnEmpty();
		UpdatePassthroughAmount();
		((FacepunchBehaviour)this).CancelInvoke((Action)UpdatePassthroughAmount);
	}

	public void UpdatePassthroughAmount()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		int num = detectedPlayers;
		detectedPlayers = 0;
		if (myTrigger.entityContents != null)
		{
			foreach (BaseEntity entityContent in myTrigger.entityContents)
			{
				if (!((Object)(object)entityContent == (Object)null) && entityContent.IsVisible(((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 0.1f, 10f))
				{
					BasePlayer component = ((Component)entityContent).GetComponent<BasePlayer>();
					bool flag = component.CanBuild();
					if ((!flag || ShouldIncludeAuthorized()) && (flag || ShouldIncludeOthers()) && (Object)(object)component != (Object)null && component.IsAlive() && !component.IsSleeping() && component.isServer)
					{
						detectedPlayers++;
					}
				}
			}
		}
		if (num != detectedPlayers && IsPowered())
		{
			MarkDirty();
			if (detectedPlayers > num)
			{
				Effect.server.Run(detectUp.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
			}
			else if (detectedPlayers < num)
			{
				Effect.server.Run(detectDown.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_up());
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetIncludeAuth(RPCMessage msg)
	{
		bool b = msg.read.Bit();
		if (msg.player.CanBuild() && IsPowered())
		{
			SetFlag(Flags.Reserved3, b);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetIncludeOthers(RPCMessage msg)
	{
		bool b = msg.read.Bit();
		if (msg.player.CanBuild() && IsPowered())
		{
			SetFlag(Flags.Reserved2, b);
		}
	}

	public bool ShouldIncludeAuthorized()
	{
		return HasFlag(Flags.Reserved3);
	}

	public bool ShouldIncludeOthers()
	{
		return HasFlag(Flags.Reserved2);
	}
}
