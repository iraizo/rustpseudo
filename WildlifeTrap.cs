using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

public class WildlifeTrap : StorageContainer
{
	public static class WildlifeTrapFlags
	{
		public const Flags Occupied = Flags.Reserved1;
	}

	[Serializable]
	public class WildlifeWeight
	{
		public TrappableWildlife wildlife;

		public int weight;
	}

	public float tickRate = 60f;

	public GameObjectRef trappedEffect;

	public float trappedEffectRepeatRate = 30f;

	public float trapSuccessRate = 0.5f;

	public List<ItemDefinition> ignoreBait;

	public List<WildlifeWeight> targetWildlife;

	public bool HasCatch()
	{
		return HasFlag(Flags.Reserved1);
	}

	public bool IsTrapActive()
	{
		return HasFlag(Flags.On);
	}

	public override void ResetState()
	{
		base.ResetState();
	}

	public void SetTrapActive(bool trapOn)
	{
		if (trapOn != IsTrapActive())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TrapThink);
			SetFlag(Flags.On, trapOn);
			if (trapOn)
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)TrapThink, tickRate * 0.8f + tickRate * Random.Range(0f, 0.4f), tickRate);
			}
		}
	}

	public int GetBaitCalories()
	{
		int num = 0;
		foreach (Item item in base.inventory.itemList)
		{
			ItemModConsumable component = ((Component)item.info).GetComponent<ItemModConsumable>();
			if ((Object)(object)component == (Object)null || ignoreBait.Contains(item.info))
			{
				continue;
			}
			foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
			{
				if (effect.type == MetabolismAttribute.Type.Calories && effect.amount > 0f)
				{
					num += Mathf.CeilToInt(effect.amount * (float)item.amount);
				}
			}
		}
		return num;
	}

	public void DestroyRandomFoodItem()
	{
		int count = base.inventory.itemList.Count;
		int num = Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num2 = num + i;
			if (num2 >= count)
			{
				num2 -= count;
			}
			Item item = base.inventory.itemList[num2];
			if (item != null && !((Object)(object)((Component)item.info).GetComponent<ItemModConsumable>() == (Object)null))
			{
				item.UseItem();
				break;
			}
		}
	}

	public void UseBaitCalories(int numToUse)
	{
		foreach (Item item in base.inventory.itemList)
		{
			int itemCalories = GetItemCalories(item);
			if (itemCalories > 0)
			{
				numToUse -= itemCalories;
				item.UseItem();
				if (numToUse <= 0)
				{
					break;
				}
			}
		}
	}

	public int GetItemCalories(Item item)
	{
		ItemModConsumable component = ((Component)item.info).GetComponent<ItemModConsumable>();
		if ((Object)(object)component == (Object)null)
		{
			return 0;
		}
		foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
		{
			if (effect.type == MetabolismAttribute.Type.Calories && effect.amount > 0f)
			{
				return Mathf.CeilToInt(effect.amount);
			}
		}
		return 0;
	}

	public virtual void TrapThink()
	{
		int baitCalories = GetBaitCalories();
		if (baitCalories <= 0)
		{
			return;
		}
		TrappableWildlife randomWildlife = GetRandomWildlife();
		if (baitCalories >= randomWildlife.caloriesForInterest && Random.Range(0f, 1f) <= randomWildlife.successRate)
		{
			UseBaitCalories(randomWildlife.caloriesForInterest);
			if (Random.Range(0f, 1f) <= trapSuccessRate)
			{
				TrapWildlife(randomWildlife);
			}
		}
	}

	public void TrapWildlife(TrappableWildlife trapped)
	{
		Item item = ItemManager.Create(trapped.inventoryObject, Random.Range(trapped.minToCatch, trapped.maxToCatch + 1), 0uL);
		if (!item.MoveToContainer(base.inventory))
		{
			item.Remove();
		}
		else
		{
			SetFlag(Flags.Reserved1, b: true);
		}
		SetTrapActive(trapOn: false);
		Hurt(StartMaxHealth() * 0.1f, DamageType.Decay, null, useProtection: false);
	}

	public void ClearTrap()
	{
		SetFlag(Flags.Reserved1, b: false);
	}

	public bool HasBait()
	{
		return GetBaitCalories() > 0;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		SetTrapActive(HasBait());
		ClearTrap();
		base.PlayerStoppedLooting(player);
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		ClearTrap();
		return base.OnStartBeingLooted(baseEntity);
	}

	public TrappableWildlife GetRandomWildlife()
	{
		int num = targetWildlife.Sum((WildlifeWeight x) => x.weight);
		int num2 = Random.Range(0, num);
		for (int i = 0; i < targetWildlife.Count; i++)
		{
			num -= targetWildlife[i].weight;
			if (num2 >= num)
			{
				return targetWildlife[i].wildlife;
			}
		}
		return null;
	}
}
