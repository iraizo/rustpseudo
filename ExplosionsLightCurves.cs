using UnityEngine;

public class ExplosionsLightCurves : MonoBehaviour
{
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float GraphTimeMultiplier = 1f;

	public float GraphIntensityMultiplier = 1f;

	private bool canUpdate;

	private float startTime;

	private Light lightSource;

	private void Awake()
	{
		lightSource = ((Component)this).GetComponent<Light>();
		lightSource.set_intensity(LightCurve.Evaluate(0f));
	}

	private void OnEnable()
	{
		startTime = Time.get_time();
		canUpdate = true;
	}

	private void Update()
	{
		float num = Time.get_time() - startTime;
		if (canUpdate)
		{
			float intensity = LightCurve.Evaluate(num / GraphTimeMultiplier) * GraphIntensityMultiplier;
			lightSource.set_intensity(intensity);
		}
		if (num >= GraphTimeMultiplier)
		{
			canUpdate = false;
		}
	}

	public ExplosionsLightCurves()
		: this()
	{
	}
}
