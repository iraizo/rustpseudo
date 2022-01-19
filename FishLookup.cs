using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class FishLookup : PrefabAttribute
{
	public ItemModFishable FallbackFish;

	private static ItemModFishable[] AvailableFish;

	public static ItemDefinition[] BaitItems;

	private static TimeSince lastShuffle;

	public static void LoadFish()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (AvailableFish != null)
		{
			if (TimeSince.op_Implicit(lastShuffle) > 5f)
			{
				AvailableFish.Shuffle((uint)Random.Range(0, 10000));
			}
			return;
		}
		List<ItemModFishable> list = Pool.GetList<ItemModFishable>();
		List<ItemDefinition> list2 = Pool.GetList<ItemDefinition>();
		ItemModFishable item = default(ItemModFishable);
		ItemModCompostable itemModCompostable = default(ItemModCompostable);
		foreach (ItemDefinition item2 in ItemManager.itemList)
		{
			if (((Component)item2).TryGetComponent<ItemModFishable>(ref item))
			{
				list.Add(item);
			}
			if (((Component)item2).TryGetComponent<ItemModCompostable>(ref itemModCompostable) && itemModCompostable.BaitValue > 0f)
			{
				list2.Add(item2);
			}
		}
		AvailableFish = list.ToArray();
		BaitItems = list2.ToArray();
		Pool.FreeList<ItemModFishable>(ref list);
		Pool.FreeList<ItemDefinition>(ref list2);
	}

	public ItemDefinition GetFish(Vector3 worldPos, WaterBody bodyType, ItemDefinition lure, out ItemModFishable fishable, ItemModFishable ignoreFish)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		LoadFish();
		ItemModCompostable itemModCompostable = default(ItemModCompostable);
		float num = (((Component)lure).TryGetComponent<ItemModCompostable>(ref itemModCompostable) ? itemModCompostable.BaitValue : 0f);
		WaterBody.FishingTag fishingTag = (((Object)(object)bodyType != (Object)null) ? bodyType.FishingType : WaterBody.FishingTag.Ocean);
		float num2 = WaterLevel.GetOverallWaterDepth(worldPos, waves: true, null, noEarlyExit: true);
		if (worldPos.y < -10f)
		{
			num2 = 10f;
		}
		int num3 = Random.Range(0, AvailableFish.Length);
		for (int i = 0; i < AvailableFish.Length; i++)
		{
			num3++;
			if (num3 >= AvailableFish.Length)
			{
				num3 = 0;
			}
			ItemModFishable itemModFishable = AvailableFish[num3];
			if (itemModFishable.CanBeFished && !(itemModFishable.MinimumBaitLevel > num) && (!(itemModFishable.MaximumBaitLevel > 0f) || !(num > itemModFishable.MaximumBaitLevel)) && !((Object)(object)itemModFishable == (Object)(object)ignoreFish) && (itemModFishable.RequiredTag == (WaterBody.FishingTag)(-1) || (itemModFishable.RequiredTag & fishingTag) != 0) && ((fishingTag & WaterBody.FishingTag.Ocean) != WaterBody.FishingTag.Ocean || ((!(itemModFishable.MinimumWaterDepth > 0f) || !(num2 < itemModFishable.MinimumWaterDepth)) && (!(itemModFishable.MaximumWaterDepth > 0f) || !(num2 > itemModFishable.MaximumWaterDepth)))) && !(Random.Range(0f, 1f) - num * 3f * 0.01f > itemModFishable.Chance))
			{
				fishable = itemModFishable;
				return ((Component)itemModFishable).GetComponent<ItemDefinition>();
			}
		}
		fishable = FallbackFish;
		return ((Component)FallbackFish).GetComponent<ItemDefinition>();
	}

	protected override Type GetIndexedType()
	{
		return typeof(FishLookup);
	}
}
