using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CompanionServer;
using Facepunch;
using Facepunch.Math;
using Network;
using UnityEngine;

namespace ConVar
{
	[Factory("chat")]
	public class Chat : ConsoleSystem
	{
		public enum ChatChannel
		{
			Global,
			Team,
			Server,
			Cards
		}

		public struct ChatEntry
		{
			public ChatChannel Channel { get; set; }

			public string Message { get; set; }

			public string UserId { get; set; }

			public string Username { get; set; }

			public string Color { get; set; }

			public int Time { get; set; }
		}

		private const float textRange = 50f;

		private const float textVolumeBoost = 0.2f;

		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		private static List<ChatEntry> History = new List<ChatEntry>();

		[ServerVar]
		public static bool serverlog = true;

		public static void Broadcast(string message, string username = "SERVER", string color = "#eee", ulong userid = 0uL)
		{
			string text = StringEx.EscapeRichText(username);
			ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=" + color + ">" + text + "</color> " + message);
			ChatEntry chatEntry = default(ChatEntry);
			chatEntry.Channel = ChatChannel.Server;
			chatEntry.Message = message;
			chatEntry.UserId = userid.ToString();
			chatEntry.Username = username;
			chatEntry.Color = color;
			chatEntry.Time = Epoch.get_Current();
			ChatEntry chatEntry2 = chatEntry;
			History.Add(chatEntry2);
			RCon.Broadcast(RCon.LogType.Chat, chatEntry2);
		}

		[ServerUserVar]
		public static void say(Arg arg)
		{
			sayImpl(ChatChannel.Global, arg);
		}

		[ServerUserVar]
		public static void teamsay(Arg arg)
		{
			sayImpl(ChatChannel.Team, arg);
		}

		[ServerUserVar]
		public static void cardgamesay(Arg arg)
		{
			sayImpl(ChatChannel.Cards, arg);
		}

