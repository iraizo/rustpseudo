using System;
using Rust.UI;
using TMPro;
using UnityEngine;

public class ItemStoreItemInfoModal : MonoBehaviour
{
	public HttpImage Icon;

	public TextMeshProUGUI Name;

	public TextMeshProUGUI Price;

	public TextMeshProUGUI Description;

	private IPlayerItemDefinition item;

	public void Show(IPlayerItemDefinition item)
	{
		this.item = item;
		Icon.Load(item.get_IconUrl());
		((TMP_Text)Name).set_text(item.get_Name());
		((TMP_Text)Description).set_text(StringExtensions.BBCodeToUnity(item.get_Description()));
		((TMP_Text)Price).set_text(item.get_LocalPriceFormatted());
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

	public ItemStoreItemInfoModal()
		: this()
	{
	}
}
