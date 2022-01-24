using System;
using System.Collections;
using ConVar;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using UnityEngine;
using UnityEngine.Networking;

public static class Auth_CentralizedBans
{
	private class BanPayload
	{
		public ulong steamId;

		public string reason;

		public long expiryDate;
	}

	private static readonly BanPayload payloadData = new BanPayload();

	public static IEnumerator Run(Connection connection)
	{
		if (!connection.active || connection.rejected || string.IsNullOrWhiteSpace(Server.bansServerEndpoint) || !Server.bansServerEndpoint.StartsWith("http"))
		{
			yield break;
		}
		connection.authStatus = "";
		if (!Server.bansServerEndpoint.EndsWith("/"))
		{
			Server.bansServerEndpoint += "/";
		}
		if (connection.ownerid != 0L && connection.ownerid != connection.userid)
		{
			string text = Server.bansServerEndpoint + connection.ownerid;
			UnityWebRequest ownerRequest = UnityWebRequest.Get(text);
			ownerRequest.set_timeout(Server.bansServerTimeout);
			yield return ownerRequest.SendWebRequest();
			if (CheckIfPlayerBanned(connection.ownerid, connection, ownerRequest))
			{
				yield break;
			}
		}
		string text2 = Server.bansServerEndpoint + connection.userid;
		UnityWebRequest userRequest = UnityWebRequest.Get(text2);
		userRequest.set_timeout(Server.bansServerTimeout);
		yield return userRequest.SendWebRequest();
		if (!CheckIfPlayerBanned(connection.userid, connection, userRequest))
		{
			connection.authStatus = "ok";
		}
	}

	private static bool CheckIfPlayerBanned(ulong steamId, Connection connection, UnityWebRequest request)
	{
		if (request.get_isNetworkError())
		{
			Debug.LogError((object)("Failed to check centralized bans due to a network error (" + request.get_error() + ")"));
			if (Server.bansServerFailureMode == 1)
			{
				Reject("Centralized Ban Error: Network Error");
				return true;
			}
			return false;
		}
		if (request.get_responseCode() == 404)
		{
			return false;
		}
		if (request.get_isHttpError())
		{
			Debug.LogError((object)$"Failed to check centralized bans due to a server error ({request.get_responseCode()}: {request.get_error()})");
			if (Server.bansServerFailureMode == 1)
			{
				Reject("Centralized Ban Error: Server Error");
				return true;
			}
			return false;
		}
		try
		{
			payloadData.steamId = 0uL;
			payloadData.reason = null;
			payloadData.expiryDate = 0L;
			JsonUtility.FromJsonOverwrite(request.get_downloadHandler().get_text(), (object)payloadData);
			if (payloadData.expiryDate > 0 && Epoch.get_Current() >= payloadData.expiryDate)
			{
				return false;
			}
			if (payloadData.steamId != steamId)
			{
				Debug.LogError((object)$"Failed to check centralized bans due to SteamID mismatch (expected {steamId}, got {payloadData.steamId})");
				if (Server.bansServerFailureMode == 1)
				{
					Reject("Centralized Ban Error: SteamID Mismatch");
					return true;
				}
				return false;
			}
			string text = payloadData.reason ?? "no reason given";
			string text2 = ((payloadData.expiryDate > 0) ? (" for " + NumberExtensions.FormatSecondsLong(payloadData.expiryDate - Epoch.get_Current())) : "");
			Reject("You are banned from this server" + text2 + " (" + text + ")");
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Failed to check centralized bans due to a malformed response: " + request.get_downloadHandler().get_text()));
			Debug.LogException(ex);
			if (Server.bansServerFailureMode == 1)
			{
				Reject("Centralized Ban Error: Malformed Response");
				return true;
			}
			return false;
		}
		void Reject(string reason)
		{
			ConnectionAuth.Reject(connection, reason);
			PlatformService.Instance.EndPlayerSession(connection.userid);
		}
	}
}
