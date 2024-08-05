using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGStrategicExplorator : CFGPanel
{
	public delegate void OnLocationPanelButtonClickDelegate(int button_id);

	public GameObject m_Bg;

	public GameObject m_ExplorationWindow;

	public Text m_ExplorationWindowTitle;

	public CFGTextExtension m_ExplorationWindowText;

	public Image m_BgStretch;

	public Image m_Mask;

	public Image m_BottomLine;

	public RectTransform m_Content;

	public CFGImageExtension m_Image;

	private float m_AnimAlpha = -1f;

	public OnLocationPanelButtonClickDelegate m_OnLocationPanelButtonClickCallback;

	public List<CFGButtonExtension> m_ExplorationButtons = new List<CFGButtonExtension>();

	public CanvasGroup m_ObjsToFadeInParent;

	public CanvasGroup m_AllPanelCG;

	public bool m_WindowNeedRefresh = true;

	public float m_InitTime = -1f;

	public GameObject m_PadBtns;

	public CFGButtonExtension m_APad;

	public CFGButtonExtension m_LPad;

	private List<Vector3> top = new List<Vector3>();

	private Vector3 content_y = default(Vector3);

	private Vector3 main_pos = default(Vector3);

	private CFGJoyMenuController m_ControllerInput = new CFGJoyMenuController();

	private bool m_bGotPointer;

	private bool m_bVisible;

	private bool m_WaitingForSkip = true;

	private float m_NextReadAnalog;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	protected override void Start()
	{
		base.Start();
		Debug.Log("StrategicHud loaded");
		HideExplorationWindow();
		foreach (CFGButtonExtension explorationButton in m_ExplorationButtons)
		{
			explorationButton.m_ButtonClickedCallback = OnBuildingButtonClick;
		}
		foreach (CFGButtonExtension explorationButton2 in m_ExplorationButtons)
		{
			RectTransform component = explorationButton2.gameObject.GetComponent<RectTransform>();
			top.Add(component.localPosition);
		}
		content_y = m_Content.localPosition;
		main_pos = base.transform.position;
		m_bGotPointer = false;
		m_ControllerInput.Reset();
		m_ControllerInput.AddList(m_ExplorationButtons, null, OnControllerItemActivated, 1, bCheckText: true);
		m_ExplorationWindowText.m_AnimBeginCallback = OnTextAnimBegin;
		m_ExplorationWindowText.m_AnimEndCallback = OnTextAnimEnd;
		bool active = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_PadBtns.SetActive(active);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
	}

	public void OnTextAnimBegin()
	{
		m_ObjsToFadeInParent.alpha = 0f;
	}

	public void OnTextAnimEnd()
	{
		m_AnimAlpha = 0.01f;
	}

	public void SetCurrentLabelsPosition()
	{
		int num = 0;
		for (int i = 0; i < m_ExplorationButtons.Count; i++)
		{
			TextGenerator cachedTextGenerator = m_ExplorationButtons[i].m_Label.cachedTextGenerator;
			float num2 = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
			if (num2 >= 1f)
			{
				m_ExplorationButtons[i].m_Label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f * (float)cachedTextGenerator.lineCount * num2);
			}
			else
			{
				m_ExplorationButtons[i].m_Label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20f * (float)cachedTextGenerator.lineCount * num2);
			}
			float num3 = 80f / num2;
			num3 = 80f * num2;
			if (cachedTextGenerator.lineCount == 2)
			{
				num3 = 110f * num2;
			}
			else if (cachedTextGenerator.lineCount == 3)
			{
				num3 = 130f * num2;
			}
			Vector3 position = Vector3.zero;
			foreach (GameObject hoverObj in m_ExplorationButtons[i].m_HoverObjs)
			{
				if (hoverObj.name == "Podkreslenie")
				{
					RectTransform component = hoverObj.GetComponent<RectTransform>();
					if (component != null)
					{
						position = component.position;
					}
				}
			}
			foreach (GameObject hoverObj2 in m_ExplorationButtons[i].m_HoverObjs)
			{
				if (hoverObj2.name == "Podswietlenie")
				{
					RectTransform component2 = hoverObj2.GetComponent<RectTransform>();
					if (component2 != null)
					{
						component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num3);
						component2.position = position;
					}
				}
			}
			foreach (GameObject clickedObj in m_ExplorationButtons[i].m_ClickedObjs)
			{
				if (clickedObj.name == "Podswietlenie")
				{
					RectTransform component3 = clickedObj.GetComponent<RectTransform>();
					if (component3 != null)
					{
						component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num3);
						component3.position = position;
					}
				}
			}
			RectTransform component4 = m_ExplorationButtons[i].gameObject.GetComponent<RectTransform>();
			if (cachedTextGenerator != null && component4 != null)
			{
				if (cachedTextGenerator.lineCount > 0 && component4.localPosition != top[i])
				{
					component4.localPosition = top[i];
				}
				if (cachedTextGenerator.lineCount > 0)
				{
					num += cachedTextGenerator.lineCount - 1;
				}
				if (cachedTextGenerator.lineCount > 0)
				{
					component4.Translate(0f, (float)(-num) * 12f / num2, 0f);
				}
				if (cachedTextGenerator.lineCount > 0)
				{
					num += cachedTextGenerator.lineCount - 1;
				}
			}
		}
	}

	public override void SetLocalisation()
	{
		m_APad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_strategicpause_chooseoption");
		m_LPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_strategicpause_movecursor");
	}

	public override void Update()
	{
		base.Update();
		if (!m_bVisible)
		{
			return;
		}
		bool active = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_PadBtns.SetActive(active);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
		{
			m_APad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Top) > 0.4f && Time.time > m_NextReadAnalog)
		{
			m_LPad.SimulateClickGraphicAndSoundOnly();
			m_NextReadAnalog = Time.time + 0.4f;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom) > 0.4f && Time.time > m_NextReadAnalog)
		{
			m_LPad.SimulateClickGraphicAndSoundOnly();
			m_NextReadAnalog = Time.time + 0.4f;
		}
		if (!m_bGotPointer)
		{
			if ((bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem)
			{
				PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
				pointerEventData.position = Input.mousePosition;
				List<RaycastResult> list = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerEventData, list);
				foreach (CFGButtonExtension explorationButton in m_ExplorationButtons)
				{
					if ((bool)explorationButton)
					{
						explorationButton.OnPointerExit(pointerEventData);
					}
				}
				foreach (RaycastResult item in list)
				{
					CFGButtonExtension component = item.gameObject.GetComponent<CFGButtonExtension>();
					if ((bool)component && m_ExplorationButtons.Contains(component))
					{
						component.OnPointerEnter(pointerEventData);
						break;
					}
				}
			}
			m_ControllerInput.SelectFirstItem(bForce: false);
			m_bGotPointer = true;
		}
		if (m_AnimAlpha > 0f)
		{
			m_ObjsToFadeInParent.alpha = m_AnimAlpha;
			m_AnimAlpha += Time.deltaTime;
			if (m_AnimAlpha >= 1f)
			{
				m_AnimAlpha = -1f;
				m_ObjsToFadeInParent.alpha = 1f;
			}
		}
		m_ObjsToFadeInParent.interactable = m_ObjsToFadeInParent.alpha >= 0.2f;
		m_ObjsToFadeInParent.blocksRaycasts = m_ObjsToFadeInParent.alpha >= 0.2f;
		if (m_ControllerInput != null && CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			if (m_ControllerInput.CurrentListObject == null)
			{
				m_ControllerInput.SelectList(0);
			}
			if (m_ControllerInput.CurrentListObject != null && m_ControllerInput.CurrentListObject.m_CurrentItem < 0)
			{
				m_ControllerInput.CurrentListObject.SelectFirstItem();
			}
		}
		if (m_ObjsToFadeInParent.alpha >= 0.1f)
		{
			ClickButton(0, KeyCode.Alpha1);
			ClickButton(1, KeyCode.Alpha2);
			ClickButton(2, KeyCode.Alpha3);
			ClickButton(3, KeyCode.Alpha4);
			ClickButton(4, KeyCode.Alpha5);
			ClickButton(5, KeyCode.Alpha6);
			ClickButton(6, KeyCode.Alpha7);
			m_ControllerInput.UpdateInput();
		}
		else if (m_WaitingForSkip && (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f || CFGInput.IsActivated(EActionCommand.Skip_DialogLine) || CFGInput.IsActivated(EActionCommand.Skip_Dialog) || CFGOptions.Gameplay.InstantTexts))
		{
			m_ExplorationWindowText.GetComponent<CFGButtonExtension>().SimulateClick(bDelayed: false, bDeselect: true);
			m_WaitingForSkip = false;
		}
	}

	public void LateUpdate()
	{
		if (!m_WindowNeedRefresh || !(m_InitTime < Time.time))
		{
			return;
		}
		TextGenerationSettings generationSettings = m_ExplorationWindowText.GetGenerationSettings(m_ExplorationWindowText.rectTransform.rect.size);
		TextGenerator textGenerator = new TextGenerator();
		textGenerator.Populate(m_ExplorationWindowText.text, generationSettings);
		if (m_Content.localPosition != content_y)
		{
			m_Content.localPosition = content_y;
		}
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		if (textGenerator.lineCount > 2)
		{
			m_Content.Translate(0f, -22f * (float)(textGenerator.lineCount - 2) * num, 0f);
		}
		int num2 = 0;
		int num3 = 0;
		foreach (CFGButtonExtension explorationButton in m_ExplorationButtons)
		{
			if (explorationButton.m_Label.text != string.Empty && explorationButton.m_Label.cachedTextGenerator.lineCount > 0)
			{
				num2 += explorationButton.m_Label.cachedTextGenerator.lineCount + num3;
			}
			num3 = ((explorationButton.m_Label.text == string.Empty || explorationButton.m_Label.cachedTextGenerator.lineCount == 0) ? (num3 + 1) : 0);
		}
		SetCurrentLabelsPosition();
		m_BgStretch.transform.SetParent(m_Mask.transform.parent);
		m_BgStretch.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_BgStretch.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_BgStretch.rectTransform.anchorMin = new Vector2(0f, 0f);
		m_BgStretch.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_Mask.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_Mask.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_Mask.rectTransform.anchorMin = new Vector2(0f, 1f - (0.34f + 0.026f * (float)(textGenerator.lineCount + num2)));
		m_Mask.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_BgStretch.transform.SetParent(m_Mask.transform);
		m_BottomLine.transform.position = new Vector3(m_BottomLine.transform.position.x, m_Mask.transform.position.y, m_BottomLine.transform.position.y);
		Vector3 vector = m_Mask.transform.parent.transform.position - m_Mask.transform.position;
		base.transform.position = main_pos;
		base.transform.Translate(0f, vector.y / 2f + 50f, 0f);
		m_WindowNeedRefresh = false;
		m_ControllerInput.SelectFirstItem(bForce: false);
	}

	private bool ClickButton(int id, KeyCode key)
	{
		if (id >= m_ExplorationButtons.Count)
		{
			return false;
		}
		if (m_ExplorationButtons[id].m_Label.text == string.Empty)
		{
			return false;
		}
		if (!Input.GetKeyUp(key))
		{
			return false;
		}
		m_ExplorationButtons[id].SimulateClick();
		return true;
	}

	public void OnBuildingButtonClick(int button_id)
	{
		if (m_OnLocationPanelButtonClickCallback != null)
		{
			m_OnLocationPanelButtonClickCallback(button_id);
		}
		if (m_ExplorationButtons[button_id].m_UserData1 != 100)
		{
			return;
		}
		foreach (CFGCharacterData teamCharacters in CFGCharacterList.GetTeamCharactersList())
		{
			teamCharacters?.SetDecayLevel(teamCharacters.DecayLevel + m_ExplorationButtons[button_id].m_UserData2);
		}
	}

	public void OnCharacterButtonClick(int a)
	{
		Debug.Log("character button click");
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			instance.LoadCharacterScreen(combat_loadout: false, null);
		}
	}

	public void OnButtonPressedMenu(int a)
	{
		Debug.Log("Menu clicked");
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
		if ((bool)cFGSessionSingle && !cFGSessionSingle.IsLoadingLevel())
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadInGameMenu();
		}
	}

	private void OnControllerItemActivated(int ItemID, bool Secondary)
	{
		if (ItemID >= 0 && ItemID < m_ExplorationButtons.Count)
		{
			m_ExplorationButtons[ItemID].SimulateClick();
		}
	}

	public void ShowExplorationWindow()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadCharacterScreen();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadCardsPanell();
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_CharacterScreen, bEnable: false);
		m_ObjsToFadeInParent.alpha = 0f;
		m_AnimAlpha = -1f;
		m_bGotPointer = false;
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_StrategicExplorator, bEnable: true);
		m_ControllerInput.SelectFirstItem(bForce: false);
		m_bVisible = true;
		m_WindowNeedRefresh = true;
		m_InitTime = Time.time;
		m_WaitingForSkip = true;
		m_ExplorationWindowText.ResetClickable();
		FadeIn();
	}

	public void HideExplorationWindow()
	{
		foreach (CFGButtonExtension explorationButton in m_ExplorationButtons)
		{
			explorationButton.ResetButton();
		}
		m_ObjsToFadeInParent.blocksRaycasts = false;
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_StrategicExplorator, bEnable: false);
		m_bVisible = false;
		CFGJoyManager.ClearJoyActions();
		m_WaitingForSkip = true;
		FadeOut();
	}

	public bool IsExplorationWindowVisible()
	{
		return GetFadeState() != EFadeState.Hidden;
	}
}
