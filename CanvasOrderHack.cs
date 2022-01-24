using UnityEngine;

public class CanvasOrderHack : MonoBehaviour
{
	private void OnEnable()
	{
		Canvas[] componentsInChildren = ((Component)this).GetComponentsInChildren<Canvas>(true);
		foreach (Canvas val in componentsInChildren)
		{
			if (val.get_overrideSorting())
			{
				int sortingOrder = val.get_sortingOrder();
				val.set_sortingOrder(sortingOrder + 1);
			}
		}
		componentsInChildren = ((Component)this).GetComponentsInChildren<Canvas>(true);
		foreach (Canvas val2 in componentsInChildren)
		{
			if (val2.get_overrideSorting())
			{
				int sortingOrder = val2.get_sortingOrder();
				val2.set_sortingOrder(sortingOrder - 1);
			}
		}
	}

	public CanvasOrderHack()
		: this()
	{
	}
}
