namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
	public class UICornerCut : UIPrimitiveBase
	{
		public Vector2 cornerSize = new Vector2(16f, 16f);

		[Header("Corners to cut")]
		[SerializeField]
		private bool m_cutUL = true;

		[SerializeField]
		private bool m_cutUR;

		[SerializeField]
		private bool m_cutLL;

		[SerializeField]
		private bool m_cutLR;

		[Tooltip("Up-Down colors become Left-Right colors")]
		[SerializeField]
		private bool m_makeColumns;

		[Header("Color the cut bars differently")]
		[SerializeField]
		private bool m_useColorUp;

		[SerializeField]
		private Color32 m_colorUp;

		[SerializeField]
		private bool m_useColorDown;

		[SerializeField]
		private Color32 m_colorDown;

		public bool CutUL
		{
			get
			{
				return m_cutUL;
			}
			set
			{
				m_cutUL = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool CutUR
		{
			get
			{
				return m_cutUR;
			}
			set
			{
				m_cutUR = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool CutLL
		{
			get
			{
				return m_cutLL;
			}
			set
			{
				m_cutLL = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool CutLR
		{
			get
			{
				return m_cutLR;
			}
			set
			{
				m_cutLR = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool MakeColumns
		{
			get
			{
				return m_makeColumns;
			}
			set
			{
				m_makeColumns = value;
				((Graphic)this).SetAllDirty();
			}
		}

		public bool UseColorUp
		{
			get
			{
				return m_useColorUp;
			}
			set
			{
				m_useColorUp = value;
			}
		}

		public Color32 ColorUp
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return m_colorUp;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				m_colorUp = value;
			}
		}

		public bool UseColorDown
		{
			get
			{
				return m_useColorDown;
			}
			set
			{
				m_useColorDown = value;
			}
		}

		public Color32 ColorDown
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return m_colorDown;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				m_colorDown = value;
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			Rect rect = ((Graphic)this).get_rectTransform().get_rect();
			Rect val = rect;
			Color32 val2 = Color32.op_Implicit(((Graphic)this).get_color());
			bool flag = m_cutUL | m_cutUR;
			bool flag2 = m_cutLL | m_cutLR;
			bool flag3 = m_cutLL | m_cutUL;
			bool flag4 = m_cutLR | m_cutUR;
			if (!(flag || flag2) || !(((Vector2)(ref cornerSize)).get_sqrMagnitude() > 0f))
			{
				return;
			}
			vh.Clear();
			if (flag3)
			{
				((Rect)(ref val)).set_xMin(((Rect)(ref val)).get_xMin() + cornerSize.x);
			}
			if (flag2)
			{
				((Rect)(ref val)).set_yMin(((Rect)(ref val)).get_yMin() + cornerSize.y);
			}
			if (flag)
			{
				((Rect)(ref val)).set_yMax(((Rect)(ref val)).get_yMax() - cornerSize.y);
			}
			if (flag4)
			{
				((Rect)(ref val)).set_xMax(((Rect)(ref val)).get_xMax() - cornerSize.x);
			}
			Vector2 val3 = default(Vector2);
			Vector2 val4 = default(Vector2);
			Vector2 val5 = default(Vector2);
			Vector2 val6 = default(Vector2);
			if (m_makeColumns)
			{
				((Vector2)(ref val3))._002Ector(((Rect)(ref rect)).get_xMin(), m_cutUL ? ((Rect)(ref val)).get_yMax() : ((Rect)(ref rect)).get_yMax());
				((Vector2)(ref val4))._002Ector(((Rect)(ref rect)).get_xMax(), m_cutUR ? ((Rect)(ref val)).get_yMax() : ((Rect)(ref rect)).get_yMax());
				((Vector2)(ref val5))._002Ector(((Rect)(ref rect)).get_xMin(), m_cutLL ? ((Rect)(ref val)).get_yMin() : ((Rect)(ref rect)).get_yMin());
				((Vector2)(ref val6))._002Ector(((Rect)(ref rect)).get_xMax(), m_cutLR ? ((Rect)(ref val)).get_yMin() : ((Rect)(ref rect)).get_yMin());
				if (flag3)
				{
					AddSquare(val5, val3, new Vector2(((Rect)(ref val)).get_xMin(), ((Rect)(ref rect)).get_yMax()), new Vector2(((Rect)(ref val)).get_xMin(), ((Rect)(ref rect)).get_yMin()), rect, m_useColorUp ? m_colorUp : val2, vh);
				}
				if (flag4)
				{
					AddSquare(val4, val6, new Vector2(((Rect)(ref val)).get_xMax(), ((Rect)(ref rect)).get_yMin()), new Vector2(((Rect)(ref val)).get_xMax(), ((Rect)(ref rect)).get_yMax()), rect, m_useColorDown ? m_colorDown : val2, vh);
				}
			}
			else
			{
				((Vector2)(ref val3))._002Ector(m_cutUL ? ((Rect)(ref val)).get_xMin() : ((Rect)(ref rect)).get_xMin(), ((Rect)(ref rect)).get_yMax());
				((Vector2)(ref val4))._002Ector(m_cutUR ? ((Rect)(ref val)).get_xMax() : ((Rect)(ref rect)).get_xMax(), ((Rect)(ref rect)).get_yMax());
				((Vector2)(ref val5))._002Ector(m_cutLL ? ((Rect)(ref val)).get_xMin() : ((Rect)(ref rect)).get_xMin(), ((Rect)(ref rect)).get_yMin());
				((Vector2)(ref val6))._002Ector(m_cutLR ? ((Rect)(ref val)).get_xMax() : ((Rect)(ref rect)).get_xMax(), ((Rect)(ref rect)).get_yMin());
				if (flag2)
				{
					AddSquare(val6, val5, new Vector2(((Rect)(ref rect)).get_xMin(), ((Rect)(ref val)).get_yMin()), new Vector2(((Rect)(ref rect)).get_xMax(), ((Rect)(ref val)).get_yMin()), rect, m_useColorDown ? m_colorDown : val2, vh);
				}
				if (flag)
				{
					AddSquare(val3, val4, new Vector2(((Rect)(ref rect)).get_xMax(), ((Rect)(ref val)).get_yMax()), new Vector2(((Rect)(ref rect)).get_xMin(), ((Rect)(ref val)).get_yMax()), rect, m_useColorUp ? m_colorUp : val2, vh);
				}
			}
			if (m_makeColumns)
			{
				AddSquare(new Rect(((Rect)(ref val)).get_xMin(), ((Rect)(ref rect)).get_yMin(), ((Rect)(ref val)).get_width(), ((Rect)(ref rect)).get_height()), rect, val2, vh);
			}
			else
			{
				AddSquare(new Rect(((Rect)(ref rect)).get_xMin(), ((Rect)(ref val)).get_yMin(), ((Rect)(ref rect)).get_width(), ((Rect)(ref val)).get_height()), rect, val2, vh);
			}
		}

		private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			int num = AddVert(((Rect)(ref rect)).get_xMin(), ((Rect)(ref rect)).get_yMin(), rectUV, color32, vh);
			int num2 = AddVert(((Rect)(ref rect)).get_xMin(), ((Rect)(ref rect)).get_yMax(), rectUV, color32, vh);
			int num3 = AddVert(((Rect)(ref rect)).get_xMax(), ((Rect)(ref rect)).get_yMax(), rectUV, color32, vh);
			int num4 = AddVert(((Rect)(ref rect)).get_xMax(), ((Rect)(ref rect)).get_yMin(), rectUV, color32, vh);
			vh.AddTriangle(num, num2, num3);
			vh.AddTriangle(num3, num4, num);
		}

		private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int num = AddVert(a.x, a.y, rectUV, color32, vh);
			int num2 = AddVert(b.x, b.y, rectUV, color32, vh);
			int num3 = AddVert(c.x, c.y, rectUV, color32, vh);
			int num4 = AddVert(d.x, d.y, rectUV, color32, vh);
			vh.AddTriangle(num, num2, num3);
			vh.AddTriangle(num3, num4, num);
		}

		private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(Mathf.InverseLerp(((Rect)(ref area)).get_xMin(), ((Rect)(ref area)).get_xMax(), x), Mathf.InverseLerp(((Rect)(ref area)).get_yMin(), ((Rect)(ref area)).get_yMax(), y));
			vh.AddVert(new Vector3(x, y), color32, val);
			return vh.get_currentVertCount() - 1;
		}
	}
}
