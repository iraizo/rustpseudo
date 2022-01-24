using UnityEngine;
using UnityEngine.UI;

public class BlueprintHeader : MonoBehaviour
{
	public Text categoryName;

	public Text unlockCount;

	public void Setup(ItemCategory name, int unlocked, int total)
	{
		categoryName.set_text(name.ToString().ToUpper());
		unlockCount.set_text($"UNLOCKED {unlocked}/{total}");
	}

	public BlueprintHeader()
		: this()
	{
	}
}
