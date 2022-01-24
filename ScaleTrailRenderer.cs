using System;
using UnityEngine;

public class ScaleTrailRenderer : ScaleRenderer
{
	private TrailRenderer trailRenderer;

	[NonSerialized]
	private float startWidth;

	[NonSerialized]
	private float endWidth;

	[NonSerialized]
	private float duration;

	[NonSerialized]
	private float startMultiplier;

	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		if (Object.op_Implicit((Object)(object)myRenderer))
		{
			trailRenderer = ((Component)myRenderer).GetComponent<TrailRenderer>();
		}
		else
		{
			trailRenderer = ((Component)this).GetComponentInChildren<TrailRenderer>();
		}
		startWidth = trailRenderer.get_startWidth();
		endWidth = trailRenderer.get_endWidth();
		duration = trailRenderer.get_time();
		startMultiplier = trailRenderer.get_widthMultiplier();
	}

	public override void SetScale_Internal(float scale)
	{
		if (scale == 0f)
		{
			trailRenderer.set_emitting(false);
			((Renderer)trailRenderer).set_enabled(false);
			trailRenderer.set_time(0f);
			trailRenderer.Clear();
			return;
		}
		if (!trailRenderer.get_emitting())
		{
			trailRenderer.Clear();
		}
		trailRenderer.set_emitting(true);
		((Renderer)trailRenderer).set_enabled(true);
		base.SetScale_Internal(scale);
		trailRenderer.set_widthMultiplier(startMultiplier * scale);
		trailRenderer.set_time(duration * scale);
	}
}
