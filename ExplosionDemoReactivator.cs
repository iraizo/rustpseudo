using UnityEngine;

public class ExplosionDemoReactivator : MonoBehaviour
{
	public float TimeDelayToReactivate = 3f;

	private void Start()
	{
		((MonoBehaviour)this).InvokeRepeating("Reactivate", 0f, TimeDelayToReactivate);
	}

	private void Reactivate()
	{
		Transform[] componentsInChildren = ((Component)this).GetComponentsInChildren<Transform>();
		foreach (Transform obj in componentsInChildren)
		{
			((Component)obj).get_gameObject().SetActive(false);
			((Component)obj).get_gameObject().SetActive(true);
		}
	}

	public ExplosionDemoReactivator()
		: this()
	{
	}
}
