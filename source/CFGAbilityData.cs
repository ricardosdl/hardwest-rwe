using System;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

[Serializable]
public class CFGAbilityData
{
	public int m_MultiShotDamage = 5;

	public CFGBullet m_MultiShotBullet;

	public CFGBullet m_PenetrateBullet;

	public CFGBullet m_ArteryShotBullet;

	public CFGBullet m_ShadowKillBullet;

	public int m_RewardedKill_AP = 1;

	public int m_RewardedKill_HP = 2;

	public int m_RewardedKill_Luck = 10;

	public Transform m_RewardedKill_FX;

	public Transform m_RewardedKill_FX_w1;

	public Transform m_RewardedKill_FX_w2;

	public Transform m_ArteryShotRevolver;

	public Transform m_ArteryShotRifle;

	public Transform m_ArteryShotShotgun;

	public Transform m_ArteryShotSniperRifle;

	public Transform m_PenetrateShotRevolver;

	public Transform m_PenetrateShotRifle;

	public Transform m_PenetrateShotShotgun;

	public Transform m_PenetrateShotSniperRifle;

	public Transform m_RicochetBounceFX;

	public Transform m_TransfusionFX;

	public Transform m_PrayerFX;

	public Transform m_ShriekFX;

	public Transform m_SmellFX;

	public Transform m_MultiShotFX;

	public Texture2D m_DemonGradient;

	public Texture2D m_DemonNoise;

	public AnimationCurve m_DemonCurveFadeIn = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve m_DemnCurveFadeOut = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public Transform m_DemonAmbient;

	public Transform m_DemonFX;

	public Transform m_CourageFX;

	public int m_DemonImageID = 1;

	public SmokeTrail m_CannibalLeftFX;

	public SmokeTrail m_CannibalRightFX;

	public ParticleSystem m_CannibalHeadFX;

	public Transform m_EqualizationFX;

	public Transform m_EqualizationPP;

	public ParticleSystem m_PenetrateMuzzleFlash;

	public string m_Demon_PrefabName = "demon_gdcdemo";

	public Transform m_AOE_Circle_Default;

	public Transform m_AOE_Circle_Explosion;

	public Material m_AOE_Circle_Explosion_ActiveMat;

	public Material m_AOE_Circle_Explosion_InactiveMat;

	public CFGAiPreset m_IntimidateAiPreset;

	public int m_Eavesdrop_Range = 5;

	public int m_Eavesdrop_Chance = 30;
}
