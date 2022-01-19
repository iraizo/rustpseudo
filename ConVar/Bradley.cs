using UnityEngine;

namespace ConVar
{
	[Factory("bradley")]
	public class Bradley : ConsoleSystem
	{
		[ServerVar]
		public static float respawnDelayMinutes = 60f;

		[ServerVar]
		public static float respawnDelayVariance = 1f;

		[ServerVar]
		public static bool enabled = true;

		[ServerVar]
		public static void quickrespawn(Arg arg)
		{
			if (!Object.op_Implicit((Object)(object)arg.Player()))
			{
				return;
			}
			BradleySpawner singleton = BradleySpawner.singleton;
			if ((Object)(object)singleton == (Object)null)
			{
				Debug.LogWarning((object)"No Spawner");
				return;
			}
			if (Object.op_Implicit((Object)(object)singleton.spawned))
			{
				singleton.spawned.Kill();
			}
			singleton.spawned = null;
			singleton.DoRespawn();
		}

		public Bradley()
			: this()
		{
		}
	}
}
