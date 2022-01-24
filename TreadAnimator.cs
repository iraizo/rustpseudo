using UnityEngine;

public class TreadAnimator : MonoBehaviour, IClientComponent
{
	public Animator mainBodyAnimator;

	public Transform[] wheelBones;

	public Vector3[] vecShocksOffsetPosition;

	public Vector3[] wheelBoneOrigin;

	public float wheelBoneDistMax = 0.26f;

	public Renderer treadRenderer;

	public Material leftTread;

	public Material rightTread;

	public TreadEffects treadEffects;

	public float traceThickness = 0.25f;

	public float heightFudge = 0.13f;

	public bool useWheelYOrigin;

	public Vector2 treadTextureDirection = new Vector2(1f, 0f);

	public bool isMetallic;

	public TreadAnimator()
		: this()
	{
	}//IL_002c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0031: Unknown result type (might be due to invalid IL or missing references)

}
