using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldSplineData
{
	public Vector3[] inputPoints;

	public Vector3[] inputTangents;

	public float inputLUTInterval;

	public List<float> LUTDistanceKeys;

	public List<Vector3> LUTPosValues;

	public float Length;

	[SerializeField]
	private int maxPointsIndex;

	public Vector3 GetStartPoint()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return inputPoints[0];
	}

	public Vector3 GetEndPoint()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return inputPoints[maxPointsIndex];
	}

	public Vector3 GetStartTangent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return inputTangents[0];
	}

	public Vector3 GetEndTangent()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return inputTangents[maxPointsIndex];
	}

	public Vector3 GetPointCubicHermite(float distance)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (distance < 0f)
		{
			return GetStartPoint();
		}
		if (distance > Length)
		{
			return GetEndPoint();
		}
		Vector3 val = GetStartPoint();
		float num = 0f;
		for (int i = 0; i < LUTDistanceKeys.Count; i++)
		{
			float num2 = LUTDistanceKeys[i];
			if (num2 > distance)
			{
				float num3 = Mathf.InverseLerp(num, num2, distance);
				return Vector3.Lerp(val, LUTPosValues[i], num3);
			}
			num = num2;
			val = LUTPosValues[i];
		}
		return GetEndPoint();
	}

	public virtual Vector3 GetTangent(float distance)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		float num = distance / Length * (float)(inputTangents.Length - 1);
		int num2 = (int)num;
		if (num <= 0f)
		{
			return GetStartTangent();
		}
		if (num >= (float)maxPointsIndex)
		{
			return GetEndTangent();
		}
		Vector3 val = inputTangents[num2];
		Vector3 val2 = inputTangents[num2 + 1];
		float num3 = num - (float)num2;
		return Vector3.Slerp(val, val2, num3);
	}

	public void SetDefaultTangents(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		pathInterpolator.RecalculateTangents();
		worldSpline.tangents = pathInterpolator.Tangents;
	}

	private void CreateLookupTable(WorldSpline worldSpline)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		Vector3 val = pathInterpolator.GetPointCubicHermite(0f);
		Length = 0f;
		AddEntry(0f, GetStartPoint());
		Vector3 pointCubicHermite;
		for (float num = worldSpline.lutInterval; num < pathInterpolator.Length; num += worldSpline.lutInterval)
		{
			pointCubicHermite = pathInterpolator.GetPointCubicHermite(num);
			Length += Vector3.Distance(pointCubicHermite, val);
			AddEntry(Length, pathInterpolator.GetPointCubicHermite(num));
			val = pointCubicHermite;
		}
		pointCubicHermite = GetEndPoint();
		Length += Vector3.Distance(pointCubicHermite, val);
		AddEntry(Length, pointCubicHermite);
	}

	private void AddEntry(float distance, Vector3 pos)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!LUTDistanceKeys.Contains(distance))
		{
			LUTDistanceKeys.Add(distance);
			LUTPosValues.Add(pos);
		}
	}
}
