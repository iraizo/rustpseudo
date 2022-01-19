using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextEntryCookie : MonoBehaviour
{
	public InputField control => ((Component)this).GetComponent<InputField>();

	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("TextEntryCookie_" + ((Object)this).get_name());
		if (!string.IsNullOrEmpty(@string))
		{
			control.set_text(@string);
		}
		((UnityEvent<string>)(object)control.get_onValueChanged()).Invoke(control.get_text());
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			PlayerPrefs.SetString("TextEntryCookie_" + ((Object)this).get_name(), control.get_text());
		}
	}

	public TextEntryCookie()
		: this()
	{
	}
}
