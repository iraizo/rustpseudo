using UnityEngine;

public class ValidBounds : SingletonComponent<ValidBounds>
{
	public Bounds worldBounds;

	public static bool Test(Vector3 vPos)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)SingletonComponent<ValidBounds>.Instance))
		{
			return true;
		}
		return SingletonComponent<ValidBounds>.Instance.IsInside(vPos);
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_blue());
		Gizmos.DrawWireCube(((Bounds)(ref worldBounds)).get_center(), ((Bounds)(ref worldBounds)).get_size());
	}

	internal bool IsInside(Vector3 vPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.IsNaNOrInfinity(vPos))
		{
			return false;
		}
		if (!((Bounds)(ref worldBounds)).Contains(vPos))
		{
			return false;
		}
		if ((Object)(object)TerrainMeta.Terrain != (Object)null)
		{
			if (World.Procedural && vPos.y < TerrainMeta.Position.y)
			{
				return false;
			}
			if (TerrainMeta.OutOfMargin(vPos))
			{
				return false;
			}
		}
		return true;
	}
}
