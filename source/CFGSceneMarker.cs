using UnityEngine;

public class CFGSceneMarker : CFGSingleton<CFGSceneMarker>
{
	private GameObject m_Object;

	private Transform m_AlternativeTransform;

	private void Awake()
	{
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SceneMarkerPrefab != null)
		{
			m_Object = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SceneMarkerPrefab.gameObject);
		}
	}

	public void Show(Transform _transform)
	{
		if (!(m_Object == null))
		{
			m_Object.SetActive(value: true);
			m_Object.transform.position = _transform.position;
		}
	}

	public void Hide()
	{
		if (!(m_Object == null))
		{
			m_Object.SetActive(value: false);
		}
	}
}
