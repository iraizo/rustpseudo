using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

public class GameModeSoftcore : GameModeVanilla
{
	public GameObjectRef reclaimManagerPrefab;

	public GameObjectRef reclaimBackpackPrefab;

	public static readonly Phrase ReclaimToast = new Phrase("softcore.reclaim", "You can reclaim some of your lost items by visiting the Outpost or Bandit Town.");

	public ItemAmount[] startingGear;

	[ServerVar]
	public static float reclaim_fraction_belt = 0.5f;

	[ServerVar]
	public static float reclaim_fraction_wear = 0f;

	[ServerVar]
	public static float reclaim_fraction_main = 0.5f;

	protected override void OnCreated()
	{
		base.OnCreated();
		SingletonComponent<ServerMgr>.Instance.CreateImportantEntity<ReclaimManager>(reclaimManagerPrefab.resourcePath);
	}

	public void AddFractionOfContainer(ItemContainer from, ref List<Item> to, float fraction = 1f)
	{
		if (from.itemList.Count == 0)
		{
			return;
		}
		fraction = Mathf.Clamp01(fraction);
		float num = Mathf.Ceil((float)from.itemList.Count * fraction);
		List<int> list = Pool.GetList<int>();
		for (int i = 0; i < from.capacity; i++)
		{
			if (from.GetSlot(i) != null)
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			Pool.FreeList<int>(ref list);
			return;
		}
		for (int j = 0; (float)j < num; j++)
		{
			int index = Random.Range(0, list.Count);
			Item item = from.GetSlot(list[index]);
			if (item.info.stackable > 1)
			{
				foreach (Item item2 in from.itemList)
				{
					if (!((Object)(object)item2.info == (Object)(object)item.info) || item2.amount >= item.amount || to.Contains(item2))
					{
						continue;
					}
					item = item2;
					for (int k = 0; k < list.Count; k++)
					{
						if (list[k] == item2.position)
						{
							index = k;
						}
					}
				}
			}
			to.Add(item);
			list.RemoveAt(index);
		}
		Pool.FreeList<int>(ref list);
	}

	public List<Item> RemoveItemsFrom(ItemContainer itemContainer, ItemAmount[] types)
	{
		List<Item> list = Pool.GetList<Item>();
		foreach (ItemAmount itemAmount in types)
		{
			for (int j = 0; (float)j < itemAmount.amount; j++)
			{
				Item item = itemContainer.FindItemByItemID(itemAmount.itemDef.itemid);
				if (item != null)
				{
					item.RemoveFromContainer();
					list.Add(item);
				}
			}
		}
		return list;
	}

	public void ReturnItemsTo(ref List<Item> source, ItemContainer itemContainer)
	{
		foreach (Item item in source)
		{
			item.MoveToContainer(itemContainer);
		}
		Pool.FreeList<Item>(ref source);
	}

	public override void OnPlayerDeath(BasePlayer instigator, BasePlayer victim, HitInfo deathInfo = null)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)victim != (Object)null && !victim.IsNpc)
		{
			SetInventoryLocked(victim, wantsLocked: false);
			int num = 0;
			if ((Object)(object)ReclaimManager.instance == (Object)null)
			{
				Debug.LogError((object)"No reclaim manage for softcore");
				return;
			}
			List<Item> to = Pool.GetList<Item>();
			List<Item> source = RemoveItemsFrom(victim.inventory.containerBelt, startingGear);
			AddFractionOfContainer(victim.inventory.containerBelt, ref to, reclaim_fraction_belt);
			AddFractionOfContainer(victim.inventory.containerWear, ref to, reclaim_fraction_wear);
			AddFractionOfContainer(victim.inventory.containerMain, ref to, reclaim_fraction_main);
			num = ReclaimManager.instance.AddPlayerReclaim(victim.userID, to, ((Object)(object)instigator == (Object)null) ? 0 : instigator.userID, ((Object)(object)instigator == (Object)null) ? "" : instigator.displayName);
			ReturnItemsTo(ref source, victim.inventory.containerBelt);
			Pool.FreeList<Item>(ref to);
			Vector3 pos = ((Component)victim).get_transform().get_position() + Vector3.get_up() * 0.25f;
			Quaternion rot = Quaternion.Euler(0f, ((Component)victim).get_transform().get_eulerAngles().y, 0f);
			ReclaimBackpack component = ((Component)GameManager.server.CreateEntity(reclaimBackpackPrefab.resourcePath, pos, rot)).GetComponent<ReclaimBackpack>();
			component.InitForPlayer(victim.userID, num);
			component.Spawn();
		}
		base.OnPlayerDeath(instigator, victim, deathInfo);
	}

	public override void OnPlayerRespawn(BasePlayer player)
	{
		base.OnPlayerRespawn(player);
		player.ShowToast(2, ReclaimToast);
	}

	public override SleepingBag[] FindSleepingBagsForPlayer(ulong playerID, bool ignoreTimers)
	{
		return SleepingBag.FindForPlayer(playerID, ignoreTimers);
	}

	public override float CorpseRemovalTime(BaseCorpse corpse)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
		{
			if ((Object)(object)monument != (Object)null && monument.IsSafeZone && ((Bounds)(ref monument.Bounds)).Contains(((Component)corpse).get_transform().get_position()))
			{
				return 30f;
			}
		}
		return Server.corpsedespawn;
	}

	public void SetInventoryLocked(BasePlayer player, bool wantsLocked)
	{
		player.inventory.containerMain.SetLocked(wantsLocked);
		player.inventory.containerBelt.SetLocked(wantsLocked);
		player.inventory.containerWear.SetLocked(wantsLocked);
	}

	public override void OnPlayerWounded(BasePlayer instigator, BasePlayer victim, HitInfo info)
	{
		base.OnPlayerWounded(instigator, victim, info);
		SetInventoryLocked(victim, wantsLocked: true);
	}

	public override void OnPlayerRevived(BasePlayer instigator, BasePlayer victim)
	{
		SetInventoryLocked(victim, wantsLocked: false);
		base.OnPlayerRevived(instigator, victim);
	}

	public override bool CanMoveItemsFrom(PlayerInventory inv, BaseEntity source, Item item)
	{
		if (item.parent != null && item.parent.HasFlag(ItemContainer.Flag.IsPlayer))
		{
			return !item.parent.IsLocked();
		}
		return base.CanMoveItemsFrom(inv, source, item);
	}
}
