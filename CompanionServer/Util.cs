using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	public static class Util
	{
		public const int OceanMargin = 500;

		public static readonly Phrase NotificationEmpty = new Phrase("app.error.empty", "Notification was not sent because it was missing some content.");

		public static readonly Phrase NotificationDisabled = new Phrase("app.error.disabled", "Rust+ features are disabled on this server.");

		public static readonly Phrase NotificationRateLimit = new Phrase("app.error.ratelimit", "You are sending too many notifications at a time. Please wait and then try again.");

		public static readonly Phrase NotificationServerError = new Phrase("app.error.servererror", "The companion server failed to send the notification.");

		public static readonly Phrase NotificationNoTargets = new Phrase("app.error.notargets", "Open the Rust+ menu in-game to pair your phone with this server.");

		public static readonly Phrase NotificationTooManySubscribers = new Phrase("app.error.toomanysubs", "There are too many players subscribed to these notifications.");

		public static readonly Phrase NotificationUnknown = new Phrase("app.error.unknown", "An unknown error occurred sending the notification.");

		public static Vector2 WorldToMap(Vector3 worldPos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(worldPos.x - TerrainMeta.Position.x, worldPos.z - TerrainMeta.Position.z);
		}

		public static void SendSignedInNotification(BasePlayer player)
		{
			if (!((Object)(object)player == (Object)null) && player.currentTeam != 0L)
			{
				RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindTeam(player.currentTeam);
				Dictionary<string, string> serverPairingData = GetServerPairingData();
				serverPairingData.Add("type", "login");
				serverPairingData.Add("targetId", player.UserIDString);
				serverPairingData.Add("targetName", StringExtensions.Truncate(player.displayName, 128, (string)null));
				playerTeam?.SendNotification(NotificationChannel.PlayerLoggedIn, player.displayName + " is now online", ConVar.Server.hostname, serverPairingData, player.userID);
			}
		}

		public static void SendDeathNotification(BasePlayer player, BaseEntity killer)
		{
			BasePlayer basePlayer;
			string value;
			string text;
			if ((basePlayer = killer as BasePlayer) != null && ((object)basePlayer).GetType() == typeof(BasePlayer))
			{
				value = basePlayer.UserIDString;
				text = basePlayer.displayName;
			}
			else
			{
				value = "";
				text = killer.ShortPrefabName;
			}
			if (!((Object)(object)player == (Object)null) && !string.IsNullOrEmpty(text))
			{
				Dictionary<string, string> serverPairingData = GetServerPairingData();
				serverPairingData.Add("type", "death");
				serverPairingData.Add("targetId", value);
				serverPairingData.Add("targetName", StringExtensions.Truncate(text, 128, (string)null));
				NotificationList.SendNotificationTo(player.userID, NotificationChannel.PlayerDied, "You were killed by " + text, ConVar.Server.hostname, serverPairingData);
			}
		}

		public static Task<NotificationSendResult> SendPairNotification(string type, BasePlayer player, string title, string message, Dictionary<string, string> data)
		{
			if (!Server.IsEnabled)
			{
				return Task.FromResult(NotificationSendResult.Disabled);
			}
			if (!Server.CanSendPairingNotification(player.userID))
			{
				return Task.FromResult(NotificationSendResult.RateLimited);
			}
			if (data == null)
			{
				data = GetPlayerPairingData(player);
			}
			data.Add("type", type);
			return NotificationList.SendNotificationTo(player.userID, NotificationChannel.Pairing, title, message, data);
		}

		public static Dictionary<string, string> GetServerPairingData()
		{
			Dictionary<string, string> dictionary = Pool.Get<Dictionary<string, string>>();
			dictionary.Clear();
			dictionary.Add("id", App.serverid);
			dictionary.Add("name", StringExtensions.Truncate(ConVar.Server.hostname, 128, (string)null));
			dictionary.Add("desc", StringExtensions.Truncate(ConVar.Server.description, 512, (string)null));
			dictionary.Add("img", StringExtensions.Truncate(ConVar.Server.headerimage, 128, (string)null));
			dictionary.Add("logo", StringExtensions.Truncate(ConVar.Server.logoimage, 128, (string)null));
			dictionary.Add("url", StringExtensions.Truncate(ConVar.Server.url, 128, (string)null));
			dictionary.Add("ip", App.GetPublicIP());
			dictionary.Add("port", App.port.ToString("G", CultureInfo.InvariantCulture));
			return dictionary;
		}

		public static Dictionary<string, string> GetPlayerPairingData(BasePlayer player)
		{
			Dictionary<string, string> serverPairingData = GetServerPairingData();
			serverPairingData.Add("playerId", player.UserIDString);
			serverPairingData.Add("playerToken", player.appToken.ToString("G", CultureInfo.InvariantCulture));
			return serverPairingData;
		}

		public static void BroadcastAppTeamRemoval(this BasePlayer player)
		{
			AppBroadcast val = Pool.Get<AppBroadcast>();
			val.teamChanged = Pool.Get<AppTeamChanged>();
			val.teamChanged.playerId = player.userID;
			val.teamChanged.teamInfo = player.GetAppTeamInfo(player.userID);
			Server.Broadcast(new PlayerTarget(player.userID), val);
		}

		public static void BroadcastAppTeamUpdate(this RelationshipManager.PlayerTeam team)
		{
			AppBroadcast val = Pool.Get<AppBroadcast>();
			val.teamChanged = Pool.Get<AppTeamChanged>();
			val.ShouldPool = false;
			foreach (ulong member in team.members)
			{
				val.teamChanged.playerId = member;
				val.teamChanged.teamInfo = team.GetAppTeamInfo(member);
				Server.Broadcast(new PlayerTarget(member), val);
			}
			val.ShouldPool = true;
			val.Dispose();
		}

		public static void BroadcastTeamChat(this RelationshipManager.PlayerTeam team, ulong steamId, string name, string message, string color)
		{
			uint current = (uint)Epoch.get_Current();
			Server.TeamChat.Record(team.teamID, steamId, name, message, color, current);
			AppBroadcast val = Pool.Get<AppBroadcast>();
			val.teamMessage = Pool.Get<AppTeamMessage>();
			val.teamMessage.message = Pool.Get<AppChatMessage>();
			val.ShouldPool = false;
			AppChatMessage message2 = val.teamMessage.message;
			message2.steamId = steamId;
			message2.name = name;
			message2.message = message;
			message2.color = color;
			message2.time = current;
			foreach (ulong member in team.members)
			{
				Server.Broadcast(new PlayerTarget(member), val);
			}
			val.ShouldPool = true;
			val.Dispose();
		}

		public static void SendNotification(this RelationshipManager.PlayerTeam team, NotificationChannel channel, string title, string body, Dictionary<string, string> data, ulong ignorePlayer = 0uL)
		{
			List<ulong> list = Pool.GetList<ulong>();
			foreach (ulong member in team.members)
			{
				if (member == ignorePlayer)
				{
					continue;
				}
				BasePlayer basePlayer = RelationshipManager.FindByID(member);
				if (!((Object)(object)basePlayer == (Object)null))
				{
					Networkable net = basePlayer.net;
					if (((net != null) ? net.get_connection() : null) != null)
					{
						continue;
					}
				}
				list.Add(member);
			}
			NotificationList.SendNotificationTo(list, channel, title, body, data);
			Pool.FreeList<ulong>(ref list);
		}

		public static string ToErrorCode(this ValidationResult result)
		{
			return result switch
			{
				ValidationResult.NotFound => "not_found", 
				ValidationResult.RateLimit => "rate_limit", 
				ValidationResult.Banned => "banned", 
				_ => "unknown", 
			};
		}

		public static string ToErrorMessage(this NotificationSendResult result)
		{
			return result switch
			{
				NotificationSendResult.Sent => null, 
				NotificationSendResult.Empty => NotificationEmpty.get_translated(), 
				NotificationSendResult.Disabled => NotificationDisabled.get_translated(), 
				NotificationSendResult.RateLimited => NotificationRateLimit.get_translated(), 
				NotificationSendResult.ServerError => NotificationServerError.get_translated(), 
				NotificationSendResult.NoTargetsFound => NotificationNoTargets.get_translated(), 
				NotificationSendResult.TooManySubscribers => NotificationTooManySubscribers.get_translated(), 
				_ => NotificationUnknown.get_translated(), 
			};
		}
	}
}
