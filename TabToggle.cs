using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabToggle : MonoBehaviour
{
	public Transform TabHolder;

	public Transform ContentHolder;

	public bool FadeIn;

	public bool FadeOut;

	public void Awake()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		if (!Object.op_Implicit((Object)(object)TabHolder))
		{
			return;
		}
		for (int i = 0; i < TabHolder.get_childCount(); i++)
		{
			Button c = ((Component)TabHolder.GetChild(i)).GetComponent<Button>();
			if (Object.op_Implicit((Object)(object)c))
			{
				((UnityEvent)c.get_onClick()).AddListener((UnityAction)delegate
				{
					SwitchTo(c);
				});
			}
		}
	}

	public void SwitchTo(Button sourceTab)
	{
		string name = ((Object)((Component)sourceTab).get_transform()).get_name();
		if (Object.op_Implicit((Object)(object)TabHolder))
		{
			for (int i = 0; i < TabHolder.get_childCount(); i++)
			{
				Button component = ((Component)TabHolder.GetChild(i)).GetComponent<Button>();
				if (Object.op_Implicit((Object)(object)component))
				{
					((Selectable)component).set_interactable(((Object)component).get_name() != name);
				}
			}
		}
		if (!Object.op_Implicit((Object)(object)ContentHolder))
		{
			return;
		}
		for (int j = 0; j < ContentHolder.get_childCount(); j++)
		{
			Transform child = ContentHolder.GetChild(j);
			if (((Object)child).get_name() == name)
			{
				Show(((Component)child).get_gameObject());
			}
			else
			{
				Hide(((Component)child).get_gameObject());
			}
		}
	}

	private void Hide(GameObject go)
	{
		if (!go.get_activeSelf())
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (FadeOut && Object.op_Implicit((Object)(object)component))
		{
			LeanTween.alphaCanvas(component, 0f, 0.1f).setOnComplete((Action)delegate
			{
				go.SetActive(false);
			});
		}
		else
		{
			go.SetActive(false);
		}
	}

	private void Show(GameObject go)
	{
		if (!go.get_activeSelf())
		{
			CanvasGroup component = go.GetComponent<CanvasGroup>();
			if (FadeIn && Object.op_Implicit((Object)(object)component))
			{
				component.set_alpha(0f);
				LeanTween.alphaCanvas(component, 1f, 0.1f);
			}
			go.SetActive(true);
		}
	}

	public TabToggle()
		: this()
	{
	}
}
