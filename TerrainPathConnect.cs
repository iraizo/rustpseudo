using UnityEngine;

public class TerrainPathConnect : MonoBehaviour
{
	public InfrastructureType Type;

	public PathFinder.Point GetPathFinderPoint(int res, Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		PathFinder.Point result = default(PathFinder.Point);
		result.x = Mathf.Clamp((int)(num * (float)res), 0, res - 1);
		result.y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1);
		return result;
	}

	public PathFinder.Point GetPathFinderPoint(int res)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetPathFinderPoint(res, ((Component)this).get_transform().get_position());
	}

	public TerrainPathConnect()
		: this()
	{
	}
}
