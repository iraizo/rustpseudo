using UnityEngine;

public class SocketMod_PhysicMaterial : SocketMod
{
	public PhysicMaterial[] ValidMaterials;

	private PhysicMaterial foundMaterial;

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position;
		Vector3 eulerAngles = ((Quaternion)(ref place.rotation)).get_eulerAngles();
		Vector3 val = position + ((Vector3)(ref eulerAngles)).get_normalized() * 0.5f;
		eulerAngles = ((Quaternion)(ref place.rotation)).get_eulerAngles();
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val, -((Vector3)(ref eulerAngles)).get_normalized(), ref val2, 1f, 27328512, (QueryTriggerInteraction)1))
		{
			foundMaterial = ((RaycastHit)(ref val2)).get_collider().GetMaterialAt(((RaycastHit)(ref val2)).get_point());
			PhysicMaterial[] validMaterials = ValidMaterials;
			for (int i = 0; i < validMaterials.Length; i++)
			{
				if ((Object)(object)validMaterials[i] == (Object)(object)foundMaterial)
				{
					return true;
				}
			}
		}
		return false;
	}
}
