using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UI Ring")]
	public class UIRing : UIPrimitiveBase
	{
		public float innerRadius = 16f;

		public float outerRadius = 32f;

		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		private List<int> indices = new List<int>();

		private List<UIVertex> vertices = new List<UIVertex>();

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			float num = innerRadius * 2f;
			float num2 = outerRadius * 2f;
			vh.Clear();
			indices.Clear();
			vertices.Clear();
			int item = 0;
			int num3 = 1;
			float num4 = 360f / (float)ArcSteps;
			float num5 = Mathf.Cos(0f);
			float num6 = Mathf.Sin(0f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = Color32.op_Implicit(((Graphic)this).get_color());
			simpleVert.position = Vector2.op_Implicit(new Vector2(num2 * num5, num2 * num6));
			vertices.Add(simpleVert);
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(num * num5, num * num6);
			simpleVert.position = Vector2.op_Implicit(val);
			vertices.Add(simpleVert);
			for (int i = 1; i <= ArcSteps; i++)
			{
				float num7 = (float)Math.PI / 180f * ((float)i * num4);
				num5 = Mathf.Cos(num7);
				num6 = Mathf.Sin(num7);
				simpleVert.color = Color32.op_Implicit(((Graphic)this).get_color());
				simpleVert.position = Vector2.op_Implicit(new Vector2(num2 * num5, num2 * num6));
				vertices.Add(simpleVert);
				simpleVert.position = Vector2.op_Implicit(new Vector2(num * num5, num * num6));
				vertices.Add(simpleVert);
				int item2 = num3;
				indices.Add(item);
				indices.Add(num3 + 1);
				indices.Add(num3);
				num3++;
				item = num3;
				num3++;
				indices.Add(item);
				indices.Add(num3);
				indices.Add(item2);
			}
			vh.AddUIVertexStream(vertices, indices);
		}

		public void SetArcSteps(int steps)
		{
			ArcSteps = steps;
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
	}
}
