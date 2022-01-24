using UnityEngine;

public class UIBackgroundBlur : ListComponent<UIBackgroundBlur>, IClientComponent
{
	public float amount = 1f;

	public static float currentMax
	{
		get
		{
			if (ListComponent<UIBackgroundBlur>.InstanceList.get_Count() == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < ListComponent<UIBackgroundBlur>.InstanceList.get_Count(); i++)
			{
				num = Mathf.Max(ListComponent<UIBackgroundBlur>.InstanceList.get_Item(i).amount, num);
			}
			return num;
		}
	}
}
