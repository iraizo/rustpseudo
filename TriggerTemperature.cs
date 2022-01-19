using ConVar;
using UnityEngine;

public class TriggerTemperature : TriggerBase
{
	public float Temperature = 50f;

	public float triggerSize;

	public float minSize;

	public bool sunlightBlocker;

	public float sunlightBlockAmount;

	[Range(0f, 24f)]
	public float blockMinHour = 8.5f;

	[Range(0f, 24f)]
	public float blockMaxHour = 18.5f;

	private void OnValidate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		triggerSize = ((Component)this).GetComponent<SphereCollider>().get_radius() * ((Component)this).get_transform().get_localScale().y;
	}

	public float WorkoutTemperature(Vector3 position, float oldTemperature)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (sunlightBlocker)
		{
			float time = Env.time;
			if (time >= blockMinHour && time <= blockMaxHour)
			{
				Vector3 position2 = TOD_Sky.get_Instance().get_Components().get_SunTransform()
					.get_position();
				if (!GamePhysics.LineOfSight(position, position2, 256))
				{
					return oldTemperature - sunlightBlockAmount;
				}
			}
			return oldTemperature;
		}
		float num = Vector3.Distance(((Component)this).get_gameObject().get_transform().get_position(), position);
		float num2 = Mathf.InverseLerp(triggerSize, minSize, num);
		return Mathf.Lerp(oldTemperature, Temperature, num2);
	}

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
}
