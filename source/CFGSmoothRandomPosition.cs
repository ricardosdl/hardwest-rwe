using UnityEngine;

public class CFGSmoothRandomPosition : MonoBehaviour
{
	public float m_Speed = 1f;

	public Vector3 m_Range = new Vector3(1f, 1f, 1f);

	private Vector3 m_Position;

	private void Start()
	{
		m_Position = base.transform.position;
	}

	private void Update()
	{
		base.transform.position = m_Position + Vector3.Scale(SmoothRandom.GetVector3(m_Speed), m_Range);
	}
}
