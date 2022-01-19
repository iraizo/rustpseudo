using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
	[Range(-10f, 10f)]
	public float speed = 1f;

	public Vector3 position;

	public Vector3 rotation;

	public Vector3 scale;

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).get_transform();
		Quaternion val = ((Component)this).get_transform().get_rotation();
		transform.set_rotation(Quaternion.Euler(((Quaternion)(ref val)).get_eulerAngles() + rotation * speed * Time.get_deltaTime()));
		Transform transform2 = ((Component)this).get_transform();
		transform2.set_localScale(transform2.get_localScale() + scale * speed * Time.get_deltaTime());
		Transform transform3 = ((Component)this).get_transform();
		transform3.set_localPosition(transform3.get_localPosition() + position * speed * Time.get_deltaTime());
	}

	public MoveOverTime()
		: this()
	{
	}
}
