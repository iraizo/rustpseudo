using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class PlayerModelHair : MonoBehaviour
{
	public struct RendererMaterials
	{
		public string[] names;

		public Material[] original;

		public Material[] replacement;

		public RendererMaterials(Renderer r)
		{
			original = r.get_sharedMaterials();
			replacement = original.Clone() as Material[];
			names = new string[original.Length];
			for (int i = 0; i < original.Length; i++)
			{
				names[i] = ((Object)original[i]).get_name();
			}
		}
	}

	public HairType type;

	private Dictionary<Renderer, RendererMaterials> materials;

	public Dictionary<Renderer, RendererMaterials> Materials => materials;

	private void CacheOriginalMaterials()
	{
		if (materials != null)
		{
			return;
		}
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		((Component)this).get_gameObject().GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		materials = new Dictionary<Renderer, RendererMaterials>();
		materials.Clear();
		foreach (SkinnedMeshRenderer item in list)
		{
			materials.Add((Renderer)(object)item, new RendererMaterials((Renderer)(object)item));
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	private void Setup(HairType type, HairSetCollection hair, int meshIndex, float typeNum, float dyeNum, MaterialPropertyBlock block)
	{
		CacheOriginalMaterials();
		HairSetCollection.HairSetEntry hairSetEntry = hair.Get(type, typeNum);
		if ((Object)(object)hairSetEntry.HairSet == (Object)null)
		{
			Debug.LogWarning((object)"Hair.Get returned a NULL hair");
			return;
		}
		int blendShapeIndex = -1;
		if (type == HairType.Facial || type == HairType.Eyebrow)
		{
			blendShapeIndex = meshIndex;
		}
		HairDye dye = null;
		HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
		if ((Object)(object)hairDyeCollection != (Object)null)
		{
			dye = hairDyeCollection.Get(dyeNum);
		}
		hairSetEntry.HairSet.Process(this, hairDyeCollection, dye, block);
		hairSetEntry.HairSet.ProcessMorphs(((Component)this).get_gameObject(), blendShapeIndex);
	}

	public void Setup(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if ((Object)(object)skinSet == (Object)null)
		{
			Debug.LogError((object)"Skin.Get returned a NULL skin");
			return;
		}
		int typeIndex = (int)type;
		GetRandomVariation(hairNum, typeIndex, index, out var typeNum, out var dyeNum);
		Setup(type, skinSet.HairCollection, index, typeNum, dyeNum, block);
	}

	public static void GetRandomVariation(float hairNum, int typeIndex, int meshIndex, out float typeNum, out float dyeNum)
	{
		int num = Mathf.FloorToInt(hairNum * 100000f);
		typeNum = GetRandomHairType(hairNum, typeIndex);
		Random.InitState(num + meshIndex);
		dyeNum = Random.Range(0f, 1f);
	}

	public static float GetRandomHairType(float hairNum, int typeIndex)
	{
		Random.InitState(Mathf.FloorToInt(hairNum * 100000f) + typeIndex);
		return Random.Range(0f, 1f);
	}

	public PlayerModelHair()
		: this()
	{
	}
}
