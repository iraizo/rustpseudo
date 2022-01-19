using UnityEngine;

[ExecuteInEditMode]
public class AdaptMeshToTerrain : MonoBehaviour
{
	public LayerMask LayerMask = LayerMask.op_Implicit(-1);

	public float RayHeight = 10f;

	public float RayMaxDistance = 20f;

	public float MinDisplacement = 0.01f;

	public float MaxDisplacement = 0.33f;

	[Range(8f, 64f)]
	public int PlaneResolution = 24;

	public AdaptMeshToTerrain()
		: this()
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0007: Unknown result type (might be due to invalid IL or missing references)

}
