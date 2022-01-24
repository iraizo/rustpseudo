using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

public class ItemCrafter : EntityComponent<BasePlayer>
{
	public List<ItemContainer> containers = new List<ItemContainer>();

	public LinkedList<ItemCraftTask> queue = new LinkedList<ItemCraftTask>();

	public int taskUID;

	public void AddContainer(ItemContainer container)
	{
		containers.Add(container);
	}

	public static float GetScaledDuration(ItemBlueprint bp, float workbenchLevel)
	{
		float num = workbenchLevel - (float)bp.workbenchLevelRequired;
		if (num == 1f)
		{
			return bp.time * 0.5f;
		}
		if (num >= 2f)
		{
			return bp.time * 0.25f;
		}
		return bp.time;
	}

	public void ServerUpdate(float delta)
	{
		if (queue.get_Count() == 0)
		{
			return;
		}
		ItemCraftTask value = queue.get_First().get_Value();
		if (value.cancelled)
		{
			value.owner.Command("note.craft_done", value.taskUID, 0);
			queue.RemoveFirst();
			return;
		}
		float currentCraftLevel = value.owner.currentCraftLevel;
		if (value.endTime > Time.get_realtimeSinceStartup())
		{
			return;
		}
		if (value.endTime == 0f)
		{
			float scaledDuration = GetScaledDuration(value.blueprint, currentCraftLevel);
			value.endTime = Time.get_realtimeSinceStartup() + scaledDuration;
			if ((Object)(object)value.owner != (Object)null)
			{
				value.owner.Command("note.craft_start", value.taskUID, scaledDuration, value.amount);
				if (value.owner.IsAdmin && Craft.instant)
				{
					value.endTime = Time.get_realtimeSinceStartup() + 1f;
				}
			}
		}
		else
		{
			FinishCrafting(value);
			if (value.amount <= 0)
			{
				queue.RemoveFirst();
			}
			else
			{
				value.endTime = 0f;
			}
		}
	}

	private void CollectIngredient(int item, int amount, List<Item> collect)
	{
		foreach (ItemContainer container in containers)
		{
			amount -= container.Take(collect, item, amount);
			if (amount <= 0)
			{
				break;
			}
		}
	}

