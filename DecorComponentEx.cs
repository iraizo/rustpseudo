using UnityEngine;

public static class DecorComponentEx
{
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		foreach (DecorComponent decorComponent in components)
		{
			if (!decorComponent.isRoot)
			{
				break;
			}
			decorComponent.Apply(ref pos, ref rot, ref scale);
		}
	}

	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = transform.get_position();
		Quaternion rot = transform.get_rotation();
		Vector3 scale = transform.get_localScale();
		transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
		transform.set_position(pos);
		transform.set_rotation(rot);
		transform.set_localScale(scale);
	}

	public static void ApplyDecorComponentsScaleOnly(this Transform transform, DecorComponent[] components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = transform.get_position();
		Quaternion rot = transform.get_rotation();
		Vector3 scale = transform.get_localScale();
		transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
		transform.set_localScale(scale);
	}
}
