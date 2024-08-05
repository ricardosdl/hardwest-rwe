using System.Collections;
using UnityEngine;

public class CFGStrategicCursor : MonoBehaviour
{
	private enum m_CursorTypes
	{
		Normal,
		Inaccesible,
		Entrance
	}

	[SerializeField]
	private m_CursorTypes m_CursorType;

	[Space(20f)]
	private Animator m_Animator;

	private void Start()
	{
		m_Animator = base.gameObject.GetComponent<Animator>();
	}

	public void PlayAnimation()
	{
		if ((bool)m_Animator)
		{
			m_Animator.SetInteger("CursorType", (int)m_CursorType);
			m_Animator.SetBool("PlayAnim", value: true);
			StartCoroutine("StopAnimation");
		}
	}

	private IEnumerator StopAnimation()
	{
		yield return new WaitForSeconds(0.2f);
		if ((bool)m_Animator)
		{
			m_Animator.SetBool("PlayAnim", value: false);
		}
		yield return null;
	}
}
