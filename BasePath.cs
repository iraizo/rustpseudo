using System.Collections.Generic;
using UnityEngine;

public class BasePath : MonoBehaviour
{
	public List<BasePathNode> nodes;

	public List<PathInterestNode> interestZones;

	public List<PathSpeedZone> speedZones;

	public void Start()
	{
	}

	private void AddChildren()
	{
		if (nodes != null)
		{
			nodes.Clear();
			nodes.AddRange(((Component)this).GetComponentsInChildren<BasePathNode>());
			foreach (BasePathNode node in nodes)
			{
				node.Path = this;
			}
		}
		if (interestZones != null)
		{
			interestZones.Clear();
			interestZones.AddRange(((Component)this).GetComponentsInChildren<PathInterestNode>());
		}
		if (speedZones != null)
		{
			speedZones.Clear();
			speedZones.AddRange(((Component)this).GetComponentsInChildren<PathSpeedZone>());
		}
	}

	private void ClearChildren()
	{
		if (nodes != null)
		{
			foreach (BasePathNode node in nodes)
			{
				node.linked.Clear();
			}
		}
		nodes.Clear();
	}

	public static void AutoGenerateLinks(BasePath path)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		path.AddChildren();
		foreach (BasePathNode node in path.nodes)
		{
			if (node.linked == null)
			{
				node.linked = new List<BasePathNode>();
			}
			else
			{
				node.linked.Clear();
			}
			foreach (BasePathNode node2 in path.nodes)
			{
				if (!((Object)(object)node == (Object)(object)node2) && GamePhysics.LineOfSight(((Component)node).get_transform().get_position(), ((Component)node2).get_transform().get_position(), 429990145) && GamePhysics.LineOfSight(((Component)node2).get_transform().get_position(), ((Component)node).get_transform().get_position(), 429990145))
				{
					node.linked.Add(node2);
				}
			}
		}
	}

	public void GetNodesNear(Vector3 point, ref List<BasePathNode> nearNodes, float dist = 10f)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		foreach (BasePathNode node in nodes)
		{
			Vector3 val = Vector3Ex.XZ(point) - Vector3Ex.XZ(((Component)node).get_transform().get_position());
			if (((Vector3)(ref val)).get_sqrMagnitude() <= dist * dist)
			{
				nearNodes.Add(node);
			}
		}
	}

	public BasePathNode GetClosestToPoint(Vector3 point)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		BasePathNode result = nodes[0];
		float num = float.PositiveInfinity;
		foreach (BasePathNode node in nodes)
		{
			if (!((Object)(object)node == (Object)null) && !((Object)(object)((Component)node).get_transform() == (Object)null))
			{
				Vector3 val = point - ((Component)node).get_transform().get_position();
				float sqrMagnitude = ((Vector3)(ref val)).get_sqrMagnitude();
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = node;
				}
			}
		}
		return result;
	}

	public PathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		PathInterestNode pathInterestNode = null;
		int num = 0;
		while ((Object)(object)pathInterestNode == (Object)null && num < 20)
		{
			pathInterestNode = interestZones[Random.Range(0, interestZones.Count)];
			Vector3 val = ((Component)pathInterestNode).get_transform().get_position() - from;
			if (!(((Vector3)(ref val)).get_sqrMagnitude() < 100f))
			{
				break;
			}
			pathInterestNode = null;
			num++;
		}
		if ((Object)(object)pathInterestNode == (Object)null)
		{
			pathInterestNode = interestZones[0];
		}
		return pathInterestNode;
	}

	public BasePath()
		: this()
	{
	}
}
