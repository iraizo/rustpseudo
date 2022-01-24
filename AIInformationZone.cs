using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

public class AIInformationZone : BaseMonoBehaviour, IServerComponent
{
	public bool ShouldSleepAI;

	public bool Virtual;

	public bool UseCalculatedCoverDistances = true;

	public static List<AIInformationZone> zones = new List<AIInformationZone>();

	public List<AICoverPoint> coverPoints = new List<AICoverPoint>();

	public List<AIMovePoint> movePoints = new List<AIMovePoint>();

	private AICoverPoint[] coverPointArray;

	private AIMovePoint[] movePointArray;

	public List<NavMeshLink> navMeshLinks = new List<NavMeshLink>();

	public List<AIMovePointPath> paths = new List<AIMovePointPath>();

	public Bounds bounds;

	private AIInformationGrid grid;

	private List<IAISleepable> sleepables = new List<IAISleepable>();

	private OBB areaBox;

	private bool isDirty = true;

	private int processIndex;

	private int halfPaths;

	private int pathSuccesses;

	private int pathFails;

	private static bool lastFrameAnyDirty = false;

	private static float rebuildStartTime = 0f;

	public static float buildTimeTest = 0f;

	private static float lastNavmeshBuildTime = 0f;

	public bool Sleeping { get; private set; }

	public int SleepingCount
	{
		get
		{
			if (!Sleeping)
			{
				return 0;
			}
			return sleepables.Count;
		}
	}

