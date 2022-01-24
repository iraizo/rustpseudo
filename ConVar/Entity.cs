using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Facepunch;
using UnityEngine;

namespace ConVar
{
	[Factory("entity")]
	public class Entity : ConsoleSystem
	{
		private struct EntityInfo
		{
			public BaseNetworkable entity;

			public uint entityID;

			public uint groupID;

			public uint parentID;

			public string status;

			public EntityInfo(BaseNetworkable src)
			{
				entity = src;
				BaseEntity baseEntity = entity as BaseEntity;
				BaseEntity baseEntity2 = (((Object)(object)baseEntity != (Object)null) ? baseEntity.GetParentEntity() : null);
				entityID = (((Object)(object)entity != (Object)null && entity.net != null) ? entity.net.ID : 0u);
				groupID = (((Object)(object)entity != (Object)null && entity.net != null && entity.net.group != null) ? entity.net.group.ID : 0u);
				parentID = (((Object)(object)baseEntity != (Object)null) ? baseEntity.parentEntity.uid : 0u);
				if ((Object)(object)baseEntity != (Object)null && baseEntity.parentEntity.uid != 0)
				{
					if ((Object)(object)baseEntity2 == (Object)null)
					{
						status = "orphan";
					}
					else
					{
						status = "child";
					}
				}
				else
				{
					status = string.Empty;
				}
			}
		}

		private static TextTable GetEntityTable(Func<EntityInfo, bool> filter)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			TextTable val = new TextTable();
			val.AddColumn("realm");
			val.AddColumn("entity");
			val.AddColumn("group");
			val.AddColumn("parent");
			val.AddColumn("name");
			val.AddColumn("position");
			val.AddColumn("local");
			val.AddColumn("rotation");
			val.AddColumn("local");
			val.AddColumn("status");
			val.AddColumn("invokes");
			foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
			{
				if (!((Object)(object)serverEntity == (Object)null))
				{
					EntityInfo arg = new EntityInfo(serverEntity);
					if (filter(arg))
					{
						string[] obj = new string[11]
						{
							"sv",
							arg.entityID.ToString(),
							arg.groupID.ToString(),
							arg.parentID.ToString(),
							arg.entity.ShortPrefabName,
							null,
							null,
							null,
							null,
							null,
							null
						};
						Vector3 val2 = ((Component)arg.entity).get_transform().get_position();
						obj[5] = ((object)(Vector3)(ref val2)).ToString();
						val2 = ((Component)arg.entity).get_transform().get_localPosition();
						obj[6] = ((object)(Vector3)(ref val2)).ToString();
						Quaternion val3 = ((Component)arg.entity).get_transform().get_rotation();
						val2 = ((Quaternion)(ref val3)).get_eulerAngles();
						obj[7] = ((object)(Vector3)(ref val2)).ToString();
						val3 = ((Component)arg.entity).get_transform().get_localRotation();
						val2 = ((Quaternion)(ref val3)).get_eulerAngles();
						obj[8] = ((object)(Vector3)(ref val2)).ToString();
						obj[9] = arg.status;
						obj[10] = arg.entity.InvokeString();
						val.AddRow(obj);
					}
				}
			}
			return val;
		}

