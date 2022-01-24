using UnityEngine;

public class LayerCullDistance : MonoBehaviour
{
	public string Layer = "Default";

	public float Distance = 1000f;

	protected void OnEnable()
	{
		Camera component = ((Component)this).GetComponent<Camera>();
		float[] layerCullDistances = component.get_layerCullDistances();
		layerCullDistances[LayerMask.NameToLayer(Layer)] = Distance;
		component.set_layerCullDistances(layerCullDistances);
	}

	public LayerCullDistance()
		: this()
	{
	}
}
