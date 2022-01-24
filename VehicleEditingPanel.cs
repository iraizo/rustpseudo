using System;
using UnityEngine;
using UnityEngine.UI;

public class VehicleEditingPanel : LootPanel
{
	[Serializable]
	private class CreateChassisEntry
	{
		public byte garageChassisIndex;

		public Button craftButton;

		public Text craftButtonText;

		public Text requirementsText;

		public ItemDefinition GetChassisItemDef(ModularCarGarage garage)
		{
			return garage.chassisBuildOptions[garageChassisIndex].itemDef;
		}
	}

	[SerializeField]
	[Range(0f, 1f)]
	private float disabledAlpha = 0.25f;

	[Header("Edit Vehicle")]
	[SerializeField]
	private CanvasGroup editGroup;

	[SerializeField]
	private GameObject moduleInternalItemsGroup;

	[SerializeField]
	private GameObject moduleInternalLiquidsGroup;

	[SerializeField]
	private GameObject destroyChassisGroup;

	[SerializeField]
	private Button itemTakeButton;

	[SerializeField]
	private Button liquidTakeButton;

	[SerializeField]
	private GameObject liquidHelp;

	[SerializeField]
	private GameObject liquidButton;

	[SerializeField]
	private Color gotColor;

	[SerializeField]
	private Color notGotColor;

	[SerializeField]
	private Text generalInfoText;

	[SerializeField]
	private Text generalWarningText;

	[SerializeField]
	private Image generalWarningImage;

	[SerializeField]
	private Text repairInfoText;

	[SerializeField]
	private Button repairButton;

	[SerializeField]
	private Text destroyChassisButtonText;

	[SerializeField]
	private Text destroyChassisCountdown;

	[SerializeField]
	private Phrase phraseEditingInfo;

	[SerializeField]
	private Phrase phraseNoOccupant;

	[SerializeField]
	private Phrase phraseBadOccupant;

	[SerializeField]
	private Phrase phraseNotDriveable;

	[SerializeField]
	private Phrase phraseNotRepairable;

	[SerializeField]
	private Phrase phraseRepairNotNeeded;

	[SerializeField]
	private Phrase phraseRepairSelectInfo;

	[SerializeField]
	private Phrase phraseRepairEnactInfo;

	[SerializeField]
	private Phrase phraseHasLock;

	[SerializeField]
	private Phrase phraseHasNoLock;

	[SerializeField]
	private Phrase phraseAddLock;

	[SerializeField]
	private Phrase phraseAddKey;

	[SerializeField]
	private Phrase phraseAddLockButton;

	[SerializeField]
	private Phrase phraseCraftKeyButton;

	[SerializeField]
	private Text carLockInfoText;

	[SerializeField]
	private Text carLockButtonText;

	[SerializeField]
	private Button actionLockButton;

	[SerializeField]
	private Button removeLockButton;

	[SerializeField]
	private Phrase phraseEmptyStorage;

	[Header("Create Chassis")]
	[SerializeField]
	private CreateChassisEntry[] chassisOptions;
}