		[ServerVar]
		[ClientVar]
		public static void find_entity(Arg args)
		{
			string filter = args.GetString(0, "");
			TextTable entityTable = GetEntityTable((EntityInfo info) => string.IsNullOrEmpty(filter) || info.entity.PrefabName.Contains(filter));
			args.ReplyWith(((object)entityTable).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void find_id(Arg args)
		{
			uint filter = args.GetUInt(0, 0u);
			TextTable entityTable = GetEntityTable((EntityInfo info) => info.entityID == filter);
			args.ReplyWith(((object)entityTable).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void find_group(Arg args)
		{
			uint filter = args.GetUInt(0, 0u);
			TextTable entityTable = GetEntityTable((EntityInfo info) => info.groupID == filter);
			args.ReplyWith(((object)entityTable).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void find_parent(Arg args)
		{
			uint filter = args.GetUInt(0, 0u);
			TextTable entityTable = GetEntityTable((EntityInfo info) => info.parentID == filter);
			args.ReplyWith(((object)entityTable).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void find_status(Arg args)
		{
			string filter = args.GetString(0, "");
			TextTable entityTable = GetEntityTable((EntityInfo info) => string.IsNullOrEmpty(filter) || info.status.Contains(filter));
			args.ReplyWith(((object)entityTable).ToString());
		}

		[ServerVar]
		[ClientVar]
		public static void find_radius(Arg args)
		{
			BasePlayer player = args.Player();
			if (!((Object)(object)player == (Object)null))
			{
				uint filter = args.GetUInt(0, 10u);
				TextTable entityTable = GetEntityTable((EntityInfo info) => Vector3.Distance(((Component)info.entity).get_transform().get_position(), ((Component)player).get_transform().get_position()) <= (float)filter);
				args.ReplyWith(((object)entityTable).ToString());
			}
		}

		[ServerVar]
		[ClientVar]
		public static void find_self(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!((Object)(object)basePlayer == (Object)null) && basePlayer.net != null)
			{
				uint filter = basePlayer.net.ID;
				TextTable entityTable = GetEntityTable((EntityInfo info) => info.entityID == filter);
				args.ReplyWith(((object)entityTable).ToString());
			}
		}

		[ServerVar]
		public static void debug_toggle(Arg args)
		{
			int @int = args.GetInt(0, 0);
			if (@int == 0)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find((uint)@int) as BaseEntity;
			if (!((Object)(object)baseEntity == (Object)null))
			{
				baseEntity.SetFlag(BaseEntity.Flags.Debugging, !baseEntity.IsDebugging());
				if (baseEntity.IsDebugging())
				{
					baseEntity.OnDebugStart();
				}
				args.ReplyWith("Debugging for " + baseEntity.net.ID + " " + (baseEntity.IsDebugging() ? "enabled" : "disabled"));
			}
		}

		[ServerVar]
		public static void nudge(int entID)
		{
			if (entID != 0)
			{
				BaseEntity baseEntity = BaseNetworkable.serverEntities.Find((uint)entID) as BaseEntity;
				if (!((Object)(object)baseEntity == (Object)null))
				{
					((Component)baseEntity).BroadcastMessage("DebugNudge", (SendMessageOptions)1);
				}
			}
		}

		[ServerVar(Name = "spawn")]
		public static string svspawn(string name, Vector3 pos, Vector3 dir)
		{
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer arg = ConsoleSystem.CurrentArgs.Player();
			if (string.IsNullOrEmpty(name))
			{
				return "No entity name provided";
			}
			string[] array = Enumerable.ToArray<string>(Enumerable.Select<string, string>(Enumerable.Where<string>((IEnumerable<string>)GameManifest.Current.entities, (Func<string, bool>)((string x) => StringEx.Contains(Path.GetFileNameWithoutExtension(x), name, CompareOptions.IgnoreCase))), (Func<string, string>)((string x) => x.ToLower())));
			if (array.Length == 0)
			{
				return "Entity type not found";
			}
			if (array.Length > 1)
			{
				string text = Enumerable.FirstOrDefault<string>((IEnumerable<string>)array, (Func<string, bool>)((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0));
				if (text == null)
				{
					Debug.Log((object)$"{arg} failed to spawn \"{name}\"");
					return "Unknown entity - could be:\n\n" + string.Join("\n", Enumerable.ToArray<string>(Enumerable.Select<string, string>((IEnumerable<string>)array, (Func<string, string>)Path.GetFileNameWithoutExtension)));
				}
				array[0] = text;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(array[0], pos, Quaternion.LookRotation(dir, Vector3.get_up()));
			if ((Object)(object)baseEntity == (Object)null)
			{
				Debug.Log((object)$"{arg} failed to spawn \"{array[0]}\" (tried to spawn \"{name}\")");
				return "Couldn't spawn " + name;
			}
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if ((Object)(object)basePlayer != (Object)null)
			{
				Quaternion val = Quaternion.LookRotation(dir, Vector3.get_up());
				basePlayer.OverrideViewAngles(((Quaternion)(ref val)).get_eulerAngles());
			}
			baseEntity.Spawn();
			Debug.Log((object)$"{arg} spawned \"{baseEntity}\" at {pos}");
			return string.Concat("spawned ", baseEntity, " at ", pos);
		}

		[ServerVar(Name = "spawnitem")]
		public static string svspawnitem(string name, Vector3 pos)
		{
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer arg = ConsoleSystem.CurrentArgs.Player();
			if (string.IsNullOrEmpty(name))
			{
				return "No entity name provided";
			}
			string[] array = Enumerable.ToArray<string>(Enumerable.Where<string>(Enumerable.Select<ItemDefinition, string>((IEnumerable<ItemDefinition>)ItemManager.itemList, (Func<ItemDefinition, string>)((ItemDefinition x) => x.shortname)), (Func<string, bool>)((string x) => StringEx.Contains(x, name, CompareOptions.IgnoreCase))));
			if (array.Length == 0)
			{
				return "Entity type not found";
			}
			if (array.Length > 1)
			{
				string text = Enumerable.FirstOrDefault<string>((IEnumerable<string>)array, (Func<string, bool>)((string x) => string.Compare(x, name, StringComparison.OrdinalIgnoreCase) == 0));
				if (text == null)
				{
					Debug.Log((object)$"{arg} failed to spawn \"{name}\"");
					return "Unknown entity - could be:\n\n" + string.Join("\n", array);
				}
				array[0] = text;
			}
			Item item = ItemManager.CreateByName(array[0], 1, 0uL);
			if (item == null)
			{
				Debug.Log((object)$"{arg} failed to spawn \"{array[0]}\" (tried to spawnitem \"{name}\")");
				return "Couldn't spawn " + name;
			}
			BaseEntity arg2 = item.CreateWorldObject(pos);
			Debug.Log((object)$"{arg} spawned \"{arg2}\" at {pos} (via spawnitem)");
			return string.Concat("spawned ", item, " at ", pos);
		}

		[ServerVar]
		public static void spawnlootfrom(Arg args)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = args.Player();
			string @string = args.GetString(0, string.Empty);
			int @int = args.GetInt(1, 1);
			Vector3 vector = args.GetVector3(1, Object.op_Implicit((Object)(object)basePlayer) ? basePlayer.CenterPoint() : Vector3.get_zero());
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(@string, vector);
			if ((Object)(object)baseEntity == (Object)null)
			{
				return;
			}
			baseEntity.Spawn();
			basePlayer.ChatMessage("Contents of " + @string + " spawned " + @int + " times");
			LootContainer component = ((Component)baseEntity).GetComponent<LootContainer>();
			if ((Object)(object)component != (Object)null)
			{
				for (int i = 0; i < @int * component.maxDefinitionsToSpawn; i++)
				{
					component.lootDefinition.SpawnIntoContainer(basePlayer.inventory.containerMain);
				}
			}
			baseEntity.Kill();
		}

		public static int DeleteBy(ulong id)
		{
			List<ulong> list = Pool.GetList<ulong>();
			list.Add(id);
			int result = DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			return result;
		}

		[ServerVar(Help = "Destroy all entities created by provided users (separate users by space)")]
		public static int DeleteBy(Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return 0;
			}
			List<ulong> list = Pool.GetList<ulong>();
			string[] args = arg.Args;
			for (int i = 0; i < args.Length; i++)
			{
				if (ulong.TryParse(args[i], out var result))
				{
					list.Add(result);
				}
			}
			int result2 = DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			return result2;
		}

		private static int DeleteBy(List<ulong> ids)
		{
			int num = 0;
			foreach (BaseEntity serverEntity in BaseNetworkable.serverEntities)
			{
				if ((Object)(object)serverEntity == (Object)null)
				{
					continue;
				}
				bool flag = false;
				foreach (ulong id in ids)
				{
					if (serverEntity.OwnerID == id)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					((FacepunchBehaviour)serverEntity).Invoke((Action)serverEntity.KillMessage, (float)num * 0.2f);
					num++;
				}
			}
			return num;
		}

		[ServerVar(Help = "Destroy all entities created by users in the provided text block (can use with copied results from ent auth)")]
		public static void DeleteByTextBlock(Arg arg)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (arg.Args.Length != 1)
			{
				arg.ReplyWith("Invalid arguments, provide a text block surrounded by \" and listing player id's at the start of each line");
				return;
			}
			MatchCollection obj = Regex.Matches(arg.GetString(0, ""), "^\\b\\d{17}", (RegexOptions)2);
			List<ulong> list = Pool.GetList<ulong>();
			foreach (Match item in obj)
			{
				if (ulong.TryParse(((Capture)item).get_Value(), out var result))
				{
					list.Add(result);
				}
			}
			int num = DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			arg.ReplyWith($"Destroyed {num} entities");
		}

		public Entity()
			: this()
		{
		}
	}
}
