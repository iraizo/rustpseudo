using ConVar;
using UnityEngine;

public class FPSGraph : Graph
{
	public void Refresh()
	{
		((Behaviour)this).set_enabled(FPS.graph > 0);
		((Rect)(ref Area)).set_width((float)(Resolution = Mathf.Clamp(FPS.graph, 0, Screen.get_width())));
	}

	protected void OnEnable()
	{
		Refresh();
	}

	protected override float GetValue()
	{
		return 1f / Time.get_deltaTime();
	}

	protected override Color GetColor(float value)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!(value < 10f))
		{
			if (!(value < 30f))
			{
				return Color.get_green();
			}
			return Color.get_yellow();
		}
		return Color.get_red();
	}
}
