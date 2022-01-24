using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieShape : Graphic
{
	[Range(0f, 1f)]
	public float outerSize = 1f;

	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	public float startRadius = -45f;

	public float endRadius = 45f;

	public float border;

	public bool debugDrawing;

	protected override void OnPopulateMesh(VertexHelper vbo)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		vbo.Clear();
		UIVertex simpleVert = UIVertex.simpleVert;
		float num = startRadius;
		float num2 = endRadius;
		if (startRadius > endRadius)
		{
			num2 = endRadius + 360f;
		}
		float num3 = Mathf.Floor((num2 - num) / 6f);
		if (num3 <= 1f)
		{
			return;
		}
		float num4 = (num2 - num) / num3;
		float num5 = num + (num2 - num) * 0.5f;
		Color val = ((Graphic)this).get_color();
		Rect rect = ((Graphic)this).get_rectTransform().get_rect();
		float num6 = ((Rect)(ref rect)).get_height() * 0.5f;
		Vector2 val2 = new Vector2(Mathf.Sin(num5 * ((float)Math.PI / 180f)), Mathf.Cos(num5 * ((float)Math.PI / 180f))) * border;
		int num7 = 0;
		for (float num8 = num; num8 < num2; num8 += num4)
		{
			if (debugDrawing)
			{
				val = ((!(val == Color.get_red())) ? Color.get_red() : Color.get_white());
			}
			simpleVert.color = Color32.op_Implicit(val);
			float num9 = Mathf.Sin(num8 * ((float)Math.PI / 180f));
			float num10 = Mathf.Cos(num8 * ((float)Math.PI / 180f));
			float num11 = num8 + num4;
			if (num11 > num2)
			{
				num11 = num2;
			}
			float num12 = Mathf.Sin(num11 * ((float)Math.PI / 180f));
			float num13 = Mathf.Cos(num11 * ((float)Math.PI / 180f));
			simpleVert.position = Vector2.op_Implicit(new Vector2(num9 * outerSize * num6, num10 * outerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num12 * outerSize * num6, num13 * outerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num12 * innerSize * num6, num13 * innerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num9 * innerSize * num6, num10 * innerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			vbo.AddTriangle(num7, num7 + 1, num7 + 2);
			vbo.AddTriangle(num7 + 2, num7 + 3, num7);
			num7 += 4;
		}
	}

	public PieShape()
		: this()
	{
	}
}
