using UnityEngine;

public class SocketMod_InWater : SocketMod
{
	public bool wantsInWater = true;

	public static Phrase WantsWaterPhrase = new Phrase("error_inwater_wants", "Must be placed in water");

	public static Phrase NoWaterPhrase = new Phrase("error_inwater", "Can't be placed in water");

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(Color.get_cyan());
		Gizmos.DrawSphere(Vector3.get_zero(), 0.1f);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = place.position + place.rotation * worldPosition;
		bool flag = WaterLevel.Test(val);
		if (!flag && wantsInWater && GamePhysics.CheckSphere(val, 0.1f, 16, (QueryTriggerInteraction)0))
		{
			flag = true;
		}
		if (flag == wantsInWater)
		{
			return true;
		}
		if (wantsInWater)
		{
			Construction.lastPlacementError = WantsWaterPhrase.get_translated();
		}
		else
		{
			Construction.lastPlacementError = NoWaterPhrase.get_translated();
		}
		return false;
	}
}
