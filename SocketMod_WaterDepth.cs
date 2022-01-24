using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_WaterDepth : SocketMod
{
	public float MinimumWaterDepth = 2f;

	public float MaximumWaterDepth = 4f;

	public bool AllowWaterVolumes;

	public static Phrase TooDeepPhrase = new Phrase("error_toodeep", "Water is too deep");

	public static Phrase TooShallowPhrase = new Phrase("error_shallow", "Water is too shallow");

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = place.position + place.rotation * worldPosition;
		if (!AllowWaterVolumes)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(val, 0.5f, list, 262144, (QueryTriggerInteraction)2);
			int count = list.Count;
			Pool.FreeList<WaterVolume>(ref list);
			if (count > 0)
			{
				Construction.lastPlacementError = "Failed Check: WaterDepth_VolumeCheck (" + hierachyName + ")";
				return false;
			}
		}
		val.y = WaterSystem.GetHeight(val) - 0.1f;
		float overallWaterDepth = WaterLevel.GetOverallWaterDepth(val, waves: false);
		if (overallWaterDepth > MinimumWaterDepth && overallWaterDepth < MaximumWaterDepth)
		{
			return true;
		}
		if (overallWaterDepth <= MinimumWaterDepth)
		{
			Construction.lastPlacementError = TooShallowPhrase.get_translated();
		}
		else
		{
			Construction.lastPlacementError = TooDeepPhrase.get_translated();
		}
		return false;
	}
}
