using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepositManager : BaseEntity
{
	[Serializable]
	public class ResourceDeposit
	{
		[Serializable]
		public enum surveySpawnType
		{
			ITEM,
			OIL,
			WATER
		}

		[Serializable]
		public class ResourceDepositEntry
		{
			public ItemDefinition type;

			public float efficiency = 1f;

			public int amount;

			public int startAmount;

			public float workNeeded = 1f;

			public float workDone;

			public surveySpawnType spawnType;

			public bool isLiquid;

			public void Subtract(int subamount)
			{
				if (subamount > 0)
				{
					amount -= subamount;
					if (amount < 0)
					{
						amount = 0;
					}
				}
			}
		}

		public float lastSurveyTime = float.NegativeInfinity;

		public Vector3 origin;

		public List<ResourceDepositEntry> _resources;

		public ResourceDeposit()
		{
			_resources = new List<ResourceDepositEntry>();
		}

		public void Add(ItemDefinition type, float efficiency, int amount, float workNeeded, surveySpawnType spawnType, bool liquid = false)
		{
			ResourceDepositEntry resourceDepositEntry = new ResourceDepositEntry();
			resourceDepositEntry.type = type;
			resourceDepositEntry.efficiency = efficiency;
			resourceDepositEntry.startAmount = (resourceDepositEntry.amount = amount);
			resourceDepositEntry.spawnType = spawnType;
			resourceDepositEntry.workNeeded = workNeeded;
			resourceDepositEntry.isLiquid = liquid;
			_resources.Add(resourceDepositEntry);
		}
	}

	public static ResourceDepositManager _manager;

	private const int resolution = 20;

	public Dictionary<Vector2i, ResourceDeposit> _deposits;

	public static Vector2i GetIndexFrom(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)pos.x / 20, (int)pos.z / 20);
	}

	public static ResourceDepositManager Get()
	{
		return _manager;
	}

	public ResourceDepositManager()
	{
		_manager = this;
		_deposits = new Dictionary<Vector2i, ResourceDeposit>();
	}

	public ResourceDeposit CreateFromPosition(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		Vector2i indexFrom = GetIndexFrom(pos);
		State state = Random.get_state();
		Random.InitState((int)SeedEx.Seed(new Vector2((float)indexFrom.x, (float)indexFrom.y), World.Seed + World.Salt));
		ResourceDeposit resourceDeposit = new ResourceDeposit
		{
			origin = new Vector3((float)(indexFrom.x * 20), 0f, (float)(indexFrom.y * 20))
		};
		if (Random.Range(0f, 1f) < 0.5f)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 100, 1f, ResourceDeposit.surveySpawnType.ITEM);
		}
		else if (0 == 0)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, Random.Range(30000, 100000), Random.Range(0.3f, 0.5f), ResourceDeposit.surveySpawnType.ITEM);
			float num = 0f;
			num = ((!World.Procedural) ? 0.1f : (((TerrainMeta.BiomeMap.GetBiome(pos, 2) > 0.5f) ? 1f : 0f) * 0.25f));
			if (Random.Range(0f, 1f) >= 1f - num)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, Random.Range(10000, 100000), Random.Range(2f, 4f), ResourceDeposit.surveySpawnType.ITEM);
			}
			float num2 = 0f;
			num2 = ((!World.Procedural) ? 0.1f : (((TerrainMeta.BiomeMap.GetBiome(pos, 1) > 0.5f) ? 1f : 0f) * (0.25f + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 8) ? 1f : 0f) + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 1) ? 1f : 0f))));
			if (Random.Range(0f, 1f) >= 1f - num2)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, Random.Range(10000, 100000), Random.Range(4f, 4f), ResourceDeposit.surveySpawnType.ITEM);
			}
			float num3 = 0f;
			if (World.Procedural)
			{
				if (TerrainMeta.BiomeMap.GetBiome(pos, 8) > 0.5f || TerrainMeta.BiomeMap.GetBiome(pos, 4) > 0.5f)
				{
					num3 += 0.25f;
				}
			}
			else
			{
				num3 += 0.15f;
			}
			if (Random.Range(0f, 1f) >= 1f - num3)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, Random.Range(5000, 10000), Random.Range(30f, 50f), ResourceDeposit.surveySpawnType.ITEM);
			}
		}
		_deposits.Add(indexFrom, resourceDeposit);
		Random.set_state(state);
		return resourceDeposit;
	}

	public ResourceDeposit GetFromPosition(Vector3 pos)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ResourceDeposit value = null;
		if (_deposits.TryGetValue(GetIndexFrom(pos), out value))
		{
			return value;
		}
		return null;
	}

	public static ResourceDeposit GetOrCreate(Vector3 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ResourceDeposit fromPosition = Get().GetFromPosition(pos);
		if (fromPosition != null)
		{
			return fromPosition;
		}
		return Get().CreateFromPosition(pos);
	}
}
