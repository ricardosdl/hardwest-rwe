using UnityEngine;

public class CFGAnimationAutoDestroy : MonoBehaviour
{
	[SerializeField]
	private int m_LoopCount;

	private float st;

	private void Awake()
	{
		Animation component = GetComponent<Animation>();
		if (m_LoopCount > 0 && component != null)
		{
			component.Stop();
			component.clip.wrapMode = WrapMode.Loop;
			component.Play(component.clip.name);
		}
		st = Time.time;
	}

	private void Update()
	{
		Animation component = GetComponent<Animation>();
		if (m_LoopCount > 0 && component != null && component.isPlaying && (bool)component.clip)
		{
			float num = (Time.time - st) / component.clip.length;
			if ((int)num >= m_LoopCount)
			{
				Object.Destroy(base.gameObject);
			}
		}
		if (component == null || !component.isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
