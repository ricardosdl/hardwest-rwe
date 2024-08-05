using UnityEngine;

public class CFGSpawnOffset : MonoBehaviour
{
	public Vector3 m_Offset = Vector3.zero;

	public bool m_InWorldSpace;

	public bool m_UseHierarhicalLocalOffset;

	[Space(20f)]
	public bool m_UseInitialRotation;

	public Vector3 m_InitialRotation = Vector3.zero;

	private void Start()
	{
		if (!m_InWorldSpace)
		{
			if (!m_UseHierarhicalLocalOffset)
			{
				base.transform.localPosition += m_Offset;
			}
			else
			{
				GameObject gameObject = new GameObject("tempParent");
				gameObject.transform.position = base.gameObject.transform.position;
				gameObject.transform.rotation = base.gameObject.transform.rotation;
				gameObject.transform.SetParent(base.gameObject.transform.parent);
				base.gameObject.transform.SetParent(gameObject.transform);
				base.gameObject.transform.localPosition += m_Offset;
				base.gameObject.transform.SetParent(gameObject.transform.parent);
				Object.Destroy(gameObject);
			}
		}
		else
		{
			base.transform.position += m_Offset;
		}
		if (m_UseInitialRotation)
		{
			base.transform.localEulerAngles = m_InitialRotation;
		}
		Object.Destroy(this);
	}
}
