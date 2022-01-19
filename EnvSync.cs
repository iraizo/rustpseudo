using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class EnvSync : PointEntity
{
	private const float syncInterval = 5f;

	private const float syncIntervalInv = 0.2f;

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateNetwork, 5f, 5f);
	}

	private void UpdateNetwork()
	{
		if (NexusServer.Started && NexusServer.LastReset.HasValue && (Object)(object)TOD_Sky.get_Instance() != (Object)null)
		{
			TOD_Time time = TOD_Sky.get_Instance().get_Components().get_Time();
			DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(NexusServer.LastReset.Value);
			double totalMinutes = (DateTimeOffset.UtcNow - dateTimeOffset).TotalMinutes;
			double num = (double)(Nexus.timeOffset / 24f) + totalMinutes / (double)time.DayLengthInMinutes;
			if (time.UseTimeCurve)
			{
				double num2 = Math.Truncate(num);
				double num3 = (num - num2) * 24.0;
				float num4 = time.TimeCurve.Evaluate((float)num3);
				num = num2 + (double)(num4 / 24f);
			}
			float num5 = (float)(num * 24.0);
			DateTime dateTime = dateTimeOffset.Date.AddHours(num5);
			TOD_Sky.get_Instance().Cycle.set_DateTime(dateTime.ToUniversalTime());
		}
		SendNetworkUpdate();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.environment = Pool.Get<Environment>();
		if (Object.op_Implicit((Object)(object)TOD_Sky.get_Instance()))
		{
			info.msg.environment.dateTime = TOD_Sky.get_Instance().Cycle.get_DateTime().ToBinary();
		}
		info.msg.environment.engineTime = Time.get_realtimeSinceStartup();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.environment != null && Object.op_Implicit((Object)(object)TOD_Sky.get_Instance()) && base.isServer)
		{
			TOD_Sky.get_Instance().Cycle.set_DateTime(DateTime.FromBinary(info.msg.environment.dateTime));
		}
	}
}
