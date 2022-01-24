using UnityEngine;

public class ItemModRecycleInto : ItemMod
{
	public ItemDefinition recycleIntoItem;

	public int numRecycledItemMin = 1;

	public int numRecycledItemMax = 1;

	public GameObjectRef successEffect;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!(command == "recycle_item"))
		{
			return;
		}
		int num = Random.Range(numRecycledItemMin, numRecycledItemMax + 1);
		item.UseItem();
		if (num > 0)
		{
			Item item2 = ItemManager.Create(recycleIntoItem, num, 0uL);
			if (!item2.MoveToContainer(player.inventory.containerMain))
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
