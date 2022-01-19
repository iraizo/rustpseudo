using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class Recycler : StorageContainer
{
	public float recycleEfficiency = 0.5f;

	public SoundDefinition grindingLoopDef;

	public GameObjectRef startSound;

	public GameObjectRef stopSound;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Recycler.OnRpcMessage", 0);
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

	public override void ResetState()
	{
		base.ResetState();
	}

	private bool CanBeRecycled(Item item)
	{
		if (item != null)
		{
			return (Object)(object)item.info.Blueprint != (Object)null;
		}
		return false;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(RecyclerItemFilter));
	}

	public bool RecyclerItemFilter(Item item, int targetSlot)
	{
		int num = Mathf.CeilToInt((float)base.inventory.capacity * 0.5f);
		if (targetSlot == -1)
		{
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				if (!base.inventory.SlotTaken(item, i))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (targetSlot < num)
		{
			return CanBeRecycled(item);
		}
		return true;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void SVSwitch(RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == IsOn() || (Object)(object)msg.player == (Object)null || (flag && !HasRecyclable()))
		{
			return;
		}
		if (flag)
		{
			foreach (Item item in base.inventory.itemList)
			{
				item.CollectedForCrafting(msg.player);
			}
			StartRecycling();
		}
		else
		{
			StopRecycling();
		}
	}

	public bool MoveItemToOutput(Item newItem)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		for (int i = 6; i < 12; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot == null)
			{
				num = i;
				break;
			}
			if (slot.CanStack(newItem))
			{
				if (slot.amount + newItem.amount <= slot.info.stackable)
				{
					num = i;
					break;
				}
				int num2 = Mathf.Min(slot.info.stackable - slot.amount, newItem.amount);
				newItem.UseItem(num2);
				slot.amount += num2;
				slot.MarkDirty();
				newItem.MarkDirty();
			}
			if (newItem.amount <= 0)
			{
				return true;
			}
		}
		if (num != -1 && newItem.MoveToContainer(base.inventory, num))
		{
			return true;
		}
		newItem.Drop(((Component)this).get_transform().get_position() + new Vector3(0f, 2f, 0f), GetInheritedDropVelocity() + ((Component)this).get_transform().get_forward() * 2f);
		return false;
	}

	public bool HasRecyclable()
	{
		for (int i = 0; i < 6; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null && (Object)(object)slot.info.Blueprint != (Object)null)
			{
				return true;
			}
		}
		return false;
	}

	public void RecycleThink()
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		float num = recycleEfficiency;
		for (int i = 0; i < 6; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (!CanBeRecycled(slot))
			{
				continue;
			}
			if (slot.hasCondition)
			{
				num = Mathf.Clamp01(num * Mathf.Clamp(slot.conditionNormalized * slot.maxConditionNormalized, 0.1f, 1f));
			}
			int num2 = 1;
			if (slot.amount > 1)
			{
				num2 = Mathf.CeilToInt(Mathf.Min((float)slot.amount, (float)slot.info.stackable * 0.1f));
			}
			if (slot.info.Blueprint.scrapFromRecycle > 0)
			{
				int num3 = slot.info.Blueprint.scrapFromRecycle * num2;
				if (slot.info.stackable == 1 && slot.hasCondition)
				{
					num3 = Mathf.CeilToInt((float)num3 * slot.conditionNormalized);
				}
				if (num3 >= 1)
				{
					Item newItem = ItemManager.CreateByName("scrap", num3, 0uL);
					MoveItemToOutput(newItem);
				}
			}
			if (!string.IsNullOrEmpty(slot.info.Blueprint.RecycleStat))
			{
				List<BasePlayer> list = Pool.GetList<BasePlayer>();
				Vis.Entities(((Component)this).get_transform().get_position(), 3f, list, 131072, (QueryTriggerInteraction)2);
				foreach (BasePlayer item in list)
				{
					if (item.IsAlive() && !item.IsSleeping() && (Object)(object)item.inventory.loot.entitySource == (Object)(object)this)
					{
						item.stats.Add(slot.info.Blueprint.RecycleStat, num2, (Stats)5);
						item.stats.Save();
					}
				}
				Pool.FreeList<BasePlayer>(ref list);
			}
			slot.UseItem(num2);
			foreach (ItemAmount ingredient in slot.info.Blueprint.ingredients)
			{
				if (ingredient.itemDef.shortname == "scrap")
				{
					continue;
				}
				float num4 = ingredient.amount / (float)slot.info.Blueprint.amountToCreate;
				int num5 = 0;
				if (num4 <= 1f)
				{
					for (int j = 0; j < num2; j++)
					{
						if (Random.Range(0f, 1f) <= num4 * num)
						{
							num5++;
						}
					}
				}
				else
				{
					num5 = Mathf.CeilToInt(Mathf.Clamp(num4 * num * Random.Range(1f, 1f), 0f, ingredient.amount)) * num2;
				}
				if (num5 <= 0)
				{
					continue;
				}
				int num6 = Mathf.CeilToInt((float)num5 / (float)ingredient.itemDef.stackable);
				for (int k = 0; k < num6; k++)
				{
					int num7 = ((num5 > ingredient.itemDef.stackable) ? ingredient.itemDef.stackable : num5);
					Item newItem2 = ItemManager.Create(ingredient.itemDef, num7, 0uL);
					if (!MoveItemToOutput(newItem2))
					{
						flag = true;
					}
					num5 -= num7;
					if (num5 <= 0)
					{
						break;
					}
				}
			}
			break;
		}
		if (flag || !HasRecyclable())
		{
			StopRecycling();
		}
	}

	public void StartRecycling()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOn())
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)RecycleThink, 5f, 5f);
			Effect.server.Run(startSound.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
			SetFlag(Flags.On, b: true);
			SendNetworkUpdateImmediate();
		}
	}

	public void StopRecycling()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		((FacepunchBehaviour)this).CancelInvoke((Action)RecycleThink);
		if (IsOn())
		{
			Effect.server.Run(stopSound.resourcePath, this, 0u, Vector3.get_zero(), Vector3.get_zero());
			SetFlag(Flags.On, b: false);
			SendNetworkUpdateImmediate();
		}
	}
}
