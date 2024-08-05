using System.Collections;
using UnityEngine;

public class CFGProjectorRandomizer : MonoBehaviour
{
	public float m_LifeTime;

	public float m_FadeTime;

	public float m_RandomOffset;

	[SerializeField]
	private AnimationCurve m_FadeInCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

	[SerializeField]
	private Projector m_Projector;

	[SerializeField]
	private Material[] m_Materials;

	private CFGCamera m_Camera;

	private CFGCell m_Cell;

	private void Start()
	{
		m_FadeInCurve.preWrapMode = WrapMode.Once;
		m_FadeInCurve.postWrapMode = WrapMode.Once;
		m_Camera = Camera.main.GetComponent<CFGCamera>();
		if (m_Projector != null)
		{
			m_Projector.transform.Rotate(0f, 0f, Random.Range(-360f, 360f));
			Vector3 localPosition = m_Projector.transform.localPosition;
			localPosition.x += Random.Range(0f - m_RandomOffset, m_RandomOffset);
			localPosition.z += Random.Range(0f - m_RandomOffset, m_RandomOffset);
			m_Projector.transform.localPosition = localPosition;
			m_Projector.material = Object.Instantiate(m_Materials[Random.Range(0, m_Materials.Length)]);
			StartCoroutine("FadeActions");
		}
		if (m_Projector == null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator FadeActions()
	{
		bool finishedFade2 = false;
		float timer2 = 0f;
		while (!finishedFade2)
		{
			m_Projector.material.SetFloat("_AlphaBoost", m_FadeInCurve.Evaluate(timer2));
			if (timer2 >= m_FadeInCurve[m_FadeInCurve.length - 1].time)
			{
				finishedFade2 = true;
			}
			timer2 += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(m_LifeTime - m_FadeTime - timer2);
		finishedFade2 = false;
		timer2 = 0f;
		while (!finishedFade2)
		{
			m_Projector.material.SetFloat("_AlphaBoost", timer2 / m_FadeTime);
			if (timer2 / m_FadeTime >= 1f)
			{
				finishedFade2 = true;
			}
			timer2 += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		m_Cell = CFGCellMap.GetCharacterCell(base.transform.position);
		if (m_Camera != null && m_Cell != null && m_Projector != null)
		{
			m_Projector.enabled = (int)m_Camera.CurrentFloorLevel >= m_Cell.Floor;
		}
	}
}
