using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class Composter : StorageContainer
{
	[Header("Composter")]
	public ItemDefinition FertilizerDef;

	[Tooltip("If enabled, entire item stacks will be composted each tick, instead of a single item of a stack.")]
	public bool CompostEntireStack;

	private float fertilizerProductionProgress;

	protected float UpdateInterval => Server.composterUpdateInterval;

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(InventoryItemFilter));
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateComposting, UpdateInterval, UpdateInterval, UpdateInterval * 0.1f);
	}

	public bool InventoryItemFilter(Item item, int targetSlot)
	{
		if (item == null)
		{
			return false;
		}
		if ((Object)(object)((Component)item.info).GetComponent<ItemModCompostable>() != (Object)null || ItemIsFertilizer(item))
		{
			return true;
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.composter = Pool.Get<Composter>();
		info.msg.composter.fertilizerProductionProgress = fertilizerProductionProgress;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.composter != null)
		{
			fertilizerProductionProgress = info.msg.composter.fertilizerProductionProgress;
		}
	}

	private bool ItemIsFertilizer(Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	public void UpdateComposting()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				CompostItem(slot);
			}
		}
	}

	private void CompostItem(Item item)
	{
		if (!ItemIsFertilizer(item))
		{
			ItemModCompostable component = ((Component)item.info).GetComponent<ItemModCompostable>();
			if (!((Object)(object)component == (Object)null))
			{
				int num = ((!CompostEntireStack) ? 1 : item.amount);
				item.UseItem(num);
				fertilizerProductionProgress += (float)num * component.TotalFertilizerProduced;
				ProduceFertilizer(Mathf.FloorToInt(fertilizerProductionProgress));
			}
		}
	}

	private void ProduceFertilizer(int amount)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (amount > 0)
		{
			Item item = ItemManager.Create(FertilizerDef, amount, 0uL);
			if (!item.MoveToContainer(base.inventory))
			{
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity);
			}
			fertilizerProductionProgress -= amount;
		}
	}
}
