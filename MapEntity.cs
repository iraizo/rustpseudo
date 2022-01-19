using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class MapEntity : HeldEntity
{
	[NonSerialized]
	public uint[] fogImages = new uint[1];

	[NonSerialized]
	public uint[] paintImages = new uint[144];

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MapEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 1443560440 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ImageUpdate "));
				}
				TimeWarning val2 = TimeWarning.New("ImageUpdate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1443560440u, "ImageUpdate", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1443560440u, "ImageUpdate", this, player))
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
							ImageUpdate(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ImageUpdate");
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

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mapEntity != null)
		{
			if (info.msg.mapEntity.fogImages.Count == fogImages.Length)
			{
				fogImages = info.msg.mapEntity.fogImages.ToArray();
			}
			if (info.msg.mapEntity.paintImages.Count == paintImages.Length)
			{
				paintImages = info.msg.mapEntity.paintImages.ToArray();
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.mapEntity = Pool.Get<MapEntity>();
		info.msg.mapEntity.fogImages = Pool.Get<List<uint>>();
		info.msg.mapEntity.fogImages.AddRange(fogImages);
		info.msg.mapEntity.paintImages = Pool.Get<List<uint>>();
		info.msg.mapEntity.paintImages.AddRange(paintImages);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	[RPC_Server.FromOwner]
	public void ImageUpdate(RPCMessage msg)
	{
		if ((Object)(object)msg.player == (Object)null)
		{
			return;
		}
		byte b = msg.read.UInt8();
		byte b2 = msg.read.UInt8();
		uint num = msg.read.UInt32();
		if ((b == 0 && fogImages[b2] == num) || (b == 1 && paintImages[b2] == num))
		{
			return;
		}
		uint num2 = (uint)(b * 1000 + b2);
		byte[] array = msg.read.BytesWithSize(10485760u);
		if (array != null)
		{
			FileStorage.server.RemoveEntityNum(net.ID, num2);
			uint num3 = FileStorage.server.Store(array, FileStorage.Type.png, net.ID, num2);
			if (b == 0)
			{
				fogImages[b2] = num3;
			}
			if (b == 1)
			{
				paintImages[b2] = num3;
			}
			InvalidateNetworkCache();
		}
	}
}
