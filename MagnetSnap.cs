using UnityEngine;

public class MagnetSnap
{
	private Transform snapLocation;

	private Vector3 prevSnapLocation;

	public MagnetSnap(Transform snapLocation)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		this.snapLocation = snapLocation;
		prevSnapLocation = snapLocation.get_position();
	}

	public void FixedUpdate(Transform target)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		PositionTarget(target);
		if (snapLocation.get_hasChanged())
		{
			prevSnapLocation = snapLocation.get_position();
			snapLocation.set_hasChanged(false);
		}
	}

	public void PositionTarget(Transform target)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)target == (Object)null))
		{
			Transform transform = ((Component)target).get_transform();
			Quaternion val = snapLocation.get_rotation();
			if (Vector3.Angle(transform.get_forward(), snapLocation.get_forward()) > 90f)
			{
				val *= Quaternion.Euler(0f, 180f, 0f);
			}
			if (transform.get_position() != snapLocation.get_position())
			{
				transform.set_position(transform.get_position() + (snapLocation.get_position() - prevSnapLocation));
				transform.set_position(Vector3.MoveTowards(transform.get_position(), snapLocation.get_position(), 1f * Time.get_fixedDeltaTime()));
			}
			if (transform.get_rotation() != val)
			{
				transform.set_rotation(Quaternion.RotateTowards(transform.get_rotation(), val, 40f * Time.get_fixedDeltaTime()));
			}
		}
	}
}
