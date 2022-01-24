using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	private HashSet<Collider> waterColliders;

	private void Awake()
	{
		ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		waterColliders = new HashSet<Collider>();
	}

	public void Clear()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (waterColliders.get_Count() == 0)
		{
			return;
		}
		Enumerator<Collider> enumerator = waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Enumerator<Collider> enumerator2 = ignoredColliders.get_Keys().GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Physics.IgnoreCollision(enumerator2.get_Current(), enumerator.get_Current(), false);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
		}
		ignoredColliders.Clear();
	}

	public void Reset(Collider collider)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (waterColliders.get_Count() != 0 && Object.op_Implicit((Object)(object)collider))
		{
			Enumerator<Collider> enumerator = waterColliders.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Physics.IgnoreCollision(collider, enumerator.get_Current(), false);
			}
			ignoredColliders.Remove(collider);
		}
	}

	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, (QueryTriggerInteraction)2);
	}

	public bool GetIgnore(Bounds bounds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, (QueryTriggerInteraction)2);
	}

	public bool GetIgnore(Vector3 start, Vector3 end, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckCapsule<WaterVisibilityTrigger>(start, end, radius, 262144, (QueryTriggerInteraction)2);
	}

	public bool GetIgnore(RaycastHit hit)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (waterColliders.Contains(((RaycastHit)(ref hit)).get_collider()))
		{
			return GetIgnore(((RaycastHit)(ref hit)).get_point());
		}
		return false;
	}

	public bool GetIgnore(Collider collider)
	{
		if (waterColliders.get_Count() == 0 || !Object.op_Implicit((Object)(object)collider))
		{
			return false;
		}
		return ignoredColliders.Contains(collider);
	}

	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (waterColliders.get_Count() == 0 || !Object.op_Implicit((Object)(object)collider))
		{
			return;
		}
		if (!GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> list = new List<Collider> { trigger };
				Enumerator<Collider> enumerator = waterColliders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Physics.IgnoreCollision(collider, enumerator.get_Current(), true);
				}
				ignoredColliders.Add(collider, list);
			}
			return;
		}
		List<Collider> list2 = ignoredColliders.get_Item(collider);
		if (ignore)
		{
			if (!list2.Contains(trigger))
			{
				list2.Add(trigger);
			}
		}
		else if (list2.Contains(trigger))
		{
			list2.Remove(trigger);
		}
	}

	protected void LateUpdate()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ignoredColliders.get_Count(); i++)
		{
			KeyValuePair<Collider, List<Collider>> byIndex = ignoredColliders.GetByIndex(i);
			Collider key = byIndex.Key;
			List<Collider> value = byIndex.Value;
			if ((Object)(object)key == (Object)null)
			{
				ignoredColliders.RemoveAt(i--);
			}
			else if (value.Count == 0)
			{
				Enumerator<Collider> enumerator = waterColliders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Physics.IgnoreCollision(key, enumerator.get_Current(), false);
				}
				ignoredColliders.RemoveAt(i--);
			}
		}
	}

	public WaterCollision()
		: this()
	{
	}
}
