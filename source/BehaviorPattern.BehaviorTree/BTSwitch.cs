using System.Collections.Generic;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

public abstract class BTSwitch<T> : BTComposite where T : BBVariable
{
	[SerializeField]
	protected T m_BBEnum;

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		yield break;
	}
}
