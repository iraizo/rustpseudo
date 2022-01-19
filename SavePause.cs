using UnityEngine;

public class SavePause : MonoBehaviour, IServerComponent
{
	protected void Awake()
	{
		Debug.LogError((object)"SavePause is deprecated and doesn't do anything anymore! Remove me!", (Object)(object)this);
	}

	public SavePause()
		: this()
	{
	}
}
