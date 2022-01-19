using UnityEngine;
using UnityEngine.Events;

public class ItemEventFlag : MonoBehaviour, IItemUpdate
{
	public Item.Flag flag;

	public UnityEvent onEnabled = new UnityEvent();

	public UnityEvent onDisable = new UnityEvent();

	internal bool firstRun = true;

	internal bool lastState;

	public virtual void OnItemUpdate(Item item)
	{
		bool flag = item.HasFlag(this.flag);
		if (firstRun || flag != lastState)
		{
			if (flag)
			{
				onEnabled.Invoke();
			}
			else
			{
				onDisable.Invoke();
			}
			lastState = flag;
			firstRun = false;
		}
	}

	public ItemEventFlag()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_000b: Expected O, but got Unknown
	//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0016: Expected O, but got Unknown

}
