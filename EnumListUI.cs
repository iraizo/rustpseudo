using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnumListUI : MonoBehaviour
{
	public Transform PrefabItem;

	public Transform Container;

	private Action<object> clickedAction;

	private CanvasScaler canvasScaler;

	private void Awake()
	{
		Hide();
	}

	public void Show(List<object> values, Action<object> clicked)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_gameObject().SetActive(true);
		clickedAction = clicked;
		foreach (Transform item in Container)
		{
			Object.Destroy((Object)(object)((Component)item).get_gameObject());
		}
		foreach (object value in values)
		{
			Transform obj = Object.Instantiate<Transform>(PrefabItem);
			obj.SetParent(Container, false);
			((Component)obj).GetComponent<EnumListItemUI>().Init(value, value.ToString(), this);
		}
	}

	public void ItemClicked(object value)
	{
		clickedAction?.Invoke(value);
		Hide();
	}

	public void Hide()
	{
		((Component)this).get_gameObject().SetActive(false);
	}

	public EnumListUI()
		: this()
	{
	}
}
