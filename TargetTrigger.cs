using UnityEngine;

public class TargetTrigger : TriggerBase
{
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if ((Object)(object)losEyes != (Object)null && !baseEntity.IsVisible(((Component)losEyes).get_transform().get_position(), baseEntity.CenterPoint()))
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}
}
