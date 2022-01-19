using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleGroupCookie : MonoBehaviour
{
	public ToggleGroup group => ((Component)this).GetComponent<ToggleGroup>();

	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("ToggleGroupCookie_" + ((Object)this).get_name());
		if (!string.IsNullOrEmpty(@string))
		{
			Transform val = ((Component)this).get_transform().Find(@string);
			if (Object.op_Implicit((Object)(object)val))
			{
				Toggle component = ((Component)val).GetComponent<Toggle>();
				if (Object.op_Implicit((Object)(object)component))
				{
					Toggle[] componentsInChildren = ((Component)this).GetComponentsInChildren<Toggle>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].set_isOn(false);
					}
					component.set_isOn(false);
					component.set_isOn(true);
					SetupListeners();
					return;
				}
			}
		}
		Toggle val2 = group.ActiveToggles().FirstOrDefault((Toggle x) => x.get_isOn());
		if (Object.op_Implicit((Object)(object)val2))
		{
			val2.set_isOn(false);
			val2.set_isOn(true);
		}
		SetupListeners();
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			Toggle[] componentsInChildren = ((Component)this).GetComponentsInChildren<Toggle>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((UnityEvent<bool>)(object)componentsInChildren[i].onValueChanged).RemoveListener((UnityAction<bool>)OnToggleChanged);
			}
		}
	}

	private void SetupListeners()
	{
		Toggle[] componentsInChildren = ((Component)this).GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			((UnityEvent<bool>)(object)componentsInChildren[i].onValueChanged).AddListener((UnityAction<bool>)OnToggleChanged);
		}
	}

	private void OnToggleChanged(bool b)
	{
		Toggle val = ((Component)this).GetComponentsInChildren<Toggle>().FirstOrDefault((Toggle x) => x.get_isOn());
		if (Object.op_Implicit((Object)(object)val))
		{
			PlayerPrefs.SetString("ToggleGroupCookie_" + ((Object)this).get_name(), ((Object)((Component)val).get_gameObject()).get_name());
		}
	}

	public ToggleGroupCookie()
		: this()
	{
	}
}
