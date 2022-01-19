using System;
using System.Collections;
using System.Threading.Tasks;
using Facepunch.Nexus;
using Facepunch.Nexus.Models;
using Network;
using UnityEngine;

public static class Auth_Nexus
{
	public static IEnumerator Run(Connection connection)
	{
		if (!connection.active || connection.rejected || !NexusServer.Started)
		{
			yield break;
		}
		connection.authStatus = "";
		Task<NexusLoginResult> loginTask = NexusServer.Login(connection.userid);
		yield return (object)new WaitUntil((Func<bool>)(() => loginTask.IsCompleted));
		if (loginTask.IsFaulted || loginTask.IsCanceled)
		{
			Reject("Nexus login failure");
			if (loginTask.Exception != null)
			{
				Debug.LogException((Exception)loginTask.Exception);
			}
			yield break;
		}
		NexusLoginResult result = loginTask.Result;
		if (((NexusLoginResult)(ref result)).get_IsRedirect())
		{
			ConsoleNetwork.SendClientCommand(connection, "nexus.redirect", ((NexusLoginResult)(ref result)).get_RedirectIpAddress(), ((NexusLoginResult)(ref result)).get_RedirectPort());
			Reject("Redirecting to another zone...");
			yield break;
		}
		if (((NexusLoginResult)(ref result)).get_AssignedZoneName() == null)
		{
			string spawnZoneName;
			NexusZoneDetails spawnZone;
			try
			{
				spawnZoneName = ZoneController.Instance.ChooseSpawnZone(connection.userid, isAlreadyAssignedToThisZone: false);
				if (string.IsNullOrWhiteSpace(spawnZoneName))
				{
					throw new Exception("ZoneController did not choose a spawn zone (returned '" + (spawnZoneName ?? "<null>") + "')");
				}
				spawnZone = NexusServer.FindZone(spawnZoneName);
				if (spawnZone == null)
				{
					throw new Exception("ZoneController picked a spawn zone which we don't know about (" + spawnZoneName + ")");
				}
			}
			catch (Exception ex)
			{
				Reject("Nexus spawn - exception while choosing spawn zone");
				Debug.LogException(ex);
				yield break;
			}
			Task assignTask = NexusServer.AssignInitialZone(connection.userid, spawnZoneName);
			yield return (object)new WaitUntil((Func<bool>)(() => assignTask.IsCompleted));
			if (assignTask.IsFaulted || assignTask.IsCanceled)
			{
				Reject("Nexus spawn - exception while registering transfer to spawn zone");
				if (assignTask.Exception != null)
				{
					Debug.LogException((Exception)assignTask.Exception);
				}
				yield break;
			}
			if (spawnZoneName != NexusServer.ZoneName)
			{
				ConsoleNetwork.SendClientCommand(connection, "nexus.redirect", spawnZone.get_IpAddress(), spawnZone.get_Port());
				Reject("Redirecting to another zone...");
				yield break;
			}
		}
		connection.authStatus = "ok";
		void Reject(string reason)
		{
			ConnectionAuth.Reject(connection, reason);
			PlatformService.Instance.EndPlayerSession(connection.userid);
		}
	}
}
