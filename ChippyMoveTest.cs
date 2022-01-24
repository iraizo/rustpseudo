using UnityEngine;

public class ChippyMoveTest : MonoBehaviour
{
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	public float speed = 0.2f;

	public float maxSpeed = 1f;

	private void FixedUpdate()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Mathf.Abs(((Vector3)(ref heading)).get_magnitude()) > 0f) ? 1f : 0f);
		speed = Mathf.MoveTowards(speed, maxSpeed * num, Time.get_fixedDeltaTime() * ((num == 0f) ? 2f : 2f));
		Vector3 position = ((Component)this).get_transform().get_position();
		Vector3 val = new Vector3(heading.x, heading.y, 0f);
		Ray val2 = default(Ray);
		((Ray)(ref val2))._002Ector(position, ((Vector3)(ref val)).get_normalized());
		if (!Physics.Raycast(val2, speed * Time.get_fixedDeltaTime(), 16777216))
		{
			Transform transform = ((Component)this).get_transform();
			transform.set_position(transform.get_position() + ((Ray)(ref val2)).get_direction() * Time.get_fixedDeltaTime() * speed);
			if (Mathf.Abs(((Vector3)(ref heading)).get_magnitude()) > 0f)
			{
				Transform transform2 = ((Component)this).get_transform();
				Vector3 forward = ((Component)this).get_transform().get_forward();
				val = new Vector3(heading.x, heading.y, 0f);
				transform2.set_rotation(QuaternionEx.LookRotationForcedUp(forward, ((Vector3)(ref val)).get_normalized()));
			}
		}
	}

	public ChippyMoveTest()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)

}
