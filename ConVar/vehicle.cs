using UnityEngine;

namespace ConVar
{
	[Factory("vehicle")]
	public class vehicle : ConsoleSystem
	{
		[ServerVar]
		[Help("how long until boat corpses despawn")]
		public static float boat_corpse_seconds = 300f;

		[ServerVar(Help = "If true, trains always explode when destroyed, and hitting a barrier always destroys the train immediately. Default: false")]
		public static bool cinematictrains = false;

		[ServerVar(Help = "Determines whether modular cars turn into wrecks when destroyed, or just immediately gib. Default: true")]
		public static bool carwrecks = true;

		[ServerVar(Help = "Determines whether vehicles drop storage items when destroyed. Default: true")]
		public static bool vehiclesdroploot = true;

		[ServerUserVar]
		public static void swapseats(Arg arg)
		{
			int targetSeat = 0;
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null || basePlayer.SwapSeatCooldown())
			{
				return;
			}
			BaseMountable mounted = basePlayer.GetMounted();
			if (!((Object)(object)mounted == (Object)null))
			{
				BaseVehicle baseVehicle = ((Component)mounted).GetComponent<BaseVehicle>();
				if ((Object)(object)baseVehicle == (Object)null)
				{
					baseVehicle = mounted.VehicleParent();
				}
				if (!((Object)(object)baseVehicle == (Object)null))
				{
					baseVehicle.SwapSeats(basePlayer, targetSeat);
				}
			}
		}

		[ServerVar]
		public static void fixcars(Arg arg)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null)
			{
				arg.ReplyWith("Null player.");
				return;
			}
			if (!basePlayer.IsAdmin)
			{
				arg.ReplyWith("Must be an admin to use fixcars.");
				return;
			}
			int @int = arg.GetInt(0, 2);
			@int = Mathf.Clamp(@int, 1, 3);
			ModularCar[] array = Object.FindObjectsOfType<ModularCar>();
			int num = 0;
			ModularCar[] array2 = array;
			foreach (ModularCar modularCar in array2)
			{
				if (modularCar.isServer && Vector3.Distance(((Component)modularCar).get_transform().get_position(), ((Component)basePlayer).get_transform().get_position()) <= 5f && modularCar.AdminFixUp(@int))
				{
					num++;
				}
			}
			MLRS[] array3 = Object.FindObjectsOfType<MLRS>();
			foreach (MLRS mLRS in array3)
			{
				if (mLRS.isServer && Vector3.Distance(((Component)mLRS).get_transform().get_position(), ((Component)basePlayer).get_transform().get_position()) <= 5f && mLRS.AdminFixUp())
				{
					num++;
				}
			}
			arg.ReplyWith($"Fixed up {num} vehicles.");
		}

		public vehicle()
			: this()
		{
		}
	}
}
