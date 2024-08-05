using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class CFGMuzzleFlash : MonoBehaviour
{
	[SerializeField]
	private SmokePlume m_Smoke;

	public void Reparent(Transform targetParent)
	{
		if ((bool)m_Smoke)
		{
			m_Smoke.transform.SetParent(targetParent);
		}
		Object.Destroy(this);
	}
}
