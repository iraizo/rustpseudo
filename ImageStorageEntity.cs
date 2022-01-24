using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class ImageStorageEntity : BaseEntity
{
	private struct ImageRequest
	{
		public IImageReceiver Receiver;

		public float Time;
	}

	private List<ImageRequest> _requests;

	protected virtual FileStorage.Type StorageType => FileStorage.Type.jpg;

	protected virtual uint CrcToLoad => 0u;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ImageStorageEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 652912521 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ImageRequested "));
				}
				TimeWarning val2 = TimeWarning.New("ImageRequested", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(652912521u, "ImageRequested", this, player, 3uL))
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
							ImageRequested(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ImageRequested");
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
	[RPC_Server.CallsPerSecond(3uL)]
	private void ImageRequested(RPCMessage msg)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)msg.player == (Object)null))
		{
			byte[] array = FileStorage.server.Get(CrcToLoad, StorageType, net.ID);
			if (array == null)
			{
				Debug.LogWarning((object)"Image entity has no image!");
				return;
			}
			SendInfo val = default(SendInfo);
			((SendInfo)(ref val))._002Ector(msg.connection);
			val.method = (SendMethod)0;
			val.channel = 2;
			SendInfo sendInfo = val;
			ClientRPCEx(sendInfo, null, "ReceiveImage", (uint)array.Length, array);
		}
	}
}
