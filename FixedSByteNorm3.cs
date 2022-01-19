using UnityEngine;

public struct FixedSByteNorm3
{
	private const int FracBits = 7;

	private const float MaxFrac = 128f;

	private const float RcpMaxFrac = 0.0078125f;

	public sbyte x;

	public sbyte y;

	public sbyte z;

	public FixedSByteNorm3(Vector3 vec)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		x = (sbyte)(vec.x * 128f);
		y = (sbyte)(vec.y * 128f);
		z = (sbyte)(vec.z * 128f);
	}

	public static explicit operator Vector3(FixedSByteNorm3 vec)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f);
	}
}
