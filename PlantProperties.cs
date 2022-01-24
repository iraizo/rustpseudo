using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Plant Properties")]
public class PlantProperties : ScriptableObject
{
	public enum State
	{
		Seed,
		Seedling,
		Sapling,
		Crossbreed,
		Mature,
		Fruiting,
		Ripe,
		Dying
	}

	[Serializable]
	public struct Stage
	{
		public State nextState;

		public float lifeLength;

		public float health;

		public float resources;

		public float yield;

		public GameObjectRef skinObject;

		public bool IgnoreConditions;

		public float lifeLengthSeconds => lifeLength * 60f;
	}

	public Phrase Description;

	public GrowableGeneProperties Genes;

	[ArrayIndexIsEnum(enumType = typeof(State))]
	public Stage[] stages = new Stage[8];

	[Header("Metabolism")]
	public AnimationCurve timeOfDayHappiness = new AnimationCurve((Keyframe[])(object)new Keyframe[3]
	{
		new Keyframe(0f, 0f),
		new Keyframe(12f, 1f),
		new Keyframe(24f, 0f)
	});

	public AnimationCurve temperatureHappiness = new AnimationCurve((Keyframe[])(object)new Keyframe[5]
	{
		new Keyframe(-10f, -1f),
		new Keyframe(1f, 0f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 0f),
		new Keyframe(80f, -1f)
	});

	public AnimationCurve temperatureWaterRequirementMultiplier = new AnimationCurve((Keyframe[])(object)new Keyframe[5]
	{
		new Keyframe(-10f, 1f),
		new Keyframe(0f, 1f),
		new Keyframe(30f, 1f),
		new Keyframe(50f, 1f),
		new Keyframe(80f, 1f)
	});

	public AnimationCurve fruitVisualScaleCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[3]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.75f, 1f),
		new Keyframe(1f, 0f)
	});

	public int MaxSeasons = 1;

	public float WaterIntake = 20f;

	public float OptimalLightQuality = 1f;

	public float OptimalWaterQuality = 1f;

	public float OptimalGroundQuality = 1f;

	public float OptimalTemperatureQuality = 1f;

	[Header("Harvesting")]
	public BaseEntity.Menu.Option pickOption;

	public ItemDefinition pickupItem;

	public BaseEntity.Menu.Option cloneOption;

	public BaseEntity.Menu.Option removeDyingOption;

	public ItemDefinition removeDyingItem;

	public GameObjectRef removeDyingEffect;

	public int pickupMultiplier = 1;

	public GameObjectRef pickEffect;

	public int maxHarvests = 1;

	public bool disappearAfterHarvest;

	[Header("Seeds")]
	public GameObjectRef CrossBreedEffect;

	public ItemDefinition SeedItem;

	public ItemDefinition CloneItem;

	public int BaseCloneCount = 1;

	[Header("Market")]
	public int BaseMarketValue = 10;

	public PlantProperties()
		: this()
	{
	}//IL_001f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0024: Unknown result type (might be due to invalid IL or missing references)
	//IL_0035: Unknown result type (might be due to invalid IL or missing references)
	//IL_003a: Unknown result type (might be due to invalid IL or missing references)
	//IL_004b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0050: Unknown result type (might be due to invalid IL or missing references)
	//IL_0055: Unknown result type (might be due to invalid IL or missing references)
	//IL_005f: Expected O, but got Unknown
	//IL_0072: Unknown result type (might be due to invalid IL or missing references)
	//IL_0077: Unknown result type (might be due to invalid IL or missing references)
	//IL_0088: Unknown result type (might be due to invalid IL or missing references)
	//IL_008d: Unknown result type (might be due to invalid IL or missing references)
	//IL_009e: Unknown result type (might be due to invalid IL or missing references)
	//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
	//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
	//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
	//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
	//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
	//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
	//IL_00de: Expected O, but got Unknown
	//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
	//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
	//IL_0107: Unknown result type (might be due to invalid IL or missing references)
	//IL_010c: Unknown result type (might be due to invalid IL or missing references)
	//IL_011d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0122: Unknown result type (might be due to invalid IL or missing references)
	//IL_0133: Unknown result type (might be due to invalid IL or missing references)
	//IL_0138: Unknown result type (might be due to invalid IL or missing references)
	//IL_0149: Unknown result type (might be due to invalid IL or missing references)
	//IL_014e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0153: Unknown result type (might be due to invalid IL or missing references)
	//IL_015d: Expected O, but got Unknown
	//IL_0170: Unknown result type (might be due to invalid IL or missing references)
	//IL_0175: Unknown result type (might be due to invalid IL or missing references)
	//IL_0186: Unknown result type (might be due to invalid IL or missing references)
	//IL_018b: Unknown result type (might be due to invalid IL or missing references)
	//IL_019c: Unknown result type (might be due to invalid IL or missing references)
	//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
	//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
	//IL_01b0: Expected O, but got Unknown

}
