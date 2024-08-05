using System.Collections.Generic;
using BehaviorPattern;
using UnityEngine;

namespace Gameplay;

[AddComponentMenu("Squad Behavior")]
public class SquadBehavior : BehaviorComponent
{
	[Task]
	[Param(typeof(BBInt), "data_0")]
	public IEnumerator<TaskResult> SimpleAction(BehaviorComponent agent, ITaskBody task)
	{
		int arg_0 = task.GetValue<int>(agent, "data_0");
		while (true)
		{
			Debug.Log("Test Run " + arg_0);
			yield return TaskResult.Success;
		}
	}

	[Check]
	[Param(typeof(BBInt), "data_0")]
	public bool SimpleCkeck(BehaviorComponent agent, ITaskBody task)
	{
		return true;
	}
}
