using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioVisualisationEntity : IOEntity
{
	public enum LightColour
	{
		Red,
		Green,
		Blue,
		Yellow,
		Pink
	}

	public enum VolumeSensitivity
	{
		Small,
		Medium,
		Large
	}

	public enum Speed
	{
		Low,
		Medium,
		High
	}

	private EntityRef<BaseEntity> connectedTo;

	public GameObjectRef SettingsDialog;

	public LightColour currentColour { get; private set; }

	public VolumeSensitivity currentVolumeSensitivity { get; private set; } = VolumeSensitivity.Medium;


	public Speed currentSpeed { get; private set; } = Speed.Medium;


	public int currentGradient { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AudioVisualisationEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 4002266471u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerUpdateSettings "));
				}
				TimeWarning val2 = TimeWarning.New("ServerUpdateSettings", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4002266471u, "ServerUpdateSettings", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(4002266471u, "ServerUpdateSettings", this, player, 3f))
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
							ServerUpdateSettings(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerUpdateSettings");
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

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(Flags.Reserved8) != next.HasFlag(Flags.Reserved8) && next.HasFlag(Flags.Reserved8))
		{
			IOEntity audioSource = GetAudioSource(this, BoomBox.BacktrackLength);
			if ((Object)(object)audioSource != (Object)null)
			{
				ClientRPC(null, "Client_PlayAudioFrom", audioSource.net.ID);
			}
			connectedTo.Set(audioSource);
		}
	}

	private IOEntity GetAudioSource(IOEntity entity, int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		IOSlot[] array = entity.inputs;
		IAudioConnectionSource audioConnectionSource = default(IAudioConnectionSource);
		AudioVisualisationEntity audioVisualisationEntity = default(AudioVisualisationEntity);
		IAudioConnectionSource audioConnectionSource2 = default(IAudioConnectionSource);
		for (int i = 0; i < array.Length; i++)
		{
			IOEntity iOEntity = array[i].connectedTo.Get(base.isServer);
			if ((Object)(object)iOEntity == (Object)(object)this)
			{
				return null;
			}
			if ((Object)(object)iOEntity != (Object)null && ((Component)iOEntity).TryGetComponent<IAudioConnectionSource>(ref audioConnectionSource))
			{
				return iOEntity;
			}
			if ((Object)(object)iOEntity != (Object)null && ((Component)iOEntity).TryGetComponent<AudioVisualisationEntity>(ref audioVisualisationEntity) && audioVisualisationEntity.connectedTo.IsSet)
			{
				return audioVisualisationEntity.connectedTo.Get(base.isServer) as IOEntity;
			}
			if ((Object)(object)iOEntity != (Object)null)
			{
				iOEntity = GetAudioSource(iOEntity, depth - 1);
				if ((Object)(object)iOEntity != (Object)null && ((Component)iOEntity).TryGetComponent<IAudioConnectionSource>(ref audioConnectionSource2))
				{
					return iOEntity;
				}
			}
		}
		return null;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.connectedSpeaker == null)
		{
			info.msg.connectedSpeaker = Pool.Get<ConnectedSpeaker>();
		}
		info.msg.connectedSpeaker.connectedTo = connectedTo.uid;
		if (info.msg.audioEntity == null)
		{
			info.msg.audioEntity = Pool.Get<AudioEntity>();
		}
		info.msg.audioEntity.colourMode = (int)currentColour;
		info.msg.audioEntity.volumeRange = (int)currentVolumeSensitivity;
		info.msg.audioEntity.speed = (int)currentSpeed;
		info.msg.audioEntity.gradient = currentGradient;
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.IsVisible(3f)]
	public void ServerUpdateSettings(RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		int num4 = msg.read.Int32();
		if (currentColour != (LightColour)num || currentVolumeSensitivity != (VolumeSensitivity)num2 || currentSpeed != (Speed)num3 || currentGradient != num4)
		{
			currentColour = (LightColour)num;
			currentVolumeSensitivity = (VolumeSensitivity)num2;
			currentSpeed = (Speed)num3;
			currentGradient = num4;
			MarkDirty();
			SendNetworkUpdate();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.audioEntity != null)
		{
			currentColour = (LightColour)info.msg.audioEntity.colourMode;
			currentVolumeSensitivity = (VolumeSensitivity)info.msg.audioEntity.volumeRange;
			currentSpeed = (Speed)info.msg.audioEntity.speed;
			currentGradient = info.msg.audioEntity.gradient;
		}
		if (info.msg.connectedSpeaker != null)
		{
			connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}
}
