using UnityEngine;

public class LightEx : UpdateBehaviour, IClientComponent
{
	public bool alterColor;

	public float colorTimeScale = 1f;

	public Color colorA = Color.get_red();

	public Color colorB = Color.get_yellow();

	public AnimationCurve blendCurve = new AnimationCurve();

	public bool loopColor = true;

	public bool alterIntensity;

	public float intensityTimeScale = 1f;

	public AnimationCurve intenseCurve = new AnimationCurve();

	public float intensityCurveScale = 3f;

	public bool loopIntensity = true;

	public bool randomOffset;

	public float randomIntensityStartScale = -1f;

	protected void OnValidate()
	{
		CheckConflict(((Component)this).get_gameObject());
	}

	public static bool CheckConflict(GameObject go)
	{
		return false;
	}
}
