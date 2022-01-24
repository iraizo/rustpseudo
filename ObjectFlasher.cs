using System;
using UnityEngine;

public class ObjectFlasher : BaseMonoBehaviour
{
	public GameObject enabledObj;

	public GameObject disabledObj;

	public float toggleLength = 1f;

	public float timeOffset;

	public float randomOffset;

	public void Awake()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)Toggle, Random.Range(0f, randomOffset) + timeOffset, toggleLength);
		disabledObj.SetActive(false);
		enabledObj.SetActive(true);
	}

	public void Toggle()
	{
		enabledObj.SetActive(!enabledObj.get_activeSelf());
		disabledObj.SetActive(!disabledObj.get_activeSelf());
	}
}
