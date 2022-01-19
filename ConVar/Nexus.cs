using System;
using Facepunch;
using ProtoBuf.Nexus;
using UnityEngine;

namespace ConVar
{
	[Factory("nexus")]
	public class Nexus : ConsoleSystem
	{
		public static readonly Phrase RedirectPhrase = new Phrase("loading.redirect", "Switching servers");

		private const string DefaultEndpoint = "https://api.facepunch.com/api/nexus/";

		[ReplicatedVar(Help = "URL endpoint to use for the Nexus API", Default = "https://api.facepunch.com/api/nexus/")]
		public static string endpoint = "https://api.facepunch.com/api/nexus/";

		[ServerVar(Clientside = true)]
		public static bool logging = true;

		[ServerVar]
		public static string secretKey = "";

		[ServerVar]
		public static string zoneController = "basic";

		[ServerVar(Help = "Time in seconds to allow the server to process nexus messages before re-sending (requires restart)")]
		public static int messageLockDuration = 5;

		[ServerVar(Help = "Maximum amount of time in seconds that transfers should be cached before auto-saving")]
		public static int transferFlushTime = 60;

		[ServerVar(Help = "How far away islands should be spawned, as a factor of the map size")]
		public static float islandSpawnDistance = 2f;

		[ServerVar(Help = "Maximum distance between zones to allow boat travel (according to the nexus map, which uses normalized coordinates)")]
		public static float maxBoatTravelDistance = 0.33f;

		[ServerVar(Help = "Time offset in hours from the nexus clock")]
		public static float timeOffset = 0f;

		[ServerVar(Help = "Multiplier for nexus RPC timeout durations in case we expect different latencies")]
		public static float rpcTimeoutMultiplier = 1f;

		[ServerVar(Help = "Time in seconds to keep players in the loading state before going to sleep")]
		public static float loadingTimeout = 900f;

		[ServerVar(Help = "Time in seconds to wait between server status pings")]
		public static float pingInterval = 30f;

		[ServerVar]
		public static void transfer(Arg arg)
		{
			if (!NexusServer.Started)
			{
				arg.ReplyWith("Server is not connected to a nexus");
				return;
			}
			string text = arg.GetString(0, "")?.Trim();
			if (string.IsNullOrWhiteSpace(text))
			{
				arg.ReplyWith("Usage: nexus.transfer <target_zone>");
				return;
			}
			if (text == NexusServer.ZoneName)
			{
				arg.ReplyWith("You're already on the target zone");
				return;
			}
			BasePlayer basePlayer = arg.get_Connection().player as BasePlayer;
			if ((Object)(object)basePlayer == (Object)null)
			{
				arg.ReplyWith("Must be run as a player");
			}
			else
			{
				NexusServer.TransferEntity(basePlayer, text, "console");
			}
		}

		[ServerVar]
		public static void refreshislands(Arg arg)
		{
			if (!NexusServer.Started)
			{
				arg.ReplyWith("Server is not connected to a nexus");
			}
			else
			{
				NexusServer.UpdateIslands();
			}
		}

		[ServerVar]
		public static void ping(Arg arg)
		{
			if (!NexusServer.Started)
			{
				arg.ReplyWith("Server is not connected to a nexus");
				return;
			}
			string @string = arg.GetString(0, "");
			if (string.IsNullOrWhiteSpace(@string))
			{
				arg.ReplyWith("Usage: nexus.ping <target_zone>");
			}
			else
			{
				SendPing(arg.Player(), @string);
			}
			static async void SendPing(BasePlayer requester, string to)
			{
				Request val = Pool.Get<Request>();
				val.ping = Pool.Get<PingRequest>();
				float startTime = Time.get_realtimeSinceStartup();
				try
				{
					await NexusServer.ZoneRpc(to, val);
					float num = Time.get_realtimeSinceStartup() - startTime;
					requester?.ConsoleMessage($"Ping took {num:F3}s");
				}
				catch (Exception arg2)
				{
					requester?.ConsoleMessage($"Failed to ping zone {to}: {arg2}");
				}
			}
		}

		[ServerVar]
		public static void broadcast_ping(Arg arg)
		{
			if (!NexusServer.Started)
			{
				arg.ReplyWith("Server is not connected to a nexus");
			}
			else
			{
				SendBroadcastPing(arg.Player());
			}
			static async void SendBroadcastPing(BasePlayer requester)
			{
				Request val = Pool.Get<Request>();
				val.ping = Pool.Get<PingRequest>();
				float startTime = Time.get_realtimeSinceStartup();
				try
				{
					using NexusRpcResult nexusRpcResult = await NexusServer.BroadcastRpc(val);
					float num = Time.get_realtimeSinceStartup() - startTime;
					string arg2 = string.Join(", ", nexusRpcResult.Responses.Keys);
					requester?.ConsoleMessage($"Broadcast ping took {num:F3}s, response received from zones: {arg2}");
				}
				catch (Exception arg3)
				{
					requester?.ConsoleMessage($"Failed to broadcast ping: {arg3}");
				}
			}
		}

		public Nexus()
			: this()
		{
		}
	}
}
