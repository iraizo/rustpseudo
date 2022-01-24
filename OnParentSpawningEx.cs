using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public static class OnParentSpawningEx
{
	public static void BroadcastOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponentsInChildren<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}

	public static void SendOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponents<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}
}