		private static void sayImpl(ChatChannel targetChannel, Arg arg)
		{
			if (!enabled)
			{
				arg.ReplyWith("Chat is disabled.");
				return;
			}
			BasePlayer basePlayer = arg.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer) || basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper)
			{
				if (basePlayer.NextChatTime == 0f)
				{
					basePlayer.NextChatTime = Time.get_realtimeSinceStartup() - 30f;
				}
				if (basePlayer.NextChatTime > Time.get_realtimeSinceStartup())
				{
					basePlayer.NextChatTime += 2f;
					float num = basePlayer.NextChatTime - Time.get_realtimeSinceStartup();
					ConsoleNetwork.SendClientCommand(basePlayer.net.get_connection(), "chat.add", 2, 0, "You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds");
					if (num > 120f)
					{
						basePlayer.Kick("Chatting too fast");
					}
					return;
				}
			}
			string @string = arg.GetString(0, "text");
			if (sayAs(targetChannel, basePlayer.userID, basePlayer.displayName, @string, basePlayer))
			{
				basePlayer.NextChatTime = Time.get_realtimeSinceStartup() + 1.5f;
			}
		}

		internal static bool sayAs(ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
		{
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)player))
			{
				player = null;
			}
			if (!enabled)
			{
				return false;
			}
			if ((Object)(object)player != (Object)null && player.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return false;
			}
			ServerUsers.UserGroup userGroup = ServerUsers.Get(userId)?.group ?? ServerUsers.UserGroup.None;
			if (userGroup == ServerUsers.UserGroup.Banned)
			{
				return false;
			}
			string text = message.Replace("\n", "").Replace("\r", "").Trim();
			if (text.Length > 128)
			{
				text = text.Substring(0, 128);
			}
			if (text.Length <= 0)
			{
				return false;
			}
			if (text.StartsWith("/") || text.StartsWith("\\"))
			{
				return false;
			}
			text = StringEx.EscapeRichText(text);
			if (serverlog)
			{
				ServerConsole.PrintColoured(ConsoleColor.DarkYellow, string.Concat("[", targetChannel, "] ", username, ": "), ConsoleColor.DarkGreen, text);
				string text2 = ((object)player)?.ToString() ?? $"{username}[{userId}]";
				switch (targetChannel)
				{
				case ChatChannel.Team:
					DebugEx.Log((object)("[TEAM CHAT] " + text2 + " : " + text), (StackTraceLogType)0);
					break;
				case ChatChannel.Cards:
					DebugEx.Log((object)("[CARDS CHAT] " + text2 + " : " + text), (StackTraceLogType)0);
					break;
				default:
					DebugEx.Log((object)("[CHAT] " + text2 + " : " + text), (StackTraceLogType)0);
					break;
				}
			}
			bool flag = userGroup == ServerUsers.UserGroup.Owner || userGroup == ServerUsers.UserGroup.Moderator;
			bool num = (((Object)(object)player != (Object)null) ? player.IsDeveloper : DeveloperList.Contains(userId));
			string text3 = "#5af";
			if (flag)
			{
				text3 = "#af5";
			}
			if (num)
			{
				text3 = "#fa5";
			}
			string text4 = StringEx.EscapeRichText(username);
			ChatEntry chatEntry = default(ChatEntry);
			chatEntry.Channel = targetChannel;
			chatEntry.Message = text;
			chatEntry.UserId = (((Object)(object)player != (Object)null) ? player.UserIDString : userId.ToString());
			chatEntry.Username = username;
			chatEntry.Color = text3;
			chatEntry.Time = Epoch.get_Current();
			ChatEntry chatEntry2 = chatEntry;
			History.Add(chatEntry2);
			RCon.Broadcast(RCon.LogType.Chat, chatEntry2);
			switch (targetChannel)
			{
			case ChatChannel.Cards:
			{
				if ((Object)(object)player == (Object)null)
				{
					return false;
				}
				if (!player.isMounted)
				{
					return false;
				}
				CardTable cardTable = player.GetMountedVehicle() as CardTable;
				if ((Object)(object)cardTable == (Object)null || !cardTable.GameController.PlayerIsInGame(player))
				{
					return false;
				}
				List<Connection> list = Pool.GetList<Connection>();
				cardTable.GameController.GetConnectionsInGame(list);
				if (list.Count > 0)
				{
					ConsoleNetwork.SendClientCommand(list, "chat.add2", 3, userId, text, text4, text3, 1f);
				}
				Pool.FreeList<Connection>(ref list);
				return true;
			}
			case ChatChannel.Global:
				if (Server.globalchat)
				{
					ConsoleNetwork.BroadcastToAllClients("chat.add2", 0, userId, text, text4, text3, 1f);
					return true;
				}
				break;
			case ChatChannel.Team:
			{
				RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userId);
				if (playerTeam == null)
				{
					return false;
				}
				List<Connection> onlineMemberConnections = playerTeam.GetOnlineMemberConnections();
				if (onlineMemberConnections != null)
				{
					ConsoleNetwork.SendClientCommand(onlineMemberConnections, "chat.add2", 1, userId, text, text4, text3, 1f);
				}
				playerTeam.BroadcastTeamChat(userId, text4, text, text3);
				return true;
			}
			}
			if ((Object)(object)player != (Object)null)
			{
				float num2 = 2500f;
				Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						BasePlayer current = enumerator.get_Current();
						Vector3 val = ((Component)current).get_transform().get_position() - ((Component)player).get_transform().get_position();
						float sqrMagnitude = ((Vector3)(ref val)).get_sqrMagnitude();
						if (!(sqrMagnitude > num2))
						{
							ConsoleNetwork.SendClientCommand(current.net.get_connection(), "chat.add2", 0, userId, text, text4, text3, Mathf.Clamp01(num2 - sqrMagnitude + 0.2f));
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return true;
			}
			return false;
		}

		[ServerVar]
		[Help("Return the last x lines of the console. Default is 200")]
		public static IEnumerable<ChatEntry> tail(Arg arg)
		{
			int @int = arg.GetInt(0, 200);
			int num = History.Count - @int;
			if (num < 0)
			{
				num = 0;
			}
			return History.Skip(num);
		}

		[ServerVar]
		[Help("Search the console for a particular string")]
		public static IEnumerable<ChatEntry> search(Arg arg)
		{
			string search = arg.GetString(0, (string)null);
			if (search == null)
			{
				return Enumerable.Empty<ChatEntry>();
			}
			return History.Where((ChatEntry x) => x.Message.Length < 4096 && StringEx.Contains(x.Message, search, CompareOptions.IgnoreCase));
		}

		public Chat()
			: this()
		{
		}
	}
}
