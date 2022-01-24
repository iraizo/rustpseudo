using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevDressPlayer : MonoBehaviour
{
	public bool DressRandomly;

	public List<ItemAmount> clothesToWear;

	private void ServerInitComponent()
	{
		BasePlayer component = ((Component)this).GetComponent<BasePlayer>();
		if (DressRandomly)
		{
			DoRandomClothes(component);
		}
		foreach (ItemAmount item in clothesToWear)
		{
			if (!((Object)(object)item.itemDef == (Object)null))
			{
				ItemManager.Create(item.itemDef, 1, 0uL).MoveToContainer(component.inventory.containerWear);
			}
		}
	}

	private void DoRandomClothes(BasePlayer player)
	{
		string text = "";
		foreach (ItemDefinition item in Enumerable.Take<ItemDefinition>((IEnumerable<ItemDefinition>)Enumerable.OrderBy<ItemDefinition, Guid>(Enumerable.Where<ItemDefinition>((IEnumerable<ItemDefinition>)ItemManager.GetItemDefinitions(), (Func<ItemDefinition, bool>)((ItemDefinition x) => Object.op_Implicit((Object)(object)((Component)x).GetComponent<ItemModWearable>()))), (Func<ItemDefinition, Guid>)((ItemDefinition x) => Guid.NewGuid())), Random.Range(0, 4)))
		{
			ItemManager.Create(item, 1, 0uL).MoveToContainer(player.inventory.containerWear);
			text = text + item.shortname + " ";
		}
		text = text.Trim();
		if (text == "")
		{
			text = "naked";
		}
		player.displayName = text;
	}

	public DevDressPlayer()
		: this()
	{
	}
}
