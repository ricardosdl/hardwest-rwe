namespace BehaviorPattern.StateMachine;

public class SMInstance : BehaviorInstance
{
	public StateMachine m_Template;

	public SMInstance(StateMachine asset)
	{
		m_Template = asset;
	}

	public override void UpdateSchedule(Scheduler schedule)
	{
		for (int num = schedule.Count - 1; num >= 0; num--)
		{
			BehaviorExec behaviorExec = schedule[num] as BehaviorExec;
			if (behaviorExec.Update())
			{
				TaskResult current = behaviorExec.Current;
				if (current == TaskResult.Running)
				{
				}
			}
		}
	}
}
