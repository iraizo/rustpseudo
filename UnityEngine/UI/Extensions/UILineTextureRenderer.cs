using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UILineTextureRenderer")]
	public class UILineTextureRenderer : UIPrimitiveBase
	{
		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		[SerializeField]
		private Vector2[] m_points;

		public float LineThickness = 2f;

		public bool UseMargins;

		public Vector2 Margin;

		public bool relativeSize;

		public Rect uvRect
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return m_UVRect;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				if (!(m_UVRect == value))
				{
					m_UVRect = value;
					((Graphic)this).SetVerticesDirty();
				}
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

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			if (m_points == null || m_points.Length < 2)
			{
				m_points = (Vector2[])(object)new Vector2[2]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 1f)
				};
			}
			int num = 24;
			Rect rect = ((Graphic)this).get_rectTransform().get_rect();
			float num2 = ((Rect)(ref rect)).get_width();
			rect = ((Graphic)this).get_rectTransform().get_rect();
			float num3 = ((Rect)(ref rect)).get_height();
			float num4 = 0f - ((Graphic)this).get_rectTransform().get_pivot().x;
			rect = ((Graphic)this).get_rectTransform().get_rect();
			float num5 = num4 * ((Rect)(ref rect)).get_width();
			float num6 = 0f - ((Graphic)this).get_rectTransform().get_pivot().y;
			rect = ((Graphic)this).get_rectTransform().get_rect();
			float num7 = num6 * ((Rect)(ref rect)).get_height();
			if (!relativeSize)
			{
				num2 = 1f;
				num3 = 1f;
			}
			List<Vector2> list = new List<Vector2>();
			list.Add(m_points[0]);
			Vector2 val = m_points[0];
			Vector2 val2 = m_points[1] - m_points[0];
			Vector2 item = val + ((Vector2)(ref val2)).get_normalized() * (float)num;
			list.Add(item);
			for (int i = 1; i < m_points.Length - 1; i++)
			{
				list.Add(m_points[i]);
			}
			Vector2 val3 = m_points[m_points.Length - 1];
			val2 = m_points[m_points.Length - 1] - m_points[m_points.Length - 2];
			item = val3 - ((Vector2)(ref val2)).get_normalized() * (float)num;
			list.Add(item);
			list.Add(m_points[m_points.Length - 1]);
			Vector2[] array = list.ToArray();
			if (UseMargins)
			{
				num2 -= Margin.x;
				num3 -= Margin.y;
				num5 += Margin.x / 2f;
				num7 += Margin.y / 2f;
			}
			vh.Clear();
			Vector2 val4 = Vector2.get_zero();
			Vector2 val5 = Vector2.get_zero();
			Vector2 val12 = default(Vector2);
			Vector2 val13 = default(Vector2);
			Vector2 val14 = default(Vector2);
			Vector2 val15 = default(Vector2);
			Vector2 val16 = default(Vector2);
			for (int j = 1; j < array.Length; j++)
			{
				Vector2 val6 = array[j - 1];
				Vector2 val7 = array[j];
				((Vector2)(ref val6))._002Ector(val6.x * num2 + num5, val6.y * num3 + num7);
				((Vector2)(ref val7))._002Ector(val7.x * num2 + num5, val7.y * num3 + num7);
				float num8 = Mathf.Atan2(val7.y - val6.y, val7.x - val6.x) * 180f / (float)Math.PI;
				Vector2 val8 = val6 + new Vector2(0f, (0f - LineThickness) / 2f);
				Vector2 val9 = val6 + new Vector2(0f, LineThickness / 2f);
				Vector2 val10 = val7 + new Vector2(0f, LineThickness / 2f);
				Vector2 val11 = val7 + new Vector2(0f, (0f - LineThickness) / 2f);
				val8 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val8), Vector2.op_Implicit(val6), new Vector3(0f, 0f, num8)));
				val9 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val9), Vector2.op_Implicit(val6), new Vector3(0f, 0f, num8)));
				val10 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val10), Vector2.op_Implicit(val7), new Vector3(0f, 0f, num8)));
				val11 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val11), Vector2.op_Implicit(val7), new Vector3(0f, 0f, num8)));
				Vector2 zero = Vector2.get_zero();
				((Vector2)(ref val12))._002Ector(0f, 1f);
				((Vector2)(ref val13))._002Ector(0.5f, 0f);
				((Vector2)(ref val14))._002Ector(0.5f, 1f);
				((Vector2)(ref val15))._002Ector(1f, 0f);
				((Vector2)(ref val16))._002Ector(1f, 1f);
				Vector2[] uvs = (Vector2[])(object)new Vector2[4] { val13, val14, val14, val13 };
				if (j > 1)
				{
					vh.AddUIVertexQuad(SetVbo((Vector2[])(object)new Vector2[4] { val4, val5, val8, val9 }, uvs));
				}
				if (j == 1)
				{
					uvs = (Vector2[])(object)new Vector2[4] { zero, val12, val14, val13 };
				}
				else if (j == array.Length - 1)
				{
					uvs = (Vector2[])(object)new Vector2[4] { val13, val14, val16, val15 };
				}
				vh.AddUIVertexQuad(SetVbo((Vector2[])(object)new Vector2[4] { val8, val9, val10, val11 }, uvs));
				val4 = val10;
				val5 = val11;
			}
		}

		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = point - pivot;
			val = Quaternion.Euler(angles) * val;
			point = val + pivot;
			return point;
		}
	}
}
