using System;
using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemListTools : MonoBehaviour
{
	public GameObject categoryButton;

	public GameObject itemButton;

	public RustInput searchInputText;

	internal Button lastCategory;

	private IOrderedEnumerable<ItemDefinition> currentItems;

	private IOrderedEnumerable<ItemDefinition> allItems;

	public void OnPanelOpened()
	{
		CacheAllItems();
		Refresh();
		searchInputText.InputField.ActivateInputField();
	}

	private void OnOpenDevTools()
	{
		searchInputText.InputField.ActivateInputField();
	}

	private void CacheAllItems()
	{
		if (allItems == null)
		{
			allItems = Enumerable.OrderBy<ItemDefinition, string>((IEnumerable<ItemDefinition>)ItemManager.GetItemDefinitions(), (Func<ItemDefinition, string>)((ItemDefinition x) => x.displayName.get_translated()));
		}
	}

	public void Refresh()
	{
		RebuildCategories();
	}

	private void RebuildCategories()
	{
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		for (int i = 0; i < categoryButton.get_transform().get_parent().get_childCount(); i++)
		{
			Transform child = categoryButton.get_transform().get_parent().GetChild(i);
			if (!((Object)(object)child == (Object)(object)categoryButton.get_transform()))
			{
				GameManager.Destroy(((Component)child).get_gameObject());
			}
		}
		categoryButton.SetActive(true);
		foreach (IGrouping<ItemCategory, ItemDefinition> item in (IEnumerable<IGrouping<ItemCategory, ItemDefinition>>)Enumerable.OrderBy<IGrouping<ItemCategory, ItemDefinition>, ItemCategory>(Enumerable.GroupBy<ItemDefinition, ItemCategory>((IEnumerable<ItemDefinition>)ItemManager.GetItemDefinitions(), (Func<ItemDefinition, ItemCategory>)((ItemDefinition x) => x.category)), (Func<IGrouping<ItemCategory, ItemDefinition>, ItemCategory>)((IGrouping<ItemCategory, ItemDefinition> x) => Enumerable.First<ItemDefinition>((IEnumerable<ItemDefinition>)x).category)))
		{
			GameObject val = Object.Instantiate<GameObject>(categoryButton);
			val.get_transform().SetParent(categoryButton.get_transform().get_parent(), false);
			((TMP_Text)val.GetComponentInChildren<TextMeshProUGUI>()).set_text(Enumerable.First<ItemDefinition>((IEnumerable<ItemDefinition>)item).category.ToString());
			Button btn = val.GetComponentInChildren<Button>();
			ItemDefinition[] itemArray = Enumerable.ToArray<ItemDefinition>((IEnumerable<ItemDefinition>)item);
			((UnityEvent)btn.get_onClick()).AddListener((UnityAction)delegate
			{
				if (Object.op_Implicit((Object)(object)lastCategory))
				{
					((Selectable)lastCategory).set_interactable(true);
				}
				lastCategory = btn;
				((Selectable)lastCategory).set_interactable(false);
				SwitchItemCategory(itemArray);
			});
			if ((Object)(object)lastCategory == (Object)null)
			{
				lastCategory = btn;
				((Selectable)lastCategory).set_interactable(false);
				SwitchItemCategory(itemArray);
			}
		}
		categoryButton.SetActive(false);
	}

	private void SwitchItemCategory(ItemDefinition[] defs)
	{
		currentItems = Enumerable.OrderBy<ItemDefinition, string>((IEnumerable<ItemDefinition>)defs, (Func<ItemDefinition, string>)((ItemDefinition x) => x.displayName.get_translated()));
		searchInputText.set_Text("");
		FilterItems(null);
	}

	public void FilterItems(string searchText)
	{
		if ((Object)(object)itemButton == (Object)null)
		{
			return;
		}
		for (int i = 0; i < itemButton.get_transform().get_parent().get_childCount(); i++)
		{
			Transform child = itemButton.get_transform().get_parent().GetChild(i);
			if (!((Object)(object)child == (Object)(object)itemButton.get_transform()))
			{
				GameManager.Destroy(((Component)child).get_gameObject());
			}
		}
		itemButton.SetActive(true);
		bool flag = !string.IsNullOrEmpty(searchText);
		string value = (flag ? searchText.ToLower() : null);
		IOrderedEnumerable<ItemDefinition> obj = (flag ? allItems : currentItems);
		int num = 0;
		foreach (ItemDefinition item in (IEnumerable<ItemDefinition>)obj)
		{
			if (!item.hidden && (!flag || item.displayName.get_translated().ToLower().Contains(value)))
			{
				GameObject obj2 = Object.Instantiate<GameObject>(itemButton);
				obj2.get_transform().SetParent(itemButton.get_transform().get_parent(), false);
				((TMP_Text)obj2.GetComponentInChildren<TextMeshProUGUI>()).set_text(item.displayName.get_translated());
				obj2.GetComponentInChildren<ItemButtonTools>().itemDef = item;
				obj2.GetComponentInChildren<ItemButtonTools>().image.set_sprite(item.iconSprite);
				num++;
				if (num >= 160)
				{
					break;
				}
			}
		}
		itemButton.SetActive(false);
	}

	public ItemListTools()
		: this()
	{
	}
}
