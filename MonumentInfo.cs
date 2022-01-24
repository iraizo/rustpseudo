using UnityEngine;

public class MonumentInfo : LandmarkInfo, IPrefabPreProcess
{
	[Header("MonumentInfo")]
	public MonumentType Type = MonumentType.Building;

	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	public int MinWorldSize;

	public Bounds Bounds = new Bounds(Vector3.get_zero(), Vector3.get_zero());

	public bool HasNavmesh;

	public bool IsSafeZone;

	[HideInInspector]
	public bool WantsDungeonLink;

	[HideInInspector]
	public bool HasDungeonLink;

	[HideInInspector]
	public DungeonGridInfo DungeonEntrance;

	[HideInInspector]
	public float PreventBuildingRadius;

	[HideInInspector]
	public Vector3 PreventBuildingOrigin;

	protected override void Awake()
	{
		base.Awake();
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.Monuments.Add(this);
		}
	}

	public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(pos, scale, rot, Bounds);
		Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
		Vector3 point2 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
		Vector3 point3 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
		Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
		int topology = TerrainMeta.TopologyMap.GetTopology(point);
		int topology2 = TerrainMeta.TopologyMap.GetTopology(point2);
		int topology3 = TerrainMeta.TopologyMap.GetTopology(point3);
		int topology4 = TerrainMeta.TopologyMap.GetTopology(point4);
		int num = TierToMask(Tier);
		int num2 = 0;
		if ((num & topology) != 0)
		{
			num2++;
		}
		if ((num & topology2) != 0)
		{
			num2++;
		}
		if ((num & topology3) != 0)
		{
			num2++;
		}
		if ((num & topology4) != 0)
		{
			num2++;
		}
		return num2 >= 3;
	}

	public float Distance(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		OBB val = new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Bounds);
		return ((OBB)(ref val)).Distance(position);
	}

	public float SqrDistance(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		OBB val = new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Bounds);
		return ((OBB)(ref val)).SqrDistance(position);
	}

	public float Distance(OBB obb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		OBB val = new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Bounds);
		return ((OBB)(ref val)).Distance(obb);
	}

	public float SqrDistance(OBB obb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		OBB val = new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Bounds);
		return ((OBB)(ref val)).SqrDistance(obb);
	}

	public bool IsInBounds(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		OBB val = new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Bounds);
		return ((OBB)(ref val)).Contains(position);
	}

	public Vector3 ClosestPointOnBounds(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		OBB val = new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_rotation(), Bounds);
		return ((OBB)(ref val)).ClosestPoint(position);
	}

	public PathFinder.Point GetPathFinderPoint(int res)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).get_transform().get_position();
		float num = TerrainMeta.NormalizeX(position.x);
		float num2 = TerrainMeta.NormalizeZ(position.z);
		PathFinder.Point result = default(PathFinder.Point);
		result.x = Mathf.Clamp((int)(num * (float)res), 0, res - 1);
		result.y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1);
		return result;
	}

	public int GetPathFinderRadius(int res)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Bounds)(ref Bounds)).get_extents().x * TerrainMeta.OneOverSize.x;
		float num2 = ((Bounds)(ref Bounds)).get_extents().z * TerrainMeta.OneOverSize.z;
		return Mathf.CeilToInt(Mathf.Max(num, num2) * (float)res);
	}

	protected void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(new Color(0f, 0.7f, 1f, 0.1f));
		Gizmos.DrawCube(((Bounds)(ref Bounds)).get_center(), ((Bounds)(ref Bounds)).get_size());
		Gizmos.set_color(new Color(0f, 0.7f, 1f, 1f));
		Gizmos.DrawWireCube(((Bounds)(ref Bounds)).get_center(), ((Bounds)(ref Bounds)).get_size());
		Gizmos.set_color(Color.get_red());
		Gizmos.DrawWireSphere(PreventBuildingOrigin, PreventBuildingRadius);
	}

	public MonumentNavMesh GetMonumentNavMesh()
	{
		return ((Component)this).GetComponent<MonumentNavMesh>();
	}

	public static int TierToMask(MonumentTier tier)
	{
		int num = 0;
		if ((tier & MonumentTier.Tier0) != 0)
		{
			num |= 0x4000000;
		}
		if ((tier & MonumentTier.Tier1) != 0)
		{
			num |= 0x8000000;
		}
		if ((tier & MonumentTier.Tier2) != 0)
		{
			num |= 0x10000000;
		}
		return num;
	}

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		HasDungeonLink = DetermineHasDungeonLink();
		WantsDungeonLink = DetermineWantsDungeonLink();
		DungeonEntrance = FindDungeonEntrance();
	}

	private DungeonGridInfo FindDungeonEntrance()
	{
		return ((Component)this).GetComponentInChildren<DungeonGridInfo>();
	}

	private bool DetermineHasDungeonLink()
	{
		return (Object)(object)((Component)this).GetComponentInChildren<DungeonGridLink>() != (Object)null;
	}

	private bool DetermineWantsDungeonLink()
	{
		if (Type == MonumentType.WaterWell)
		{
			return false;
		}
		if (Type == MonumentType.Building && displayPhrase.token.StartsWith("mining_quarry"))
		{
			return false;
		}
		if (Type == MonumentType.Radtown && displayPhrase.token.StartsWith("swamp"))
		{
			return false;
		}
		return true;
	}
}
