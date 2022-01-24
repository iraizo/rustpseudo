using System;
using System.Collections.Generic;
using System.IO;
using ConVar;
using EasyAntiCheat.Server;
using EasyAntiCheat.Server.Cerberus;
using EasyAntiCheat.Server.Hydra;
using EasyAntiCheat.Server.Scout;
using Network;
using UnityEngine;

public static class EACServer
{
	public static ICerberus<Client> playerTracker;

	public static Scout eacScout;

	private static Dictionary<Client, Connection> client2connection = new Dictionary<Client, Connection>();

	private static Dictionary<Connection, Client> connection2client = new Dictionary<Connection, Client>();

	private static Dictionary<Connection, ClientStatus> connection2status = new Dictionary<Connection, ClientStatus>();

	private static EasyAntiCheatServer<Client> easyAntiCheat = null;

	public static void Encrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (easyAntiCheat != null)
		{
			easyAntiCheat.get_NetProtect().ProtectMessage(GetClient(connection), src, (long)srcOffset, dst, (long)dstOffset);
		}
	}

	public static void Decrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (easyAntiCheat != null)
		{
			easyAntiCheat.get_NetProtect().UnprotectMessage(GetClient(connection), src, (long)srcOffset, dst, (long)dstOffset);
		}
	}

	public static Client GetClient(Connection connection)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		connection2client.TryGetValue(connection, out var value);
		return value;
	}

	public static Connection GetConnection(Client client)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		client2connection.TryGetValue(client, out var value);
		return value;
	}

	public static bool IsAuthenticated(Connection connection)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		connection2status.TryGetValue(connection, out var value);
		return (int)value == 5;
	}

	private static void OnAuthenticatedLocal(Connection connection)
	{
		if (connection.authStatus == string.Empty)
		{
			connection.authStatus = "ok";
		}
		connection2status[connection] = (ClientStatus)2;
	}

	private static void OnAuthenticatedRemote(Connection connection)
	{
		connection2status[connection] = (ClientStatus)5;
	}

	public static bool ShouldIgnore(Connection connection)
	{
		if (connection.authLevel >= 3)
		{
			return true;
		}
		return false;
	}

	private static void HandleClientUpdate(ClientStatusUpdate<Client> clientStatus)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Invalid comparison between Unknown and I4
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Invalid comparison between Unknown and I4
		TimeWarning val = TimeWarning.New("AntiCheatKickPlayer", 10);
		try
		{
			Client client = clientStatus.get_Client();
			Connection connection = GetConnection(client);
			if (connection == null)
			{
				Debug.LogError((object)("EAC status update for invalid client: " + ((Client)(ref client)).get_ClientID()));
			}
			else
			{
				if (ShouldIgnore(connection))
				{
					return;
				}
				if (clientStatus.get_RequiresKick())
				{
					string text = clientStatus.get_Message();
					if (string.IsNullOrEmpty(text))
					{
						ClientStatus status = clientStatus.get_Status();
						text = ((object)(ClientStatus)(ref status)).ToString();
					}
					Debug.Log((object)$"[EAC] Kicking {connection.userid} / {connection.username} ({text})");
					connection.authStatus = "eac";
					Net.sv.Kick(connection, "EAC: " + text, false);
					DateTime? dateTime = default(DateTime?);
					if (clientStatus.IsBanned(ref dateTime))
					{
						connection.authStatus = "eacbanned";
						ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Kicking " + connection.username + " (banned by anticheat)");
						if (!dateTime.HasValue)
						{
							Entity.DeleteBy(connection.userid);
						}
					}
					easyAntiCheat.UnregisterClient(client);
					client2connection.Remove(client);
					connection2client.Remove(connection);
					connection2status.Remove(connection);
				}
				else if ((int)clientStatus.get_Status() == 2)
				{
					OnAuthenticatedLocal(connection);
					easyAntiCheat.SetClientNetworkState(client, false);
				}
				else if ((int)clientStatus.get_Status() == 5)
				{
					OnAuthenticatedRemote(connection);
				}
				return;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void SendToClient(Client client, byte[] message, int messageLength)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Connection connection = GetConnection(client);
		if (connection == null)
		{
			Debug.LogError((object)("EAC network packet for invalid client: " + ((Client)(ref client)).get_ClientID()));
		}
		else if (((BaseNetwork)Net.sv).get_write().Start())
		{
			((BaseNetwork)Net.sv).get_write().PacketID((Type)22);
			((BaseNetwork)Net.sv).get_write().UInt32((uint)messageLength);
			((Stream)(object)((BaseNetwork)Net.sv).get_write()).Write(message, 0, messageLength);
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(connection));
		}
	}

	public static void DoStartup()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		if (Server.secure)
		{
			client2connection.Clear();
			connection2client.Clear();
			connection2status.Clear();
			Log.SetOut((TextWriter)new StreamWriter(Server.rootFolder + "/Log.EAC.txt", append: false)
			{
				AutoFlush = true
			});
			Log.set_Prefix("");
			Log.set_Level((LogLevel)3);
			easyAntiCheat = new EasyAntiCheatServer<Client>((ClientStatusHandler<Client>)HandleClientUpdate, 20, Server.hostname);
			playerTracker = easyAntiCheat.get_Cerberus();
			playerTracker.LogGameRoundStart(World.Name, string.Empty, 0);
			eacScout = new Scout();
		}
	}

	public static void DoUpdate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (easyAntiCheat == null)
		{
			return;
		}
		easyAntiCheat.HandleClientUpdates();
		if (Net.sv != null && Net.sv.IsConnected())
		{
			Client client = default(Client);
			byte[] message = default(byte[]);
			int messageLength = default(int);
			while (easyAntiCheat.PopNetworkMessage(ref client, ref message, ref messageLength))
			{
				SendToClient(client, message, messageLength);
			}
		}
	}

	public static void DoShutdown()
	{
		client2connection.Clear();
		connection2client.Clear();
		connection2status.Clear();
		if (eacScout != null)
		{
			Debug.Log((object)"EasyAntiCheat Scout Shutting Down");
			eacScout.Dispose();
			eacScout = null;
		}
		if (easyAntiCheat != null)
		{
			Debug.Log((object)"EasyAntiCheat Server Shutting Down");
			easyAntiCheat.Dispose();
			easyAntiCheat = null;
		}
	}

	public static void OnLeaveGame(Connection connection)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (easyAntiCheat != null)
		{
			Client client = GetClient(connection);
			easyAntiCheat.UnregisterClient(client);
			client2connection.Remove(client);
			connection2client.Remove(connection);
			connection2status.Remove(connection);
		}
	}

	public static void OnJoinGame(Connection connection)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (easyAntiCheat != null)
		{
			Client val = easyAntiCheat.GenerateCompatibilityClient();
			easyAntiCheat.RegisterClient(val, connection.userid.ToString(), connection.ipaddress, connection.ownerid.ToString(), connection.username, (PlayerRegisterFlags)((connection.authLevel != 0) ? 1 : 0));
			client2connection.Add(val, connection);
			connection2client.Add(connection, val);
			connection2status.Add(connection, (ClientStatus)0);
			if (ShouldIgnore(connection))
			{
				OnAuthenticatedLocal(connection);
				OnAuthenticatedRemote(connection);
			}
		}
		else
		{
			OnAuthenticatedLocal(connection);
			OnAuthenticatedRemote(connection);
		}
	}

	public static void OnStartLoading(Connection connection)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (easyAntiCheat != null)
		{
			Client client = GetClient(connection);
			easyAntiCheat.SetClientNetworkState(client, false);
		}
	}

	public static void OnFinishLoading(Connection connection)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (easyAntiCheat != null)
		{
			Client client = GetClient(connection);
			easyAntiCheat.SetClientNetworkState(client, true);
		}
	}

	public static void OnMessageReceived(Message message)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!connection2client.ContainsKey(message.connection))
		{
			Debug.LogError((object)("EAC network packet from invalid connection: " + message.connection.userid));
			return;
		}
		Client client = GetClient(message.connection);
		byte[] array = default(byte[]);
		int num = default(int);
		if (message.get_read().TemporaryBytesWithSize(ref array, ref num))
		{
			easyAntiCheat.PushNetworkMessage(client, array, num);
		}
	}
}
