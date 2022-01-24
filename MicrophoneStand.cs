using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

public class MicrophoneStand : BaseMountable
{
	public enum SpeechMode
	{
		Normal,
		HighPitch,
		LowPitch
	}

	public VoiceProcessor VoiceProcessor;

	public AudioSource VoiceSource;

	private SpeechMode currentSpeechMode;

	public AudioMixerGroup NormalMix;

	public AudioMixerGroup HighPitchMix;

	public AudioMixerGroup LowPitchMix;

	public Phrase NormalPhrase = new Phrase("microphone_normal", "Normal");

	public Phrase NormalDescPhrase = new Phrase("microphone_normal_desc", "No voice effect");

	public Phrase HighPitchPhrase = new Phrase("microphone_high", "High Pitch");

	public Phrase HighPitchDescPhrase = new Phrase("microphone_high_desc", "High pitch voice");

	public Phrase LowPitchPhrase = new Phrase("microphone_low", "Low");

	public Phrase LowPitchDescPhrase = new Phrase("microphone_low_desc", "Low pitch voice");

	public GameObjectRef IOSubEntity;

	public Transform IOSubEntitySpawnPos;

	public bool IsStatic;

	public EntityRef<IOEntity> ioEntity;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MicrophoneStand.OnRpcMessage", 0);
		try
		{
			if (rpc == 1420522459 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetMode "));
				}
				TimeWarning val2 = TimeWarning.New("SetMode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage mode = rPCMessage;
						SetMode(mode);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in SetMode");
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
	public void SetMode(RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)_mounted))
		{
			SpeechMode speechMode = (SpeechMode)msg.read.Int32();
			if (speechMode != currentSpeechMode)
			{
				currentSpeechMode = speechMode;
				SendNetworkUpdate();
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.microphoneStand == null)
		{
			info.msg.microphoneStand = Pool.Get<MicrophoneStand>();
		}
		info.msg.microphoneStand.microphoneMode = (int)currentSpeechMode;
		info.msg.microphoneStand.IORef = ioEntity.uid;
	}

	public void SpawnChildEntity()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		MicrophoneStandIOEntity microphoneStandIOEntity = GameManager.server.CreateEntity(IOSubEntity.resourcePath, IOSubEntitySpawnPos.get_localPosition(), IOSubEntitySpawnPos.get_localRotation()) as MicrophoneStandIOEntity;
		microphoneStandIOEntity.enableSaving = enableSaving;
		microphoneStandIOEntity.SetParent(this);
		microphoneStandIOEntity.Spawn();
		ioEntity.Set(microphoneStandIOEntity);
		SendNetworkUpdate();
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		SpawnChildEntity();
	}

	public override void PostMapEntitySpawn()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.PostMapEntitySpawn();
		if (!IsStatic)
		{
			return;
		}
		SpawnChildEntity();
		int num = 128;
		List<ConnectedSpeaker> list = Pool.GetList<ConnectedSpeaker>();
		GamePhysics.OverlapSphere<ConnectedSpeaker>(((Component)this).get_transform().get_position(), (float)num, list, 256, (QueryTriggerInteraction)1);
		IOEntity iOEntity = ioEntity.Get(serverside: true);
		List<MicrophoneStand> list2 = Pool.GetList<MicrophoneStand>();
		int num2 = 0;
		foreach (ConnectedSpeaker item in list)
		{
			bool flag = true;
			list2.Clear();
			GamePhysics.OverlapSphere<MicrophoneStand>(((Component)item).get_transform().get_position(), (float)num, list2, 256, (QueryTriggerInteraction)1);
			if (list2.Count > 1)
			{
				float num3 = Distance((BaseEntity)item);
				foreach (MicrophoneStand item2 in list2)
				{
					if (!item2.isClient && item2.Distance((BaseEntity)item) < num3)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				iOEntity.outputs[0].connectedTo.Set(item);
				item.inputs[0].connectedTo.Set(iOEntity);
				iOEntity = item;
				num2++;
			}
		}
		Pool.FreeList<ConnectedSpeaker>(ref list);
		Pool.FreeList<MicrophoneStand>(ref list2);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.microphoneStand != null)
		{
			currentSpeechMode = (SpeechMode)info.msg.microphoneStand.microphoneMode;
			ioEntity.uid = info.msg.microphoneStand.IORef;
		}
	}
}
