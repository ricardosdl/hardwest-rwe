using UnityEngine;

public abstract class NavGoalEvaluator
{
	public NavGoalEvaluator m_NextEvaluator;

	public virtual bool Initialize(NavigationComponent navHandle)
	{
		if (m_NextEvaluator != null)
		{
			return m_NextEvaluator.Initialize(navHandle);
		}
		return true;
	}

	public virtual bool Evaluate(CFGCell possibleGoal)
	{
		Debug.LogWarning("Evaluate not implemented!");
		return false;
	}

	public virtual bool DetermineFinalGoal()
	{
		if (m_NextEvaluator != null)
		{
			return m_NextEvaluator.DetermineFinalGoal();
		}
		return false;
	}

	public virtual bool SeedWorkingSet(NavigationComponent navHandle, CFGCell start, ref PathSearchData pathData)
	{
		if (start == null)
		{
			return false;
		}
		pathData.InitOpenSet(start);
		return true;
	}
}
