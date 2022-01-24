using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TexasHoldEmUI : MonoBehaviour
{
	[SerializeField]
	private Image[] holeCardImages;

	[FormerlySerializedAs("flopCardImages")]
	[SerializeField]
	private Image[] communityCardImages;

	[SerializeField]
	private RustText potText;

	[SerializeField]
	private TexasHoldEmPlayerWidget[] playerWidgets;

	[SerializeField]
	private GameObject raiseRoot;

	[SerializeField]
	private Phrase phraseNotEnoughBuyIn;

	[SerializeField]
	private Phrase phraseTooMuchBuyIn;

	[SerializeField]
	private Phrase phraseYouWinTheRound;

	[SerializeField]
	private Phrase phraseRoundWinner;

	[SerializeField]
	private Phrase phraseRoundWinners;

	[SerializeField]
	private Phrase phraseScrapWon;

	[SerializeField]
	private Phrase phraseScrapReturned;

	[SerializeField]
	private Phrase phraseWinningHand;

	[SerializeField]
	private Phrase phraseRoyalFlush;

	[SerializeField]
	private Phrase phraseStraightFlush;

	[SerializeField]
	private Phrase phraseFourOfAKind;

	[SerializeField]
	private Phrase phraseFullHouse;

	[SerializeField]
	private Phrase phraseFlush;

	[SerializeField]
	private Phrase phraseStraight;

	[SerializeField]
	private Phrase phraseThreeOfAKind;

	[SerializeField]
	private Phrase phraseTwoPair;

	[SerializeField]
	private Phrase phrasePair;

	[SerializeField]
	private Phrase phraseHighCard;

	public TexasHoldEmUI()
		: this()
	{
	}
}
