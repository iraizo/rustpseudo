using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class ModularCarLock
{
	public enum LockType
	{
		Door,
		General
	}

	private readonly bool isServer;

	private readonly ModularCar owner;

	public int LockID;

	public const BaseEntity.Flags FLAG_CENTRAL_LOCKING = BaseEntity.Flags.Reserved2;

	public const float LOCK_DESTROY_HEALTH = 0.15f;

	public bool HasALock => LockID > 0;

	public bool IsLocked
	{
		get
		{
			if ((Object)(object)owner != (Object)null)
			{
				return owner.IsLocked();
			}
			return false;
		}
	}

	public bool CentralLockingIsOn
	{
		get
		{
			if ((Object)(object)owner != (Object)null)
			{
				return owner.HasFlag(BaseEntity.Flags.Reserved2);
			}
			return false;
		}
	}

	public ModularCarLock(ModularCar owner, bool isServer)
	{
		this.owner = owner;
		this.isServer = isServer;
		if (isServer)
		{
			EnableCentralLockingIfNoDriver();
		}
	}

	public bool PlayerHasUnlockPermission(BasePlayer player)
	{
		if (!HasALock)
		{
			return true;
		}
		if (player.IsDead())
		{
			return false;
		}
		return Enumerable.Any<Item>((IEnumerable<Item>)player.inventory.FindItemIDs(owner.carKeyDefinition.itemid), (Func<Item, bool>)((Item key) => KeyCanUnlockThis(key)));
	}

	public bool PlayerCanUseThis(BasePlayer player, LockType lockType)
	{
		if (lockType == LockType.Door && !CentralLockingIsOn)
		{
			return true;
		}
		return PlayerHasUnlockPermission(player);
	}

	public bool PlayerCanDestroyLock(BaseVehicleModule viaModule)
	{
		if (!HasALock)
		{
			return false;
		}
		return viaModule.healthFraction <= 0.15f;
	}

	private bool KeyCanUnlockThis(Item key)
	{
		if (HasALock && key.instanceData != null)
		{
			return key.instanceData.dataInt == LockID;
		}
		return false;
	}

	public bool CanHaveALock()
	{
		if (!owner.IsDead())
		{
			return owner.HasDriverMountPoints();
		}
		return false;
	}

	public void AddALock()
	{
		if (isServer && !HasALock && !owner.IsDead())
		{
			LockID = Random.Range(1, 100000);
			owner.SendNetworkUpdate();
		}
	}

	public void RemoveLock()
	{
		if (isServer && HasALock)
		{
			LockID = 0;
			owner.SendNetworkUpdate();
		}
	}

	public void EnableCentralLockingIfNoDriver()
	{
		if (!owner.HasDriver() && !CentralLockingIsOn)
		{
			owner.SetFlag(BaseEntity.Flags.Reserved2, b: true);
		}
	}

	public void ToggleCentralLocking()
	{
		owner.SetFlag(BaseEntity.Flags.Reserved2, !CentralLockingIsOn);
	}

	public bool CanCraftAKey(BasePlayer player, bool free)
	{
		ItemBlueprint bp = ItemManager.FindBlueprint(owner.carKeyDefinition);
		return player.inventory.crafting.CanCraft(bp, 1, free);
	}

	public bool TryCraftAKey(BasePlayer player, bool free)
	{
		if (!isServer)
		{
			return false;
		}
		if (!HasALock)
		{
			Debug.LogError((object)(GetType().Name + ": Can't create a key: No lock."));
			return false;
		}
		ItemBlueprint bp = ItemManager.FindBlueprint(owner.carKeyDefinition);
		if (player.inventory.crafting.CanCraft(bp, 1, free))
		{
			InstanceData val = Pool.Get<InstanceData>();
			val.dataInt = LockID;
			return player.inventory.crafting.CraftItem(bp, player, val, 1, 0, null, free);
		}
		return false;
	}
}
