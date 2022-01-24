using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class RepairBench : StorageContainer
{
	public float maxConditionLostOnRepair = 0.2f;

	public GameObjectRef skinchangeEffect;

	public const float REPAIR_COST_FRACTION = 0.2f;

	private float nextSkinChangeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RepairBench.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 1942825351 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ChangeSkin "));
				}
				TimeWarning val2 = TimeWarning.New("ChangeSkin", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1942825351u, "ChangeSkin", this, player, 3f))
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
							ChangeSkin(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ChangeSkin");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1178348163 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RepairItem "));
				}
				TimeWarning val2 = TimeWarning.New("RepairItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1178348163u, "RepairItem", this, player, 3f))
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
							RepairItem(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RepairItem");
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

	public static float GetRepairFraction(Item itemToRepair)
	{
		return 1f - itemToRepair.condition / itemToRepair.maxCondition;
	}

	public static float RepairCostFraction(Item itemToRepair)
	{
		return GetRepairFraction(itemToRepair) * 0.2f;
	}

	public static void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
	{
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			allIngredients.Add(new ItemAmount(ingredient.itemDef, ingredient.amount));
		}
		foreach (ItemAmount ingredient2 in bp.ingredients)
		{
			if (ingredient2.itemDef.category != ItemCategory.Component || !((Object)(object)ingredient2.itemDef.Blueprint != (Object)null))
			{
				continue;
			}
			bool flag = false;
			ItemAmount itemAmount = ingredient2.itemDef.Blueprint.ingredients[0];
			foreach (ItemAmount allIngredient in allIngredients)
			{
				if ((Object)(object)allIngredient.itemDef == (Object)(object)itemAmount.itemDef)
				{
					allIngredient.amount += itemAmount.amount * ingredient2.amount;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				allIngredients.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount * ingredient2.amount));
			}
		}
	}

	public void debugprint(string toPrint)
	{
		if (Global.developer > 0)
		{
			Debug.LogWarning((object)toPrint);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ChangeSkin(RPCMessage msg)
	{
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_realtimeSinceStartup() < nextSkinChangeTime)
		{
			return;
		}
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		bool flag = false;
		if (num != 0 && !flag && !player.blueprints.CheckSkinOwnership(num, player.userID))
		{
			debugprint("RepairBench.ChangeSkin player does not have item :" + num + ":");
			return;
		}
		ulong Skin = ItemDefinition.FindSkin(slot.info.itemid, num);
		if (Skin == slot.skin && (Object)(object)slot.info.isRedirectOf == (Object)null)
		{
			debugprint("RepairBench.ChangeSkin cannot apply same skin twice : " + Skin + ": " + slot.skin);
			return;
		}
		nextSkinChangeTime = Time.get_realtimeSinceStartup() + 0.75f;
		ItemSkinDirectory.Skin skin = Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>((IEnumerable<ItemSkinDirectory.Skin>)slot.info.skins, (Func<ItemSkinDirectory.Skin, bool>)((ItemSkinDirectory.Skin x) => (ulong)x.id == Skin));
		if ((Object)(object)slot.info.isRedirectOf != (Object)null)
		{
			Skin = ItemDefinition.FindSkin(slot.info.isRedirectOf.itemid, num);
			skin = Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>((IEnumerable<ItemSkinDirectory.Skin>)slot.info.isRedirectOf.skins, (Func<ItemSkinDirectory.Skin, bool>)((ItemSkinDirectory.Skin x) => (ulong)x.id == Skin));
		}
		ItemSkin itemSkin = ((skin.id == 0) ? null : (skin.invItem as ItemSkin));
		if (Object.op_Implicit((Object)(object)itemSkin) && ((Object)(object)itemSkin.Redirect != (Object)null || (Object)(object)slot.info.isRedirectOf != (Object)null))
		{
			ItemDefinition template = itemSkin.Redirect;
			bool flag2 = false;
			if ((Object)(object)itemSkin.Redirect == (Object)null && (Object)(object)slot.info.isRedirectOf != (Object)null)
			{
				template = slot.info.isRedirectOf;
				flag2 = num != 0;
			}
			float condition = slot.condition;
			float maxCondition = slot.maxCondition;
			int amount = slot.amount;
			slot.Remove();
			ItemManager.DoRemoves();
			Item item = ItemManager.Create(template, 1, 0uL);
			item.MoveToContainer(base.inventory, 0, allowStack: false);
			item.maxCondition = maxCondition;
			item.condition = condition;
			item.amount = amount;
			if (flag2)
			{
				ApplySkinToItem(item, Skin);
			}
		}
		else if (!Object.op_Implicit((Object)(object)itemSkin) && (Object)(object)slot.info.isRedirectOf != (Object)null)
		{
			ItemDefinition isRedirectOf = slot.info.isRedirectOf;
			float condition2 = slot.condition;
			float maxCondition2 = slot.maxCondition;
			slot.Remove();
			ItemManager.DoRemoves();
			Item item2 = ItemManager.Create(isRedirectOf, 1, Skin);
			item2.MoveToContainer(base.inventory, 0, allowStack: false);
			item2.maxCondition = maxCondition2;
			item2.condition = condition2;
		}
		else
		{
			ApplySkinToItem(slot, Skin);
		}
		if (skinchangeEffect.isValid)
		{
			Effect.server.Run(skinchangeEffect.resourcePath, this, 0u, new Vector3(0f, 1.5f, 0f), Vector3.get_zero());
		}
	}

	private void ApplySkinToItem(Item item, ulong Skin)
	{
		item.skin = Skin;
		item.MarkDirty();
		BaseEntity heldEntity = item.GetHeldEntity();
		if ((Object)(object)heldEntity != (Object)null)
		{
			heldEntity.skinID = Skin;
			heldEntity.SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RepairItem(RPCMessage msg)
	{
		Item slot = base.inventory.GetSlot(0);
		BasePlayer player = msg.player;
		RepairAnItem(slot, player, this, maxConditionLostOnRepair, mustKnowBlueprint: true);
	}

	public static void RepairAnItem(Item itemToRepair, BasePlayer player, BaseEntity repairBenchEntity, float maxConditionLostOnRepair, bool mustKnowBlueprint)
	{
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		if (itemToRepair == null)
		{
			return;
		}
		ItemDefinition info = itemToRepair.info;
		ItemBlueprint component = ((Component)info).GetComponent<ItemBlueprint>();
		if (!Object.op_Implicit((Object)(object)component) || !info.condition.repairable || itemToRepair.condition == itemToRepair.maxCondition)
		{
			return;
		}
		if (mustKnowBlueprint)
		{
			ItemDefinition itemDefinition = (((Object)(object)info.isRedirectOf != (Object)null) ? info.isRedirectOf : info);
			if (!player.blueprints.HasUnlocked(itemDefinition) && (!((Object)(object)itemDefinition.Blueprint != (Object)null) || itemDefinition.Blueprint.isResearchable))
			{
				return;
			}
		}
		float num = RepairCostFraction(itemToRepair);
		bool flag = false;
		List<ItemAmount> list = Pool.GetList<ItemAmount>();
		GetRepairCostList(component, list);
		foreach (ItemAmount item in list)
		{
			if (item.itemDef.category != ItemCategory.Component)
			{
				int amount = player.inventory.GetAmount(item.itemDef.itemid);
				if (Mathf.CeilToInt(item.amount * num) > amount)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Pool.FreeList<ItemAmount>(ref list);
			return;
		}
		foreach (ItemAmount item2 in list)
		{
			if (item2.itemDef.category != ItemCategory.Component)
			{
				int amount2 = Mathf.CeilToInt(item2.amount * num);
				player.inventory.Take(null, item2.itemid, amount2);
			}
		}
		Pool.FreeList<ItemAmount>(ref list);
		itemToRepair.DoRepair(maxConditionLostOnRepair);
		if (Global.developer > 0)
		{
			Debug.Log((object)("Item repaired! condition : " + itemToRepair.condition + "/" + itemToRepair.maxCondition));
		}
		Effect.server.Run("assets/bundled/prefabs/fx/repairbench/itemrepair.prefab", repairBenchEntity, 0u, Vector3.get_zero(), Vector3.get_zero());
	}
}
