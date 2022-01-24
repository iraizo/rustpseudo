using UnityEngine;

namespace ConVar
{
	[Factory("gamemode")]
	public class gamemode : ConsoleSystem
	{
		[ServerUserVar]
		public static void setteam(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null)
			{
				return;
			}
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
			if (Object.op_Implicit((Object)(object)activeGameMode))
			{
				int @int = arg.GetInt(0, 0);
				if (@int >= 0 && @int < activeGameMode.GetNumTeams())
				{
					activeGameMode.ResetPlayerScores(basePlayer);
					activeGameMode.SetPlayerTeam(basePlayer, @int);
					basePlayer.Respawn();
				}
			}
		}

		[ServerVar]
		public static void set(Arg arg)
		{
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			string @string = arg.GetString(0, "");
			if (string.IsNullOrEmpty(@string))
			{
				Debug.Log((object)"Invalid gamemode");
			}
			BaseGameMode baseGameMode = null;
			GameObjectRef gameObjectRef = null;
			GameModeManifest gameModeManifest = GameModeManifest.Get();
			Debug.Log((object)("total gamemodes : " + gameModeManifest.gameModePrefabs.Count));
			foreach (GameObjectRef gameModePrefab in gameModeManifest.gameModePrefabs)
			{
				BaseGameMode component = gameModePrefab.Get().GetComponent<BaseGameMode>();
				if (component.shortname == @string)
				{
					baseGameMode = component;
					gameObjectRef = gameModePrefab;
					Debug.Log((object)("Found :" + component.shortname + " prefab name is :" + component.PrefabName + ": rpath is " + gameModePrefab.resourcePath + ":"));
					break;
				}
				Debug.Log((object)("search name " + @string + "searched against : " + component.shortname));
			}
			if ((Object)(object)baseGameMode == (Object)null)
			{
				Debug.Log((object)("Unknown gamemode : " + @string));
				return;
			}
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
			if (Object.op_Implicit((Object)(object)activeGameMode))
			{
				if (baseGameMode.shortname == activeGameMode.shortname)
				{
					Debug.Log((object)"Same gamemode, resetting");
				}
				if (activeGameMode.permanent)
				{
					Debug.LogError((object)"This game mode is permanent, you must reset the server to switch game modes.");
					return;
				}
				activeGameMode.ShutdownGame();
				activeGameMode.Kill();
				BaseGameMode.SetActiveGameMode(null, serverside: true);
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(gameObjectRef.resourcePath, Vector3.get_zero(), Quaternion.get_identity());
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				Debug.Log((object)("Spawning new game mode : " + baseGameMode.shortname));
				baseEntity.Spawn();
			}
			else
			{
				Debug.Log((object)("Failed to create new game mode :" + baseGameMode.PrefabName));
			}
		}

		public gamemode()
			: this()
		{
		}
	}
}
