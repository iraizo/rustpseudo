using UnityEngine;

public class ScreenFov : BaseScreenShake
{
	public AnimationCurve FovAdjustment;

	public override void Setup()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if ((bool)cam)
		{
			Camera component = cam.component;
			component.set_fieldOfView(component.get_fieldOfView() + FovAdjustment.Evaluate(delta));
		}
	}
}
