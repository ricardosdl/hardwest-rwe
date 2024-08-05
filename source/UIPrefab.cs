using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIPrefab
{
	public GameObject m_UICanvasPrefab;

	public GameObject m_EventSystemPrefab;

	public GameObject m_CharacterScreenPrefab;

	public GameObject m_BarterScreenPrefab;

	public GameObject m_MainMenuPrefab;

	public GameObject m_ScenarioMenuPrefab;

	public GameObject m_ScenarioDifficultyPrefab;

	public GameObject m_GamePlusPrefab;

	public GameObject m_LoadingScreenPrefab;

	public GameObject m_InGameMenuPrefab;

	public GameObject m_MissionEndPrefab;

	public GameObject m_CharacterFlagPrefab;

	public GameObject m_StrategicExploratorPrefab;

	public GameObject m_StrategicExploratorButtonsPrefab;

	public GameObject m_ObjectivePanelPrefab;

	public GameObject m_DialogPanelPrefab;

	public GameObject m_HUDPrefab;

	public GameObject m_HUDEnemyPanelPrefab;

	public GameObject m_HUDAbilitiesPrefab;

	public GameObject m_TermsOfShootingPrefab;

	public GameObject m_SubtitlesPrefab;

	public GameObject m_SplashPrefab;

	public GameObject m_TooltipPrefab;

	public GameObject m_SetupStagePanelPrefab;

	public GameObject m_CardsPanelPrefab;

	public GameObject m_TacticalDetailsPanelPrefab;

	public GameObject m_StrategicEventPopup;

	public GameObject m_GamepadStrategicPanel;

	public GameObject m_CassandraSP;

	public GameObject m_CustomS5;

	public GameObject m_CustomS1;

	public GameObject m_CustomS2;

	public GameObject m_CustomS3;

	public GameObject m_CustomS4;

	public GameObject m_CustomS6;

	public GameObject m_CustomS7;

	public GameObject m_CustomS9;

	public GameObject m_Options;

	public GameObject m_TutorialPopup;

	public GameObject m_TutorialPanel;

	public GameObject m_TutorialMarker;

	public GameObject m_TimeMarker;

	public GameObject m_CreditsPanelPrefab;

	public GameObject m_TutorialMiniPopup;

	public GameObject m_HUDSave;

	public GameObject m_HUDActions;

	public List<Sprite> m_CharacterIcons = new List<Sprite>();

	public List<Sprite> m_ItemsIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesHoverIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesClickedIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesDisabledIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesUsableIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesUsableHoverIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesUsableClickedIcons = new List<Sprite>();

	public List<Sprite> m_AbilitiesUsableDisabledIcons = new List<Sprite>();

	public List<Sprite> m_CardsIcons = new List<Sprite>();

	public List<Sprite> m_BuffsIcons = new List<Sprite>();

	public List<Sprite> m_PassiveAbIconsActive = new List<Sprite>();

	public List<Sprite> m_PassiveAbIconsUnactive = new List<Sprite>();

	public List<Sprite> m_ObjMainIcons = new List<Sprite>();

	public List<Sprite> m_ObjSecondIcons = new List<Sprite>();

	public List<Sprite> m_XboxIcons = new List<Sprite>();

	public List<Sprite> m_ShieldIcons = new List<Sprite>();

	public float m_NotificationDelay = 1f;

	public float m_StackDelay = 10f;

	public float m_StrategicExploratorAnimSpeed = 0.05f;
}
