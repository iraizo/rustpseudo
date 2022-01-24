using UnityEngine;

public class ScaleBySpeed : MonoBehaviour
{
	public float minScale = 0.001f;

	public float maxScale = 1f;

	public float minSpeed;

	public float maxSpeed = 1f;

	public MonoBehaviour component;

	public bool toggleComponent = true;

	public bool onlyWhenSubmerged;

	public float submergedThickness = 0.33f;

	private Vector3 prevPosition = Vector3.get_zero();

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		prevPosition = ((Component)this).get_transform().get_position();
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).get_transform().get_position();
		Vector3 val = position - prevPosition;
		float sqrMagnitude = ((Vector3)(ref val)).get_sqrMagnitude();
		float num = minScale;
		bool enabled = WaterSystem.GetHeight(position) > position.y - submergedThickness;
		if (sqrMagnitude > 0.0001f)
		{
			sqrMagnitude = Mathf.Sqrt(sqrMagnitude);
			float num2 = Mathf.Clamp(sqrMagnitude, minSpeed, maxSpeed) / (maxSpeed - minSpeed);
			num = Mathf.Lerp(minScale, maxScale, Mathf.Clamp01(num2));
			if ((Object)(object)component != (Object)null && toggleComponent)
			{
				((Behaviour)component).set_enabled(enabled);
			}
		}
		else if ((Object)(object)component != (Object)null && toggleComponent)
		{
			((Behaviour)component).set_enabled(false);
		}
		((Component)this).get_transform().set_localScale(new Vector3(num, num, num));
		prevPosition = position;
	}

	public ScaleBySpeed()
		: this()
	{
	}//IL_0034: Unknown result type (might be due to invalid IL or missing references)
	//IL_0039: Unknown result type (might be due to invalid IL or missing references)

}
