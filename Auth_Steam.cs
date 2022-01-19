using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;

public static class Auth_Steam
{
	internal static List<Connection> waitingList = new List<Connection>();

	public static IEnumerator Run(Connection connection)
	{
		connection.authStatus = "";
		if (!PlatformService.Instance.BeginPlayerSession(connection.userid, connection.token))
		{
			ConnectionAuth.Reject(connection, "Steam Auth Failed");
			yield break;
		}
		waitingList.Add(connection);
		Stopwatch timeout = Stopwatch.StartNew();
		while (timeout.Elapsed.TotalSeconds < 30.0 && connection.active && !(connection.authStatus != ""))
		{
			yield return null;
		}
		waitingList.Remove(connection);
		if (connection.active)
		{
			if (connection.authStatus.Length == 0)
			{
				ConnectionAuth.Reject(connection, "Steam Auth Timeout");
				PlatformService.Instance.EndPlayerSession(connection.userid);
			}
			else if (connection.authStatus == "banned")
			{
				ConnectionAuth.Reject(connection, "Auth: " + connection.authStatus);
				PlatformService.Instance.EndPlayerSession(connection.userid);
			}
			else if (connection.authStatus == "gamebanned")
			{
				ConnectionAuth.Reject(connection, "Steam Auth: " + connection.authStatus);
				PlatformService.Instance.EndPlayerSession(connection.userid);
			}
			else if (connection.authStatus == "vacbanned")
			{
				ConnectionAuth.Reject(connection, "Steam Auth: " + connection.authStatus);
				PlatformService.Instance.EndPlayerSession(connection.userid);
			}
			else if (connection.authStatus != "ok")
			{
				ConnectionAuth.Reject(connection, "Steam Auth Failed", "Steam Auth Error: " + connection.authStatus);
				PlatformService.Instance.EndPlayerSession(connection.userid);
			}
			else
			{
				string text = (Server.censorplayerlist ? RandomUsernames.Get(connection.userid + (ulong)Random.Range(0, 100000)) : connection.username);
				PlatformService.Instance.UpdatePlayerSession(connection.userid, text);
			}
		}
	}

	public static bool ValidateConnecting(ulong steamid, ulong ownerSteamID, AuthResponse response)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Invalid comparison between Unknown and I4
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Invalid comparison between Unknown and I4
		Connection val = waitingList.Find((Connection x) => x.userid == steamid);
		if (val == null)
		{
			return false;
		}
		val.ownerid = ownerSteamID;
		if (ServerUsers.Is(ownerSteamID, ServerUsers.UserGroup.Banned) || ServerUsers.Is(steamid, ServerUsers.UserGroup.Banned))
		{
			val.authStatus = "banned";
			return true;
		}
		if ((int)response == 2)
		{
			val.authStatus = "ok";
			return true;
		}
		if ((int)response == 3)
		{
			val.authStatus = "vacbanned";
			return true;
		}
		if ((int)response == 4)
		{
			val.authStatus = "gamebanned";
			return true;
		}
		if ((int)response == 1)
		{
			val.authStatus = "ok";
			return true;
		}
		val.authStatus = ((object)(AuthResponse)(ref response)).ToString();
		return true;
	}
}
