using ProtoBuf;
using UnityEngine;

public abstract class ItemModAssociatedEntity<T> : ItemMod where T : BaseEntity
{
	public GameObjectRef entityPrefab;

	protected virtual bool AllowNullParenting => false;

	protected virtual bool AllowHeldEntityParenting => false;

	protected virtual bool ShouldAutoCreateEntity => true;

	public override void OnItemCreated(Item item)
	{
		base.OnItemCreated(item);
		if (ShouldAutoCreateEntity)
		{
			CreateAssociatedEntity(item);
		}
	}

	public T CreateAssociatedEntity(Item item)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (item.instanceData != null)
		{
			return null;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(entityPrefab.resourcePath, Vector3.get_zero());
		T component = ((Component)baseEntity).GetComponent<T>();
		OnAssociatedItemCreated(component);
		baseEntity.Spawn();
		item.instanceData = new InstanceData();
		item.instanceData.ShouldPool = false;
		item.instanceData.subEntity = baseEntity.net.ID;
		item.MarkDirty();
		return component;
	}

	protected virtual void OnAssociatedItemCreated(T ent)
	{
	}

	public override void OnRemove(Item item)
	{
		base.OnRemove(item);
		T associatedEntity = GetAssociatedEntity(item);
		if (Object.op_Implicit((Object)(object)associatedEntity))
		{
			associatedEntity.Kill();
		}
	}

	public override void OnMovedToWorld(Item item)
	{
		UpdateParent(item);
		base.OnMovedToWorld(item);
	}

	public override void OnRemovedFromWorld(Item item)
	{
		UpdateParent(item);
		base.OnRemovedFromWorld(item);
	}

	public void UpdateParent(Item item)
	{
		BaseEntity entityForParenting = GetEntityForParenting(item);
		if ((Object)(object)entityForParenting == (Object)null)
		{
			if (AllowNullParenting)
			{
				T associatedEntity = GetAssociatedEntity(item);
				if ((Object)(object)associatedEntity != (Object)null)
				{
					associatedEntity.SetParent(null, worldPositionStays: false, sendImmediate: true);
				}
			}
		}
		else if (entityForParenting.isServer && entityForParenting.IsFullySpawned())
		{
			T associatedEntity2 = GetAssociatedEntity(item);
			if (Object.op_Implicit((Object)(object)associatedEntity2))
			{
				associatedEntity2.SetParent(entityForParenting, worldPositionStays: false, sendImmediate: true);
			}
		}
	}

	public override void OnParentChanged(Item item)
	{
		base.OnParentChanged(item);
		UpdateParent(item);
	}

	public BaseEntity GetEntityForParenting(Item item = null)
	{
		if (item != null)
		{
			BasePlayer ownerPlayer = item.GetOwnerPlayer();
			if (Object.op_Implicit((Object)(object)ownerPlayer))
			{
				return ownerPlayer;
			}
			BaseEntity baseEntity = ((item.parent == null) ? null : item.parent.entityOwner);
			if ((Object)(object)baseEntity != (Object)null)
			{
				return baseEntity;
			}
			BaseEntity worldEntity = item.GetWorldEntity();
			if (Object.op_Implicit((Object)(object)worldEntity))
			{
				return worldEntity;
			}
			if (AllowHeldEntityParenting && item.parentItem != null && (Object)(object)item.parentItem.GetHeldEntity() != (Object)null)
			{
				return item.parentItem.GetHeldEntity();
			}
			return null;
		}
		return null;
	}

	public static T GetAssociatedEntity(Item item, bool isServer = true)
	{
		if (item?.instanceData == null)
		{
			return null;
		}
		BaseNetworkable baseNetworkable = null;
		if (isServer)
		{
			baseNetworkable = BaseNetworkable.serverEntities.Find(item.instanceData.subEntity);
		}
		if (Object.op_Implicit((Object)(object)baseNetworkable))
		{
			return ((Component)baseNetworkable).GetComponent<T>();
		}
		return null;
	}
}
