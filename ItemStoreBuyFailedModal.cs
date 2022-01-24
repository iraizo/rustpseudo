using System;
using UnityEngine;

public class ItemStoreBuyFailedModal : MonoBehaviour
{
	public void Show(ulong orderid)
	{
		((Component)this).get_gameObject().SetActive(true);
		((Component)this).GetComponent<CanvasGroup>().set_alpha(0f);
		LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	public void Hide()
	{
		LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete((Action)delegate
		{
			((Component)this).get_gameObject().SetActive(false);
		});
	}

	public ItemStoreBuyFailedModal()
		: this()
	{
	}
}
