using System.Collections.Generic;
using UnityEngine;

public class DungeonGridInfo : LandmarkInfo
{
	[Header("DungeonGridInfo")]
	public int CellSize = 216;

	public float LinkHeight = 1.5f;

	public float LinkRadius = 3f;

	internal List<GameObject> Links = new List<GameObject>();

	public float Distance(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).get_transform().get_position() - position;
		return ((Vector3)(ref val)).get_magnitude();
	}

	public float SqrDistance(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).get_transform().get_position() - position;
		return ((Vector3)(ref val)).get_sqrMagnitude();
	}

	public bool IsValidSpawnPosition(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		OBB bounds = ((Component)this).GetComponentInChildren<DungeonVolume>().GetBounds(position, Quaternion.get_identity());
		Vector3 val = WorldSpaceGrid.ClosestGridCell(bounds.position, TerrainMeta.Size.x * 2f, (float)CellSize);
		Vector3 val2 = bounds.position - val;
		if (!(Mathf.Abs(val2.x) > 3f))
		{
			return Mathf.Abs(val2.z) > 3f;
		}
		return true;
	}

	public Vector3 SnapPosition(Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		pos.x = (float)Mathf.RoundToInt(pos.x / LinkRadius) * LinkRadius;
		pos.y = (float)Mathf.CeilToInt(pos.y / LinkHeight) * LinkHeight;
		pos.z = (float)Mathf.RoundToInt(pos.z / LinkRadius) * LinkRadius;
		return pos;
	}

	protected override void Awake()
	{
		base.Awake();
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.DungeonGridEntrances.Add(this);
		}
	}

	protected void Start()
	{
		((Component)this).get_transform().SetHierarchyGroup("Dungeon");
	}
}
