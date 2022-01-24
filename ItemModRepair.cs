using UnityEngine;

public class ItemModRepair : ItemMod
{
	public float conditionLost = 0.05f;

	public GameObjectRef successEffect;

	public int workbenchLvlRequired;

	public bool HasCraftLevel(BasePlayer player = null)
	{
		if ((Object)(object)player != (Object)null && player.isServer)
		{
			return player.currentCraftLevel >= (float)workbenchLvlRequired;
		}
		return false;
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (command == "refill" && !player.IsSwimming() && HasCraftLevel(player) && !(item.conditionNormalized >= 1f))
		{
			item.DoRepair(conditionLost);
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
