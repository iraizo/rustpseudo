using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class CableCurve
	{
		[SerializeField]
		private Vector2 m_start;

		[SerializeField]
		private Vector2 m_end;

		[SerializeField]
		private float m_slack;

		[SerializeField]
		private int m_steps;

		[SerializeField]
		private bool m_regen;

		private static Vector2[] emptyCurve = (Vector2[])(object)new Vector2[2]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0f)
		};

		[SerializeField]
		private Vector2[] points;

		public bool regenPoints
		{
			get
			{
				return m_regen;
			}
			set
			{
				m_regen = value;
			}
		}

		public Vector2 start
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return m_start;
			}
			set
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				if (value != m_start)
				{
					m_regen = true;
				}
				m_start = value;
			}
		}

		public Vector2 end
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return m_end;
			}
			set
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				if (value != m_end)
				{
					m_regen = true;
				}
				m_end = value;
			}
		}

		public float slack
		{
			get
			{
				return m_slack;
			}
			set
			{
				if (value != m_slack)
				{
					m_regen = true;
				}
				m_slack = Mathf.Max(0f, value);
			}
		}

		public int steps
		{
			get
			{
				return m_steps;
			}
			set
			{
				if (value != m_steps)
				{
					m_regen = true;
				}
				m_steps = Mathf.Max(2, value);
			}
		}

		public Vector2 midPoint
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				Vector2 result = Vector2.get_zero();
				if (m_steps == 2)
				{
					return (points[0] + points[1]) * 0.5f;
				}
				if (m_steps > 2)
				{
					int num = m_steps / 2;
					result = ((m_steps % 2 != 0) ? points[num] : ((points[num] + points[num + 1]) * 0.5f));
				}
				return result;
			}
		}

		public CableCurve()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			points = emptyCurve;
			m_start = Vector2.get_up();
			m_end = Vector2.get_up() + Vector2.get_right();
			m_slack = 0.5f;
			m_steps = 20;
			m_regen = true;
		}

		public CableCurve(Vector2[] inputPoints)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			points = inputPoints;
			m_start = inputPoints[0];
			m_end = inputPoints[1];
			m_slack = 0.5f;
			m_steps = 20;
			m_regen = true;
		}

		public CableCurve(List<Vector2> inputPoints)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			points = inputPoints.ToArray();
			m_start = inputPoints[0];
			m_end = inputPoints[1];
			m_slack = 0.5f;
			m_steps = 20;
			m_regen = true;
		}

		public CableCurve(CableCurve v)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			points = v.Points();
			m_start = v.start;
			m_end = v.end;
			m_slack = v.slack;
			m_steps = v.steps;
			m_regen = v.regenPoints;
		}

		public Vector2[] Points()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			if (!m_regen)
			{
				return points;
			}
			if (m_steps < 2)
			{
				return emptyCurve;
			}
			float num = Vector2.Distance(m_end, m_start);
			float num2 = Vector2.Distance(new Vector2(m_end.x, m_start.y), m_start);
			float num3 = num + Mathf.Max(0.0001f, m_slack);
			float num4 = 0f;
			float y = m_start.y;
			float num5 = num2;
			float y2 = end.y;
			if (num5 - num4 == 0f)
			{
				return emptyCurve;
			}
			float num6 = Mathf.Sqrt(Mathf.Pow(num3, 2f) - Mathf.Pow(y2 - y, 2f)) / (num5 - num4);
			int num7 = 30;
			int num8 = 0;
			int num9 = num7 * 10;
			bool flag = false;
			float num10 = 0f;
			float num11 = 0f;
			float num12 = 100f;
			float num13 = 0f;
			for (int i = 0; i < num7; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					num8++;
					num11 = num10 + num12;
					num13 = (float)Math.Sinh(num11) / num11;
					if (!float.IsInfinity(num13))
					{
						if (num13 == num6)
						{
							flag = true;
							num10 = num11;
							break;
						}
						if (num13 > num6)
						{
							break;
						}
						num10 = num11;
						if (num8 > num9)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
				num12 *= 0.1f;
			}
			float num14 = (num5 - num4) / 2f / num10;
			float num15 = (num4 + num5 - num14 * Mathf.Log((num3 + y2 - y) / (num3 - y2 + y))) / 2f;
			float num16 = (y2 + y - num3 * (float)Math.Cosh(num10) / (float)Math.Sinh(num10)) / 2f;
			points = (Vector2[])(object)new Vector2[m_steps];
			float num17 = m_steps - 1;
			for (int k = 0; k < m_steps; k++)
			{
				float num18 = (float)k / num17;
				Vector2 zero = Vector2.get_zero();
				zero.x = Mathf.Lerp(start.x, end.x, num18);
				zero.y = num14 * (float)Math.Cosh((num18 * num2 - num15) / num14) + num16;
				points[k] = zero;
			}
			m_regen = false;
			return points;
		}
	}
}
