using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

public class HitboxSystem : MonoBehaviour, IPrefabPreProcess
{
	[Serializable]
	public class HitboxShape
	{
		public Transform bone;

		public HitboxDefinition.Type type;

		public Matrix4x4 localTransform;

		public PhysicMaterial colliderMaterial;

		private Matrix4x4 transform;

		private Matrix4x4 inverseTransform;

		public Matrix4x4 Transform => transform;

		public Vector3 Position => ((Matrix4x4)(ref transform)).MultiplyPoint(Vector3.get_zero());

		public Quaternion Rotation => ((Matrix4x4)(ref transform)).get_rotation();

		public Vector3 Size { get; private set; }

		public void UpdateTransform()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			TimeWarning val = TimeWarning.New("HitboxSystem.UpdateTransform", 0);
			try
			{
				transform = bone.get_localToWorldMatrix() * localTransform;
				Size = ((Matrix4x4)(ref transform)).get_lossyScale();
				transform = Matrix4x4.TRS(Position, Rotation, Vector3.get_one());
				inverseTransform = ((Matrix4x4)(ref transform)).get_inverse();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		public Vector3 TransformPoint(Vector3 pt)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref transform)).MultiplyPoint(pt);
		}

		public Vector3 InverseTransformPoint(Vector3 pt)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref inverseTransform)).MultiplyPoint(pt);
		}

		public Vector3 TransformDirection(Vector3 pt)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref transform)).MultiplyVector(pt);
		}

		public Vector3 InverseTransformDirection(Vector3 pt)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref inverseTransform)).MultiplyVector(pt);
		}

		public bool Trace(Ray ray, out RaycastHit hit, float forgivness = 0f, float maxDistance = float.PositiveInfinity)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			TimeWarning val = TimeWarning.New("Hitbox.Trace", 0);
			try
			{
				((Ray)(ref ray)).set_origin(InverseTransformPoint(((Ray)(ref ray)).get_origin()));
				((Ray)(ref ray)).set_direction(InverseTransformDirection(((Ray)(ref ray)).get_direction()));
				if (type == HitboxDefinition.Type.BOX)
				{
					AABB val2 = default(AABB);
					((AABB)(ref val2))._002Ector(Vector3.get_zero(), Size);
					if (!((AABB)(ref val2)).Trace(ray, ref hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				else
				{
					Capsule val3 = default(Capsule);
					((Capsule)(ref val3))._002Ector(Vector3.get_zero(), Size.x, Size.y * 0.5f);
					if (!((Capsule)(ref val3)).Trace(ray, ref hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				((RaycastHit)(ref hit)).set_point(TransformPoint(((RaycastHit)(ref hit)).get_point()));
				((RaycastHit)(ref hit)).set_normal(TransformDirection(((RaycastHit)(ref hit)).get_normal()));
				return true;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		public Bounds GetBounds()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 val = Transform;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					((Matrix4x4)(ref val)).set_Item(i, j, Mathf.Abs(((Matrix4x4)(ref val)).get_Item(i, j)));
				}
			}
			Bounds result = default(Bounds);
			Matrix4x4 val2 = Transform;
			((Bounds)(ref result)).set_center(((Matrix4x4)(ref val2)).MultiplyPoint(Vector3.get_zero()));
			((Bounds)(ref result)).set_extents(((Matrix4x4)(ref val)).MultiplyVector(Size));
			return result;
		}
	}

	public List<HitboxShape> hitboxes = new List<HitboxShape>();

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		List<HitboxDefinition> list = Pool.GetList<HitboxDefinition>();
		((Component)this).GetComponentsInChildren<HitboxDefinition>(list);
		if (serverside)
		{
			foreach (HitboxDefinition item2 in list)
			{
				if (preProcess != null)
				{
					preProcess.RemoveComponent((Component)(object)item2);
				}
			}
			if (preProcess != null)
			{
				preProcess.RemoveComponent((Component)(object)this);
			}
		}
		if (clientside)
		{
			hitboxes.Clear();
			foreach (HitboxDefinition item3 in (IEnumerable<HitboxDefinition>)Enumerable.OrderBy<HitboxDefinition, int>((IEnumerable<HitboxDefinition>)list, (Func<HitboxDefinition, int>)((HitboxDefinition x) => x.priority)))
			{
				HitboxShape item = new HitboxShape
				{
					bone = ((Component)item3).get_transform(),
					localTransform = item3.LocalMatrix,
					colliderMaterial = item3.physicMaterial,
					type = item3.type
				};
				hitboxes.Add(item);
				if (preProcess != null)
				{
					preProcess.RemoveComponent((Component)(object)item3);
				}
			}
		}
		Pool.FreeList<HitboxDefinition>(ref list);
	}

	public HitboxSystem()
		: this()
	{
	}
}
