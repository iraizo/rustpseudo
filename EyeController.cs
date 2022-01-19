using UnityEngine;

public class EyeController : MonoBehaviour
{
	public const float MaxLookDot = 0.8f;

	public bool debug;

	public Transform LeftEye;

	public Transform RightEye;

	public Transform EyeTransform;

	public Vector3 Fudge = new Vector3(0f, 90f, 0f);

	public Vector3 FlickerRange;

	private Transform Focus;

	private float FocusUpdateTime;

	public EyeController()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)

}
