using System;
using UnityEngine;

public class PathInterpolator
{
	public Vector3[] Points;

	public Vector3[] Tangents;

	protected bool initialized;

	public int MinIndex { get; set; }

	public int MaxIndex { get; set; }

	public virtual float Length { get; private set; }

	public virtual float StepSize { get; private set; }

	public bool Circular { get; private set; }

	public int DefaultMinIndex => 0;

	public int DefaultMaxIndex => Points.Length - 1;

	public float StartOffset => Length * (float)(MinIndex - DefaultMinIndex) / (float)(DefaultMaxIndex - DefaultMinIndex);

	public float EndOffset => Length * (float)(DefaultMaxIndex - MaxIndex) / (float)(DefaultMaxIndex - DefaultMinIndex);

	public PathInterpolator(Vector3[] points)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (points.Length < 2)
		{
			throw new ArgumentException("Point list too short.");
		}
		Points = points;
		MinIndex = DefaultMinIndex;
		MaxIndex = DefaultMaxIndex;
		Circular = Vector3.Distance(points[0], points[points.Length - 1]) < 0.1f;
	}

	public PathInterpolator(Vector3[] points, Vector3[] tangents)
		: this(points)
	{
		if (tangents.Length != points.Length)
		{
			throw new ArgumentException("Points and tangents lengths must match. Points: " + points.Length + " Tangents: " + tangents.Length);
		}
		Tangents = tangents;
		RecalculateLength();
		initialized = true;
	}

	public void RecalculateTangents()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (Tangents == null || Tangents.Length != Points.Length)
		{
			Tangents = (Vector3[])(object)new Vector3[Points.Length];
		}
		for (int i = 0; i < Points.Length; i++)
		{
			int num = i - 1;
			int num2 = i + 1;
			if (num < 0)
			{
				num = (Circular ? (Points.Length - 2) : 0);
			}
			if (num2 > Points.Length - 1)
			{
				num2 = (Circular ? 1 : (Points.Length - 1));
			}
			Vector3 val = Points[num];
			Vector3 val2 = Points[num2];
			Vector3[] tangents = Tangents;
			int num3 = i;
			Vector3 val3 = val2 - val;
			tangents[num3] = ((Vector3)(ref val3)).get_normalized();
		}
		RecalculateLength();
		initialized = true;
	}

	protected virtual void RecalculateLength()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < Points.Length - 1; i++)
		{
			Vector3 val = Points[i];
			Vector3 val2 = Points[i + 1];
			float num2 = num;
			Vector3 val3 = val2 - val;
			num = num2 + ((Vector3)(ref val3)).get_magnitude();
		}
		Length = num;
		StepSize = num / (float)Points.Length;
	}

	public void Resample(float distance)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		Vector3[] array = (Vector3[])(object)new Vector3[Mathf.RoundToInt(Length / distance)];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = GetPointCubicHermite((float)i * distance);
		}
		Points = array;
		initialized = false;
	}

	public void Smoothen(int iterations)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Smoothen(iterations, Vector3.get_one());
	}

	public void Smoothen(int iterations, Vector3 multipliers)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < iterations; i++)
		{
			for (int j = MinIndex + ((!Circular) ? 1 : 0); j <= MaxIndex - 1; j += 2)
			{
				SmoothenIndex(j, multipliers);
			}
			for (int k = MinIndex + (Circular ? 1 : 2); k <= MaxIndex - 1; k += 2)
			{
				SmoothenIndex(k, multipliers);
			}
		}
		initialized = false;
	}

	private void SmoothenIndex(int i, Vector3 multipliers)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		int num = i - 1;
		int num2 = i + 1;
		if (i == 0)
		{
			num = Points.Length - 2;
		}
		Vector3 val = Points[num];
		Vector3 val2 = Points[i];
		Vector3 val3 = Points[num2];
		Vector3 val4 = (val + val2 + val2 + val3) * 0.25f;
		if (multipliers != Vector3.get_one())
		{
			val4.x = Mathf.LerpUnclamped(val2.x, val4.x, multipliers.x);
			val4.y = Mathf.LerpUnclamped(val2.y, val4.y, multipliers.y);
			val4.z = Mathf.LerpUnclamped(val2.z, val4.z, multipliers.z);
		}
		Points[i] = val4;
		if (i == 0)
		{
			Points[Points.Length - 1] = Points[0];
		}
	}

	public Vector3 GetStartPoint()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Points[MinIndex];
	}

	public Vector3 GetEndPoint()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Points[MaxIndex];
	}

	public Vector3 GetStartTangent()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return Tangents[MinIndex];
	}

	public Vector3 GetEndTangent()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return Tangents[MaxIndex];
	}

	public Vector3 GetPoint(float distance)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		float num = distance / Length * (float)(Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)MinIndex)
		{
			return GetStartPoint();
		}
		if (num >= (float)MaxIndex)
		{
			return GetEndPoint();
		}
		Vector3 val = Points[num2];
		Vector3 val2 = Points[num2 + 1];
		float num3 = num - (float)num2;
		return Vector3.Lerp(val, val2, num3);
	}

	public virtual Vector3 GetTangent(float distance)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		float num = distance / Length * (float)(Tangents.Length - 1);
		int num2 = (int)num;
		if (num <= (float)MinIndex)
		{
			return GetStartTangent();
		}
		if (num >= (float)MaxIndex)
		{
			return GetEndTangent();
		}
		Vector3 val = Tangents[num2];
		Vector3 val2 = Tangents[num2 + 1];
		float num3 = num - (float)num2;
		return Vector3.Slerp(val, val2, num3);
	}

	public virtual Vector3 GetPointCubicHermite(float distance)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (Length == 0f)
		{
			return GetStartPoint();
		}
		float num = distance / Length * (float)(Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)MinIndex)
		{
			return GetStartPoint();
		}
		if (num >= (float)MaxIndex)
		{
			return GetEndPoint();
		}
		Vector3 val = Points[num2];
		Vector3 val2 = Points[num2 + 1];
		Vector3 val3 = Tangents[num2] * StepSize;
		Vector3 val4 = Tangents[num2 + 1] * StepSize;
		float num3 = num - (float)num2;
		float num4 = num3 * num3;
		float num5 = num3 * num4;
		return (2f * num5 - 3f * num4 + 1f) * val + (num5 - 2f * num4 + num3) * val3 + (-2f * num5 + 3f * num4) * val2 + (num5 - num4) * val4;
	}
}
