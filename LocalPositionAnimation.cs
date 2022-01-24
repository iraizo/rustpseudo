using UnityEngine;

public class LocalPositionAnimation : MonoBehaviour, IClientComponent
{
	public Vector3 centerPosition;

	public bool worldSpace;

	public float scaleX = 1f;

	public float timeScaleX = 1f;

	public AnimationCurve movementX = new AnimationCurve();

	public float scaleY = 1f;

	public float timeScaleY = 1f;

	public AnimationCurve movementY = new AnimationCurve();

	public float scaleZ = 1f;

	public float timeScaleZ = 1f;

	public AnimationCurve movementZ = new AnimationCurve();

	public LocalPositionAnimation()
		: this()
	{
	}//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_0021: Expected O, but got Unknown
	//IL_0038: Unknown result type (might be due to invalid IL or missing references)
	//IL_0042: Expected O, but got Unknown
	//IL_0059: Unknown result type (might be due to invalid IL or missing references)
	//IL_0063: Expected O, but got Unknown

}
