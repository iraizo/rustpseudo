using Rust.UI;
using TMPro;
using UnityEngine;

public class ItemStoreItem : MonoBehaviour
{
	public HttpImage Icon;

	public RustText Name;

	public TextMeshProUGUI Price;

	public RustText ItemName;

	public GameObject InCartTag;

	private IPlayerItemDefinition item;

	internal void Init(IPlayerItemDefinition item, bool inCart)
	{
		this.item = item;
		Icon.Load(item.get_IconUrl());
		((TMP_Text)Name).set_text(item.get_Name());
		((TMP_Text)Price).set_text(item.get_LocalPriceFormatted());
		InCartTag.SetActive(inCart);
		if (!string.IsNullOrWhiteSpace(item.get_ItemShortName()))
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.get_ItemShortName());
			if ((Object)(object)itemDefinition != (Object)null)
			{
				ItemName.SetPhrase(itemDefinition.displayName);
			}
			else
			{
				ItemName.SetText("");
			}
		}
		else
		{
			ItemName.SetText("");
		}
	}

	public ItemStoreItem()
		: this()
	{
	}
}
