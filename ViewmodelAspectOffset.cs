using UnityEngine;

public class ViewmodelAspectOffset : MonoBehaviour
{
	public Vector3 OffsetAmount = Vector3.get_zero();

	[Tooltip("What aspect ratio should we start moving the viewmodel? 16:9 = 1.7, 21:9 = 2.3")]
	public float aspectCutoff = 2f;

	public ViewmodelAspectOffset()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)

}
