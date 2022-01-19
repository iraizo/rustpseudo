using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using UnityEngine;

namespace ConVar
{
	[Factory("pool")]
	public class Pool : ConsoleSystem
	{
		[ServerVar]
		[ClientVar]
		public static int mode = 2;

		[ServerVar]
		[ClientVar]
		public static bool prewarm = true;

		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		[ServerVar]
		[ClientVar]
		public static bool debug = false;

		[ServerVar]
		[ClientVar]
		public static void print_memory(Arg arg)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			if (Pool.directory.Count == 0)
			{
				arg.ReplyWith("Memory pool is empty.");
				return;
			}
			TextTable val = new TextTable();
			val.AddColumn("type");
			val.AddColumn("pooled");
			val.AddColumn("active");
			val.AddColumn("hits");
			val.AddColumn("misses");
			val.AddColumn("spills");
			foreach (KeyValuePair<Type, ICollection> item in Pool.directory.OrderByDescending((KeyValuePair<Type, ICollection> x) => x.Value.get_ItemsCreated()))
			{
				string text = item.Key.ToString().Replace("System.Collections.Generic.", "");
				ICollection value = item.Value;
				val.AddRow(new string[6]
				{
					text,
					NumberExtensions.FormatNumberShort(value.get_ItemsInStack()),
					NumberExtensions.FormatNumberShort(value.get_ItemsInUse()),
					NumberExtensions.FormatNumberShort(value.get_ItemsTaken()),
					NumberExtensions.FormatNumberShort(value.get_ItemsCreated()),
					NumberExtensions.FormatNumberShort(value.get_ItemsSpilled())
				});
			}
			arg.ReplyWith(((object)val).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void print_prefabs(Arg arg)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable val = new TextTable();
			val.AddColumn("id");
			val.AddColumn("name");
			val.AddColumn("count");
			foreach (KeyValuePair<uint, PrefabPool> item in pool.storage)
			{
				string text = item.Key.ToString();
				string text2 = StringPool.Get(item.Key);
				string text3 = item.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || StringEx.Contains(text2, @string, CompareOptions.IgnoreCase))
				{
					val.AddRow(new string[3]
					{
						text,
						Path.GetFileNameWithoutExtension(text2),
						text3
					});
				}
			}
			arg.ReplyWith(((object)val).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void print_assets(Arg arg)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			if (AssetPool.storage.Count == 0)
			{
				arg.ReplyWith("Asset pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable val = new TextTable();
			val.AddColumn("type");
			val.AddColumn("allocated");
			val.AddColumn("available");
			foreach (KeyValuePair<Type, Pool> item in AssetPool.storage)
			{
				string text = item.Key.ToString();
				string text2 = item.Value.allocated.ToString();
				string text3 = item.Value.available.ToString();
				if (string.IsNullOrEmpty(@string) || StringEx.Contains(text, @string, CompareOptions.IgnoreCase))
				{
					val.AddRow(new string[3] { text, text2, text3 });
				}
			}
			arg.ReplyWith(((object)val).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void clear_memory(Arg arg)
		{
			Pool.Clear();
		}

		[ServerVar]
		[ClientVar]
		public static void clear_prefabs(Arg arg)
		{
			GameManager.server.pool.Clear();
		}

		[ServerVar]
		[ClientVar]
		public static void clear_assets(Arg arg)
		{
			AssetPool.Clear();
		}

		[ServerVar]
		[ClientVar]
		public static void export_prefabs(Arg arg)
		{
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, PrefabPool> item in pool.storage)
			{
				string arg2 = item.Key.ToString();
				string text = StringPool.Get(item.Key);
				string arg3 = item.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || StringEx.Contains(text, @string, CompareOptions.IgnoreCase))
				{
					stringBuilder.AppendLine($"{arg2},{Path.GetFileNameWithoutExtension(text)},{arg3}");
				}
			}
			File.WriteAllText("prefabs.csv", stringBuilder.ToString());
		}

		public Pool()
			: this()
		{
		}
	}
}
