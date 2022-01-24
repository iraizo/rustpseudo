using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Recoil Properties")]
public class RecoilProperties : ScriptableObject
{
	public float recoilYawMin;

	public float recoilYawMax;

	public float recoilPitchMin;

	public float recoilPitchMax;

	public float timeToTakeMin;

	public float timeToTakeMax = 0.1f;

	public float ADSScale = 0.5f;

	public float movementPenalty;

	public float clampPitch = float.NegativeInfinity;

	public AnimationCurve pitchCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public AnimationCurve yawCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public bool useCurves;

	public int shotsUntilMax = 30;

	public RecoilProperties()
		: this()
	{
	}//IL_0034: Unknown result type (might be due to invalid IL or missing references)
	//IL_0039: Unknown result type (might be due to invalid IL or missing references)
	//IL_004a: Unknown result type (might be due to invalid IL or missing references)
	//IL_004f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0054: Unknown result type (might be due to invalid IL or missing references)
	//IL_005e: Expected O, but got Unknown
	//IL_0071: Unknown result type (might be due to invalid IL or missing references)
	//IL_0076: Unknown result type (might be due to invalid IL or missing references)
	//IL_0087: Unknown result type (might be due to invalid IL or missing references)
	//IL_008c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0091: Unknown result type (might be due to invalid IL or missing references)
	//IL_009b: Expected O, but got Unknown

}
