using Rust.UI;
using TMPro;
using UnityEngine;

public class CopyText : MonoBehaviour
{
	public RustText TargetText;

	public void TriggerCopy()
	{
		if ((Object)(object)TargetText != (Object)null)
		{
			GUIUtility.set_systemCopyBuffer(((TMP_Text)TargetText).get_text());
		}
	}

	public CopyText()
		: this()
	{
	}
}
