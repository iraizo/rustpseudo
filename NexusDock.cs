using UnityEngine;

public class NexusDock : SingletonComponent<NexusDock>
{
	[Header("Targets")]
	public Transform Arrival;

	public Transform Docking;

	public Transform Docked;

	public Transform CastingOff;

	public Transform Departure;

	[Header("Ejection")]
	public BoxCollider EjectionZone;

	public float TraceHeight = 100f;

	public LayerMask TraceLayerMask = LayerMask.op_Implicit(429990145);

	public bool TryFindEjectionPosition(out Vector3 position, float radius = 5f)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)EjectionZone == (Object)null)
		{
			Debug.LogError((object)"EjectionZone is null, cannot find an eject position", (Object)(object)this);
			position = Vector3.get_zero();
			return false;
		}
		Transform transform = ((Component)EjectionZone).get_transform();
		Vector3 size = EjectionZone.get_size();
		float num = transform.get_position().y - size.y / 2f;
		RaycastHit val3 = default(RaycastHit);
		for (int i = 0; i < 10; i++)
		{
			Vector3 val = Vector3Ex.Scale(size, Random.get_value() - 0.5f, 0f, Random.get_value() - 0.5f);
			Vector3 val2 = transform.TransformPoint(val);
			if (Physics.SphereCast(Vector3Ex.WithY(val2, num + TraceHeight), radius, Vector3.get_down(), ref val3, TraceHeight + radius, LayerMask.op_Implicit(TraceLayerMask), (QueryTriggerInteraction)1) && !(((RaycastHit)(ref val3)).get_point().y < val2.y - size.y) && !(((RaycastHit)(ref val3)).get_point().y > val2.y + size.y))
			{
				float height = WaterSystem.GetHeight(val2);
				if (!(((RaycastHit)(ref val3)).get_point().y < height))
				{
					position = ((RaycastHit)(ref val3)).get_point();
					return true;
				}
			}
		}
		position = Vector3.get_zero();
		return false;
	}
}
