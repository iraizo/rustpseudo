using UnityEngine;

public class TickHistory
{
	private Deque<Vector3> points = new Deque<Vector3>(8);

	public int Count => points.get_Count();

	public void Reset()
	{
		points.Clear();
	}

	public void Reset(Vector3 point)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Reset();
		AddPoint(point);
	}

	public float Distance(BasePlayer player, Vector3 point)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (points.get_Count() == 0)
		{
			return player.Distance(point);
		}
		Vector3 position = ((Component)player).get_transform().get_position();
		Quaternion rotation = ((Component)player).get_transform().get_rotation();
		Bounds bounds = player.bounds;
		Matrix4x4 tickHistoryMatrix = player.tickHistoryMatrix;
		float num = float.MaxValue;
		Line val3 = default(Line);
		OBB val5 = default(OBB);
		for (int i = 0; i < points.get_Count(); i++)
		{
			Vector3 val = ((Matrix4x4)(ref tickHistoryMatrix)).MultiplyPoint3x4(points.get_Item(i));
			Vector3 val2 = ((i == points.get_Count() - 1) ? position : ((Matrix4x4)(ref tickHistoryMatrix)).MultiplyPoint3x4(points.get_Item(i + 1)));
			((Line)(ref val3))._002Ector(val, val2);
			Vector3 val4 = ((Line)(ref val3)).ClosestPoint(point);
			((OBB)(ref val5))._002Ector(val4, rotation, bounds);
			num = Mathf.Min(num, ((OBB)(ref val5)).Distance(point));
		}
		return num;
	}

	public void AddPoint(Vector3 point, int limit = -1)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		while (limit > 0 && points.get_Count() >= limit)
		{
			points.PopFront();
		}
		points.PushBack(point);
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < points.get_Count(); i++)
		{
			Vector3 val = points.get_Item(i);
			val = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(val);
			points.set_Item(i, val);
		}
	}
}
