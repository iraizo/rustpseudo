using UnityEngine;
using UnityEngine.UI;

public class UIFishing : SingletonComponent<UIFishing>
{
	public Slider TensionLine;

	public Image FillImage;

	public Gradient FillGradient;

	private void Start()
	{
		((Component)this).get_gameObject().SetActive(false);
	}
}
