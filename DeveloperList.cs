using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Facepunch.Models;
using UnityEngine;

public static class DeveloperList
{
	public static bool Contains(string steamid)
	{
		if (Application.Manifest == null)
		{
			return false;
		}
		if (Application.Manifest.Administrators == null)
		{
			return false;
		}
		return Enumerable.Any<Administrator>((IEnumerable<Administrator>)Application.Manifest.Administrators, (Func<Administrator, bool>)((Administrator x) => x.UserId == steamid));
	}

	public static bool Contains(ulong steamid)
	{
		return Contains(steamid.ToString());
	}

	public static bool IsDeveloper(BasePlayer ply)
	{
		if ((Object)(object)ply != (Object)null)
		{
			return Contains(ply.UserIDString);
		}
		return false;
	}
}
