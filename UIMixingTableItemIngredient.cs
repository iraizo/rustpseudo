using UnityEngine;
using UnityEngine.UI;

public class UIMixingTableItemIngredient : MonoBehaviour
{
	public Image ItemIcon;

	public Text ItemCount;

	public Tooltip ToolTip;

	public void Init(Recipe.RecipeIngredient ingredient)
	{
		ItemIcon.set_sprite(ingredient.Ingredient.iconSprite);
		ItemCount.set_text(ingredient.Count.ToString());
		((Behaviour)ItemIcon).set_enabled(true);
		((Behaviour)ItemCount).set_enabled(true);
		ToolTip.Text = ingredient.Count + " x " + ingredient.Ingredient.displayName.get_translated();
		((Behaviour)ToolTip).set_enabled(true);
	}

	public void InitBlank()
	{
		((Behaviour)ItemIcon).set_enabled(false);
		((Behaviour)ItemCount).set_enabled(false);
		((Behaviour)ToolTip).set_enabled(false);
	}

	public UIMixingTableItemIngredient()
		: this()
	{
	}
}
