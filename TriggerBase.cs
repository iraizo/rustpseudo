using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

public class TriggerBase : BaseMonoBehaviour
{
	public LayerMask interestLayers;

	[NonSerialized]
	public HashSet<GameObject> contents;

	[NonSerialized]
	public HashSet<BaseEntity> entityContents;

	public bool HasAnyContents => !((ICollection<GameObject>)contents).IsNullOrEmpty();

	public bool HasAnyEntityContents => !((ICollection<BaseEntity>)entityContents).IsNullOrEmpty();

	internal virtual GameObject InterestedInObject(GameObject obj)
	{
		int num = 1 << obj.get_layer();
		if ((((LayerMask)(ref interestLayers)).get_value() & num) != num)
		{
			return null;
		}
		return obj;
	}

	protected virtual void OnDisable()
	{
		if (!Application.isQuitting && contents != null)
		{
			GameObject[] array = Enumerable.ToArray<GameObject>((IEnumerable<GameObject>)contents);
			foreach (GameObject targetObj in array)
			{
				OnTriggerExit(targetObj);
			}
			contents = null;
		}
	}

	internal virtual void OnEntityEnter(BaseEntity ent)
	{
		if (!((Object)(object)ent == (Object)null))
		{
			if (entityContents == null)
			{
				entityContents = new HashSet<BaseEntity>();
			}
			entityContents.Add(ent);
		}
	}

	internal virtual void OnEntityLeave(BaseEntity ent)
	{
		if (entityContents != null)
		{
			entityContents.Remove(ent);
		}
	}

	internal virtual void OnObjectAdded(GameObject obj, Collider col)
	{
		if (!((Object)(object)obj == (Object)null))
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.EnterTrigger(this);
				OnEntityEnter(baseEntity);
			}
		}
	}

	internal virtual void OnObjectRemoved(GameObject obj)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)obj == (Object)null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (!Object.op_Implicit((Object)(object)baseEntity))
		{
			return;
		}
		bool flag = false;
		Enumerator<GameObject> enumerator = contents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				GameObject current = enumerator.get_Current();
				if ((Object)(object)current == (Object)null)
				{
					Debug.LogWarning((object)("Trigger " + ((object)this).ToString() + " contains null object."));
				}
				else if ((Object)(object)current.ToBaseEntity() == (Object)(object)baseEntity)
				{
					flag = true;
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (!flag)
		{
			baseEntity.LeaveTrigger(this);
			OnEntityLeave(baseEntity);
		}
	}

	internal void RemoveInvalidEntities()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (((ICollection<BaseEntity>)entityContents).IsNullOrEmpty())
		{
			return;
		}
		Collider component = ((Component)this).GetComponent<Collider>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		Bounds bounds = component.get_bounds();
		((Bounds)(ref bounds)).Expand(1f);
		List<BaseEntity> list = null;
		Enumerator<BaseEntity> enumerator = entityContents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity current = enumerator.get_Current();
				if ((Object)(object)current == (Object)null)
				{
					if (Debugging.checktriggers)
					{
						Debug.LogWarning((object)("Trigger " + ((object)this).ToString() + " contains destroyed entity."));
					}
					if (list == null)
					{
						list = Pool.GetList<BaseEntity>();
					}
					list.Add(current);
				}
				else if (!((Bounds)(ref bounds)).Contains(current.ClosestPoint(((Component)this).get_transform().get_position())))
				{
					if (Debugging.checktriggers)
					{
						Debug.LogWarning((object)("Trigger " + ((object)this).ToString() + " contains entity that is too far away: " + ((object)current).ToString()));
					}
					if (list == null)
					{
						list = Pool.GetList<BaseEntity>();
					}
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (list == null)
		{
			return;
		}
		foreach (BaseEntity item in list)
		{
			RemoveEntity(item);
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	internal bool CheckEntity(BaseEntity ent)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent == (Object)null)
		{
			return true;
		}
		Collider component = ((Component)this).GetComponent<Collider>();
		if ((Object)(object)component == (Object)null)
		{
			return true;
		}
		Bounds bounds = component.get_bounds();
		((Bounds)(ref bounds)).Expand(1f);
		return ((Bounds)(ref bounds)).Contains(ent.ClosestPoint(((Component)this).get_transform().get_position()));
	}

	internal virtual void OnObjects()
	{
	}

	internal virtual void OnEmpty()
	{
		contents = null;
		entityContents = null;
	}

	public void RemoveObject(GameObject obj)
	{
		if (!((Object)(object)obj == (Object)null))
		{
			Collider component = obj.GetComponent<Collider>();
			if (!((Object)(object)component == (Object)null))
			{
				OnTriggerExit(component);
			}
		}
	}

	public void RemoveEntity(BaseEntity ent)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)this == (Object)null || contents == null || (Object)(object)ent == (Object)null)
		{
			return;
		}
		List<GameObject> list = Pool.GetList<GameObject>();
		Enumerator<GameObject> enumerator = contents.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				GameObject current = enumerator.get_Current();
				if ((Object)(object)current != (Object)null && (Object)(object)current.GetComponentInParent<BaseEntity>() == (Object)(object)ent)
				{
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		foreach (GameObject item in list)
		{
			OnTriggerExit(item);
		}
		Pool.FreeList<GameObject>(ref list);
	}

	public void OnTriggerEnter(Collider collider)
	{
		if ((Object)(object)this == (Object)null || !((Behaviour)this).get_enabled())
		{
			return;
		}
		TimeWarning val = TimeWarning.New("TriggerBase.OnTriggerEnter", 0);
		try
		{
			GameObject val2 = InterestedInObject(((Component)collider).get_gameObject());
			if ((Object)(object)val2 == (Object)null)
			{
				return;
			}
			if (contents == null)
			{
				contents = new HashSet<GameObject>();
			}
			if (contents.Contains(val2))
			{
				return;
			}
			int count = contents.get_Count();
			contents.Add(val2);
			OnObjectAdded(val2, collider);
			if (count == 0 && contents.get_Count() == 1)
			{
				OnObjects();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (Debugging.checktriggers)
		{
			RemoveInvalidEntities();
		}
	}

	internal virtual bool SkipOnTriggerExit(Collider collider)
	{
		return false;
	}

	public void OnTriggerExit(Collider collider)
	{
		if ((Object)(object)this == (Object)null || (Object)(object)collider == (Object)null || SkipOnTriggerExit(collider))
		{
			return;
		}
		GameObject val = InterestedInObject(((Component)collider).get_gameObject());
		if (!((Object)(object)val == (Object)null))
		{
			OnTriggerExit(val);
			if (Debugging.checktriggers)
			{
				RemoveInvalidEntities();
			}
		}
	}

	private void OnTriggerExit(GameObject targetObj)
	{
		if (contents != null && contents.Contains(targetObj))
		{
			contents.Remove(targetObj);
			OnObjectRemoved(targetObj);
			if (contents == null || contents.get_Count() == 0)
			{
				OnEmpty();
			}
		}
	}
}
