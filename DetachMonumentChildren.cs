using UnityEngine;

public class DetachMonumentChildren : MonoBehaviour
{
	private void Awake()
	{
		((Component)this).get_transform().DetachChildren();
	}

	public DetachMonumentChildren()
		: this()
	{
	}
}
