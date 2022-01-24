using System.Collections.Generic;
using System.Net;
using CompanionServer;
using Facepunch.Extend;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	[Factory("app")]
	public class App : ConsoleSystem
	{
		[ServerVar]
		public static string listenip = "";

		[ServerVar]
		public static int port;

		[ServerVar]
		public static string publicip = "";

		[ServerVar(Help = "Disables updating entirely - emergency use only")]
		public static bool update = true;

		[ServerVar(Help = "Enables sending push notifications")]
		public static bool notifications = true;

		[ServerVar(Help = "Max number of queued messages - set to 0 to disable message processing")]
		public static int queuelimit = 100;

		[ReplicatedVar(Default = "")]
		public static string serverid = "";

		[ServerVar(Help = "Cooldown time before alarms can send another notification (in seconds)")]
		public static float alarmcooldown = 30f;

		[ServerVar]
		public static int maxconnections = 500;

		[ServerVar]
		public static int maxconnectionsperip = 5;

		[ServerUserVar]
		public static async void pair(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!((Object)(object)basePlayer == (Object)null))
			{
				Dictionary<string, string> playerPairingData = Util.GetPlayerPairingData(basePlayer);
				NotificationSendResult notificationSendResult = await Util.SendPairNotification("server", basePlayer, StringExtensions.Truncate(Server.hostname, 128, (string)null), "Tap to pair with this server.", playerPairingData);
				arg.ReplyWith((notificationSendResult == NotificationSendResult.Sent) ? "Sent pairing notification." : notificationSendResult.ToErrorMessage());
			}
		}

		[ServerVar]
		public static void info(Arg arg)
		{
			if (!CompanionServer.Server.IsEnabled)
			{
				arg.ReplyWith("Companion server is not enabled");
				return;
			}
			Listener listener = CompanionServer.Server.Listener;
			arg.ReplyWith($"Server ID: {serverid}\nListening on: {listener.Address}:{listener.Port}\nApp connects to: {GetPublicIP()}:{port}");
		}

		[ServerVar]
		public static void resetlimiter(Arg arg)
		{
			CompanionServer.Server.Listener?.Limiter?.Clear();
		}

		[ServerVar]
		public static void connections(Arg arg)
		{
			string text = CompanionServer.Server.Listener?.Limiter?.ToString() ?? "Not available";
			arg.ReplyWith(text);
		}

		public static IPAddress GetListenIP()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Invalid comparison between Unknown and I4
			if (!string.IsNullOrWhiteSpace(listenip))
			{
				IPAddress val = default(IPAddress);
				if (!IPAddress.TryParse(listenip, ref val) || (int)val.get_AddressFamily() != 2)
				{
					Debug.LogError((object)("Invalid app.listenip: " + listenip));
					return IPAddress.Any;
				}
				return val;
			}
			return IPAddress.Any;
		}

		public static string GetPublicIP()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Invalid comparison between Unknown and I4
			IPAddress val = default(IPAddress);
			if (!string.IsNullOrWhiteSpace(publicip) && IPAddress.TryParse(publicip, ref val) && (int)val.get_AddressFamily() == 2)
			{
				return publicip;
			}
			return ((object)SteamServer.get_PublicIp()).ToString();
		}

		public App()
			: this()
		{
		}
	}
}
