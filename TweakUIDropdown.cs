using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIDropdown : TweakUIBase
{
	[Serializable]
	public class NameValue
	{
		public string value;

		public Color imageColor;

		public Phrase label;
	}

	public Button Left;

	public Button Right;

	public TextMeshProUGUI Current;

	public Image BackgroundImage;

	public NameValue[] nameValues;

	public bool assignImageColor;

	public int currentValue;

	protected override void Init()
	{
		base.Init();
		ResetToConvar();
	}

	protected void OnEnable()
	{
		ResetToConvar();
	}

	public void OnValueChanged()
	{
		if (ApplyImmediatelyOnChange)
		{
			SetConvarValue();
		}
	}

	public void ChangeValue(int change)
	{
		currentValue += change;
		if (currentValue < 0)
		{
			currentValue = 0;
		}
		if (currentValue > nameValues.Length - 1)
		{
			currentValue = nameValues.Length - 1;
		}
		((Selectable)Left).set_interactable(currentValue > 0);
		((Selectable)Right).set_interactable(currentValue < nameValues.Length - 1);
		if (ApplyImmediatelyOnChange)
		{
			SetConvarValue();
		}
		else
		{
			ShowValue(nameValues[currentValue].value);
		}
	}

	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		NameValue nameValue = nameValues[currentValue];
		if (conVar != null && !(conVar.get_String() == nameValue.value))
		{
			conVar.Set(nameValue.value);
		}
	}

	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (conVar != null)
		{
			string @string = conVar.get_String();
			ShowValue(@string);
		}
	}

	private void ShowValue(string value)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < nameValues.Length; i++)
		{
			if (!(nameValues[i].value != value))
			{
				((TMP_Text)Current).set_text(nameValues[i].label.get_translated());
				currentValue = i;
				if (assignImageColor)
				{
					((Graphic)BackgroundImage).set_color(nameValues[i].imageColor);
				}
			}
		}
	}
}
