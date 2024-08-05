public class BTSelector : BTControl
{
	public override void PostLoad(BTTemplate template)
	{
		base.PostLoad(template);
		m_DataOffset = template.m_DataTemplate.InstanceMemory(0);
	}

	protected override EBTResult OnExecute(BehaviorTree agent)
	{
		EBTResult eBTResult = EBTResult.Success;
		if (m_Children.Count > 0)
		{
			agent.m_Data.SetMemory(m_DataOffset, 0);
			return ExecuteChildren(agent);
		}
		return EBTResult.Error;
	}

	protected EBTResult ExecuteChildren(BehaviorTree agent)
	{
		EBTResult eBTResult = EBTResult.Fail;
		int num = agent.m_Data.GetMemory<int>(m_DataOffset);
		if (num < m_Children.Count)
		{
			do
			{
				eBTResult = m_Children[num].Execute(agent, out var _);
				if (eBTResult != EBTResult.Execution)
				{
					num++;
				}
			}
			while (num < m_Children.Count && eBTResult == EBTResult.Fail);
		}
		else
		{
			eBTResult = ((m_Children.Count > 0) ? EBTResult.Success : EBTResult.Error);
		}
		agent.m_Data.SetMemory(m_DataOffset, num);
		return eBTResult;
	}

	public override void ChildExecutionFinished(BehaviorTree agent, BTNode child, EBTResult result)
	{
	}
}
