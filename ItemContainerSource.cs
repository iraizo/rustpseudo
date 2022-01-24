using UnityEngine;

public abstract class ItemContainerSource : MonoBehaviour
{
	public abstract ItemContainer GetItemContainer();

	protected ItemContainerSource()
		: this()
	{
	}
}
