using UnityEngine;

public class PlayerDetectionTrigger : TriggerBase
{
	public BaseDetector myDetector;

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
		return ((Component)baseEntity).get_gameObject();
	}

	internal override void OnObjects()
	{
		base.OnObjects();
		myDetector.OnObjects();
	}

	internal override void OnEmpty()
	{
		base.OnEmpty();
		myDetector.OnEmpty();
	}
}
