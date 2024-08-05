using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public static class CFGCheats
{
	private static bool m_GlobalInvulnerability;

	private static bool m_InfiniteLuck;

	private static bool m_InfiniteAP;

	private static bool m_InfiniteAmmo;

	private static bool m_AllCharactersVisible;

	private static bool m_UseOutline = true;

	private static bool m_DisableCameraFocuses;

	public static bool GlobalInvulnerability
	{
		get
		{
			return m_GlobalInvulnerability && AreCheatsAllowed;
		}
		set
		{
			m_GlobalInvulnerability = value;
		}
	}

	public static bool InfiniteLuck
	{
		get
		{
			return m_InfiniteLuck && AreCheatsAllowed;
		}
		set
		{
			m_InfiniteLuck = value;
		}
	}

	public static bool InfiniteAP
	{
		get
		{
			return m_InfiniteAP && AreCheatsAllowed;
		}
		set
		{
			m_InfiniteAP = value;
		}
	}

	public static bool InfiniteAmmo
	{
		get
		{
			return m_InfiniteAmmo && AreCheatsAllowed;
		}
		set
		{
			m_InfiniteAmmo = value;
		}
	}

	public static bool AllCharactersVisible
	{
		get
		{
			return m_AllCharactersVisible && AreCheatsAllowed;
		}
		set
		{
			m_AllCharactersVisible = value;
		}
	}

	public static bool UseOutline
	{
		get
		{
			return m_UseOutline;
		}
		set
		{
			m_UseOutline = value;
		}
	}

	public static bool DisableCameraFocuses
	{
		get
		{
			return m_DisableCameraFocuses && AreCheatsAllowed;
		}
		set
		{
			m_DisableCameraFocuses = value;
		}
	}

	public static bool AreCheatsAllowed => CFGOptions.DevOptions.AllowCheats;

	public static void Check_Cheats(CFGScenarioMenuNew scenario_menu)
	{
		if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.U))
		{
			scenario_menu.UnlockAllScenarios();
		}
	}

	[Conditional("USE_CHEATS")]
	public static void Check_Cheats(CFGSelectionManager mgr)
	{
		if (!AreCheatsAllowed)
		{
			return;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			if (CFGOptions.DevOptions.AllowTimeScale && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				float num = -1f;
				if (Input.GetKeyDown(KeyCode.Keypad1))
				{
					num = 1f;
				}
				if (Input.GetKeyDown(KeyCode.Keypad2))
				{
					num = 2f;
				}
				if (Input.GetKeyDown(KeyCode.Keypad3))
				{
					num = 3f;
				}
				if (Input.GetKeyDown(KeyCode.Keypad4))
				{
					num = 4f;
				}
				if (num > 0f)
				{
					UnityEngine.Debug.Log("Time scale set to: " + num);
					Time.timeScale = num;
				}
				return;
			}
			CFGCharacter cFGCharacter = mgr.ObjectUnderCursor as CFGCharacter;
			if (CFGInput.LastReadInputDevice == EInputMode.Gamepad && (bool)mgr.CellUnderCursor)
			{
				cFGCharacter = mgr.CellUnderCursor.CurrentCharacter;
			}
			if (cFGCharacter == null)
			{
				cFGCharacter = mgr.SelectedCharacter;
			}
			if (cFGCharacter != null)
			{
				if (CFGInput.IsActivated(EActionCommand.Dev_SelfDamage))
				{
					UnityEngine.Debug.Log("Cheat: Damage " + cFGCharacter);
					cFGCharacter.TakeDamage(1, null, bSilent: false);
				}
				if (CFGInput.IsActivated(EActionCommand.Dev_SelfDamage_Large))
				{
					UnityEngine.Debug.Log("Cheat: Large Damage");
					cFGCharacter.TakeDamage(9999, null, bSilent: false);
				}
				if (CFGInput.IsActivated(EActionCommand.Dev_SelfHeal))
				{
					UnityEngine.Debug.Log("Cheat: Heal");
					cFGCharacter.Heal(1, bSilent: true);
				}
				if (CFGInput.IsActivated(EActionCommand.Dev_SelfHeal_Large))
				{
					UnityEngine.Debug.Log("Cheat: Large Heal");
					cFGCharacter.Heal(9999, bSilent: true);
				}
				if (CFGInput.IsActivated(EActionCommand.Dev_RegenAP))
				{
					UnityEngine.Debug.Log("Cheat: AP++");
					if (cFGCharacter.ActionPoints < cFGCharacter.MaxActionPoints)
					{
						cFGCharacter.ActionPoints++;
					}
					cFGCharacter.CharacterData.SetLuck(cFGCharacter.CharacterData.Luck + 50, bAllowSplash: true);
				}
				if (CFGInput.IsActivated(EActionCommand.Dev_Invulnerable) && cFGCharacter.CharacterData != null)
				{
					cFGCharacter.CharacterData.Invulnerable = !cFGCharacter.CharacterData.Invulnerable;
					UnityEngine.Debug.Log(string.Concat("Cheat: Invulnerable on ", cFGCharacter, " set to ", cFGCharacter.CharacterData.Invulnerable));
				}
			}
			if (Input.GetKeyDown(KeyCode.Keypad8))
			{
				CFGCell cellUnderCursor = mgr.CellUnderCursor;
				if (cellUnderCursor == null)
				{
					return;
				}
				Vector3 worldPosition = cellUnderCursor.WorldPosition;
				CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
				if (aiOwner == null)
				{
					return;
				}
				List<CFGCharacter> list = new List<CFGCharacter>(aiOwner.Characters);
				foreach (CFGCharacter item in list)
				{
					if (!(item == null) && item.CurrentCell != null)
					{
						float num2 = Vector3.Distance(item.CurrentCell.WorldPosition, worldPosition);
						if (!(num2 > 10f))
						{
							item.TakeDamage(1000, null, bSilent: false);
						}
					}
				}
			}
			if (CFGInput.IsActivated(EActionCommand.Dev_MassHeal))
			{
				CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
				if (playerOwner != null)
				{
					UnityEngine.Debug.Log("Cheat: Mass heal");
					foreach (CFGCharacter character in playerOwner.Characters)
					{
						if (character != null)
						{
							character.Heal(9999, bSilent: true);
						}
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.Pause))
			{
				CFGCharacter cFGCharacter2 = mgr.TargetedObject as CFGCharacter;
				if (cFGCharacter2 == null)
				{
					cFGCharacter2 = mgr.SelectedCharacter;
				}
				if (cFGCharacter2 != null && cFGCharacter2.CharacterData != null)
				{
					if (cFGCharacter2.Imprisoned)
					{
						UnityEngine.Debug.Log("Character: " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cFGCharacter2.CharacterData.Definition.NameID) + " is no longer imprisoned");
					}
					cFGCharacter2.Imprisoned = false;
				}
			}
		}
		if (CFGInput.IsActivated(EActionCommand.Dev_TeleportSelectedChar))
		{
			CFGCharacter cFGCharacter3 = mgr.TargetedObject as CFGCharacter;
			if (cFGCharacter3 == null)
			{
				cFGCharacter3 = mgr.SelectedCharacter;
			}
			if (cFGCharacter3 != null)
			{
				CFGCell cellUnderCursor2 = mgr.CellUnderCursor;
				if (cellUnderCursor2 != null && cellUnderCursor2.CanStandOnThisTile(can_stand_now: true))
				{
					cFGCharacter3.Translate(cellUnderCursor2.WorldPosition, cFGCharacter3.Transform.rotation);
				}
			}
		}
		if (CFGInput.IsActivated(EActionCommand.Dev_AddItems))
		{
			UnityEngine.Debug.Log("Cheat: Cash + 1000");
			CFGInventory.Cash_Add(1000, SetAsNew: true);
			CFGInventory.AddItem("opium", 10, SetAsNew: true);
			CFGInventory.AddItem("skin_vest", 10, SetAsNew: true);
			CFGInventory.AddItem("eagle_elixir", 10, SetAsNew: true);
			CFGInventory.AddItem("bear_figurine", 10, SetAsNew: true);
			CFGInventory.AddItem("pistol_derringer", 10, SetAsNew: true);
		}
		if (CFGInput.IsActivated(EActionCommand.Dev_Win))
		{
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				UnityEngine.Debug.Log("Cheat: Tactical Win");
				CFGSingleton<CFGGame>.Instance.MissionComplete();
				for (int i = 0; i < 4; i++)
				{
					CFGCharacterList.GetTeamCharacter(i)?.SetState(ECharacterStateFlag.Imprisoned, Value: false);
				}
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				UnityEngine.Debug.Log("Cheat: Strategic Win");
				CFGSingleton<CFGGame>.Instance.ScenarioComplete();
			}
		}
		if (CFGInput.IsActivated(EActionCommand.Dev_Lose))
		{
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				UnityEngine.Debug.Log("Cheat: Tactical Lose");
				CFGSingleton<CFGGame>.Instance.MissionFail("Failed (debug)....");
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				UnityEngine.Debug.Log("Cheat: Strategic Lose");
			}
		}
		if (Input.GetKeyDown(KeyCode.F7) && (CFGSingleton<CFGGame>.Instance.IsInGame() || CFGSingleton<CFGGame>.Instance.IsInStrategic()))
		{
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if ((bool)component)
			{
				component.SetFreeCamMode(!component.IsInFreeCamMode());
			}
		}
	}

	[Conditional("USE_CHEATS")]
	public static void Check_Cheats(CFGGame game)
	{
		if ((!CFGSingleton<CFGGame>.Instance.IsInGame() && !CFGSingleton<CFGGame>.Instance.IsInStrategic()) || !AreCheatsAllowed)
		{
			return;
		}
		if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))
		{
			if (Input.GetKeyDown(KeyCode.End))
			{
				CFGCamera component = Camera.main.GetComponent<CFGCamera>();
				if ((bool)component)
				{
					component.SetEnabled(!component.IsEnabled());
				}
			}
			if (Input.GetKeyDown(KeyCode.F10))
			{
				UnityEngine.Debug.Log("QuickSave!");
				if (!game.CanSaveGame)
				{
					UnityEngine.Debug.LogWarning("Cannot save game right now. Please try later");
				}
				game.CreateSaveGame(CFG_SG_SaveGame.eSG_Source.QuickSave);
			}
			if (Input.GetKeyDown(KeyCode.F11))
			{
				UnityEngine.Debug.Log("QuickLoad!");
				if (!game.CanSaveGame)
				{
					UnityEngine.Debug.LogWarning("Cannot load game right now. Please try later");
				}
				CFG_SG_Manager.LoadLastQuickSave();
			}
			if (Input.GetKeyDown(KeyCode.F12) && (game.IsInGame() || game.IsInStrategic()))
			{
				CFGStaticDataContainer.ApplyLootNode("loot_s0_test");
			}
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			if (Input.GetKeyUp(KeyCode.F5))
			{
				UnityEngine.Debug.Log("Set Nightmare Mode OFF");
				CFGGame.SetNightmareMode(enabled: false, Input.GetKey(KeyCode.LeftShift));
			}
			if (Input.GetKeyUp(KeyCode.F6))
			{
				UnityEngine.Debug.Log("Set Nightmare Mode ON");
				CFGGame.SetNightmareMode(enabled: true, Input.GetKey(KeyCode.LeftShift));
			}
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void ClearUnityConsoleWindow()
	{
		Type type = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
		MethodInfo method = type.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
		method.Invoke(null, null);
	}

	[Conditional("USE_CHEATS")]
	public static void OnAppInit()
	{
		float timeScale = (CFGOptions.DevOptions.TimeScale = Mathf.Clamp(CFGOptions.DevOptions.TimeScale, 0.5f, 8f));
		if (CFGOptions.DevOptions.AllowTimeScale)
		{
			Time.timeScale = timeScale;
		}
	}
}
