using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class BaseCorpse : BaseCombatEntity
{
	public GameObjectRef prefabRagdoll;

	public BaseEntity parentEnt;

	[NonSerialized]
	internal ResourceDispenser resourceDispenser;

	public override TraitFlag Traits => base.Traits | TraitFlag.Food | TraitFlag.Meat;

	public override void ServerInit()
	{
		SetupRigidBody();
		ResetRemovalTime();
		resourceDispenser = ((Component)this).GetComponent<ResourceDispenser>();
		base.ServerInit();
	}

	public virtual void InitCorpse(BaseEntity pr)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		parentEnt = pr;
		((Component)this).get_transform().SetPositionAndRotation(parentEnt.CenterPoint(), ((Component)parentEnt).get_transform().get_rotation());
	}

	public virtual bool CanRemove()
	{
		return true;
	}

	public void RemoveCorpse()
	{
		if (!CanRemove())
		{
			ResetRemovalTime();
		}
		else
		{
			Kill();
		}
	}

	public void ResetRemovalTime(float dur)
	{
		TimeWarning val = TimeWarning.New("ResetRemovalTime", 0);
		try
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)RemoveCorpse))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)RemoveCorpse);
			}
			((FacepunchBehaviour)this).Invoke((Action)RemoveCorpse, dur);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual float GetRemovalTime()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if ((Object)(object)activeGameMode != (Object)null)
		{
			return activeGameMode.CorpseRemovalTime(this);
		}
		return Server.corpsedespawn;
	}

	public void ResetRemovalTime()
	{
		ResetRemovalTime(GetRemovalTime());
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.corpse = Pool.Get<Corpse>();
		if (parentEnt.IsValid())
		{
			info.msg.corpse.parentID = parentEnt.net.ID;
		}
	}

	public void TakeChildren(BaseEntity takeChildrenFrom)
	{
		if (takeChildrenFrom.children == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("Corpse.TakeChildren", 0);
		try
		{
			BaseEntity[] array = takeChildrenFrom.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SwitchParent(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
	}

	private Rigidbody SetupRigidBody()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			GameObject val = base.gameManager.FindPrefab(prefabRagdoll.resourcePath);
			if ((Object)(object)val == (Object)null)
			{
				return null;
			}
			Ragdoll component = val.GetComponent<Ragdoll>();
			if ((Object)(object)component == (Object)null)
			{
				return null;
			}
			if ((Object)(object)component.primaryBody == (Object)null)
			{
				Debug.LogError((object)("[BaseCorpse] ragdoll.primaryBody isn't set!" + ((Object)((Component)component).get_gameObject()).get_name()));
				return null;
			}
			BoxCollider component2 = ((Component)component.primaryBody).GetComponent<BoxCollider>();
			if ((Object)(object)component2 == (Object)null)
			{
				Debug.LogError((object)"Ragdoll has unsupported primary collider (make it supported) ", (Object)(object)component);
				return null;
			}
			BoxCollider obj = ((Component)this).get_gameObject().AddComponent<BoxCollider>();
			obj.set_size(component2.get_size() * 2f);
			obj.set_center(component2.get_center());
			((Collider)obj).set_sharedMaterial(((Collider)component2).get_sharedMaterial());
		}
		Rigidbody val2 = ((Component)this).get_gameObject().AddComponent<Rigidbody>();
		if ((Object)(object)val2 == (Object)null)
		{
			Debug.LogError((object)("[BaseCorpse] already has a RigidBody defined - and it shouldn't!" + ((Object)((Component)this).get_gameObject()).get_name()));
			return null;
		}
		val2.set_mass(10f);
		val2.set_useGravity(true);
		val2.set_drag(0.5f);
		val2.set_collisionDetectionMode((CollisionDetectionMode)0);
		if (base.isServer)
		{
			Buoyancy component3 = ((Component)this).GetComponent<Buoyancy>();
			if ((Object)(object)component3 != (Object)null)
			{
				component3.rigidBody = val2;
			}
			Physics.ApplyDropped(val2);
			Vector3 velocity = Vector3Ex.Range(-1f, 1f);
			velocity.y += 1f;
			val2.set_velocity(velocity);
			val2.set_angularVelocity(Vector3Ex.Range(-10f, 10f));
		}
		return val2;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.corpse != null)
		{
			Load(info.msg.corpse);
		}
	}

	private void Load(Corpse corpse)
	{
		if (base.isServer)
		{
			parentEnt = BaseNetworkable.serverEntities.Find(corpse.parentID) as BaseEntity;
		}
		_ = base.isClient;
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			ResetRemovalTime();
			if (Object.op_Implicit((Object)(object)resourceDispenser))
			{
				resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				base.OnAttacked(info);
			}
		}
	}

	public override string Categorize()
	{
		return "corpse";
	}

	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		ResetRemovalTime();
		Hurt(timeSpent * 5f);
		baseNpc.AddCalories(timeSpent * 2f);
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}
}
