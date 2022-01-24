using UnityEngine;

public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	public bool isDynamic;

	public float enabledRadius = 20f;

	public bool toggleFade;

	public float toggleFadeDuration = 0.5f;

	protected void OnValidate()
	{
		LightEx.CheckConflict(((Component)this).get_gameObject());
	}

	public AmbientLightLOD()
		: this()
	{
	}
}
