using UnityEngine;

public class SocketMod_TerrainCheck : SocketMod
{
	public bool wantsInTerrain = true;

	private void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		bool flag = IsInTerrain(((Component)this).get_transform().get_position());
		if (!wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.set_color(flag ? Color.get_green() : Color.get_red());
		Gizmos.DrawSphere(Vector3.get_zero(), 0.1f);
	}

	public static bool IsInTerrain(Vector3 vPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!Object.op_Implicit((Object)(object)TerrainMeta.Collision) || !TerrainMeta.Collision.GetIgnore(vPoint))
		{
			Terrain[] activeTerrains = Terrain.get_activeTerrains();
			foreach (Terrain val in activeTerrains)
			{
				if (val.SampleHeight(vPoint) + ((Component)val).get_transform().get_position().y > vPoint.y)
				{
					return true;
				}
			}
		}
		if (Physics.Raycast(new Ray(vPoint + Vector3.get_up() * 3f, Vector3.get_down()), 3f, 65536))
		{
			return true;
		}
		return false;
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (IsInTerrain(place.position + place.rotation * worldPosition) == wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = fullName + ": not in terrain";
		return false;
	}
}
