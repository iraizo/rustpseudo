using UnityEngine;

public class AimConeUtil
{
	public static Vector3 GetModifiedAimConeDirection(float aimCone, Vector3 inputVec, bool anywhereInside = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(inputVec);
		Vector2 val2;
		if (!anywhereInside)
		{
			Vector2 insideUnitCircle = Random.get_insideUnitCircle();
			val2 = ((Vector2)(ref insideUnitCircle)).get_normalized();
		}
		else
		{
			val2 = Random.get_insideUnitCircle();
		}
		Vector2 val3 = val2;
		return val * Quaternion.Euler(val3.x * aimCone * 0.5f, val3.y * aimCone * 0.5f, 0f) * Vector3.get_forward();
	}

	public static Quaternion GetAimConeQuat(float aimCone)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Vector3 insideUnitSphere = Random.get_insideUnitSphere();
		return Quaternion.Euler(insideUnitSphere.x * aimCone * 0.5f, insideUnitSphere.y * aimCone * 0.5f, 0f);
	}
}
