using System;
using UnityEngine;

[Serializable]
public struct SubsurfaceProfileData
{
	[Range(0.1f, 50f)]
	public float ScatterRadius;

	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color SubsurfaceColor;

	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color FalloffColor;

	public static SubsurfaceProfileData Default
	{
		get
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			SubsurfaceProfileData result = default(SubsurfaceProfileData);
			result.ScatterRadius = 1.2f;
			result.SubsurfaceColor = new Color(0.48f, 0.41f, 0.28f);
			result.FalloffColor = new Color(1f, 0.37f, 0.3f);
			return result;
		}
	}

	public static SubsurfaceProfileData Invalid
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			SubsurfaceProfileData result = default(SubsurfaceProfileData);
			result.ScatterRadius = 0f;
			result.SubsurfaceColor = Color.get_clear();
			result.FalloffColor = Color.get_clear();
			return result;
		}
	}
}
