using UnityEngine;

public class ExplosionsDeactivateRendererByTime : MonoBehaviour
{
	public float TimeDelay = 1f;

	private Renderer rend;

	private void Awake()
	{
		rend = ((Component)this).GetComponent<Renderer>();
	}

	private void DeactivateRenderer()
	{
		rend.set_enabled(false);
	}

	private void OnEnable()
	{
		rend.set_enabled(true);
		((MonoBehaviour)this).Invoke("DeactivateRenderer", TimeDelay);
	}

	public ExplosionsDeactivateRendererByTime()
		: this()
	{
	}
}
