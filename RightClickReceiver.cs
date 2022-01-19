using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class RightClickReceiver : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public UnityEvent ClickReceiver;

	public void OnPointerClick(PointerEventData eventData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)eventData.get_button() == 1)
		{
			UnityEvent clickReceiver = ClickReceiver;
			if (clickReceiver != null)
			{
				clickReceiver.Invoke();
			}
		}
	}

	public RightClickReceiver()
		: this()
	{
	}
}
