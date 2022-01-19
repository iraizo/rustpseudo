using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class MixingTable : StorageContainer
{
	public GameObject Particles;

	public RecipeList Recipes;

	public bool OnlyAcceptValidIngredients;

	private float lastTickTimestamp;

	private List<Item> inventoryItems = new List<Item>();

	private const float mixTickInterval = 1f;

	private Recipe currentRecipe;

	private int currentQuantity;

	protected ItemDefinition currentProductionItem;

	public float RemainingMixTime { get; private set; }

	public float TotalMixTime { get; private set; }

	public BasePlayer MixStartingPlayer { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MixingTable.OnRpcMessage", 0);
		try
		{
			if (rpc == 4167839872u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SVSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("SVSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4167839872u, "SVSwitch", this, player, 3f))
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
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							SVSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
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

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
		base.inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
		RecipeDictionary.CacheRecipes(Recipes);
	}

	private bool CanAcceptItem(Item item, int targetSlot)
	{
		if (item == null)
		{
			return false;
		}
		if (!OnlyAcceptValidIngredients)
		{
			return true;
		}
		if (GetItemWaterAmount(item) > 0)
		{
			item = item.contents.itemList[0];
		}
		if (!((Object)(object)item.info == (Object)(object)currentProductionItem))
		{
			return RecipeDictionary.ValidIngredientForARecipe(item, Recipes);
		}
		return true;
	}

	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		if (IsOn())
		{
			StopMixing();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void SVSwitch(RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag != IsOn() && !((Object)(object)msg.player == (Object)null))
		{
			if (flag)
			{
				StartMixing(msg.player);
			}
			else
			{
				StopMixing();
			}
		}
	}

	private void StartMixing(BasePlayer player)
	{
		if (IsOn() || !CanStartMixing(player))
		{
			return;
		}
		MixStartingPlayer = player;
		List<Item> orderedContainerItems = GetOrderedContainerItems(base.inventory);
		currentRecipe = RecipeDictionary.GetMatchingRecipeAndQuantity(Recipes, orderedContainerItems, out var quantity);
		currentQuantity = quantity;
		if (!((Object)(object)currentRecipe == (Object)null) && (!currentRecipe.RequiresBlueprint || !((Object)(object)currentRecipe.ProducedItem != (Object)null) || player.blueprints.HasUnlocked(currentRecipe.ProducedItem)))
		{
			if (base.isServer)
			{
				lastTickTimestamp = Time.get_realtimeSinceStartup();
			}
			RemainingMixTime = currentRecipe.MixingDuration * (float)currentQuantity;
			TotalMixTime = RemainingMixTime;
			ReturnExcessItems(orderedContainerItems, player);
			if (RemainingMixTime == 0f)
			{
				ProduceItem(currentRecipe, currentQuantity);
				return;
			}
			((FacepunchBehaviour)this).InvokeRepeating((Action)TickMix, 1f, 1f);
			SetFlag(Flags.On, b: true);
			SendNetworkUpdateImmediate();
		}
	}

	protected virtual bool CanStartMixing(BasePlayer player)
	{
		return true;
	}

	public void StopMixing()
	{
		currentRecipe = null;
		currentQuantity = 0;
		RemainingMixTime = 0f;
		((FacepunchBehaviour)this).CancelInvoke((Action)TickMix);
		if (IsOn())
		{
			SetFlag(Flags.On, b: false);
			SendNetworkUpdateImmediate();
		}
	}

	private void TickMix()
	{
		if ((Object)(object)currentRecipe == (Object)null)
		{
			StopMixing();
			return;
		}
		if (base.isServer)
		{
			lastTickTimestamp = Time.get_realtimeSinceStartup();
			RemainingMixTime -= 1f;
		}
		SendNetworkUpdateImmediate();
		if (RemainingMixTime <= 0f)
		{
			ProduceItem(currentRecipe, currentQuantity);
		}
	}

	private void ProduceItem(Recipe recipe, int quantity)
	{
		StopMixing();
		ConsumeInventory(recipe, quantity);
		CreateRecipeItems(recipe, quantity);
	}

	private void ConsumeInventory(Recipe recipe, int quantity)
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			Item item = base.inventory.GetSlot(i);
			if (item != null)
			{
				if (GetItemWaterAmount(item) > 0)
				{
					item = item.contents.itemList[0];
				}
				int num = recipe.Ingredients[i].Count * quantity;
				if (num > 0)
				{
					item.UseItem(num);
				}
			}
		}
		ItemManager.DoRemoves();
	}

	private void ReturnExcessItems(List<Item> orderedContainerItems, BasePlayer player)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null || (Object)(object)currentRecipe == (Object)null || orderedContainerItems == null || orderedContainerItems.Count != currentRecipe.Ingredients.Length)
		{
			return;
		}
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot == null)
			{
				continue;
			}
			int num = slot.amount - currentRecipe.Ingredients[i].Count * currentQuantity;
			if (num > 0)
			{
				Item item = slot.SplitItem(num);
				if (!item.MoveToContainer(player.inventory.containerMain) && !item.MoveToContainer(player.inventory.containerBelt))
				{
					item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity);
				}
			}
		}
		ItemManager.DoRemoves();
	}

	protected virtual void CreateRecipeItems(Recipe recipe, int quantity)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)recipe == (Object)null || (Object)(object)recipe.ProducedItem == (Object)null)
		{
			return;
		}
		int num = quantity * recipe.ProducedItemCount;
		int stackable = recipe.ProducedItem.stackable;
		int num2 = Mathf.CeilToInt((float)num / (float)stackable);
		currentProductionItem = recipe.ProducedItem;
		for (int i = 0; i < num2; i++)
		{
			int num3 = ((num > stackable) ? stackable : num);
			Item item = ItemManager.Create(recipe.ProducedItem, num3, 0uL);
			if (!item.MoveToContainer(base.inventory))
			{
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity);
			}
			num -= num3;
			if (num <= 0)
			{
				break;
			}
		}
		currentProductionItem = null;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.mixingTable = Pool.Get<MixingTable>();
		if (info.forDisk)
		{
			info.msg.mixingTable.remainingMixTime = RemainingMixTime;
		}
		else
		{
			info.msg.mixingTable.remainingMixTime = RemainingMixTime - Mathf.Max(Time.get_realtimeSinceStartup() - lastTickTimestamp, 0f);
		}
		info.msg.mixingTable.totalMixTime = TotalMixTime;
	}

	private int GetItemWaterAmount(Item item)
	{
		if (item == null)
		{
			return 0;
		}
		if (item.contents != null && item.contents.capacity == 1 && item.contents.allowedContents == ItemContainer.ContentsType.Liquid && item.contents.itemList.Count > 0)
		{
			return item.contents.itemList[0].amount;
		}
		return 0;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mixingTable != null)
		{
			RemainingMixTime = info.msg.mixingTable.remainingMixTime;
			TotalMixTime = info.msg.mixingTable.totalMixTime;
		}
	}

	public List<Item> GetOrderedContainerItems(ItemContainer container)
	{
		if (container == null)
		{
			return null;
		}
		if (container.itemList == null)
		{
			return null;
		}
		if (container.itemList.Count == 0)
		{
			return null;
		}
		inventoryItems.Clear();
		for (int i = 0; i < container.capacity; i++)
		{
			Item item = container.GetSlot(i);
			if (item == null)
			{
				break;
			}
			if (GetItemWaterAmount(item) > 0)
			{
				item = item.contents.itemList[0];
			}
			inventoryItems.Add(item);
		}
		return inventoryItems;
	}
}
