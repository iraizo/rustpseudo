using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	[Factory("inventory")]
	public class Inventory : ConsoleSystem
	{
		private class SavedLoadout
		{
			public struct SavedItem
			{
				public int id;

				public int amount;

				public ulong skin;

				public int[] containedItems;
			}

			public SavedItem[] belt;

			public SavedItem[] wear;

			public int heldItemIndex;

			public SavedLoadout()
			{
			}

			public SavedLoadout(BasePlayer player)
			{
				belt = SaveItems(player.inventory.containerBelt);
				wear = SaveItems(player.inventory.containerWear);
				heldItemIndex = GetSlotIndex(player);
			}

			private static SavedItem[] SaveItems(ItemContainer itemContainer)
			{
				List<SavedItem> list = new List<SavedItem>();
				for (int i = 0; i < itemContainer.capacity; i++)
				{
					Item slot = itemContainer.GetSlot(i);
					if (slot == null)
					{
						continue;
					}
					SavedItem savedItem = default(SavedItem);
					savedItem.id = slot.info.itemid;
					savedItem.amount = slot.amount;
					savedItem.skin = slot.skin;
					SavedItem item = savedItem;
					if (slot.contents != null && slot.contents.itemList != null)
					{
						List<int> list2 = new List<int>();
						foreach (Item item2 in slot.contents.itemList)
						{
							list2.Add(item2.info.itemid);
						}
						item.containedItems = list2.ToArray();
					}
					list.Add(item);
				}
				return list.ToArray();
			}

			public void LoadItemsOnTo(BasePlayer player)
			{
				player.inventory.containerBelt.Clear();
				player.inventory.containerWear.Clear();
				SavedItem[] array = belt;
				foreach (SavedItem item in array)
				{
					player.inventory.GiveItem(LoadItem(item), player.inventory.containerBelt);
				}
				array = wear;
				foreach (SavedItem item2 in array)
				{
					player.inventory.GiveItem(LoadItem(item2), player.inventory.containerWear);
				}
				EquipItemInSlot(player, heldItemIndex);
			}

			private Item LoadItem(SavedItem item)
			{
				Item item2 = ItemManager.CreateByItemID(item.id, item.amount, item.skin);
				if (item.containedItems != null && item.containedItems.Length != 0)
				{
					int[] containedItems = item.containedItems;
					foreach (int itemID in containedItems)
					{
						item2.contents.AddItem(ItemManager.FindItemDefinition(itemID), 1, 0uL);
					}
				}
				return item2;
			}
		}

		private const string LoadoutDirectory = "loadouts";

		[ServerUserVar]
		public static void lighttoggle(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && !basePlayer.IsDead() && !basePlayer.IsSleeping() && !basePlayer.InGesture)
			{
				basePlayer.LightToggle();
			}
		}

		[ServerUserVar]
		public static void endloot(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && !basePlayer.IsDead() && !basePlayer.IsSleeping())
			{
				basePlayer.inventory.loot.Clear();
			}
		}

		[ServerVar]
		public static void give(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, arg.GetULong(3, 0uL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int num = (item.amount = arg.GetInt(1, 1));
			float num2 = (item.conditionNormalized = arg.GetFloat(2, 1f));
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item))
			{
				item.Remove();
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", item.info.itemid, num);
			Debug.Log((object)("giving " + basePlayer.displayName + " " + num + " x " + item.info.displayName.english));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently gave yourself " + num + " x " + item.info.displayName.english);
			}
			else
			{
				Chat.Broadcast(basePlayer.displayName + " gave themselves " + num + " x " + item.info.displayName.english, "SERVER", "#eee", 0uL);
			}
		}

		[ServerVar]
		public static void resetbp(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				basePlayer.blueprints.Reset();
			}
		}

		[ServerVar]
		public static void unlockall(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				basePlayer.blueprints.UnlockAll();
			}
		}

		[ServerVar]
		public static void giveall(Arg arg)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Item item = null;
			string text = "SERVER";
			if ((Object)(object)arg.Player() != (Object)null)
			{
				text = arg.Player().displayName;
			}
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, 0uL);
					if (item == null)
					{
						arg.ReplyWith("Invalid Item!");
						return;
					}
					item.amount = arg.GetInt(1, 1);
					item.OnVirginSpawn();
					if (!current.inventory.GiveItem(item))
					{
						item.Remove();
						arg.ReplyWith("Couldn't give item (inventory full?)");
						continue;
					}
					current.Command("note.inv", item.info.itemid, item.amount);
					Debug.Log((object)(" [ServerVar] giving " + current.displayName + " " + item.amount + " x " + item.info.displayName.english));
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			if (item != null)
			{
				Chat.Broadcast(text + " gave everyone " + item.amount + " x " + item.info.displayName.english, "SERVER", "#eee", 0uL);
			}
		}

		[ServerVar]
		public static void giveto(Arg arg)
		{
			string text = "SERVER";
			if ((Object)(object)arg.Player() != (Object)null)
			{
				text = arg.Player().displayName;
			}
			BasePlayer basePlayer = BasePlayer.Find(arg.GetString(0, ""));
			if ((Object)(object)basePlayer == (Object)null)
			{
				arg.ReplyWith("Couldn't find player!");
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(1, ""), 1, arg.GetULong(3, 0uL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			item.amount = arg.GetInt(2, 1);
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item))
			{
				item.Remove();
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", item.info.itemid, item.amount);
			Debug.Log((object)(" [ServerVar] giving " + basePlayer.displayName + " " + item.amount + " x " + item.info.displayName.english));
			Chat.Broadcast(text + " gave " + basePlayer.displayName + " " + item.amount + " x " + item.info.displayName.english, "SERVER", "#eee", 0uL);
		}

		[ServerVar]
		public static void giveid(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0uL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			item.amount = arg.GetInt(1, 1);
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item))
			{
				item.Remove();
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", item.info.itemid, item.amount);
			Debug.Log((object)(" [ServerVar] giving " + basePlayer.displayName + " " + item.amount + " x " + item.info.displayName.english));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently gave yourself " + item.amount + " x " + item.info.displayName.english);
			}
			else
			{
				Chat.Broadcast(basePlayer.displayName + " gave themselves " + item.amount + " x " + item.info.displayName.english, "SERVER", "#eee", 0uL);
			}
		}

		[ServerVar]
		public static void givearm(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0uL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			item.amount = arg.GetInt(1, 1);
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, basePlayer.inventory.containerBelt))
			{
				item.Remove();
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", item.info.itemid, item.amount);
			Debug.Log((object)(" [ServerVar] giving " + basePlayer.displayName + " " + item.amount + " x " + item.info.displayName.english));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently gave yourself " + item.amount + " x " + item.info.displayName.english);
			}
			else
			{
				Chat.Broadcast(basePlayer.displayName + " gave themselves " + item.amount + " x " + item.info.displayName.english, "SERVER", "#eee", 0uL);
			}
		}

		[ServerVar(Help = "Copies the players inventory to the player in front of them")]
		public static void copyTo(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if ((!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic) || (Object)(object)basePlayer == (Object)null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(1) && arg.GetString(0, "").ToLower() != "true")
			{
				basePlayer2 = arg.GetPlayer(0);
				if ((Object)(object)basePlayer2 == (Object)null)
				{
					uint uInt = arg.GetUInt(0, 0u);
					basePlayer2 = BasePlayer.FindByID(uInt);
					if ((Object)(object)basePlayer2 == (Object)null)
					{
						basePlayer2 = BasePlayer.FindBot(uInt);
					}
				}
			}
			else
			{
				basePlayer2 = RelationshipManager.GetLookingAtPlayer(basePlayer);
			}
			if ((Object)(object)basePlayer2 == (Object)null)
			{
				return;
			}
			basePlayer2.inventory.containerBelt.Clear();
			basePlayer2.inventory.containerWear.Clear();
			int num = 0;
			foreach (Item item2 in basePlayer.inventory.containerBelt.itemList)
			{
				basePlayer2.inventory.containerBelt.AddItem(item2.info, item2.amount, item2.skin);
				if (item2.contents != null)
				{
					Item item = basePlayer2.inventory.containerBelt.itemList[num];
					foreach (Item item3 in item2.contents.itemList)
					{
						item.contents.AddItem(item3.info, item3.amount, item3.skin);
					}
				}
				num++;
			}
			foreach (Item item4 in basePlayer.inventory.containerWear.itemList)
			{
				basePlayer2.inventory.containerWear.AddItem(item4.info, item4.amount, item4.skin);
			}
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently copied items to " + basePlayer2.displayName);
			}
			else
			{
				Chat.Broadcast(basePlayer.displayName + " copied their inventory to " + basePlayer2.displayName, "SERVER", "#eee", 0uL);
			}
		}

		[ServerVar(Help = "Deploys a loadout to players in a radius eg. inventory.deployLoadoutInRange testloadout 30")]
		public static void deployLoadoutInRange(Arg arg)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if ((!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic) || (Object)(object)basePlayer == (Object)null)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			if (!LoadLoadout(@string, out var so))
			{
				arg.ReplyWith("Can't find loadout: " + @string);
				return;
			}
			float @float = arg.GetFloat(1, 0f);
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			global::Vis.Entities(((Component)basePlayer).get_transform().get_position(), @float, list, 131072, (QueryTriggerInteraction)2);
			int num = 0;
			foreach (BasePlayer item in list)
			{
				if (!((Object)(object)item == (Object)(object)basePlayer) && !item.isClient)
				{
					so.LoadItemsOnTo(item);
					num++;
				}
			}
			arg.ReplyWith($"Applied loadout {@string} to {num} players");
			Pool.FreeList<BasePlayer>(ref list);
		}

		[ServerVar(Help = "Deploys the given loadout to a target player. eg. inventory.deployLoadout testloadout jim")]
		public static void deployLoadout(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if ((basePlayer.IsAdmin || basePlayer.IsDeveloper || Server.cinematic) && !((Object)(object)basePlayer == (Object)null))
			{
				string @string = arg.GetString(0, "");
				BasePlayer playerOrSleeperOrBot = arg.GetPlayerOrSleeperOrBot(1);
				SavedLoadout so;
				if ((Object)(object)playerOrSleeperOrBot == (Object)null)
				{
					arg.ReplyWith("Could not find player " + arg.GetString(1, ""));
				}
				else if (LoadLoadout(@string, out so))
				{
					so.LoadItemsOnTo(playerOrSleeperOrBot);
					arg.ReplyWith("Deployed loadout " + @string + " to " + playerOrSleeperOrBot.displayName);
				}
				else
				{
					arg.ReplyWith("Could not find loadout " + @string);
				}
			}
		}

		private static string GetLoadoutPath(string loadoutName)
		{
			return Server.GetServerFolder("loadouts") + "/" + loadoutName + ".ldt";
		}

		[ServerVar(Help = "Saves the current equipped loadout of the calling player. eg. inventory.saveLoadout loaduoutname")]
		public static void saveloadout(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!((Object)(object)basePlayer == (Object)null) && (basePlayer.IsAdmin || basePlayer.IsDeveloper || Server.cinematic))
			{
				string @string = arg.GetString(0, "");
				string text = JsonConvert.SerializeObject((object)new SavedLoadout(basePlayer), (Formatting)1);
				string loadoutPath = GetLoadoutPath(@string);
				File.WriteAllText(loadoutPath, text);
				arg.ReplyWith("Saved loadout to " + loadoutPath);
			}
		}

		private static bool LoadLoadout(string name, out SavedLoadout so)
		{
			so = new SavedLoadout();
			string loadoutPath = GetLoadoutPath(name);
			if (!File.Exists(loadoutPath))
			{
				return false;
			}
			so = JsonConvert.DeserializeObject<SavedLoadout>(File.ReadAllText(loadoutPath));
			if (so == null)
			{
				return false;
			}
			return true;
		}

		[ServerVar(Help = "Prints all saved inventory loadouts")]
		public static void listloadouts(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null || (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic))
			{
				return;
			}
			string serverFolder = Server.GetServerFolder("loadouts");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in Directory.EnumerateFiles(serverFolder))
			{
				stringBuilder.AppendLine(item);
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void defs(Arg arg)
		{
			if (SteamInventory.get_Definitions() == null)
			{
				arg.ReplyWith("no definitions");
				return;
			}
			if (SteamInventory.get_Definitions().Length == 0)
			{
				arg.ReplyWith("0 definitions");
				return;
			}
			string[] array = Enumerable.ToArray<string>(Enumerable.Select<InventoryDef, string>((IEnumerable<InventoryDef>)SteamInventory.get_Definitions(), (Func<InventoryDef, string>)((InventoryDef x) => x.get_Name())));
			arg.ReplyWith((object)array);
		}

		[ClientVar]
		[ServerVar]
		public static void reloaddefs(Arg arg)
		{
			SteamInventory.LoadItemDefinitions();
		}

		[ServerVar]
		public static void equipslottarget(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if ((basePlayer.IsAdmin || basePlayer.IsDeveloper || Server.cinematic) && !((Object)(object)basePlayer == (Object)null))
			{
				BasePlayer lookingAtPlayer = RelationshipManager.GetLookingAtPlayer(basePlayer);
				if (!((Object)(object)lookingAtPlayer == (Object)null))
				{
					int @int = arg.GetInt(0, 0);
					EquipItemInSlot(lookingAtPlayer, @int);
					arg.ReplyWith($"Equipped slot {@int} on player {lookingAtPlayer.displayName}");
				}
			}
		}

		[ServerVar]
		public static void equipslot(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if ((!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic) || (Object)(object)basePlayer == (Object)null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(2))
			{
				basePlayer2 = arg.GetPlayer(1);
				if ((Object)(object)basePlayer2 == (Object)null)
				{
					uint uInt = arg.GetUInt(1, 0u);
					basePlayer2 = BasePlayer.FindByID(uInt);
					if ((Object)(object)basePlayer2 == (Object)null)
					{
						basePlayer2 = BasePlayer.FindBot(uInt);
					}
				}
			}
			if (!((Object)(object)basePlayer2 == (Object)null))
			{
				int @int = arg.GetInt(0, 0);
				EquipItemInSlot(basePlayer2, @int);
				Debug.Log((object)$"Equipped slot {@int} on player {basePlayer2.displayName}");
			}
		}

		private static void EquipItemInSlot(BasePlayer player, int slot)
		{
			uint itemID = 0u;
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && i == slot)
				{
					itemID = player.inventory.containerBelt.itemList[i].uid;
					break;
				}
			}
			player.UpdateActiveItem(itemID);
		}

		private static int GetSlotIndex(BasePlayer player)
		{
			if (player.GetActiveItem() == null)
			{
				return -1;
			}
			uint uid = player.GetActiveItem().uid;
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && player.inventory.containerBelt.itemList[i].uid == uid)
				{
					return i;
				}
			}
			return -1;
		}

		public Inventory()
			: this()
		{
		}
	}
}
