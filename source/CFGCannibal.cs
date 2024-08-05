using System.Collections;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class CFGCannibal : MonoBehaviour
{
	private SmokeTrail m_BloodLeft;

	private SmokeTrail m_BloodRight;

	private ParticleSystem m_HeadParticles;

	private void Start()
	{
		Transform effectParent = base.transform.FindChild("head");
		m_HeadParticles = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_CannibalHeadFX;
		SetEffect(ref m_HeadParticles, effectParent, new Vector3(-0.3898f, 0.1392f, 0f));
		m_BloodRight = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_CannibalRightFX;
		Transform effectParent2 = base.transform.FindChild("Dummy_weapon_hand");
		SetEffect(ref m_BloodRight, effectParent2, new Vector3(0.0936f, -0.0368f, 0f));
		m_BloodLeft = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_CannibalLeftFX;
		Transform effectParent3 = base.transform.FindChild("Dummy_rifle_target");
		SetEffect(ref m_BloodLeft, effectParent3, new Vector3(0.607f, -0.031f, -0.011f));
		StartCoroutine("TrailsActions");
	}

	private IEnumerator TrailsActions()
	{
		yield return new WaitForSeconds(0.2f);
		m_BloodLeft.Emit = true;
		m_BloodRight.Emit = true;
		yield return new WaitForSeconds(2f);
		m_BloodLeft.Emit = false;
		m_BloodRight.Emit = false;
		Object.Destroy(this);
	}

	private void SetEffect(ref ParticleSystem effect, Transform effectParent, Vector3 targetPosition)
	{
		if (effectParent != null && effect != null)
		{
			effect = Object.Instantiate(effect);
			effect.transform.SetParent(effectParent);
			effect.transform.localPosition = targetPosition;
			effect.transform.localEulerAngles = Vector3.zero;
		}
	}

	private void SetEffect(ref SmokeTrail effect, Transform effectParent, Vector3 targetPosition)
	{
		if (effectParent != null && effect != null)
		{
			effect = Object.Instantiate(effect);
			effect.transform.SetParent(effectParent);
			effect.transform.localPosition = targetPosition;
			effect.transform.localEulerAngles = Vector3.zero;
			effect.Emit = false;
		}
	}
}
