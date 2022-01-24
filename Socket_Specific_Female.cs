using UnityEngine;

public class Socket_Specific_Female : Socket_Base
{
	public int rotationDegrees;

	public int rotationOffset;

	public string[] allowedMaleSockets;

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

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.DrawWireCube(selectCenter, selectSize);
	}

	public bool CanAccept(Socket_Specific socket)
	{
		string[] array = allowedMaleSockets;
		foreach (string text in array)
		{
			if (socket.targetSocketName == text)
			{
				return true;
			}
		}
		return false;
	}
}
