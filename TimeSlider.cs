using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
	private Slider slider;

	private void Start()
	{
		slider = ((Component)this).GetComponent<Slider>();
	}

	private void Update()
	{
		if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
		{
			slider.set_value(TOD_Sky.get_Instance().Cycle.Hour);
		}
	}

	public void OnValue(float f)
	{
		if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
		{
			TOD_Sky.get_Instance().Cycle.Hour = f;
			TOD_Sky.get_Instance().UpdateAmbient();
			TOD_Sky.get_Instance().UpdateReflection();
			TOD_Sky.get_Instance().UpdateFog();
		}
	}

	public TimeSlider()
		: this()
	{
	}
}
