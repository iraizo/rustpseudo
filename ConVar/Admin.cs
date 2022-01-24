using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	[Factory("global")]
	public class Admin : ConsoleSystem
	{
		[Preserve]
		public struct PlayerInfo
		{
			public string SteamID;

			public string OwnerSteamID;

			public string DisplayName;

			public int Ping;

			public string Address;

			public int ConnectedSeconds;

			public float VoiationLevel;

			public float CurrentLevel;

			public float UnspentXp;

			public float Health;
		}

		[Preserve]
		public struct ServerInfoOutput
		{
			public string Hostname;

			public int MaxPlayers;

			public int Players;

			public int Queued;

			public int Joining;

			public int EntityCount;

			public string GameTime;

			public int Uptime;

			public string Map;

			public float Framerate;

			public int Memory;

			public int Collections;

			public int NetworkIn;

			public int NetworkOut;

			public bool Restarting;

			public string SaveCreatedTime;
		}

		[Preserve]
		public struct ServerConvarInfo
		{
			public string FullName;

			public string Value;

			public string Help;
		}

		[ReplicatedVar(Help = "Controls whether the in-game admin UI is displayed to admins")]
		public static bool allowAdminUI = true;

		[ServerVar(Help = "Print out currently connected clients")]
		public static void status(Arg arg)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Expected O, but got Unknown
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			string @string = arg.GetString(0, "");
			string text = string.Empty;
			if (@string.Length == 0)
			{
				text = text + "hostname: " + Server.hostname + "\n";
				text = text + "version : " + 2325 + " secure (secure mode enabled, connected to Steam3)\n";
				text = text + "map     : " + Server.level + "\n";
				text += $"players : {Enumerable.Count<BasePlayer>((IEnumerable<BasePlayer>)BasePlayer.activePlayerList)} ({Server.maxplayers} max) ({SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued} queued) ({SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining} joining)\n\n";
			}
			TextTable val = new TextTable();
			val.AddColumn("id");
			val.AddColumn("name");
			val.AddColumn("ping");
			val.AddColumn("connected");
			val.AddColumn("addr");
			val.AddColumn("owner");
			val.AddColumn("violation");
			val.AddColumn("kicks");
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					try
					{
						if (!current.IsValid())
						{
							continue;
						}
						string userIDString = current.UserIDString;
						if (current.net.get_connection() == null)
						{
							val.AddRow(new string[2] { userIDString, "NO CONNECTION" });
							continue;
						}
						string text2 = current.net.get_connection().ownerid.ToString();
						string text3 = StringExtensions.QuoteSafe(current.displayName);
						string text4 = Net.sv.GetAveragePing(current.net.get_connection()).ToString();
						string text5 = current.net.get_connection().ipaddress;
						string text6 = current.violationLevel.ToString("0.0");
						string text7 = current.GetAntiHackKicks().ToString();
						if (!arg.get_IsAdmin() && !arg.get_IsRcon())
						{
							text5 = "xx.xxx.xx.xxx";
						}
						string text8 = current.net.get_connection().GetSecondsConnected() + "s";
						if (@string.Length <= 0 || StringEx.Contains(text3, @string, CompareOptions.IgnoreCase) || userIDString.Contains(@string) || text2.Contains(@string) || text5.Contains(@string))
						{
							val.AddRow(new string[8]
							{
								userIDString,
								text3,
								text4,
								text8,
								text5,
								(text2 == userIDString) ? string.Empty : text2,
								text6,
								text7
							});
						}
					}
					catch (Exception ex)
					{
						val.AddRow(new string[2]
						{
							current.UserIDString,
							StringExtensions.QuoteSafe(ex.Message)
						});
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			arg.ReplyWith(text + ((object)val).ToString());
		}

		[ServerVar(Help = "Print out stats of currently connected clients")]
		public static void stats(Arg arg)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			TextTable table = new TextTable();
			table.AddColumn("id");
			table.AddColumn("name");
			table.AddColumn("time");
			table.AddColumn("kills");
			table.AddColumn("deaths");
			table.AddColumn("suicides");
			table.AddColumn("player");
			table.AddColumn("building");
			table.AddColumn("entity");
			Action<ulong, string> action = delegate(ulong id, string name)
			{
				ServerStatistics.Storage storage = ServerStatistics.Get(id);
				string text2 = TimeSpan.FromSeconds(storage.Get("time")).ToShortString();
				string text3 = storage.Get("kill_player").ToString();
				string text4 = (storage.Get("deaths") - storage.Get("death_suicide")).ToString();
				string text5 = storage.Get("death_suicide").ToString();
				string text6 = storage.Get("hit_player_direct_los").ToString();
				string text7 = storage.Get("hit_player_indirect_los").ToString();
				string text8 = storage.Get("hit_building_direct_los").ToString();
				string text9 = storage.Get("hit_building_indirect_los").ToString();
				string text10 = storage.Get("hit_entity_direct_los").ToString();
				string text11 = storage.Get("hit_entity_indirect_los").ToString();
				table.AddRow(new string[9]
				{
					id.ToString(),
					name,
					text2,
					text3,
					text4,
					text5,
					text6 + " / " + text7,
					text8 + " / " + text9,
					text10 + " / " + text11
				});
			};
			ulong uInt = arg.GetUInt64(0, 0uL);
			if (uInt == 0L)
			{
				string @string = arg.GetString(0, "");
				Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						BasePlayer current = enumerator.get_Current();
						try
						{
							if (current.IsValid())
							{
								string text = StringExtensions.QuoteSafe(current.displayName);
								if (@string.Length <= 0 || StringEx.Contains(text, @string, CompareOptions.IgnoreCase))
								{
									action(current.userID, text);
								}
							}
						}
						catch (Exception ex)
						{
							table.AddRow(new string[2]
							{
								current.UserIDString,
								StringExtensions.QuoteSafe(ex.Message)
							});
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
			else
			{
				string arg2 = "N/A";
				BasePlayer basePlayer = BasePlayer.FindByID(uInt);
				if (Object.op_Implicit((Object)(object)basePlayer))
				{
					arg2 = StringExtensions.QuoteSafe(basePlayer.displayName);
				}
				action(uInt, arg2);
			}
			arg.ReplyWith(((object)table).ToString());
		}

		[ServerVar]
		public static void killplayer(Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				basePlayer = BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				arg.ReplyWith("Player not found");
			}
			else
			{
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, useProtection: false);
			}
		}

		[ServerVar]
		public static void injureplayer(Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				basePlayer = BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				arg.ReplyWith("Player not found");
			}
			else
			{
				Global.InjurePlayer(basePlayer);
			}
		}

		[ServerVar]
		public static void recoverplayer(Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				basePlayer = BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				arg.ReplyWith("Player not found");
			}
			else
			{
				Global.RecoverPlayer(basePlayer);
			}
		}

		[ServerVar]
		public static void kick(Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!Object.op_Implicit((Object)(object)player) || player.net == null || player.net.get_connection() == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			string @string = arg.GetString(1, "no reason given");
			arg.ReplyWith("Kicked: " + player.displayName);
			Chat.Broadcast("Kicking " + player.displayName + " (" + @string + ")", "SERVER", "#eee", 0uL);
			player.Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
		}

		[ServerVar]
		public static void kickall(Arg arg)
		{
			BasePlayer[] array = Enumerable.ToArray<BasePlayer>((IEnumerable<BasePlayer>)BasePlayer.activePlayerList);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
			}
		}

		[ServerVar(Help = "ban <player> <reason> [optional duration]")]
		public static void ban(Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!Object.op_Implicit((Object)(object)player) || player.net == null || player.net.get_connection() == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			ServerUsers.User user = ServerUsers.Get(player.userID);
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith($"User {player.userID} is already banned");
				return;
			}
			string @string = arg.GetString(1, "No Reason Given");
			if (TryGetBanExpiry(arg, 2, out var expiry, out var durationSuffix))
			{
				ServerUsers.Set(player.userID, ServerUsers.UserGroup.Banned, player.displayName, @string, expiry);
				string text = "";
				if (player.IsConnected && player.net.get_connection().ownerid != 0L && player.net.get_connection().ownerid != player.net.get_connection().userid)
				{
					text += $" and also banned ownerid {player.net.get_connection().ownerid}";
					ServerUsers.Set(player.net.get_connection().ownerid, ServerUsers.UserGroup.Banned, player.displayName, arg.GetString(1, $"Family share owner of {player.net.get_connection().userid}"), -1L);
				}
				ServerUsers.Save();
				arg.ReplyWith($"Kickbanned User{durationSuffix}: {player.userID} - {player.displayName}{text}");
				Chat.Broadcast("Kickbanning " + player.displayName + durationSuffix + " (" + @string + ")", "SERVER", "#eee", 0uL);
				Net.sv.Kick(player.net.get_connection(), "Banned" + durationSuffix + ": " + @string, false);
			}
		}

		[ServerVar]
		public static void moderatorid(Arg arg)
		{
			ulong uInt = arg.GetUInt64(0, 0uL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (uInt < 70000000000000000L)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(uInt);
			if (user != null && user.group == ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith("User " + uInt + " is already a Moderator");
				return;
			}
			ServerUsers.Set(uInt, ServerUsers.UserGroup.Moderator, @string, string2, -1L);
			arg.ReplyWith("Added moderator " + @string + ", steamid " + uInt);
		}

		[ServerVar]
		public static void ownerid(Arg arg)
		{
			ulong uInt = arg.GetUInt64(0, 0uL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (uInt < 70000000000000000L)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(uInt);
			if (user != null && user.group == ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith("User " + uInt + " is already an Owner");
				return;
			}
			ServerUsers.Set(uInt, ServerUsers.UserGroup.Owner, @string, string2, -1L);
			arg.ReplyWith("Added owner " + @string + ", steamid " + uInt);
		}

		[ServerVar]
		public static void removemoderator(Arg arg)
		{
			ulong uInt = arg.GetUInt64(0, 0uL);
			if (uInt < 70000000000000000L)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(uInt);
			if (user == null || user.group != ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith("User " + uInt + " isn't a moderator");
				return;
			}
			ServerUsers.Remove(uInt);
			arg.ReplyWith("Removed Moderator: " + uInt);
		}

		[ServerVar]
		public static void removeowner(Arg arg)
		{
			ulong uInt = arg.GetUInt64(0, 0uL);
			if (uInt < 70000000000000000L)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(uInt);
			if (user == null || user.group != ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith("User " + uInt + " isn't an owner");
				return;
			}
			ServerUsers.Remove(uInt);
			arg.ReplyWith("Removed Owner: " + uInt);
		}

		[ServerVar(Help = "banid <steamid> <username> <reason> [optional duration]")]
		public static void banid(Arg arg)
		{
			ulong uInt = arg.GetUInt64(0, 0uL);
			string text = arg.GetString(1, "unnamed");
			string @string = arg.GetString(2, "no reason");
			if (uInt < 70000000000000000L)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(uInt);
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith("User " + uInt + " is already banned");
			}
			else
			{
				if (!TryGetBanExpiry(arg, 3, out var expiry, out var durationSuffix))
				{
					return;
				}
				string text2 = "";
				BasePlayer basePlayer = BasePlayer.FindByID(uInt);
				if ((Object)(object)basePlayer != (Object)null && basePlayer.IsConnected)
				{
					text = basePlayer.displayName;
					if (basePlayer.IsConnected && basePlayer.net.get_connection().ownerid != 0L && basePlayer.net.get_connection().ownerid != basePlayer.net.get_connection().userid)
					{
						text2 += $" and also banned ownerid {basePlayer.net.get_connection().ownerid}";
						ServerUsers.Set(basePlayer.net.get_connection().ownerid, ServerUsers.UserGroup.Banned, basePlayer.displayName, arg.GetString(1, $"Family share owner of {basePlayer.net.get_connection().userid}"), expiry);
					}
					Chat.Broadcast("Kickbanning " + basePlayer.displayName + durationSuffix + " (" + @string + ")", "SERVER", "#eee", 0uL);
					Net.sv.Kick(basePlayer.net.get_connection(), "Banned" + durationSuffix + ": " + @string, false);
				}
				ServerUsers.Set(uInt, ServerUsers.UserGroup.Banned, text, @string, expiry);
				arg.ReplyWith($"Banned User{durationSuffix}: {uInt} - \"{text}\" for \"{@string}\"{text2}");
			}
		}

		private static bool TryGetBanExpiry(Arg arg, int n, out long expiry, out string durationSuffix)
		{
			expiry = arg.GetTimestamp(n, -1L);
			durationSuffix = null;
			int current = Epoch.get_Current();
			if (expiry > 0 && expiry <= current)
			{
				arg.ReplyWith("Expiry time is in the past");
				return false;
			}
			durationSuffix = ((expiry > 0) ? (" for " + NumberExtensions.FormatSecondsLong(expiry - current)) : "");
			return true;
		}

		[ServerVar]
		public static void unban(Arg arg)
		{
			ulong uInt = arg.GetUInt64(0, 0uL);
			if (uInt < 70000000000000000L)
			{
				arg.ReplyWith($"This doesn't appear to be a 64bit steamid: {uInt}");
				return;
			}
			ServerUsers.User user = ServerUsers.Get(uInt);
			if (user == null || user.group != ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith($"User {uInt} isn't banned");
				return;
			}
			ServerUsers.Remove(uInt);
			arg.ReplyWith("Unbanned User: " + uInt);
		}

		[ServerVar]
		public static void skipqueue(Arg arg)
		{
			ulong uInt = arg.GetUInt64(0, 0uL);
			if (uInt < 70000000000000000L)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			}
			else
			{
				SingletonComponent<ServerMgr>.Instance.connectionQueue.SkipQueue(uInt);
			}
		}

		[ServerVar(Help = "Print out currently connected clients etc")]
		public static void players(Arg arg)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			TextTable val = new TextTable();
			val.AddColumn("id");
			val.AddColumn("name");
			val.AddColumn("ping");
			val.AddColumn("snap");
			val.AddColumn("updt");
			val.AddColumn("posi");
			val.AddColumn("dist");
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					string userIDString = current.UserIDString;
					string text = current.displayName.ToString();
					if (text.Length >= 14)
					{
						text = text.Substring(0, 14) + "..";
					}
					string text2 = text;
					string text3 = Net.sv.GetAveragePing(current.net.get_connection()).ToString();
					string text4 = current.GetQueuedUpdateCount(BasePlayer.NetworkQueue.Update).ToString();
					string text5 = current.GetQueuedUpdateCount(BasePlayer.NetworkQueue.UpdateDistance).ToString();
					val.AddRow(new string[7]
					{
						userIDString,
						text2,
						text3,
						string.Empty,
						text4,
						string.Empty,
						text5
					});
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			arg.ReplyWith(((object)val).ToString());
		}

		[ServerVar(Help = "Sends a message in chat")]
		public static void say(Arg arg)
		{
			Chat.Broadcast(arg.FullString, "SERVER", "#eee", 0uL);
		}

		[ServerVar(Help = "Show user info for players on server.")]
		public static void users(Arg arg)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					text = text + current.userID + ":\"" + current.displayName + "\"\n";
					num++;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			text = text + num + "users\n";
			arg.ReplyWith(text);
		}

		[ServerVar(Help = "Show user info for players on server.")]
		public static void sleepingusers(Arg arg)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			Enumerator<BasePlayer> enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					text += $"{current.userID}:{current.displayName}\n";
					num++;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			text += $"{num} sleeping users\n";
			arg.ReplyWith(text);
		}

		[ServerVar(Help = "Show user info for sleeping players on server in range of the player.")]
		public static void sleepingusersinrange(Arg arg)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer fromPlayer = arg.Player();
			if ((Object)(object)fromPlayer == (Object)null)
			{
				return;
			}
			float range = arg.GetFloat(0, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			Enumerator<BasePlayer> enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					list.Add(current);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			list.RemoveAll((BasePlayer p) => p.Distance2D((BaseEntity)fromPlayer) > range);
			list.Sort((BasePlayer player, BasePlayer basePlayer) => (!(player.Distance2D((BaseEntity)fromPlayer) < basePlayer.Distance2D((BaseEntity)fromPlayer))) ? 1 : (-1));
			foreach (BasePlayer item in list)
			{
				text += $"{item.userID}:{item.displayName}:{item.Distance2D((BaseEntity)fromPlayer)}m\n";
				num++;
			}
			Pool.FreeList<BasePlayer>(ref list);
			text += $"{num} sleeping users within {range}m\n";
			arg.ReplyWith(text);
		}

		[ServerVar(Help = "Show user info for players on server in range of the player.")]
		public static void usersinrange(Arg arg)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer fromPlayer = arg.Player();
			if ((Object)(object)fromPlayer == (Object)null)
			{
				return;
			}
			float range = arg.GetFloat(0, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					list.Add(current);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			list.RemoveAll((BasePlayer p) => p.Distance2D((BaseEntity)fromPlayer) > range);
			list.Sort((BasePlayer player, BasePlayer basePlayer) => (!(player.Distance2D((BaseEntity)fromPlayer) < basePlayer.Distance2D((BaseEntity)fromPlayer))) ? 1 : (-1));
			foreach (BasePlayer item in list)
			{
				text += $"{item.userID}:{item.displayName}:{item.Distance2D((BaseEntity)fromPlayer)}m\n";
				num++;
			}
			Pool.FreeList<BasePlayer>(ref list);
			text += $"{num} users within {range}m\n";
			arg.ReplyWith(text);
		}

		[ServerVar(Help = "List of banned users (sourceds compat)")]
		public static void banlist(Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString());
		}

		[ServerVar(Help = "List of banned users - shows reasons and usernames")]
		public static void banlistex(Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListStringEx());
		}

		[ServerVar(Help = "List of banned users, by ID (sourceds compat)")]
		public static void listid(Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString(bHeader: true));
		}

		[ServerVar]
		public static void mute(Arg arg)
		{
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!Object.op_Implicit((Object)(object)playerOrSleeper) || playerOrSleeper.net == null || playerOrSleeper.net.get_connection() == null)
			{
				arg.ReplyWith("Player not found");
			}
			else
			{
				playerOrSleeper.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, b: true);
			}
		}

		[ServerVar]
		public static void unmute(Arg arg)
		{
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!Object.op_Implicit((Object)(object)playerOrSleeper) || playerOrSleeper.net == null || playerOrSleeper.net.get_connection() == null)
			{
				arg.ReplyWith("Player not found");
			}
			else
			{
				playerOrSleeper.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, b: false);
			}
		}

		[ServerVar(Help = "Print a list of currently muted players")]
		public static void mutelist(Arg arg)
		{
			var enumerable = Enumerable.Select(Enumerable.Where<BasePlayer>(BasePlayer.allPlayerList, (Func<BasePlayer, bool>)((BasePlayer x) => x.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))), (BasePlayer x) => new
			{
				SteamId = x.UserIDString,
				Name = x.displayName
			});
			arg.ReplyWith((object)enumerable);
		}

		[ServerVar]
		public static void clientperf(Arg arg)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					current.ClientRPCPlayer(null, current, "GetPerformanceReport");
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		[ServerVar(Help = "Get information about all the cars in the world")]
		public static void carstats(Arg arg)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			HashSet<ModularCar> allCarsList = ModularCar.allCarsList;
			TextTable val = new TextTable();
			val.AddColumn("id");
			val.AddColumn("sockets");
			val.AddColumn("modules");
			val.AddColumn("complete");
			val.AddColumn("engine");
			val.AddColumn("health");
			val.AddColumn("location");
			int count = allCarsList.get_Count();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Enumerator<ModularCar> enumerator = allCarsList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ModularCar current = enumerator.get_Current();
					string text = current.net.ID.ToString();
					string text2 = current.TotalSockets.ToString();
					string text3 = current.NumAttachedModules.ToString();
					string text4;
					if (current.IsComplete())
					{
						text4 = "Complete";
						num++;
					}
					else
					{
						text4 = "Partial";
					}
					string text5;
					if (current.HasAnyWorkingEngines())
					{
						text5 = "Working";
						num2++;
					}
					else
					{
						text5 = "Broken";
					}
					string text6 = ((current.TotalMaxHealth() != 0f) ? $"{current.TotalHealth() / current.TotalMaxHealth():0%}" : "0");
					string text7;
					if (current.IsOutside())
					{
						text7 = "Outside";
					}
					else
					{
						text7 = "Inside";
						num3++;
					}
					val.AddRow(new string[7] { text, text2, text3, text4, text5, text6, text7 });
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			string text8 = "";
			text8 = ((count != 1) ? (text8 + $"\nThe world contains {count} modular cars.") : (text8 + "\nThe world contains 1 modular car."));
			text8 = ((num != 1) ? (text8 + $"\n{num} ({(float)num / (float)count:0%}) are in a completed state.") : (text8 + $"\n1 ({1f / (float)count:0%}) is in a completed state."));
			text8 = ((num2 != 1) ? (text8 + $"\n{num2} ({(float)num2 / (float)count:0%}) are driveable.") : (text8 + $"\n1 ({1f / (float)count:0%}) is driveable."));
			arg.ReplyWith(string.Concat(str1: (num3 != 1) ? (text8 + $"\n{num3} ({(float)num3 / (float)count:0%}) are sheltered indoors.") : (text8 + $"\n1 ({1f / (float)count:0%}) is sheltered indoors."), str0: ((object)val).ToString()));
		}

		[ServerVar]
		public static string teaminfo(Arg arg)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			ulong num = arg.GetUInt64(0, 0uL);
			if (num == 0L)
			{
				BasePlayer player = arg.GetPlayer(0);
				if ((Object)(object)player == (Object)null)
				{
					return "Player not found";
				}
				num = player.userID;
			}
			RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(num);
			if (playerTeam == null)
			{
				return "Player is not in a team";
			}
			TextTable val = new TextTable();
			val.AddColumn("steamID");
			val.AddColumn("username");
			val.AddColumn("online");
			val.AddColumn("leader");
			foreach (ulong memberId in playerTeam.members)
			{
				bool flag = Enumerable.FirstOrDefault<Connection>((IEnumerable<Connection>)Net.sv.connections, (Func<Connection, bool>)((Connection c) => c.connected && c.userid == memberId)) != null;
				val.AddRow(new string[4]
				{
					memberId.ToString(),
					GetPlayerName(memberId),
					flag ? "x" : "",
					(memberId == playerTeam.teamLeader) ? "x" : ""
				});
			}
			return ((object)val).ToString();
		}

		[ServerVar]
		public static void entid(Arg arg)
		{
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(arg.GetUInt(1, 0u)) as BaseEntity;
			if (!((Object)(object)baseEntity == (Object)null) && !(baseEntity is BasePlayer))
			{
				string @string = arg.GetString(0, "");
				if ((Object)(object)arg.Player() != (Object)null)
				{
					Debug.Log((object)("[ENTCMD] " + arg.Player().displayName + "/" + arg.Player().userID + " used *" + @string + "* on ent: " + ((Object)baseEntity).get_name()));
				}
				switch (@string)
				{
				case "kill":
					baseEntity.AdminKill();
					break;
				case "lock":
					baseEntity.SetFlag(BaseEntity.Flags.Locked, b: true);
					break;
				case "unlock":
					baseEntity.SetFlag(BaseEntity.Flags.Locked, b: false);
					break;
				case "debug":
					baseEntity.SetFlag(BaseEntity.Flags.Debugging, b: true);
					break;
				case "undebug":
					baseEntity.SetFlag(BaseEntity.Flags.Debugging, b: false);
					break;
				case "who":
					arg.ReplyWith("Owner ID: " + baseEntity.OwnerID);
					break;
				case "auth":
					arg.ReplyWith(AuthList(baseEntity));
					break;
				default:
					arg.ReplyWith("Unknown command");
					break;
				}
			}
		}

		private static string AuthList(BaseEntity ent)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			if (ent != null)
			{
				BuildingPrivlidge buildingPrivlidge;
				List<PlayerNameID> authorizedPlayers;
				if ((buildingPrivlidge = ent as BuildingPrivlidge) == null)
				{
					AutoTurret autoTurret;
					if ((autoTurret = ent as AutoTurret) == null)
					{
						CodeLock codeLock;
						if ((codeLock = ent as CodeLock) != null)
						{
							return CodeLockAuthList(codeLock);
						}
						goto IL_0042;
					}
					authorizedPlayers = autoTurret.authorizedPlayers;
				}
				else
				{
					authorizedPlayers = buildingPrivlidge.authorizedPlayers;
				}
				if (authorizedPlayers == null || authorizedPlayers.Count == 0)
				{
					return "Nobody is authed to this entity";
				}
				TextTable val = new TextTable();
				val.AddColumn("steamID");
				val.AddColumn("username");
				foreach (PlayerNameID item in authorizedPlayers)
				{
					val.AddRow(new string[2]
					{
						item.userid.ToString(),
						GetPlayerName(item.userid)
					});
				}
				return ((object)val).ToString();
			}
			goto IL_0042;
			IL_0042:
			return "Entity has no auth list";
		}

		private static string CodeLockAuthList(CodeLock codeLock)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			if (codeLock.whitelistPlayers.Count == 0 && codeLock.guestPlayers.Count == 0)
			{
				return "Nobody is authed to this entity";
			}
			TextTable val = new TextTable();
			val.AddColumn("steamID");
			val.AddColumn("username");
			val.AddColumn("isGuest");
			foreach (ulong whitelistPlayer in codeLock.whitelistPlayers)
			{
				val.AddRow(new string[3]
				{
					whitelistPlayer.ToString(),
					GetPlayerName(whitelistPlayer),
					""
				});
			}
			foreach (ulong guestPlayer in codeLock.guestPlayers)
			{
				val.AddRow(new string[3]
				{
					guestPlayer.ToString(),
					GetPlayerName(guestPlayer),
					"x"
				});
			}
			return ((object)val).ToString();
		}

		private static string GetPlayerName(ulong steamId)
		{
			BasePlayer basePlayer = Enumerable.FirstOrDefault<BasePlayer>(BasePlayer.allPlayerList, (Func<BasePlayer, bool>)((BasePlayer p) => p.userID == steamId));
			string text;
			if (!((Object)(object)basePlayer != (Object)null))
			{
				text = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId);
				if (text == null)
				{
					return "[unknown]";
				}
			}
			else
			{
				text = basePlayer.displayName;
			}
			return text;
		}

		[ServerVar(Help = "Get a list of players")]
		public static PlayerInfo[] playerlist()
		{
			return Enumerable.ToArray<PlayerInfo>(Enumerable.Select<BasePlayer, PlayerInfo>((IEnumerable<BasePlayer>)BasePlayer.activePlayerList, (Func<BasePlayer, PlayerInfo>)delegate(BasePlayer x)
			{
				PlayerInfo result = default(PlayerInfo);
				result.SteamID = x.UserIDString;
				result.OwnerSteamID = x.OwnerID.ToString();
				result.DisplayName = x.displayName;
				result.Ping = Net.sv.GetAveragePing(x.net.get_connection());
				result.Address = x.net.get_connection().ipaddress;
				result.ConnectedSeconds = (int)x.net.get_connection().GetSecondsConnected();
				result.VoiationLevel = x.violationLevel;
				result.Health = x.Health();
				return result;
			}));
		}

		[ServerVar(Help = "List of banned users")]
		public static ServerUsers.User[] Bans()
		{
			return Enumerable.ToArray<ServerUsers.User>(ServerUsers.GetAll(ServerUsers.UserGroup.Banned));
		}

		[ServerVar(Help = "Get a list of information about the server")]
		public static ServerInfoOutput ServerInfo()
		{
			ServerInfoOutput result = default(ServerInfoOutput);
			result.Hostname = Server.hostname;
			result.MaxPlayers = Server.maxplayers;
			result.Players = BasePlayer.activePlayerList.get_Count();
			result.Queued = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued;
			result.Joining = SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining;
			result.EntityCount = BaseNetworkable.serverEntities.Count;
			result.GameTime = (((Object)(object)TOD_Sky.get_Instance() != (Object)null) ? TOD_Sky.get_Instance().Cycle.get_DateTime().ToString() : DateTime.UtcNow.ToString());
			result.Uptime = (int)Time.get_realtimeSinceStartup();
			result.Map = Server.level;
			result.Framerate = Performance.report.frameRate;
			result.Memory = (int)Performance.report.memoryAllocations;
			result.Collections = (int)Performance.report.memoryCollections;
			result.NetworkIn = (int)((Net.sv != null) ? ((BaseNetwork)Net.sv).GetStat((Connection)null, (StatTypeLong)3) : 0);
			result.NetworkOut = (int)((Net.sv != null) ? ((BaseNetwork)Net.sv).GetStat((Connection)null, (StatTypeLong)1) : 0);
			result.Restarting = SingletonComponent<ServerMgr>.Instance.Restarting;
			result.SaveCreatedTime = SaveRestore.SaveCreatedTime.ToString();
			return result;
		}

		[ServerVar(Help = "Get information about this build")]
		public static BuildInfo BuildInfo()
		{
			return BuildInfo.get_Current();
		}

		[ServerVar]
		public static void AdminUI_RequestPlayerList(Arg arg)
		{
			if (allowAdminUI)
			{
				ConsoleNetwork.SendClientCommand(arg.get_Connection(), "AdminUI_ReceivePlayerList", JsonConvert.SerializeObject((object)playerlist()));
			}
		}

		[ServerVar]
		public static void AdminUI_RequestServerInfo(Arg arg)
		{
			if (allowAdminUI)
			{
				ConsoleNetwork.SendClientCommand(arg.get_Connection(), "AdminUI_ReceiveServerInfo", JsonConvert.SerializeObject((object)ServerInfo()));
			}
		}

		[ServerVar]
		public static void AdminUI_RequestServerConvars(Arg arg)
		{
			if (!allowAdminUI)
			{
				return;
			}
			List<ServerConvarInfo> list = Pool.GetList<ServerConvarInfo>();
			Command[] all = Index.get_All();
			foreach (Command val in all)
			{
				if (val.get_Server() && val.Variable && val.ServerAdmin && val.ShowInAdminUI)
				{
					list.Add(new ServerConvarInfo
					{
						FullName = val.FullName,
						Value = val.GetOveride?.Invoke(),
						Help = val.Description
					});
				}
			}
			ConsoleNetwork.SendClientCommand(arg.get_Connection(), "AdminUI_ReceiveCommands", JsonConvert.SerializeObject((object)list));
			Pool.FreeList<ServerConvarInfo>(ref list);
		}

		public Admin()
			: this()
		{
		}
	}
}
