using UnityEngine;

public class ConvarToggleChildren : MonoBehaviour
{
	public string ConvarName;

	public string ConvarEnabled = "True";

	private bool state;

	private Command Command;

	protected void Awake()
	{
		Command = Client.Find(ConvarName);
		if (Command == null)
		{
			Command = Server.Find(ConvarName);
		}
		if (Command != null)
		{
			SetState(Command.get_String() == ConvarEnabled);
		}
	}

	protected void Update()
	{
		if (Command != null)
		{
			bool flag = Command.get_String() == ConvarEnabled;
			if (state != flag)
			{
				SetState(flag);
			}
		}
	}

	private void SetState(bool newState)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in ((Component)this).get_transform())
		{
			((Component)item).get_gameObject().SetActive(newState);
		}
		state = newState;
	}

	public ConvarToggleChildren()
		: this()
	{
	}
}
