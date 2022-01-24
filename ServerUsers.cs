using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch.Extend;
using Facepunch.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public static class ServerUsers
{
	public enum UserGroup
	{
		None,
		Owner,
		Moderator,
		Banned
	}

	public class User
	{
		public ulong steamid;

		[JsonConverter(typeof(StringEnumConverter))]
		public UserGroup group;

		public string username;

		public string notes;

		public long expiry;

		[JsonIgnore]
		public bool IsExpired
		{
			get
			{
				if (expiry > 0)
				{
					return Epoch.get_Current() > expiry;
				}
				return false;
			}
		}
	}

	private static Dictionary<ulong, User> users = new Dictionary<ulong, User>();

	public static void Remove(ulong uid)
	{
		users.Remove(uid);
	}

	public static void Set(ulong uid, UserGroup group, string username, string notes, long expiry = -1L)
	{
		Remove(uid);
		User value = new User
		{
			steamid = uid,
			group = group,
			username = username,
			notes = notes,
			expiry = expiry
		};
		users.Add(uid, value);
	}

	public static User Get(ulong uid)
	{
		if (!users.TryGetValue(uid, out var value))
		{
			return null;
		}
		if (!value.IsExpired)
		{
			return value;
		}
		Remove(uid);
		return null;
	}

	public static bool Is(ulong uid, UserGroup group)
	{
		User user = Get(uid);
		if (user == null)
		{
			return false;
		}
		return user.group == group;
	}

	public static IEnumerable<User> GetAll(UserGroup group)
	{
		return Enumerable.Where<User>(Enumerable.Where<User>((IEnumerable<User>)users.Values, (Func<User, bool>)((User x) => x.group == group)), (Func<User, bool>)((User x) => !x.IsExpired));
	}

	public static void Clear()
	{
		users.Clear();
	}

	public static void Load()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Clear();
		string serverFolder = Server.GetServerFolder("cfg");
		Option server;
		if (File.Exists(serverFolder + "/bans.cfg"))
		{
			string text = File.ReadAllText(serverFolder + "/bans.cfg");
			if (!string.IsNullOrEmpty(text))
			{
				Debug.Log((object)("Running " + serverFolder + "/bans.cfg"));
				server = Option.get_Server();
				ConsoleSystem.RunFile(((Option)(ref server)).Quiet(), text);
			}
		}
		if (File.Exists(serverFolder + "/users.cfg"))
		{
			string text2 = File.ReadAllText(serverFolder + "/users.cfg");
			if (!string.IsNullOrEmpty(text2))
			{
				Debug.Log((object)("Running " + serverFolder + "/users.cfg"));
				server = Option.get_Server();
				ConsoleSystem.RunFile(((Option)(ref server)).Quiet(), text2);
			}
		}
	}

	public static void Save()
	{
		foreach (ulong item in Enumerable.ToList<ulong>(Enumerable.Select<KeyValuePair<ulong, User>, ulong>(Enumerable.Where<KeyValuePair<ulong, User>>((IEnumerable<KeyValuePair<ulong, User>>)users, (Func<KeyValuePair<ulong, User>, bool>)((KeyValuePair<ulong, User> kv) => kv.Value.IsExpired)), (Func<KeyValuePair<ulong, User>, ulong>)((KeyValuePair<ulong, User> kv) => kv.Key))))
		{
			Remove(item);
		}
		string serverFolder = Server.GetServerFolder("cfg");
		StringBuilder stringBuilder = new StringBuilder(67108864);
		stringBuilder.Clear();
		foreach (User item2 in GetAll(UserGroup.Banned))
		{
			if (!(item2.notes == "EAC"))
			{
				stringBuilder.Append("banid ");
				stringBuilder.Append(item2.steamid);
				stringBuilder.Append(' ');
				stringBuilder.Append(StringExtensions.QuoteSafe(item2.username));
				stringBuilder.Append(' ');
				stringBuilder.Append(StringExtensions.QuoteSafe(item2.notes));
				stringBuilder.Append(' ');
				stringBuilder.Append(item2.expiry);
				stringBuilder.Append("\r\n");
			}
		}
		File.WriteAllText(serverFolder + "/bans.cfg", stringBuilder.ToString());
		stringBuilder.Clear();
		foreach (User item3 in GetAll(UserGroup.Owner))
		{
			stringBuilder.Append("ownerid ");
			stringBuilder.Append(item3.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(StringExtensions.QuoteSafe(item3.username));
			stringBuilder.Append(' ');
			stringBuilder.Append(StringExtensions.QuoteSafe(item3.notes));
			stringBuilder.Append("\r\n");
		}
		foreach (User item4 in GetAll(UserGroup.Moderator))
		{
			stringBuilder.Append("moderatorid ");
			stringBuilder.Append(item4.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(StringExtensions.QuoteSafe(item4.username));
			stringBuilder.Append(' ');
			stringBuilder.Append(StringExtensions.QuoteSafe(item4.notes));
			stringBuilder.Append("\r\n");
		}
		File.WriteAllText(serverFolder + "/users.cfg", stringBuilder.ToString());
	}

	public static string BanListString(bool bHeader = false)
	{
		List<User> list = Enumerable.ToList<User>(GetAll(UserGroup.Banned));
		StringBuilder stringBuilder = new StringBuilder(67108864);
		if (bHeader)
		{
			if (list.Count == 0)
			{
				return "ID filter list: empty\n";
			}
			if (list.Count == 1)
			{
				stringBuilder.Append("ID filter list: 1 entry\n");
			}
			else
			{
				stringBuilder.Append($"ID filter list: {list.Count} entries\n");
			}
		}
		int num = 1;
		foreach (User item in list)
		{
			stringBuilder.Append(num);
			stringBuilder.Append(' ');
			stringBuilder.Append(item.steamid);
			stringBuilder.Append(" : ");
			if (item.expiry > 0)
			{
				stringBuilder.Append(((double)(item.expiry - Epoch.get_Current()) / 60.0).ToString("F3"));
				stringBuilder.Append(" min");
			}
			else
			{
				stringBuilder.Append("permanent");
			}
			stringBuilder.Append('\n');
			num++;
		}
		return stringBuilder.ToString();
	}

	public static string BanListStringEx()
	{
		IEnumerable<User> all = GetAll(UserGroup.Banned);
		StringBuilder stringBuilder = new StringBuilder(67108864);
		int num = 1;
		foreach (User item in all)
		{
			stringBuilder.Append(num);
			stringBuilder.Append(' ');
			stringBuilder.Append(item.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(StringExtensions.QuoteSafe(item.username));
			stringBuilder.Append(' ');
			stringBuilder.Append(StringExtensions.QuoteSafe(item.notes));
			stringBuilder.Append(' ');
			stringBuilder.Append(item.expiry);
			stringBuilder.Append('\n');
			num++;
		}
		return stringBuilder.ToString();
	}
}
