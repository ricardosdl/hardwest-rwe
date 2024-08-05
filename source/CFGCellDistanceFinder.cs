using System.Collections.Generic;

public static class CFGCellDistanceFinder
{
	private static Queue<CFGPair<int, CFGCell>> s_Queue = new Queue<CFGPair<int, CFGCell>>();

	private static HashSet<CFGCell> s_Closed = new HashSet<CFGCell>();

	public static HashSet<CFGCell> FindCellsInDistance(CFGCell start, int distance)
	{
		HashSet<CFGCell> hashSet = new HashSet<CFGCell>();
		if (!CFGCellMap.IsValid || start == null)
		{
			return hashSet;
		}
		distance *= 10;
		s_Queue.Enqueue(new CFGPair<int, CFGCell>(0, start));
		s_Closed.Add(start);
		for (CFGPair<int, CFGCell> cFGPair = s_Queue.Dequeue(); cFGPair != null; cFGPair = ((s_Queue.Count <= 0) ? null : s_Queue.Dequeue()))
		{
			if (cFGPair.First < distance)
			{
				foreach (CFGCell item in cFGPair.Second.FindNeighbours())
				{
					if (item == start || item.StairsType != CFGCell.EStairsType.Slope || cFGPair.Second.StairsType != 0)
					{
						TryQueueTile(cFGPair.First, item);
					}
				}
			}
		}
		hashSet.UnionWith(s_Closed);
		s_Queue.Clear();
		s_Closed.Clear();
		return hashSet;
	}

	private static void TryQueueTile(int distance, CFGCell tile)
	{
		if ((bool)tile && !s_Closed.Contains(tile))
		{
			s_Queue.Enqueue(new CFGPair<int, CFGCell>(distance + tile.CostToMove, tile));
			if (tile.HaveFloor)
			{
				s_Closed.Add(tile);
			}
		}
	}
}
