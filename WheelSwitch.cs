using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class WheelSwitch : IOEntity
{
	public Transform wheelObj;

	public float rotateSpeed = 90f;

	public Flags BeingRotated = Flags.Reserved1;

	public Flags RotatingLeft = Flags.Reserved2;

	public Flags RotatingRight = Flags.Reserved3;

	public float rotateProgress;

	public Animator animator;

	public float kineticEnergyPerSec = 1f;

	private BasePlayer rotatorPlayer;

	private float progressTickRate = 0.1f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WheelSwitch.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 2223603322u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - BeginRotate "));
				}
				TimeWarning val2 = TimeWarning.New("BeginRotate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2223603322u, "BeginRotate", this, player, 3f))
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
							BeginRotate(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BeginRotate");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 434251040 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - CancelRotate "));
				}
				TimeWarning val2 = TimeWarning.New("CancelRotate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(434251040u, "CancelRotate", this, player, 3f))
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
							CancelRotate(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in CancelRotate");
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
		CancelPlayerRotation();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void BeginRotate(RPCMessage msg)
	{
		if (!IsBeingRotated())
		{
			SetFlag(BeingRotated, b: true);
			rotatorPlayer = msg.player;
			((FacepunchBehaviour)this).InvokeRepeating((Action)RotateProgress, 0f, progressTickRate);
		}
	}

	public void CancelPlayerRotation()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)RotateProgress);
		SetFlag(BeingRotated, b: false);
		IOSlot[] array = outputs;
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				iOSlot.connectedTo.Get().IOInput(this, ioType, 0f, iOSlot.connectedToSlot);
			}
		}
		rotatorPlayer = null;
	}

	public void RotateProgress()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)rotatorPlayer) || rotatorPlayer.IsDead() || rotatorPlayer.IsSleeping() || Vector3Ex.Distance2D(((Component)rotatorPlayer).get_transform().get_position(), ((Component)this).get_transform().get_position()) > 2f)
		{
			CancelPlayerRotation();
			return;
		}
		float num = kineticEnergyPerSec * progressTickRate;
		IOSlot[] array = outputs;
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				num = iOSlot.connectedTo.Get().IOInput(this, ioType, num, iOSlot.connectedToSlot);
			}
		}
		if (num == 0f)
		{
			SetRotateProgress(rotateProgress + 0.1f);
		}
		SendNetworkUpdate();
	}

	public void SetRotateProgress(float newValue)
	{
		float num = rotateProgress;
		rotateProgress = newValue;
		SetFlag(Flags.Reserved4, num != newValue);
		SendNetworkUpdate();
		((FacepunchBehaviour)this).CancelInvoke((Action)StoppedRotatingCheck);
		((FacepunchBehaviour)this).Invoke((Action)StoppedRotatingCheck, 0.25f);
	}

	public void StoppedRotatingCheck()
	{
		SetFlag(Flags.Reserved4, b: false);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void CancelRotate(RPCMessage msg)
	{
		CancelPlayerRotation();
	}

	public void Powered()
	{
		float inputAmount = kineticEnergyPerSec * progressTickRate;
		IOSlot[] array = outputs;
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				inputAmount = iOSlot.connectedTo.Get().IOInput(this, ioType, inputAmount, iOSlot.connectedToSlot);
			}
		}
		SetRotateProgress(rotateProgress + 0.1f);
	}

	public override float IOInput(IOEntity from, IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount < 0f)
		{
			SetRotateProgress(rotateProgress + inputAmount);
			SendNetworkUpdate();
		}
		if (inputType == IOType.Electric && slot == 1)
		{
			if (inputAmount == 0f)
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)Powered);
			}
			else
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)Powered, 0f, progressTickRate);
			}
		}
		return Mathf.Clamp(inputAmount - 1f, 0f, inputAmount);
	}

	public bool IsBeingRotated()
	{
		return HasFlag(BeingRotated);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity != null)
		{
			rotateProgress = info.msg.sphereEntity.radius;
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<SphereEntity>();
		info.msg.sphereEntity.radius = rotateProgress;
	}
}
