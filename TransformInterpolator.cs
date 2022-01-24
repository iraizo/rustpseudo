using System.Collections.Generic;
using UnityEngine;

public class TransformInterpolator
{
	public struct Segment
	{
		public Entry tick;

		public Entry prev;

		public Entry next;
	}

	public struct Entry
	{
		public float time;

		public Vector3 pos;

		public Quaternion rot;
	}

	public List<Entry> list = new List<Entry>(32);

	public Entry last;

	public void Add(Entry tick)
	{
		last = tick;
		list.Add(tick);
	}

	public void Cull(float beforeTime)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].time < beforeTime)
			{
				list.RemoveAt(i);
				i--;
			}
		}
	}

	public void Clear()
	{
		list.Clear();
	}

	public Segment Query(float time, float interpolation, float extrapolation, float smoothing)
	{
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		Segment result = default(Segment);
		if (list.Count == 0)
		{
			result.prev = last;
			result.next = last;
			result.tick = last;
			return result;
		}
		float num = time - interpolation - smoothing * 0.5f;
		float num2 = Mathf.Min(time - interpolation, last.time);
		float num3 = num2 - smoothing;
		Entry entry = list[0];
		Entry entry2 = last;
		Entry entry3 = list[0];
		Entry entry4 = last;
		foreach (Entry item in list)
		{
			if (item.time < num3)
			{
				entry = item;
			}
			else if (entry2.time >= item.time)
			{
				entry2 = item;
			}
			if (item.time < num2)
			{
				entry3 = item;
			}
			else if (entry4.time >= item.time)
			{
				entry4 = item;
			}
		}
		Entry prev = default(Entry);
		if (entry2.time - entry.time <= Mathf.Epsilon)
		{
			prev.time = num3;
			prev.pos = entry2.pos;
			prev.rot = entry2.rot;
		}
		else
		{
			float num4 = (num3 - entry.time) / (entry2.time - entry.time);
			prev.time = num3;
			prev.pos = Vector3.LerpUnclamped(entry.pos, entry2.pos, num4);
			prev.rot = Quaternion.SlerpUnclamped(entry.rot, entry2.rot, num4);
		}
		result.prev = prev;
		Entry entry5 = default(Entry);
		if (entry4.time - entry3.time <= Mathf.Epsilon)
		{
			entry5.time = num2;
			entry5.pos = entry4.pos;
			entry5.rot = entry4.rot;
		}
		else
		{
			float num5 = (num2 - entry3.time) / (entry4.time - entry3.time);
			entry5.time = num2;
			entry5.pos = Vector3.LerpUnclamped(entry3.pos, entry4.pos, num5);
			entry5.rot = Quaternion.SlerpUnclamped(entry3.rot, entry4.rot, num5);
		}
		result.next = entry5;
		if (entry5.time - prev.time <= Mathf.Epsilon)
		{
			result.prev = entry5;
			result.tick = entry5;
			return result;
		}
		if (num - entry5.time > extrapolation)
		{
			result.prev = entry5;
			result.tick = entry5;
			return result;
		}
		Entry tick = default(Entry);
		float num6 = Mathf.Min(num - prev.time, entry5.time + extrapolation - prev.time) / (entry5.time - prev.time);
		tick.time = num;
		tick.pos = Vector3.LerpUnclamped(prev.pos, entry5.pos, num6);
		tick.rot = Quaternion.SlerpUnclamped(prev.rot, entry5.rot, num6);
		result.tick = tick;
		return result;
	}
}
