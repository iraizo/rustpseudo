using System;
using UnityEngine;

public class LifeScale : BaseMonoBehaviour
{
	[NonSerialized]
	private bool initialized;

	[NonSerialized]
	private Vector3 initialScale;

	public Vector3 finalScale = Vector3.get_one();

	private Vector3 targetLerpScale = Vector3.get_zero();

	private Action updateScaleAction;

	protected void Awake()
	{
		updateScaleAction = UpdateScale;
	}

	public void OnEnable()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Init();
		((Component)this).get_transform().set_localScale(initialScale);
	}

	public void SetProgress(float progress)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Init();
		targetLerpScale = Vector3.Lerp(initialScale, finalScale, progress);
		((FacepunchBehaviour)this).InvokeRepeating(updateScaleAction, 0f, 0.015f);
	}

	public void Init()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			initialScale = ((Component)this).get_transform().get_localScale();
			initialized = true;
		}
	}

	public void UpdateScale()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().set_localScale(Vector3.Lerp(((Component)this).get_transform().get_localScale(), targetLerpScale, Time.get_deltaTime()));
		if (((Component)this).get_transform().get_localScale() == targetLerpScale)
		{
			targetLerpScale = Vector3.get_zero();
			((FacepunchBehaviour)this).CancelInvoke(updateScaleAction);
		}
	}
}
