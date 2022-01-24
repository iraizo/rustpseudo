using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class RHIB : MotorRowboat
{
	public GameObject steeringWheel;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float rhibpopulation;

	private float targetGasPedal;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RHIB.OnRpcMessage", 0);
		try
		{
			if (rpc == 1382282393 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_Release "));
				}
				TimeWarning val2 = TimeWarning.New("Server_Release", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1382282393u, "Server_Release", this, player, 6f))
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
							Server_Release(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_Release");
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
	[RPC_Server.IsVisible(6f)]
	public void Server_Release(RPCMessage msg)
	{
		if (!((Object)(object)GetParentEntity() == (Object)null))
		{
			SetParent(null, worldPositionStays: true, sendImmediate: true);
			rigidBody.set_isKinematic(false);
		}
	}

	public override void VehicleFixedUpdate()
	{
		gasPedal = Mathf.MoveTowards(gasPedal, targetGasPedal, Time.get_fixedDeltaTime() * 1f);
		base.VehicleFixedUpdate();
	}

	public override bool EngineOn()
	{
		return base.EngineOn();
	}

	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		base.DriverInput(inputState, player);
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			targetGasPedal = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			targetGasPedal = -0.5f;
		}
		else
		{
			targetGasPedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			steering = 1f;
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			steering = -1f;
		}
		else
		{
			steering = 0f;
		}
	}

	public void AddFuel(int amount)
	{
		StorageContainer storageContainer = fuelSystem.fuelStorageInstance.Get(serverside: true);
		if (Object.op_Implicit((Object)(object)storageContainer))
		{
			((Component)storageContainer).GetComponent<StorageContainer>().inventory.AddItem(ItemManager.FindItemDefinition("lowgradefuel"), amount, 0uL);
		}
	}
}
