using System.Collections.Generic;
using Rust;
using UnityEngine;

public abstract class BaseScreenShake : MonoBehaviour
{
	public static List<BaseScreenShake> list = new List<BaseScreenShake>();

	public float length = 2f;

	internal float timeTaken;

	private int currentFrame = -1;

	public static void Apply(Camera cam, BaseViewModel vm)
	{
		CachedTransform<Camera> cam2 = new CachedTransform<Camera>(cam);
		CachedTransform<BaseViewModel> vm2 = new CachedTransform<BaseViewModel>(vm);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Run(ref cam2, ref vm2);
		}
		cam2.Apply();
		vm2.Apply();
	}

	protected void OnEnable()
	{
		list.Add(this);
		timeTaken = 0f;
		Setup();
	}

	protected void OnDisable()
	{
		if (!Application.isQuitting)
		{
			list.Remove(this);
		}
	}

	public void Run(ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (!(timeTaken > length))
		{
			if (Time.get_frameCount() != currentFrame)
			{
				timeTaken += Time.get_deltaTime();
				currentFrame = Time.get_frameCount();
			}
			float delta = Mathf.InverseLerp(0f, length, timeTaken);
			Run(delta, ref cam, ref vm);
		}
	}

	public abstract void Setup();

	public abstract void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm);

	protected BaseScreenShake()
		: this()
	{
	}
}
