using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

public static class ConsoleNetwork
{
	internal static void Init()
	{
	}

	internal static void OnClientCommand(Message packet)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (packet.get_read().get_Unread() > Server.maxpacketsize_command)
		{
			Debug.LogWarning((object)"Dropping client command due to size");
			return;
		}
		string text = packet.get_read().StringRaw(1048576u);
		if (packet.connection == null || !packet.connection.connected)
		{
			Debug.LogWarning((object)("Client without connection tried to run command: " + text));
			return;
		}
		Option val = Option.get_Server();
		val = ((Option)(ref val)).FromConnection(packet.connection);
		string text2 = ConsoleSystem.Run(((Option)(ref val)).Quiet(), text, Array.Empty<object>());
		if (!string.IsNullOrEmpty(text2))
		{
			SendClientReply(packet.connection, text2);
		}
	}

	internal static void SendClientReply(Connection cn, string strCommand)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv.IsConnected())
		{
			((BaseNetwork)Net.sv).get_write().Start();
			((BaseNetwork)Net.sv).get_write().PacketID((Type)11);
			((BaseNetwork)Net.sv).get_write().String(strCommand);
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv.IsConnected())
		{
			((BaseNetwork)Net.sv).get_write().Start();
			((BaseNetwork)Net.sv).get_write().PacketID((Type)12);
			((BaseNetwork)Net.sv).get_write().String(ConsoleSystem.BuildCommand(strCommand, args));
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv.IsConnected())
		{
			((BaseNetwork)Net.sv).get_write().Start();
			((BaseNetwork)Net.sv).get_write().PacketID((Type)12);
			((BaseNetwork)Net.sv).get_write().String(ConsoleSystem.BuildCommand(strCommand, args));
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(cn));
		}
	}

	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv.IsConnected())
		{
			((BaseNetwork)Net.sv).get_write().Start();
			((BaseNetwork)Net.sv).get_write().PacketID((Type)12);
			((BaseNetwork)Net.sv).get_write().String(ConsoleSystem.BuildCommand(strCommand, args));
			((BaseNetwork)Net.sv).get_write().Send(new SendInfo(Net.sv.connections));
		}
	}
}
