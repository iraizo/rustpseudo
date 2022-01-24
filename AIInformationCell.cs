using System;
using System.Collections.Generic;
using UnityEngine;

public class AIInformationCell
{
	public Bounds BoundingBox;

	public List<AIInformationCell> NeighbourCells = new List<AIInformationCell>();

	public AIInformationCellContents<AIMovePoint> MovePoints = new AIInformationCellContents<AIMovePoint>();

	public AIInformationCellContents<AICoverPoint> CoverPoints = new AIInformationCellContents<AICoverPoint>();

	public int X { get; }

	public int Z { get; }

	public AIInformationCell(Bounds bounds, GameObject root, int x, int z)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		BoundingBox = bounds;
		X = x;
		Z = z;
		MovePoints.Init(bounds, root);
		CoverPoints.Init(bounds, root);
	}

	public void DebugDraw(Color color, bool points, float scale = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Color color2 = Gizmos.get_color();
		Gizmos.set_color(color);
		Gizmos.DrawWireCube(((Bounds)(ref BoundingBox)).get_center(), ((Bounds)(ref BoundingBox)).get_size() * scale);
		Gizmos.set_color(color2);
		if (!points)
		{
			return;
		}
		Enumerator<AIMovePoint> enumerator = MovePoints.Items.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AIMovePoint current = enumerator.get_Current();
				Gizmos.DrawLine(((Bounds)(ref BoundingBox)).get_center(), ((Component)current).get_transform().get_position());
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		Enumerator<AICoverPoint> enumerator2 = CoverPoints.Items.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				AICoverPoint current2 = enumerator2.get_Current();
				Gizmos.DrawLine(((Bounds)(ref BoundingBox)).get_center(), ((Component)current2).get_transform().get_position());
			}
		}
		finally
		{
			((IDisposable)enumerator2).Dispose();
		}
	}
}
