using UnityEngine;

public class CFGSpawnFxOnStart : MonoBehaviour
{
	[SerializeField]
	protected ParticleSystem m_FxPrefab;

	public ParticleSystem FxPrefab => m_FxPrefab;

	private void Start()
	{
		if (m_FxPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(m_FxPrefab.gameObject, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.SetParent(base.transform);
		}
		Object.Destroy(this);
	}
}
