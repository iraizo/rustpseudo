using UnityEngine;

public class ConstructionSocket : Socket_Base
{
	public enum Type
	{
		None,
		Foundation,
		Floor,
		Misc,
		Doorway,
		Wall,
		Block,
		Ramp,
		StairsTriangle,
		Stairs,
		FloorFrameTriangle,
		Window,
		Shutters,
		WallFrame,
		FloorFrame,
		WindowDressing,
		DoorDressing,
		Elevator,
		DoubleDoorDressing
	}

	public Type socketType;

	public int rotationDegrees;

	public int rotationOffset;

	public bool restrictPlacementRotation;

	public bool restrictPlacementAngle;

	public float faceAngle;

	public float angleAllowed = 150f;

	[Range(0f, 1f)]
	public float support = 1f;

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
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_forward() * 0.6f);
		Gizmos.set_color(Color.get_blue());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_right() * 0.1f);
		Gizmos.set_color(Color.get_green());
		Gizmos.DrawLine(Vector3.get_zero(), Vector3.get_up() * 0.1f);
		Gizmos.DrawIcon(((Component)this).get_transform().get_position(), "light_circle_green.png", false);
	}

	private void OnDrawGizmosSelected()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (female)
		{
			Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
			Gizmos.DrawWireCube(selectCenter, selectSize);
		}
	}

	public override bool TestTarget(Construction.Target target)
	{
		if (!base.TestTarget(target))
		{
			return false;
		}
		return IsCompatible(target.socket);
	}

	public override bool IsCompatible(Socket_Base socket)
	{
		if (!base.IsCompatible(socket))
		{
			return false;
		}
		ConstructionSocket constructionSocket = socket as ConstructionSocket;
		if (constructionSocket == null)
		{
			return false;
		}
		if (constructionSocket.socketType == Type.None || socketType == Type.None)
		{
			return false;
		}
		if (constructionSocket.socketType != socketType)
		{
			return false;
		}
		return true;
	}

	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 val = Matrix4x4.TRS(position, rotation, Vector3.get_one());
		Matrix4x4 val2 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.get_one());
		Vector3 val3 = ((Matrix4x4)(ref val)).MultiplyPoint3x4(worldPosition);
		Vector3 val4 = ((Matrix4x4)(ref val2)).MultiplyPoint3x4(socket.worldPosition);
		if (Vector3.Distance(val3, val4) > 0.01f)
		{
			return false;
		}
		Vector3 val5 = ((Matrix4x4)(ref val)).MultiplyVector(worldRotation * Vector3.get_forward());
		Vector3 val6 = ((Matrix4x4)(ref val2)).MultiplyVector(socket.worldRotation * Vector3.get_forward());
		float num = Vector3.Angle(val5, val6);
		if (male && female)
		{
			num = Mathf.Min(num, Vector3.Angle(-val5, val6));
		}
		if (socket.male && socket.female)
		{
			num = Mathf.Min(num, Vector3.Angle(val5, -val6));
		}
		if (num > 1f)
		{
			return false;
		}
		return true;
	}

	public bool TestRestrictedAngles(Vector3 suggestedPos, Quaternion suggestedAng, Construction.Target target)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (restrictPlacementAngle)
		{
			Quaternion val = Quaternion.Euler(0f, faceAngle, 0f) * suggestedAng;
			float num = Vector3Ex.DotDegrees(Vector3Ex.XZ3D(((Ray)(ref target.ray)).get_direction()), val * Vector3.get_forward());
			if (num > angleAllowed * 0.5f)
			{
				return false;
			}
			if (num < angleAllowed * -0.5f)
			{
				return false;
			}
		}
		return true;
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)target.entity) || !Object.op_Implicit((Object)(object)((Component)target.entity).get_transform()))
		{
			return null;
		}
		if (!CanConnectToEntity(target))
		{
			return null;
		}
		ConstructionSocket constructionSocket = target.socket as ConstructionSocket;
		Vector3 val = target.GetWorldPosition();
		Quaternion val2 = target.GetWorldRotation(female: true);
		if (constructionSocket != null && !IsCompatible(constructionSocket))
		{
			return null;
		}
		if (rotationDegrees > 0 && (constructionSocket == null || !constructionSocket.restrictPlacementRotation))
		{
			Construction.Placement placement = new Construction.Placement();
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < 360; i += rotationDegrees)
			{
				Quaternion val3 = Quaternion.Euler(0f, (float)(rotationOffset + i), 0f);
				Vector3 direction = ((Ray)(ref target.ray)).get_direction();
				Vector3 val4 = val3 * val2 * Vector3.get_up();
				float num3 = Vector3.Angle(direction, val4);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			for (int j = 0; j < 360; j += rotationDegrees)
			{
				Quaternion val5 = val2 * Quaternion.Inverse(rotation);
				Quaternion val6 = Quaternion.Euler(target.rotation);
				Quaternion val7 = Quaternion.Euler(0f, (float)(rotationOffset + j) + num2, 0f);
				Quaternion val8 = val6 * val7 * val5;
				Vector3 val9 = val8 * position;
				placement.position = val - val9;
				placement.rotation = val8;
				if (CheckSocketMods(placement))
				{
					return placement;
				}
			}
		}
		Construction.Placement placement2 = new Construction.Placement();
		Quaternion val10 = val2 * Quaternion.Inverse(rotation);
		Vector3 val11 = val10 * position;
		placement2.position = val - val11;
		placement2.rotation = val10;
		if (!TestRestrictedAngles(val, val2, target))
		{
			return null;
		}
		return placement2;
	}

	protected virtual bool CanConnectToEntity(Construction.Target target)
	{
		return true;
	}
}
