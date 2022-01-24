using System;
using System.IO;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class WorldNetworking
{
	private const int prefabsPerPacket = 100;

	private const int pathsPerPacket = 10;

	public static void OnMessageReceived(Message message)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected I4, but got Unknown
		WorldSerialization serialization = World.Serialization;
		WorldMessage val = WorldMessage.Deserialize((Stream)(object)message.get_read());
		try
		{
			MessageType status = val.status;
			switch (status - 1)
			{
			case 0:
				SendWorldData(message.connection);
				return;
			}
			if (val.prefabs != null)
			{
				serialization.world.prefabs.AddRange(val.prefabs);
				val.prefabs.Clear();
			}
			if (val.paths != null)
			{
				serialization.world.paths.AddRange(val.paths);
				val.paths.Clear();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void SendWorldData(Connection connection)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (connection.hasRequestedWorld)
		{
			DebugEx.LogWarning((object)$"{connection} requested world data more than once", (StackTraceLogType)0);
			return;
		}
		connection.hasRequestedWorld = true;
		WorldSerialization serialization = World.Serialization;
		WorldMessage data = Pool.Get<WorldMessage>();
		for (int i = 0; i < serialization.world.prefabs.Count; i++)
		{
			if (data.prefabs != null && data.prefabs.Count >= 100)
			{
				data.status = (MessageType)2;
				SendWorldData(connection, ref data);
				data = Pool.Get<WorldMessage>();
			}
			if (data.prefabs == null)
			{
				data.prefabs = Pool.GetList<PrefabData>();
			}
			data.prefabs.Add(serialization.world.prefabs[i]);
		}
		for (int j = 0; j < serialization.world.paths.Count; j++)
		{
			if (data.paths != null && data.paths.Count >= 10)
			{
				data.status = (MessageType)2;
				SendWorldData(connection, ref data);
				data = Pool.Get<WorldMessage>();
			}
			if (data.paths == null)
			{
				data.paths = Pool.GetList<PathData>();
			}
			data.paths.Add(serialization.world.paths[j]);
		}
		if (data != null)
		{
			data.status = (MessageType)3;
			SendWorldData(connection, ref data);
		}
	}

	private static void SendWorldData(Connection connection, ref WorldMessage data)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).get_write().Start())
		{
			((BaseNetwork)Net.sv).get_write().PacketID((Type)24);
			data.ToProto((Stream)(object)((BaseNetwork)Net.sv).get_write());
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
		}
		if (data.prefabs != null)
		{
			data.prefabs.Clear();
		}
		if (data.paths != null)
		{
			data.paths.Clear();
		}
		data.Dispose();
		data = null;
	}
}
