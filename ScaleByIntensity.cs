using UnityEngine;

public class ScaleByIntensity : MonoBehaviour
{
	public Vector3 initialScale = Vector3.get_zero();

	public Light intensitySource;

	public float maxIntensity = 1f;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		initialScale = ((Component)this).get_transform().get_localScale();
	}

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().set_localScale(((Behaviour)intensitySource).get_enabled() ? (initialScale * intensitySource.get_intensity() / maxIntensity) : Vector3.get_zero());
	}

	public ScaleByIntensity()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)

}
