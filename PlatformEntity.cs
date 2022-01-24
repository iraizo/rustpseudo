using UnityEngine;

public class PlatformEntity : BaseEntity
{
	private const float movementSpeed = 1f;

	private const float rotationSpeed = 10f;

	private const float radius = 10f;

	private Vector3 targetPosition = Vector3.get_zero();

	private Quaternion targetRotation = Quaternion.get_identity();

	protected void FixedUpdate()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		if (targetPosition == Vector3.get_zero() || Vector3.Distance(((Component)this).get_transform().get_position(), targetPosition) < 0.01f)
		{
			Vector2 val = Random.get_insideUnitCircle() * 10f;
			targetPosition = ((Component)this).get_transform().get_position() + new Vector3(val.x, 0f, val.y);
			if ((Object)(object)TerrainMeta.HeightMap != (Object)null && (Object)(object)TerrainMeta.WaterMap != (Object)null)
			{
				float height = TerrainMeta.HeightMap.GetHeight(targetPosition);
				float height2 = TerrainMeta.WaterMap.GetHeight(targetPosition);
				targetPosition.y = Mathf.Max(height, height2) + 1f;
			}
			targetRotation = Quaternion.LookRotation(targetPosition - ((Component)this).get_transform().get_position());
		}
		((Component)this).get_transform().SetPositionAndRotation(Vector3.MoveTowards(((Component)this).get_transform().get_position(), targetPosition, Time.get_fixedDeltaTime() * 1f), Quaternion.RotateTowards(((Component)this).get_transform().get_rotation(), targetRotation, Time.get_fixedDeltaTime() * 10f));
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}
}
