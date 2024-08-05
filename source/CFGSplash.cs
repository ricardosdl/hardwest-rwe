using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGSplash : UIBehaviour
{
	public CFGImageExtension m_SplashMain;

	public Text m_TextCenter;

	public CFGImageExtension m_Splash;

	public Text m_Text;

	public CFGImageExtension m_BuffRecivedImage;

	public Text m_BuffRecivedText;

	public CFGCharacter m_Char;

	public int m_Number;

	public float m_FloatingSpeed = 0.015f;

	public bool m_Reversed;

	protected override void Start()
	{
		base.Start();
		Text[] componentsInChildren = base.gameObject.GetComponentsInChildren<Text>();
		foreach (Text text in componentsInChildren)
		{
			int num = 1;
			num = ((!text.resizeTextForBestFit) ? text.fontSize : text.resizeTextMaxSize);
			if (Screen.height > 900 || Screen.width > 1600)
			{
				text.resizeTextForBestFit = false;
			}
			if (text as CFGTextExtension == null)
			{
				text.fontSize = Mathf.FloorToInt(Mathf.Min(num * Screen.width / 1600, num * Screen.height / 900));
			}
		}
	}
}
