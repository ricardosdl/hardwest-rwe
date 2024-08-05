using UnityEngine;

public class CFGRotatingObjectFixed : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_EulerAnglesRotation = Vector3.zero;

	private void Update()
	{
		base.transform.Rotate(m_EulerAnglesRotation * Time.deltaTime);
	}
}
