using TMPro;
using UnityEngine;

public class ItemStoreCartItem : MonoBehaviour
{
	public int Index;

	public TextMeshProUGUI Name;

	public TextMeshProUGUI Price;

	public void Init(int index, IPlayerItemDefinition def)
	{
		Index = index;
		((TMP_Text)Name).set_text(def.get_Name());
		((TMP_Text)Price).set_text(def.get_LocalPriceFormatted());
	}

	public ItemStoreCartItem()
		: this()
	{
	}
}
