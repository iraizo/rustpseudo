using UnityEngine;

public class ItemModUnwrap : ItemMod
{
	public LootSpawn revealList;

	public GameObjectRef successEffect;

	public int minTries = 1;

	public int maxTries = 1;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (command == "unwrap" && item.amount > 0)
		{
			item.UseItem();
			int num = Random.Range(minTries, maxTries + 1);
			for (int i = 0; i < num; i++)
			{
				revealList.SpawnIntoContainer(player.inventory.containerMain);
			}
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
