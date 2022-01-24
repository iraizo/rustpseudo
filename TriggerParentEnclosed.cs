using ConVar;
using UnityEngine;

public class TriggerParentEnclosed : TriggerParent
{
	public enum TriggerMode
	{
		TriggerPoint,
		PivotPoint
	}

	public float Padding;

	[Tooltip("AnyIntersect: Look for any intersection with the trigger. OriginIntersect: Only consider objects in the trigger if their origin is inside")]
	public TriggerMode intersectionMode;

	public bool CheckBoundsOnUnparent;

	private BoxCollider boxCollider;

	protected void OnEnable()
	{
		boxCollider = ((Component)this).GetComponent<BoxCollider>();
	}

	protected override bool ShouldParent(BaseEntity ent)
	{
		if (!base.ShouldParent(ent))
		{
			return false;
		}
		return IsInside(ent, Padding);
	}

	internal override bool SkipOnTriggerExit(Collider collider)
	{
		if (!CheckBoundsOnUnparent)
		{
			return false;
		}
		if (!Debugging.checkparentingtriggers)
		{
			return false;
		}
		BaseEntity baseEntity = collider.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return false;
		}
		return IsInside(baseEntity, 0f);
	}

	private bool IsInside(BaseEntity ent, float padding)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = default(Bounds);
		((Bounds)(ref val))._002Ector(boxCollider.get_center(), boxCollider.get_size());
		if (padding > 0f)
		{
			((Bounds)(ref val)).Expand(padding);
		}
		OBB val2 = default(OBB);
		((OBB)(ref val2))._002Ector(((Component)boxCollider).get_transform(), val);
		Vector3 val3 = ((intersectionMode == TriggerMode.TriggerPoint) ? ent.TriggerPoint() : ent.PivotPoint());
		return ((OBB)(ref val2)).Contains(val3);
	}
}
