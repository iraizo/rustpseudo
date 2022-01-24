using UnityEngine;
using UnityEngine.UI;

public class WorkbenchPanel : LootPanel, IInventoryChanged
{
	public Button experimentButton;

	public Text timerText;

	public Text costText;

	public GameObject expermentCostParent;

	public GameObject controlsParent;

	public GameObject allUnlockedNotification;

	public GameObject informationParent;

	public GameObject cycleIcon;

	public TechTreeDialog techTreeDialog;
}
