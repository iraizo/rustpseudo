using UnityEngine;

public class ExplosionsScaleCurves : MonoBehaviour
{
	public AnimationCurve ScaleCurveX = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public AnimationCurve ScaleCurveY = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public AnimationCurve ScaleCurveZ = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public Vector3 GraphTimeMultiplier = Vector3.get_one();

	public Vector3 GraphScaleMultiplier = Vector3.get_one();

	private float startTime;

	private Transform t;

	private float evalX;

	private float evalY;

	private float evalZ;

	private void Awake()
	{
		t = ((Component)this).get_transform();
	}

	private void OnEnable()
	{
		startTime = Time.get_time();
		evalX = 0f;
		evalY = 0f;
		evalZ = 0f;
	}

	private void Update()
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.get_time() - startTime;
		if (num <= GraphTimeMultiplier.x)
		{
			evalX = ScaleCurveX.Evaluate(num / GraphTimeMultiplier.x) * GraphScaleMultiplier.x;
		}
		if (num <= GraphTimeMultiplier.y)
		{
			evalY = ScaleCurveY.Evaluate(num / GraphTimeMultiplier.y) * GraphScaleMultiplier.y;
		}
		if (num <= GraphTimeMultiplier.z)
		{
			evalZ = ScaleCurveZ.Evaluate(num / GraphTimeMultiplier.z) * GraphScaleMultiplier.z;
		}
		t.set_localScale(new Vector3(evalX, evalY, evalZ));
	}

	public ExplosionsScaleCurves()
		: this()
	{
	}//IL_005e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0063: Unknown result type (might be due to invalid IL or missing references)
	//IL_0069: Unknown result type (might be due to invalid IL or missing references)
	//IL_006e: Unknown result type (might be due to invalid IL or missing references)

}
