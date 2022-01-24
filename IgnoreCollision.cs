using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
	public Collider collider;

	protected void OnTriggerEnter(Collider other)
	{
		Debug.Log((object)("IgnoreCollision: " + ((Object)((Component)collider).get_gameObject()).get_name() + " + " + ((Object)((Component)other).get_gameObject()).get_name()));
		Physics.IgnoreCollision(other, collider, true);
	}

	public IgnoreCollision()
		: this()
	{
	}
}
