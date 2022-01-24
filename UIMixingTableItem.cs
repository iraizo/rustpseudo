using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMixingTableItem : MonoBehaviour
{
	public Image ItemIcon;

	public Tooltip ItemTooltip;

	public RustText TextItemNameAndQuantity;

	public UIMixingTableItemIngredient[] Ingredients;

	public void Init(Recipe recipe)
	{
		if ((Object)(object)recipe == (Object)null)
		{
			return;
		}
		ItemIcon.set_sprite(recipe.DisplayIcon);
		((TMP_Text)TextItemNameAndQuantity).set_text(recipe.ProducedItemCount + " x " + recipe.DisplayName);
		ItemTooltip.Text = recipe.DisplayDescription;
		for (int i = 0; i < Ingredients.Length; i++)
		{
			if (i >= recipe.Ingredients.Length)
			{
				Ingredients[i].InitBlank();
			}
			else
			{
				Ingredients[i].Init(recipe.Ingredients[i]);
			}
		}
	}

	public UIMixingTableItem()
		: this()
	{
	}
}
