using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Recipe")]
public class Recipe : ScriptableObject
{
	[Serializable]
	public struct RecipeIngredient
	{
		public ItemDefinition Ingredient;

		public int Count;
	}

	[Header("Produced Item")]
	public ItemDefinition ProducedItem;

	public int ProducedItemCount = 1;

	public bool CanQueueMultiple = true;

	[Header("Spawned Item")]
	public GameObjectRef SpawnedItem;

	public string SpawnedItemName;

	public string SpawnedItemDescription;

	public Sprite SpawnedItemIcon;

	[Header("Misc")]
	public bool RequiresBlueprint;

	public RecipeIngredient[] Ingredients;

	public float MixingDuration;

	public string DisplayName
	{
		get
		{
			if ((Object)(object)ProducedItem != (Object)null)
			{
				return ProducedItem.displayName.get_translated();
			}
			if (SpawnedItem != null)
			{
				return SpawnedItemName;
			}
			return null;
		}
	}

	public string DisplayDescription
	{
		get
		{
			if ((Object)(object)ProducedItem != (Object)null)
			{
				return ProducedItem.displayDescription.get_translated();
			}
			if (SpawnedItem != null)
			{
				return SpawnedItemDescription;
			}
			return null;
		}
	}

	public Sprite DisplayIcon
	{
		get
		{
			if ((Object)(object)ProducedItem != (Object)null)
			{
				return ProducedItem.iconSprite;
			}
			if (SpawnedItem != null)
			{
				return SpawnedItemIcon;
			}
			return null;
		}
	}

	public bool ContainsItem(Item item)
	{
		if (item == null)
		{
			return false;
		}
		if (Ingredients == null)
		{
			return false;
		}
		RecipeIngredient[] ingredients = Ingredients;
		for (int i = 0; i < ingredients.Length; i++)
		{
			RecipeIngredient recipeIngredient = ingredients[i];
			if ((Object)(object)item.info == (Object)(object)recipeIngredient.Ingredient)
			{
				return true;
			}
		}
		return false;
	}

	public Recipe()
		: this()
	{
	}
}
