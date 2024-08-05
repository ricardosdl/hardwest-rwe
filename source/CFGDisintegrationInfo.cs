using System;
using UnityEngine;

[Serializable]
public class CFGDisintegrationInfo
{
	public float m_DelayAfterDeath = 4f;

	public float m_DisintegrationTime = 3f;

	public int m_MaxBodyCount = 10;

	public Transform m_FX;

	public Transform m_WeaponAmmo;

	public bool m_ActivateOnEndTurn = true;

	public bool m_RemoveNearbyBodies;
}
