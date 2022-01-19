using UnityEngine;

public class ItemSelector : PropertyAttribute
{
	public ItemCategory category = ItemCategory.All;

	public ItemSelector(ItemCategory category = ItemCategory.All)
		: this()
	{
		this.category = category;
	}
}
