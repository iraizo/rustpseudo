using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class VendingMachine : StorageContainer
{
	public static class VendingMachineFlags
	{
		public const Flags EmptyInv = Flags.Reserved1;

		public const Flags IsVending = Flags.Reserved2;

		public const Flags Broadcasting = Flags.Reserved4;

		public const Flags OutOfStock = Flags.Reserved5;

		public const Flags NoDirectAccess = Flags.Reserved6;
	}

	[Header("VendingMachine")]
	public static readonly Phrase WaitForVendingMessage = new Phrase("vendingmachine.wait", "Please wait...");

	public GameObjectRef adminMenuPrefab;

	public string customerPanel = "";

	public SellOrderContainer sellOrders;

	public SoundPlayer buySound;

	public string shopName = "A Shop";

	public GameObjectRef mapMarkerPrefab;

	public ItemDefinition blueprintBaseDef;

	private Action fullUpdateCached;

	protected BasePlayer vend_Player;

	private int vend_sellOrderID;

	private int vend_numberOfTransactions;

	protected bool transactionActive;

	private VendingMachineMapMarker myMarker;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("VendingMachine.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 3011053703u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - BuyItem "));
				}
				TimeWarning val2 = TimeWarning.New("BuyItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3011053703u, "BuyItem", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3011053703u, "BuyItem", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage rpc2 = rPCMessage;
							BuyItem(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BuyItem");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1626480840 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_AddSellOrder "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_AddSellOrder", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1626480840u, "RPC_AddSellOrder", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							RPC_AddSellOrder(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_AddSellOrder");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 169239598 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Broadcast "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Broadcast", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(169239598u, "RPC_Broadcast", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg3 = rPCMessage;
							RPC_Broadcast(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Broadcast");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3680901137u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_DeleteSellOrder "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_DeleteSellOrder", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3680901137u, "RPC_DeleteSellOrder", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg4 = rPCMessage;
							RPC_DeleteSellOrder(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_DeleteSellOrder");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2555993359u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenAdmin "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenAdmin", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2555993359u, "RPC_OpenAdmin", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg5 = rPCMessage;
							RPC_OpenAdmin(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_OpenAdmin");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 36164441 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenShop "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenShop", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(36164441u, "RPC_OpenShop", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg6 = rPCMessage;
							RPC_OpenShop(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_OpenShop");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3346513099u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RotateVM "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RotateVM", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3346513099u, "RPC_RotateVM", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg7 = rPCMessage;
							RPC_RotateVM(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in RPC_RotateVM");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1012779214 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_UpdateShopName "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_UpdateShopName", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1012779214u, "RPC_UpdateShopName", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg8 = rPCMessage;
							RPC_UpdateShopName(msg8);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in RPC_UpdateShopName");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3559014831u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - TransactionStart "));
				}
				TimeWarning val2 = TimeWarning.New("TransactionStart", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3559014831u, "TransactionStart", this, player, 3f))
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
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage rpc3 = rPCMessage;
							TransactionStart(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in TransactionStart");
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

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vendingMachine != null)
		{
			shopName = info.msg.vendingMachine.shopName;
			if (info.msg.vendingMachine.sellOrderContainer != null)
			{
				sellOrders = info.msg.vendingMachine.sellOrderContainer;
				sellOrders.ShouldPool = false;
			}
			if (info.fromDisk && base.isServer)
			{
				RefreshSellOrderStockLevel();
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		base.Save(info);
		info.msg.vendingMachine = new VendingMachine();
		info.msg.vendingMachine.ShouldPool = false;
		info.msg.vendingMachine.shopName = shopName;
		if (sellOrders == null)
		{
			return;
		}
		info.msg.vendingMachine.sellOrderContainer = new SellOrderContainer();
		info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
		info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<SellOrder>();
		foreach (SellOrder sellOrder in sellOrders.sellOrders)
		{
			SellOrder val = new SellOrder();
			val.ShouldPool = false;
			sellOrder.CopyTo(val);
			info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(val);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isServer)
		{
			InstallDefaultSellOrders();
			SetFlag(Flags.Reserved2, b: false);
			base.inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
			RefreshSellOrderStockLevel();
			ItemContainer itemContainer = base.inventory;
			itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
			UpdateMapMarker();
			fullUpdateCached = FullUpdate;
		}
	}

	public override void DestroyShared()
	{
		if (Object.op_Implicit((Object)(object)myMarker))
		{
			myMarker.Kill();
			myMarker = null;
		}
		base.DestroyShared();
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	public void FullUpdate()
	{
		RefreshSellOrderStockLevel();
		UpdateMapMarker();
		SendNetworkUpdate();
	}

	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		((FacepunchBehaviour)this).CancelInvoke(fullUpdateCached);
		((FacepunchBehaviour)this).Invoke(fullUpdateCached, 0.2f);
	}

	public void RefreshSellOrderStockLevel(ItemDefinition itemDef = null)
	{
		foreach (SellOrder sellOrder in sellOrders.sellOrders)
		{
			if ((Object)(object)itemDef == (Object)null || itemDef.itemid == sellOrder.itemToSellID)
			{
				List<Item> list = Pool.GetList<Item>();
				GetItemsToSell(sellOrder, list);
				sellOrder.inStock = ((list.Count >= 0) ? (Enumerable.Sum<Item>((IEnumerable<Item>)list, (Func<Item, int>)((Item x) => x.amount)) / sellOrder.itemToSellAmount) : 0);
				float itemCondition = 0f;
				float itemConditionMax = 0f;
				if (list.Count > 0 && list[0].hasCondition)
				{
					itemCondition = list[0].condition;
					itemConditionMax = list[0].maxCondition;
				}
				sellOrder.itemCondition = itemCondition;
				sellOrder.itemConditionMax = itemConditionMax;
				Pool.FreeList<Item>(ref list);
			}
		}
	}

	public bool OutOfStock()
	{
		foreach (SellOrder sellOrder in sellOrders.sellOrders)
		{
			if (sellOrder.inStock > 0)
			{
				return true;
			}
		}
		return false;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Reserved2, b: false);
		RefreshSellOrderStockLevel();
		UpdateMapMarker();
	}

	public void UpdateEmptyFlag()
	{
		SetFlag(Flags.Reserved1, base.inventory.itemList.Count == 0);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		UpdateEmptyFlag();
		if ((Object)(object)vend_Player != (Object)null && (Object)(object)vend_Player == (Object)(object)player)
		{
			ClearPendingOrder();
		}
	}

	public virtual void InstallDefaultSellOrders()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		sellOrders = new SellOrderContainer();
		sellOrders.ShouldPool = false;
		sellOrders.sellOrders = new List<SellOrder>();
	}

	public virtual bool HasVendingSounds()
	{
		return true;
	}

	public virtual float GetBuyDuration()
	{
		return 2.5f;
	}

	public void SetPendingOrder(BasePlayer buyer, int sellOrderId, int numberOfTransactions)
	{
		ClearPendingOrder();
		vend_Player = buyer;
		vend_sellOrderID = sellOrderId;
		vend_numberOfTransactions = numberOfTransactions;
		SetFlag(Flags.Reserved2, b: true);
		if (HasVendingSounds())
		{
			ClientRPC(null, "CLIENT_StartVendingSounds", sellOrderId);
		}
	}

	public void ClearPendingOrder()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)CompletePendingOrder);
		vend_Player = null;
		vend_sellOrderID = -1;
		vend_numberOfTransactions = -1;
		SetFlag(Flags.Reserved2, b: false);
		ClientRPC(null, "CLIENT_CancelVendingSounds");
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void BuyItem(RPCMessage rpc)
	{
		if (OccupiedCheck(rpc.player))
		{
			int sellOrderId = rpc.read.Int32();
			int numberOfTransactions = rpc.read.Int32();
			if (IsVending())
			{
				rpc.player.ShowToast(1, WaitForVendingMessage);
				return;
			}
			SetPendingOrder(rpc.player, sellOrderId, numberOfTransactions);
			((FacepunchBehaviour)this).Invoke((Action)CompletePendingOrder, GetBuyDuration());
		}
	}

	public virtual void CompletePendingOrder()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DoTransaction(vend_Player, vend_sellOrderID, vend_numberOfTransactions);
		ClearPendingOrder();
		Decay.RadialDecayTouch(((Component)this).get_transform().get_position(), 40f, 2097408);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void TransactionStart(RPCMessage rpc)
	{
	}

	private void GetItemsToSell(SellOrder sellOrder, List<Item> items)
	{
		if (sellOrder.itemToSellIsBP)
		{
			foreach (Item item in base.inventory.itemList)
			{
				if (item.info.itemid == blueprintBaseDef.itemid && item.blueprintTarget == sellOrder.itemToSellID)
				{
					items.Add(item);
				}
			}
			return;
		}
		foreach (Item item2 in base.inventory.itemList)
		{
			if (item2.info.itemid == sellOrder.itemToSellID)
			{
				items.Add(item2);
			}
		}
	}

	public bool DoTransaction(BasePlayer buyer, int sellOrderId, int numberOfTransactions = 1, ItemContainer targetContainer = null, Action<BasePlayer, Item> onCurrencyRemoved = null, Action<BasePlayer, Item> onItemPurchased = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		if (sellOrderId < 0 || sellOrderId >= sellOrders.sellOrders.Count)
		{
			return false;
		}
		if (targetContainer == null && Vector3.Distance(((Component)buyer).get_transform().get_position(), ((Component)this).get_transform().get_position()) > 4f)
		{
			return false;
		}
		SellOrder sellOrder = sellOrders.sellOrders[sellOrderId];
		List<Item> list = Pool.GetList<Item>();
		GetItemsToSell(sellOrder, list);
		if (list == null || list.Count == 0)
		{
			Pool.FreeList<Item>(ref list);
			return false;
		}
		numberOfTransactions = Mathf.Clamp(numberOfTransactions, 1, list[0].hasCondition ? 1 : 1000000);
		int num = sellOrder.itemToSellAmount * numberOfTransactions;
		int num2 = Enumerable.Sum<Item>((IEnumerable<Item>)list, (Func<Item, int>)((Item x) => x.amount));
		if (num > num2)
		{
			Pool.FreeList<Item>(ref list);
			return false;
		}
		List<Item> list2 = buyer.inventory.FindItemIDs(sellOrder.currencyID);
		if (sellOrder.currencyIsBP)
		{
			list2 = Enumerable.ToList<Item>(Enumerable.Where<Item>((IEnumerable<Item>)buyer.inventory.FindItemIDs(blueprintBaseDef.itemid), (Func<Item, bool>)((Item x) => x.blueprintTarget == sellOrder.currencyID)));
		}
		list2 = Enumerable.ToList<Item>(Enumerable.Where<Item>((IEnumerable<Item>)list2, (Func<Item, bool>)((Item x) => !x.hasCondition || (x.conditionNormalized >= 0.5f && x.maxConditionNormalized > 0.5f))));
		if (list2.Count == 0)
		{
			Pool.FreeList<Item>(ref list);
			return false;
		}
		int num3 = Enumerable.Sum<Item>((IEnumerable<Item>)list2, (Func<Item, int>)((Item x) => x.amount));
		int num4 = sellOrder.currencyAmountPerItem * numberOfTransactions;
		if (num3 < num4)
		{
			Pool.FreeList<Item>(ref list);
			return false;
		}
		transactionActive = true;
		int num5 = 0;
		foreach (Item item3 in list2)
		{
			int num6 = Mathf.Min(num4 - num5, item3.amount);
			Item item = ((item3.amount > num6) ? item3.SplitItem(num6) : item3);
			TakeCurrencyItem(item);
			onCurrencyRemoved?.Invoke(buyer, item);
			num5 += num6;
			if (num5 >= num4)
			{
				break;
			}
		}
		int num7 = 0;
		foreach (Item item4 in list)
		{
			int num8 = num - num7;
			Item item2 = ((item4.amount > num8) ? item4.SplitItem(num8) : item4);
			if (item2 == null)
			{
				Debug.LogError((object)"Vending machine error, contact developers!");
			}
			else
			{
				num7 += item2.amount;
				if (targetContainer == null)
				{
					GiveSoldItem(item2, buyer);
				}
				else if (!item2.MoveToContainer(targetContainer))
				{
					item2.Drop(targetContainer.dropPosition, targetContainer.dropVelocity);
				}
				onItemPurchased?.Invoke(buyer, item2);
			}
			if (num7 >= num)
			{
				break;
			}
		}
		Pool.FreeList<Item>(ref list);
		UpdateEmptyFlag();
		transactionActive = false;
		return true;
	}

	public virtual void TakeCurrencyItem(Item takenCurrencyItem)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!takenCurrencyItem.MoveToContainer(base.inventory))
		{
			takenCurrencyItem.Drop(base.inventory.dropPosition, Vector3.get_zero());
		}
	}

	public virtual void GiveSoldItem(Item soldItem, BasePlayer buyer)
	{
		buyer.GiveItem(soldItem, GiveItemReason.PickedUp);
	}

	public void SendSellOrders(BasePlayer player = null)
	{
		if (Object.op_Implicit((Object)(object)player))
		{
			ClientRPCPlayer<SellOrderContainer>(null, player, "CLIENT_ReceiveSellOrders", sellOrders);
		}
		else
		{
			ClientRPC<SellOrderContainer>(null, "CLIENT_ReceiveSellOrders", sellOrders);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Broadcast(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		bool b = msg.read.Bit();
		if (CanPlayerAdmin(player))
		{
			SetFlag(Flags.Reserved4, b);
			UpdateMapMarker();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_UpdateShopName(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		string text = msg.read.String(32);
		if (CanPlayerAdmin(player))
		{
			shopName = text;
			UpdateMapMarker();
		}
	}

	public void UpdateMapMarker()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (IsBroadcasting())
		{
			bool flag = false;
			if ((Object)(object)myMarker == (Object)null)
			{
				myMarker = GameManager.server.CreateEntity(mapMarkerPrefab.resourcePath, ((Component)this).get_transform().get_position(), Quaternion.get_identity()) as VendingMachineMapMarker;
				flag = true;
			}
			myMarker.SetFlag(Flags.Busy, OutOfStock());
			myMarker.markerShopName = shopName;
			myMarker.server_vendingMachine = this;
			if (flag)
			{
				myMarker.Spawn();
			}
			else
			{
				myMarker.SendNetworkUpdate();
			}
		}
		else if (Object.op_Implicit((Object)(object)myMarker))
		{
			myMarker.Kill();
			myMarker = null;
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_OpenShop(RPCMessage msg)
	{
		if (OccupiedCheck(msg.player))
		{
			SendSellOrders(msg.player);
			PlayerOpenLoot(msg.player, customerPanel);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_OpenAdmin(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanPlayerAdmin(player))
		{
			SendSellOrders(player);
			PlayerOpenLoot(player);
			ClientRPCPlayer(null, player, "CLIENT_OpenAdminMenu");
		}
	}

	public bool CanAcceptItem(Item item, int targetSlot)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (transactionActive)
		{
			return true;
		}
		if (item.parent == null)
		{
			return true;
		}
		if (base.inventory.itemList.Contains(item))
		{
			return true;
		}
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			return false;
		}
		return CanPlayerAdmin(ownerPlayer);
	}

	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		return CanPlayerAdmin(player);
	}

	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (panelName == customerPanel)
		{
			return true;
		}
		if (base.CanOpenLootPanel(player, panelName))
		{
			return CanPlayerAdmin(player);
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_DeleteSellOrder(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanPlayerAdmin(player))
		{
			int num = msg.read.Int32();
			if (num >= 0 && num < sellOrders.sellOrders.Count)
			{
				sellOrders.sellOrders.RemoveAt(num);
			}
			RefreshSellOrderStockLevel();
			UpdateMapMarker();
			SendSellOrders(player);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RotateVM(RPCMessage msg)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (CanRotate())
		{
			UpdateEmptyFlag();
			if (msg.player.CanBuild() && IsInventoryEmpty())
			{
				((Component)this).get_transform().set_rotation(Quaternion.LookRotation(-((Component)this).get_transform().get_forward(), ((Component)this).get_transform().get_up()));
				SendNetworkUpdate();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_AddSellOrder(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanPlayerAdmin(player))
		{
			if (sellOrders.sellOrders.Count >= 7)
			{
				player.ChatMessage("Too many sell orders - remove some");
				return;
			}
			int itemToSellID = msg.read.Int32();
			int itemToSellAmount = msg.read.Int32();
			int currencyToUseID = msg.read.Int32();
			int currencyAmount = msg.read.Int32();
			byte bpState = msg.read.UInt8();
			AddSellOrder(itemToSellID, itemToSellAmount, currencyToUseID, currencyAmount, bpState);
		}
	}

	public void AddSellOrder(int itemToSellID, int itemToSellAmount, int currencyToUseID, int currencyAmount, byte bpState)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemToSellID);
		ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(currencyToUseID);
		if (!((Object)(object)itemDefinition == (Object)null) && !((Object)(object)itemDefinition2 == (Object)null))
		{
			currencyAmount = Mathf.Clamp(currencyAmount, 1, 10000);
			itemToSellAmount = Mathf.Clamp(itemToSellAmount, 1, itemDefinition.stackable);
			SellOrder val = new SellOrder();
			val.ShouldPool = false;
			val.itemToSellID = itemToSellID;
			val.itemToSellAmount = itemToSellAmount;
			val.currencyID = currencyToUseID;
			val.currencyAmountPerItem = currencyAmount;
			val.currencyIsBP = bpState == 3 || bpState == 2;
			val.itemToSellIsBP = bpState == 3 || bpState == 1;
			sellOrders.sellOrders.Add(val);
			RefreshSellOrderStockLevel(itemDefinition);
			UpdateMapMarker();
			SendNetworkUpdate();
		}
	}

	public void RefreshAndSendNetworkUpdate()
	{
		RefreshSellOrderStockLevel();
		SendNetworkUpdate();
	}

	public void UpdateOrCreateSalesSheet()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("note");
		List<Item> list = base.inventory.FindItemsByItemID(itemDefinition.itemid);
		Item item = null;
		foreach (Item item4 in list)
		{
			if (item4.text.Length == 0)
			{
				item = item4;
				break;
			}
		}
		if (item == null)
		{
			ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition("paper");
			Item item2 = base.inventory.FindItemByItemID(itemDefinition2.itemid);
			if (item2 != null)
			{
				item = ItemManager.CreateByItemID(itemDefinition.itemid, 1, 0uL);
				if (!item.MoveToContainer(base.inventory))
				{
					item.Drop(GetDropPosition(), GetDropVelocity());
				}
				item2.UseItem();
			}
		}
		if (item == null)
		{
			return;
		}
		foreach (SellOrder sellOrder in sellOrders.sellOrders)
		{
			ItemDefinition itemDefinition3 = ItemManager.FindItemDefinition(sellOrder.itemToSellID);
			Item item3 = item;
			item3.text = item3.text + itemDefinition3.displayName.get_translated() + "\n";
		}
		item.MarkDirty();
	}

	protected virtual bool CanRotate()
	{
		return true;
	}

	public bool IsBroadcasting()
	{
		return HasFlag(Flags.Reserved4);
	}

	public bool IsInventoryEmpty()
	{
		return HasFlag(Flags.Reserved1);
	}

	public bool IsVending()
	{
		return HasFlag(Flags.Reserved2);
	}

	public bool PlayerBehind(BasePlayer player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)this).get_transform().get_forward();
		Vector3 val = ((Component)player).get_transform().get_position() - ((Component)this).get_transform().get_position();
		return Vector3.Dot(forward, ((Vector3)(ref val)).get_normalized()) <= -0.7f;
	}

	public bool PlayerInfront(BasePlayer player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)this).get_transform().get_forward();
		Vector3 val = ((Component)player).get_transform().get_position() - ((Component)this).get_transform().get_position();
		return Vector3.Dot(forward, ((Vector3)(ref val)).get_normalized()) >= 0.7f;
	}

	public virtual bool CanPlayerAdmin(BasePlayer player)
	{
		if (PlayerBehind(player))
		{
			return OccupiedCheck(player);
		}
		return false;
	}
}
