using System;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizedClock
{
	public struct TimedEvent
	{
		public float time;

		public float delta;

		public float variance;

		public Action<uint> action;
	}

	public List<TimedEvent> events = new List<TimedEvent>();

	private static float CurrentTime => Time.get_realtimeSinceStartup();

	public void Add(float delta, float variance, Action<uint> action)
	{
		TimedEvent item = default(TimedEvent);
		item.time = CurrentTime;
		item.delta = delta;
		item.variance = variance;
		item.action = action;
		events.Add(item);
	}

	public void Tick()
	{
		for (int i = 0; i < events.Count; i++)
		{
			TimedEvent value = events[i];
			float time = value.time;
			float currentTime = CurrentTime;
			float delta = value.delta;
			float num = time - time % delta;
			uint obj = (uint)(time / delta);
			SeedRandom.Wanghash(ref obj);
			SeedRandom.Wanghash(ref obj);
			SeedRandom.Wanghash(ref obj);
			float num2 = SeedRandom.Range(ref obj, 0f - value.variance, value.variance);
			float num3 = num + delta + num2;
			if (time < num3 && currentTime >= num3)
			{
				value.action(obj);
				value.time = currentTime;
			}
			else if (currentTime > time || currentTime < num - 5f)
			{
				value.time = currentTime;
			}
			events[i] = value;
		}
	}
}
