using System.Collections.Generic;
using UnityEngine;

public static class Vis
{
	private static int colCount = 0;

	private static Collider[] colBuffer = (Collider[])(object)new Collider[8192];

	private static void Buffer(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		int num = colCount;
		colCount = Physics.OverlapSphereNonAlloc(position, radius, colBuffer, layerMask, triggerInteraction);
		for (int i = colCount; i < num; i++)
		{
			colBuffer[i] = null;
		}
		if (colCount >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Vis query is exceeding collider buffer length.");
			colCount = colBuffer.Length;
		}
	}

	public static bool AnyColliders(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Buffer(position, radius, layerMask, triggerInteraction);
		return colCount > 0;
	}

	public static void Colliders<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Collider
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider obj = colBuffer[i];
			T val = (T)(object)((obj is T) ? obj : null);
			if (!((Object)(object)val == (Object)null) && ((Collider)val).get_enabled())
			{
				list.Add(val);
			}
		}
	}

	public static void Components<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Component
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.get_enabled())
			{
				T component = ((Component)val).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
	}

	public static void Entities<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : class
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.get_enabled())
			{
				T val2 = val.ToBaseEntity() as T;
				if (val2 != null)
				{
					list.Add(val2);
				}
			}
		}
	}

	public static void EntityComponents<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : EntityComponentBase
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if ((Object)(object)val == (Object)null || !val.get_enabled())
			{
				continue;
			}
			BaseEntity baseEntity = val.ToBaseEntity();
			if (!((Object)(object)baseEntity == (Object)null))
			{
				T component = ((Component)baseEntity).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
	}

	private static void Buffer(OBB bounds, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		layerMask = GamePhysics.HandleTerrainCollision(bounds.position, layerMask);
		int num = colCount;
		colCount = Physics.OverlapBoxNonAlloc(bounds.position, bounds.extents, colBuffer, bounds.rotation, layerMask, triggerInteraction);
		for (int i = colCount; i < num; i++)
		{
			colBuffer[i] = null;
		}
		if (colCount >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Vis query is exceeding collider buffer length.");
			colCount = colBuffer.Length;
		}
	}

	public static void Colliders<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Collider
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider obj = colBuffer[i];
			T val = (T)(object)((obj is T) ? obj : null);
			if (!((Object)(object)val == (Object)null) && ((Collider)val).get_enabled())
			{
				list.Add(val);
			}
		}
	}

	public static void Components<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Component
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.get_enabled())
			{
				T component = ((Component)val).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
	}

	public static void Entities<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : BaseEntity
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.get_enabled())
			{
				T val2 = val.ToBaseEntity() as T;
				if (!((Object)(object)val2 == (Object)null))
				{
					list.Add(val2);
				}
			}
		}
	}

	public static void EntityComponents<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : EntityComponentBase
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if ((Object)(object)val == (Object)null || !val.get_enabled())
			{
				continue;
			}
			BaseEntity baseEntity = val.ToBaseEntity();
			if (!((Object)(object)baseEntity == (Object)null))
			{
				T component = ((Component)baseEntity).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
	}

	private static void Buffer(Vector3 startPosition, Vector3 endPosition, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		layerMask = GamePhysics.HandleTerrainCollision(startPosition, layerMask);
		int num = colCount;
		colCount = Physics.OverlapCapsuleNonAlloc(startPosition, endPosition, radius, colBuffer, layerMask, triggerInteraction);
		for (int i = colCount; i < num; i++)
		{
			colBuffer[i] = null;
		}
		if (colCount >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Vis query is exceeding collider buffer length.");
			colCount = colBuffer.Length;
		}
	}

	public static void Entities<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : BaseEntity
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.get_enabled())
			{
				T val2 = val.ToBaseEntity() as T;
				if (!((Object)(object)val2 == (Object)null))
				{
					list.Add(val2);
				}
			}
		}
	}
}
