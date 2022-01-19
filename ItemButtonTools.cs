using UnityEngine;
using UnityEngine.UI;

public class ItemButtonTools : MonoBehaviour
{
	public Image image;

	public ItemDefinition itemDef;

	public void GiveSelf(int amount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ConsoleSystem.Run(Option.get_Client(), "inventory.giveid", new object[2] { itemDef.itemid, amount });
	}

	public void GiveArmed()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ConsoleSystem.Run(Option.get_Client(), "inventory.givearm", new object[1] { itemDef.itemid });
	}

	public void GiveBlueprint()
	{
	}

	public ItemButtonTools()
		: this()
	{
	}
}
