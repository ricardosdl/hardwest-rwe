using System.Collections;
using UnityEngine;

public class CFGUsableFenceAnimation : MonoBehaviour
{
	public CFGUsableObject m_UsableScript;

	public MeshRenderer m_FenceLeavesRenderer;

	private Material m_LeavesMaterial;

	[SerializeField]
	private AnimationCurve m_FadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	private float m_Timer;

	private void Start()
	{
		if (m_FenceLeavesRenderer != null)
		{
			for (int i = 0; i < m_FenceLeavesRenderer.materials.Length; i++)
			{
				if (m_FenceLeavesRenderer.materials[i].shader.name == "CFG/Custom/Wind Foliage AT Fading")
				{
					m_FenceLeavesRenderer.materials[i] = Object.Instantiate(m_FenceLeavesRenderer.materials[i]);
					m_LeavesMaterial = m_FenceLeavesRenderer.materials[i];
					break;
				}
			}
		}
		else
		{
			Debug.LogWarning("CFGUsableFenceAnimation with unassigned mesh renderer!", base.gameObject);
			base.enabled = false;
		}
		if (m_LeavesMaterial == null)
		{
			Debug.LogWarning("CFGUsableFenceAnimation can't find any material with WIND FOLIAGE AT FADING shader!", base.gameObject);
			base.enabled = false;
		}
		if (m_UsableScript == null)
		{
			Debug.LogWarning("CFGUsableFenceAnimation with unassigned UsableScript!", base.gameObject);
			base.enabled = false;
		}
		StartCoroutine(m_StartGetUsedState());
	}

	private IEnumerator m_StartGetUsedState()
	{
		while (!m_UsableScript.Used)
		{
			yield return null;
		}
		StartCoroutine(m_FadeAnimation());
	}

	private IEnumerator m_FadeAnimation()
	{
		while (m_Timer < m_FadeCurve[m_FadeCurve.length - 1].time)
		{
			m_LeavesMaterial.SetFloat("_Fade", m_FadeCurve.Evaluate(m_Timer));
			m_Timer += Time.deltaTime;
			yield return null;
		}
		base.enabled = false;
	}
}
