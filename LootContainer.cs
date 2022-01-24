using System;
using Rust;
using UnityEngine;

public class LootContainer : StorageContainer
{
	public enum spawnType
	{
		GENERIC,
		PLAYER,
		TOWN,
		AIRDROP,
		CRASHSITE,
		ROADSIDE
	}

	[Serializable]
	public struct LootSpawnSlot
	{
		public LootSpawn definition;

		public int numberToSpawn;

		public float probability;
	}

	public bool destroyOnEmpty = true;

	public LootSpawn lootDefinition;

	public int maxDefinitionsToSpawn;

	public float minSecondsBetweenRefresh = 3600f;

	public float maxSecondsBetweenRefresh = 7200f;

	public bool initialLootSpawn = true;

	public float xpLootedScale = 1f;

	public float xpDestroyedScale = 1f;

	public bool BlockPlayerItemInput;

	public int scrapAmount;

	public string deathStat = "";

	public LootSpawnSlot[] LootSpawnSlots;

	public spawnType SpawnType;

	private static ItemDefinition scrapDef;

	public bool shouldRefreshContents
	{
		get
		{
			if (minSecondsBetweenRefresh > 0f)
			{
				return maxSecondsBetweenRefresh > 0f;
			}
			return false;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (initialLootSpawn)
		{
			SpawnLoot();
		}
		if (BlockPlayerItemInput && !Application.isLoadingSave && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, b: true);
		}
		SetFlag(Flags.Reserved6, PlayerInventory.IsBirthday());
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (BlockPlayerItemInput && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, b: true);
		}
	}

	public virtual void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log((object)"CONTACT DEVELOPERS! LootContainer::PopulateLoot has null inventory!!!");
			return;
		}
		base.inventory.Clear();
		ItemManager.DoRemoves();
		PopulateLoot();
		if (shouldRefreshContents)
		{
			((FacepunchBehaviour)this).Invoke((Action)SpawnLoot, Random.Range(minSecondsBetweenRefresh, maxSecondsBetweenRefresh));
		}
	}

	public int ScoreForRarity(Rarity rarity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		return (rarity - 1) switch
		{
			0 => 1, 
			1 => 2, 
			2 => 3, 
			3 => 4, 
			_ => 5000, 
		};
	}

	public virtual void PopulateLoot()
	{
		if (LootSpawnSlots.Length != 0)
		{
			LootSpawnSlot[] lootSpawnSlots = LootSpawnSlots;
			for (int i = 0; i < lootSpawnSlots.Length; i++)
			{
				LootSpawnSlot lootSpawnSlot = lootSpawnSlots[i];
				for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
				{
					if (Random.Range(0f, 1f) <= lootSpawnSlot.probability)
					{
						lootSpawnSlot.definition.SpawnIntoContainer(base.inventory);
					}
				}
			}
		}
		else if ((Object)(object)lootDefinition != (Object)null)
		{
			for (int k = 0; k < maxDefinitionsToSpawn; k++)
			{
				lootDefinition.SpawnIntoContainer(base.inventory);
			}
		}
		if (SpawnType == spawnType.ROADSIDE || SpawnType == spawnType.TOWN)
		{
			foreach (Item item in base.inventory.itemList)
			{
				if (item.hasCondition)
				{
					item.condition = Random.Range(item.info.condition.foundCondition.fractionMin, item.info.condition.foundCondition.fractionMax) * item.info.condition.max;
				}
			}
		}
		GenerateScrap();
	}

	public void GenerateScrap()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (scrapAmount <= 0)
		{
			return;
		}
		if ((Object)(object)scrapDef == (Object)null)
		{
			scrapDef = ItemManager.FindItemDefinition("scrap");
		}
		int num = scrapAmount;
		if (num > 0)
		{
			Item item = ItemManager.Create(scrapDef, num, 0uL);
			if (!item.MoveToContainer(base.inventory))
			{
				item.Drop(((Component)this).get_transform().get_position(), GetInheritedDropVelocity());
			}
		}
	}

	public override void DropBonusItems(BaseEntity initiator, ItemContainer container)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		base.DropBonusItems(initiator, container);
		if ((Object)(object)initiator == (Object)null || container == null)
		{
			return;
		}
		BasePlayer basePlayer = initiator as BasePlayer;
		if ((Object)(object)basePlayer == (Object)null || scrapAmount <= 0 || !((Object)(object)scrapDef != (Object)null))
		{
			return;
		}
		float num = (((Object)(object)basePlayer.modifiers != (Object)null) ? (1f + basePlayer.modifiers.GetValue(Modifier.ModifierType.Scrap_Yield)) : 0f);
		if (num > 1f)
		{
			float variableValue = basePlayer.modifiers.GetVariableValue(Modifier.ModifierType.Scrap_Yield, 0f);
			float num2 = Mathf.Max((float)scrapAmount * num - (float)scrapAmount, 0f);
			variableValue += num2;
			int num3 = 0;
			if (variableValue >= 1f)
			{
				num3 = (int)variableValue;
				variableValue -= (float)num3;
			}
			basePlayer.modifiers.SetVariableValue(Modifier.ModifierType.Scrap_Yield, variableValue);
			if (num3 > 0)
			{
				ItemManager.Create(scrapDef, num3, 0uL)?.Drop(GetDropPosition() + new Vector3(0f, 0.5f, 0f), GetInheritedDropVelocity());
			}
		}
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (destroyOnEmpty && (base.inventory == null || base.inventory.itemList == null || base.inventory.itemList.Count == 0))
		{
			Kill(DestroyMode.Gib);
		}
	}

	public void RemoveMe()
	{
		Kill(DestroyMode.Gib);
	}

	public override bool ShouldDropItemsIndividually()
	{
		return true;
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (info != null && (Object)(object)info.InitiatorPlayer != (Object)null && !string.IsNullOrEmpty(deathStat))
		{
			info.InitiatorPlayer.stats.Add(deathStat, 1, Stats.Life);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	public override void InitShared()
	{
		base.InitShared();
	}
}
