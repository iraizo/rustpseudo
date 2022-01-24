using System;
using ConVar;
using Rust;
using UnityEngine;

public class EventSchedule : BaseMonoBehaviour
{
	[Tooltip("The minimum amount of hours between events")]
	public float minimumHoursBetween = 12f;

	[Tooltip("The maximum amount of hours between events")]
	public float maxmumHoursBetween = 24f;

	private float hoursRemaining;

	private long lastRun;

	private void OnEnable()
	{
		hoursRemaining = Random.Range(minimumHoursBetween, maxmumHoursBetween);
		((FacepunchBehaviour)this).InvokeRepeating((Action)RunSchedule, 1f, 1f);
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)RunSchedule);
		}
	}

	private void RunSchedule()
	{
		if (!Application.isLoading && ConVar.Server.events)
		{
			CountHours();
			if (!(hoursRemaining > 0f))
			{
				Trigger();
			}
		}
	}

	private void Trigger()
	{
		hoursRemaining = Random.Range(minimumHoursBetween, maxmumHoursBetween);
		TriggeredEvent[] components = ((Component)this).GetComponents<TriggeredEvent>();
		if (components.Length != 0)
		{
			TriggeredEvent triggeredEvent = components[Random.Range(0, components.Length)];
			if (!((Object)(object)triggeredEvent == (Object)null))
			{
				((Component)triggeredEvent).SendMessage("RunEvent", (SendMessageOptions)1);
			}
		}
	}

	private void CountHours()
	{
		if (Object.op_Implicit((Object)(object)TOD_Sky.get_Instance()))
		{
			if (lastRun != 0L)
			{
				hoursRemaining -= (float)TOD_Sky.get_Instance().Cycle.get_DateTime().Subtract(DateTime.FromBinary(lastRun)).TotalSeconds / 60f / 60f;
			}
			lastRun = TOD_Sky.get_Instance().Cycle.get_DateTime().ToBinary();
		}
	}
}
