using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	public class Time : BaseHandler<AppEmpty>
	{
		public override void Execute()
		{
			TOD_Sky instance = TOD_Sky.get_Instance();
			TOD_Time time = instance.get_Components().get_Time();
			AppTime val = Pool.Get<AppTime>();
			val.dayLengthMinutes = time.DayLengthInMinutes;
			val.timeScale = (time.ProgressTime ? Time.get_timeScale() : 0f);
			val.sunrise = instance.get_SunriseTime();
			val.sunset = instance.get_SunsetTime();
			val.time = instance.Cycle.Hour;
			AppResponse val2 = Pool.Get<AppResponse>();
			val2.time = val;
			Send(val2);
		}
	}
}
