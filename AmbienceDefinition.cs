using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Ambience Definition")]
public class AmbienceDefinition : ScriptableObject
{
	[Serializable]
	public class ValueRange
	{
		public float min;

		public float max;

		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}

	[Header("Sound")]
	public List<SoundDefinition> sounds;

	[Horizontal(2, -1)]
	public ValueRange stingFrequency = new ValueRange(15f, 30f);

	[Header("Environment")]
	[InspectorFlags]
	public Enum biomes = (Enum)(-1);

	[InspectorFlags]
	public Enum topologies = (Enum)(-1);

	public EnvironmentType environmentType = EnvironmentType.Underground;

	public bool useEnvironmentType;

	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	[Horizontal(2, -1)]
	public ValueRange rain = new ValueRange(0f, 1f);

	[Horizontal(2, -1)]
	public ValueRange wind = new ValueRange(0f, 1f);

	[Horizontal(2, -1)]
	public ValueRange snow = new ValueRange(0f, 1f);

	public AmbienceDefinition()
		: this()
	{
	}//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001e: Unknown result type (might be due to invalid IL or missing references)

}
