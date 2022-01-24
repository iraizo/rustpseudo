using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
	public class UICircle : UIPrimitiveBase
	{
		[Tooltip("The Arc Invert property will invert the construction of the Arc.")]
		public bool ArcInvert = true;

		[Tooltip("The Arc property is a percentage of the entire circumference of the circle.")]
		[Range(0f, 1f)]
		public float Arc = 1f;

		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		[Tooltip("The Arc Rotation property permits adjusting the geometry orientation around the Z axis.")]
		[Range(0f, 360f)]
		public int ArcRotation;

		[Tooltip("The Progress property allows the primitive to be used as a progression indicator.")]
		[Range(0f, 1f)]
		public float Progress;

		private float _progress;

		public Color ProgressColor = new Color(255f, 255f, 255f, 255f);

		public bool Fill = true;

		public float Thickness = 5f;

		public int Padding;

		private List<int> indices = new List<int>();

		private List<UIVertex> vertices = new List<UIVertex>();

		private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			int num = ((!ArcInvert) ? 1 : (-1));
			Rect rect = ((Graphic)this).get_rectTransform().get_rect();
			float width = ((Rect)(ref rect)).get_width();
			rect = ((Graphic)this).get_rectTransform().get_rect();
			float num2;
			if (!(width < ((Rect)(ref rect)).get_height()))
			{
				rect = ((Graphic)this).get_rectTransform().get_rect();
				num2 = ((Rect)(ref rect)).get_height();
			}
			else
			{
				rect = ((Graphic)this).get_rectTransform().get_rect();
				num2 = ((Rect)(ref rect)).get_width();
			}
			float num3 = num2 - (float)Padding;
			float num4 = (0f - ((Graphic)this).get_rectTransform().get_pivot().x) * num3;
			float num5 = (0f - ((Graphic)this).get_rectTransform().get_pivot().x) * num3 + Thickness;
			vh.Clear();
			indices.Clear();
			vertices.Clear();
			int item = 0;
			int num6 = 1;
			int num7 = 0;
			float num8 = Arc * 360f / (float)ArcSteps;
			_progress = (float)ArcSteps * Progress;
			float num9 = (float)num * ((float)Math.PI / 180f) * (float)ArcRotation;
			float num10 = Mathf.Cos(num9);
			float num11 = Mathf.Sin(num9);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = Color32.op_Implicit((_progress > 0f) ? ProgressColor : ((Graphic)this).get_color());
			simpleVert.position = Vector2.op_Implicit(new Vector2(num4 * num10, num4 * num11));
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f);
			vertices.Add(simpleVert);
			Vector2 zero = default(Vector2);
			((Vector2)(ref zero))._002Ector(num5 * num10, num5 * num11);
			if (Fill)
			{
				zero = Vector2.get_zero();
			}
			simpleVert.position = Vector2.op_Implicit(zero);
			simpleVert.uv0 = (Vector2)(Fill ? uvCenter : new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f));
			vertices.Add(simpleVert);
			for (int i = 1; i <= ArcSteps; i++)
			{
				float num12 = (float)num * ((float)Math.PI / 180f) * ((float)i * num8 + (float)ArcRotation);
				num10 = Mathf.Cos(num12);
				num11 = Mathf.Sin(num12);
				simpleVert.color = Color32.op_Implicit(((float)i > _progress) ? ((Graphic)this).get_color() : ProgressColor);
				simpleVert.position = Vector2.op_Implicit(new Vector2(num4 * num10, num4 * num11));
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f);
				vertices.Add(simpleVert);
				if (!Fill)
				{
					simpleVert.position = Vector2.op_Implicit(new Vector2(num5 * num10, num5 * num11));
					simpleVert.uv0 = new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f);
					vertices.Add(simpleVert);
					num7 = num6;
					indices.Add(item);
					indices.Add(num6 + 1);
					indices.Add(num6);
					num6++;
					item = num6;
					num6++;
					indices.Add(item);
					indices.Add(num6);
					indices.Add(num7);
				}
				else
				{
					indices.Add(item);
					indices.Add(num6 + 1);
					if ((float)i > _progress)
					{
						indices.Add(ArcSteps + 2);
					}
					else
					{
						indices.Add(1);
					}
					num6++;
					item = num6;
				}
			}
			if (Fill)
			{
				simpleVert.position = Vector2.op_Implicit(zero);
				simpleVert.color = Color32.op_Implicit(((Graphic)this).get_color());
				simpleVert.uv0 = uvCenter;
				vertices.Add(simpleVert);
			}
			vh.AddUIVertexStream(vertices, indices);
		}

		public void SetProgress(float progress)
		{
			Progress = progress;
			((Graphic)this).SetVerticesDirty();
		}

		public void SetArcSteps(int steps)
		{
			ArcSteps = steps;
			((Graphic)this).SetVerticesDirty();
		}

		public void SetInvertArc(bool invert)
		{
			ArcInvert = invert;
			((Graphic)this).SetVerticesDirty();
		}

		public void SetArcRotation(int rotation)
		{
			ArcRotation = rotation;
			((Graphic)this).SetVerticesDirty();
		}

		public void SetFill(bool fill)
		{
			Fill = fill;
			((Graphic)this).SetVerticesDirty();
		}

		public void SetBaseColor(Color color)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((Graphic)this).set_color(color);
			((Graphic)this).SetVerticesDirty();
		}

		public void UpdateBaseAlpha(float value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Color color = ((Graphic)this).get_color();
			color.a = value;
			((Graphic)this).set_color(color);
			((Graphic)this).SetVerticesDirty();
		}

		public void SetProgressColor(Color color)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			ProgressColor = color;
			((Graphic)this).SetVerticesDirty();
		}

		public void UpdateProgressAlpha(float value)
		{
			ProgressColor.a = value;
			((Graphic)this).SetVerticesDirty();
		}

		public void SetPadding(int padding)
		{
			Padding = padding;
			((Graphic)this).SetVerticesDirty();
		}

		public void SetThickness(int thickness)
		{
			Thickness = thickness;
			((Graphic)this).SetVerticesDirty();
		}
	}
}
