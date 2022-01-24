using UnityEngine;

public class ModularVehicleShopFront : ShopFront
{
	[SerializeField]
	private float maxUseDistance = 1.5f;

	public override bool CanBeLooted(BasePlayer player)
	{
		if (WithinUseDistance(player))
		{
			return base.CanBeLooted(player);
		}
		return false;
	}

	private bool WithinUseDistance(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Distance(player.eyes.position) <= maxUseDistance;
	}
}
