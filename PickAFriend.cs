using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class PickAFriend : UIDialog
{
	public InputField input;

	public RustText headerText;

	public bool AutoSelectInputField;

	public Action<ulong, string> onSelected;

	public Phrase sleepingBagHeaderPhrase = new Phrase("assign_to_friend", "Assign To a Friend");

	public Phrase turretHeaderPhrase = new Phrase("authorize_a_friend", "Authorize a Friend");

	public SteamFriendsList friendsList;

	public Func<ulong, bool> shouldShowPlayer
	{
		set
		{
			if ((Object)(object)friendsList != (Object)null)
			{
				friendsList.shouldShowPlayer = value;
			}
		}
	}
}
