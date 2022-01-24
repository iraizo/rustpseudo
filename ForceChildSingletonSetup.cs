using UnityEngine;

public class ForceChildSingletonSetup : MonoBehaviour
{
	[ComponentHelp("Any child objects of this object that contain SingletonComponents will be registered - even if they're not enabled")]
	private void Awake()
	{
		SingletonComponent[] componentsInChildren = ((Component)this).GetComponentsInChildren<SingletonComponent>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SingletonSetup();
		}
	}

	public ForceChildSingletonSetup()
		: this()
	{
	}
}
