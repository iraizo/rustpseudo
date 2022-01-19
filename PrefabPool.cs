using System.Collections.Generic;
using UnityEngine;

public class PrefabPool
{
	public Stack<Poolable> stack = new Stack<Poolable>();

	public int Count => stack.Count;

	public void Push(Poolable info)
	{
		stack.Push(info);
		info.EnterPool();
	}

	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		Push(component);
	}

	public GameObject Pop(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		while (stack.Count > 0)
		{
			Poolable poolable = stack.Pop();
			if (Object.op_Implicit((Object)(object)poolable))
			{
				((Component)poolable).get_transform().set_position(pos);
				((Component)poolable).get_transform().set_rotation(rot);
				poolable.LeavePool();
				return ((Component)poolable).get_gameObject();
			}
		}
		return null;
	}

	public void Clear()
	{
		foreach (Poolable item in stack)
		{
			if (Object.op_Implicit((Object)(object)item))
			{
				Object.Destroy((Object)(object)((Component)item).get_gameObject());
			}
		}
		stack.Clear();
	}
}
