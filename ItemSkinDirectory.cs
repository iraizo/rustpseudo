using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSkinDirectory : ScriptableObject
{
	[Serializable]
	public struct Skin
	{
		public int id;

		public int itemid;

		public string name;

		public bool isSkin;

		private SteamInventoryItem _invItem;

		public SteamInventoryItem invItem
		{
			get
			{
				if ((Object)(object)_invItem == (Object)null && !string.IsNullOrEmpty(name))
				{
					_invItem = FileSystem.Load<SteamInventoryItem>(name, true);
				}
				return _invItem;
			}
		}
	}

	private static ItemSkinDirectory _Instance;

	public Skin[] skins;

	public static ItemSkinDirectory Instance
	{
		get
		{
			if ((Object)(object)_Instance == (Object)null)
			{
				_Instance = FileSystem.Load<ItemSkinDirectory>("assets/skins.asset", true);
				if ((Object)(object)_Instance == (Object)null)
				{
					throw new Exception("Couldn't load assets/skins.asset");
				}
				if (_Instance.skins == null || _Instance.skins.Length == 0)
				{
					throw new Exception("Loaded assets/skins.asset but something is wrong");
				}
			}
			return _Instance;
		}
	}

	public static Skin[] ForItem(ItemDefinition item)
	{
		return Enumerable.ToArray<Skin>(Enumerable.Where<Skin>((IEnumerable<Skin>)Instance.skins, (Func<Skin, bool>)((Skin x) => x.isSkin && x.itemid == item.itemid)));
	}

	public static Skin FindByInventoryDefinitionId(int id)
	{
		return Enumerable.FirstOrDefault<Skin>(Enumerable.Where<Skin>((IEnumerable<Skin>)Instance.skins, (Func<Skin, bool>)((Skin x) => x.id == id)));
	}

	public ItemSkinDirectory()
		: this()
	{
	}
}
