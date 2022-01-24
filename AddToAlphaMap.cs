using UnityEngine;

public class AddToAlphaMap : ProceduralObject
{
	public Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.get_one());

	public override void Process()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(((Component)this).get_transform(), bounds);
		Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
		Vector3 point2 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
		Vector3 point3 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
		Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
		TerrainMeta.AlphaMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
		{
			TerrainMeta.AlphaMap.SetAlpha(x, z, 0f);
		});
		GameManager.Destroy((Component)(object)this);
	}
}
