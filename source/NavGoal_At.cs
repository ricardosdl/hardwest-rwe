public class NavGoal_At : NavGoalEvaluator
{
	public CFGCell m_Goal;

	public NavGoal_At(CFGCell atTile)
	{
		m_Goal = atTile;
	}

	public override bool Initialize(NavigationComponent navHandle)
	{
		return base.Initialize(navHandle);
	}

	public override bool Evaluate(CFGCell possibleGoal)
	{
		if (possibleGoal == m_Goal)
		{
			return true;
		}
		return false;
	}
}
