using System.Collections.Generic;
using UnityEngine;

public class AIMovePointPath : MonoBehaviour
{
	public enum Mode
	{
		Loop,
		Reverse
	}

	public enum PathDirection
	{
		Forwards,
		Backwards
	}

	public Color DebugPathColor = Color.get_green();

	public Mode LoopMode;

	public List<AIMovePoint> Points = new List<AIMovePoint>();

	public void Clear()
	{
		Points.Clear();
	}

	public void AddPoint(AIMovePoint point)
	{
		Points.Add(point);
	}

	public AIMovePoint FindNearestPoint(Vector3 position)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		AIMovePoint result = null;
		foreach (AIMovePoint point in Points)
		{
			float num2 = Vector3.SqrMagnitude(position - ((Component)point).get_transform().get_position());
			if (num2 < num)
			{
				num = num2;
				result = point;
			}
		}
		return result;
	}

	public AIMovePoint GetNextPoint(AIMovePoint current, ref PathDirection pathDirection)
	{
		int num = 0;
		foreach (AIMovePoint point in Points)
		{
			if ((Object)(object)point == (Object)(object)current)
			{
				return GetNextPoint(num, ref pathDirection);
			}
			num++;
		}
		return null;
	}

	private AIMovePoint GetNextPoint(int currentPointIndex, ref PathDirection pathDirection)
	{
		int num = currentPointIndex + ((pathDirection == PathDirection.Forwards) ? 1 : (-1));
		if (num < 0)
		{
			if (LoopMode == Mode.Loop)
			{
				num = Points.Count - 1;
			}
			else
			{
				num = 1;
				pathDirection = PathDirection.Forwards;
			}
		}
		else if (num >= Points.Count)
		{
			if (LoopMode == Mode.Loop)
			{
				num = 0;
			}
			else
			{
				num = Points.Count - 2;
				pathDirection = PathDirection.Backwards;
			}
		}
		return Points[num];
	}

	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Color color = Gizmos.get_color();
		Gizmos.set_color(DebugPathColor);
		int num = -1;
		foreach (AIMovePoint point in Points)
		{
			num++;
			if (!((Object)(object)point == (Object)null))
			{
				if (num + 1 < Points.Count)
				{
					Gizmos.DrawLine(((Component)point).get_transform().get_position(), ((Component)Points[num + 1]).get_transform().get_position());
				}
				else if (LoopMode == Mode.Loop)
				{
					Gizmos.DrawLine(((Component)point).get_transform().get_position(), ((Component)Points[0]).get_transform().get_position());
				}
			}
		}
		Gizmos.set_color(color);
	}

	private void OnDrawGizmosSelected()
	{
		if (Points == null)
		{
			return;
		}
		foreach (AIMovePoint point in Points)
		{
			point.DrawLookAtPoints();
		}
	}

	public AIMovePointPath()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)

}
