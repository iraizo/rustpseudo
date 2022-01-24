using UnityEngine;

public class WaterResource
{
	public static ItemDefinition GetAtPoint(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return ItemManager.FindItemDefinition(IsFreshWater(pos) ? "water" : "water.salt");
	}

	public static bool IsFreshWater(Vector3 pos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.TopologyMap == (Object)null)
		{
			return false;
		}
		return TerrainMeta.TopologyMap.GetTopology(pos, 245760);
	}

	public static ItemDefinition Merge(ItemDefinition first, ItemDefinition second)
	{
		if ((Object)(object)first == (Object)(object)second)
		{
			return first;
		}
		if (first.shortname == "water.salt" || second.shortname == "water.salt")
		{
			return ItemManager.FindItemDefinition("water.salt");
		}
		return ItemManager.FindItemDefinition("water");
	}
}
