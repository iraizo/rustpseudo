using System;
using UnityEngine;

public class BasePathFinder
{
	private static Vector3[] preferedTopologySamples = (Vector3[])(object)new Vector3[4];

	private static Vector3[] topologySamples = (Vector3[])(object)new Vector3[4];

	private Vector3 chosenPosition;

	private const float halfPI = (float)Math.PI / 180f;

	public virtual Vector3 GetRandomPatrolPoint()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.get_zero();
	}

	public virtual AIMovePoint GetBestRoamPoint(Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float anchorClampDistance, float lookupMaxRange = 20f)
	{
		return null;
	}

	public void DebugDraw()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Color color = Gizmos.get_color();
		Gizmos.set_color(Color.get_green());
		Gizmos.DrawSphere(chosenPosition, 5f);
		Gizmos.set_color(Color.get_blue());
		Vector3[] array = topologySamples;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawSphere(array[i], 2.5f);
		}
		Gizmos.set_color(color);
	}

	public virtual Vector3 GetRandomPositionAround(Vector3 position, float minDistFrom = 0f, float maxDistFrom = 2f)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (maxDistFrom < 0f)
		{
			maxDistFrom = 0f;
		}
		Vector2 val = Random.get_insideUnitCircle() * maxDistFrom;
		float num = Mathf.Clamp(Mathf.Max(Mathf.Abs(val.x), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(val.x);
		float num2 = Mathf.Clamp(Mathf.Max(Mathf.Abs(val.y), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(val.y);
		return position + new Vector3(num, 0f, num2);
	}

	public virtual Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		float radius = Random.Range(minRange, maxRange);
		int num = 0;
		int num2 = 0;
		float num3 = Random.Range(0f, 90f);
		for (float num4 = 0f; num4 < 360f; num4 += 90f)
		{
			Vector3 pointOnCircle = GetPointOnCircle(((Component)navigator).get_transform().get_position(), radius, num4 + num3);
			if (navigator.GetNearestNavmeshPosition(pointOnCircle, out var position, 10f) && navigator.IsAcceptableWaterDepth(position))
			{
				topologySamples[num] = position;
				num++;
				if (navigator.IsPositionATopologyPreference(position))
				{
					preferedTopologySamples[num2] = position;
					num2++;
				}
			}
		}
		if (Random.Range(0f, 1f) <= 0.9f && num2 > 0)
		{
			chosenPosition = preferedTopologySamples[Random.Range(0, num2)];
		}
		else if (num > 0)
		{
			chosenPosition = topologySamples[Random.Range(0, num)];
		}
		else
		{
			chosenPosition = fallbackPos;
		}
		return chosenPosition;
	}

	public virtual bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)fleeFrom == (Object)null)
		{
			result = ((Component)navigator).get_transform().get_position();
			return false;
		}
		Vector3 dirFromThreat = Vector3Ex.Direction2D(((Component)navigator).get_transform().get_position(), ((Component)fleeFrom).get_transform().get_position());
		if (TestFleeDirection(navigator, dirFromThreat, 0f, minRange, maxRange, out result))
		{
			return true;
		}
		bool flag = Random.Range(0, 2) == 1;
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 45f : 315f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 315f : 45f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 90f : 270f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 270f : 90f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, 135f + Random.Range(0f, 90f), minRange, maxRange, out result))
		{
			return true;
		}
		return false;
	}

	private bool TestFleeDirection(BaseNavigator navigator, Vector3 dirFromThreat, float offsetDegrees, float minRange, float maxRange, out Vector3 result)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		result = ((Component)navigator).get_transform().get_position();
		Vector3 val = Quaternion.Euler(0f, offsetDegrees, 0f) * dirFromThreat;
		Vector3 target = ((Component)navigator).get_transform().get_position() + val * Random.Range(minRange, maxRange);
		if (!navigator.GetNearestNavmeshPosition(target, out var position, 20f))
		{
			return false;
		}
		if (!navigator.IsAcceptableWaterDepth(position))
		{
			return false;
		}
		result = position;
		return true;
	}

	public static Vector3 GetPointOnCircle(Vector3 center, float radius, float degrees)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float num = center.x + radius * Mathf.Cos(degrees * ((float)Math.PI / 180f));
		float num2 = center.z + radius * Mathf.Sin(degrees * ((float)Math.PI / 180f));
		return new Vector3(num, center.y, num2);
	}
}
