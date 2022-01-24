using UnityEngine;

public class ItemModCookable : ItemMod
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition becomeOnCooked;

	public float cookTime = 30f;

	public int amountOfBecome = 1;

	public int lowTemp;

	public int highTemp;

	public bool setCookingFlag;

	public void OnValidate()
	{
		if (amountOfBecome < 1)
		{
			amountOfBecome = 1;
		}
		if ((Object)(object)becomeOnCooked == (Object)null)
		{
			Debug.LogWarning((object)("[ItemModCookable] becomeOnCooked is unset! [" + ((Object)this).get_name() + "]"), (Object)(object)((Component)this).get_gameObject());
		}
	}

	public override void OnItemCreated(Item itemcreated)
	{
		float cooktimeLeft = cookTime;
		itemcreated.onCycle += delegate(Item item, float delta)
		{
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			float temperature = item.temperature;
			if (temperature < (float)lowTemp || temperature > (float)highTemp || cooktimeLeft < 0f)
			{
				if (setCookingFlag && item.HasFlag(Item.Flag.Cooking))
				{
					item.SetFlag(Item.Flag.Cooking, b: false);
					item.MarkDirty();
				}
			}
			else
			{
				if (setCookingFlag && !item.HasFlag(Item.Flag.Cooking))
				{
					item.SetFlag(Item.Flag.Cooking, b: true);
					item.MarkDirty();
				}
				cooktimeLeft -= delta;
				if (!(cooktimeLeft > 0f))
				{
					int position = item.position;
					if (item.amount > 1)
					{
						cooktimeLeft = cookTime;
						item.amount--;
						item.MarkDirty();
					}
					else
					{
						item.Remove();
					}
					if ((Object)(object)becomeOnCooked != (Object)null)
					{
						Item item2 = ItemManager.Create(becomeOnCooked, amountOfBecome, 0uL);
						if (item2 != null && !item2.MoveToContainer(item.parent, position) && !item2.MoveToContainer(item.parent))
						{
							item2.Drop(item.parent.dropPosition, item.parent.dropVelocity);
							if (Object.op_Implicit((Object)(object)item.parent.entityOwner))
							{
								BaseOven component = ((Component)item.parent.entityOwner).GetComponent<BaseOven>();
								if ((Object)(object)component != (Object)null)
								{
									component.OvenFull();
								}
							}
						}
					}
				}
			}
		};
	}
}
