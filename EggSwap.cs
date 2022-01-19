using UnityEngine;

public class EggSwap : MonoBehaviour
{
	public Renderer[] eggRenderers;

	public void Show(int index)
	{
		HideAll();
		eggRenderers[index].set_enabled(true);
	}

	public void HideAll()
	{
		Renderer[] array = eggRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].set_enabled(false);
		}
	}

	public EggSwap()
		: this()
	{
	}
}
