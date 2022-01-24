using System.Collections.Generic;
using UnityEngine;

public class ConstructionSkin : BasePrefab
{
	private List<GameObject> conditionals;

	public int DetermineConditionalModelState(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(prefabID);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].RunTests(parent))
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	private void CreateConditionalModels(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(prefabID);
		for (int i = 0; i < array.Length; i++)
		{
			if (!parent.GetConditionalModel(i))
			{
				continue;
			}
			GameObject val = array[i].InstantiateSkin(parent);
			if (!((Object)(object)val == (Object)null))
			{
				if (conditionals == null)
				{
					conditionals = new List<GameObject>();
				}
				conditionals.Add(val);
			}
		}
	}

	private void DestroyConditionalModels(BuildingBlock parent)
	{
		if (conditionals != null)
		{
			for (int i = 0; i < conditionals.Count; i++)
			{
				parent.gameManager.Retire(conditionals[i]);
			}
			conditionals.Clear();
		}
	}

	public void Refresh(BuildingBlock parent)
	{
		DestroyConditionalModels(parent);
		CreateConditionalModels(parent);
	}

	public void Destroy(BuildingBlock parent)
	{
		DestroyConditionalModels(parent);
		parent.gameManager.Retire(((Component)this).get_gameObject());
	}
}
