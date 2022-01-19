using UnityEngine;

public class ItemModGiveOxygen : ItemMod, IAirSupply
{
	public enum AirSupplyType
	{
		Lungs,
		ScubaTank,
		Submarine
	}

	public AirSupplyType airType = AirSupplyType.ScubaTank;

	public int amountToConsume = 1;

	public GameObjectRef inhaleEffect;

	public GameObjectRef exhaleEffect;

	public GameObjectRef bubblesEffect;

	private float timeRemaining;

	private float cycleTime;

	private bool inhaled;

	public AirSupplyType AirType => airType;

	public float GetAirTimeRemaining()
	{
		return timeRemaining;
	}

	public override void ModInit()
	{
		base.ModInit();
		cycleTime = 1f;
		ItemMod[] array = siblingMods;
		for (int i = 0; i < array.Length; i++)
		{
			ItemModCycle itemModCycle;
			if ((itemModCycle = array[i] as ItemModCycle) != null)
			{
				cycleTime = itemModCycle.timeBetweenCycles;
			}
		}
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (!item.hasCondition || item.conditionNormalized == 0f || (Object)(object)player == (Object)null)
		{
			return;
		}
		float num = Mathf.Clamp01(0.525f);
		if (!(player.AirFactor() > num) && item.parent != null && item.parent == player.inventory.containerWear)
		{
			Effect.server.Run((!inhaled) ? inhaleEffect.resourcePath : exhaleEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.get_zero(), Vector3.get_forward());
			inhaled = !inhaled;
			if (!inhaled && WaterLevel.GetWaterDepth(player.eyes.position, Object.op_Implicit((Object)(object)player)) > 3f)
			{
				Effect.server.Run(bubblesEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.get_zero(), Vector3.get_forward());
			}
			item.LoseCondition(amountToConsume);
			player.metabolism.oxygen.Add(1f);
		}
	}

	public override void OnChanged(Item item)
	{
		if (item.hasCondition)
		{
			timeRemaining = item.condition * ((float)amountToConsume / cycleTime);
		}
		else
		{
			timeRemaining = 0f;
		}
	}
}
