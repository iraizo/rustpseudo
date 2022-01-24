using Facepunch;
using ProtoBuf;
using UnityEngine;

public class PlayerBlueprints : EntityComponent<BasePlayer>
{
	public SteamInventory steamInventory;

	internal void Reset()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (persistantPlayerInfo.unlockedItems != null)
		{
			persistantPlayerInfo.unlockedItems.Clear();
		}
		else
		{
			persistantPlayerInfo.unlockedItems = Pool.GetList<int>();
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdate();
	}

	internal void UnlockAll()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		foreach (ItemBlueprint bp in ItemManager.bpList)
		{
			if (bp.userCraftable && !bp.defaultBlueprint && !persistantPlayerInfo.unlockedItems.Contains(bp.targetItem.itemid))
			{
				persistantPlayerInfo.unlockedItems.Add(bp.targetItem.itemid);
			}
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdateImmediate();
		base.baseEntity.ClientRPCPlayer(null, base.baseEntity, "UnlockedBlueprint", 0);
	}

	public bool IsUnlocked(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (persistantPlayerInfo.unlockedItems != null)
		{
			return persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid);
		}
		return false;
	}

	public void Unlock(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (!persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
		{
			persistantPlayerInfo.unlockedItems.Add(itemDef.itemid);
			base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
			base.baseEntity.SendNetworkUpdateImmediate();
			base.baseEntity.ClientRPCPlayer(null, base.baseEntity, "UnlockedBlueprint", itemDef.itemid);
			base.baseEntity.stats.Add("blueprint_studied", 1, (Stats)5);
		}
	}

	public bool HasUnlocked(ItemDefinition targetItem)
	{
		if (Object.op_Implicit((Object)(object)targetItem.Blueprint))
		{
			if (targetItem.Blueprint.NeedsSteamItem)
			{
				if ((Object)(object)targetItem.steamItem != (Object)null && !steamInventory.HasItem(targetItem.steamItem.id))
				{
					return false;
				}
				if ((Object)(object)targetItem.steamItem == (Object)null)
				{
					bool flag = false;
					ItemSkinDirectory.Skin[] skins = targetItem.skins;
					for (int i = 0; i < skins.Length; i++)
					{
						ItemSkinDirectory.Skin skin = skins[i];
						if (steamInventory.HasItem(skin.id))
						{
							flag = true;
							break;
						}
					}
					if (!flag && targetItem.skins2 != null)
					{
						IPlayerItemDefinition[] skins2 = targetItem.skins2;
						foreach (IPlayerItemDefinition val in skins2)
						{
							if (steamInventory.HasItem(val.get_DefinitionId()))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			if (targetItem.Blueprint.NeedsSteamDLC && (Object)(object)targetItem.steamDlc != (Object)null && targetItem.steamDlc.HasLicense(base.baseEntity.userID))
			{
				return true;
			}
		}
		int[] defaultBlueprints = ItemManager.defaultBlueprints;
		for (int i = 0; i < defaultBlueprints.Length; i++)
		{
			if (defaultBlueprints[i] == targetItem.itemid)
			{
				return true;
			}
		}
		if (base.baseEntity.isServer)
		{
			return IsUnlocked(targetItem);
		}
		return false;
	}

	public bool CanCraft(int itemid, int skinItemId, ulong playerId)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemid);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return false;
		}
		if (skinItemId != 0 && !CheckSkinOwnership(skinItemId, playerId))
		{
			return false;
		}
		if (base.baseEntity.currentCraftLevel < (float)itemDefinition.Blueprint.workbenchLevelRequired)
		{
			return false;
		}
		if (HasUnlocked(itemDefinition))
		{
			return true;
		}
		return false;
	}

	public bool CheckSkinOwnership(int skinItemId, ulong playerId)
	{
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId(skinItemId);
		if ((Object)(object)skin.invItem != (Object)null && skin.invItem.HasUnlocked(playerId))
		{
			return true;
		}
		return steamInventory.HasItem(skinItemId);
	}
}
