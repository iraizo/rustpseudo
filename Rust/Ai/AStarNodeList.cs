using System.Collections.Generic;

namespace Rust.AI
{
	public class AStarNodeList : List<AStarNode>
	{
		private class AStarNodeComparer : IComparer<AStarNode>
		{
			int IComparer<AStarNode>.Compare(AStarNode lhs, AStarNode rhs)
			{
				if (lhs < rhs)
				{
					return -1;
				}
				if (lhs > rhs)
				{
					return 1;
				}
				return 0;
			}
		}

		private readonly AStarNodeComparer comparer = new AStarNodeComparer();

		public bool Contains(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode aStarNode = base[i];
				if (aStarNode != null && ((object)aStarNode.Node).Equals((object)n))
				{
					return true;
				}
			}
			return false;
		}

		public AStarNode GetAStarNodeOf(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode aStarNode = base[i];
				if (aStarNode != null && ((object)aStarNode.Node).Equals((object)n))
				{
					return aStarNode;
				}
			}
			return null;
		}

		public void AStarNodeSort()
		{
			Sort(comparer);
		}
	}
}
