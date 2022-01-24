using UnityEngine;

public class ExplosionsShaderQueue : MonoBehaviour
{
	public int AddQueue = 1;

	private Renderer rend;

	private void Start()
	{
		rend = ((Component)this).GetComponent<Renderer>();
		if ((Object)(object)rend != (Object)null)
		{
			Material sharedMaterial = rend.get_sharedMaterial();
			sharedMaterial.set_renderQueue(sharedMaterial.get_renderQueue() + AddQueue);
		}
		else
		{
			((MonoBehaviour)this).Invoke("SetProjectorQueue", 0.1f);
		}
	}

	private void SetProjectorQueue()
	{
		Material material = ((Component)this).GetComponent<Projector>().get_material();
		material.set_renderQueue(material.get_renderQueue() + AddQueue);
	}

	private void OnDisable()
	{
		if ((Object)(object)rend != (Object)null)
		{
			rend.get_sharedMaterial().set_renderQueue(-1);
		}
	}

	public ExplosionsShaderQueue()
		: this()
	{
	}
}
