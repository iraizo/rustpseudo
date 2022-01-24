using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Facepunch;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	public class NotificationList
	{
		private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/push/send";

		private static readonly HttpClient Http = new HttpClient();

		private readonly HashSet<ulong> _subscriptions = new HashSet<ulong>();

		private double _lastSend;

		public bool AddSubscription(ulong steamId)
		{
			if (steamId == 0L)
			{
				return false;
			}
			if (_subscriptions.get_Count() >= 50)
			{
				return false;
			}
			return _subscriptions.Add(steamId);
		}

		public bool RemoveSubscription(ulong steamId)
		{
			return _subscriptions.Remove(steamId);
		}

		public bool HasSubscription(ulong steamId)
		{
			return _subscriptions.Contains(steamId);
		}

		public List<ulong> ToList()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			List<ulong> list = Pool.GetList<ulong>();
			Enumerator<ulong> enumerator = _subscriptions.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ulong current = enumerator.get_Current();
					list.Add(current);
				}
				return list;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		public void LoadFrom(List<ulong> steamIds)
		{
			_subscriptions.Clear();
			if (steamIds == null)
			{
				return;
			}
			foreach (ulong steamId in steamIds)
			{
				_subscriptions.Add(steamId);
			}
		}

		public void IntersectWith(List<PlayerNameID> players)
		{
			List<ulong> list = Pool.GetList<ulong>();
			foreach (PlayerNameID player in players)
			{
				list.Add(player.userid);
			}
			_subscriptions.IntersectWith((IEnumerable<ulong>)list);
			Pool.FreeList<ulong>(ref list);
		}

		public Task<NotificationSendResult> SendNotification(NotificationChannel channel, string title, string body, string type)
		{
			double realtimeSinceStartup = TimeEx.get_realtimeSinceStartup();
			if (realtimeSinceStartup - _lastSend < 15.0)
			{
				return Task.FromResult(NotificationSendResult.RateLimited);
			}
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			if (!string.IsNullOrWhiteSpace(type))
			{
				serverPairingData["type"] = type;
			}
			_lastSend = realtimeSinceStartup;
			return SendNotificationImpl((ICollection<ulong>)_subscriptions, channel, title, body, serverPairingData);
		}

		public static async Task<NotificationSendResult> SendNotificationTo(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			NotificationSendResult notificationSendResult = await SendNotificationImpl(steamIds, channel, title, body, data);
			if (notificationSendResult == NotificationSendResult.NoTargetsFound)
			{
				notificationSendResult = NotificationSendResult.Sent;
			}
			return notificationSendResult;
		}

		public static async Task<NotificationSendResult> SendNotificationTo(ulong steamId, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			HashSet<ulong> set = Pool.Get<HashSet<ulong>>();
			set.Clear();
			set.Add(steamId);
			NotificationSendResult result = await SendNotificationImpl((ICollection<ulong>)set, channel, title, body, data);
			set.Clear();
			Pool.Free<HashSet<ulong>>(ref set);
			return result;
		}

		private static async Task<NotificationSendResult> SendNotificationImpl(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			if (!Server.IsEnabled || !App.notifications)
			{
				return NotificationSendResult.Disabled;
			}
			if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
			{
				return NotificationSendResult.Empty;
			}
			if (steamIds.Count == 0)
			{
				return NotificationSendResult.Sent;
			}
			PushRequest pushRequest = Pool.Get<PushRequest>();
			pushRequest.ServerToken = Server.Token;
			pushRequest.Channel = channel;
			pushRequest.Title = title;
			pushRequest.Body = body;
			pushRequest.Data = data;
			pushRequest.SteamIds = Pool.GetList<ulong>();
			foreach (ulong steamId in steamIds)
			{
				pushRequest.SteamIds.Add(steamId);
			}
			string content = JsonConvert.SerializeObject((object)pushRequest);
			Pool.Free<PushRequest>(ref pushRequest);
			try
			{
				StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
				HttpResponseMessage httpResponseMessage = await Http.PostAsync("https://companion-rust.facepunch.com/api/push/send", content2);
				if (!httpResponseMessage.IsSuccessStatusCode)
				{
					DebugEx.LogWarning((object)$"Failed to send notification: {httpResponseMessage.StatusCode}", (StackTraceLogType)0);
					return NotificationSendResult.ServerError;
				}
				if ((int)httpResponseMessage.StatusCode == 202)
				{
					return NotificationSendResult.NoTargetsFound;
				}
				return NotificationSendResult.Sent;
			}
			catch (Exception arg)
			{
				DebugEx.LogWarning((object)$"Exception thrown when sending notification: {arg}", (StackTraceLogType)0);
				return NotificationSendResult.Failed;
			}
		}
	}
}
