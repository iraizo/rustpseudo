using UnityEngine;
using UnityEngine.UI;

public class ToolgunScreen : MonoBehaviour
{
	public Text blockInfoText;

	public Text noBlockText;

	public void SetScreenText(string newText)
	{
		bool flag = string.IsNullOrEmpty(newText);
		((Component)blockInfoText).get_gameObject().SetActive(!flag);
		((Component)noBlockText).get_gameObject().SetActive(flag);
		blockInfoText.set_text(newText);
	}

	public ToolgunScreen()
		: this()
	{
	}
}
