using UnityEngine;

public class WaterVolume : TriggerBase
{
	public Bounds WaterBounds = new Bounds(Vector3.get_zero(), Vector3.get_one());

	private OBB cachedBounds;

	private Transform cachedTransform;

	public Transform[] cutOffPlanes = (Transform[])(object)new Transform[0];

	public bool waterEnabled = true;

	private void OnEnable()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		cachedTransform = ((Component)this).get_transform();
		cachedBounds = new OBB(cachedTransform, WaterBounds);
	}

	public bool Test(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		UpdateCachedTransform();
		if (((OBB)(ref cachedBounds)).Contains(pos))
		{
			if (!CheckCutOffPlanes(pos))
			{
				info = default(WaterLevel.WaterInfo);
				return false;
			}
			Plane val = default(Plane);
			((Plane)(ref val))._002Ector(cachedBounds.up, cachedBounds.position);
			Vector3 val2 = ((Plane)(ref val)).ClosestPointOnPlane(pos);
			float y = (val2 + cachedBounds.up * cachedBounds.extents.y).y;
			float y2 = (val2 + -cachedBounds.up * cachedBounds.extents.y).y;
			info.isValid = true;
			info.currentDepth = Mathf.Max(0f, y - pos.y);
			info.overallDepth = Mathf.Max(0f, y - y2);
			info.surfaceLevel = y;
			return true;
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	public bool Test(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		UpdateCachedTransform();
		if (((OBB)(ref cachedBounds)).Contains(((Bounds)(ref bounds)).ClosestPoint(cachedBounds.position)))
		{
			if (!CheckCutOffPlanes(((Bounds)(ref bounds)).get_center()))
			{
				info = default(WaterLevel.WaterInfo);
				return false;
			}
			Plane val = default(Plane);
			((Plane)(ref val))._002Ector(cachedBounds.up, cachedBounds.position);
			Vector3 val2 = ((Plane)(ref val)).ClosestPointOnPlane(((Bounds)(ref bounds)).get_center());
			float y = (val2 + cachedBounds.up * cachedBounds.extents.y).y;
			float y2 = (val2 + -cachedBounds.up * cachedBounds.extents.y).y;
			info.isValid = true;
			info.currentDepth = Mathf.Max(0f, y - ((Bounds)(ref bounds)).get_min().y);
			info.overallDepth = Mathf.Max(0f, y - y2);
			info.surfaceLevel = y;
			return true;
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	public bool Test(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (!waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		UpdateCachedTransform();
		Vector3 val = (start + end) * 0.5f;
		float num = Mathf.Min(start.y, end.y) - radius;
		if (((OBB)(ref cachedBounds)).Distance(start) < radius || ((OBB)(ref cachedBounds)).Distance(end) < radius)
		{
			if (!CheckCutOffPlanes(val))
			{
				info = default(WaterLevel.WaterInfo);
				return false;
			}
			Plane val2 = default(Plane);
			((Plane)(ref val2))._002Ector(cachedBounds.up, cachedBounds.position);
			Vector3 val3 = ((Plane)(ref val2)).ClosestPointOnPlane(val);
			float y = (val3 + cachedBounds.up * cachedBounds.extents.y).y;
			float y2 = (val3 + -cachedBounds.up * cachedBounds.extents.y).y;
			info.isValid = true;
			info.currentDepth = Mathf.Max(0f, y - num);
			info.overallDepth = Mathf.Max(0f, y - y2);
			info.surfaceLevel = y;
			return true;
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	private bool CheckCutOffPlanes(Vector3 pos)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		int num = cutOffPlanes.Length;
		bool flag = true;
		for (int i = 0; i < num; i++)
		{
			if ((Object)(object)cutOffPlanes[i] != (Object)null && cutOffPlanes[i].InverseTransformPoint(pos).y > 0f)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		return true;
	}

	private void UpdateCachedTransform()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedTransform != (Object)null && cachedTransform.get_hasChanged())
		{
			cachedBounds = new OBB(cachedTransform, WaterBounds);
			cachedTransform.set_hasChanged(false);
		}
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}
}
