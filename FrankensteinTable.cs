using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class FrankensteinTable : StorageContainer
{
	public GameObjectRef FrankensteinPrefab;

	public Transform SpawnLocation;

	public ItemDefinition WeaponItem;

	public List<ItemDefinition> HeadItems;

	public List<ItemDefinition> TorsoItems;

	public List<ItemDefinition> LegItems;

	[HideInInspector]
	public List<ItemDefinition> ItemsToUse;

	public FrankensteinTableVisuals TableVisuals;

	[Header("Timings")]
	public float TableDownDuration = 0.9f;

	private bool waking;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FrankensteinTable.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 629197370 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - CreateFrankenstein "));
				}
				TimeWarning val2 = TimeWarning.New("CreateFrankenstein", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(629197370u, "CreateFrankenstein", this, player, 3f))
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
							CreateFrankenstein(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in CreateFrankenstein");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4797457 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestSleepFrankenstein "));
				}
				TimeWarning val2 = TimeWarning.New("RequestSleepFrankenstein", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4797457u, "RequestSleepFrankenstein", this, player, 3f))
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
							RequestSleepFrankenstein(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RequestSleepFrankenstein");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3804893505u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestWakeFrankenstein "));
				}
				TimeWarning val2 = TimeWarning.New("RequestWakeFrankenstein", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3804893505u, "RequestWakeFrankenstein", this, player, 3f))
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
							RequestWakeFrankenstein(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RequestWakeFrankenstein");
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

	public bool IsHeadItem(ItemDefinition itemDef)
	{
		return HeadItems.Contains(itemDef);
	}

	public bool IsTorsoItem(ItemDefinition itemDef)
	{
		return TorsoItems.Contains(itemDef);
	}

	public bool IsLegsItem(ItemDefinition itemDef)
	{
		return LegItems.Contains(itemDef);
	}

	public bool HasValidItems(ItemContainer container)
	{
		return GetValidItems(container) != null;
	}

	public List<ItemDefinition> GetValidItems(ItemContainer container)
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
		bool set = false;
		bool set2 = false;
		bool set3 = false;
		List<ItemDefinition> list = new List<ItemDefinition>();
		for (int i = 0; i < container.capacity; i++)
		{
			Item slot = container.GetSlot(i);
			if (slot != null)
			{
				CheckItem(slot.info, list, HeadItems, ref set);
				CheckItem(slot.info, list, TorsoItems, ref set2);
				CheckItem(slot.info, list, LegItems, ref set3);
				if (set && set2 && set3)
				{
					return list;
				}
			}
		}
		return null;
	}

	public bool HasAllValidItems(List<ItemDefinition> items)
	{
		if (items == null)
		{
			return false;
		}
		if (items.Count < 3)
		{
			return false;
		}
		bool set = false;
		bool set2 = false;
		bool set3 = false;
		foreach (ItemDefinition item in items)
		{
			if ((Object)(object)item == (Object)null)
			{
				return false;
			}
			CheckItem(item, null, HeadItems, ref set);
			CheckItem(item, null, TorsoItems, ref set2);
			CheckItem(item, null, LegItems, ref set3);
		}
		return set && set2 && set3;
	}

	private void CheckItem(ItemDefinition item, List<ItemDefinition> itemList, List<ItemDefinition> validItems, ref bool set)
	{
		if (!set && validItems.Contains(item))
		{
			set = true;
			itemList?.Add(item);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
		base.inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		SendNetworkUpdateImmediate();
	}

	private bool CanAcceptItem(Item item, int targetSlot)
	{
		if (item == null)
		{
			return false;
		}
		if (HeadItems != null && IsHeadItem(item.info))
		{
			return true;
		}
		if (TorsoItems != null && IsTorsoItem(item.info))
		{
			return true;
		}
		if (LegItems != null && IsLegsItem(item.info))
		{
			return true;
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void CreateFrankenstein(RPCMessage msg)
	{
	}

	private bool CanStartCreating(BasePlayer player)
	{
		if (waking)
		{
			return false;
		}
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if ((Object)(object)player.PetEntity != (Object)null)
		{
			return false;
		}
		if (!HasValidItems(base.inventory))
		{
			return false;
		}
		return true;
	}

	private bool IsInventoryEmpty()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			if (base.inventory.GetSlot(i) != null)
			{
				return false;
			}
		}
		return true;
	}

	private void ConsumeInventory()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			slot?.UseItem(slot.amount);
		}
		ItemManager.DoRemoves();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RequestWakeFrankenstein(RPCMessage msg)
	{
		WakeFrankenstein(msg.player);
	}

	private void WakeFrankenstein(BasePlayer owner)
	{
		if (!((Object)(object)owner == (Object)null) && CanStartCreating(owner))
		{
			waking = true;
			base.inventory.SetLocked(isLocked: true);
			SendNetworkUpdateImmediate();
			((MonoBehaviour)this).StartCoroutine(DelayWakeFrankenstein(owner));
			ClientRPC(null, "CL_WakeFrankenstein");
		}
	}

	private IEnumerator DelayWakeFrankenstein(BasePlayer owner)
	{
		yield return (object)new WaitForSeconds(1.5f);
		yield return (object)new WaitForSeconds(TableDownDuration);
		if ((Object)(object)owner != (Object)null && (Object)(object)owner.PetEntity != (Object)null)
		{
			base.inventory.SetLocked(isLocked: false);
			SendNetworkUpdateImmediate();
			waking = false;
			yield break;
		}
		ItemsToUse = GetValidItems(base.inventory);
		BaseEntity baseEntity = GameManager.server.CreateEntity(FrankensteinPrefab.resourcePath, SpawnLocation.get_position(), SpawnLocation.get_rotation(), startActive: false);
		baseEntity.enableSaving = false;
		((Component)baseEntity).get_gameObject().AwakeFromInstantiate();
		baseEntity.Spawn();
		EquipFrankenstein(baseEntity as FrankensteinPet);
		ConsumeInventory();
		base.inventory.SetLocked(isLocked: false);
		SendNetworkUpdateImmediate();
		((MonoBehaviour)this).StartCoroutine(WaitForFrankensteinBrainInit(baseEntity as BasePet, owner));
		waking = false;
		yield return null;
	}

	private void EquipFrankenstein(FrankensteinPet frank)
	{
		if (ItemsToUse == null || (Object)(object)frank == (Object)null || (Object)(object)frank.inventory == (Object)null)
		{
			return;
		}
		foreach (ItemDefinition item in ItemsToUse)
		{
			frank.inventory.GiveItem(ItemManager.Create(item, 1, 0uL), frank.inventory.containerWear);
		}
		if ((Object)(object)WeaponItem != (Object)null)
		{
			((MonoBehaviour)this).StartCoroutine(frank.DelayEquipWeapon(WeaponItem, 1.5f));
		}
		ItemsToUse.Clear();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RequestSleepFrankenstein(RPCMessage msg)
	{
		SleepFrankenstein(msg.player);
	}

	private void SleepFrankenstein(BasePlayer owner)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (IsInventoryEmpty() && !((Object)(object)owner == (Object)null) && !((Object)(object)owner.PetEntity == (Object)null))
		{
			FrankensteinPet frankensteinPet = owner.PetEntity as FrankensteinPet;
			if (!((Object)(object)frankensteinPet == (Object)null) && !(Vector3.Distance(((Component)this).get_transform().get_position(), ((Component)frankensteinPet).get_transform().get_position()) >= 5f))
			{
				ReturnFrankensteinItems(frankensteinPet);
				ItemManager.DoRemoves();
				SendNetworkUpdateImmediate();
				frankensteinPet.Kill();
			}
		}
	}

	private void ReturnFrankensteinItems(FrankensteinPet frank)
	{
		if (!((Object)(object)frank == (Object)null) && !((Object)(object)frank.inventory == (Object)null) && frank.inventory.containerWear != null)
		{
			for (int i = 0; i < frank.inventory.containerWear.capacity; i++)
			{
				frank.inventory.containerWear.GetSlot(i)?.MoveToContainer(base.inventory);
			}
		}
	}

	private IEnumerator WaitForFrankensteinBrainInit(BasePet frankenstein, BasePlayer player)
	{
		yield return (object)new WaitForEndOfFrame();
		frankenstein.ApplyPetStatModifiers();
		frankenstein.Brain.SetOwningPlayer(player);
		frankenstein.CreateMapMarker();
		player.SendClientPetLink();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			return;
		}
		info.msg.FrankensteinTable = Pool.Get<FrankensteinTable>();
		info.msg.FrankensteinTable.itemIds = new List<int>();
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				info.msg.FrankensteinTable.itemIds.Add(slot.info.itemid);
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}
}
