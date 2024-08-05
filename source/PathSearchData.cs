using System.Collections.Generic;
using System.Linq;

public struct PathSearchData
{
	public Dictionary<CFGCell, int> FScore;

	public Dictionary<CFGCell, int> GScore;

	public HashSet<CFGCell> OpenSet;

	private HashSet<CFGCell> ClosedSet;

	private Dictionary<CFGCell, CFGCell> PathTrail;

	public static int VHCost = 2;

	public static int DCost = 3;

	public bool IsOpen => OpenSet != null && OpenSet.Count > 0;

	public PathSearchData(bool test)
	{
		FScore = new Dictionary<CFGCell, int>();
		GScore = new Dictionary<CFGCell, int>();
		OpenSet = new HashSet<CFGCell>();
		ClosedSet = new HashSet<CFGCell>();
		PathTrail = new Dictionary<CFGCell, CFGCell>();
	}

	public CFGCell PopBestNode()
	{
		FScore = FScore.OrderBy((KeyValuePair<CFGCell, int> p) => p.Value).ToDictionary((KeyValuePair<CFGCell, int> x) => x.Key, (KeyValuePair<CFGCell, int> x) => x.Value);
		CFGCell key = FScore.ElementAt(0).Key;
		OpenSet.Remove(key);
		ClosedSet.Add(key);
		FScore.Remove(key);
		return key;
	}

	private int dist(CFGCell fromNode, CFGCell toNode)
	{
		if (toNode.PositionX != fromNode.PositionX && toNode.PositionZ != fromNode.PositionZ)
		{
			return DCost;
		}
		return VHCost;
	}

	public bool AddToOpenSet(CFGCell fromNode, CFGCell toNode)
	{
		if (toNode == null || ClosedSet.Contains(toNode))
		{
			return false;
		}
		int num = GScore[fromNode] + dist(fromNode, toNode);
		bool flag = OpenSet.Contains(toNode);
		if (!flag || num < GScore[toNode])
		{
			AddPathTrail(fromNode, toNode);
			if (flag)
			{
				GScore[toNode] = num;
				FScore[toNode] = GScore[toNode];
			}
			else
			{
				GScore.Add(toNode, num);
				FScore.Add(toNode, num);
			}
			if (!flag)
			{
				OpenSet.Add(toNode);
			}
		}
		return true;
	}

	public void InitOpenSet(CFGCell node)
	{
		if (!OpenSet.Contains(node))
		{
			GScore[node] = 0;
			FScore[node] = 0;
			OpenSet.Add(node);
		}
	}

	private void AddPathTrail(CFGCell fromNode, CFGCell toNode)
	{
		if (toNode != null)
		{
			if (PathTrail.ContainsKey(toNode))
			{
				PathTrail[toNode] = fromNode;
			}
			else
			{
				PathTrail.Add(toNode, fromNode);
			}
		}
	}

	public void Recycle()
	{
	}

	public LinkedList<CFGCell> CreatePath(CFGCell fromNode, CFGCell toNode)
	{
		LinkedList<CFGCell> linkedList = new LinkedList<CFGCell>();
		LinkedListNode<CFGCell> linkedListNode = linkedList.AddLast(toNode);
		while (toNode != fromNode)
		{
			linkedListNode = linkedList.AddBefore(linkedListNode, PathTrail[toNode]);
			toNode = linkedListNode.Value;
		}
		linkedListNode = linkedList.Last;
		while (linkedListNode != null && (bool)linkedListNode.Value && linkedListNode.Value.StairsType == CFGCell.EStairsType.Slope)
		{
			linkedList.RemoveLast();
			linkedListNode = linkedList.Last;
		}
		return linkedList;
	}
}
