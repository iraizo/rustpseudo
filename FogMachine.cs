using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class FogMachine : StorageContainer
{
	public const Flags FogFieldOn = Flags.Reserved8;

	public const Flags MotionMode = Flags.Reserved7;

	public const Flags Emitting = Flags.Reserved6;

	public const Flags Flag_HasJuice = Flags.Reserved5;

	public float fogLength = 60f;

	public float nozzleBlastDuration = 5f;

	public float fuelPerSec = 1f;

	private float pendingFuel;

	public bool IsEmitting()
	{
		return HasFlag(Flags.Reserved6);
	}

	public bool HasJuice()
	{
		return HasFlag(Flags.Reserved5);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetFogOn(RPCMessage msg)
	{
		if (!IsEmitting() && !IsOn() && HasFuel() && msg.player.CanBuild())
		{
			SetFlag(Flags.On, b: true);
			((FacepunchBehaviour)this).InvokeRepeating((Action)StartFogging, 0f, fogLength - 1f);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetFogOff(RPCMessage msg)
	{
		if (IsOn() && msg.player.CanBuild())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)StartFogging);
			SetFlag(Flags.On, b: false);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetMotionDetection(RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (msg.player.CanBuild())
		{
			SetFlag(Flags.Reserved7, flag);
			if (flag)
			{
				SetFlag(Flags.On, b: false);
			}
			UpdateMotionMode();
		}
	}

	public void UpdateMotionMode()
	{
		if (HasFlag(Flags.Reserved7))
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)CheckTrigger, Random.Range(0f, 0.5f), 0.5f, 0.1f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)CheckTrigger);
		}
	}

	public void CheckTrigger()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!IsEmitting() && BasePlayer.AnyPlayersVisibleToEntity(((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 3f, 3f, this, ((Component)this).get_transform().get_position() + Vector3.get_up() * 0.1f, ignorePlayersWithPriv: true))
		{
			StartFogging();
		}
	}

	public void StartFogging()
	{
		if (!UseFuel(1f))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)StartFogging);
			SetFlag(Flags.On, b: false);
			return;
		}
		SetFlag(Flags.Reserved6, b: true);
		((FacepunchBehaviour)this).Invoke((Action)EnableFogField, 1f);
		((FacepunchBehaviour)this).Invoke((Action)DisableNozzle, nozzleBlastDuration);
		((FacepunchBehaviour)this).Invoke((Action)FinishFogging, fogLength);
	}

	public virtual void EnableFogField()
	{
		SetFlag(Flags.Reserved8, b: true);
	}

	public void DisableNozzle()
	{
		SetFlag(Flags.Reserved6, b: false);
	}

	public virtual void FinishFogging()
	{
		SetFlag(Flags.Reserved8, b: false);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Reserved8, b: false);
		SetFlag(Flags.Reserved6, b: false);
		SetFlag(Flags.Reserved5, HasFuel());
		if (IsOn())
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)StartFogging, 0f, fogLength - 1f);
		}
		UpdateMotionMode();
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		SetFlag(Flags.Reserved5, HasFuel());
		base.PlayerStoppedLooting(player);
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

	public bool HasFuel()
	{
		return GetFuelAmount() >= 1;
	}

	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		pendingFuel += seconds * fuelPerSec;
		if (pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(pendingFuel);
			slot.UseItem(num);
			pendingFuel -= num;
		}
		return true;
	}

	public virtual bool MotionModeEnabled()
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FogMachine.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 2788115565u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetFogOff "));
				}
				TimeWarning val2 = TimeWarning.New("SetFogOff", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2788115565u, "SetFogOff", this, player, 3f))
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
							RPCMessage fogOff = rPCMessage;
							SetFogOff(fogOff);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetFogOff");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3905831928u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetFogOn "));
				}
				TimeWarning val2 = TimeWarning.New("SetFogOn", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3905831928u, "SetFogOn", this, player, 3f))
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
							RPCMessage fogOn = rPCMessage;
							SetFogOn(fogOn);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetFogOn");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1773639087 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetMotionDetection "));
				}
				TimeWarning val2 = TimeWarning.New("SetMotionDetection", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1773639087u, "SetMotionDetection", this, player, 3f))
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
							RPCMessage motionDetection = rPCMessage;
							SetMotionDetection(motionDetection);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in SetMotionDetection");
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
}
