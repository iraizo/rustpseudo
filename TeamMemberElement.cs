using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class TeamMemberElement : MonoBehaviour
{
	public RustText nameText;

	public RawImage icon;

	public Color onlineColor;

	public Color offlineColor;

	public Color deadColor;

	public GameObject hoverOverlay;

	public RawImage memberIcon;

	public RawImage leaderIcon;

	public RawImage deadIcon;

	public int teamIndex;

	public TeamMemberElement()
		: this()
	{
	}
}
