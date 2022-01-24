using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class StorageMonitor : AppIOEntity
{
	private readonly Action<Item, bool> _onContainerChangedHandler;

	private readonly Action _resetSwitchHandler;

	private double _lastPowerOnUpdate;

	public override AppEntityType Type => (AppEntityType)3;

	public override bool Value
	{
		get
		{
			return IsOn();
		}
		set
		{
		}
	}

	public StorageMonitor()
	{
		_onContainerChangedHandler = OnContainerChanged;
		_resetSwitchHandler = ResetSwitch;
	}

	internal override void FillEntityPayload(AppEntityPayload payload)
	{
		base.FillEntityPayload(payload);
		StorageContainer storageContainer = GetStorageContainer();
		if ((Object)(object)storageContainer == (Object)null || !HasFlag(Flags.Reserved8))
		{
			return;
		}
		payload.items = Pool.GetList<Item>();
		foreach (Item item in storageContainer.inventory.itemList)
		{
			Item val = Pool.Get<Item>();
			val.itemId = (item.IsBlueprint() ? item.blueprintTargetDef.itemid : item.info.itemid);
			val.quantity = item.amount;
			val.itemIsBlueprint = item.IsBlueprint();
			payload.items.Add(val);
		}
		payload.capacity = storageContainer.inventory.capacity;
		BuildingPrivlidge buildingPrivlidge;
		if ((buildingPrivlidge = storageContainer as BuildingPrivlidge) != null)
		{
			payload.hasProtection = true;
			float protectedMinutes = buildingPrivlidge.GetProtectedMinutes();
			if (protectedMinutes > 0f)
			{
				payload.protectionExpiry = (uint)DateTimeOffset.UtcNow.AddMinutes(protectedMinutes).ToUnixTimeSeconds();
			}
		}
	}

	public override void Init()
	{
		base.Init();
		StorageContainer storageContainer = GetStorageContainer();
		if ((Object)(object)storageContainer != (Object)null && storageContainer.inventory != null)
		{
			ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, _onContainerChangedHandler);
		}
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		StorageContainer storageContainer = GetStorageContainer();
		if ((Object)(object)storageContainer != (Object)null && storageContainer.inventory != null)
		{
			ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<Item, bool>)Delegate.Remove(inventory.onItemAddedRemoved, _onContainerChangedHandler);
		}
	}

	private StorageContainer GetStorageContainer()
	{
		return GetParentEntity() as StorageContainer;
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!IsOn())
		{
			return 0;
		}
		return GetCurrentEnergy();
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		bool flag = HasFlag(Flags.Reserved8);
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			bool num = inputAmount >= ConsumptionAmount();
			double realtimeSinceStartup = TimeEx.get_realtimeSinceStartup();
			if (num && !flag && _lastPowerOnUpdate < realtimeSinceStartup - 1.0)
			{
				_lastPowerOnUpdate = realtimeSinceStartup;
				BroadcastValueChange();
			}
		}
	}

	private void OnContainerChanged(Item item, bool added)
	{
		if (HasFlag(Flags.Reserved8))
		{
			((FacepunchBehaviour)this).Invoke(_resetSwitchHandler, 0.5f);
			if (!IsOn())
			{
				SetFlag(Flags.On, b: true);
				SendNetworkUpdateImmediate();
				MarkDirty();
				BroadcastValueChange();
			}
		}
	}

	private void ResetSwitch()
	{
		SetFlag(Flags.On, b: false);
		SendNetworkUpdateImmediate();
		MarkDirty();
		BroadcastValueChange();
	}
}