	public static AIInformationZone Merge(List<AIInformationZone> zones, GameObject newRoot)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		AIInformationZone aIInformationZone = newRoot.AddComponent<AIInformationZone>();
		aIInformationZone.UseCalculatedCoverDistances = false;
		foreach (AIInformationZone zone in zones)
		{
			if ((Object)(object)zone == (Object)null)
			{
				continue;
			}
			foreach (AIMovePoint movePoint in zone.movePoints)
			{
				aIInformationZone.AddMovePoint(movePoint);
				((Component)movePoint).get_transform().SetParent(newRoot.get_transform());
			}
			foreach (AICoverPoint coverPoint in zone.coverPoints)
			{
				aIInformationZone.AddCoverPoint(coverPoint);
				((Component)coverPoint).get_transform().SetParent(newRoot.get_transform());
			}
		}
		aIInformationZone.bounds = EncapsulateBounds(zones);
		ref Bounds reference = ref aIInformationZone.bounds;
		((Bounds)(ref reference)).set_extents(((Bounds)(ref reference)).get_extents() + new Vector3(5f, 0f, 5f));
		ref Bounds reference2 = ref aIInformationZone.bounds;
		((Bounds)(ref reference2)).set_center(((Bounds)(ref reference2)).get_center() - ((Component)aIInformationZone).get_transform().get_position());
		for (int num = zones.Count - 1; num >= 0; num--)
		{
			AIInformationZone aIInformationZone2 = zones[num];
			if (!((Object)(object)aIInformationZone2 == (Object)null))
			{
				Object.Destroy((Object)(object)aIInformationZone2);
			}
		}
		return aIInformationZone;
	}

	public static Bounds EncapsulateBounds(List<AIInformationZone> zones)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		((Bounds)(ref result)).set_center(((Component)zones[0]).get_transform().get_position());
		foreach (AIInformationZone zone in zones)
		{
			if (!((Object)(object)zone == (Object)null))
			{
				Vector3 center = ((Bounds)(ref zone.bounds)).get_center() + ((Component)zone).get_transform().get_position();
				Bounds val = zone.bounds;
				((Bounds)(ref val)).set_center(center);
				((Bounds)(ref result)).Encapsulate(val);
			}
		}
		return result;
	}

	public void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		AddInitialPoints();
		areaBox = new OBB(((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_lossyScale(), ((Component)this).get_transform().get_rotation(), bounds);
		zones.Add(this);
		grid = ((Component)this).GetComponent<AIInformationGrid>();
		if ((Object)(object)grid != (Object)null)
		{
			grid.Init();
		}
	}

	public void RegisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable != null && sleepable.AllowedToSleep() && !sleepables.Contains(sleepable))
		{
			sleepables.Add(sleepable);
			if (Sleeping && sleepable.AllowedToSleep())
			{
				sleepable.SleepAI();
			}
		}
	}

	public void UnregisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable != null)
		{
			sleepables.Remove(sleepable);
		}
	}

	public void SleepAI()
	{
		if (!AI.sleepwake || !ShouldSleepAI)
		{
			return;
		}
		foreach (IAISleepable sleepable in sleepables)
		{
			sleepable?.SleepAI();
		}
		Sleeping = true;
	}

	public void WakeAI()
	{
		foreach (IAISleepable sleepable in sleepables)
		{
			sleepable?.WakeAI();
		}
		Sleeping = false;
	}

	private void AddCoverPoint(AICoverPoint point)
	{
		if (!coverPoints.Contains(point))
		{
			coverPoints.Add(point);
			MarkDirty();
		}
	}

	private void RemoveCoverPoint(AICoverPoint point, bool markDirty = true)
	{
		coverPoints.Remove(point);
		if (markDirty)
		{
			MarkDirty();
		}
	}

	private void AddMovePoint(AIMovePoint point)
	{
		if (!movePoints.Contains(point))
		{
			movePoints.Add(point);
			MarkDirty();
		}
	}

	private void RemoveMovePoint(AIMovePoint point, bool markDirty = true)
	{
		movePoints.Remove(point);
		if (markDirty)
		{
			MarkDirty();
		}
	}

	public void MarkDirty(bool completeRefresh = false)
	{
		isDirty = true;
		processIndex = 0;
		halfPaths = 0;
		pathSuccesses = 0;
		pathFails = 0;
		if (!completeRefresh)
		{
			return;
		}
		Debug.Log((object)"AIInformationZone performing complete refresh, please wait...");
		foreach (AIMovePoint movePoint in movePoints)
		{
			movePoint.distances.Clear();
			movePoint.distancesToCover.Clear();
		}
	}

	private bool PassesBudget(float startTime, float budgetSeconds)
	{
		if (Time.get_realtimeSinceStartup() - startTime > budgetSeconds)
		{
			return false;
		}
		return true;
	}

	public bool ProcessDistancesAttempt()
	{
		return true;
	}

	private bool ProcessDistances()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Invalid comparison between Unknown and I4
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		if (!UseCalculatedCoverDistances)
		{
			return true;
		}
		float realtimeSinceStartup = Time.get_realtimeSinceStartup();
		float budgetSeconds = AIThinkManager.framebudgetms / 1000f * 0.25f;
		if (realtimeSinceStartup < lastNavmeshBuildTime + 60f)
		{
			budgetSeconds = 0.1f;
		}
		int num = 1 << NavMesh.GetAreaFromName("HumanNPC");
		NavMeshPath val = new NavMeshPath();
		while (PassesBudget(realtimeSinceStartup, budgetSeconds))
		{
			AIMovePoint aIMovePoint = movePoints[processIndex];
			bool flag = true;
			int num2 = 0;
			for (int num3 = aIMovePoint.distances.get_Keys().get_Count() - 1; num3 >= 0; num3--)
			{
				AIMovePoint aIMovePoint2 = aIMovePoint.distances.get_Keys().get_Item(num3);
				if (!movePoints.Contains(aIMovePoint2))
				{
					aIMovePoint.distances.Remove(aIMovePoint2);
				}
			}
			for (int num4 = aIMovePoint.distancesToCover.get_Keys().get_Count() - 1; num4 >= 0; num4--)
			{
				AICoverPoint aICoverPoint = aIMovePoint.distancesToCover.get_Keys().get_Item(num4);
				if (!coverPoints.Contains(aICoverPoint))
				{
					num2++;
					aIMovePoint.distancesToCover.Remove(aICoverPoint);
				}
			}
			foreach (AICoverPoint coverPoint in coverPoints)
			{
				if ((Object)(object)coverPoint == (Object)null || aIMovePoint.distancesToCover.Contains(coverPoint))
				{
					continue;
				}
				float num5 = -1f;
				if (Vector3.Distance(((Component)aIMovePoint).get_transform().get_position(), ((Component)coverPoint).get_transform().get_position()) > 40f)
				{
					num5 = -2f;
				}
				else if (NavMesh.CalculatePath(((Component)aIMovePoint).get_transform().get_position(), ((Component)coverPoint).get_transform().get_position(), num, val) && (int)val.get_status() == 0)
				{
					int num6 = val.get_corners().Length;
					if (num6 > 1)
					{
						Vector3 val2 = val.get_corners()[0];
						float num7 = 0f;
						for (int i = 0; i < num6; i++)
						{
							Vector3 val3 = val.get_corners()[i];
							num7 += Vector3.Distance(val2, val3);
							val2 = val3;
						}
						num5 = num7;
						pathSuccesses++;
					}
					else
					{
						num5 = Vector3.Distance(((Component)aIMovePoint).get_transform().get_position(), ((Component)coverPoint).get_transform().get_position());
						halfPaths++;
					}
				}
				else
				{
					pathFails++;
					num5 = -2f;
				}
				aIMovePoint.distancesToCover.Add(coverPoint, num5);
				if (!PassesBudget(realtimeSinceStartup, budgetSeconds))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				processIndex++;
			}
			if (processIndex >= movePoints.Count - 1)
			{
				break;
			}
		}
		return processIndex >= movePoints.Count - 1;
	}

	public static void BudgetedTick()
	{
		if (!AI.move || Time.get_realtimeSinceStartup() < buildTimeTest)
		{
			return;
		}
		bool flag = false;
		foreach (AIInformationZone zone in zones)
		{
			if (zone.isDirty)
			{
				flag = true;
				_ = zone.isDirty;
				zone.isDirty = !zone.ProcessDistancesAttempt();
				break;
			}
		}
		if (Global.developer > 0)
		{
			if (flag && !lastFrameAnyDirty)
			{
				Debug.Log((object)"AIInformationZones rebuilding...");
				rebuildStartTime = Time.get_realtimeSinceStartup();
			}
			if (lastFrameAnyDirty && !flag)
			{
				Debug.Log((object)("AIInformationZone rebuild complete! Duration : " + (Time.get_realtimeSinceStartup() - rebuildStartTime) + " seconds."));
			}
		}
		lastFrameAnyDirty = flag;
	}

	public void NavmeshBuildingComplete()
	{
		lastNavmeshBuildTime = Time.get_realtimeSinceStartup();
		buildTimeTest = Time.get_realtimeSinceStartup() + 15f;
		MarkDirty(completeRefresh: true);
	}

	public Vector3 ClosestPointTo(Vector3 target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref areaBox)).ClosestPoint(target);
	}

	public void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_matrix(((Component)this).get_transform().get_localToWorldMatrix());
		Gizmos.set_color(new Color(1f, 0f, 0f, 0.5f));
		Gizmos.DrawCube(((Bounds)(ref bounds)).get_center(), ((Bounds)(ref bounds)).get_size());
	}

	private void AddInitialPoints()
	{
		AICoverPoint[] componentsInChildren = ((Component)((Component)this).get_transform()).GetComponentsInChildren<AICoverPoint>();
		foreach (AICoverPoint point in componentsInChildren)
		{
			AddCoverPoint(point);
		}
		AIMovePoint[] componentsInChildren2 = ((Component)((Component)this).get_transform()).GetComponentsInChildren<AIMovePoint>(true);
		foreach (AIMovePoint point2 in componentsInChildren2)
		{
			AddMovePoint(point2);
		}
		RefreshPointArrays();
		NavMeshLink[] componentsInChildren3 = ((Component)((Component)this).get_transform()).GetComponentsInChildren<NavMeshLink>(true);
		navMeshLinks.AddRange(componentsInChildren3);
		AIMovePointPath[] componentsInChildren4 = ((Component)((Component)this).get_transform()).GetComponentsInChildren<AIMovePointPath>();
		paths.AddRange(componentsInChildren4);
	}

	private void RefreshPointArrays()
	{
		movePointArray = movePoints?.ToArray();
		coverPointArray = coverPoints?.ToArray();
	}

	public void AddDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints, Func<Vector3, bool> validatePoint = null)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (movePoints != null)
		{
			foreach (AIMovePoint aIMovePoint in movePoints)
			{
				if (!((Object)(object)aIMovePoint == (Object)null) && (validatePoint == null || (validatePoint != null && validatePoint(((Component)aIMovePoint).get_transform().get_position()))))
				{
					AddMovePoint(aIMovePoint);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aICoverPoint in coverPoints)
			{
				if (!((Object)(object)aICoverPoint == (Object)null) && (validatePoint == null || (validatePoint != null && validatePoint(((Component)aICoverPoint).get_transform().get_position()))))
				{
					AddCoverPoint(aICoverPoint);
				}
			}
		}
		RefreshPointArrays();
	}

	public void RemoveDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aIMovePoint in movePoints)
			{
				if (!((Object)(object)aIMovePoint == (Object)null))
				{
					RemoveMovePoint(aIMovePoint, markDirty: false);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aICoverPoint in coverPoints)
			{
				if (!((Object)(object)aICoverPoint == (Object)null))
				{
					RemoveCoverPoint(aICoverPoint, markDirty: false);
				}
			}
		}
		MarkDirty();
		RefreshPointArrays();
	}

	public AIMovePointPath GetNearestPath(Vector3 position)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (paths == null || paths.Count == 0)
		{
			return null;
		}
		float num = float.MaxValue;
		AIMovePointPath result = null;
		foreach (AIMovePointPath path in paths)
		{
			foreach (AIMovePoint point in path.Points)
			{
				float num2 = Vector3.SqrMagnitude(((Component)point).get_transform().get_position() - position);
				if (num2 < num)
				{
					num = num2;
					result = path;
				}
			}
		}
		return result;
	}

	public static AIInformationZone GetForPoint(Vector3 point, bool fallBackToNearest = true)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (zones == null || zones.Count == 0)
		{
			return null;
		}
		foreach (AIInformationZone zone in zones)
		{
			if (!((Object)(object)zone == (Object)null) && !zone.Virtual && ((OBB)(ref zone.areaBox)).Contains(point))
			{
				return zone;
			}
		}
		if (!fallBackToNearest)
		{
			return null;
		}
		float num = float.PositiveInfinity;
		AIInformationZone aIInformationZone = zones[0];
		foreach (AIInformationZone zone2 in zones)
		{
			if (!((Object)(object)zone2 == (Object)null) && !((Object)(object)((Component)zone2).get_transform() == (Object)null) && !zone2.Virtual)
			{
				float num2 = Vector3.Distance(((Component)zone2).get_transform().get_position(), point);
				if (num2 < num)
				{
					num = num2;
					aIInformationZone = zone2;
				}
			}
		}
		if (aIInformationZone.Virtual)
		{
			aIInformationZone = null;
		}
		return aIInformationZone;
	}

	public bool PointInside(Vector3 point)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref areaBox)).Contains(point);
	}

	public AIMovePoint GetBestMovePointNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false, BaseEntity forObject = null, bool returnClosest = false)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		AIPoint aIPoint = null;
		AIPoint aIPoint2 = null;
		float num = -1f;
		float num2 = float.PositiveInfinity;
		int pointCount;
		AIPoint[] movePointsInRange = GetMovePointsInRange(targetPosition, maxRange, out pointCount);
		if (movePointsInRange == null || pointCount <= 0)
		{
			return null;
		}
		for (int i = 0; i < pointCount; i++)
		{
			AIPoint aIPoint3 = movePointsInRange[i];
			if (!((Component)((Component)aIPoint3).get_transform().get_parent()).get_gameObject().get_activeSelf() || (!(fromPosition.y < WaterSystem.OceanLevel) && ((Component)aIPoint3).get_transform().get_position().y < WaterSystem.OceanLevel))
			{
				continue;
			}
			float num3 = 0f;
			Vector3 position = ((Component)aIPoint3).get_transform().get_position();
			float num4 = Vector3.Distance(targetPosition, position);
			if (num4 < num2)
			{
				aIPoint2 = aIPoint3;
				num2 = num4;
			}
			if (!(num4 > maxRange))
			{
				num3 += (aIPoint3.CanBeUsedBy(forObject) ? 100f : 0f);
				num3 += (1f - Mathf.InverseLerp(minRange, maxRange, num4)) * 100f;
				if (!(num3 < num) && (!checkLOS || !Physics.Linecast(targetPosition + Vector3.get_up() * 1f, position + Vector3.get_up() * 1f, 1218519297, (QueryTriggerInteraction)1)) && num3 > num)
				{
					aIPoint = aIPoint3;
					num = num3;
				}
			}
		}
		if ((Object)(object)aIPoint == (Object)null && returnClosest)
		{
			return aIPoint2 as AIMovePoint;
		}
		return aIPoint as AIMovePoint;
	}

	public AIPoint[] GetMovePointsInRange(Vector3 currentPos, float maxRange, out int pointCount)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		pointCount = 0;
		AIMovePoint[] movePointsInRange;
		if ((Object)(object)grid != (Object)null && AI.usegrid)
		{
			movePointsInRange = grid.GetMovePointsInRange(currentPos, maxRange, out pointCount);
		}
		else
		{
			movePointsInRange = movePointArray;
			if (movePointsInRange != null)
			{
				pointCount = movePointsInRange.Length;
			}
		}
		return movePointsInRange;
	}

	private AIMovePoint GetClosestRaw(Vector3 pos, bool onlyIncludeWithCover = false)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		AIMovePoint result = null;
		float num = float.PositiveInfinity;
		foreach (AIMovePoint movePoint in movePoints)
		{
			if (!onlyIncludeWithCover || movePoint.distancesToCover.get_Count() != 0)
			{
				float num2 = Vector3.Distance(((Component)movePoint).get_transform().get_position(), pos);
				if (num2 < num)
				{
					num = num2;
					result = movePoint;
				}
			}
		}
		return result;
	}

	public AICoverPoint GetBestCoverPoint(Vector3 currentPosition, Vector3 hideFromPosition, float minRange = 0f, float maxRange = 20f, BaseEntity forObject = null, bool allowObjectToReuse = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		AICoverPoint aICoverPoint = null;
		float num = 0f;
		AIMovePoint closestRaw = GetClosestRaw(currentPosition, onlyIncludeWithCover: true);
		int pointCount;
		AICoverPoint[] coverPointsInRange = GetCoverPointsInRange(currentPosition, maxRange, out pointCount);
		if (coverPointsInRange == null || pointCount <= 0)
		{
			return null;
		}
		for (int i = 0; i < pointCount; i++)
		{
			AICoverPoint aICoverPoint2 = coverPointsInRange[i];
			Vector3 position = ((Component)aICoverPoint2).get_transform().get_position();
			Vector3 val = hideFromPosition - position;
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			float num2 = Vector3.Dot(((Component)aICoverPoint2).get_transform().get_forward(), normalized);
			if (num2 < 1f - aICoverPoint2.coverDot)
			{
				continue;
			}
			float num3 = -1f;
			if (UseCalculatedCoverDistances && (Object)(object)closestRaw != (Object)null && closestRaw.distancesToCover.Contains(aICoverPoint2) && !isDirty)
			{
				num3 = closestRaw.distancesToCover.get_Item(aICoverPoint2);
				if (num3 == -2f)
				{
					continue;
				}
			}
			else
			{
				num3 = Vector3.Distance(currentPosition, position);
			}
			float num4 = 0f;
			if (aICoverPoint2.InUse())
			{
				bool flag = aICoverPoint2.IsUsedBy(forObject);
				if (!(allowObjectToReuse && flag))
				{
					num4 -= 1000f;
				}
			}
			if (minRange > 0f)
			{
				num4 -= (1f - Mathf.InverseLerp(0f, minRange, num3)) * 100f;
			}
			float num5 = Mathf.Abs(position.y - currentPosition.y);
			num4 += (1f - Mathf.InverseLerp(1f, 5f, num5)) * 500f;
			num4 += Mathf.InverseLerp(1f - aICoverPoint2.coverDot, 1f, num2) * 50f;
			num4 += (1f - Mathf.InverseLerp(2f, maxRange, num3)) * 100f;
			float num6 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
			val = ((Component)aICoverPoint2).get_transform().get_position() - currentPosition;
			float num7 = Vector3.Dot(((Vector3)(ref val)).get_normalized(), normalized);
			num4 -= Mathf.InverseLerp(-1f, 0.25f, num7) * 50f * num6;
			if (num4 > num)
			{
				aICoverPoint = aICoverPoint2;
				num = num4;
			}
		}
		if (Object.op_Implicit((Object)(object)aICoverPoint))
		{
			return aICoverPoint;
		}
		return null;
	}

	private AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		pointCount = 0;
		AICoverPoint[] coverPointsInRange;
		if ((Object)(object)grid != (Object)null && AI.usegrid)
		{
			coverPointsInRange = grid.GetCoverPointsInRange(position, maxRange, out pointCount);
		}
		else
		{
			coverPointsInRange = coverPointArray;
			if (coverPointsInRange != null)
			{
				pointCount = coverPointsInRange.Length;
			}
		}
		return coverPointsInRange;
	}

	public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		NavMeshLink result = null;
		float num = float.PositiveInfinity;
		foreach (NavMeshLink navMeshLink in navMeshLinks)
		{
			float num2 = Vector3.Distance(((Component)navMeshLink).get_gameObject().get_transform().get_position(), pos);
			if (num2 < num)
			{
				result = navMeshLink;
				num = num2;
				if (num2 < 0.25f)
				{
					return result;
				}
			}
		}
		return result;
	}
}
