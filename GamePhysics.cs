using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

public static class GamePhysics
{
	public const int BufferLength = 8192;

	private static RaycastHit[] hitBuffer = (RaycastHit[])(object)new RaycastHit[8192];

	private static RaycastHit[] hitBufferB = (RaycastHit[])(object)new RaycastHit[8192];

	private static Collider[] colBuffer = (Collider[])(object)new Collider[8192];

	public static bool CheckSphere(Vector3 position, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(position, layerMask);
		return Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
	}

	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision((start + end) * 0.5f, layerMask);
		return Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
	}

	public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(obb.position, layerMask);
		return Physics.CheckBox(obb.position, obb.extents, obb.rotation, layerMask, triggerInteraction);
	}

	public static bool CheckBounds(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(((Bounds)(ref bounds)).get_center(), layerMask);
		return Physics.CheckBox(((Bounds)(ref bounds)).get_center(), ((Bounds)(ref bounds)).get_extents(), Quaternion.get_identity(), layerMask, triggerInteraction);
	}

	public static bool CheckInsideNonConvexMesh(Vector3 point, int layerMask = -5)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		bool queriesHitBackfaces = Physics.get_queriesHitBackfaces();
		Physics.set_queriesHitBackfaces(true);
		int num = Physics.RaycastNonAlloc(point, Vector3.get_up(), hitBuffer, 100f, layerMask);
		int num2 = Physics.RaycastNonAlloc(point, -Vector3.get_up(), hitBufferB, 100f, layerMask);
		if (num >= hitBuffer.Length)
		{
			Debug.LogWarning((object)"CheckInsideNonConvexMesh query is exceeding hitBuffer length.");
			return false;
		}
		if (num2 > hitBufferB.Length)
		{
			Debug.LogWarning((object)"CheckInsideNonConvexMesh query is exceeding hitBufferB length.");
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if ((Object)(object)((RaycastHit)(ref hitBuffer[i])).get_collider() == (Object)(object)((RaycastHit)(ref hitBufferB[j])).get_collider())
				{
					Physics.set_queriesHitBackfaces(queriesHitBackfaces);
					return true;
				}
			}
		}
		Physics.set_queriesHitBackfaces(queriesHitBackfaces);
		return false;
	}

	public static bool CheckInsideAnyCollider(Vector3 point, int layerMask = -5)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.CheckSphere(point, 0f, layerMask))
		{
			return true;
		}
		if (CheckInsideNonConvexMesh(point, layerMask))
		{
			return true;
		}
		if ((Object)(object)TerrainMeta.HeightMap != (Object)null && TerrainMeta.HeightMap.GetHeight(point) > point.y)
		{
			return true;
		}
		return false;
	}

	public static void OverlapSphere(Vector3 position, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(position, layerMask);
		BufferToList(Physics.OverlapSphereNonAlloc(position, radius, colBuffer, layerMask, triggerInteraction), list);
	}

	public static void OverlapCapsule(Vector3 point0, Vector3 point1, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(point0, layerMask);
		layerMask = HandleTerrainCollision(point1, layerMask);
		BufferToList(Physics.OverlapCapsuleNonAlloc(point0, point1, radius, colBuffer, layerMask, triggerInteraction), list);
	}

	public static void OverlapOBB(OBB obb, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(obb.position, layerMask);
		BufferToList(Physics.OverlapBoxNonAlloc(obb.position, obb.extents, colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	public static void OverlapBounds(Bounds bounds, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(((Bounds)(ref bounds)).get_center(), layerMask);
		BufferToList(Physics.OverlapBoxNonAlloc(((Bounds)(ref bounds)).get_center(), ((Bounds)(ref bounds)).get_extents(), colBuffer, Quaternion.get_identity(), layerMask, triggerInteraction), list);
	}

	private static void BufferToList(int count, List<Collider> list)
	{
		if (count >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(colBuffer[i]);
			colBuffer[i] = null;
		}
	}

	public static bool CheckSphere<T>(Vector3 pos, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckCapsule<T>(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckOBB<T>(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapOBB(obb, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckBounds<T>(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapBounds(bounds, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	private static bool CheckComponent<T>(List<Collider> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (((Component)list[i]).get_gameObject().GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public static void OverlapSphere<T>(Vector3 position, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(position, layerMask);
		BufferToList(Physics.OverlapSphereNonAlloc(position, radius, colBuffer, layerMask, triggerInteraction), list);
	}

	public static void OverlapCapsule<T>(Vector3 point0, Vector3 point1, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(point0, layerMask);
		layerMask = HandleTerrainCollision(point1, layerMask);
		BufferToList(Physics.OverlapCapsuleNonAlloc(point0, point1, radius, colBuffer, layerMask, triggerInteraction), list);
	}

	public static void OverlapOBB<T>(OBB obb, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(obb.position, layerMask);
		BufferToList(Physics.OverlapBoxNonAlloc(obb.position, obb.extents, colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	public static void OverlapBounds<T>(Bounds bounds, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleTerrainCollision(((Bounds)(ref bounds)).get_center(), layerMask);
		BufferToList(Physics.OverlapBoxNonAlloc(((Bounds)(ref bounds)).get_center(), ((Bounds)(ref bounds)).get_extents(), colBuffer, Quaternion.get_identity(), layerMask, triggerInteraction), list);
	}

	private static void BufferToList<T>(int count, List<T> list) where T : Component
	{
		if (count >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			T component = ((Component)colBuffer[i]).get_gameObject().GetComponent<T>();
			if (Object.op_Implicit((Object)(object)component))
			{
				list.Add(component);
			}
			colBuffer[i] = null;
		}
	}

	public static bool Trace(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction);
		if (list.Count == 0)
		{
			hitInfo = default(RaycastHit);
			Pool.FreeList<RaycastHit>(ref list);
			return false;
		}
		Sort(list);
		hitInfo = list[0];
		Pool.FreeList<RaycastHit>(ref list);
		return true;
	}

	public static void TraceAll(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction);
		Sort(hits);
	}

	public static void TraceAllUnordered(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		num = ((radius != 0f) ? Physics.SphereCastNonAlloc(ray, radius, hitBuffer, maxDistance, layerMask, triggerInteraction) : Physics.RaycastNonAlloc(ray, hitBuffer, maxDistance, layerMask, triggerInteraction));
		if (num == 0)
		{
			return;
		}
		if (num >= hitBuffer.Length)
		{
			Debug.LogWarning((object)"Physics query is exceeding hit buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit val = hitBuffer[i];
			if (Verify(val))
			{
				hits.Add(val);
			}
		}
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightInternal(p0, p1, layerMask, radius, padding0, padding1);
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightInternal(p0, p1, layerMask, radius, padding, padding);
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, Vector3 p2, int layerMask, float radius, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (LineOfSightInternal(p0, p1, layerMask, radius, padding, 0f))
		{
			return LineOfSightInternal(p1, p2, layerMask, radius, 0f, padding);
		}
		return false;
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int layerMask, float radius, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (LineOfSightInternal(p0, p1, layerMask, radius, padding, 0f) && LineOfSightInternal(p1, p2, layerMask, radius, 0f, 0f))
		{
			return LineOfSightInternal(p2, p3, layerMask, radius, 0f, padding);
		}
		return false;
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int layerMask, float radius, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (LineOfSightInternal(p0, p1, layerMask, radius, padding, 0f) && LineOfSightInternal(p1, p2, layerMask, radius, 0f, 0f) && LineOfSightInternal(p2, p3, layerMask, radius, 0f, 0f))
		{
			return LineOfSightInternal(p3, p4, layerMask, radius, 0f, padding);
		}
		return false;
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding0, float padding1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, layerMask, 0f, padding0, padding1);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, layerMask, 0f, padding);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, int layerMask, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, p2, layerMask, 0f, padding);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int layerMask, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, p2, p3, layerMask, 0f, padding);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int layerMask, float padding = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, p2, p3, p4, layerMask, 0f, padding);
	}

	private static bool LineOfSightInternal(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		if (!ValidBounds.Test(p0))
		{
			return false;
		}
		if (!ValidBounds.Test(p1))
		{
			return false;
		}
		Vector3 val = p1 - p0;
		float magnitude = ((Vector3)(ref val)).get_magnitude();
		if (magnitude <= padding0 + padding1)
		{
			return true;
		}
		Vector3 val2 = val / magnitude;
		Ray val3 = default(Ray);
		((Ray)(ref val3))._002Ector(p0 + val2 * padding0, val2);
		float num = magnitude - padding0 - padding1;
		bool flag;
		RaycastHit hitInfo = default(RaycastHit);
		if (((uint)layerMask & 0x800000u) != 0)
		{
			flag = Trace(val3, 0f, out hitInfo, num, layerMask, (QueryTriggerInteraction)1);
			if (radius > 0f && !flag)
			{
				flag = Trace(val3, radius, out hitInfo, num, layerMask, (QueryTriggerInteraction)1);
			}
		}
		else
		{
			flag = Physics.Raycast(val3, ref hitInfo, num, layerMask, (QueryTriggerInteraction)1);
			if (radius > 0f && !flag)
			{
				flag = Physics.SphereCast(val3, radius, ref hitInfo, num, layerMask, (QueryTriggerInteraction)1);
			}
		}
		if (!flag)
		{
			if (ConVar.Vis.lineofsight)
			{
				ConsoleNetwork.BroadcastToAllClients("ddraw.line", 60f, Color.get_green(), p0, p1);
			}
			return true;
		}
		if (ConVar.Vis.lineofsight)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.line", 60f, Color.get_red(), p0, p1);
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", 60f, Color.get_white(), ((RaycastHit)(ref hitInfo)).get_point(), ((Object)((RaycastHit)(ref hitInfo)).get_collider()).get_name());
		}
		return false;
	}

	public static bool Verify(RaycastHit hitInfo)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return Verify(((RaycastHit)(ref hitInfo)).get_collider(), ((RaycastHit)(ref hitInfo)).get_point());
	}

	public static bool Verify(Collider collider, Vector3 point)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (collider is TerrainCollider && Object.op_Implicit((Object)(object)TerrainMeta.Collision) && TerrainMeta.Collision.GetIgnore(point))
		{
			return false;
		}
		return collider.get_enabled();
	}

	public static int HandleTerrainCollision(Vector3 position, int layerMask)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		int num = 8388608;
		if ((layerMask & num) != 0 && Object.op_Implicit((Object)(object)TerrainMeta.Collision) && TerrainMeta.Collision.GetIgnore(position))
		{
			layerMask &= ~num;
		}
		return layerMask;
	}

	public static void Sort(List<RaycastHit> hits)
	{
		hits.Sort((RaycastHit a, RaycastHit b) => ((RaycastHit)(ref a)).get_distance().CompareTo(((RaycastHit)(ref b)).get_distance()));
	}

	public static void Sort(RaycastHit[] hits)
	{
		Array.Sort(hits, (RaycastHit a, RaycastHit b) => ((RaycastHit)(ref a)).get_distance().CompareTo(((RaycastHit)(ref b)).get_distance()));
	}
}
