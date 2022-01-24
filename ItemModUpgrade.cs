using UnityEngine;

public class ItemModUpgrade : ItemMod
{
	public int numForUpgrade = 10;

	public float upgradeSuccessChance = 1f;

	public int numToLoseOnFail = 2;

	public ItemDefinition upgradedItem;

	public int numUpgradedItem = 1;

	public GameObjectRef successEffect;

	public GameObjectRef failEffect;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (!(command == "upgrade_item") || item.amount < numForUpgrade)
		{
			return;
		}
		if (Random.Range(0f, 1f) <= upgradeSuccessChance)
		{
			item.UseItem(numForUpgrade);
			Item item2 = ItemManager.Create(upgradedItem, numUpgradedItem, 0uL);
			if (!item2.MoveToContainer(player.inventory.containerMain))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity());
			}
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
		else
		{
			item.UseItem(numToLoseOnFail);
			if (failEffect.isValid)
			{
				Effect.server.Run(failEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
