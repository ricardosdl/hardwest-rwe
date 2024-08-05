using UnityEngine;

public class CFGStrategicClouds : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_CloudsSpeed = Vector3.one;

	private void FixedUpdate()
	{
		base.transform.Translate(m_CloudsSpeed, Space.World);
	}
}
