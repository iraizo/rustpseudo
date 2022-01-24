using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyCodeEntry : UIDialog
{
	public Text textDisplay;

	public Action<string> onCodeEntered;

	public Text typeDisplay;

	public Phrase masterCodePhrase;

	public Phrase guestCodePhrase;

	public GameObject memoryKeycodeButton;
}
