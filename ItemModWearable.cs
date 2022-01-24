using Rust;
using UnityEngine;

public class ItemModWearable : ItemMod
{
	public GameObjectRef entityPrefab = new GameObjectRef();

	public GameObjectRef entityPrefabFemale = new GameObjectRef();

	public ProtectionProperties protectionProperties;

	public ArmorProperties armorProperties;

	public ClothingMovementProperties movementProperties;

	public UIBlackoutOverlay.blackoutType occlusionType = UIBlackoutOverlay.blackoutType.NONE;

	public bool blocksAiming;

	public bool emissive;

	public float accuracyBonus;

	public bool blocksEquipping;

	public float eggVision;

	public float weight;

	public bool equipOnRightClick = true;

	public bool npcOnly;

	public GameObjectRef viewmodelAddition;

	public Wearable targetWearable
	{
		get
		{
			if (entityPrefab.isValid)
			{
				return entityPrefab.Get().GetComponent<Wearable>();
			}
			return null;
		}
	}

	private void DoPrepare()
	{
		if (!entityPrefab.isValid)
		{
			Debug.LogWarning((object)("ItemModWearable: entityPrefab is null! " + ((Component)this).get_gameObject()), (Object)(object)((Component)this).get_gameObject());
		}
		if (entityPrefab.isValid && (Object)(object)targetWearable == (Object)null)
		{
			Debug.LogWarning((object)("ItemModWearable: entityPrefab doesn't have a Wearable component! " + ((Component)this).get_gameObject()), (Object)(object)entityPrefab.Get());
		}
	}

	public override void ModInit()
	{
		if (string.IsNullOrEmpty(entityPrefab.resourcePath))
		{
			Debug.LogWarning((object)string.Concat(this, " - entityPrefab is null or something.. - ", entityPrefab.guid));
		}
	}

	public bool ProtectsArea(HitArea area)
	{
		if ((Object)(object)armorProperties == (Object)null)
		{
			return false;
		}
		return armorProperties.Contains(area);
	}

	public bool HasProtections()
	{
		return (Object)(object)protectionProperties != (Object)null;
	}

	internal float GetProtection(Item item, DamageType damageType)
	{
		if ((Object)(object)protectionProperties == (Object)null)
		{
			return 0f;
		}
		return protectionProperties.Get(damageType) * ConditionProtectionScale(item);
	}

	public float ConditionProtectionScale(Item item)
	{
		if (!item.isBroken)
		{
			return 1f;
		}
		return 0.25f;
	}

	public void CollectProtection(Item item, ProtectionProperties protection)
	{
		if (!((Object)(object)protectionProperties == (Object)null))
		{
			protection.Add(protectionProperties, ConditionProtectionScale(item));
		}
	}

	private bool IsHeadgear()
	{
		Wearable component = entityPrefab.Get().GetComponent<Wearable>();
		if ((Object)(object)component != (Object)null && (component.occupationOver & (Wearable.OccupationSlots.HeadTop | Wearable.OccupationSlots.Face | Wearable.OccupationSlots.HeadBack)) != 0)
		{
			return true;
		}
		return false;
	}

	public bool IsFootwear()
	{
		Wearable component = entityPrefab.Get().GetComponent<Wearable>();
		if ((Object)(object)component != (Object)null && (component.occupationOver & (Wearable.OccupationSlots.LeftFoot | Wearable.OccupationSlots.RightFoot)) != 0)
		{
			return true;
		}
		return false;
	}

	public override void OnAttacked(Item item, HitInfo info)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (!item.hasCondition)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < 25; i++)
		{
			DamageType damageType = (DamageType)i;
			if (info.damageTypes.Has(damageType))
			{
				num += Mathf.Clamp(info.damageTypes.types[i] * GetProtection(item, damageType), 0f, item.condition);
				if (num >= item.condition)
				{
					break;
				}
			}
		}
		item.LoseCondition(num);
		if (item != null && item.isBroken && Object.op_Implicit((Object)(object)item.GetOwnerPlayer()) && IsHeadgear() && info.damageTypes.Total() >= item.GetOwnerPlayer().health)
		{
			Vector3 vPos = ((Component)item.GetOwnerPlayer()).get_transform().get_position() + new Vector3(0f, 1.8f, 0f);
			Vector3 vVelocity = item.GetOwnerPlayer().GetInheritedDropVelocity() + Vector3.get_up() * 3f;
			Quaternion rotation = default(Quaternion);
			BaseEntity baseEntity = item.Drop(vPos, vVelocity, rotation);
			rotation = Random.get_rotation();
			baseEntity.SetAngularVelocity(((Quaternion)(ref rotation)).get_eulerAngles() * 5f);
		}
	}

	public bool CanExistWith(ItemModWearable wearable)
	{
		if ((Object)(object)wearable == (Object)null)
		{
			return true;
		}
		Wearable wearable2 = targetWearable;
		Wearable wearable3 = wearable.targetWearable;
		if ((wearable2.occupationOver & wearable3.occupationOver) != 0)
		{
			return false;
		}
		if ((wearable2.occupationUnder & wearable3.occupationUnder) != 0)
		{
			return false;
		}
		return true;
	}
}
