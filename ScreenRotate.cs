using UnityEngine;

public class ScreenRotate : BaseScreenShake
{
	public AnimationCurve Pitch;

	public AnimationCurve Yaw;

	public AnimationCurve Roll;

	public AnimationCurve ViewmodelEffect;

	public bool useViewModelEffect = true;

	public override void Setup()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.get_zero();
		zero.x = Pitch.Evaluate(delta);
		zero.y = Yaw.Evaluate(delta);
		zero.z = Roll.Evaluate(delta);
		if ((bool)cam)
		{
			ref Quaternion rotation = ref cam.rotation;
			rotation *= Quaternion.Euler(zero);
		}
		if ((bool)vm && useViewModelEffect)
		{
			ref Quaternion rotation2 = ref vm.rotation;
			rotation2 *= Quaternion.Euler(zero * -1f * (1f - ViewmodelEffect.Evaluate(delta)));
		}
	}
}
