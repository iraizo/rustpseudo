using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemManager
{
	private struct ItemRemove
	{
		public Item item;

		public float time;
	}

	public static List<ItemDefinition> itemList;

	public static Dictionary<int, ItemDefinition> itemDictionary;

	public static Dictionary<string, ItemDefinition> itemDictionaryByName;

	public static List<ItemBlueprint> bpList;

	public static int[] defaultBlueprints;

	public static ItemDefinition blueprintBaseDef;

	private static List<ItemRemove> ItemRemoves = new List<ItemRemove>();

	public static void InvalidateWorkshopSkinCache()
	{
		if (itemList == null)
		{
			return;
		}
		foreach (ItemDefinition item in itemList)
		{
			item.InvalidateWorkshopSkinCache();
		}
	}

	public static void Initialize()
	{
		if (itemList != null)
		{
			return;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		GameObject[] array = FileSystem.LoadAllFromBundle<GameObject>("items.preload.bundle", "l:ItemDefinition");
		if (array.Length == 0)
		{
			throw new Exception("items.preload.bundle has no items!");
		}
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			Debug.Log((object)("Loading Items Took: " + stopwatch.Elapsed.TotalMilliseconds / 1000.0 + " seconds"));
		}
		List<ItemDefinition> list = (from x in array
			select x.GetComponent<ItemDefinition>() into x
			where (Object)(object)x != (Object)null
			select x).ToList();
		List<ItemBlueprint> list2 = (from x in array
			select x.GetComponent<ItemBlueprint>() into x
			where (Object)(object)x != (Object)null && x.userCraftable
			select x).ToList();
		Dictionary<int, ItemDefinition> dictionary = new Dictionary<int, ItemDefinition>();
		Dictionary<string, ItemDefinition> dictionary2 = new Dictionary<string, ItemDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (ItemDefinition item in list)
		{
			item.Initialize(list);
			if (dictionary.ContainsKey(item.itemid))
			{
				ItemDefinition itemDefinition = dictionary[item.itemid];
				Debug.LogWarning((object)("Item ID duplicate " + item.itemid + " (" + ((Object)item).get_name() + ") - have you given your items unique shortnames?"), (Object)(object)((Component)item).get_gameObject());
				Debug.LogWarning((object)("Other item is " + ((Object)itemDefinition).get_name()), (Object)(object)itemDefinition);
			}
			else if (string.IsNullOrEmpty(item.shortname))
			{
				Debug.LogWarning((object)$"{item} has a null short name! id: {item.itemid} {item.displayName.english}");
			}
			else
			{
				dictionary.Add(item.itemid, item);
				dictionary2.Add(item.shortname, item);
			}
		}
		stopwatch.Stop();
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			Debug.Log((object)("Building Items Took: " + stopwatch.Elapsed.TotalMilliseconds / 1000.0 + " seconds / Items: " + list.Count + " / Blueprints: " + list2.Count));
		}
		defaultBlueprints = (from x in list2
			where !x.NeedsSteamItem && !x.NeedsSteamDLC && x.defaultBlueprint
			select x.targetItem.itemid).ToArray();
		itemList = list;
		bpList = list2;
		itemDictionary = dictionary;
		itemDictionaryByName = dictionary2;
		blueprintBaseDef = FindItemDefinition("blueprintbase");
	}

	public static Item CreateByName(string strName, int iAmount = 1, ulong skin = 0uL)
	{
		ItemDefinition itemDefinition = itemList.Find((ItemDefinition x) => x.shortname == strName);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return null;
		}
		return CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	public static Item CreateByPartialName(string strName, int iAmount = 1, ulong skin = 0uL)
	{
		ItemDefinition itemDefinition = itemList.Find((ItemDefinition x) => x.shortname == strName);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			itemDefinition = itemList.Find((ItemDefinition x) => StringEx.Contains(x.shortname, strName, CompareOptions.IgnoreCase));
		}
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return null;
		}
		return CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	public static Item CreateByItemID(int itemID, int iAmount = 1, ulong skin = 0uL)
	{
		ItemDefinition itemDefinition = FindItemDefinition(itemID);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return null;
		}
		return Create(itemDefinition, iAmount, skin);
	}

	public static Item Create(ItemDefinition template, int iAmount = 1, ulong skin = 0uL)
	{
		TrySkinChangeItem(ref template, ref skin);
		if ((Object)(object)template == (Object)null)
		{
			Debug.LogWarning((object)"Creating invalid/missing item!");
			return null;
		}
		Item item = new Item();
		item.isServer = true;
		if (iAmount <= 0)
		{
			Debug.LogError((object)("Creating item with less than 1 amount! (" + template.displayName.english + ")"));
			return null;
		}
		item.info = template;
		item.amount = iAmount;
		item.skin = skin;
		item.Initialize(template);
		return item;
	}

	private static void TrySkinChangeItem(ref ItemDefinition template, ref ulong skinId)
	{
		if (skinId == 0L)
		{
			return;
		}
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId((int)skinId);
		if (skin.id != 0)
		{
			ItemSkin itemSkin = skin.invItem as ItemSkin;
			if (!((Object)(object)itemSkin == (Object)null) && !((Object)(object)itemSkin.Redirect == (Object)null))
			{
				template = itemSkin.Redirect;
				skinId = 0uL;
			}
		}
	}

	public static Item Load(Item load, Item created, bool isServer)
	{
		if (created == null)
		{
			created = new Item();
		}
		created.isServer = isServer;
		created.Load(load);
		if ((Object)(object)created.info == (Object)null)
		{
			Debug.LogWarning((object)"Item loading failed - item is invalid");
			return null;
		}
		if ((Object)(object)created.info == (Object)(object)blueprintBaseDef && (Object)(object)created.blueprintTargetDef == (Object)null)
		{
			Debug.LogWarning((object)"Blueprint item loading failed - invalid item target");
			return null;
		}
		return created;
	}

	public static ItemDefinition FindItemDefinition(int itemID)
	{
		Initialize();
		if (itemDictionary.TryGetValue(itemID, out var value))
		{
			return value;
		}
		return null;
	}

	public static ItemDefinition FindItemDefinition(string shortName)
	{
		Initialize();
		if (itemDictionaryByName.TryGetValue(shortName, out var value))
		{
			return value;
		}
		return null;
	}

	public static ItemBlueprint FindBlueprint(ItemDefinition item)
	{
		return ((Component)item).GetComponent<ItemBlueprint>();
	}

	public static List<ItemDefinition> GetItemDefinitions()
	{
		Initialize();
		return itemList;
	}

	public static List<ItemBlueprint> GetBlueprints()
	{
		Initialize();
		return bpList;
	}

	public static void DoRemoves()
	{
		TimeWarning val = TimeWarning.New("DoRemoves", 0);
		try
		{
			for (int i = 0; i < ItemRemoves.Count; i++)
			{
				if (!(ItemRemoves[i].time > Time.get_time()))
				{
					Item item = ItemRemoves[i].item;
					ItemRemoves.RemoveAt(i--);
					item.DoRemove();
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void Heartbeat()
	{
		DoRemoves();
	}

	public static void RemoveItem(Item item, float fTime = 0f)
	{
		Assert.IsTrue(item.isServer, "RemoveItem: Removing a client item!");
		ItemRemove item2 = default(ItemRemove);
		item2.item = item;
		item2.time = Time.get_time() + fTime;
		ItemRemoves.Add(item2);
	}
}
