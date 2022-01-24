using UnityEngine;

public class ItemModReveal : ItemMod
{
	public int numForReveal = 10;

	public ItemDefinition revealedItemOverride;

	public int revealedItemAmount = 1;

	public LootSpawn revealList;

	public GameObjectRef successEffect;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (command == "reveal" && item.amount >= numForReveal)
		{
			int position = item.position;
			item.UseItem(numForReveal);
			Item item2 = null;
			if (Object.op_Implicit((Object)(object)revealedItemOverride))
			{
				item2 = ItemManager.Create(revealedItemOverride, revealedItemAmount, 0uL);
			}
			if (item2 != null && !item2.MoveToContainer(player.inventory.containerMain, (item.amount == 0) ? position : (-1)))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity());
			}
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
