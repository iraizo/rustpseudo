using UnityEngine;

namespace ConVar
{
	[Factory("spawn")]
	public class Spawn : ConsoleSystem
	{
		[ServerVar]
		public static float min_rate = 0.5f;

		[ServerVar]
		public static float max_rate = 1f;

		[ServerVar]
		public static float min_density = 0.5f;

		[ServerVar]
		public static float max_density = 1f;

		[ServerVar]
		public static float player_base = 100f;

		[ServerVar]
		public static float player_scale = 2f;

		[ServerVar]
		public static bool respawn_populations = true;

		[ServerVar]
		public static bool respawn_groups = true;

		[ServerVar]
		public static bool respawn_individuals = true;

		[ServerVar]
		public static float tick_populations = 60f;

		[ServerVar]
		public static float tick_individuals = 300f;

		[ServerVar]
		public static void fill_populations(Arg args)
		{
			if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
			{
				SingletonComponent<SpawnHandler>.Instance.FillPopulations();
			}
		}

		[ServerVar]
		public static void fill_groups(Arg args)
		{
			if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
			{
				SingletonComponent<SpawnHandler>.Instance.FillGroups();
			}
		}

		[ServerVar]
		public static void fill_individuals(Arg args)
		{
			if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
			{
				SingletonComponent<SpawnHandler>.Instance.FillIndividuals();
			}
		}

		[ServerVar]
		public static void report(Arg args)
		{
			if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
			{
				args.ReplyWith(SingletonComponent<SpawnHandler>.Instance.GetReport(detailed: false));
			}
			else
			{
				args.ReplyWith("No spawn handler found.");
			}
		}

		[ServerVar]
		public static void scalars(Arg args)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			TextTable val = new TextTable();
			val.AddColumn("Type");
			val.AddColumn("Value");
			val.AddRow(new string[2]
			{
				"Player Fraction",
				SpawnHandler.PlayerFraction().ToString()
			});
			val.AddRow(new string[2]
			{
				"Player Excess",
				SpawnHandler.PlayerExcess().ToString()
			});
			val.AddRow(new string[2]
			{
				"Population Rate",
				SpawnHandler.PlayerLerp(min_rate, max_rate).ToString()
			});
			val.AddRow(new string[2]
			{
				"Population Density",
				SpawnHandler.PlayerLerp(min_density, max_density).ToString()
			});
			val.AddRow(new string[2]
			{
				"Group Rate",
				SpawnHandler.PlayerScale(player_scale).ToString()
			});
			args.ReplyWith(((object)val).ToString());
		}

		[ServerVar]
		public static void cargoshipevent(Arg args)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/content/vehicles/boats/cargoship/cargoshiptest.prefab");
			if ((Object)(object)baseEntity != (Object)null)
			{
				((Component)baseEntity).SendMessage("TriggeredEventSpawn", (SendMessageOptions)1);
				baseEntity.Spawn();
				args.ReplyWith("Cargo ship event has been started");
			}
			else
			{
				args.ReplyWith("Couldn't find cargo ship prefab - maybe it has been renamed?");
			}
		}

		public Spawn()
			: this()
		{
		}
	}
}
