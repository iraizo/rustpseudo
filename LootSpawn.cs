using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Loot Spawn")]
public class LootSpawn : ScriptableObject
{
	[Serializable]
	public struct Entry
	{
		[Tooltip("If this category is chosen, we will spawn 1+ this amount")]
		public int extraSpawns;

		[Tooltip("If a subcategory exists we'll choose from there instead of any items specified")]
		public LootSpawn category;

		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;
	}

	public ItemAmountRanged[] items;

	public Entry[] subSpawn;

	public ItemDefinition GetBlueprintBaseDef()
	{
		return ItemManager.FindItemDefinition("blueprintbase");
	}

	public void SpawnIntoContainer(ItemContainer container)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (subSpawn != null && subSpawn.Length != 0)
		{
			SubCategoryIntoContainer(container);
		}
		else
		{
			if (items == null)
			{
				return;
			}
			ItemAmountRanged[] array = items;
			foreach (ItemAmountRanged itemAmountRanged in array)
			{
				if (itemAmountRanged == null)
				{
					continue;
				}
				Item item = null;
				if (itemAmountRanged.itemDef.spawnAsBlueprint)
				{
					ItemDefinition blueprintBaseDef = GetBlueprintBaseDef();
					if ((Object)(object)blueprintBaseDef == (Object)null)
					{
						continue;
					}
					Item item2 = ItemManager.Create(blueprintBaseDef, 1, 0uL);
					item2.blueprintTarget = itemAmountRanged.itemDef.itemid;
					item = item2;
				}
				else
				{
					item = ItemManager.CreateByItemID(itemAmountRanged.itemid, (int)itemAmountRanged.GetAmount(), 0uL);
				}
				if (item == null)
				{
					continue;
				}
				item.OnVirginSpawn();
				if (!item.MoveToContainer(container))
				{
					if (Object.op_Implicit((Object)(object)container.playerOwner))
					{
						item.Drop(container.playerOwner.GetDropPosition(), container.playerOwner.GetDropVelocity());
					}
					else
					{
						item.Remove();
					}
				}
			}
		}
	}

	private void SubCategoryIntoContainer(ItemContainer container)
	{
		int num = subSpawn.Sum((Entry x) => x.weight);
		int num2 = Random.Range(0, num);
		for (int i = 0; i < subSpawn.Length; i++)
		{
			if ((Object)(object)subSpawn[i].category == (Object)null)
			{
				continue;
			}
			num -= subSpawn[i].weight;
			if (num2 >= num)
			{
				for (int j = 0; j < 1 + subSpawn[i].extraSpawns; j++)
				{
					subSpawn[i].category.SpawnIntoContainer(container);
				}
				return;
			}
		}
		Debug.LogWarning((object)"SubCategoryIntoContainer: This should never happen!", (Object)(object)this);
	}

	public LootSpawn()
		: this()
	{
	}
}
