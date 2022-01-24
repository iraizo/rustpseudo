using Rust;
using UnityEngine;

public class SavePause : MonoBehaviour, IServerComponent
{
	private bool tracked;

	protected void OnEnable()
	{
		if (Object.op_Implicit((Object)(object)SingletonComponent<SaveRestore>.Instance) && !tracked)
		{
			tracked = true;
			SingletonComponent<SaveRestore>.Instance.timedSavePause++;
		}
	}

	protected void OnDisable()
	{
		if (!Application.isQuitting && Object.op_Implicit((Object)(object)SingletonComponent<SaveRestore>.Instance) && tracked)
		{
			tracked = false;
			SingletonComponent<SaveRestore>.Instance.timedSavePause--;
		}
	}

	public SavePause()
		: this()
	{
	}
}
