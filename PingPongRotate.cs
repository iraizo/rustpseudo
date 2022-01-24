using UnityEngine;

public class PingPongRotate : MonoBehaviour
{
	public Vector3 rotationSpeed = Vector3.get_zero();

	public Vector3 offset = Vector3.get_zero();

	public Vector3 rotationAmount = Vector3.get_zero();

	private void Update()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.get_identity();
		for (int i = 0; i < 3; i++)
		{
			val *= GetRotation(i);
		}
		((Component)this).get_transform().set_rotation(val);
	}

	public Quaternion GetRotation(int index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_zero();
		switch (index)
		{
		case 0:
			val = Vector3.get_right();
			break;
		case 1:
			val = Vector3.get_up();
			break;
		case 2:
			val = Vector3.get_forward();
			break;
		}
		return Quaternion.AngleAxis(Mathf.Sin((((Vector3)(ref offset)).get_Item(index) + Time.get_time()) * ((Vector3)(ref rotationSpeed)).get_Item(index)) * ((Vector3)(ref rotationAmount)).get_Item(index), val);
	}

	public PingPongRotate()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)

}
