using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	public static class AStarPath
	{
		private static float Heuristic(BasePathNode from, BasePathNode to)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)from).get_transform().get_position(), ((Component)to).get_transform().get_position());
		}

		public static bool FindPath(BasePathNode start, BasePathNode goal, out Stack<BasePathNode> path, out float pathCost)
		{
			path = null;
			pathCost = -1f;
			bool result = false;
			if ((Object)(object)start == (Object)(object)goal)
			{
				return false;
			}
			AStarNodeList aStarNodeList = new AStarNodeList();
			HashSet<BasePathNode> hashSet = new HashSet<BasePathNode>();
			AStarNode item = new AStarNode(0f, Heuristic(start, goal), null, start);
			aStarNodeList.Add(item);
			while (aStarNodeList.Count > 0)
			{
				AStarNode aStarNode = aStarNodeList[0];
				aStarNodeList.RemoveAt(0);
				hashSet.Add(aStarNode.Node);
				if (aStarNode.Satisfies(goal))
				{
					path = new Stack<BasePathNode>();
					pathCost = 0f;
					while (aStarNode.Parent != null)
					{
						pathCost += aStarNode.F;
						path.Push(aStarNode.Node);
						aStarNode = aStarNode.Parent;
					}
					if (aStarNode != null)
					{
						path.Push(aStarNode.Node);
					}
					result = true;
					break;
				}
				foreach (BasePathNode item2 in aStarNode.Node.linked)
				{
					if (!hashSet.Contains(item2))
					{
						float num = aStarNode.G + Heuristic(aStarNode.Node, item2);
						AStarNode aStarNodeOf = aStarNodeList.GetAStarNodeOf(item2);
						if (aStarNodeOf == null)
						{
							aStarNodeOf = new AStarNode(num, Heuristic(item2, goal), aStarNode, item2);
							aStarNodeList.Add(aStarNodeOf);
							aStarNodeList.AStarNodeSort();
						}
						else if (num < aStarNodeOf.G)
						{
							aStarNodeOf.Update(num, aStarNodeOf.H, aStarNode, item2);
							aStarNodeList.AStarNodeSort();
						}
					}
				}
			}
			return result;
		}
	}
}
