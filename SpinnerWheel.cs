using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SpinnerWheel : Signage
{
	public Transform wheel;

	public float velocity;

	public Quaternion targetRotation = Quaternion.get_identity();

	[Header("Sound")]
	public SoundDefinition spinLoopSoundDef;

	public SoundDefinition spinStartSoundDef;

	public SoundDefinition spinAccentSoundDef;

	public SoundDefinition spinStopSoundDef;

	public float minTimeBetweenSpinAccentSounds = 0.3f;

	public float spinAccentAngleDelta = 180f;

	private Sound spinSound;

	private SoundModulation.Modulator spinSoundGain;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SpinnerWheel.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3019675107u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_AnyoneSpin "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_AnyoneSpin", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3019675107u, "RPC_AnyoneSpin", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							RPC_AnyoneSpin(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_AnyoneSpin");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1455840454 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Spin "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Spin", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1455840454u, "RPC_Spin", this, player, 3f))
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
							RPCMessage rpc3 = rPCMessage;
							RPC_Spin(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Spin");
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

	public virtual bool AllowPlayerSpins()
	{
		return true;
	}

	public override void Save(SaveInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.spinnerWheel = Pool.Get<SpinnerWheel>();
		SpinnerWheel spinnerWheel = info.msg.spinnerWheel;
		Quaternion rotation = wheel.get_rotation();
		spinnerWheel.spin = ((Quaternion)(ref rotation)).get_eulerAngles();
	}

	public override void Load(LoadInfo info)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.spinnerWheel != null)
		{
			Quaternion rotation = Quaternion.Euler(info.msg.spinnerWheel.spin);
			if (base.isServer)
			{
				((Component)wheel).get_transform().set_rotation(rotation);
			}
		}
	}

	public virtual float GetMaxSpinSpeed()
	{
		return 720f;
	}

	public virtual void Update_Server()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (velocity > 0f)
		{
			float num = Mathf.Clamp(GetMaxSpinSpeed() * velocity, 0f, GetMaxSpinSpeed());
			velocity -= Time.get_deltaTime() * Mathf.Clamp(velocity / 2f, 0.1f, 1f);
			if (velocity < 0f)
			{
				velocity = 0f;
			}
			wheel.Rotate(Vector3.get_up(), num * Time.get_deltaTime(), (Space)1);
			SendNetworkUpdate();
		}
	}

	public void Update_Client()
	{
	}

	public void Update()
	{
		if (base.isClient)
		{
			Update_Client();
		}
		if (base.isServer)
		{
			Update_Server();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && AllowPlayerSpins() && (AnyoneSpin() || rpc.player.CanBuild()) && !(velocity > 15f))
		{
			velocity += Random.Range(4f, 7f);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_AnyoneSpin(RPCMessage rpc)
	{
		if (rpc.player.CanInteract())
		{
			SetFlag(Flags.Reserved3, rpc.read.Bit());
		}
	}

	public bool AnyoneSpin()
	{
		return HasFlag(Flags.Reserved3);
	}
}
