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

public class PlayerInventory : EntityComponent<BasePlayer>
{
	public enum Type
	{
		Main,
		Belt,
		Wear
	}

	public interface ICanMoveFrom
	{
		bool CanMoveFrom(BasePlayer player, Item item);
	}

	public ItemContainer containerMain;

	public ItemContainer containerBelt;

	public ItemContainer containerWear;

	public ItemCrafter crafting;

	public PlayerLoot loot;

	[ServerVar]
	public static bool forceBirthday;

	private static float nextCheckTime;

	private static bool wasBirthday;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlayerInventory.OnRpcMessage", 0);
		try
		{
			BaseEntity.RPCMessage rPCMessage;
			if (rpc == 3482449460u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ItemCmd "));
				}
				TimeWarning val2 = TimeWarning.New("ItemCmd", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3482449460u, "ItemCmd", GetBaseEntity(), player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(BaseEntity.RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							BaseEntity.RPCMessage msg2 = rPCMessage;
							ItemCmd(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ItemCmd");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3041092525u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - MoveItem "));
				}
				TimeWarning val2 = TimeWarning.New("MoveItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3041092525u, "MoveItem", GetBaseEntity(), player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(BaseEntity.RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							BaseEntity.RPCMessage msg3 = rPCMessage;
							MoveItem(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in MoveItem");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	protected void Initialize()
	{
		containerMain = new ItemContainer();
		containerMain.SetFlag(ItemContainer.Flag.IsPlayer, b: true);
		containerBelt = new ItemContainer();
		containerBelt.SetFlag(ItemContainer.Flag.IsPlayer, b: true);
		containerBelt.SetFlag(ItemContainer.Flag.Belt, b: true);
		containerWear = new ItemContainer();
		containerWear.SetFlag(ItemContainer.Flag.IsPlayer, b: true);
		containerWear.SetFlag(ItemContainer.Flag.Clothing, b: true);
		crafting = ((Component)this).GetComponent<ItemCrafter>();
		crafting.AddContainer(containerMain);
		crafting.AddContainer(containerBelt);
		loot = ((Component)this).GetComponent<PlayerLoot>();
		if (!Object.op_Implicit((Object)(object)loot))
		{
			loot = ((Component)this).get_gameObject().AddComponent<PlayerLoot>();
		}
	}

	public void DoDestroy()
	{
		if (containerMain != null)
		{
			containerMain.Kill();
			containerMain = null;
		}
		if (containerBelt != null)
		{
			containerBelt.Kill();
			containerBelt = null;
		}
		if (containerWear != null)
		{
			containerWear.Kill();
			containerWear = null;
		}
	}

	public void ServerInit(BasePlayer owner)
	{
		Initialize();
		containerMain.ServerInitialize(null, 24);
		if (containerMain.uid == 0)
		{
			containerMain.GiveUID();
		}
		containerBelt.ServerInitialize(null, 6);
		if (containerBelt.uid == 0)
		{
			containerBelt.GiveUID();
		}
		containerWear.ServerInitialize(null, 7);
		if (containerWear.uid == 0)
		{
			containerWear.GiveUID();
		}
		containerMain.playerOwner = owner;
		containerBelt.playerOwner = owner;
		containerWear.playerOwner = owner;
		containerWear.onItemAddedRemoved = OnClothingChanged;
		containerWear.canAcceptItem = CanWearItem;
		containerBelt.canAcceptItem = CanEquipItem;
		containerMain.onPreItemRemove = OnItemRemoved;
		containerWear.onPreItemRemove = OnItemRemoved;
		containerBelt.onPreItemRemove = OnItemRemoved;
		containerMain.onDirty += OnContentsDirty;
		containerBelt.onDirty += OnContentsDirty;
		containerWear.onDirty += OnContentsDirty;
		containerBelt.onItemAddedRemoved = OnItemAddedOrRemoved;
		containerMain.onItemAddedRemoved = OnItemAddedOrRemoved;
	}

	public void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		if (item.info.isHoldable)
		{
			((FacepunchBehaviour)this).Invoke((Action)UpdatedVisibleHolsteredItems, 0.1f);
		}
		if (bAdded)
		{
			BasePlayer basePlayer = base.baseEntity;
			if (!basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash) && basePlayer.IsHostileItem(item))
			{
				base.baseEntity.SetPlayerFlag(BasePlayer.PlayerFlags.DisplaySash, b: true);
			}
			if (bAdded)
			{
				basePlayer.ProcessMissionEvent(BaseMission.MissionEventType.ACQUIRE_ITEM, item.info.shortname, item.amount);
			}
		}
	}

	public void UpdatedVisibleHolsteredItems()
	{
		List<HeldEntity> list = Pool.GetList<HeldEntity>();
		List<Item> items = Pool.GetList<Item>();
		AllItemsNoAlloc(ref items);
		foreach (Item item in items)
		{
			if (item.info.isHoldable && !((Object)(object)item.GetHeldEntity() == (Object)null))
			{
				HeldEntity component = ((Component)item.GetHeldEntity()).GetComponent<HeldEntity>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Pool.FreeList<Item>(ref items);
		IOrderedEnumerable<HeldEntity> obj = Enumerable.OrderByDescending<HeldEntity, float>((IEnumerable<HeldEntity>)list, (Func<HeldEntity, float>)((HeldEntity x) => x.hostileScore));
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		foreach (HeldEntity item2 in (IEnumerable<HeldEntity>)obj)
		{
			if (!((Object)(object)item2 == (Object)null) && item2.holsterInfo.displayWhenHolstered)
			{
				if (flag3 && !item2.IsDeployed() && item2.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.BACK)
				{
					item2.SetVisibleWhileHolstered(visible: true);
					flag3 = false;
				}
				else if (flag2 && !item2.IsDeployed() && item2.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.RIGHT_THIGH)
				{
					item2.SetVisibleWhileHolstered(visible: true);
					flag2 = false;
				}
				else if (flag && !item2.IsDeployed() && item2.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.LEFT_THIGH)
				{
					item2.SetVisibleWhileHolstered(visible: true);
					flag = false;
				}
				else
				{
					item2.SetVisibleWhileHolstered(visible: false);
				}
			}
		}
		Pool.FreeList<HeldEntity>(ref list);
	}

	private void OnContentsDirty()
	{
		if ((Object)(object)base.baseEntity != (Object)null)
		{
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	private bool CanMoveItemsFrom(BaseEntity entity, Item item)
	{
		ICanMoveFrom canMoveFrom;
		if ((canMoveFrom = entity as ICanMoveFrom) != null && !canMoveFrom.CanMoveFrom(base.baseEntity, item))
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			return BaseGameMode.GetActiveGameMode(serverside: true).CanMoveItemsFrom(this, entity, item);
		}
		return true;
	}

	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void ItemCmd(BaseEntity.RPCMessage msg)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)msg.player != (Object)null && msg.player.IsWounded())
		{
			return;
		}
		uint id = msg.read.UInt32();
		string text = msg.read.String(256);
		Item item = FindItemUID(id);
		if (item == null || item.IsLocked() || !CanMoveItemsFrom(item.parent.entityOwner, item))
		{
			return;
		}
		if (text == "drop")
		{
			int num = item.amount;
			if (msg.read.get_Unread() >= 4)
			{
				num = msg.read.Int32();
			}
			base.baseEntity.stats.Add("item_drop", 1, (Stats)5);
			if (num < item.amount)
			{
				item.SplitItem(num)?.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity());
			}
			else
			{
				item.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity());
			}
			base.baseEntity.SignalBroadcast(BaseEntity.Signal.Gesture, "drop_item");
		}
		else
		{
			item.ServerCommand(text, base.baseEntity);
			ItemManager.DoRemoves();
			ServerUpdate(0f);
		}
	}

	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void MoveItem(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		uint num2 = msg.read.UInt32();
		int iTargetPos = msg.read.Int8();
		int num3 = (int)msg.read.UInt32();
		Item item = FindItemUID(num);
		if (item == null)
		{
			msg.player.ChatMessage("Invalid item (" + num + ")");
			return;
		}
		if (!CanMoveItemsFrom(item.parent.entityOwner, item))
		{
			msg.player.ChatMessage("Cannot move item!");
			return;
		}
		if (num3 <= 0)
		{
			num3 = item.amount;
		}
		num3 = Mathf.Clamp(num3, 1, item.info.stackable);
		if (msg.player.GetActiveItem() == item)
		{
			msg.player.UpdateActiveItem(0u);
		}
		if (num2 == 0)
		{
			if (!GiveItem(item))
			{
				msg.player.ChatMessage("GiveItem failed!");
			}
			return;
		}
		ItemContainer itemContainer = FindContainer(num2);
		if (itemContainer == null)
		{
			msg.player.ChatMessage("Invalid container (" + num2 + ")");
			return;
		}
		ItemContainer parent = item.parent;
		if ((parent != null && parent.IsLocked()) || itemContainer.IsLocked())
		{
			msg.player.ChatMessage("Container is locked!");
			return;
		}
		if (itemContainer.PlayerItemInputBlocked())
		{
			msg.player.ChatMessage("Container does not accept player items!");
			return;
		}
		TimeWarning val = TimeWarning.New("Split", 0);
		try
		{
			if (item.amount > num3)
			{
				int split_Amount = num3;
				if (itemContainer.maxStackSize > 0)
				{
					split_Amount = Mathf.Min(num3, itemContainer.maxStackSize);
				}
				Item item2 = item.SplitItem(split_Amount);
				if (!item2.MoveToContainer(itemContainer, iTargetPos))
				{
					item.amount += item2.amount;
					item2.Remove();
				}
				ItemManager.DoRemoves();
				ServerUpdate(0f);
				return;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (item.MoveToContainer(itemContainer, iTargetPos))
		{
			ItemManager.DoRemoves();
			ServerUpdate(0f);
		}
	}

	private void OnClothingChanged(Item item, bool bAdded)
	{
		base.baseEntity.SV_ClothingChanged();
		ItemManager.DoRemoves();
		ServerUpdate(0f);
	}

	private void OnItemRemoved(Item item)
	{
		base.baseEntity.InvalidateNetworkCache();
	}

	private bool CanEquipItem(Item item, int targetSlot)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		ItemModContainerRestriction component = ((Component)item.info).GetComponent<ItemModContainerRestriction>();
		if ((Object)(object)component == (Object)null)
		{
			return true;
		}
		Item[] array = containerBelt.itemList.ToArray();
		foreach (Item item2 in array)
		{
			if (item2 != item)
			{
				ItemModContainerRestriction component2 = ((Component)item2.info).GetComponent<ItemModContainerRestriction>();
				if (!((Object)(object)component2 == (Object)null) && !component.CanExistWith(component2) && !item2.MoveToContainer(containerMain))
				{
					item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity());
				}
			}
		}
		return true;
	}

	private bool CanWearItem(Item item, int targetSlot)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		ItemModWearable component = ((Component)item.info).GetComponent<ItemModWearable>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		if (component.npcOnly)
		{
			BasePlayer basePlayer = base.baseEntity;
			if ((Object)(object)basePlayer != (Object)null && !basePlayer.IsNpc)
			{
				return false;
			}
		}
		Item[] array = containerWear.itemList.ToArray();
		foreach (Item item2 in array)
		{
			if (item2 == item)
			{
				continue;
			}
			ItemModWearable component2 = ((Component)item2.info).GetComponent<ItemModWearable>();
			if (!((Object)(object)component2 == (Object)null) && !component.CanExistWith(component2))
			{
				bool flag = false;
				if (item.parent == containerBelt)
				{
					flag = item2.MoveToContainer(containerBelt);
				}
				if (!flag && !item2.MoveToContainer(containerMain))
				{
					item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity());
				}
			}
		}
		return true;
	}

	public void ServerUpdate(float delta)
	{
		loot.Check();
		if (delta > 0f)
		{
			crafting.ServerUpdate(delta);
		}
		float currentTemperature = base.baseEntity.currentTemperature;
		UpdateContainer(delta, Type.Main, containerMain, bSendInventoryToEveryone: false, currentTemperature);
		UpdateContainer(delta, Type.Belt, containerBelt, bSendInventoryToEveryone: true, currentTemperature);
		UpdateContainer(delta, Type.Wear, containerWear, bSendInventoryToEveryone: true, currentTemperature);
	}

	public void UpdateContainer(float delta, Type type, ItemContainer container, bool bSendInventoryToEveryone, float temperature)
	{
		if (container != null)
		{
			container.temperature = temperature;
			if (delta > 0f)
			{
				container.OnCycle(delta);
			}
			if (container.dirty)
			{
				SendUpdatedInventory(type, container, bSendInventoryToEveryone);
				base.baseEntity.InvalidateNetworkCache();
			}
		}
	}

	public void SendSnapshot()
	{
		TimeWarning val = TimeWarning.New("PlayerInventory.SendSnapshot", 0);
		try
		{
			SendUpdatedInventory(Type.Main, containerMain);
			SendUpdatedInventory(Type.Belt, containerBelt, bSendInventoryToEveryone: true);
			SendUpdatedInventory(Type.Wear, containerWear, bSendInventoryToEveryone: true);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SendUpdatedInventory(Type type, ItemContainer container, bool bSendInventoryToEveryone = false)
	{
		UpdateItemContainer val = Pool.Get<UpdateItemContainer>();
		try
		{
			val.type = (int)type;
			if (container != null)
			{
				container.dirty = false;
				val.container = Pool.Get<List<ItemContainer>>();
				val.container.Add(container.Save());
			}
			if (bSendInventoryToEveryone)
			{
				base.baseEntity.ClientRPC<UpdateItemContainer>(null, "UpdatedItemContainer", val);
			}
			else
			{
				base.baseEntity.ClientRPCPlayer<UpdateItemContainer>(null, base.baseEntity, "UpdatedItemContainer", val);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public Item FindItemUID(uint id)
	{
		if (id == 0)
		{
			return null;
		}
		if (containerMain != null)
		{
			Item item = containerMain.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (containerBelt != null)
		{
			Item item2 = containerBelt.FindItemByUID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (containerWear != null)
		{
			Item item3 = containerWear.FindItemByUID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return loot.FindItem(id);
	}

	public Item FindItemID(string itemName)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemName);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return null;
		}
		return FindItemID(itemDefinition.itemid);
	}

	public Item FindItemID(int id)
	{
		if (containerMain != null)
		{
			Item item = containerMain.FindItemByItemID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (containerBelt != null)
		{
			Item item2 = containerBelt.FindItemByItemID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (containerWear != null)
		{
			Item item3 = containerWear.FindItemByItemID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	public List<Item> FindItemIDs(int id)
	{
		List<Item> list = new List<Item>();
		if (containerMain != null)
		{
			list.AddRange(containerMain.FindItemsByItemID(id));
		}
		if (containerBelt != null)
		{
			list.AddRange(containerBelt.FindItemsByItemID(id));
		}
		if (containerWear != null)
		{
			list.AddRange(containerWear.FindItemsByItemID(id));
		}
		return list;
	}

	public ItemContainer FindContainer(uint id)
	{
		TimeWarning val = TimeWarning.New("FindContainer", 0);
		try
		{
			ItemContainer itemContainer = containerMain.FindContainer(id);
			if (itemContainer != null)
			{
				return itemContainer;
			}
			itemContainer = containerBelt.FindContainer(id);
			if (itemContainer != null)
			{
				return itemContainer;
			}
			itemContainer = containerWear.FindContainer(id);
			if (itemContainer != null)
			{
				return itemContainer;
			}
			return loot.FindContainer(id);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public ItemContainer GetContainer(Type id)
	{
		if (id == Type.Main)
		{
			return containerMain;
		}
		if (Type.Belt == id)
		{
			return containerBelt;
		}
		if (Type.Wear == id)
		{
			return containerWear;
		}
		return null;
	}

	public bool GiveItem(Item item, ItemContainer container = null)
	{
		if (item == null)
		{
			return false;
		}
		int position = -1;
		GetIdealPickupContainer(item, ref container, ref position);
		if (container != null && item.MoveToContainer(container, position))
		{
			return true;
		}
		if (item.MoveToContainer(containerMain))
		{
			return true;
		}
		if (item.MoveToContainer(containerBelt))
		{
			return true;
		}
		return false;
	}

	protected void GetIdealPickupContainer(Item item, ref ItemContainer container, ref int position)
	{
		if (item.info.stackable > 1)
		{
			if (containerBelt != null && containerBelt.FindItemByItemID(item.info.itemid) != null)
			{
				container = containerBelt;
				return;
			}
			if (containerMain != null && containerMain.FindItemByItemID(item.info.itemid) != null)
			{
				container = containerMain;
				return;
			}
		}
		if (item.info.isUsable && !item.info.HasFlag(ItemDefinition.Flag.NotStraightToBelt))
		{
			container = containerBelt;
		}
	}

	public void Strip()
	{
		containerMain.Clear();
		containerBelt.Clear();
		containerWear.Clear();
		ItemManager.DoRemoves();
	}

	public static bool IsBirthday()
	{
		if (forceBirthday)
		{
			return true;
		}
		if (Time.get_time() < nextCheckTime)
		{
			return wasBirthday;
		}
		nextCheckTime = Time.get_time() + 60f;
		DateTime now = DateTime.Now;
		wasBirthday = now.Day == 11 && now.Month == 12;
		return wasBirthday;
	}

	public static bool IsChristmas()
	{
		return XMas.enabled;
	}

	public void GiveDefaultItems()
	{
		Strip();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if ((Object)(object)activeGameMode != (Object)null && activeGameMode.HasLoadouts())
		{
			BaseGameMode.GetActiveGameMode(serverside: true).LoadoutPlayer(base.baseEntity);
			return;
		}
		ulong num = 0uL;
		int infoInt = base.baseEntity.GetInfoInt("client.rockskin", 0);
		bool flag = false;
		if (infoInt > 0 && base.baseEntity.blueprints.steamInventory.HasItem(infoInt))
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition("rock");
			if ((Object)(object)itemDefinition != (Object)null && ItemDefinition.FindSkin(itemDefinition.itemid, infoInt) != 0L)
			{
				IPlayerItemDefinition itemDefinition2 = PlatformService.Instance.GetItemDefinition(infoInt);
				if (itemDefinition2 != null)
				{
					num = itemDefinition2.get_WorkshopDownload();
				}
				if (num == 0L && itemDefinition.skins != null)
				{
					ItemSkinDirectory.Skin[] skins = itemDefinition.skins;
					for (int i = 0; i < skins.Length; i++)
					{
						ItemSkinDirectory.Skin skin = skins[i];
						ItemSkin itemSkin;
						if (skin.id == infoInt && (Object)(object)skin.invItem != (Object)null && (itemSkin = skin.invItem as ItemSkin) != null && (Object)(object)itemSkin.Redirect != (Object)null)
						{
							GiveItem(ItemManager.CreateByName(itemSkin.Redirect.shortname, 1, 0uL), containerBelt);
							flag = true;
							break;
						}
					}
				}
			}
		}
		if (!flag)
		{
			GiveItem(ItemManager.CreateByName("rock", 1, num), containerBelt);
		}
		GiveItem(ItemManager.CreateByName("torch", 1, 0uL), containerBelt);
		if (IsBirthday())
		{
			GiveItem(ItemManager.CreateByName("cakefiveyear", 1, 0uL), containerBelt);
			GiveItem(ItemManager.CreateByName("partyhat", 1, 0uL), containerWear);
		}
		if (IsChristmas())
		{
			GiveItem(ItemManager.CreateByName("snowball", 1, 0uL), containerBelt);
			GiveItem(ItemManager.CreateByName("snowball", 1, 0uL), containerBelt);
			GiveItem(ItemManager.CreateByName("snowball", 1, 0uL), containerBelt);
		}
	}

	public PlayerInventory Save(bool bForDisk)
	{
		PlayerInventory val = Pool.Get<PlayerInventory>();
		if (bForDisk)
		{
			val.invMain = containerMain.Save();
		}
		val.invBelt = containerBelt.Save();
		val.invWear = containerWear.Save();
		return val;
	}

	public void Load(PlayerInventory msg)
	{
		if (msg.invMain != null)
		{
			containerMain.Load(msg.invMain);
		}
		if (msg.invBelt != null)
		{
			containerBelt.Load(msg.invBelt);
		}
		if (msg.invWear != null)
		{
			containerWear.Load(msg.invWear);
		}
	}

	public int Take(List<Item> collect, int itemid, int amount)
	{
		int num = 0;
		if (containerMain != null)
		{
			int num2 = containerMain.Take(collect, itemid, amount);
			num += num2;
			amount -= num2;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (containerBelt != null)
		{
			int num3 = containerBelt.Take(collect, itemid, amount);
			num += num3;
			amount -= num3;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (containerWear != null)
		{
			int num4 = containerWear.Take(collect, itemid, amount);
			num += num4;
			amount -= num4;
		}
		return num;
	}

	public int GetAmount(int itemid)
	{
		if (itemid == 0)
		{
			return 0;
		}
		int num = 0;
		if (containerMain != null)
		{
			num += containerMain.GetAmount(itemid, onlyUsableAmounts: true);
		}
		if (containerBelt != null)
		{
			num += containerBelt.GetAmount(itemid, onlyUsableAmounts: true);
		}
		if (containerWear != null)
		{
			num += containerWear.GetAmount(itemid, onlyUsableAmounts: true);
		}
		return num;
	}

	public Item[] AllItems()
	{
		List<Item> list = new List<Item>();
		if (containerMain != null)
		{
			list.AddRange(containerMain.itemList);
		}
		if (containerBelt != null)
		{
			list.AddRange(containerBelt.itemList);
		}
		if (containerWear != null)
		{
			list.AddRange(containerWear.itemList);
		}
		return list.ToArray();
	}

	public int AllItemsNoAlloc(ref List<Item> items)
	{
		items.Clear();
		if (containerMain != null)
		{
			items.AddRange(containerMain.itemList);
		}
		if (containerBelt != null)
		{
			items.AddRange(containerBelt.itemList);
		}
		if (containerWear != null)
		{
			items.AddRange(containerWear.itemList);
		}
		return items.Count;
	}

	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (containerMain != null)
		{
			containerMain.FindAmmo(list, ammoType);
		}
		if (containerBelt != null)
		{
			containerBelt.FindAmmo(list, ammoType);
		}
	}

	public bool HasAmmo(AmmoTypes ammoType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!containerMain.HasAmmo(ammoType))
		{
			return containerBelt.HasAmmo(ammoType);
		}
		return true;
	}
}
