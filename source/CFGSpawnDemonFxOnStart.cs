using UnityEngine;

public class CFGSpawnDemonFxOnStart : CFGSpawnFxOnStart
{
	private void Awake()
	{
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_DemonFX != null)
		{
			m_FxPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_DemonFX.GetComponent<ParticleSystem>();
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_Demon, base.transform);
		}
	}
}
