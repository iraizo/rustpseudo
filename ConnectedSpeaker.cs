using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class ConnectedSpeaker : IOEntity
{
	public AudioSource SoundSource;

	private EntityRef<IOEntity> connectedTo;

	public VoiceProcessor VoiceProcessor;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ConnectedSpeaker.OnRpcMessage", 0);
		try
		{
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
		if (!base.isServer || old.HasFlag(Flags.Reserved8) == next.HasFlag(Flags.Reserved8))
		{
			return;
		}
		if (next.HasFlag(Flags.Reserved8))
		{
			IAudioConnectionSource connectionSource = GetConnectionSource(this, BoomBox.BacktrackLength);
			if (connectionSource != null)
			{
				ClientRPC(null, "Client_PlayAudioFrom", connectionSource.ToEntity().net.ID);
				connectedTo.Set(connectionSource.ToEntity());
			}
		}
		else if (connectedTo.IsSet)
		{
			ClientRPC(null, "Client_StopPlayingAudio");
			connectedTo.Set(null);
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.connectedSpeaker != null)
		{
			connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}

	private IAudioConnectionSource GetConnectionSource(IOEntity entity, int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		IOSlot[] array = entity.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			IOEntity iOEntity = array[i].connectedTo.Get(base.isServer);
			if ((Object)(object)iOEntity == (Object)(object)this)
			{
				return null;
			}
			IAudioConnectionSource result;
			if ((Object)(object)iOEntity != (Object)null && (result = iOEntity as IAudioConnectionSource) != null)
			{
				return result;
			}
			if ((Object)(object)iOEntity != (Object)null)
			{
				IAudioConnectionSource connectionSource = GetConnectionSource(iOEntity, depth - 1);
				if (connectionSource != null)
				{
					return connectionSource;
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
	}
}
