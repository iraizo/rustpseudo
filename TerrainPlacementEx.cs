using UnityEngine;

public static class TerrainPlacementEx
{
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (placements.Length != 0)
		{
			Matrix4x4 localToWorld = Matrix4x4.TRS(pos, rot, scale);
			Matrix4x4 inverse = ((Matrix4x4)(ref localToWorld)).get_inverse();
			for (int i = 0; i < placements.Length; i++)
			{
				placements[i].Apply(localToWorld, inverse);
			}
		}
	}

	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		transform.ApplyTerrainPlacements(placements, transform.get_position(), transform.get_rotation(), transform.get_lossyScale());
	}
}
