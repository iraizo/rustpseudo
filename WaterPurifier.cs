using System;
using Rust;
using UnityEngine;

public class WaterPurifier : LiquidContainer
{
	public static class WaterPurifierFlags
	{
		public const Flags Boiling = Flags.Reserved1;
	}

	public GameObjectRef storagePrefab;

	public Transform storagePrefabAnchor;

	public ItemDefinition freshWater;

	public int waterToProcessPerMinute = 120;

	public int freshWaterRatio = 4;

	public bool stopWhenOutputFull;

	protected LiquidContainer waterStorage;

	private float dirtyWaterProcssed;

	private float pendingFreshWater;

	public bool IsBoiling()
	{
		return HasFlag(Flags.Reserved1);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			SpawnStorageEnt(load: false);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SpawnStorageEnt(load: true);
	}

	protected virtual void SpawnStorageEnt(bool load)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (load)
		{
			BaseEntity baseEntity = GetParentEntity();
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				foreach (BaseEntity child in baseEntity.children)
				{
					LiquidContainer liquidContainer;
					if ((Object)(object)child != (Object)(object)this && (liquidContainer = child as LiquidContainer) != null)
					{
						waterStorage = liquidContainer;
						break;
					}
				}
			}
		}
		if ((Object)(object)waterStorage != (Object)null)
		{
			waterStorage.SetConnectedTo(this);
			return;
		}
		waterStorage = GameManager.server.CreateEntity(storagePrefab.resourcePath, storagePrefabAnchor.get_localPosition(), storagePrefabAnchor.get_localRotation()) as LiquidContainer;
		waterStorage.SetParent(GetParentEntity());
		waterStorage.Spawn();
		waterStorage.SetConnectedTo(this);
	}

	internal override void OnParentRemoved()
	{
		Kill(DestroyMode.Gib);
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (!waterStorage.IsDestroyed)
		{
			waterStorage.Kill();
		}
	}

	public void ParentTemperatureUpdate(float temp)
	{
	}

	public void CheckCoolDown()
	{
		if (!Object.op_Implicit((Object)(object)GetParentEntity()) || !GetParentEntity().IsOn() || !HasDirtyWater())
		{
			SetFlag(Flags.Reserved1, b: false);
			((FacepunchBehaviour)this).CancelInvoke((Action)CheckCoolDown);
		}
	}

	public bool HasDirtyWater()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot != null && slot.info.itemType == ItemContainer.ContentsType.Liquid)
		{
			return slot.amount > 0;
		}
		return false;
	}

	public void Cook(float timeCooked)
	{
		if (!((Object)(object)waterStorage == (Object)null))
		{
			bool flag = HasDirtyWater();
			if (!IsBoiling() && flag)
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)CheckCoolDown, 2f, 2f);
				SetFlag(Flags.Reserved1, b: true);
			}
			if (IsBoiling() && flag)
			{
				ConvertWater(timeCooked);
			}
		}
	}

	protected void ConvertWater(float timeCooked)
	{
		if (stopWhenOutputFull)
		{
			Item slot = waterStorage.inventory.GetSlot(0);
			if (slot != null && slot.amount >= slot.MaxStackable())
			{
				return;
			}
		}
		float num = timeCooked * ((float)waterToProcessPerMinute / 60f);
		dirtyWaterProcssed += num;
		if (dirtyWaterProcssed >= 1f)
		{
			Item slot2 = base.inventory.GetSlot(0);
			int num2 = Mathf.Min(Mathf.FloorToInt(dirtyWaterProcssed), slot2.amount);
			num = num2;
			slot2.UseItem(num2);
			dirtyWaterProcssed -= num2;
			SendNetworkUpdate();
		}
		pendingFreshWater += num / (float)freshWaterRatio;
		if (!(pendingFreshWater >= 1f))
		{
			return;
		}
		int num3 = Mathf.FloorToInt(pendingFreshWater);
		pendingFreshWater -= num3;
		Item slot3 = waterStorage.inventory.GetSlot(0);
		if (slot3 != null && (Object)(object)slot3.info != (Object)(object)freshWater)
		{
			slot3.RemoveFromContainer();
			slot3.Remove();
		}
		if (slot3 == null)
		{
			Item item = ItemManager.Create(freshWater, num3, 0uL);
			if (!item.MoveToContainer(waterStorage.inventory))
			{
				item.Remove();
			}
		}
		else
		{
			slot3.amount += num3;
			slot3.amount = Mathf.Clamp(slot3.amount, 0, waterStorage.maxStackSize);
			waterStorage.inventory.MarkDirty();
		}
		waterStorage.SendNetworkUpdate();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			SetFlag(Flags.On, b: false);
		}
	}
}
