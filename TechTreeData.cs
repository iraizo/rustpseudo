using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTechTree", menuName = "Rust/Tech Tree", order = 2)]
public class TechTreeData : ScriptableObject
{
	[Serializable]
	public class NodeInstance
	{
		public int id;

		public ItemDefinition itemDef;

		public Vector2 graphPosition;

		public List<int> outputs = new List<int>();

		public List<int> inputs = new List<int>();

		public string groupName;

		public int costOverride = -1;

		public bool IsGroup()
		{
			if ((Object)(object)itemDef == (Object)null && groupName != "Entry")
			{
				return !string.IsNullOrEmpty(groupName);
			}
			return false;
		}
	}

	public string shortname;

	public int nextID;

	private Dictionary<int, NodeInstance> _idToNode;

	private NodeInstance _entryNode;

	public List<NodeInstance> nodes = new List<NodeInstance>();

	public NodeInstance GetByID(int id)
	{
		if (Application.get_isPlaying())
		{
			if (_idToNode == null)
			{
				_idToNode = Enumerable.ToDictionary<NodeInstance, int, NodeInstance>((IEnumerable<NodeInstance>)nodes, (Func<NodeInstance, int>)((NodeInstance n) => n.id), (Func<NodeInstance, NodeInstance>)((NodeInstance n) => n));
			}
			_idToNode.TryGetValue(id, out var value);
			return value;
		}
		_idToNode = null;
		foreach (NodeInstance node in nodes)
		{
			if (node.id == id)
			{
				return node;
			}
		}
		return null;
	}

	public NodeInstance GetEntryNode()
	{
		if (Application.get_isPlaying() && _entryNode != null && _entryNode.groupName == "Entry")
		{
			return _entryNode;
		}
		_entryNode = null;
		foreach (NodeInstance node in nodes)
		{
			if (node.groupName == "Entry")
			{
				_entryNode = node;
				return node;
			}
		}
		Debug.LogError((object)"NO ENTRY NODE FOR TECH TREE, This will Fail hard");
		return null;
	}

	public void ClearInputs(NodeInstance node)
	{
		foreach (int output in node.outputs)
		{
			NodeInstance byID = GetByID(output);
			byID.inputs.Clear();
			ClearInputs(byID);
		}
	}

	public void SetupInputs(NodeInstance node)
	{
		foreach (int output in node.outputs)
		{
			NodeInstance byID = GetByID(output);
			if (!byID.inputs.Contains(node.id))
			{
				byID.inputs.Add(node.id);
			}
			SetupInputs(byID);
		}
	}

	public bool PlayerHasPathForUnlock(BasePlayer player, NodeInstance node)
	{
		NodeInstance entryNode = GetEntryNode();
		if (entryNode == null)
		{
			return false;
		}
		return CheckChainRecursive(player, entryNode, node);
	}

	public bool CheckChainRecursive(BasePlayer player, NodeInstance start, NodeInstance target)
	{
		if (start.groupName != "Entry")
		{
			if (start.IsGroup())
			{
				foreach (int input in start.inputs)
				{
					if (!PlayerHasPathForUnlock(player, GetByID(input)))
					{
						return false;
					}
				}
			}
			else if (!HasPlayerUnlocked(player, start))
			{
				return false;
			}
		}
		bool result = false;
		foreach (int output in start.outputs)
		{
			if (output == target.id)
			{
				return true;
			}
			if (CheckChainRecursive(player, GetByID(output), target))
			{
				result = true;
			}
		}
		return result;
	}

	public bool PlayerCanUnlock(BasePlayer player, NodeInstance node)
	{
		if (PlayerHasPathForUnlock(player, node))
		{
			return !HasPlayerUnlocked(player, node);
		}
		return false;
	}

	public bool HasPlayerUnlocked(BasePlayer player, NodeInstance node)
	{
		if (node.IsGroup())
		{
			bool result = true;
			{
				foreach (int output in node.outputs)
				{
					NodeInstance byID = GetByID(output);
					if (!HasPlayerUnlocked(player, byID))
					{
						result = false;
					}
				}
				return result;
			}
		}
		return player.blueprints.HasUnlocked(node.itemDef);
	}

	public void GetNodesRequiredToUnlock(BasePlayer player, NodeInstance node, List<NodeInstance> foundNodes)
	{
		foundNodes.Add(node);
		if (node == GetEntryNode())
		{
			return;
		}
		if (node.inputs.Count == 1)
		{
			GetNodesRequiredToUnlock(player, GetByID(node.inputs[0]), foundNodes);
			return;
		}
		List<NodeInstance> list = Pool.GetList<NodeInstance>();
		int num = int.MaxValue;
		foreach (int input in node.inputs)
		{
			List<NodeInstance> list2 = Pool.GetList<NodeInstance>();
			GetNodesRequiredToUnlock(player, GetByID(input), list2);
			int num2 = 0;
			foreach (NodeInstance item in list2)
			{
				if (!((Object)(object)item.itemDef == (Object)null) && !HasPlayerUnlocked(player, item))
				{
					num2 += ResearchTable.ScrapForResearch(item.itemDef);
				}
			}
			if (num2 < num)
			{
				list.Clear();
				list.AddRange(list2);
				num = num2;
			}
			Pool.FreeList<NodeInstance>(ref list2);
		}
		foundNodes.AddRange(list);
		Pool.FreeList<NodeInstance>(ref list);
	}

	public TechTreeData()
		: this()
	{
	}
}
