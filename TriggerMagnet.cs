using UnityEngine;

public class TriggerMagnet : TriggerBase
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
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!baseEntity.syncPosition)
		{
			return null;
		}
		if (!Object.op_Implicit((Object)(object)((Component)baseEntity).GetComponent<MagnetLiftable>()))
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}
}