	private void CollectIngredients(ItemBlueprint bp, ItemCraftTask task, int amount = 1, BasePlayer player = null)
	{
		List<Item> list = new List<Item>();
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			CollectIngredient(ingredient.itemid, (int)ingredient.amount * amount, list);
		}
		task.potentialOwners = new List<ulong>();
		foreach (Item item in list)
		{
			item.CollectedForCrafting(player);
			if (!task.potentialOwners.Contains(player.userID))
			{
				task.potentialOwners.Add(player.userID);
			}
		}
		task.takenItems = list;
	}

	public bool CraftItem(ItemBlueprint bp, BasePlayer owner, InstanceData instanceData = null, int amount = 1, int skinID = 0, Item fromTempBlueprint = null, bool free = false)
	{
		if (!CanCraft(bp, amount, free))
		{
			return false;
		}
		taskUID++;
		ItemCraftTask itemCraftTask = Pool.Get<ItemCraftTask>();
		itemCraftTask.blueprint = bp;
		if (!free)
		{
			CollectIngredients(bp, itemCraftTask, amount, owner);
		}
		itemCraftTask.endTime = 0f;
		itemCraftTask.taskUID = taskUID;
		itemCraftTask.owner = owner;
		itemCraftTask.instanceData = instanceData;
		if (itemCraftTask.instanceData != null)
		{
			itemCraftTask.instanceData.ShouldPool = false;
		}
		itemCraftTask.amount = amount;
		itemCraftTask.skinID = skinID;
		if (fromTempBlueprint != null && itemCraftTask.takenItems != null)
		{
			fromTempBlueprint.RemoveFromContainer();
			itemCraftTask.takenItems.Add(fromTempBlueprint);
			itemCraftTask.conditionScale = 0.5f;
		}
		queue.AddLast(itemCraftTask);
		if ((Object)(object)itemCraftTask.owner != (Object)null)
		{
			itemCraftTask.owner.Command("note.craft_add", itemCraftTask.taskUID, itemCraftTask.blueprint.targetItem.itemid, amount, itemCraftTask.skinID);
		}
		return true;
	}

	private void FinishCrafting(ItemCraftTask task)
	{
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		task.amount--;
		task.numCrafted++;
		ulong skin = ItemDefinition.FindSkin(task.blueprint.targetItem.itemid, task.skinID);
		Item item = ItemManager.CreateByItemID(task.blueprint.targetItem.itemid, 1, skin);
		item.amount = task.blueprint.amountToCreate;
		if (item.hasCondition && task.conditionScale != 1f)
		{
			item.maxCondition *= task.conditionScale;
			item.condition = item.maxCondition;
		}
		item.OnVirginSpawn();
		foreach (ItemAmount ingredient in task.blueprint.ingredients)
		{
			int num = (int)ingredient.amount;
			if (task.takenItems == null)
			{
				continue;
			}
			foreach (Item takenItem in task.takenItems)
			{
				if ((Object)(object)takenItem.info == (Object)(object)ingredient.itemDef)
				{
					int num2 = Mathf.Min(takenItem.amount, num);
					takenItem.UseItem(num);
					num -= num2;
				}
				_ = 0;
			}
		}
		Analytics.Crafting(task.blueprint.targetItem.shortname, task.skinID);
		task.owner.Command("note.craft_done", task.taskUID, 1, task.amount);
		if (task.instanceData != null)
		{
			item.instanceData = task.instanceData;
		}
		if (!string.IsNullOrEmpty(task.blueprint.UnlockAchievment))
		{
			task.owner.GiveAchievement(task.blueprint.UnlockAchievment);
		}
		if (task.owner.inventory.GiveItem(item))
		{
			task.owner.Command("note.inv", item.info.itemid, item.amount);
			return;
		}
		ItemContainer itemContainer = Enumerable.First<ItemContainer>((IEnumerable<ItemContainer>)containers);
		task.owner.Command("note.inv", item.info.itemid, item.amount);
		task.owner.Command("note.inv", item.info.itemid, -item.amount);
		item.Drop(itemContainer.dropPosition, itemContainer.dropVelocity);
	}

	public bool CancelTask(int iID, bool ReturnItems)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		if (queue.get_Count() == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = Enumerable.FirstOrDefault<ItemCraftTask>((IEnumerable<ItemCraftTask>)queue, (Func<ItemCraftTask, bool>)((ItemCraftTask x) => x.taskUID == iID && !x.cancelled));
		if (itemCraftTask == null)
		{
			return false;
		}
		itemCraftTask.cancelled = true;
		if ((Object)(object)itemCraftTask.owner == (Object)null)
		{
			return true;
		}
		itemCraftTask.owner.Command("note.craft_done", itemCraftTask.taskUID, 0);
		if (itemCraftTask.takenItems != null && itemCraftTask.takenItems.Count > 0 && ReturnItems)
		{
			foreach (Item takenItem in itemCraftTask.takenItems)
			{
				if (takenItem != null && takenItem.amount > 0)
				{
					if (takenItem.IsBlueprint() && (Object)(object)takenItem.blueprintTargetDef == (Object)(object)itemCraftTask.blueprint.targetItem)
					{
						takenItem.UseItem(itemCraftTask.numCrafted);
					}
					if (takenItem.amount > 0 && !takenItem.MoveToContainer(itemCraftTask.owner.inventory.containerMain))
					{
						takenItem.Drop(itemCraftTask.owner.inventory.containerMain.dropPosition + Random.get_value() * Vector3.get_down() + Random.get_insideUnitSphere(), itemCraftTask.owner.inventory.containerMain.dropVelocity);
						itemCraftTask.owner.Command("note.inv", takenItem.info.itemid, -takenItem.amount);
					}
				}
			}
		}
		return true;
	}

	public bool CancelBlueprint(int itemid)
	{
		if (queue.get_Count() == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = Enumerable.FirstOrDefault<ItemCraftTask>((IEnumerable<ItemCraftTask>)queue, (Func<ItemCraftTask, bool>)((ItemCraftTask x) => x.blueprint.targetItem.itemid == itemid && !x.cancelled));
		if (itemCraftTask == null)
		{
			return false;
		}
		return CancelTask(itemCraftTask.taskUID, ReturnItems: true);
	}

	public void CancelAll(bool returnItems)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<ItemCraftTask> enumerator = queue.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemCraftTask current = enumerator.get_Current();
				CancelTask(current.taskUID, returnItems);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	private bool DoesHaveUsableItem(int item, int iAmount)
	{
		int num = 0;
		foreach (ItemContainer container in containers)
		{
			num += container.GetAmount(item, onlyUsableAmounts: true);
		}
		return num >= iAmount;
	}

	public bool CanCraft(ItemBlueprint bp, int amount = 1, bool free = false)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)amount / (float)bp.targetItem.craftingStackable;
		Enumerator<ItemCraftTask> enumerator = queue.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemCraftTask current = enumerator.get_Current();
				if (!current.cancelled)
				{
					num += (float)current.amount / (float)current.blueprint.targetItem.craftingStackable;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (num > 8f)
		{
			return false;
		}
		if (amount < 1 || amount > bp.targetItem.craftingStackable)
		{
			return false;
		}
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			if (!DoesHaveUsableItem(ingredient.itemid, (int)ingredient.amount * amount))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanCraft(ItemDefinition def, int amount = 1, bool free = false)
	{
		ItemBlueprint component = ((Component)def).GetComponent<ItemBlueprint>();
		if (CanCraft(component, amount, free))
		{
			return true;
		}
		return false;
	}

	public bool FastTrackTask(int taskID)
	{
		if (queue.get_Count() == 0)
		{
			return false;
		}
		ItemCraftTask value = queue.get_First().get_Value();
		if (value == null)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = Enumerable.FirstOrDefault<ItemCraftTask>((IEnumerable<ItemCraftTask>)queue, (Func<ItemCraftTask, bool>)((ItemCraftTask x) => x.taskUID == taskID && !x.cancelled));
		if (itemCraftTask == null)
		{
			return false;
		}
		if (itemCraftTask == value)
		{
			return false;
		}
		value.endTime = 0f;
		queue.Remove(itemCraftTask);
		queue.AddFirst(itemCraftTask);
		itemCraftTask.owner.Command("note.craft_fasttracked", taskID);
		return true;
	}
}
