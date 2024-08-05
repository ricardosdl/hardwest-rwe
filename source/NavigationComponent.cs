using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Navigation/Pathfinder")]
public class NavigationComponent : MonoBehaviour
{
	private NavGoalEvaluator NavGoalList;

	public bool EvaluateGoal(CFGCell possibleGoal)
	{
		NavGoalEvaluator navGoalEvaluator = NavGoalList;
		bool flag = false;
		while (navGoalEvaluator != null)
		{
			if (!navGoalEvaluator.Evaluate(possibleGoal))
			{
				return false;
			}
			navGoalEvaluator = navGoalEvaluator.m_NextEvaluator;
		}
		return true;
	}

	public void AddSuccesssorNodes(CFGCell fromNode, ref PathSearchData pathData)
	{
		bool flag = false;
		foreach (CFGCell item in fromNode.FindNeighbours())
		{
			flag |= EvaluateGoal(item);
			pathData.AddToOpenSet(fromNode, item);
		}
		if (flag)
		{
			return;
		}
		foreach (CFGCell item2 in fromNode.FindNeighbours(exclude_occupied: false))
		{
			if (EvaluateGoal(item2))
			{
				if (item2.CanStandOnThisTile(can_stand_now: false))
				{
					pathData.AddToOpenSet(fromNode, item2);
				}
				break;
			}
		}
	}

	public void SetGoalEvaluators(NavGoalEvaluator[] goalEvals)
	{
		NavGoalList = null;
		NavGoalEvaluator navGoalEvaluator = null;
		foreach (NavGoalEvaluator navGoalEvaluator2 in goalEvals)
		{
			if (navGoalEvaluator2 != null)
			{
				if (NavGoalList == null)
				{
					NavGoalList = navGoalEvaluator2;
					navGoalEvaluator = NavGoalList;
				}
				else
				{
					navGoalEvaluator.m_NextEvaluator = navGoalEvaluator2;
					navGoalEvaluator = navGoalEvaluator2;
				}
			}
		}
	}

	public bool GeneratePath(CFGCell start, NavGoalEvaluator[] goalEvals, out LinkedList<CFGCell> path)
	{
		SetGoalEvaluators(goalEvals);
		PathSearchData pathData = new PathSearchData(test: false);
		path = null;
		if (NavGoalList == null || !NavGoalList.Initialize(this))
		{
			Debug.LogWarning("initialize search returned false");
			return false;
		}
		if (!NavGoalList.SeedWorkingSet(this, start, ref pathData))
		{
			Debug.LogWarning("Failed to seed working set");
			return false;
		}
		while (pathData.IsOpen)
		{
			CFGCell cFGCell = pathData.PopBestNode();
			if (EvaluateGoal(cFGCell))
			{
				path = pathData.CreatePath(start, cFGCell);
				return true;
			}
			AddSuccesssorNodes(cFGCell, ref pathData);
		}
		return false;
	}

	public HashSet<CFGCell> FindTilesInDistance(CFGCell start, int distance)
	{
		HashSet<CFGCell> result = new HashSet<CFGCell>();
		if (start == null)
		{
			return result;
		}
		return result;
	}
}
