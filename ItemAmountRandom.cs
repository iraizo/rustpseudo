using System;
using UnityEngine;

[Serializable]
public class ItemAmountRandom
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	public AnimationCurve amount = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public int RandomAmount()
	{
		return Mathf.RoundToInt(amount.Evaluate(Random.Range(0f, 1f)));
	}
}
