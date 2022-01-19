using UnityEngine;

public class VitalRadial : MonoBehaviour
{
	private void Awake()
	{
		Debug.LogWarning((object)("VitalRadial is obsolete " + ((Component)this).get_transform().GetRecursiveName()), (Object)(object)((Component)this).get_gameObject());
	}

	public VitalRadial()
		: this()
	{
	}
}
