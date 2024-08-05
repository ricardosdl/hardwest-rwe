using UnityEngine;
using UnityEngine.UI;

public class CFGCreditsPanel : CFGPanel
{
	public Text m_Texts;

	public Text m_Names;

	public Text m_Headers;

	public float m_Speed = 90f;

	private string[] m_SeparatedText;

	private int m_CurrentLine;

	private Vector3 m_TextsStart;

	private Vector3 m_NamesStart;

	private Vector3 m_HeadersStart;

	protected override void Start()
	{
		base.Start();
		string input = CFGData.ReadAllText(CFGData.GetDataPathFor("credits.tsv"));
		m_SeparatedText = input.ToLines();
		m_TextsStart = m_Texts.transform.localPosition;
		m_NamesStart = m_Names.transform.localPosition;
		m_HeadersStart = m_Headers.transform.localPosition;
		LoadNextPart();
	}

	public override void Update()
	{
		base.Update();
		if (IsPanelWaitingForDestroy())
		{
			return;
		}
		float y = Time.deltaTime * m_Speed;
		m_Texts.transform.Translate(0f, y, 0f);
		m_Names.transform.Translate(0f, y, 0f);
		m_Headers.transform.Translate(0f, y, 0f);
		if (Input.GetKeyDown(KeyCode.Escape) || CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.Start) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.Back) > 0f)
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadCreditsPanel(OnMenuUnloaded_MainMenu);
		}
		else if (m_Names.transform.position.y > (float)Screen.height + m_Names.preferredHeight)
		{
			if (m_CurrentLine < m_SeparatedText.Length)
			{
				m_Texts.transform.localPosition = m_TextsStart;
				m_Names.transform.localPosition = m_NamesStart;
				m_Headers.transform.localPosition = m_HeadersStart;
				LoadNextPart();
			}
			else
			{
				CFGSingleton<CFGWindowMgr>.Instance.UnloadCreditsPanel(OnMenuUnloaded_MainMenu);
			}
		}
	}

	private void LoadNextPart()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		for (int i = 0; i < 1000; i++)
		{
			if (m_CurrentLine >= m_SeparatedText.Length)
			{
				break;
			}
			string[] array = m_SeparatedText[m_CurrentLine].Split('\t');
			text += ((!(array[1] == "1")) ? "\n" : (array[0] + "\n"));
			text2 += ((!(array[1] == "2")) ? "\n" : (array[0] + "\n"));
			text3 += ((!(array[1] == "0")) ? "\n" : (array[0] + "\n"));
			m_CurrentLine++;
		}
		m_Texts.text = text;
		if (m_CurrentLine < 1500)
		{
			m_Names.text = text2.Substring(1);
		}
		else
		{
			m_Names.text = text2;
		}
		m_Headers.text = text3;
	}

	private void OnMenuUnloaded_MainMenu()
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadMainMenus();
	}
}
