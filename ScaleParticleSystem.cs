using System;
using UnityEngine;

public class ScaleParticleSystem : ScaleRenderer
{
	public ParticleSystem pSystem;

	public bool scaleGravity;

	[NonSerialized]
	private float startSize;

	[NonSerialized]
	private float startLifeTime;

	[NonSerialized]
	private float startSpeed;

	[NonSerialized]
	private float startGravity;

	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		startGravity = pSystem.get_gravityModifier();
		startSpeed = pSystem.get_startSpeed();
		startSize = pSystem.get_startSize();
		startLifeTime = pSystem.get_startLifetime();
	}

	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		pSystem.set_startSize(startSize * scale);
		pSystem.set_startLifetime(startLifeTime * scale);
		pSystem.set_startSpeed(startSpeed * scale);
		pSystem.set_gravityModifier(startGravity * scale);
	}
}
