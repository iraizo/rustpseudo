using System.Collections.Generic;
using UnityEngine;

public class WorldSpline : MonoBehaviour
{
	public int dataIndex = -1;

	public Vector3[] points;

	public Vector3[] tangents;

	[Range(0.05f, 100f)]
	public float lutInterval = 0.25f;

	[SerializeField]
	private bool showGizmos = true;

	private static List<Vector3> visualSplineList = new List<Vector3>();

	public WorldSplineData GetData()
	{
		return WorldSplineSharedData.GetDataFor(this);
	}

	public void CheckValidity()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		lutInterval = Mathf.Clamp(lutInterval, 0.05f, 100f);
		if (points == null || points.Length < 2)
		{
			points = (Vector3[])(object)new Vector3[2];
			points[0] = Vector3.get_zero();
			points[1] = Vector3.get_zero();
		}
		if (tangents != null && points.Length == tangents.Length)
		{
			return;
		}
		Vector3[] array = (Vector3[])(object)new Vector3[points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (tangents != null && i < tangents.Length)
			{
				array[i] = tangents[i];
			}
			else
			{
				array[i] = Vector3.get_forward();
			}
		}
		tangents = array;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (showGizmos)
		{
			DrawSplineGizmo(this, ((Component)this).get_transform(), Color.get_magenta());
		}
	}

	protected static void DrawSplineGizmo(WorldSpline ws, Transform tr, Color splineColour)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ws == (Object)null)
		{
			return;
		}
		WorldSplineData data = ws.GetData();
		if (data == null || ws.points.Length < 2 || ws.points.Length != ws.tangents.Length)
		{
			return;
		}
		Vector3[] pointsWorld = ws.GetPointsWorld();
		Vector3[] tangentsWorld = ws.GetTangentsWorld();
		for (int i = 0; i < pointsWorld.Length; i++)
		{
			Gizmos.set_color(Color.get_magenta());
			Gizmos.DrawSphere(pointsWorld[i], 0.25f);
			if (((Vector3)(ref tangentsWorld[i])).get_magnitude() > 0f)
			{
				Gizmos.set_color(Color.get_cyan());
				Vector3 val = pointsWorld[i] + tangentsWorld[i] + Vector3.get_up() * 0.1f;
				Gizmos.DrawLine(pointsWorld[i] + Vector3.get_up() * 0.1f, val);
			}
		}
		Gizmos.set_color(splineColour);
		Vector3[] visualSpline = GetVisualSpline(ws, data, 1f);
		for (int j = 0; j < visualSpline.Length - 1; j++)
		{
			Gizmos.set_color(Color.Lerp(Color.get_white(), Color.get_magenta(), (float)j / (float)(visualSpline.Length - 1)));
			Gizmos.DrawLine(visualSpline[j], visualSpline[j + 1]);
			Gizmos.DrawLine(visualSpline[j], visualSpline[j] + Vector3.get_up() * 0.25f);
		}
	}

	private static Vector3[] GetVisualSpline(WorldSpline ws, WorldSplineData data, float distBetweenPoints)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		visualSplineList.Clear();
		if ((Object)(object)ws != (Object)null && ws.points.Length > 1)
		{
			Vector3 startPointWorld = ws.GetStartPointWorld();
			Vector3 endPointWorld = ws.GetEndPointWorld();
			visualSplineList.Add(startPointWorld);
			for (float num = distBetweenPoints; num <= data.Length - distBetweenPoints; num += distBetweenPoints)
			{
				visualSplineList.Add(ws.GetPointCubicHermiteWorld(num, data));
			}
			visualSplineList.Add(endPointWorld);
		}
		return visualSplineList.ToArray();
	}

	public Vector3 GetStartPointWorld()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().TransformPoint(points[0]);
	}

	public Vector3 GetEndPointWorld()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().TransformPoint(points[points.Length - 1]);
	}

	public Vector3 GetStartTangentWorld()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Scale(((Component)this).get_transform().get_rotation() * tangents[0], ((Component)this).get_transform().get_localScale());
	}

	public Vector3 GetEndTangentWorld()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Scale(((Component)this).get_transform().get_rotation() * tangents[tangents.Length - 1], ((Component)this).get_transform().get_localScale());
	}

	public Vector3 GetTangentWorld(float distance)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetTangentWorld(distance, GetData());
	}

	public Vector3 GetTangentWorld(float distance, WorldSplineData data)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Scale(((Component)this).get_transform().get_rotation() * data.GetTangent(distance), ((Component)this).get_transform().get_localScale());
	}

	public Vector3 GetPointCubicHermiteWorld(float distance)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetPointCubicHermiteWorld(distance, GetData());
	}

	public Vector3 GetPointCubicHermiteWorld(float distance, WorldSplineData data)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().TransformPoint(data.GetPointCubicHermite(distance));
	}

	public Vector3[] GetPointsWorld()
	{
		return PointsToWorld(points, ((Component)this).get_transform());
	}

	public Vector3[] GetTangentsWorld()
	{
		return TangentsToWorld(tangents, ((Component)this).get_transform());
	}

	private static Vector3[] PointsToWorld(Vector3[] points, Transform tr)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			array[i] = tr.TransformPoint(points[i]);
		}
		return array;
	}

	private static Vector3[] TangentsToWorld(Vector3[] tangents, Transform tr)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[tangents.Length];
		for (int i = 0; i < tangents.Length; i++)
		{
			array[i] = Vector3.Scale(tr.get_rotation() * tangents[i], tr.get_localScale());
		}
		return array;
	}

	public WorldSpline()
		: this()
	{
	}
}
