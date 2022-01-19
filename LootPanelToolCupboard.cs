using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootPanelToolCupboard : LootPanel
{
	public List<VirtualItemIcon> costIcons;

	public Text costPerTimeText;

	public Text protectedText;

	public GameObject baseNotProtectedObj;

	public GameObject baseProtectedObj;

	public Phrase protectedPrefix;

	public Tooltip costToolTip;

	public Phrase blocksPhrase;
}
