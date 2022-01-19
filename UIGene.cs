using UnityEngine;
using UnityEngine.UI;

public class UIGene : MonoBehaviour
{
	public GameObject Child;

	public Color PositiveColour;

	public Color NegativeColour;

	public Color PositiveTextColour;

	public Color NegativeTextColour;

	public Image ImageBG;

	public Text TextGene;

	public void Init(GrowableGene gene)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		bool flag = gene.IsPositive();
		((Graphic)ImageBG).set_color(flag ? PositiveColour : NegativeColour);
		((Graphic)TextGene).set_color(flag ? PositiveTextColour : NegativeTextColour);
		TextGene.set_text(gene.GetDisplayCharacter());
		Show();
	}

	public void InitPrevious(GrowableGene gene)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)ImageBG).set_color(Color.get_black());
		((Graphic)TextGene).set_color(Color.get_grey());
		TextGene.set_text(GrowableGene.GetDisplayCharacter(gene.PreviousType));
		Show();
	}

	public void Hide()
	{
		Child.get_gameObject().SetActive(false);
	}

	public void Show()
	{
		Child.get_gameObject().SetActive(true);
	}

	public UIGene()
		: this()
	{
	}
}
