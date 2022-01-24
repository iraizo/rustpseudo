using UnityEngine;

public class EntityFlag_ToggleNotify : EntityFlag_Toggle
{
	public bool UseEntityParent;

	protected override void OnStateToggled(bool state)
	{
		base.OnStateToggled(state);
		IFlagNotify flagNotify;
		if (!UseEntityParent && (Object)(object)base.baseEntity != (Object)null && (flagNotify = base.baseEntity as IFlagNotify) != null)
		{
			flagNotify.OnFlagToggled(state);
		}
		IFlagNotify flagNotify2;
		if (UseEntityParent && (Object)(object)base.baseEntity != (Object)null && (Object)(object)base.baseEntity.GetParentEntity() != (Object)null && (flagNotify2 = base.baseEntity.GetParentEntity() as IFlagNotify) != null)
		{
			flagNotify2.OnFlagToggled(state);
		}
	}
}
