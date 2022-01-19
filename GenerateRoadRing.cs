using System.Collections.Generic;
using UnityEngine;

public class GenerateRoadRing : ProceduralComponent
{
	private class RingNode
	{
		public int attempts;

		public PathFinder.Point position;

		public PathFinder.Point direction;

		public RingNode next;

		public RingNode prev;

		public PathFinder.Node path;

		public RingNode(int pos_x, int pos_y, int dir_x, int dir_y, int stepcount)
		{
			position = new PathFinder.Point(pos_x, pos_y);
			direction = new PathFinder.Point(dir_x, dir_y);
			attempts = stepcount;
		}
	}

	public const float Width = 12f;

	public const float InnerPadding = 1f;

	public const float OuterPadding = 1f;

	public const float InnerFade = 1f;

	public const float OuterFade = 8f;

	public const float RandomScale = 0.75f;

	public const float MeshOffset = 0f;

	public const float TerrainOffset = -0.125f;

	private const int Smoothen = 4;

	private const int MaxDepth = 250000;

	public int MinWorldSize;

	public override void Process(uint seed)
	{
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked || World.Size < MinWorldSize)
		{
			return;
		}
		int[,] array = TerrainPath.CreateRoadCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array);
		int length = array.GetLength(0);
		int num = length / 4;
		int num2 = 4;
		int stepcount = num / num2;
		int num3 = length / 2;
		int pos_x = num;
		int pos_x2 = length - num;
		int pos_y = num;
		int pos_y2 = length - num;
		int num4 = 0;
		int dir_x = -num2;
		int dir_x2 = num2;
		int dir_y = -num2;
		int dir_y2 = num2;
		List<RingNode> list = ((World.Size >= 5000) ? new List<RingNode>
		{
			new RingNode(num3, pos_y2, num4, dir_y, stepcount),
			new RingNode(pos_x2, pos_y2, dir_x, dir_y, stepcount),
			new RingNode(pos_x2, num3, dir_x, num4, stepcount),
			new RingNode(pos_x2, pos_y, dir_x, dir_y2, stepcount),
			new RingNode(num3, pos_y, num4, dir_y2, stepcount),
			new RingNode(pos_x, pos_y, dir_x2, dir_y2, stepcount),
			new RingNode(pos_x, num3, dir_x2, num4, stepcount),
			new RingNode(pos_x, pos_y2, dir_x2, dir_y, stepcount)
		} : new List<RingNode>
		{
			new RingNode(pos_x2, pos_y2, dir_x, dir_y, stepcount),
			new RingNode(pos_x2, pos_y, dir_x, dir_y2, stepcount),
			new RingNode(pos_x, pos_y, dir_x2, dir_y2, stepcount),
			new RingNode(pos_x, pos_y2, dir_x2, dir_y, stepcount)
		});
		for (int i = 0; i < list.Count; i++)
		{
			RingNode ringNode = list[i];
			RingNode next = list[(i + 1) % list.Count];
			RingNode prev = list[(i - 1 + list.Count) % list.Count];
			ringNode.next = next;
			ringNode.prev = prev;
			while (!pathFinder.IsWalkable(ringNode.position))
			{
				if (ringNode.attempts <= 0)
				{
					return;
				}
				ringNode.position += ringNode.direction;
				ringNode.attempts--;
			}
		}
		foreach (RingNode item in list)
		{
			item.path = pathFinder.FindPath(item.position, item.next.position, 250000);
		}
		bool flag = false;
		while (!flag)
		{
			flag = true;
			PathFinder.Point point = new PathFinder.Point(0, 0);
			foreach (RingNode item2 in list)
			{
				point += item2.position;
			}
			point /= list.Count;
			float num5 = float.MinValue;
			RingNode ringNode2 = null;
			foreach (RingNode item3 in list)
			{
				if (item3.path == null)
				{
					Vector2 val = new Vector2((float)(item3.position.x - point.x), (float)(item3.position.y - point.y));
					float num6 = ((Vector2)(ref val)).get_magnitude();
					if (item3.prev.path == null)
					{
						num6 *= 1.5f;
					}
					if (num6 > num5)
					{
						num5 = num6;
						ringNode2 = item3;
					}
				}
			}
			if (ringNode2 == null)
			{
				continue;
			}
			do
			{
				if (ringNode2.attempts <= 0)
				{
					return;
				}
				ringNode2.position += ringNode2.direction;
				ringNode2.attempts--;
			}
			while (!pathFinder.IsWalkable(ringNode2.position));
			ringNode2.path = pathFinder.FindPath(ringNode2.position, ringNode2.next.position, 250000);
			ringNode2.prev.path = pathFinder.FindPath(ringNode2.prev.position, ringNode2.position, 250000);
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		for (int j = 0; j < list.Count; j++)
		{
			RingNode ringNode3 = list[j];
			RingNode ringNode4 = list[(j + 1) % list.Count];
			for (PathFinder.Node node = ringNode3.path; node != null; node = node.next)
			{
				for (PathFinder.Node node2 = ringNode4.path; node2 != null; node2 = node2.next)
				{
					if (Mathf.Abs(node.point.x - node2.point.x) <= 1 && Mathf.Abs(node.point.y - node2.point.y) <= 1)
					{
						node.next = null;
						ringNode4.path = node2;
						break;
					}
				}
			}
		}
		PathFinder.Node node3 = null;
		PathFinder.Node node4 = null;
		foreach (RingNode item4 in list)
		{
			if (node3 == null)
			{
				node3 = item4.path;
				node4 = item4.path;
			}
			else
			{
				node4.next = item4.path;
			}
			while (node4.next != null)
			{
				node4 = node4.next;
			}
		}
		node4.next = new PathFinder.Node(node3.point, node3.cost, node3.heuristic);
		List<Vector3> list2 = new List<Vector3>();
		for (PathFinder.Node node5 = node3; node5 != null; node5 = node5.next)
		{
			float normX = ((float)node5.point.x + 0.5f) / (float)length;
			float normZ = ((float)node5.point.y + 0.5f) / (float)length;
			float num7 = TerrainMeta.DenormalizeX(normX);
			float num8 = TerrainMeta.DenormalizeZ(normZ);
			float num9 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
			list2.Add(new Vector3(num7, num9, num8));
		}
		if (list2.Count >= 2)
		{
			int count = TerrainMeta.Path.Roads.Count;
			PathList pathList = new PathList("Road " + count, list2.ToArray());
			pathList.Width = 12f;
			pathList.InnerPadding = 1f;
			pathList.OuterPadding = 1f;
			pathList.InnerFade = 1f;
			pathList.OuterFade = 8f;
			pathList.RandomScale = 0.75f;
			pathList.MeshOffset = 0f;
			pathList.TerrainOffset = -0.125f;
			pathList.Topology = 2048;
			pathList.Splat = 128;
			pathList.Start = false;
			pathList.End = false;
			pathList.ProcgenStartNode = node3;
			pathList.ProcgenEndNode = node4;
			pathList.Path.Smoothen(4);
			pathList.Path.RecalculateTangents();
			pathList.AdjustPlacementMap(24f);
			TerrainMeta.Path.Roads.Add(pathList);
		}
	}
}
