using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricalBranch : IOEntity
{
	public int branchAmount = 2;

	public GameObjectRef branchPanelPrefab;

	private float nextChangeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ElectricalBranch.OnRpcMessage", 0);
		try
		{
			if (rpc == 643124146 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetBranchOffPower "));
				}
				TimeWarning val2 = TimeWarning.New("SetBranchOffPower", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(643124146u, "SetBranchOffPower", this, player, 3f))
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
							RPCMessage branchOffPower = rPCMessage;
							SetBranchOffPower(branchOffPower);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetBranchOffPower");
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
	public void SetBranchOffPower(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && player.CanBuild() && !(Time.get_time() < nextChangeTime))
		{
			nextChangeTime = Time.get_time() + 1f;
			int num = msg.read.Int32();
			num = (branchAmount = Mathf.Clamp(num, 2, 10000000));
			MarkDirtyForceUpdateOutputs();
			SendNetworkUpdate();
		}
	}

	public override bool AllowDrainFrom(int outputSlot)
	{
		if (outputSlot == 1)
		{
			return false;
		}
		return true;
	}

	public override int DesiredPower()
	{
		return branchAmount;
	}

	public void SetBranchAmount(int newAmount)
	{
		newAmount = Mathf.Clamp(newAmount, 2, 100000000);
		branchAmount = newAmount;
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return outputSlot switch
		{
			0 => Mathf.Clamp(GetCurrentEnergy() - branchAmount, 0, GetCurrentEnergy()), 
			1 => Mathf.Min(GetCurrentEnergy(), branchAmount), 
			_ => 0, 
		};
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = branchAmount;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			branchAmount = info.msg.ioEntity.genericInt1;
		}
	}
}
