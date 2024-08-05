using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGMaskedProgressBar : UIBehaviour
{
	public Image m_Bar;

	public Image m_Mask;

	private int m_Progress = -1;

	protected override void Start()
	{
		base.Start();
		m_Bar.fillAmount = 0f;
	}

	public void SetProgress(int percent)
	{
		m_Progress = percent;
		m_Bar.fillAmount = (float)percent / 100f;
	}
}
