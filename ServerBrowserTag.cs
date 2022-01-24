using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerBrowserTag : MonoBehaviour
{
	public string[] serverHasAnyOf;

	public string[] serverHasNoneOf;

	public bool Test(in ServerInfo serverInfo)
	{
		if (serverHasAnyOf != null && serverHasAnyOf.Length != 0)
		{
			bool flag = false;
			for (int i = 0; i < serverHasAnyOf.Length; i++)
			{
				string text = serverHasAnyOf[i];
				if (Enumerable.Contains<string>((IEnumerable<string>)((ServerInfo)(ref serverInfo)).get_Tags(), text))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (serverHasNoneOf != null && serverHasNoneOf.Length != 0)
		{
			for (int j = 0; j < serverHasNoneOf.Length; j++)
			{
				string text2 = serverHasNoneOf[j];
				if (Enumerable.Contains<string>((IEnumerable<string>)((ServerInfo)(ref serverInfo)).get_Tags(), text2))
				{
					return false;
				}
			}
		}
		return true;
	}

	public ServerBrowserTag()
		: this()
	{
	}
}
