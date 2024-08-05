using UnityEngine;

public class BTTask_Log : BTTask
{
	public string m_Text = string.Empty;

	protected override EBTResult OnExecute(BehaviorTree agent)
	{
		Debug.Log(string.Concat(agent, " : ", m_Text));
		return EBTResult.Success;
	}
}
