using UnityEngine;

namespace ConVar
{
	[Factory("xmas")]
	public class XMas : ConsoleSystem
	{
		private const string path = "assets/prefabs/misc/xmas/xmasrefill.prefab";

		[ServerVar]
		public static bool enabled = false;

		[ServerVar]
		public static float spawnRange = 40f;

		[ServerVar]
		public static int spawnAttempts = 5;

		[ServerVar]
		public static int giftsPerPlayer = 2;

		[ServerVar]
		public static void refill(Arg arg)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/xmasrefill.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.Spawn();
			}
		}

		public XMas()
			: this()
		{
		}
	}
}
