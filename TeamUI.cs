using System;
using TMPro;
using UnityEngine;

public class TeamUI : MonoBehaviour
{
	public static Phrase invitePhrase = new Phrase("team.invited", "{0} has invited you to join a team");

	public RectTransform MemberPanel;

	public GameObject memberEntryPrefab;

	public TeamMemberElement[] elements;

	public GameObject NoTeamPanel;

	public GameObject TeamPanel;

	public GameObject LeaveTeamButton;

	public GameObject InviteAcceptPanel;

	public TextMeshProUGUI inviteText;

	public static bool dirty = true;

	[NonSerialized]
	public static ulong pendingTeamID;

	[NonSerialized]
	public static string pendingTeamLeaderName;

	public TeamUI()
		: this()
	{
	}
}
