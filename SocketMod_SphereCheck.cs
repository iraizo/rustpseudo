using UnityEngine;

public class SocketMod_SphereCheck : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public bool wantsCollide;

	public static Phrase Error_WantsCollideConstruction = new Phrase("error_wantsconstruction", "Must be placed on construction");

	public static Phrase Error_DoesNotWantCollideConstruction = new Phrase("error_doesnotwantconstruction", "Cannot be placed on construction");

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.get_zero(), sphereRadius);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position + place.rotation * worldPosition;
		if (wantsCollide == GamePhysics.CheckSphere(position, sphereRadius, ((LayerMask)(ref layerMask)).get_value(), (QueryTriggerInteraction)0))
		{
			return true;
		}
		bool flag = false;
		Construction.lastPlacementError = "Failed Check: Sphere Test (" + hierachyName + ")";
		if (LayerMask.op_Implicit(layerMask) == 2097152 && wantsCollide)
		{
			Construction.lastPlacementError = Error_WantsCollideConstruction.get_translated();
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
			}
		}
		else if (!wantsCollide && (LayerMask.op_Implicit(layerMask) & 0x200000) == 2097152)
		{
			Construction.lastPlacementError = Error_DoesNotWantCollideConstruction.get_translated();
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
			}
		}
		else
		{
			Construction.lastPlacementError = "Failed Check: Sphere Test (" + hierachyName + ")";
		}
		return false;
	}
}
