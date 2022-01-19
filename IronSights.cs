using UnityEngine;

public class IronSights : MonoBehaviour
{
	public bool Enabled;

	[Header("View Setup")]
	public IronsightAimPoint aimPoint;

	public float fieldOfViewOffset = -20f;

	public float zoomFactor = 1f;

	[Header("Animation")]
	public float introSpeed = 1f;

	public AnimationCurve introCurve = new AnimationCurve();

	public float outroSpeed = 1f;

	public AnimationCurve outroCurve = new AnimationCurve();

	[Header("Sounds")]
	public SoundDefinition upSound;

	public SoundDefinition downSound;

	[Header("Info")]
	public IronSightOverride ironsightsOverride;

	public bool processUltrawideOffset;

	public IronSights()
		: this()
	{
	}//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_002c: Expected O, but got Unknown
	//IL_0038: Unknown result type (might be due to invalid IL or missing references)
	//IL_0042: Expected O, but got Unknown

}
