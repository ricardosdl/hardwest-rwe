using UnityEngine;

public class CFGFloatingText : MonoBehaviour
{
	private TextMesh m_TextMesh;

	private float m_Lifetime;

	public static void SpawnText(Vector3 world_pos, string text, Color color)
	{
		if (CFGOptions.Gameplay.ShowFloatingTexts)
		{
			TextMesh floatingTextPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FloatingTextPrefab;
			if (floatingTextPrefab != null)
			{
				TextMesh textMesh = Object.Instantiate(floatingTextPrefab, world_pos, Quaternion.identity) as TextMesh;
				textMesh.text = text;
				textMesh.color = color;
				Object.Destroy(textMesh.gameObject, 3f);
			}
		}
	}

	private void Start()
	{
		m_TextMesh = GetComponent<TextMesh>();
	}

	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		position.y += Time.deltaTime * 0.7f;
		Transform transform = Camera.main.transform;
		if (transform != null)
		{
			Vector3 worldPosition = position + (position - transform.position);
			base.transform.LookAt(worldPosition, transform.up);
		}
		base.transform.position = position;
		m_Lifetime += Time.deltaTime;
		if (m_TextMesh != null && m_Lifetime > 2f)
		{
			Color color = m_TextMesh.color;
			color.a = 1f - (m_Lifetime - 2f);
			m_TextMesh.color = color;
		}
	}
}
