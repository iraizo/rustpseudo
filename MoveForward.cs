using UnityEngine;

public class MoveForward : MonoBehaviour
{
	public float Speed = 2f;

	protected void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).GetComponent<Rigidbody>().set_velocity(Speed * ((Component)this).get_transform().get_forward());
	}

	public MoveForward()
		: this()
	{
	}
}
