using UnityEngine;

public class DecorSwim : DecorComponent
{
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		pos.y = TerrainMeta.WaterMap.GetHeight(pos);
	}
}
