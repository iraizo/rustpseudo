using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class Item
{
	[Flags]
	public enum Flag
	{
		None = 0x0,
		Placeholder = 0x1,
		IsOn = 0x2,
		OnFire = 0x4,
		IsLocked = 0x8,
		Cooking = 0x10
	}

	private static string DefaultArmourBreakEffectPath = string.Empty;

	private float _condition;

	private float _maxCondition = 100f;

	public ItemDefinition info;

	public uint uid;

	public bool dirty;

	public int amount = 1;

	public int position;

	public float busyTime;

	public float removeTime;

	public float fuel;

	public bool isServer;

	public InstanceData instanceData;

	public ulong skin;

	public string name;

	public string text;

	public Flag flags;

	public ItemContainer contents;

	public ItemContainer parent;

	private EntityRef worldEnt;

	private EntityRef heldEntity;

	public float condition
	{
		get
		{
			return _condition;
		}
		set
		{
			float num = _condition;
			_condition = Mathf.Clamp(value, 0f, maxCondition);
			if (isServer && Mathf.Ceil(value) != Mathf.Ceil(num))
			{
				MarkDirty();
			}
		}
	}

	public float maxCondition
	{
		get
		{
			return _maxCondition;
		}
		set
		{
			_maxCondition = Mathf.Clamp(value, 0f, info.condition.max);
			if (isServer)
			{
				MarkDirty();
			}
		}
	}

	public float maxConditionNormalized => _maxCondition / info.condition.max;

	public float conditionNormalized
	{
		get
		{
			if (!hasCondition)
			{
				return 1f;
			}
			return condition / maxCondition;
		}
		set
		{
			if (hasCondition)
			{
				condition = value * maxCondition;
			}
		}
	}

	public bool hasCondition
	{
		get
		{
			if ((Object)(object)info != (Object)null && info.condition.enabled)
			{
				return info.condition.max > 0f;
			}
			return false;
		}
	}

	public bool isBroken
	{
		get
		{
			if (hasCondition)
			{
				return condition <= 0f;
			}
			return false;
		}
	}

	public int despawnMultiplier
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected I4, but got Unknown
			if (!((Object)(object)info != (Object)null))
			{
				return 1;
			}
			return Mathf.Clamp((info.rarity - 1) * 4, 1, 100);
		}
	}

	public ItemDefinition blueprintTargetDef
	{
		get
		{
			if (!IsBlueprint())
			{
				return null;
			}
			return ItemManager.FindItemDefinition(blueprintTarget);
		}
	}

	public int blueprintTarget
	{
		get
		{
			if (instanceData == null)
			{
				return 0;
			}
			return instanceData.blueprintTarget;
		}
		set
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			if (instanceData == null)
			{
				instanceData = new InstanceData();
			}
			instanceData.ShouldPool = false;
			instanceData.blueprintTarget = value;
		}
	}

	public int blueprintAmount
	{
		get
		{
			return amount;
		}
		set
		{
			amount = value;
		}
	}

	public Item parentItem
	{
		get
		{
			if (parent == null)
			{
				return null;
			}
			return parent.parent;
		}
	}

	public float temperature
	{
		get
		{
			if (parent != null)
			{
				return parent.temperature;
			}
			return 15f;
		}
	}

	public BaseEntity.TraitFlag Traits => info.Traits;

	public event Action<Item> OnDirty;

	public event Action<Item, float> onCycle;

	public void LoseCondition(float amount)
	{
		if (hasCondition && !Debugging.disablecondition)
		{
			float num = condition;
			condition -= amount;
			if (Global.developer > 0)
			{
				Debug.Log((object)(info.shortname + " was damaged by: " + amount + "cond is: " + condition + "/" + maxCondition));
			}
			if (condition <= 0f && condition < num)
			{
				OnBroken();
			}
		}
	}

	public void RepairCondition(float amount)
	{
		if (hasCondition)
		{
			condition += amount;
		}
	}

	public void DoRepair(float maxLossFraction)
	{
		if (hasCondition)
		{
			if (info.condition.maintainMaxCondition)
			{
				maxLossFraction = 0f;
			}
			float num = 1f - condition / maxCondition;
			maxLossFraction = Mathf.Clamp(maxLossFraction, 0f, info.condition.max);
			maxCondition *= 1f - maxLossFraction * num;
			condition = maxCondition;
			BaseEntity baseEntity = GetHeldEntity();
			if ((Object)(object)baseEntity != (Object)null)
			{
				baseEntity.SetFlag(BaseEntity.Flags.Broken, b: false);
			}
			if (Global.developer > 0)
			{
				Debug.Log((object)(info.shortname + " was repaired! new cond is: " + condition + "/" + maxCondition));
			}
		}
	}

	public ItemContainer GetRootContainer()
	{
		ItemContainer itemContainer = parent;
		int num = 0;
		while (itemContainer != null && num <= 8 && itemContainer.parent != null && itemContainer.parent.parent != null)
		{
			itemContainer = itemContainer.parent.parent;
			num++;
		}
		if (num == 8)
		{
			Debug.LogWarning((object)"GetRootContainer failed with 8 iterations");
		}
		return itemContainer;
	}

	public virtual void OnBroken()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		if (!hasCondition)
		{
			return;
		}
		BaseEntity baseEntity = GetHeldEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			baseEntity.SetFlag(BaseEntity.Flags.Broken, b: true);
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (Object.op_Implicit((Object)(object)ownerPlayer))
		{
			if (ownerPlayer.GetActiveItem() == this)
			{
				Effect.server.Run("assets/bundled/prefabs/fx/item_break.prefab", ownerPlayer, 0u, Vector3.get_zero(), Vector3.get_zero());
				ownerPlayer.ChatMessage("Your active item was broken!");
			}
			ItemModWearable itemModWearable = default(ItemModWearable);
			if (((Component)info).TryGetComponent<ItemModWearable>(ref itemModWearable) && ownerPlayer.inventory.containerWear.itemList.Contains(this))
			{
				if (itemModWearable.breakEffect.isValid)
				{
					Effect.server.Run(itemModWearable.breakEffect.resourcePath, ownerPlayer, 0u, Vector3.get_zero(), Vector3.get_zero());
				}
				else if (!string.IsNullOrEmpty(DefaultArmourBreakEffectPath))
				{
					Effect.server.Run(DefaultArmourBreakEffectPath, ownerPlayer, 0u, Vector3.get_zero(), Vector3.get_zero());
				}
			}
		}
		if ((!info.condition.repairable && !Object.op_Implicit((Object)(object)((Component)info).GetComponent<ItemModRepair>())) || maxCondition <= 5f)
		{
			Remove();
		}
		else if (parent != null && parent.HasFlag(ItemContainer.Flag.NoBrokenItems))
		{
			ItemContainer rootContainer = GetRootContainer();
			if (rootContainer.HasFlag(ItemContainer.Flag.NoBrokenItems))
			{
				Remove();
			}
			else
			{
				BasePlayer playerOwner = rootContainer.playerOwner;
				if ((Object)(object)playerOwner != (Object)null && !MoveToContainer(playerOwner.inventory.containerMain))
				{
					Drop(((Component)playerOwner).get_transform().get_position(), playerOwner.eyes.BodyForward() * 1.5f);
				}
			}
		}
		MarkDirty();
	}

	public bool IsBlueprint()
	{
		return blueprintTarget != 0;
	}

	public bool HasFlag(Flag f)
	{
		return (flags & f) == f;
	}

	public void SetFlag(Flag f, bool b)
	{
		if (b)
		{
			flags |= f;
		}
		else
		{
			flags &= ~f;
		}
	}

	public bool IsOn()
	{
		return HasFlag(Flag.IsOn);
	}

	public bool IsOnFire()
	{
		return HasFlag(Flag.OnFire);
	}

	public bool IsCooking()
	{
		return HasFlag(Flag.Cooking);
	}

	public bool IsLocked()
	{
		if (!HasFlag(Flag.IsLocked))
		{
			if (parent != null)
			{
				return parent.IsLocked();
			}
			return false;
		}
		return true;
	}

	public void MarkDirty()
	{
		OnChanged();
		dirty = true;
		if (parent != null)
		{
			parent.MarkDirty();
		}
		if (this.OnDirty != null)
		{
			this.OnDirty(this);
		}
	}

	public void OnChanged()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnChanged(this);
		}
		if (contents != null)
		{
			contents.OnChanged();
		}
	}

	public void CollectedForCrafting(BasePlayer crafter)
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].CollectedForCrafting(this, crafter);
		}
	}

	public void ReturnedFromCancelledCraft(BasePlayer crafter)
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ReturnedFromCancelledCraft(this, crafter);
		}
	}

	public void Initialize(ItemDefinition template)
	{
		uid = Net.sv.TakeUID();
		float num2 = (condition = (maxCondition = info.condition.max));
		OnItemCreated();
	}

	public void OnItemCreated()
	{
		this.onCycle = null;
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnItemCreated(this);
		}
	}

	public void OnVirginSpawn()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnVirginItem(this);
		}
	}

	protected void RemoveFromWorld()
	{
		BaseEntity worldEntity = GetWorldEntity();
		if (!((Object)(object)worldEntity == (Object)null))
		{
			SetWorldEntity(null);
			OnRemovedFromWorld();
			if (contents != null)
			{
				contents.OnRemovedFromWorld();
			}
			if (worldEntity.IsValid())
			{
				worldEntity.Kill();
			}
		}
	}

	public void OnRemovedFromWorld()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnRemovedFromWorld(this);
		}
	}

	public void RemoveFromContainer()
	{
		if (parent != null)
		{
			SetParent(null);
		}
	}

	public void SetParent(ItemContainer target)
	{
		if (target == parent)
		{
			return;
		}
		if (parent != null)
		{
			parent.Remove(this);
			parent = null;
		}
		if (target == null)
		{
			position = 0;
		}
		else
		{
			parent = target;
			if (!parent.Insert(this))
			{
				Remove();
				Debug.LogError((object)"Item.SetParent caused remove - this shouldn't ever happen");
			}
		}
		MarkDirty();
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnParentChanged(this);
		}
	}

	public void OnAttacked(HitInfo hitInfo)
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnAttacked(this, hitInfo);
		}
	}

	public bool IsChildContainer(ItemContainer c)
	{
		if (contents == null)
		{
			return false;
		}
		if (contents == c)
		{
			return true;
		}
		foreach (Item item in contents.itemList)
		{
			if (item.IsChildContainer(c))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanMoveTo(ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true)
	{
		if (IsChildContainer(newcontainer))
		{
			return false;
		}
		if (newcontainer.CanAcceptItem(this, iTargetPos) != 0)
		{
			return false;
		}
		if (iTargetPos >= newcontainer.capacity)
		{
			return false;
		}
		if (parent != null && newcontainer == parent && iTargetPos == position)
		{
			return false;
		}
		return true;
	}

	public bool MoveToContainer(ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true, bool ignoreStackLimit = false)
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("MoveToContainer", 0);
		try
		{
			ItemContainer itemContainer = parent;
			if (!CanMoveTo(newcontainer, iTargetPos, allowStack))
			{
				return false;
			}
			if (iTargetPos >= 0 && newcontainer.SlotTaken(this, iTargetPos))
			{
				Item slot = newcontainer.GetSlot(iTargetPos);
				if (allowStack)
				{
					int num = slot.MaxStackable();
					if (slot.CanStack(this))
					{
						if (ignoreStackLimit)
						{
							num = int.MaxValue;
						}
						if (slot.amount >= num)
						{
							return false;
						}
						slot.amount += amount;
						slot.MarkDirty();
						RemoveFromWorld();
						RemoveFromContainer();
						Remove();
						int num2 = slot.amount - num;
						if (num2 > 0)
						{
							Item item = slot.SplitItem(num2);
							if (item != null && !item.MoveToContainer(newcontainer, -1, allowStack: false) && (itemContainer == null || !item.MoveToContainer(itemContainer)))
							{
								item.Drop(newcontainer.dropPosition, newcontainer.dropVelocity);
							}
							slot.amount = num;
						}
						return true;
					}
				}
				if (parent != null)
				{
					ItemContainer newcontainer2 = parent;
					int iTargetPos2 = position;
					if (!slot.CanMoveTo(newcontainer2, iTargetPos2))
					{
						return false;
					}
					RemoveFromContainer();
					slot.RemoveFromContainer();
					slot.MoveToContainer(newcontainer2, iTargetPos2);
					return MoveToContainer(newcontainer, iTargetPos);
				}
				return false;
			}
			if (parent == newcontainer)
			{
				if (iTargetPos >= 0 && iTargetPos != position && !parent.SlotTaken(this, iTargetPos))
				{
					position = iTargetPos;
					MarkDirty();
					return true;
				}
				return false;
			}
			if (iTargetPos == -1 && allowStack && info.stackable > 1)
			{
				Item item2 = Enumerable.FirstOrDefault<Item>((IEnumerable<Item>)Enumerable.OrderBy<Item, int>((IEnumerable<Item>)newcontainer.FindItemsByItemID(info.itemid), (Func<Item, int>)((Item x) => x.amount)));
				if (item2 != null && item2.CanStack(this))
				{
					int num3 = item2.MaxStackable();
					if (ignoreStackLimit)
					{
						num3 = int.MaxValue;
					}
					if (item2.amount < num3)
					{
						item2.amount += amount;
						item2.MarkDirty();
						int num4 = item2.amount - num3;
						if (num4 <= 0)
						{
							RemoveFromWorld();
							RemoveFromContainer();
							Remove();
							return true;
						}
						amount = num4;
						MarkDirty();
						item2.amount = num3;
						return MoveToContainer(newcontainer, iTargetPos, allowStack);
					}
				}
			}
			if (newcontainer.maxStackSize > 0 && newcontainer.maxStackSize < amount)
			{
				Item item3 = SplitItem(newcontainer.maxStackSize);
				if (item3 != null && !item3.MoveToContainer(newcontainer, iTargetPos, allowStack: false) && (itemContainer == null || !item3.MoveToContainer(itemContainer)))
				{
					item3.Drop(newcontainer.dropPosition, newcontainer.dropVelocity);
				}
				return true;
			}
			if (!newcontainer.CanAccept(this))
			{
				return false;
			}
			RemoveFromContainer();
			RemoveFromWorld();
			position = iTargetPos;
			SetParent(newcontainer);
			return true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public BaseEntity CreateWorldObject(Vector3 pos, Quaternion rotation = default(Quaternion), BaseEntity parentEnt = null, uint parentBone = 0u)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity worldEntity = GetWorldEntity();
		if ((Object)(object)worldEntity != (Object)null)
		{
			return worldEntity;
		}
		worldEntity = GameManager.server.CreateEntity("assets/prefabs/misc/burlap sack/generic_world.prefab", pos, rotation);
		if ((Object)(object)worldEntity == (Object)null)
		{
			Debug.LogWarning((object)"Couldn't create world object for prefab: items/generic_world");
			return null;
		}
		WorldItem worldItem = worldEntity as WorldItem;
		if ((Object)(object)worldItem != (Object)null)
		{
			worldItem.InitializeItem(this);
		}
		if ((Object)(object)parentEnt != (Object)null)
		{
			worldEntity.SetParent(parentEnt, parentBone);
		}
		worldEntity.Spawn();
		SetWorldEntity(worldEntity);
		return GetWorldEntity();
	}

	public BaseEntity Drop(Vector3 vPos, Vector3 vVelocity, Quaternion rotation = default(Quaternion))
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		RemoveFromWorld();
		BaseEntity baseEntity = null;
		if (vPos != Vector3.get_zero() && !info.HasFlag(ItemDefinition.Flag.NoDropping))
		{
			baseEntity = CreateWorldObject(vPos, rotation);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.SetVelocity(vVelocity);
			}
		}
		else
		{
			Remove();
		}
		RemoveFromContainer();
		return baseEntity;
	}

	public BaseEntity DropAndTossUpwards(Vector3 vPos, float force = 2f)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		float num = Random.get_value() * (float)Math.PI * 2f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Sin(num), 1f, Mathf.Cos(num));
		return Drop(vPos + Vector3.get_up() * 0.1f, val * force);
	}

	public bool IsBusy()
	{
		if (busyTime > Time.get_time())
		{
			return true;
		}
		return false;
	}

	public void BusyFor(float fTime)
	{
		busyTime = Time.get_time() + fTime;
	}

	public void Remove(float fTime = 0f)
	{
		if (removeTime > 0f)
		{
			return;
		}
		if (isServer)
		{
			ItemMod[] itemMods = info.itemMods;
			for (int i = 0; i < itemMods.Length; i++)
			{
				itemMods[i].OnRemove(this);
			}
		}
		this.onCycle = null;
		removeTime = Time.get_time() + fTime;
		this.OnDirty = null;
		position = -1;
		if (isServer)
		{
			ItemManager.RemoveItem(this, fTime);
		}
	}

	internal void DoRemove()
	{
		this.OnDirty = null;
		this.onCycle = null;
		if (isServer && uid != 0 && Net.sv != null)
		{
			Net.sv.ReturnUID(uid);
			uid = 0u;
		}
		if (contents != null)
		{
			contents.Kill();
			contents = null;
		}
		if (isServer)
		{
			RemoveFromWorld();
			RemoveFromContainer();
		}
		BaseEntity baseEntity = GetHeldEntity();
		if (baseEntity.IsValid())
		{
			Debug.LogWarning((object)("Item's Held Entity not removed!" + info.displayName.english + " -> " + baseEntity), (Object)(object)baseEntity);
		}
	}

	public void SwitchOnOff(bool bNewState)
	{
		if (HasFlag(Flag.IsOn) != bNewState)
		{
			SetFlag(Flag.IsOn, bNewState);
			MarkDirty();
		}
	}

	public void LockUnlock(bool bNewState)
	{
		if (HasFlag(Flag.IsLocked) != bNewState)
		{
			SetFlag(Flag.IsLocked, bNewState);
			MarkDirty();
		}
	}

	public BasePlayer GetOwnerPlayer()
	{
		if (parent == null)
		{
			return null;
		}
		return parent.GetOwnerPlayer();
	}

	public Item SplitItem(int split_Amount)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		Assert.IsTrue(split_Amount > 0, "split_Amount <= 0");
		if (split_Amount <= 0)
		{
			return null;
		}
		if (split_Amount >= amount)
		{
			return null;
		}
		amount -= split_Amount;
		Item item = ItemManager.CreateByItemID(info.itemid, 1, 0uL);
		item.amount = split_Amount;
		item.skin = skin;
		if (IsBlueprint())
		{
			item.blueprintTarget = blueprintTarget;
		}
		if (info.amountType == ItemDefinition.AmountType.Genetics && instanceData != null && instanceData.dataInt != 0)
		{
			item.instanceData = new InstanceData();
			item.instanceData.dataInt = instanceData.dataInt;
			item.instanceData.ShouldPool = false;
		}
		MarkDirty();
		return item;
	}

	public bool CanBeHeld()
	{
		if (isBroken)
		{
			return false;
		}
		return true;
	}

	public bool CanStack(Item item)
	{
		if (item == this)
		{
			return false;
		}
		if (info.stackable <= 1)
		{
			return false;
		}
		if (item.info.stackable <= 1)
		{
			return false;
		}
		if (item.info.itemid != info.itemid)
		{
			return false;
		}
		if (hasCondition && condition != item.info.condition.max)
		{
			return false;
		}
		if (item.hasCondition && item.condition != item.info.condition.max)
		{
			return false;
		}
		if (!IsValid())
		{
			return false;
		}
		if (IsBlueprint() && blueprintTarget != item.blueprintTarget)
		{
			return false;
		}
		if (item.skin != skin)
		{
			return false;
		}
		if (item.info.amountType == ItemDefinition.AmountType.Genetics || info.amountType == ItemDefinition.AmountType.Genetics)
		{
			int num = ((item.instanceData != null) ? item.instanceData.dataInt : (-1));
			int num2 = ((instanceData != null) ? instanceData.dataInt : (-1));
			if (num != num2)
			{
				return false;
			}
		}
		if (instanceData != null && instanceData.subEntity != 0 && Object.op_Implicit((Object)(object)((Component)info).GetComponent<ItemModSign>()))
		{
			return false;
		}
		if (item.instanceData != null && item.instanceData.subEntity != 0 && Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModSign>()))
		{
			return false;
		}
		return true;
	}

	public bool IsValid()
	{
		if (removeTime > 0f)
		{
			return false;
		}
		return true;
	}

	public void SetWorldEntity(BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			worldEnt.Set(null);
			MarkDirty();
		}
		else if (worldEnt.uid != ent.net.ID)
		{
			worldEnt.Set(ent);
			MarkDirty();
			OnMovedToWorld();
			if (contents != null)
			{
				contents.OnMovedToWorld();
			}
		}
	}

	public void OnMovedToWorld()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnMovedToWorld(this);
		}
	}

	public BaseEntity GetWorldEntity()
	{
		return worldEnt.Get(isServer);
	}

	public void SetHeldEntity(BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.heldEntity.Set(null);
			MarkDirty();
		}
		else
		{
			if (this.heldEntity.uid == ent.net.ID)
			{
				return;
			}
			this.heldEntity.Set(ent);
			MarkDirty();
			if (ent.IsValid())
			{
				HeldEntity heldEntity = ent as HeldEntity;
				if ((Object)(object)heldEntity != (Object)null)
				{
					heldEntity.SetupHeldEntity(this);
				}
			}
		}
	}

	public BaseEntity GetHeldEntity()
	{
		return heldEntity.Get(isServer);
	}

	public void OnCycle(float delta)
	{
		if (this.onCycle != null)
		{
			this.onCycle(this, delta);
		}
	}

	public void ServerCommand(string command, BasePlayer player)
	{
		HeldEntity heldEntity = GetHeldEntity() as HeldEntity;
		if ((Object)(object)heldEntity != (Object)null)
		{
			heldEntity.ServerCommand(this, command, player);
		}
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ServerCommand(this, command, player);
		}
	}

	public void UseItem(int amountToConsume = 1)
	{
		if (amountToConsume > 0)
		{
			amount -= amountToConsume;
			if (amount <= 0)
			{
				amount = 0;
				Remove();
			}
			else
			{
				MarkDirty();
			}
		}
	}

	public bool HasAmmo(AmmoTypes ammoType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ItemModProjectile itemModProjectile = default(ItemModProjectile);
		if (((Component)info).TryGetComponent<ItemModProjectile>(ref itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			return true;
		}
		if (contents != null)
		{
			return contents.HasAmmo(ammoType);
		}
		return false;
	}

	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		ItemModProjectile itemModProjectile = default(ItemModProjectile);
		if (((Component)info).TryGetComponent<ItemModProjectile>(ref itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			list.Add(this);
		}
		else if (contents != null)
		{
			contents.FindAmmo(list, ammoType);
		}
	}

	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		ItemModProjectile itemModProjectile = default(ItemModProjectile);
		if (((Component)info).TryGetComponent<ItemModProjectile>(ref itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			num += amount;
		}
		if (contents != null)
		{
			num += contents.GetAmmoAmount(ammoType);
		}
		return num;
	}

	public override string ToString()
	{
		return "Item." + info.shortname + "x" + amount + "." + uid;
	}

	public Item FindItem(uint iUID)
	{
		if (uid == iUID)
		{
			return this;
		}
		if (contents == null)
		{
			return null;
		}
		return contents.FindItemByUID(iUID);
	}

	public int MaxStackable()
	{
		int num = info.stackable;
		if (parent != null && parent.maxStackSize > 0)
		{
			num = Mathf.Min(parent.maxStackSize, num);
		}
		return num;
	}

	public virtual Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
	{
		dirty = false;
		Item val = Pool.Get<Item>();
		val.UID = uid;
		val.itemid = info.itemid;
		val.slot = position;
		val.amount = amount;
		val.flags = (int)flags;
		val.removetime = removeTime;
		val.locktime = busyTime;
		val.instanceData = instanceData;
		val.worldEntity = worldEnt.uid;
		val.heldEntity = heldEntity.uid;
		val.skinid = skin;
		val.name = name;
		val.text = text;
		if (hasCondition)
		{
			val.conditionData = Pool.Get<ConditionData>();
			val.conditionData.maxCondition = _maxCondition;
			val.conditionData.condition = _condition;
		}
		if (contents != null && bIncludeContainer)
		{
			val.contents = contents.Save();
		}
		return val;
	}

	public virtual void Load(Item load)
	{
		if ((Object)(object)info == (Object)null || info.itemid != load.itemid)
		{
			info = ItemManager.FindItemDefinition(load.itemid);
		}
		uid = load.UID;
		name = load.name;
		text = load.text;
		amount = load.amount;
		position = load.slot;
		busyTime = load.locktime;
		removeTime = load.removetime;
		flags = (Flag)load.flags;
		worldEnt.uid = load.worldEntity;
		heldEntity.uid = load.heldEntity;
		if (isServer)
		{
			Net.sv.RegisterUID(uid);
		}
		if (instanceData != null)
		{
			instanceData.ShouldPool = true;
			instanceData.ResetToPool();
			instanceData = null;
		}
		instanceData = load.instanceData;
		if (instanceData != null)
		{
			instanceData.ShouldPool = false;
		}
		skin = load.skinid;
		if ((Object)(object)info == (Object)null || info.itemid != load.itemid)
		{
			info = ItemManager.FindItemDefinition(load.itemid);
		}
		if ((Object)(object)info == (Object)null)
		{
			return;
		}
		_condition = 0f;
		_maxCondition = 0f;
		if (load.conditionData != null)
		{
			_condition = load.conditionData.condition;
			_maxCondition = load.conditionData.maxCondition;
		}
		else if (info.condition.enabled)
		{
			_condition = info.condition.max;
			_maxCondition = info.condition.max;
		}
		if (load.contents != null)
		{
			if (contents == null)
			{
				contents = new ItemContainer();
				if (isServer)
				{
					contents.ServerInitialize(this, load.contents.slots);
				}
			}
			contents.Load(load.contents);
		}
		if (isServer)
		{
			removeTime = 0f;
			OnItemCreated();
		}
	}
}
