using Rust;

namespace UnityEngine
{
	public static class ColliderEx
	{
		public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (obj is TerrainCollider)
			{
				return TerrainMeta.Physics.GetMaterial(pos);
			}
			return obj.get_sharedMaterial();
		}

		public static bool IsOnLayer(this Collider col, Layer rustLayer)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)col != (Object)null)
			{
				return ((Component)col).get_gameObject().IsOnLayer(rustLayer);
			}
			return false;
		}

		public static bool IsOnLayer(this Collider col, int layer)
		{
			if ((Object)(object)col != (Object)null)
			{
				return ((Component)col).get_gameObject().IsOnLayer(layer);
			}
			return false;
		}

		public static Vector3 GetLocalCentre(this Collider col)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			SphereCollider val;
			if ((val = (SphereCollider)(object)((col is SphereCollider) ? col : null)) != null)
			{
				return val.get_center();
			}
			BoxCollider val2;
			if ((val2 = (BoxCollider)(object)((col is BoxCollider) ? col : null)) != null)
			{
				return val2.get_center();
			}
			CapsuleCollider val3;
			if ((val3 = (CapsuleCollider)(object)((col is CapsuleCollider) ? col : null)) != null)
			{
				return val3.get_center();
			}
			_ = col is MeshCollider;
			return Vector3.get_zero();
		}

		public static float GetRadius(this Collider col, Vector3 transformScale)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			float result = 1f;
			SphereCollider val;
			BoxCollider val2;
			CapsuleCollider val3;
			MeshCollider val4;
			if ((val = (SphereCollider)(object)((col is SphereCollider) ? col : null)) != null)
			{
				result = val.get_radius() * Vector3Ex.Max(transformScale);
			}
			else if ((val2 = (BoxCollider)(object)((col is BoxCollider) ? col : null)) != null)
			{
				result = Vector3Ex.Max(Vector3.Scale(val2.get_size(), transformScale)) * 0.5f;
			}
			else if ((val3 = (CapsuleCollider)(object)((col is CapsuleCollider) ? col : null)) != null)
			{
				float num = val3.get_direction() switch
				{
					0 => transformScale.y, 
					1 => transformScale.x, 
					_ => transformScale.x, 
				};
				result = val3.get_radius() * num;
			}
			else if ((val4 = (MeshCollider)(object)((col is MeshCollider) ? col : null)) != null)
			{
				Bounds bounds = ((Collider)val4).get_bounds();
				result = Vector3Ex.Max(Vector3.Scale(((Bounds)(ref bounds)).get_size(), transformScale)) * 0.5f;
			}
			return result;
		}
	}
}
