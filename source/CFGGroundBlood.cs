using UnityEngine;

public class CFGGroundBlood : MonoBehaviour
{
	public Vector3 m_EmitterSize = new Vector3(1f, 0.1f, 1f);

	public Vector3 m_EmitterOffset;

	public Vector2 m_FadeInTimeRange = new Vector2(15f, 35f);

	public Vector2 m_SizeRange = new Vector2(0.5f, 1.5f);

	public int m_BloodAmount = 2;

	public Material[] m_Materials;

	public GameObject m_BloodProjector;

	private Projector[] m_BloodObject;

	private Material[] m_Material;

	private float m_Timer = 1E-11f;

	private float[] m_FadeInTime;

	private float m_LongestTime;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero + m_EmitterOffset, m_EmitterSize);
	}

	private void Start()
	{
		m_FadeInTime = new float[m_BloodAmount];
		m_Timer = 0f;
		m_BloodObject = new Projector[m_BloodAmount];
		m_Material = new Material[m_BloodAmount];
		for (int i = 0; i < m_Material.Length; i++)
		{
			m_Material[i] = m_Materials[Random.Range(0, m_Materials.Length)];
		}
		for (int j = 0; j < m_BloodAmount; j++)
		{
			GameObject gameObject = Object.Instantiate(m_BloodProjector);
			m_BloodObject[j] = gameObject.GetComponent<Projector>();
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.gameObject.transform.Translate(m_EmitterOffset, Space.Self);
			gameObject.gameObject.transform.localPosition += new Vector3(Random.Range(0f - m_EmitterSize.x, m_EmitterSize.x) * 0.5f, Random.Range(0f - m_EmitterSize.y, m_EmitterSize.y) * 0.5f, Random.Range(0f - m_EmitterSize.z, m_EmitterSize.z) * 0.5f);
			Vector3 localEulerAngles = m_BloodObject[j].transform.localEulerAngles;
			localEulerAngles.y = Random.Range(0, 360);
			gameObject.gameObject.transform.localEulerAngles = localEulerAngles;
			float orthographicSize = Random.Range(m_SizeRange.x, m_SizeRange.y);
			m_BloodObject[j].orthographicSize = orthographicSize;
			m_BloodObject[j].material = Object.Instantiate(m_Material[j]);
			m_FadeInTime[j] = Random.Range(m_FadeInTimeRange.x, m_FadeInTimeRange.y);
			if (m_FadeInTime[j] > m_LongestTime)
			{
				m_LongestTime = m_FadeInTime[j];
			}
			UpdateBlood(j);
		}
	}

	private void Update()
	{
		for (int i = 0; i < m_BloodAmount; i++)
		{
			UpdateBlood(i);
		}
		if (m_Timer >= m_LongestTime)
		{
			for (int j = 0; j < m_BloodAmount; j++)
			{
				m_BloodObject[j].transform.SetParent(base.transform.parent.transform);
				if (m_Material[j] != null)
				{
					Object.Destroy(m_BloodObject[j].material);
					m_BloodObject[j].material = m_Material[j];
				}
			}
			Object.Destroy(base.gameObject);
		}
		m_Timer += Time.deltaTime;
	}

	private void UpdateBlood(int bloodIndex)
	{
		m_BloodObject[bloodIndex].material.SetFloat("_AlphaBoost", 1f - m_Timer / m_LongestTime);
	}

	private void UpdateBlood(int bloodIndex, float alphaValue)
	{
		m_BloodObject[bloodIndex].material.SetFloat("_AlphaBoost", alphaValue);
	}
}
