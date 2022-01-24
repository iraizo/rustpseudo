using UnityEngine;

public class EyeBlink : MonoBehaviour
{
	public Transform LeftEye;

	public Transform LeftEyelid;

	public Vector3 LeftEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	public Transform RightEye;

	public Transform RightEyelid;

	public Vector3 RightEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	public Vector3 ClosedEyelidPosition;

	public Vector3 ClosedEyelidRotation;

	public Vector2 TimeWithoutBlinking = new Vector2(1f, 10f);

	public float BlinkSpeed = 0.2f;

	public Vector3 LeftEyeInitial;

	public Vector3 RightEyeInitial;

	public EyeBlink()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_002a: Unknown result type (might be due to invalid IL or missing references)
	//IL_002f: Unknown result type (might be due to invalid IL or missing references)
	//IL_003f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0044: Unknown result type (might be due to invalid IL or missing references)

}
