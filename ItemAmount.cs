using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

[Serializable]
public class ItemAmount : ISerializationCallbackReceiver
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	public float amount;

	[NonSerialized]
	public float startAmount;

	public int itemid
	{
		get
		{
			if ((Object)(object)itemDef == (Object)null)
			{
				return 0;
			}
			return itemDef.itemid;
		}
	}

	public ItemAmount(ItemDefinition item = null, float amt = 0f)
	{
		itemDef = item;
		amount = amt;
		startAmount = amount;
	}

	public virtual float GetAmount()
	{
		return amount;
	}

	public virtual void OnAfterDeserialize()
	{
		startAmount = amount;
	}

	public virtual void OnBeforeSerialize()
	{
	}

	public static ItemAmountList SerialiseList(List<ItemAmount> list)
	{
		ItemAmountList val = Pool.Get<ItemAmountList>();
		val.amount = Pool.GetList<float>();
		val.itemID = Pool.GetList<int>();
		foreach (ItemAmount item in list)
		{
			val.amount.Add(item.amount);
			val.itemID.Add(item.itemid);
		}
		return val;
	}

	public static void DeserialiseList(List<ItemAmount> target, ItemAmountList source)
	{
		target.Clear();
		if (source.amount.Count == source.itemID.Count)
		{
			for (int i = 0; i < source.amount.Count; i++)
			{
				target.Add(new ItemAmount(ItemManager.FindItemDefinition(source.itemID[i]), source.amount[i]));
			}
		}
	}
}
