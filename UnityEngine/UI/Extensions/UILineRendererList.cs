using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UILineRendererList")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRendererList : UIPrimitiveBase
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
		internal List<Vector2> m_points;

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

		public List<Vector2> Points
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

		public void AddPoint(Vector2 pointToAdd)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			m_points.Add(pointToAdd);
			((Graphic)this).SetAllDirty();
		}

		public void RemovePoint(Vector2 pointToRemove)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			m_points.Remove(pointToRemove);
			((Graphic)this).SetAllDirty();
		}

		public void ClearPoints()
		{
			m_points.Clear();
			((Graphic)this).SetAllDirty();
		}

		private void PopulateMesh(VertexHelper vh, List<Vector2> pointsToDraw)
		{
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			if (BezierMode != 0 && BezierMode != BezierType.Catenary && pointsToDraw.Count > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = bezierSegmentsPerCurve;
				pointsToDraw = BezierMode switch
				{
					BezierType.Basic => bezierPath.GetDrawingPoints0(), 
					BezierType.Improved => bezierPath.GetDrawingPoints1(), 
					_ => bezierPath.GetDrawingPoints2(), 
				};
			}
			if (BezierMode == BezierType.Catenary && pointsToDraw.Count == 2)
			{
				CableCurve cableCurve = new CableCurve(pointsToDraw);
				cableCurve.slack = base.Resoloution;
				cableCurve.steps = BezierSegmentsPerCurve;
				pointsToDraw.Clear();
				pointsToDraw.AddRange(cableCurve.Points());
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
				for (int i = 1; i < pointsToDraw.Count; i += 2)
				{
					Vector2 val = pointsToDraw[i - 1];
					Vector2 val2 = pointsToDraw[i];
					((Vector2)(ref val))._002Ector(val.x * num2 + num5, val.y * num4 + num6);
					((Vector2)(ref val2))._002Ector(val2.x * num2 + num5, val2.y * num4 + num6);
					if (lineCaps)
					{
						list.Add(CreateLineCap(val, val2, SegmentType.Start));
					}
					list.Add(CreateLineSegment(val, val2, SegmentType.Middle));
					if (lineCaps)
					{
						list.Add(CreateLineCap(val, val2, SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Count; j++)
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
					if (lineCaps && j == pointsToDraw.Count - 1)
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
			if (m_points != null && m_points.Count > 0)
			{
				GeneratedUVs();
				vh.Clear();
				PopulateMesh(vh, m_points);
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
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
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

		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type)
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
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = new Vector2(start.y - end.y, end.x - start.x);
			Vector2 val2 = ((Vector2)(ref val)).get_normalized() * lineThickness / 2f;
			Vector2 val3 = start - val2;
			Vector2 val4 = start + val2;
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
	}
}
