using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBelt : SingletonComponent<UIBelt>
{
	public List<ItemIcon> ItemIcons;

	protected override void Awake()
	{
		ItemIcons = Enumerable.ToList<ItemIcon>((IEnumerable<ItemIcon>)Enumerable.OrderBy<ItemIcon, int>((IEnumerable<ItemIcon>)((Component)this).GetComponentsInChildren<ItemIcon>(), (Func<ItemIcon, int>)((ItemIcon s) => s.slot)));
	}

	public ItemIcon GetItemIconAtSlot(int slot)
	{
		if (slot < 0 || slot >= ItemIcons.Count)
		{
			return null;
		}
		return ItemIcons[slot];
	}
}
