using System.Collections.Generic;
using UnityEngine;

public class AIInformationCellContents<T> where T : AIPoint
{
	public HashSet<T> Items = new HashSet<T>();

	public int Count => Items.get_Count();

	public bool Empty => Items.get_Count() == 0;

	public void Init(Bounds cellBounds, GameObject root)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Clear();
		T[] componentsInChildren = root.GetComponentsInChildren<T>(true);
		foreach (T val in componentsInChildren)
		{
			if (((Bounds)(ref cellBounds)).Contains(((Component)val).get_gameObject().get_transform().get_position()))
			{
				Add(val);
			}
		}
	}

	public void Clear()
	{
		Items.Clear();
	}

	public void Add(T item)
	{
		Items.Add(item);
	}

	public void Remove(T item)
	{
		Items.Remove(item);
	}
}
