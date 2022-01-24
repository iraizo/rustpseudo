using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class StripRig : MonoBehaviour, IPrefabPreProcess
{
	public Transform root;

	public bool fromClient;

	public bool fromServer;

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (Object.op_Implicit((Object)(object)root) && ((serverside && fromServer) || (clientside && fromClient)))
		{
			SkinnedMeshRenderer component = ((Component)this).GetComponent<SkinnedMeshRenderer>();
			Strip(preProcess, component);
		}
		preProcess.RemoveComponent((Component)(object)this);
	}

	public void Strip(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		List<Transform> list = Pool.GetList<Transform>();
		((Component)root).GetComponentsInChildren<Transform>(list);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (preProcess != null)
			{
				preProcess.NominateForDeletion(((Component)list[num]).get_gameObject());
			}
			else
			{
				Object.DestroyImmediate((Object)(object)((Component)list[num]).get_gameObject());
			}
		}
		Pool.FreeList<Transform>(ref list);
	}

	public StripRig()
		: this()
	{
	}
}
