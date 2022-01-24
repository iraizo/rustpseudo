using UnityEngine;
using UnityEngine.UI;

public class ItemSearchEntry : MonoBehaviour
{
	public Button button;

	public Text text;

	public RawImage image;

	public RawImage bpImage;

	private ItemDefinition itemInfo;

	private AddSellOrderManager manager;

	public ItemSearchEntry()
		: this()
	{
	}
}
