using UnityEngine;

public class TriggerLadder : TriggerBase
{
	internal override GameObject InterestedInObject(GameObject obj)
	{
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
		if ((Object)(object)(baseEntity as BasePlayer) == (Object)null)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}
}
