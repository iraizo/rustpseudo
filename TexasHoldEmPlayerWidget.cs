using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class TexasHoldEmPlayerWidget : MonoBehaviour
{
	[SerializeField]
	private RawImage avatar;

	[SerializeField]
	private RustText playerName;

	[SerializeField]
	private RustText scrapTotal;

	[SerializeField]
	private RustText betTotal;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Color inactiveBackground;

	[SerializeField]
	private Color activeBackground;

	[SerializeField]
	private Color foldedBackground;

	[SerializeField]
	private Color winnerBackground;

	[SerializeField]
	private Animation actionShowAnimation;

	[SerializeField]
	private RustText actionText;

	[SerializeField]
	private Sprite dealerChip;

	[SerializeField]
	private Sprite smallBlindChip;

	[SerializeField]
	private Sprite bigBlindChip;

	[SerializeField]
	private Sprite canSeeIcon;

	[SerializeField]
	private Sprite cannotSeeIcon;

	[SerializeField]
	private Sprite noChip;

	[SerializeField]
	private Image chip;

	[SerializeField]
	private Image[] cardsDisplay;

	[SerializeField]
	private Phrase allInPhrase;

	[SerializeField]
	private Phrase foldPhrase;

	[SerializeField]
	private Phrase raisePhrase;

	[SerializeField]
	private Phrase betPhrase;

	[SerializeField]
	private Phrase checkPhrase;

	[SerializeField]
	private Phrase callPhrase;

	public TexasHoldEmPlayerWidget()
		: this()
	{
	}
}
