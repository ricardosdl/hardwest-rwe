using UnityEngine;

public class CFGCutscenePlaySound : MonoBehaviour
{
	public AudioClip m_AudioClip;

	public Transform m_Transform;

	public float m_MinDistance = 1f;

	public float m_MaxDistance = 500f;

	public void PlaySound()
	{
		if (m_AudioClip != null)
		{
			GameObject gameObject = new GameObject();
			if (m_Transform != null)
			{
				gameObject.transform.position = m_Transform.position;
			}
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.priority = 0;
			audioSource.minDistance = m_MinDistance;
			audioSource.maxDistance = m_MaxDistance;
			audioSource.clip = m_AudioClip;
			audioSource.Play();
			Object.Destroy(gameObject, m_AudioClip.length);
		}
	}
}
