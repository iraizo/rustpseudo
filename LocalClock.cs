using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalClock
{
	public struct TimedEvent
	{
		public float time;

		public float delta;

		public float variance;

		public Action action;
	}

	public List<TimedEvent> events = new List<TimedEvent>();

	public void Add(float delta, float variance, Action action)
	{
		TimedEvent item = default(TimedEvent);
		item.time = Time.get_time() + delta + Random.Range(0f - variance, variance);
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
			if (Time.get_time() > value.time)
			{
				float delta = value.delta;
				float variance = value.variance;
				value.action();
				value.time = Time.get_time() + delta + Random.Range(0f - variance, variance);
				events[i] = value;
			}
		}
	}
}
