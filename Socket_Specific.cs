using UnityEngine;

public class Socket_Specific : Socket_Base
{
	public bool useFemaleRotation = true;

	public string targetSocketName;

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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(Color.get_red());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_forward() * 0.2f);
		Gizmos.set_color(Color.get_blue());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_right() * 0.1f);
		Gizmos.set_color(Color.get_green());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_up() * 0.1f);
		Gizmos.DrawIcon(((Component)this).get_transform().get_position(), "light_circle_green.png", false);
	}

	public override bool TestTarget(Construction.Target target)
	{
		if (!base.TestTarget(target))
		{
			return false;
		}
		Socket_Specific_Female socket_Specific_Female = target.socket as Socket_Specific_Female;
		if (socket_Specific_Female == null)
		{
			return false;
		}
		return socket_Specific_Female.CanAccept(this);
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = target.socket.rotation;
		if (target.socket.male && target.socket.female)
		{
			val = target.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
		}
		Transform transform = ((Component)target.entity).get_transform();
		Matrix4x4 localToWorldMatrix = transform.get_localToWorldMatrix();
		Vector3 val2 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(target.socket.position);
		Quaternion val3;
		if (useFemaleRotation)
		{
			val3 = transform.get_rotation() * val;
		}
		else
		{
			Vector3 val4 = new Vector3(val2.x, 0f, val2.z);
			Vector3 val5 = default(Vector3);
			((Vector3)(ref val5))._002Ector(target.player.eyes.position.x, 0f, target.player.eyes.position.z);
			Vector3 val6 = val4 - val5;
			val3 = Quaternion.LookRotation(((Vector3)(ref val6)).get_normalized()) * val;
		}
		Construction.Placement placement = new Construction.Placement();
		Quaternion val7 = val3 * Quaternion.Inverse(rotation);
		Vector3 val8 = val7 * position;
		placement.position = val2 - val8;
		placement.rotation = val7;
		return placement;
	}
}
