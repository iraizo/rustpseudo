using UnityEngine;

public class ItemInformationPanel : MonoBehaviour
{
	public virtual bool EligableForDisplay(ItemDefinition info)
	{
		Debug.LogWarning((object)"ItemInformationPanel.EligableForDisplay");
		return false;
	}

	public virtual void SetupForItem(ItemDefinition info, Item item = null)
	{
		Debug.LogWarning((object)"ItemInformationPanel.SetupForItem");
	}

	public ItemInformationPanel()
		: this()
	{
	}
}
