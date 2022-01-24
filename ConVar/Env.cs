using System;
using UnityEngine;

namespace ConVar
{
	[Factory("env")]
	public class Env : ConsoleSystem
	{
		[ServerVar]
		public static bool progresstime
		{
			get
			{
				if ((Object)(object)TOD_Sky.get_Instance() == (Object)null)
				{
					return false;
				}
				return TOD_Sky.get_Instance().get_Components().get_Time()
					.ProgressTime;
			}
			set
			{
				if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
				{
					TOD_Sky.get_Instance().get_Components().get_Time()
						.ProgressTime = value;
				}
			}
		}

		[ServerVar(ShowInAdminUI = true)]
		public static float time
		{
			get
			{
				if ((Object)(object)TOD_Sky.get_Instance() == (Object)null)
				{
					return 0f;
				}
				return TOD_Sky.get_Instance().Cycle.Hour;
			}
			set
			{
				if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
				{
					TOD_Sky.get_Instance().Cycle.Hour = value;
				}
			}
		}

		[ServerVar]
		public static int day
		{
			get
			{
				if ((Object)(object)TOD_Sky.get_Instance() == (Object)null)
				{
					return 0;
				}
				return TOD_Sky.get_Instance().Cycle.Day;
			}
			set
			{
				if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
				{
					TOD_Sky.get_Instance().Cycle.Day = value;
				}
			}
		}

		[ServerVar]
		public static int month
		{
			get
			{
				if ((Object)(object)TOD_Sky.get_Instance() == (Object)null)
				{
					return 0;
				}
				return TOD_Sky.get_Instance().Cycle.Month;
			}
			set
			{
				if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
				{
					TOD_Sky.get_Instance().Cycle.Month = value;
				}
			}
		}

		[ServerVar]
		public static int year
		{
			get
			{
				if ((Object)(object)TOD_Sky.get_Instance() == (Object)null)
				{
					return 0;
				}
				return TOD_Sky.get_Instance().Cycle.Year;
			}
			set
			{
				if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
				{
					TOD_Sky.get_Instance().Cycle.Year = value;
				}
			}
		}

		[ReplicatedVar(Default = "0")]
		public static float oceanlevel
		{
			get
			{
				return WaterSystem.OceanLevel;
			}
			set
			{
				WaterSystem.OceanLevel = value;
			}
		}

		[ServerVar]
		public static void addtime(Arg arg)
		{
			if (!((Object)(object)TOD_Sky.get_Instance() == (Object)null))
			{
				DateTime dateTime = TOD_Sky.get_Instance().Cycle.get_DateTime().AddTicks(arg.GetTicks(0, 0L));
				TOD_Sky.get_Instance().Cycle.set_DateTime(dateTime);
			}
		}

		public Env()
			: this()
		{
		}
	}
}
