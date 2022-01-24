using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRenderer : UIPrimitiveBase
	{
		private enum SegmentType
		{
			Start,
			Middle,
			End,
			Full
		}

		public enum JoinType
		{
			Bevel,
			Miter
		}

		public enum BezierType
		{
			None,
			Quick,
			Basic,
			Improved,
			Catenary
		}

		private const float MIN_MITER_JOIN = (float)Math.PI / 12f;

		private const float MIN_BEVEL_NICE_JOIN = (float)Math.PI / 6f;

		private static Vector2 UV_TOP_LEFT;

		private static Vector2 UV_BOTTOM_LEFT;

		private static Vector2 UV_TOP_CENTER_LEFT;

		private static Vector2 UV_TOP_CENTER_RIGHT;

		private static Vector2 UV_BOTTOM_CENTER_LEFT;

		private static Vector2 UV_BOTTOM_CENTER_RIGHT;

		private static Vector2 UV_TOP_RIGHT;

		private static Vector2 UV_BOTTOM_RIGHT;

		private static Vector2[] startUvs;

		private static Vector2[] middleUvs;

		private static Vector2[] endUvs;

		private static Vector2[] fullUvs;

		[SerializeField]
		[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		internal Vector2[] m_points;

		[SerializeField]
		[Tooltip("Segments to be drawn\n This is a list of arrays of points")]
		internal List<Vector2[]> m_segments;

		[SerializeField]
		[Tooltip("Thickness of the line")]
		internal float lineThickness = 2f;

		[SerializeField]
		[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
		internal bool relativeSize;

		[SerializeField]
		[Tooltip("Do the points identify a single line or split pairs of lines")]
		internal bool lineList;

		[SerializeField]
		[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
		internal bool lineCaps;

		[SerializeField]
		[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
		internal int bezierSegmentsPerCurve = 10;

		[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public JoinType LineJoins;

		[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
		public BezierType BezierMode;

		[HideInInspector]
		public bool drivenExternally;

		public float LineThickness
		{
			get
			{
				return lineThickness;
			}
			set
			{
				lineThickness = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool RelativeSize
		{
			get
			{
				return relativeSize;
			}
			set
			{
				relativeSize = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool LineList
		{
			get
			{
				return lineList;
			}
			set
			{
				lineList = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool LineCaps
		{
			get
			{
				return lineCaps;
			}
			set
			{
				lineCaps = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public int BezierSegmentsPerCurve
		{
			get
			{
				return bezierSegmentsPerCurve;
			}
			set
			{
				bezierSegmentsPerCurve = value;
			}
		}

		public Vector2[] Points
		{
			get
			{
				return m_points;
			}
			set
			{
				if (m_points != value)
				{
					m_points = value;
					((Graphic)this).SetAllDirty();
				}
			}
		}

		public List<Vector2[]> Segments
		{
			get
			{
				return m_segments;
			}
			set
			{
				m_segments = value;
				((Graphic)this).SetAllDirty();
			}
		}

		private void PopulateMesh(VertexHelper vh, Vector2[] pointsToDraw)
		{
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			if (BezierMode != 0 && BezierMode != BezierType.Catenary && pointsToDraw.Length > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = bezierSegmentsPerCurve;
				pointsToDraw = (BezierMode switch
				{
					BezierType.Basic => bezierPath.GetDrawingPoints0(), 
					BezierType.Improved => bezierPath.GetDrawingPoints1(), 
					_ => bezierPath.GetDrawingPoints2(), 
				}).ToArray();
			}
			if (BezierMode == BezierType.Catenary && pointsToDraw.Length == 2)
			{
				pointsToDraw = new CableCurve(pointsToDraw)
				{
					slack = base.Resoloution,
					steps = BezierSegmentsPerCurve
				}.Points();
			}
			if (base.ImproveResolution != 0)
			{
				pointsToDraw = IncreaseResolution(pointsToDraw);
			}
			Rect rect;
			float num;
			if (relativeSize)
			{
				rect = ((Graphic)this).get_rectTransform().get_rect();
				num = ((Rect)(ref rect)).get_width();
			}
			else
			{
				num = 1f;
			}
			float num2 = num;
			float num3;
			if (relativeSize)
			{
				rect = ((Graphic)this).get_rectTransform().get_rect();
				num3 = ((Rect)(ref rect)).get_height();
			}
			else
			{
				num3 = 1f;
			}
			float num4 = num3;
			float num5 = (0f - ((Graphic)this).get_rectTransform().get_pivot().x) * num2;
			float num6 = (0f - ((Graphic)this).get_rectTransform().get_pivot().y) * num4;
			List<UIVertex[]> list = new List<UIVertex[]>();
			if (lineList)
			{
				for (int i = 1; i < pointsToDraw.Length; i += 2)
				{
					Vector2 val = pointsToDraw[i - 1];
					Vector2 val2 = pointsToDraw[i];
					((Vector2)(ref val))._002Ector(val.x * num2 + num5, val.y * num4 + num6);
					((Vector2)(ref val2))._002Ector(val2.x * num2 + num5, val2.y * num4 + num6);
					if (lineCaps)
					{
						list.Add(CreateLineCap(val, val2, SegmentType.Start));
					}
					list.Add(CreateLineSegment(val, val2, SegmentType.Middle, (list.Count > 1) ? list[list.Count - 2] : null));
					if (lineCaps)
					{
						list.Add(CreateLineCap(val, val2, SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Length; j++)
				{
					Vector2 val3 = pointsToDraw[j - 1];
					Vector2 val4 = pointsToDraw[j];
					((Vector2)(ref val3))._002Ector(val3.x * num2 + num5, val3.y * num4 + num6);
					((Vector2)(ref val4))._002Ector(val4.x * num2 + num5, val4.y * num4 + num6);
					if (lineCaps && j == 1)
					{
						list.Add(CreateLineCap(val3, val4, SegmentType.Start));
					}
					list.Add(CreateLineSegment(val3, val4, SegmentType.Middle));
					if (lineCaps && j == pointsToDraw.Length - 1)
					{
						list.Add(CreateLineCap(val3, val4, SegmentType.End));
					}
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				if (!lineList && k < list.Count - 1)
				{
					Vector3 val5 = list[k][1].position - list[k][2].position;
					Vector3 val6 = list[k + 1][2].position - list[k + 1][1].position;
					float num7 = Vector2.Angle(Vector2.op_Implicit(val5), Vector2.op_Implicit(val6)) * ((float)Math.PI / 180f);
					float num8 = Mathf.Sign(Vector3.Cross(((Vector3)(ref val5)).get_normalized(), ((Vector3)(ref val6)).get_normalized()).z);
					float num9 = lineThickness / (2f * Mathf.Tan(num7 / 2f));
					Vector3 position = list[k][2].position - ((Vector3)(ref val5)).get_normalized() * num9 * num8;
					Vector3 position2 = list[k][3].position + ((Vector3)(ref val5)).get_normalized() * num9 * num8;
					JoinType joinType = LineJoins;
					if (joinType == JoinType.Miter)
					{
						if (num9 < ((Vector3)(ref val5)).get_magnitude() / 2f && num9 < ((Vector3)(ref val6)).get_magnitude() / 2f && num7 > (float)Math.PI / 12f)
						{
							list[k][2].position = position;
							list[k][3].position = position2;
							list[k + 1][0].position = position2;
							list[k + 1][1].position = position;
						}
						else
						{
							joinType = JoinType.Bevel;
						}
					}
					if (joinType == JoinType.Bevel)
					{
						if (num9 < ((Vector3)(ref val5)).get_magnitude() / 2f && num9 < ((Vector3)(ref val6)).get_magnitude() / 2f && num7 > (float)Math.PI / 6f)
						{
							if (num8 < 0f)
							{
								list[k][2].position = position;
								list[k + 1][1].position = position;
							}
							else
							{
								list[k][3].position = position2;
								list[k + 1][0].position = position2;
							}
						}
						UIVertex[] array = (UIVertex[])(object)new UIVertex[4]
						{
							list[k][2],
							list[k][3],
							list[k + 1][0],
							list[k + 1][1]
						};
						vh.AddUIVertexQuad(array);
					}
				}
				vh.AddUIVertexQuad(list[k]);
			}
			if (vh.get_currentVertCount() > 64000)
			{
				Debug.LogError((object)("Max Verticies size is 64000, current mesh vertcies count is [" + vh.get_currentVertCount() + "] - Cannot Draw"));
				vh.Clear();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (m_points != null && m_points.Length != 0)
			{
				GeneratedUVs();
				vh.Clear();
				PopulateMesh(vh, m_points);
			}
			else if (m_segments != null && m_segments.Count > 0)
			{
				GeneratedUVs();
				vh.Clear();
				for (int i = 0; i < m_segments.Count; i++)
				{
					Vector2[] pointsToDraw = m_segments[i];
					PopulateMesh(vh, pointsToDraw);
				}
			}
		}

		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, SegmentType type)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val;
			switch (type)
			{
			case SegmentType.Start:
			{
				val = end - start;
				Vector2 start2 = start - ((Vector2)(ref val)).get_normalized() * lineThickness / 2f;
				return CreateLineSegment(start2, start, SegmentType.Start);
			}
			case SegmentType.End:
			{
				val = end - start;
				Vector2 end2 = end + ((Vector2)(ref val)).get_normalized() * lineThickness / 2f;
				return CreateLineSegment(end, end2, SegmentType.End);
			}
			default:
				Debug.LogError((object)"Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
				return null;
			}
		}

		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type, UIVertex[] previousVert = null)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = new Vector2(start.y - end.y, end.x - start.x);
			Vector2 val2 = ((Vector2)(ref val)).get_normalized() * lineThickness / 2f;
			Vector2 val3 = Vector2.get_zero();
			Vector2 val4 = Vector2.get_zero();
			if (previousVert != null)
			{
				((Vector2)(ref val3))._002Ector(previousVert[3].position.x, previousVert[3].position.y);
				((Vector2)(ref val4))._002Ector(previousVert[2].position.x, previousVert[2].position.y);
			}
			else
			{
				val3 = start - val2;
				val4 = start + val2;
			}
			Vector2 val5 = end + val2;
			Vector2 val6 = end - val2;
			return type switch
			{
				SegmentType.Start => SetVbo((Vector2[])(object)new Vector2[4] { val3, val4, val5, val6 }, startUvs), 
				SegmentType.End => SetVbo((Vector2[])(object)new Vector2[4] { val3, val4, val5, val6 }, endUvs), 
				SegmentType.Full => SetVbo((Vector2[])(object)new Vector2[4] { val3, val4, val5, val6 }, fullUvs), 
				_ => SetVbo((Vector2[])(object)new Vector2[4] { val3, val4, val5, val6 }, middleUvs), 
			};
		}

		protected override void GeneratedUVs()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)base.activeSprite != (Object)null)
			{
				Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
				Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
				UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
				UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
				UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
				UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
				UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
				UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
				UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
				UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
			}
			else
			{
				UV_TOP_LEFT = Vector2.get_zero();
				UV_BOTTOM_LEFT = new Vector2(0f, 1f);
				UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
				UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
				UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
				UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
				UV_TOP_RIGHT = new Vector2(1f, 0f);
				UV_BOTTOM_RIGHT = Vector2.get_one();
			}
			startUvs = (Vector2[])(object)new Vector2[4] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_CENTER_LEFT, UV_TOP_CENTER_LEFT };
			middleUvs = (Vector2[])(object)new Vector2[4] { UV_TOP_CENTER_LEFT, UV_BOTTOM_CENTER_LEFT, UV_BOTTOM_CENTER_RIGHT, UV_TOP_CENTER_RIGHT };
			endUvs = (Vector2[])(object)new Vector2[4] { UV_TOP_CENTER_RIGHT, UV_BOTTOM_CENTER_RIGHT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
			fullUvs = (Vector2[])(object)new Vector2[4] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
		}

		protected override void ResolutionToNativeSize(float distance)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (base.UseNativeSize)
			{
				Rect rect = base.activeSprite.get_rect();
				m_Resolution = distance / (((Rect)(ref rect)).get_width() / base.pixelsPerUnit);
				rect = base.activeSprite.get_rect();
				lineThickness = ((Rect)(ref rect)).get_height() / base.pixelsPerUnit;
			}
		}

		private int GetSegmentPointCount()
		{
			List<Vector2[]> segments = Segments;
			if (segments != null && segments.Count > 0)
			{
				int num = 0;
				{
					foreach (Vector2[] segment in Segments)
					{
						num += segment.Length;
					}
					return num;
				}
			}
			return Points.Length;
		}

		public Vector2 GetPosition(int index, int segmentIndex = 0)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			if (segmentIndex > 0)
			{
				return Segments[segmentIndex - 1][index - 1];
			}
			if (Segments.Count > 0)
			{
				int num = 0;
				int num2 = index;
				foreach (Vector2[] segment in Segments)
				{
					if (num2 - segment.Length > 0)
					{
						num2 -= segment.Length;
						num++;
						continue;
					}
					break;
				}
				return Segments[num][num2 - 1];
			}
			return Points[index - 1];
		}

		public Vector2 GetPositionBySegment(int index, int segment)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return Segments[segment][index - 1];
		}

		public Vector2 GetClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = p3 - p1;
			Vector2 val2 = p2 - p1;
			float num = Mathf.Clamp01(Vector2.Dot(val, ((Vector2)(ref val2)).get_normalized()) / ((Vector2)(ref val2)).get_magnitude());
			return p1 + val2 * num;
		}
	}
}
