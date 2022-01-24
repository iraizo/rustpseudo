using Rust;
using UnityEngine;

[RequireComponent(typeof(ItemModConsumable))]
public class ItemModConsume : ItemMod
{
	public GameObjectRef consumeEffect;

	public string eatGesture = "eat_2hand";

	[Tooltip("Items that are given on consumption of this item")]
	public ItemAmountRandom[] product;

	public ItemModConsumable primaryConsumable;

	public virtual ItemModConsumable GetConsumable()
	{
		if (Object.op_Implicit((Object)(object)primaryConsumable))
		{
			return primaryConsumable;
		}
		return ((Component)this).GetComponent<ItemModConsumable>();
	}

	public virtual GameObjectRef GetConsumeEffect()
	{
		return consumeEffect;
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (item.amount < 1)
		{
			return;
		}
		GameObjectRef gameObjectRef = GetConsumeEffect();
		if (gameObjectRef.isValid)
		{
			Vector3 posLocal = (player.IsDucked() ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 2f, 0f));
			Effect.server.Run(gameObjectRef.resourcePath, player, 0u, posLocal, Vector3.get_zero());
		}
		player.metabolism.MarkConsumption();
		ItemModConsumable consumable = GetConsumable();
		if (!string.IsNullOrEmpty(consumable.achievementWhenEaten))
		{
			player.GiveAchievement(consumable.achievementWhenEaten);
		}
		float num = Mathf.Max(consumable.amountToConsume, 1);
		float num2 = Mathf.Min((float)item.amount, num);
		float num3 = num2 / num;
		float num4 = item.conditionNormalized;
		if (consumable.conditionFractionToLose > 0f)
		{
			num4 = consumable.conditionFractionToLose;
		}
		foreach (ItemModConsumable.ConsumableEffect effect in consumable.effects)
		{
			if (Mathf.Clamp01(player.healthFraction + player.metabolism.pending_health.Fraction()) > effect.onlyIfHealthLessThan)
			{
				continue;
			}
			if (effect.type == MetabolismAttribute.Type.Health)
			{
				if (effect.amount < 0f)
				{
					player.OnAttacked(new HitInfo(player, player, DamageType.Generic, (0f - effect.amount) * num3 * num4, ((Component)player).get_transform().get_position() + ((Component)player).get_transform().get_forward() * 1f));
				}
				else
				{
					player.health += effect.amount * num3 * num4;
				}
			}
			else
			{
				player.metabolism.ApplyChange(effect.type, effect.amount * num3 * num4, effect.time * num3 * num4);
			}
		}
		if ((Object)(object)player.modifiers != (Object)null)
		{
			player.modifiers.Add(consumable.modifiers);
		}
		if (product != null)
		{
			ItemAmountRandom[] array = product;
			foreach (ItemAmountRandom itemAmountRandom in array)
			{
				int num5 = Mathf.RoundToInt((float)itemAmountRandom.RandomAmount() * num4);
				if (num5 > 0)
				{
					Item item2 = ItemManager.Create(itemAmountRandom.itemDef, num5, 0uL);
					player.GiveItem(item2);
				}
			}
		}
		if (string.IsNullOrEmpty(eatGesture))
		{
			player.SignalBroadcast(BaseEntity.Signal.Gesture, eatGesture);
		}
		if (consumable.conditionFractionToLose > 0f)
		{
			item.LoseCondition(consumable.conditionFractionToLose * item.maxCondition);
		}
		else
		{
			item.UseItem((int)num2);
		}
	}

	public override bool CanDoAction(Item item, BasePlayer player)
	{
		return player.metabolism.CanConsume();
	}
}
