using UnityEngine;

public class LineRendererActivate : MonoBehaviour, IClientComponent
{
	private void OnEnable()
	{
		((Renderer)((Component)this).GetComponent<LineRenderer>()).set_enabled(true);
	}

	public LineRendererActivate()
		: this()
	{
	}
}
