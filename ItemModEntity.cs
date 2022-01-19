using UnityEngine;

public class ItemModEntity : ItemMod
{
	public GameObjectRef entityPrefab = new GameObjectRef();

	public string defaultBone;

	public override void OnItemCreated(Item item)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)item.GetHeldEntity() == (Object)null)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(entityPrefab.resourcePath);
			if ((Object)(object)baseEntity == (Object)null)
			{
				Debug.LogWarning((object)("Couldn't create item entity " + item.info.displayName.english + " (" + entityPrefab.resourcePath + ")"));
			}
			else
			{
				baseEntity.skinID = item.skin;
				baseEntity.Spawn();
				item.SetHeldEntity(baseEntity);
			}
		}
	}

	public override void OnRemove(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (!((Object)(object)heldEntity == (Object)null))
		{
			heldEntity.Kill();
			item.SetHeldEntity(null);
		}
	}

	private bool ParentToParent(Item item, BaseEntity ourEntity)
	{
		if (item.parentItem == null)
		{
			return false;
		}
		BaseEntity baseEntity = item.parentItem.GetWorldEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			baseEntity = item.parentItem.GetHeldEntity();
		}
		ourEntity.SetFlag(BaseEntity.Flags.Disabled, b: false);
		ourEntity.limitNetworking = false;
		ourEntity.SetParent(baseEntity, defaultBone);
		return true;
	}

	private bool ParentToPlayer(Item item, BaseEntity ourEntity)
	{
		HeldEntity heldEntity = ourEntity as HeldEntity;
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (Object.op_Implicit((Object)(object)ownerPlayer))
		{
			heldEntity.SetOwnerPlayer(ownerPlayer);
			return true;
		}
		heldEntity.ClearOwnerPlayer();
		return true;
	}

	public override void OnParentChanged(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (!((Object)(object)heldEntity == (Object)null) && !ParentToParent(item, heldEntity) && !ParentToPlayer(item, heldEntity))
		{
			heldEntity.SetParent(null);
			heldEntity.limitNetworking = true;
			heldEntity.SetFlag(BaseEntity.Flags.Disabled, b: true);
		}
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (!((Object)(object)heldEntity == (Object)null))
		{
			HeldEntity heldEntity2 = heldEntity as HeldEntity;
			if (!((Object)(object)heldEntity2 == (Object)null))
			{
				heldEntity2.CollectedForCrafting(item, crafter);
			}
		}
	}

	public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (!((Object)(object)heldEntity == (Object)null))
		{
			HeldEntity heldEntity2 = heldEntity as HeldEntity;
			if (!((Object)(object)heldEntity2 == (Object)null))
			{
				heldEntity2.ReturnedFromCancelledCraft(item, crafter);
			}
		}
	}
}
