namespace UnityEngine
{
	public static class RayEx
	{
		public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			return ((Ray)(ref ray)).get_origin() + Vector3.Dot(pos - ((Ray)(ref ray)).get_origin(), ((Ray)(ref ray)).get_direction()) * ((Ray)(ref ray)).get_direction();
		}

		public static float Distance(this Ray ray, Vector3 pos)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.Cross(((Ray)(ref ray)).get_direction(), pos - ((Ray)(ref ray)).get_origin());
			return ((Vector3)(ref val)).get_magnitude();
		}

		public static float SqrDistance(this Ray ray, Vector3 pos)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.Cross(((Ray)(ref ray)).get_direction(), pos - ((Ray)(ref ray)).get_origin());
			return ((Vector3)(ref val)).get_sqrMagnitude();
		}

		public static bool IsNaNOrInfinity(this Ray r)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (!Vector3Ex.IsNaNOrInfinity(((Ray)(ref r)).get_origin()))
			{
				return Vector3Ex.IsNaNOrInfinity(((Ray)(ref r)).get_direction());
			}
			return true;
		}
	}
}
