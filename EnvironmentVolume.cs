using UnityEngine;

public class EnvironmentVolume : MonoBehaviour
{
	[InspectorFlags]
	public EnvironmentType Type = EnvironmentType.Underground;

	public Vector3 Center = Vector3.get_zero();

	public Vector3 Size = Vector3.get_one();

	public Collider trigger { get; private set; }

	protected virtual void Awake()
	{
		UpdateTrigger();
	}

	public void UpdateTrigger()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)trigger))
		{
			trigger = ((Component)this).get_gameObject().GetComponent<Collider>();
		}
		if (!Object.op_Implicit((Object)(object)trigger))
		{
			trigger = (Collider)(object)((Component)this).get_gameObject().AddComponent<BoxCollider>();
		}
		trigger.set_isTrigger(true);
		Collider obj = trigger;
		BoxCollider val = (BoxCollider)(object)((obj is BoxCollider) ? obj : null);
		if (Object.op_Implicit((Object)(object)val))
		{
			val.set_center(Center);
			val.set_size(Size);
		}
	}

	public EnvironmentVolume()
		: this()
	{
	}//IL_0008: Unknown result type (might be due to invalid IL or missing references)
	//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0013: Unknown result type (might be due to invalid IL or missing references)
	//IL_0018: Unknown result type (might be due to invalid IL or missing references)

}
