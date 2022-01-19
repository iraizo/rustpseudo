using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConVar;
using UnityEngine;

public class SteamStatistics
{
	private BasePlayer player;

	public Dictionary<string, int> intStats = new Dictionary<string, int>();

	private Task refresh;

	public SteamStatistics(BasePlayer p)
	{
		player = p;
	}

	public void Init()
	{
		if (PlatformService.Instance.get_IsValid())
		{
			refresh = PlatformService.Instance.LoadPlayerStats(player.userID);
			intStats.Clear();
		}
	}

	public void Save()
	{
		if (PlatformService.Instance.get_IsValid())
		{
			PlatformService.Instance.SavePlayerStats(player.userID);
		}
	}

	public void Add(string name, int var)
	{
		if (!PlatformService.Instance.get_IsValid() || refresh == null || !refresh.IsCompleted)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("PlayerStats.Add", 0);
		try
		{
			int value = 0;
			if (intStats.TryGetValue(name, out value))
			{
				intStats[name] += var;
				PlatformService.Instance.SetPlayerStatInt(player.userID, name, (long)intStats[name]);
				return;
			}
			value = (int)PlatformService.Instance.GetPlayerStatInt(player.userID, name, 0L);
			if (!PlatformService.Instance.SetPlayerStatInt(player.userID, name, (long)(value + var)))
			{
				if (Global.developer > 0)
				{
					Debug.LogWarning((object)("[STEAMWORKS] Couldn't SetUserStat: " + name));
				}
			}
			else
			{
				intStats.Add(name, value + var);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
