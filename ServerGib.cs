using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class ServerGib : BaseCombatEntity
{
	public GameObject _gibSource;

	public string _gibName;

	public PhysicMaterial physicsMaterial;

	private MeshCollider meshCollider;

	private Rigidbody rigidBody;

	public override float BoundsPadding()
	{
		return 3f;
	}

	public static List<ServerGib> CreateGibs(string entityToCreatePath, GameObject creator, GameObject gibSource, Vector3 inheritVelocity, float spreadVelocity)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		List<ServerGib> list = new List<ServerGib>();
		MeshRenderer[] componentsInChildren = gibSource.GetComponentsInChildren<MeshRenderer>(true);
		foreach (MeshRenderer val in componentsInChildren)
		{
			MeshFilter component = ((Component)val).GetComponent<MeshFilter>();
			Vector3 val2 = ((Component)val).get_transform().get_localPosition();
			Vector3 normalized = ((Vector3)(ref val2)).get_normalized();
			Matrix4x4 localToWorldMatrix = creator.get_transform().get_localToWorldMatrix();
			Vector3 val3 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(((Component)val).get_transform().get_localPosition()) + normalized * 0.5f;
			Quaternion val4 = creator.get_transform().get_rotation() * ((Component)val).get_transform().get_localRotation();
			BaseEntity baseEntity = GameManager.server.CreateEntity(entityToCreatePath, val3, val4);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				ServerGib component2 = ((Component)baseEntity).GetComponent<ServerGib>();
				((Component)component2).get_transform().SetPositionAndRotation(val3, val4);
				component2._gibName = ((Object)val).get_name();
				MeshCollider component3 = ((Component)val).GetComponent<MeshCollider>();
				Mesh physicsMesh = (((Object)(object)component3 != (Object)null) ? component3.get_sharedMesh() : component.get_sharedMesh());
				component2.PhysicsInit(physicsMesh);
				val2 = ((Component)val).get_transform().get_localPosition();
				Vector3 val5 = ((Vector3)(ref val2)).get_normalized() * spreadVelocity;
				component2.rigidBody.set_velocity(inheritVelocity + val5);
				Rigidbody obj = component2.rigidBody;
				val2 = Vector3Ex.Range(-1f, 1f);
				obj.set_angularVelocity(((Vector3)(ref val2)).get_normalized() * 1f);
				component2.rigidBody.WakeUp();
				component2.Spawn();
				list.Add(component2);
			}
		}
		foreach (ServerGib item in list)
		{
			foreach (ServerGib item2 in list)
			{
				if (!((Object)(object)item == (Object)(object)item2))
				{
					Physics.IgnoreCollision((Collider)(object)item2.GetCollider(), (Collider)(object)item.GetCollider(), true);
				}
			}
		}
		return list;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && _gibName != "")
		{
			info.msg.servergib = Pool.Get<ServerGib>();
			info.msg.servergib.gibName = _gibName;
		}
	}

	public MeshCollider GetCollider()
	{
		return meshCollider;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)RemoveMe, 1800f);
	}

	public void RemoveMe()
	{
		Kill();
	}

	public virtual void PhysicsInit(Mesh physicsMesh)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Mesh sharedMesh = null;
		MeshFilter component = ((Component)this).get_gameObject().GetComponent<MeshFilter>();
		if ((Object)(object)component != (Object)null)
		{
			sharedMesh = component.get_sharedMesh();
			component.set_sharedMesh(physicsMesh);
		}
		meshCollider = ((Component)this).get_gameObject().AddComponent<MeshCollider>();
		meshCollider.set_sharedMesh(physicsMesh);
		meshCollider.set_convex(true);
		((Collider)meshCollider).set_material(physicsMaterial);
		if ((Object)(object)component != (Object)null)
		{
			component.set_sharedMesh(sharedMesh);
		}
		Rigidbody val = ((Component)this).get_gameObject().AddComponent<Rigidbody>();
		val.set_useGravity(true);
		Bounds val2 = ((Collider)meshCollider).get_bounds();
		Vector3 size = ((Bounds)(ref val2)).get_size();
		float magnitude = ((Vector3)(ref size)).get_magnitude();
		val2 = ((Collider)meshCollider).get_bounds();
		size = ((Bounds)(ref val2)).get_size();
		val.set_mass(Mathf.Clamp(magnitude * ((Vector3)(ref size)).get_magnitude() * 20f, 10f, 2000f));
		val.set_interpolation((RigidbodyInterpolation)1);
		if (base.isServer)
		{
			val.set_drag(0.1f);
			val.set_angularDrag(0.1f);
		}
		rigidBody = val;
		((Component)this).get_gameObject().set_layer(LayerMask.NameToLayer("Default"));
		if (base.isClient)
		{
			val.set_isKinematic(true);
		}
	}
}
