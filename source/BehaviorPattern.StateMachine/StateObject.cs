using UnityEngine;

namespace BehaviorPattern.StateMachine;

public abstract class StateObject : ScriptableObject
{
	protected virtual void OnEnable()
	{
		base.hideFlags = HideFlags.HideInHierarchy;
	}
}
