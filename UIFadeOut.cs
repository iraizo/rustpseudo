using UnityEngine;

public class UIFadeOut : MonoBehaviour
{
	public float secondsToFadeOut = 3f;

	public bool destroyOnFaded = true;

	public CanvasGroup targetGroup;

	private float timeStarted;

	private void Start()
	{
		timeStarted = Time.get_realtimeSinceStartup();
	}

	private void Update()
	{
		targetGroup.set_alpha(Mathf.InverseLerp(timeStarted + secondsToFadeOut, timeStarted, Time.get_realtimeSinceStartup()));
		if (destroyOnFaded && Time.get_realtimeSinceStartup() > timeStarted + secondsToFadeOut)
		{
			GameManager.Destroy(((Component)this).get_gameObject());
		}
	}

	public UIFadeOut()
		: this()
	{
	}
}
