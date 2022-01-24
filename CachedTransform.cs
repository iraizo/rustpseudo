using UnityEngine;

public struct CachedTransform<T> where T : Component
{
	public T component;

	public Vector3 position;

	public Quaternion rotation;

	public Vector3 localScale;

	public Matrix4x4 localToWorldMatrix => Matrix4x4.TRS(position, rotation, localScale);

	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 val = localToWorldMatrix;
			return ((Matrix4x4)(ref val)).get_inverse();
		}
	}

	public Vector3 forward => rotation * Vector3.get_forward();

	public Vector3 up => rotation * Vector3.get_up();

	public Vector3 right => rotation * Vector3.get_right();

	public CachedTransform(T instance)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		component = instance;
		if (Object.op_Implicit((Object)(object)component))
		{
			position = ((Component)component).get_transform().get_position();
			rotation = ((Component)component).get_transform().get_rotation();
			localScale = ((Component)component).get_transform().get_localScale();
		}
		else
		{
			position = Vector3.get_zero();
			rotation = Quaternion.get_identity();
			localScale = Vector3.get_one();
		}
	}

	public void Apply()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)component))
		{
			((Component)component).get_transform().SetPositionAndRotation(position, rotation);
			((Component)component).get_transform().set_localScale(localScale);
		}
	}

	public void RotateAround(Vector3 center, Vector3 axis, float angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.AngleAxis(angle, axis);
		Vector3 val2 = val * (position - center);
		position = center + val2;
		rotation *= Quaternion.Inverse(rotation) * val * rotation;
	}

	public static implicit operator bool(CachedTransform<T> instance)
	{
		return (Object)(object)instance.component != (Object)null;
	}
}
