using System;
using UnityEngine;

public class RunConsoleCommand : MonoBehaviour
{
	public void ClientRun(string command)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ConsoleSystem.Run(Option.get_Client(), command, Array.Empty<object>());
	}

	public RunConsoleCommand()
		: this()
	{
	}
}
