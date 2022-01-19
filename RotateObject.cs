using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public float rotateSpeed_X = 1f;

	public float rotateSpeed_Y = 1f;

	public float rotateSpeed_Z = 1f;

	public bool localSpace;

	protected void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (localSpace)
		{
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(rotateSpeed_X, rotateSpeed_Y, rotateSpeed_Z);
			((Component)this).get_transform().Rotate(val * Time.get_deltaTime(), (Space)1);
			return;
		}
		if (rotateSpeed_X != 0f)
		{
			((Component)this).get_transform().Rotate(Vector3.get_up(), Time.get_deltaTime() * rotateSpeed_X);
		}
		if (rotateSpeed_Y != 0f)
		{
			((Component)this).get_transform().Rotate(((Component)this).get_transform().get_forward(), Time.get_deltaTime() * rotateSpeed_Y);
		}
		if (rotateSpeed_Z != 0f)
		{
			((Component)this).get_transform().Rotate(((Component)this).get_transform().get_right(), Time.get_deltaTime() * rotateSpeed_Z);
		}
	}

	public RotateObject()
		: this()
	{
	}
}
