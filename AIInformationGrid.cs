using System;
using System.Collections.Generic;
using UnityEngine;

public class AIInformationGrid : MonoBehaviour
{
	public int CellSize = 10;

	public Bounds BoundingBox;

	public AIInformationCell[] Cells;

	private Vector3 origin;

	private int xCellCount;

	private int zCellCount;

	private const int maxPointResults = 2048;

	private AIMovePoint[] movePointResults = new AIMovePoint[2048];

	private AICoverPoint[] coverPointResults = new AICoverPoint[2048];

	private const int maxCellResults = 512;

	private AIInformationCell[] resultCells = new AIInformationCell[512];

	[ContextMenu("Init")]
	public void Init()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		AIInformationZone component = ((Component)this).GetComponent<AIInformationZone>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogWarning((object)"Unable to Init AIInformationGrid, no AIInformationZone found!");
			return;
		}
		BoundingBox = component.bounds;
		((Bounds)(ref BoundingBox)).set_center(((Component)this).get_transform().get_position() + ((Bounds)(ref component.bounds)).get_center() + new Vector3(0f, ((Bounds)(ref BoundingBox)).get_extents().y, 0f));
		float num = ((Bounds)(ref BoundingBox)).get_extents().x * 2f;
		float num2 = ((Bounds)(ref BoundingBox)).get_extents().z * 2f;
		xCellCount = (int)Mathf.Ceil(num / (float)CellSize);
		zCellCount = (int)Mathf.Ceil(num2 / (float)CellSize);
		Cells = new AIInformationCell[xCellCount * zCellCount];
		Vector3 val = (origin = ((Bounds)(ref BoundingBox)).get_min());
		val.x = ((Bounds)(ref BoundingBox)).get_min().x + (float)CellSize / 2f;
		val.z = ((Bounds)(ref BoundingBox)).get_min().z + (float)CellSize / 2f;
		Bounds bounds = default(Bounds);
		for (int i = 0; i < zCellCount; i++)
		{
			for (int j = 0; j < xCellCount; j++)
			{
				Vector3 val2 = val;
				((Bounds)(ref bounds))._002Ector(val2, new Vector3((float)CellSize, ((Bounds)(ref BoundingBox)).get_extents().y * 2f, (float)CellSize));
				Cells[GetIndex(j, i)] = new AIInformationCell(bounds, ((Component)this).get_gameObject(), j, i);
				val.x += CellSize;
			}
			val.x = ((Bounds)(ref BoundingBox)).get_min().x + (float)CellSize / 2f;
			val.z += CellSize;
		}
	}

	private int GetIndex(int x, int z)
	{
		return z * xCellCount + x;
	}

	public AIInformationCell CellAt(int x, int z)
	{
		return Cells[GetIndex(x, z)];
	}

	public AIMovePoint[] GetMovePointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		pointCount = 0;
		int cellCount;
		AIInformationCell[] cellsInRange = GetCellsInRange(position, maxRange, out cellCount);
		if (cellCount > 0)
		{
			for (int i = 0; i < cellCount; i++)
			{
				if (cellsInRange[i] == null)
				{
					continue;
				}
				Enumerator<AIMovePoint> enumerator = cellsInRange[i].MovePoints.Items.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						AIMovePoint current = enumerator.get_Current();
						movePointResults[pointCount] = current;
						pointCount++;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
		}
		return movePointResults;
	}

	public AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		pointCount = 0;
		int cellCount;
		AIInformationCell[] cellsInRange = GetCellsInRange(position, maxRange, out cellCount);
		if (cellCount > 0)
		{
			for (int i = 0; i < cellCount; i++)
			{
				if (cellsInRange[i] == null)
				{
					continue;
				}
				Enumerator<AICoverPoint> enumerator = cellsInRange[i].CoverPoints.Items.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						AICoverPoint current = enumerator.get_Current();
						coverPointResults[pointCount] = current;
						pointCount++;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
		}
		return coverPointResults;
	}

	public AIInformationCell[] GetCellsInRange(Vector3 position, float maxRange, out int cellCount)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		cellCount = 0;
		int num = (int)(maxRange / (float)CellSize);
		AIInformationCell cell = GetCell(position);
		if (cell == null)
		{
			return resultCells;
		}
		int num2 = Mathf.Max(cell.X - num, 0);
		int num3 = Mathf.Min(cell.X + num, xCellCount - 1);
		int num4 = Mathf.Max(cell.Z - num, 0);
		int num5 = Mathf.Min(cell.Z + num, zCellCount - 1);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				resultCells[cellCount] = CellAt(j, i);
				cellCount++;
				if (cellCount >= 512)
				{
					return resultCells;
				}
			}
		}
		return resultCells;
	}

	public AIInformationCell GetCell(Vector3 position)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Cells == null)
		{
			return null;
		}
		Vector3 val = position - origin;
		if (val.x < 0f || val.z < 0f)
		{
			return null;
		}
		int num = (int)(val.x / (float)CellSize);
		int num2 = (int)(val.z / (float)CellSize);
		if (num < 0 || num >= xCellCount)
		{
			return null;
		}
		if (num2 < 0 || num2 >= zCellCount)
		{
			return null;
		}
		return CellAt(num, num2);
	}

	public void OnDrawGizmos()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (Cells != null)
		{
			AIInformationCell[] cells = Cells;
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i]?.DebugDraw(Color.get_white(), points: false);
			}
		}
	}

	public AIInformationGrid()
		: this()
	{
	}
}
