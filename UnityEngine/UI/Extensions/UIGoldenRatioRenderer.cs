using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	public class UIGoldenRatioRenderer : UILineRenderer
	{
		private enum Orientations
		{
			Left,
			Top,
			Right,
			Bottom
		}

		private readonly List<Vector2> _points = new List<Vector2>();

		private readonly List<Rect> _rects = new List<Rect>();

		private int canvasWidth;

		private int canvasHeight;

		public float lineThickness2 = 1f;

		private void DrawSpiral(VertexHelper vh)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			_points.Clear();
			_rects.Clear();
			float num = (1f + Mathf.Sqrt(5f)) / 2f;
			Rect pixelRect = ((Graphic)this).get_canvas().get_pixelRect();
			canvasWidth = (int)((Rect)(ref pixelRect)).get_width();
			pixelRect = ((Graphic)this).get_canvas().get_pixelRect();
			canvasHeight = (int)((Rect)(ref pixelRect)).get_height();
			Orientations orientation;
			float num2;
			float num3;
			if (canvasWidth > canvasHeight)
			{
				orientation = Orientations.Left;
				if ((float)canvasWidth / (float)canvasHeight > num)
				{
					num2 = canvasHeight;
					num3 = num2 * num;
				}
				else
				{
					num3 = canvasWidth;
					num2 = num3 / num;
				}
			}
			else
			{
				orientation = Orientations.Top;
				if ((float)canvasHeight / (float)canvasWidth > num)
				{
					num3 = canvasWidth;
					num2 = num3 * num;
				}
				else
				{
					num2 = canvasHeight;
					num3 = num2 / num;
				}
			}
			float num4 = -canvasWidth / 2;
			float num5 = canvasHeight / 2;
			num4 += ((float)canvasWidth - num3) / 2f;
			num5 += ((float)canvasHeight - num2) / 2f;
			List<Vector2> list = new List<Vector2>();
			DrawPhiRectangles(vh, list, num4, num5, num3, num2, orientation);
			if (list.Count > 1)
			{
				Vector2 val = list[0];
				Vector2 val2 = list[list.Count - 1];
				float num6 = val.x - val2.x;
				float num7 = val.y - val2.y;
				float num8 = Mathf.Sqrt(num6 * num6 + num7 * num7);
				float num9 = Mathf.Atan2(num7, num6);
				float num10 = (float)Math.PI / 50f;
				float num11 = 1f - 1f / num / 25f * 0.78f;
				Vector2 item = default(Vector2);
				while (num8 > 32f)
				{
					((Vector2)(ref item))._002Ector(val2.x + num8 * Mathf.Cos(num9), (float)canvasHeight - (val2.y + num8 * Mathf.Sin(num9)));
					_points.Add(item);
					num9 += num10;
					num8 *= num11;
				}
			}
		}

		private void DrawPhiRectangles(VertexHelper vh, List<Vector2> points, float x, float y, float width, float height, Orientations orientation)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			if (!(width < 1f) && !(height < 1f))
			{
				if (width >= 10f && height >= 10f)
				{
					_rects.Add(new Rect(x, y, width, height));
				}
				switch (orientation)
				{
				case Orientations.Left:
					points.Add(new Vector2(x, y + height));
					x += height;
					width -= height;
					orientation = Orientations.Top;
					break;
				case Orientations.Top:
					points.Add(new Vector2(x, y));
					y += width;
					height -= width;
					orientation = Orientations.Right;
					break;
				case Orientations.Right:
					points.Add(new Vector2(x + width, y));
					width -= height;
					orientation = Orientations.Bottom;
					break;
				case Orientations.Bottom:
					points.Add(new Vector2(x + width, y + height));
					height -= width;
					orientation = Orientations.Left;
					break;
				}
				DrawPhiRectangles(vh, points, x, y, width, height, orientation);
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)((Graphic)this).get_canvas() == (Object)null)
			{
				return;
			}
			relativeSize = false;
			DrawSpiral(vh);
			m_points = _points.ToArray();
			base.OnPopulateMesh(vh);
			foreach (Rect rect in _rects)
			{
				Rect current = rect;
				DrawRect(vh, new Rect(((Rect)(ref current)).get_x(), ((Rect)(ref current)).get_y() - lineThickness2 * 0.5f, ((Rect)(ref current)).get_width(), lineThickness2));
				DrawRect(vh, new Rect(((Rect)(ref current)).get_x() - lineThickness2 * 0.5f, ((Rect)(ref current)).get_y(), lineThickness2, ((Rect)(ref current)).get_height()));
				DrawRect(vh, new Rect(((Rect)(ref current)).get_x(), ((Rect)(ref current)).get_y() + ((Rect)(ref current)).get_height() - lineThickness2 * 0.5f, ((Rect)(ref current)).get_width(), lineThickness2));
				DrawRect(vh, new Rect(((Rect)(ref current)).get_x() + ((Rect)(ref current)).get_width() - lineThickness2 * 0.5f, ((Rect)(ref current)).get_y(), lineThickness2, ((Rect)(ref current)).get_height()));
			}
		}

		private void DrawRect(VertexHelper vh, Rect rect)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] array = (Vector2[])(object)new Vector2[4]
			{
				new Vector2(((Rect)(ref rect)).get_x(), ((Rect)(ref rect)).get_y()),
				new Vector2(((Rect)(ref rect)).get_x() + ((Rect)(ref rect)).get_width(), ((Rect)(ref rect)).get_y()),
				new Vector2(((Rect)(ref rect)).get_x() + ((Rect)(ref rect)).get_width(), ((Rect)(ref rect)).get_y() + ((Rect)(ref rect)).get_height()),
				new Vector2(((Rect)(ref rect)).get_x(), ((Rect)(ref rect)).get_y() + ((Rect)(ref rect)).get_height())
			};
			UIVertex[] array2 = (UIVertex[])(object)new UIVertex[4];
			for (int i = 0; i < array2.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = Color32.op_Implicit(((Graphic)this).get_color());
				Vector2 val = array[i];
				Rect pixelRect = ((Graphic)this).get_canvas().get_pixelRect();
				simpleVert.position = Vector2.op_Implicit(Vector2Ex.WithY(val, ((Rect)(ref pixelRect)).get_height() - array[i].y));
				array2[i] = simpleVert;
			}
			vh.AddUIVertexQuad(array2);
		}
	}
}
