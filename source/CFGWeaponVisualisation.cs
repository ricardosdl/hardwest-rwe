using UnityEngine;

public class CFGWeaponVisualisation : MonoBehaviour
{
	public CFGSoundDef m_FireSoundDef;

	public CFGSoundDef m_ReloadSoundDef;

	[SerializeField]
	private Transform m_BarrelEndPoint;

	public Transform BarrelEndPoint => m_BarrelEndPoint;
}
