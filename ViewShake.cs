using System.Collections.Generic;
using UnityEngine;

public class ViewShake
{
	protected class ShakeParameters
	{
		public float endTime;

		public float duration;

		public float amplitude;

		public float frequency;

		public float nextShake;

		public float angle;

		public Vector3 offset;

		public bool infinite;
	}

	protected List<ShakeParameters> Entries = new List<ShakeParameters>();

	public Vector3 PositionOffset { get; protected set; }

	public Vector3 AnglesOffset { get; protected set; }

	public void AddShake(float amplitude, float frequency, float duration)
	{
		Entries.Add(new ShakeParameters
		{
			amplitude = amplitude,
			frequency = Mathf.Max(frequency, 0.01f),
			duration = duration,
			endTime = Time.get_time() + duration,
			nextShake = 0f,
			angle = 0f,
			infinite = (duration <= 0f)
		});
	}

	public void Update()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_zero();
		Vector3 zero = Vector3.get_zero();
		Entries.RemoveAll((ShakeParameters i) => !i.infinite && Time.get_time() > i.endTime);
		foreach (ShakeParameters entry in Entries)
		{
			if (Time.get_time() > entry.nextShake)
			{
				entry.nextShake = Time.get_time() + 1f / entry.frequency;
				entry.offset = new Vector3(Random.Range(0f - entry.amplitude, entry.amplitude), Random.Range(0f - entry.amplitude, entry.amplitude), Random.Range(0f - entry.amplitude, entry.amplitude));
				entry.angle = Random.Range((0f - entry.amplitude) * 0.25f, entry.amplitude * 0.25f);
			}
			float num = 0f;
			float num2 = (entry.infinite ? 1f : ((entry.endTime - Time.get_time()) / entry.duration));
			if (num2 != 0f)
			{
				num = entry.frequency / num2;
			}
			num2 *= num2;
			float num3 = Time.get_time() * num;
			num2 *= Mathf.Sin(num3);
			val += entry.offset * num2;
			zero.z += entry.angle * num2;
			if (!entry.infinite)
			{
				entry.amplitude -= entry.amplitude * Time.get_deltaTime() / (entry.duration * entry.frequency);
			}
		}
		PositionOffset = val * 0.01f;
		AnglesOffset = zero;
	}

	public void Stop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Entries.Clear();
		PositionOffset = Vector3.get_zero();
		AnglesOffset = Vector3.get_zero();
	}
}
