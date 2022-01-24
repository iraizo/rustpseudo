using System;
using UnityEngine;

public class PrefabInformation : PrefabAttribute
{
	public ItemDefinition associatedItemDefinition;

	public Phrase title;

	public Phrase description;

	public Sprite sprite;

	protected override Type GetIndexedType()
	{
		return typeof(PrefabInformation);
	}
}
