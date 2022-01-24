using UnityEngine;

public class Socket_Terrain : Socket_Base
{
	public float placementHeight;

	public bool alignToNormal;

	private void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(Color.get_red());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_forward() * 0.2f);
		Gizmos.set_color(Color.get_blue());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_right() * 0.1f);
		Gizmos.set_color(Color.get_green());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_up() * 0.1f);
		Gizmos.set_color(new Color(0f, 1f, 0f, 0.2f));
		Gizmos.DrawCube(Vector3.get_zero(), new Vector3(0.1f, 0.1f, placementHeight));
		Gizmos.set_color(new Color(0f, 1f, 0f, 0.5f));
		Gizmos.DrawWireCube(Vector3.get_zero(), new Vector3(0.1f, 0.1f, placementHeight));
		Gizmos.DrawIcon(((Component)this).get_transform().get_position(), "light_circle_green.png", false);
	}

	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 eulerAngles = ((Quaternion)(ref rotation)).get_eulerAngles();
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		Vector3 direction = ((Ray)(ref target.ray)).get_direction();
		direction.y = 0f;
		((Vector3)(ref direction)).Normalize();
		Vector3 val = Vector3.get_up();
		if (alignToNormal)
		{
			val = target.normal;
		}
		Quaternion val2 = Quaternion.LookRotation(direction, val) * Quaternion.Euler(0f, eulerAngles.y, 0f) * Quaternion.Euler(target.rotation);
		Vector3 val3 = target.position;
		val3 -= val2 * position;
		return new Construction.Placement
		{
			rotation = val2,
			position = val3
		};
	}
}
