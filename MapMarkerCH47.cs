using ProtoBuf;
using UnityEngine;

public class MapMarkerCH47 : MapMarker
{
	private GameObject createdMarker;

	private float GetRotation()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GetParentEntity();
		if (!Object.op_Implicit((Object)(object)baseEntity))
		{
			return 0f;
		}
		Vector3 forward = ((Component)baseEntity).get_transform().get_forward();
		forward.y = 0f;
		((Vector3)(ref forward)).Normalize();
		return Mathf.Atan2(forward.x, 0f - forward.z) * 57.29578f + 180f;
	}

	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.rotation = GetRotation();
		return appMarkerData;
	}
}
