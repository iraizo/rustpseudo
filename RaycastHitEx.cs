using Rust;
using UnityEngine;

public static class RaycastHitEx
{
	public static Transform GetTransform(this RaycastHit hit)
	{
		return ((RaycastHit)(ref hit)).get_transform();
	}

	public static Rigidbody GetRigidbody(this RaycastHit hit)
	{
		return ((RaycastHit)(ref hit)).get_rigidbody();
	}

	public static Collider GetCollider(this RaycastHit hit)
	{
		return ((RaycastHit)(ref hit)).get_collider();
	}

	public static BaseEntity GetEntity(this RaycastHit hit)
	{
		return ((RaycastHit)(ref hit)).get_collider().ToBaseEntity();
	}

	public static bool IsOnLayer(this RaycastHit hit, Layer rustLayer)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((RaycastHit)(ref hit)).get_collider() != (Object)null)
		{
			return ((Component)((RaycastHit)(ref hit)).get_collider()).get_gameObject().IsOnLayer(rustLayer);
		}
		return false;
	}

	public static bool IsOnLayer(this RaycastHit hit, int layer)
	{
		if ((Object)(object)((RaycastHit)(ref hit)).get_collider() != (Object)null)
		{
			return ((Component)((RaycastHit)(ref hit)).get_collider()).get_gameObject().IsOnLayer(layer);
		}
		return false;
	}
}
