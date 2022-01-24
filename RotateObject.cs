using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public float rotateSpeed_X = 1f;

	public float rotateSpeed_Y = 1f;

	public float rotateSpeed_Z = 1f;

	public bool localSpace;

	private Vector3 rotateVector;

	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		rotateVector = new Vector3(rotateSpeed_X, rotateSpeed_Y, rotateSpeed_Z);
	}

	private void Update()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (localSpace)
		{
			((Component)this).get_transform().Rotate(rotateVector * Time.get_deltaTime(), (Space)1);
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
