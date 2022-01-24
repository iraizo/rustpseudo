using UnityEngine;

public class FlashlightBeam : MonoBehaviour, IClientComponent
{
	public Vector2 scrollDir;

	public Vector3 localEndPoint = new Vector3(0f, 0f, 2f);

	public LineRenderer beamRenderer;

	public FlashlightBeam()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)

}
