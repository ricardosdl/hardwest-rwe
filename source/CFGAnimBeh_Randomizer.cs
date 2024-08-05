using UnityEngine;

public class CFGAnimBeh_Randomizer : StateMachineBehaviour
{
	public int m_ClipsCount = 2;

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		animator.SetInteger("Random", Random.Range(0, m_ClipsCount));
	}
}
