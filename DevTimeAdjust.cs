using UnityEngine;

public class DevTimeAdjust : MonoBehaviour
{
	private void Start()
	{
		if (Object.op_Implicit((Object)(object)TOD_Sky.get_Instance()))
		{
			TOD_Sky.get_Instance().Cycle.Hour = PlayerPrefs.GetFloat("DevTime");
		}
	}

	private void OnGUI()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)TOD_Sky.get_Instance()))
		{
			float num = (float)Screen.get_width() * 0.2f;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector((float)Screen.get_width() - (num + 20f), (float)Screen.get_height() - 30f, num, 20f);
			float hour = TOD_Sky.get_Instance().Cycle.Hour;
			hour = GUI.HorizontalSlider(val, hour, 0f, 24f);
			((Rect)(ref val)).set_y(((Rect)(ref val)).get_y() - 20f);
			GUI.Label(val, "Time Of Day");
			if (hour != TOD_Sky.get_Instance().Cycle.Hour)
			{
				TOD_Sky.get_Instance().Cycle.Hour = hour;
				PlayerPrefs.SetFloat("DevTime", hour);
			}
		}
	}

	public DevTimeAdjust()
		: this()
	{
	}
}
