using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class Spline
	{
		public const int k_Precision = 128;

		public const float k_Step = 0.0078125f;

		public AnimationCurve curve;

		[SerializeField]
		private bool m_Loop;

		[SerializeField]
		private float m_ZeroValue;

		[SerializeField]
		private float m_Range;

		private AnimationCurve m_InternalLoopingCurve;

		private int frameCount = -1;

		public float[] cachedData;

		public Spline(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
		{
			Assert.IsNotNull<AnimationCurve>(curve);
			this.curve = curve;
			m_ZeroValue = zeroValue;
			m_Loop = loop;
			m_Range = ((Vector2)(ref bounds)).get_magnitude();
			cachedData = new float[128];
		}

		public void Cache(int frame)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			if (frame == frameCount)
			{
				return;
			}
			int length = curve.get_length();
			if (m_Loop && length > 1)
			{
				if (m_InternalLoopingCurve == null)
				{
					m_InternalLoopingCurve = new AnimationCurve();
				}
				Keyframe val = curve.get_Item(length - 1);
				((Keyframe)(ref val)).set_time(((Keyframe)(ref val)).get_time() - m_Range);
				Keyframe val2 = curve.get_Item(0);
				((Keyframe)(ref val2)).set_time(((Keyframe)(ref val2)).get_time() + m_Range);
				m_InternalLoopingCurve.set_keys(curve.get_keys());
				m_InternalLoopingCurve.AddKey(val);
				m_InternalLoopingCurve.AddKey(val2);
			}
			for (int i = 0; i < 128; i++)
			{
				cachedData[i] = Evaluate((float)i * 0.0078125f, length);
			}
			frameCount = Time.get_renderedFrameCount();
		}

		public float Evaluate(float t, int length)
		{
			if (length == 0)
			{
				return m_ZeroValue;
			}
			if (!m_Loop || length == 1)
			{
				return curve.Evaluate(t);
			}
			return m_InternalLoopingCurve.Evaluate(t);
		}

		public float Evaluate(float t)
		{
			return Evaluate(t, curve.get_length());
		}

		public override int GetHashCode()
		{
			return 17 * 23 + ((object)curve).GetHashCode();
		}
	}
}
