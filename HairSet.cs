using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Hair Set")]
public class HairSet : ScriptableObject
{
	[Serializable]
	public class MeshReplace
	{
		[HideInInspector]
		public string FindName;

		public Mesh Find;

		public Mesh[] ReplaceShapes;

		public bool Test(string materialName)
		{
			return FindName == materialName;
		}
	}

	public MeshReplace[] MeshReplacements;

	public void Process(PlayerModelHair playerModelHair, HairDyeCollection dyeCollection, HairDye dye, MaterialPropertyBlock block)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		((Component)playerModelHair).get_gameObject().GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer item in list)
		{
			if (!((Object)(object)item.get_sharedMesh() == (Object)null) && !((Object)(object)((Renderer)item).get_sharedMaterial() == (Object)null))
			{
				string name = ((Object)item.get_sharedMesh()).get_name();
				((Object)((Renderer)item).get_sharedMaterial()).get_name();
				if (!((Component)item).get_gameObject().get_activeSelf())
				{
					((Component)item).get_gameObject().SetActive(true);
				}
				for (int i = 0; i < MeshReplacements.Length; i++)
				{
					MeshReplacements[i].Test(name);
				}
				if (dye != null && ((Component)item).get_gameObject().get_activeSelf())
				{
					dye.Apply(dyeCollection, block);
				}
			}
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
	{
	}

	public HairSet()
		: this()
	{
	}
}
