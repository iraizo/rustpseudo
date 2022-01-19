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
				string value = serverHasAnyOf[i];
				if (((ServerInfo)(ref serverInfo)).get_Tags().Contains(value))
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
				string value2 = serverHasNoneOf[j];
				if (((ServerInfo)(ref serverInfo)).get_Tags().Contains(value2))
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
