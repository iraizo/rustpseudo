using UnityEngine;

public class EnvironmentVolumeTrigger : MonoBehaviour
{
	[HideInInspector]
	public Vector3 Center = Vector3.get_zero();

	[HideInInspector]
	public Vector3 Size = Vector3.get_one();

	public EnvironmentVolume volume { get; private set; }

	protected void Awake()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		volume = ((Component)this).get_gameObject().GetComponent<EnvironmentVolume>();
		if ((Object)(object)volume == (Object)null)
		{
			volume = ((Component)this).get_gameObject().AddComponent<EnvironmentVolume>();
			volume.Center = Center;
			volume.Size = Size;
			volume.UpdateTrigger();
		}
	}

	public EnvironmentVolumeTrigger()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0011: Unknown result type (might be due to invalid IL or missing references)

}
