using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemModConditionHasContents : ItemMod
{
	[Tooltip("Can be null to mean any item")]
	public ItemDefinition itemDef;

	public bool requiredState;

	public override bool Passes(Item item)
	{
		if (item.contents == null)
		{
			return !requiredState;
		}
		if (item.contents.itemList.Count == 0)
		{
			return !requiredState;
		}
		if (Object.op_Implicit((Object)(object)itemDef) && !Enumerable.Any<Item>((IEnumerable<Item>)item.contents.itemList, (Func<Item, bool>)((Item x) => (Object)(object)x.info == (Object)(object)itemDef)))
		{
			return !requiredState;
		}
		return requiredState;
	}
}
