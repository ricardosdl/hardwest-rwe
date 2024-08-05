using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Category("Alterate")]
public abstract class BTAlteratorBase<T> : BTNode where T : BBVariable
{
	public BBPropertyRef m_Result;

	public BBVariable m_Value;

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		while (true)
		{
			BBProperty property = m_Result.GetProperty(agent);
			if (property != null)
			{
				property.SetFromVariable(agent, m_Value);
				yield return TaskResult.Success;
			}
			else
			{
				yield return TaskResult.Fail;
			}
		}
	}
}
