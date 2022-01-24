using System;
using UnityEngine;

public class DirectionProperties : PrefabAttribute
{
	private const float radius = 200f;

	public Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_zero());

	public ProtectionProperties extraProtection;

	protected override Type GetIndexedType()
	{
		return typeof(DirectionProperties);
	}

	public bool IsWeakspot(Transform tx, HitInfo info)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (((Bounds)(ref bounds)).get_size() == Vector3.get_zero())
		{
			return false;
		}
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if ((Object)(object)initiatorPlayer == (Object)null)
		{
			return false;
		}
		BaseEntity hitEntity = info.HitEntity;
		if ((Object)(object)hitEntity == (Object)null)
		{
			return false;
		}
		Matrix4x4 worldToLocalMatrix = tx.get_worldToLocalMatrix();
		Vector3 val = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(info.PointStart) - worldPosition;
		float num = Vector3Ex.DotDegrees(worldForward, val);
		Vector3 val2 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(info.HitPositionWorld);
		OBB val3 = default(OBB);
		((OBB)(ref val3))._002Ector(worldPosition, worldRotation, bounds);
		Vector3 position = initiatorPlayer.eyes.position;
		Vector3 target = tx.TransformPoint(val3.position);
		if (!hitEntity.IsVisible(position, target))
		{
			return false;
		}
		if (num > 100f)
		{
			return ((OBB)(ref val3)).Contains(val2);
		}
		return false;
	}
}
