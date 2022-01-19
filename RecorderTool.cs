using System;
using System.IO;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class RecorderTool : ThrownWeapon, ICassettePlayer
{
	[ClientVar(Saved = true)]
	public static bool debugRecording;

	public AudioSource RecorderAudioSource;

	public SoundDefinition RecordStartSfx;

	public SoundDefinition RewindSfx;

	public SoundDefinition RecordFinishedSfx;

	public SoundDefinition PlayTapeSfx;

	public SoundDefinition StopTapeSfx;

	public float ThrowScale = 3f;

	public Cassette cachedCassette { get; private set; }

	public Sprite LoadedCassetteIcon
	{
		get
		{
			if (!((Object)(object)cachedCassette != (Object)null))
			{
				return null;
			}
			return cachedCassette.HudSprite;
		}
	}

	public BaseEntity ToBaseEntity => this;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RecorderTool.OnRpcMessage", 0);
		try
		{
			if (rpc == 3075830603u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_TogglePlaying "));
				}
				TimeWarning val2 = TimeWarning.New("Server_TogglePlaying", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(3075830603u, "Server_TogglePlaying", this, player))
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
							Server_TogglePlaying(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_TogglePlaying");
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

	private bool HasCassette()
	{
		return (Object)(object)cachedCassette != (Object)null;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void Server_TogglePlaying(RPCMessage msg)
	{
		bool b = ((Stream)(object)msg.read).ReadByte() == 1;
		SetFlag(Flags.On, b);
	}

	public void OnCassetteInserted(Cassette c)
	{
		cachedCassette = c;
		ClientRPC(null, "Client_OnCassetteInserted", c.net.ID);
	}

	public void OnCassetteRemoved(Cassette c)
	{
		cachedCassette = null;
		ClientRPC(null, "Client_OnCassetteRemoved");
	}

	protected override void SetUpThrownWeapon(BaseEntity ent)
	{
		base.SetUpThrownWeapon(ent);
		if ((Object)(object)GetOwnerPlayer() != (Object)null)
		{
			ent.OwnerID = GetOwnerPlayer().userID;
		}
		DeployedRecorder deployedRecorder;
		if ((Object)(object)cachedCassette != (Object)null && (deployedRecorder = ent as DeployedRecorder) != null)
		{
			GetItem().contents.itemList[0].SetParent(deployedRecorder.inventory);
		}
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (IsDisabled())
		{
			SetFlag(Flags.On, b: false);
		}
	}
}
