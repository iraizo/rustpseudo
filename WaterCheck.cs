using System;
using UnityEngine;

public class WaterCheck : PrefabAttribute
{
	public bool Rotate = true;

	protected void OnDrawGizmosSelected()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(new Color(0f, 0f, 0.5f, 1f));
		Gizmos.DrawSphere(((Component)this).get_transform().get_position(), 1f);
	}

	public bool Check(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return pos.y <= TerrainMeta.WaterMap.GetHeight(pos);
	}

	protected override Type GetIndexedType()
	{
		return typeof(WaterCheck);
	}
}
