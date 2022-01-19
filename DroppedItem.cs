using System;
using ConVar;
using UnityEngine;

public class DroppedItem : WorldItem
{
	[Header("DroppedItem")]
	public GameObject itemModel;

	private Collider childCollider;

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (GetDespawnDuration() < float.PositiveInfinity)
		{
			((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
		}
		ReceiveCollisionMessages(b: true);
	}

	public virtual float GetDespawnDuration()
	{
		if (item != null && item.info.quickDespawn)
		{
			return 30f;
		}
		int num = ((item == null) ? 1 : item.despawnMultiplier);
		return Server.itemdespawn * (float)num;
	}

	public void IdleDestroy()
	{
		DestroyItem();
		Kill();
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (item != null && item.info.stackable > 1)
		{
			DroppedItem droppedItem = hitEntity as DroppedItem;
			if (!((Object)(object)droppedItem == (Object)null) && droppedItem.item != null && !((Object)(object)droppedItem.item.info != (Object)(object)item.info))
			{
				droppedItem.OnDroppedOn(this);
			}
		}
	}

	public void OnDroppedOn(DroppedItem di)
	{
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		if (item == null || di.item == null || (Object)(object)di.item.info != (Object)(object)item.info || (di.item.IsBlueprint() && di.item.blueprintTarget != item.blueprintTarget) || (di.item.hasCondition && di.item.condition != di.item.maxCondition) || (item.hasCondition && item.condition != item.maxCondition))
		{
			return;
		}
		if ((Object)(object)di.item.info != (Object)null)
		{
			if (di.item.info.amountType == ItemDefinition.AmountType.Genetics)
			{
				int num = ((di.item.instanceData != null) ? di.item.instanceData.dataInt : (-1));
				int num2 = ((item.instanceData != null) ? item.instanceData.dataInt : (-1));
				if (num != num2)
				{
					return;
				}
			}
			if (((Object)(object)((Component)di.item.info).GetComponent<ItemModSign>() != (Object)null && (Object)(object)ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(di.item) != (Object)null) || ((Object)(object)item.info != (Object)null && (Object)(object)((Component)item.info).GetComponent<ItemModSign>() != (Object)null && (Object)(object)ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(item) != (Object)null))
			{
				return;
			}
		}
		int num3 = di.item.amount + item.amount;
		if (num3 <= item.info.stackable && num3 != 0)
		{
			di.DestroyItem();
			di.Kill();
			item.amount = num3;
			item.MarkDirty();
			if (GetDespawnDuration() < float.PositiveInfinity)
			{
				((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
			}
			Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", this, 0u, Vector3.get_zero(), Vector3.get_zero());
		}
	}

	internal override void OnParentRemoved()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if ((Object)(object)component == (Object)null)
		{
			base.OnParentRemoved();
			return;
		}
		Vector3 val = ((Component)this).get_transform().get_position();
		Quaternion rotation = ((Component)this).get_transform().get_rotation();
		SetParent(null);
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val + Vector3.get_up() * 2f, Vector3.get_down(), ref val2, 2f, 27328512) && val.y < ((RaycastHit)(ref val2)).get_point().y)
		{
			val += Vector3.get_up() * 1.5f;
		}
		((Component)this).get_transform().set_position(val);
		((Component)this).get_transform().set_rotation(rotation);
		Physics.ApplyDropped(component);
		component.set_isKinematic(false);
		component.set_useGravity(true);
		component.WakeUp();
		if (GetDespawnDuration() < float.PositiveInfinity)
		{
			((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
		}
	}

	public override void PostInitShared()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		base.PostInitShared();
		GameObject val = null;
		val = ((item == null || !item.info.worldModelPrefab.isValid) ? Object.Instantiate<GameObject>(itemModel) : item.info.worldModelPrefab.Instantiate());
		val.get_transform().SetParent(((Component)this).get_transform(), false);
		val.get_transform().set_localPosition(Vector3.get_zero());
		val.get_transform().set_localRotation(Quaternion.get_identity());
		val.SetLayerRecursive(((Component)this).get_gameObject().get_layer());
		childCollider = val.GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)childCollider))
		{
			childCollider.set_enabled(false);
			childCollider.set_enabled(true);
		}
		if (base.isServer)
		{
			WorldModel component = val.GetComponent<WorldModel>();
			float mass = (Object.op_Implicit((Object)(object)component) ? component.mass : 1f);
			float drag = 0.1f;
			float angularDrag = 0.1f;
			Rigidbody obj = ((Component)this).get_gameObject().AddComponent<Rigidbody>();
			obj.set_mass(mass);
			obj.set_drag(drag);
			obj.set_angularDrag(angularDrag);
			obj.set_interpolation((RigidbodyInterpolation)0);
			Physics.ApplyDropped(obj);
			Renderer[] componentsInChildren = val.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].set_enabled(false);
			}
		}
		if (item != null)
		{
			PhysicsEffects component2 = ((Component)this).get_gameObject().GetComponent<PhysicsEffects>();
			if ((Object)(object)component2 != (Object)null)
			{
				component2.entity = this;
				if ((Object)(object)item.info.physImpactSoundDef != (Object)null)
				{
					component2.physImpactSoundDef = item.info.physImpactSoundDef;
				}
			}
		}
		val.SetActive(true);
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}
}
