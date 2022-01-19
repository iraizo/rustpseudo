using System;
using System.Collections.Generic;
using UnityEngine;

public static class RendererEx
{
	private static readonly Memoized<Material[], int> ArrayCache = new Memoized<Material[], int>((Func<int, Material[]>)((int n) => (Material[])(object)new Material[n]));

	public static void SetSharedMaterials(this Renderer renderer, List<Material> materials)
	{
		if (materials.Count != 0)
		{
			if (materials.Count > 10)
			{
				throw new ArgumentOutOfRangeException("materials");
			}
			Material[] array = ArrayCache.Get(materials.Count);
			for (int i = 0; i < materials.Count; i++)
			{
				array[i] = materials[i];
			}
			renderer.set_sharedMaterials(array);
		}
	}
}
