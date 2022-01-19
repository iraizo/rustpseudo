using System;
using UnityEngine;

public class DecorSocketMale : PrefabAttribute
{
	protected override Type GetIndexedType()
	{
		return typeof(DecorSocketMale);
	}

	protected void OnDrawGizmos()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(new Color(0.5f, 0.5f, 1f, 1f));
		Gizmos.DrawSphere(((Component)this).get_transform().get_position(), 1f);
	}
}
