using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using UnityEngine;

public class ConnectionAuth : MonoBehaviour
{
	[NonSerialized]
	public static List<Connection> m_AuthConnection = new List<Connection>();

	public bool IsAuthed(ulong iSteamID)
	{
		if (Object.op_Implicit((Object)(object)BasePlayer.FindByID(iSteamID)))
		{
			return true;
		}
		if (SingletonComponent<ServerMgr>.Instance.connectionQueue.IsJoining(iSteamID))
		{
			return true;
		}
		if (SingletonComponent<ServerMgr>.Instance.connectionQueue.IsQueued(iSteamID))
		{
			return true;
		}
		return false;
	}

	public static void Reject(Connection connection, string strReason, string strReasonPrivate = null)
	{
		DebugEx.Log((object)(((object)connection).ToString() + " Rejecting connection - " + (string.IsNullOrEmpty(strReasonPrivate) ? strReason : strReasonPrivate)), (StackTraceLogType)0);
		Net.sv.Kick(connection, strReason, false);
		m_AuthConnection.Remove(connection);
	}

	public static void OnDisconnect(Connection connection)
	{
		m_AuthConnection.Remove(connection);
	}

	public void Approve(Connection connection)
	{
		m_AuthConnection.Remove(connection);
		SingletonComponent<ServerMgr>.Instance.connectionQueue.Join(connection);
	}

	public void OnNewConnection(Connection connection)
	{
		connection.connected = false;
		if (connection.token == null || connection.token.Length < 32)
		{
			Reject(connection, "Invalid Token");
			return;
		}
		if (connection.userid == 0L)
		{
			Reject(connection, "Invalid SteamID");
			return;
		}
		if (connection.protocol != 2326)
		{
			if (!DeveloperList.Contains(connection.userid))
			{
				Reject(connection, "Incompatible Version");
				return;
			}
			DebugEx.Log((object)("Not kicking " + connection.userid + " for incompatible protocol (is a developer)"), (StackTraceLogType)0);
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Banned))
		{
			ServerUsers.User user = ServerUsers.Get(connection.userid);
			string text = user?.notes ?? "no reason given";
			string text2 = ((user != null && user.expiry > 0) ? (" for " + NumberExtensions.FormatSecondsLong(user.expiry - Epoch.get_Current())) : "");
			Reject(connection, "You are banned from this server" + text2 + " (" + text + ")");
			return;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Moderator))
		{
			DebugEx.Log((object)(((object)connection).ToString() + " has auth level 1"), (StackTraceLogType)0);
			connection.authLevel = 1u;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Owner))
		{
			DebugEx.Log((object)(((object)connection).ToString() + " has auth level 2"), (StackTraceLogType)0);
			connection.authLevel = 2u;
		}
		if (DeveloperList.Contains(connection.userid))
		{
			DebugEx.Log((object)(((object)connection).ToString() + " is a developer"), (StackTraceLogType)0);
			connection.authLevel = 3u;
		}
		m_AuthConnection.Add(connection);
		((MonoBehaviour)this).StartCoroutine(AuthorisationRoutine(connection));
	}

	public IEnumerator AuthorisationRoutine(Connection connection)
	{
		yield return ((MonoBehaviour)this).StartCoroutine(Auth_Steam.Run(connection));
		yield return ((MonoBehaviour)this).StartCoroutine(Auth_EAC.Run(connection));
		yield return ((MonoBehaviour)this).StartCoroutine(Auth_CentralizedBans.Run(connection));
		yield return ((MonoBehaviour)this).StartCoroutine(Auth_Nexus.Run(connection));
		if (!connection.rejected && connection.active)
		{
			if (IsAuthed(connection.userid))
			{
				Reject(connection, "You are already connected as a player!");
			}
			else
			{
				Approve(connection);
			}
		}
	}

	public ConnectionAuth()
		: this()
	{
	}
}
