using UnityEngine;
using UnityEngine.UI;

public class CFGWeaponRange : CFGPanel
{
	public Image m_WeaponMark;

	public Image m_Bar;

	private Vector3 m_WeaponMarkPosition = default(Vector3);

	protected override void Start()
	{
		base.Start();
		if (m_WeaponMark != null)
		{
			m_WeaponMarkPosition = m_WeaponMark.transform.localPosition;
		}
		SetWeaponMarkPosition(0f);
	}

	public void SetParams(int min_dist, int max_dist, int penalty, int vis_range)
	{
		if (m_Bar != null && m_Bar.material != null)
		{
			m_Bar.material.SetVector("_RangeParams", new Vector4(min_dist, max_dist, penalty, vis_range));
		}
	}

	public void SetWeaponMarkPosition(float percent)
	{
		if (m_Bar != null && m_WeaponMark != null)
		{
			float width = m_Bar.rectTransform.rect.width;
			m_WeaponMark.transform.localPosition = new Vector3(m_WeaponMarkPosition.x + width * percent, m_WeaponMarkPosition.y, m_WeaponMarkPosition.z);
		}
	}
}
