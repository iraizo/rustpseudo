using System;
using UnityEngine;
using UnityEngine.UI;

public class UIIntegerEntry : MonoBehaviour
{
	public InputField textEntry;

	public event Action textChanged;

	public void OnAmountTextChanged()
	{
		this.textChanged();
	}

	public void SetAmount(int amount)
	{
		if (amount != GetIntAmount())
		{
			textEntry.set_text(amount.ToString());
		}
	}

	public int GetIntAmount()
	{
		int result = 0;
		int.TryParse(textEntry.get_text(), out result);
		return result;
	}

	public void PlusMinus(int delta)
	{
		SetAmount(GetIntAmount() + delta);
	}

	public UIIntegerEntry()
		: this()
	{
	}
}
