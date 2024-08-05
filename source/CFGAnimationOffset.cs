using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CFGAnimationOffset : MonoBehaviour
{
	public float m_TimeOffset = 3f;

	private void Awake()
	{
		Animation component = GetComponent<Animation>();
		if (component != null && component.clip != null)
		{
			string text = component.clip.name;
			component[text].time = Random.Range(m_TimeOffset * -1f, m_TimeOffset);
		}
		else
		{
			Debug.Log("Animator is not assigned to the object with CFGAnimatorOffset Offset script! Zjebales!", base.gameObject);
			base.enabled = false;
		}
	}
}
