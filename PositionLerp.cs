using System;
using UnityEngine;

public class PositionLerp : IDisposable
{
	private static ListHashSet<PositionLerp> InstanceList = new ListHashSet<PositionLerp>(8);

	public static bool DebugLog = false;

	public static bool DebugDraw = false;

	public static int TimeOffsetInterval = 16;

	public static float TimeOffset = 0f;

	public const int TimeOffsetIntervalMin = 4;

	public const int TimeOffsetIntervalMax = 64;

	private bool enabled = true;

	private Action idleDisable;

	private TransformInterpolator interpolator = new TransformInterpolator();

	private ILerpTarget target;

	private float timeOffset0 = float.MaxValue;

	private float timeOffset1 = float.MaxValue;

	private float timeOffset2 = float.MaxValue;

	private float timeOffset3 = float.MaxValue;

	private int timeOffsetCount;

	private float lastClientTime;

	private float lastServerTime;

	private float extrapolatedTime;

	private float enabledTime;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
			if (enabled)
			{
				OnEnable();
			}
			else
			{
				OnDisable();
			}
		}
	}

	private static float LerpTime => Time.get_time();

	private void OnEnable()
	{
		InstanceList.Add(this);
		enabledTime = LerpTime;
	}

	private void OnDisable()
	{
		InstanceList.Remove(this);
	}

	public void Initialize(ILerpTarget target)
	{
		this.target = target;
		Enabled = true;
	}

	public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
	{
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		float interpolationDelay = target.GetInterpolationDelay();
		float interpolationSmoothing = target.GetInterpolationSmoothing();
		float num = interpolationDelay + interpolationSmoothing + 1f;
		float lerpTime = LerpTime;
		timeOffset0 = Mathf.Min(timeOffset0, lerpTime - serverTime);
		timeOffsetCount++;
		if (timeOffsetCount >= TimeOffsetInterval / 4)
		{
			timeOffset3 = timeOffset2;
			timeOffset2 = timeOffset1;
			timeOffset1 = timeOffset0;
			timeOffset0 = float.MaxValue;
			timeOffsetCount = 0;
		}
		TimeOffset = Mathx.Min(timeOffset0, timeOffset1, timeOffset2, timeOffset3);
		lerpTime = serverTime + TimeOffset;
		if (DebugLog && interpolator.list.Count > 0 && serverTime < lastServerTime)
		{
			Debug.LogWarning((object)(target.ToString() + " adding tick from the past: server time " + serverTime + " < " + lastServerTime));
		}
		else if (DebugLog && interpolator.list.Count > 0 && lerpTime < lastClientTime)
		{
			Debug.LogWarning((object)(target.ToString() + " adding tick from the past: client time " + lerpTime + " < " + lastClientTime));
		}
		else
		{
			lastClientTime = lerpTime;
			lastServerTime = serverTime;
			interpolator.Add(new TransformInterpolator.Entry
			{
				time = lerpTime,
				pos = position,
				rot = rotation
			});
		}
		interpolator.Cull(lerpTime - num);
	}

	public void Snapshot(Vector3 position, Quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Snapshot(position, rotation, LerpTime - TimeOffset);
	}

	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		interpolator.Clear();
		Snapshot(position, rotation, serverTime);
		target.SetNetworkPosition(position);
		target.SetNetworkRotation(rotation);
	}

	public void SnapTo(Vector3 position, Quaternion rotation)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		interpolator.last = new TransformInterpolator.Entry
		{
			pos = position,
			rot = rotation,
			time = LerpTime
		};
		Wipe();
	}

	public void SnapToEnd()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		float interpolationDelay = target.GetInterpolationDelay();
		TransformInterpolator.Segment segment = interpolator.Query(LerpTime, interpolationDelay, 0f, 0f);
		target.SetNetworkPosition(segment.tick.pos);
		target.SetNetworkRotation(segment.tick.rot);
		Wipe();
	}

	public void Wipe()
	{
		interpolator.Clear();
		timeOffsetCount = 0;
		timeOffset0 = float.MaxValue;
		timeOffset1 = float.MaxValue;
		timeOffset2 = float.MaxValue;
		timeOffset3 = float.MaxValue;
	}

	public static void WipeAll()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<PositionLerp> enumerator = InstanceList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.get_Current().Wipe();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	protected void DoCycle()
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		if (target == null)
		{
			return;
		}
		float interpolationInertia = target.GetInterpolationInertia();
		float num = ((interpolationInertia > 0f) ? Mathf.InverseLerp(0f, interpolationInertia, LerpTime - enabledTime) : 1f);
		float extrapolationTime = target.GetExtrapolationTime();
		float interpolation = target.GetInterpolationDelay() * num;
		float num2 = target.GetInterpolationSmoothing() * num;
		TransformInterpolator.Segment segment = interpolator.Query(LerpTime, interpolation, extrapolationTime, num2);
		if (segment.next.time >= interpolator.last.time)
		{
			extrapolatedTime = Mathf.Min(extrapolatedTime + Time.get_deltaTime(), extrapolationTime);
		}
		else
		{
			extrapolatedTime = Mathf.Max(extrapolatedTime - Time.get_deltaTime(), 0f);
		}
		if (extrapolatedTime > 0f && extrapolationTime > 0f && num2 > 0f)
		{
			float num3 = Time.get_deltaTime() / (extrapolatedTime / extrapolationTime * num2);
			segment.tick.pos = Vector3.Lerp(target.GetNetworkPosition(), segment.tick.pos, num3);
			segment.tick.rot = Quaternion.Slerp(target.GetNetworkRotation(), segment.tick.rot, num3);
		}
		target.SetNetworkPosition(segment.tick.pos);
		target.SetNetworkRotation(segment.tick.rot);
		if (DebugDraw)
		{
			target.DrawInterpolationState(segment, interpolator.list);
		}
		if (LerpTime - lastClientTime > 10f)
		{
			if (idleDisable == null)
			{
				idleDisable = target.LerpIdleDisable;
			}
			ILerpTarget lerpTarget = target;
			InvokeHandler.Invoke((Behaviour)((lerpTarget is Behaviour) ? lerpTarget : null), idleDisable, 0f);
		}
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		Quaternion rotation = ((Matrix4x4)(ref matrix)).get_rotation();
		for (int i = 0; i < interpolator.list.Count; i++)
		{
			TransformInterpolator.Entry value = interpolator.list[i];
			value.pos = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(value.pos);
			value.rot = rotation * value.rot;
			interpolator.list[i] = value;
		}
		interpolator.last.pos = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(interpolator.last.pos);
		interpolator.last.rot = rotation * interpolator.last.rot;
	}

	public Quaternion GetEstimatedAngularVelocity()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (target == null)
		{
			return Quaternion.get_identity();
		}
		float extrapolationTime = target.GetExtrapolationTime();
		float interpolationDelay = target.GetInterpolationDelay();
		float interpolationSmoothing = target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = interpolator.Query(LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing);
		TransformInterpolator.Entry next = segment.next;
		TransformInterpolator.Entry prev = segment.prev;
		if (next.time == prev.time)
		{
			return Quaternion.get_identity();
		}
		return Quaternion.Euler((((Quaternion)(ref prev.rot)).get_eulerAngles() - ((Quaternion)(ref next.rot)).get_eulerAngles()) / (prev.time - next.time));
	}

	public Vector3 GetEstimatedVelocity()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (target == null)
		{
			return Vector3.get_zero();
		}
		float extrapolationTime = target.GetExtrapolationTime();
		float interpolationDelay = target.GetInterpolationDelay();
		float interpolationSmoothing = target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = interpolator.Query(LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing);
		TransformInterpolator.Entry next = segment.next;
		TransformInterpolator.Entry prev = segment.prev;
		if (next.time == prev.time)
		{
			return Vector3.get_zero();
		}
		return (prev.pos - next.pos) / (prev.time - next.time);
	}

	public void Dispose()
	{
		target = null;
		idleDisable = null;
		interpolator.Clear();
		timeOffset0 = float.MaxValue;
		timeOffset1 = float.MaxValue;
		timeOffset2 = float.MaxValue;
		timeOffset3 = float.MaxValue;
		lastClientTime = 0f;
		lastServerTime = 0f;
		extrapolatedTime = 0f;
		timeOffsetCount = 0;
		Enabled = false;
	}

	public static void Clear()
	{
		InstanceList.Clear();
	}

	public static void Cycle()
	{
		PositionLerp[] buffer = InstanceList.get_Values().get_Buffer();
		int count = InstanceList.get_Count();
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}
}
