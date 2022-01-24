using UnityEngine;

public class SocketMod_AngleCheck : SocketMod
{
	public bool wantsAngle = true;

	public Vector3 worldNormal = Vector3.get_up();

	public float withinDegrees = 45f;

	public static Phrase ErrorPhrase = new Phrase("error_anglecheck", "Invalid angle");

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(Color.get_yellow());
		Gizmos.DrawFrustum(Vector3.get_zero(), withinDegrees, 1f, 0f, 1f);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.DotDegrees(worldNormal, place.rotation * Vector3.get_up()) < withinDegrees)
		{
			return true;
		}
		Construction.lastPlacementError = ErrorPhrase.get_translated();
		return false;
	}
}
