using UnityEngine;

public class CFGParticleSystemAutoDestroy : MonoBehaviour
{
	private void Update()
	{
		if ((!GetComponent<ParticleSystem>() || !GetComponent<ParticleSystem>().IsAlive()) && (!GetComponent<AudioSource>() || !GetComponent<AudioSource>().isPlaying))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
