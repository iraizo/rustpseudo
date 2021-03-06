using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
	public struct Point : IEquatable<Point>
	{
		public int x;

		public int y;

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static Point operator +(Point a, Point b)
		{
			return new Point(a.x + b.x, a.y + b.y);
		}

		public static Point operator -(Point a, Point b)
		{
			return new Point(a.x - b.x, a.y - b.y);
		}

		public static Point operator *(Point p, int i)
		{
			return new Point(p.x * i, p.y * i);
		}

		public static Point operator /(Point p, int i)
		{
			return new Point(p.x / i, p.y / i);
		}

		public static bool operator ==(Point a, Point b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Point a, Point b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode();
		}

		public override bool Equals(object other)
		{
			if (!(other is Point))
			{
				return false;
			}
			return Equals((Point)other);
		}

		public bool Equals(Point other)
		{
			if (x == other.x)
			{
				return y == other.y;
			}
			return false;
		}
	}

	public class Node : IMinHeapNode<Node>, ILinkedListNode<Node>
	{
		public Point point;

		public int cost;

		public int heuristic;

		public Node next { get; set; }

		public Node child { get; set; }

		public int order => cost + heuristic;

		public Node(Point point, int cost, int heuristic, Node next = null)
		{
			this.point = point;
			this.cost = cost;
			this.heuristic = heuristic;
			this.next = next;
		}
	}

	private int[,] costmap;

	private bool[,] visited;

	private Point[] neighbors;

	public Point PushPoint;

	public int PushRadius;

	public int PushDistance;

	public int PushMultiplier;

	private static Point[] mooreNeighbors = new Point[8]
	{
		new Point(0, 1),
		new Point(-1, 0),
		new Point(1, 0),
		new Point(0, -1),
		new Point(-1, 1),
		new Point(1, 1),
		new Point(-1, -1),
		new Point(1, -1)
	};

	private static Point[] neumannNeighbors = new Point[4]
	{
		new Point(0, 1),
		new Point(-1, 0),
		new Point(1, 0),
		new Point(0, -1)
	};

	public PathFinder(int[,] costmap, bool diagonals = true)
	{
		this.costmap = costmap;
		neighbors = (diagonals ? mooreNeighbors : neumannNeighbors);
	}

	public int GetResolution(int index)
	{
		return costmap.GetLength(index);
	}

	public Node FindPath(Point start, Point end, int depth = int.MaxValue)
	{
		return FindPathReversed(end, start, depth);
	}

	private Node FindPathReversed(Point start, Point end, int depth = int.MaxValue)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (visited == null)
		{
			visited = new bool[costmap.GetLength(0), costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(visited, 0, visited.Length);
		}
		int num = 0;
		int num2 = costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = costmap.GetLength(1) - 1;
		IntrusiveMinHeap<Node> val = default(IntrusiveMinHeap<Node>);
		int cost = Cost(start);
		int heuristic = Heuristic(start, end);
		val.Add(new Node(start, cost, heuristic));
		visited[start.x, start.y] = true;
		while (!val.get_Empty() && depth-- > 0)
		{
			Node node = val.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < neighbors.Length; i++)
			{
				Point point = node.point + neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4 && !visited[point.x, point.y])
				{
					visited[point.x, point.y] = true;
					int num5 = Cost(point);
					if (num5 != int.MaxValue)
					{
						int cost2 = node.cost + num5;
						int heuristic2 = Heuristic(point, end);
						val.Add(new Node(point, cost2, heuristic2, node));
					}
				}
			}
		}
		return null;
	}

	public Node FindPathDirected(List<Point> startList, List<Point> endList, int depth = int.MaxValue)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		return FindPathReversed(endList, startList, depth);
	}

	public Node FindPathUndirected(List<Point> startList, List<Point> endList, int depth = int.MaxValue)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		if (startList.Count > endList.Count)
		{
			return FindPathReversed(endList, startList, depth);
		}
		return FindPathReversed(startList, endList, depth);
	}

	private Node FindPathReversed(List<Point> startList, List<Point> endList, int depth = int.MaxValue)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (visited == null)
		{
			visited = new bool[costmap.GetLength(0), costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(visited, 0, visited.Length);
		}
		int num = 0;
		int num2 = costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = costmap.GetLength(1) - 1;
		IntrusiveMinHeap<Node> val = default(IntrusiveMinHeap<Node>);
		foreach (Point start in startList)
		{
			int cost = Cost(start);
			int heuristic = Heuristic(start, endList);
			val.Add(new Node(start, cost, heuristic));
			visited[start.x, start.y] = true;
		}
		while (!val.get_Empty() && depth-- > 0)
		{
			Node node = val.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < neighbors.Length; i++)
			{
				Point point = node.point + neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4 && !visited[point.x, point.y])
				{
					visited[point.x, point.y] = true;
					int num5 = Cost(point);
					if (num5 != int.MaxValue)
					{
						int cost2 = node.cost + num5;
						int heuristic2 = Heuristic(point, endList);
						val.Add(new Node(point, cost2, heuristic2, node));
					}
				}
			}
		}
		return null;
	}

	public Node FindClosestWalkable(Point start, int depth = int.MaxValue)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (visited == null)
		{
			visited = new bool[costmap.GetLength(0), costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(visited, 0, visited.Length);
		}
		int num = 0;
		int num2 = costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = costmap.GetLength(1) - 1;
		if (start.x < num)
		{
			return null;
		}
		if (start.x > num2)
		{
			return null;
		}
		if (start.y < num3)
		{
			return null;
		}
		if (start.y > num4)
		{
			return null;
		}
		IntrusiveMinHeap<Node> val = default(IntrusiveMinHeap<Node>);
		int cost = 1;
		int heuristic = Heuristic(start);
		val.Add(new Node(start, cost, heuristic));
		visited[start.x, start.y] = true;
		while (!val.get_Empty() && depth-- > 0)
		{
			Node node = val.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < neighbors.Length; i++)
			{
				Point point = node.point + neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4 && !visited[point.x, point.y])
				{
					visited[point.x, point.y] = true;
					int cost2 = node.cost + 1;
					int heuristic2 = Heuristic(point);
					val.Add(new Node(point, cost2, heuristic2, node));
				}
			}
		}
		return null;
	}

	public bool IsWalkable(Point point)
	{
		return costmap[point.x, point.y] != int.MaxValue;
	}

	public Node Reverse(Node start)
	{
		Node node = null;
		Node next = null;
		for (Node node2 = start; node2 != null; node2 = node2.next)
		{
			if (node != null)
			{
				node.next = next;
			}
			next = node;
			node = node2;
		}
		if (node != null)
		{
			node.next = next;
		}
		return node;
	}

	public Node FindEnd(Node start)
	{
		for (Node node = start; node != null; node = node.next)
		{
			if (node.next == null)
			{
				return node;
			}
		}
		return start;
	}

	public int Cost(Point a)
	{
		int num = costmap[a.x, a.y];
		int num2 = 0;
		if (num != int.MaxValue && PushMultiplier > 0)
		{
			int num3 = Mathf.Max(0, Heuristic(a, PushPoint) - PushRadius * PushRadius);
			int num4 = Mathf.Max(0, PushDistance * PushDistance - num3);
			num2 = PushMultiplier * num4;
		}
		return num + num2;
	}

	public int Heuristic(Point a)
	{
		if (costmap[a.x, a.y] != int.MaxValue)
		{
			return 0;
		}
		return 1;
	}

	public int Heuristic(Point a, Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return num * num + num2 * num2;
	}

	public int Heuristic(Point a, List<Point> b)
	{
		int num = int.MaxValue;
		for (int i = 0; i < b.Count; i++)
		{
			num = Mathf.Min(num, Heuristic(a, b[i]));
		}
		return num;
	}
}
