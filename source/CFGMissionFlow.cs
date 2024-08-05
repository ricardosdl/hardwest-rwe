using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CFGMissionFlow : CFGUniversalMethods
{
	private bool m_DialogStarted;

	private bool m_ShowTutorialReady = true;

	private bool m_ShowTutorialPanelReady = true;

	[CFGFlowCode(Category = "Variables")]
	public void Global_SetInt(string Name, int Value)
	{
		CFGSingletonResourcePrefab<CFGStrategicManager>.Instance.Set_Int(Name, Value);
	}

	[CFGFlowCode(Category = "Variables")]
	public int Global_GetInt(string Name)
	{
		return CFGSingletonResourcePrefab<CFGStrategicManager>.Instance.Get_Int(Name);
	}

	[CFGFlowCode(Category = "Variables")]
	public void Global_SetString(string Name, string Value)
	{
		CFGSingletonResourcePrefab<CFGStrategicManager>.Instance.Set_String(Name, Value);
	}

	[CFGFlowCode(Category = "Variables")]
	public string Global_GetString(string Name)
	{
		return CFGSingletonResourcePrefab<CFGStrategicManager>.Instance.Get_String(Name);
	}

	[CFGFlowCode(Category = "Variables")]
	public void ApplyModifiedGlobals()
	{
		CFGPlayerProgress.ApplyCurrentCampaignGlobals();
		Debug.Log("Universals modified in current campaign has been applied to the starter table");
	}

	[CFGFlowCode(Category = "Economy")]
	public void TrinketsAnalyze()
	{
		CFGEconomy.UpdateFateTrader();
		CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
		if (sessionSingle != null)
		{
			CFGVariableContainer.Instance.SaveValuesGlobal(sessionSingle.CampaignName);
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public void TrinketModifyCount(string Variable, int NewCount)
	{
		CFGEconomy.ModifyTrinketCount(Variable, NewCount);
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_Invoke(string ShopID)
	{
		if (CFGEconomy.SelectShop(ShopID))
		{
			CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
			if ((bool)instance)
			{
				instance.LoadBarterScreen();
			}
		}
	}

	[Obsolete("NIE MA JUŻ CRAFTINGU! - zgromiłam bloczek - Aga")]
	[CFGFlowCode(Category = "Economy")]
	public void Shop_Crafting_Invoke(string ShopID)
	{
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_ResetPricesToDefault(string ShopID)
	{
		CFGDef_Shop value = null;
		CFGStore cFGStore = CFGEconomy.RegisterShop(ShopID);
		if (cFGStore == null)
		{
			Debug.LogWarning("Failed to find shop: " + ShopID);
			return;
		}
		if (!CFGStaticDataContainer.ShopList.TryGetValue(ShopID, out value))
		{
			Debug.LogWarning("Failed to find shop definition : " + ShopID);
			return;
		}
		foreach (KeyValuePair<string, CFGStoreGood> good in cFGStore.m_Goods)
		{
			CFGDef_Shop.CFGDef_ShopItem value2 = null;
			good.Value.BuyModifier = 1f;
			good.Value.SellModifier = 1f;
			if (value.ItemList.TryGetValue(good.Key, out value2))
			{
				good.Value.BaseBuyPrice = value2.BaseBuy;
				good.Value.BaseSellPrice = value2.BaseSell;
			}
			else
			{
				good.Value.BaseBuyPrice = good.Value.ItemDef.DefBuyVal;
				good.Value.BaseSellPrice = good.Value.ItemDef.DefSellVal;
			}
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_SetPricesAsPercentageOfDefault(string ShopID, string ItemID, int Buy_Percent, int Sell_Percent)
	{
		CFGStore cFGStore = CFGEconomy.RegisterShop(ShopID);
		if (cFGStore == null)
		{
			Debug.LogWarning("Failed to find shop: " + ShopID);
		}
		else
		{
			cFGStore.SetPricesAsPercentageOfDefault(ItemID, Buy_Percent, Sell_Percent);
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_SetPricesAsPercentOfDefByCategory(string ShopID, CFGDef_Item.EItemType ItemType, int Buy_Percent, int Sell_Percent)
	{
		CFGEconomy.Shop_SetPricesAsPercentOfDefByCategory(ShopID, ItemType, Buy_Percent, Sell_Percent);
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_ModifyPrice(string ShopID, string ItemID, float PriceBuy, float PriceSell)
	{
		CFGStore cFGStore = CFGEconomy.RegisterShop(ShopID);
		if (cFGStore == null)
		{
			Debug.LogWarning("Failed to find shop: " + ShopID);
			return;
		}
		CFGStoreGood value = null;
		if (!cFGStore.m_Goods.TryGetValue(ItemID, out value))
		{
			Debug.LogWarning("Shop: " + ShopID + " has no item " + ItemID);
			return;
		}
		float num = (float)value.BaseBuyPrice * PriceBuy;
		float num2 = (float)value.BaseSellPrice * PriceSell;
		value.BaseBuyPrice = Mathf.Clamp((int)num, 0, 999999);
		value.BaseSellPrice = Mathf.Clamp((int)num2, 0, 999999);
		value.BuyModifier = 1f;
		value.SellModifier = 1f;
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_ModifyPriceByCategory(string ShopID, CFGDef_Item.EItemType ItemType, float PriceBuy, float PriceSell)
	{
		CFGStore cFGStore = CFGEconomy.RegisterShop(ShopID);
		if (cFGStore == null)
		{
			Debug.LogWarning("Failed to find shop: " + ShopID);
			return;
		}
		foreach (KeyValuePair<string, CFGStoreGood> good in cFGStore.m_Goods)
		{
			CFGStoreGood value = good.Value;
			if (value != null && value.ItemID != null && value.ItemDef != null && value.ItemDef.ItemType == ItemType && !(value.ItemID == CFGInventory.StrCashItemID))
			{
				good.Value.BuyModifier = 1f;
				good.Value.SellModifier = 1f;
				float num = (float)value.BaseBuyPrice * PriceBuy;
				float num2 = (float)value.BaseSellPrice * PriceSell;
				value.BaseBuyPrice = Mathf.Clamp((int)num, 0, 999999);
				value.BaseSellPrice = Mathf.Clamp((int)num2, 0, 999999);
			}
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_SetPrice(string ShopID, string ItemID, int PriceBuy, int PriceSell)
	{
		CFGStore cFGStore = CFGEconomy.RegisterShop(ShopID);
		if (cFGStore == null)
		{
			Debug.LogWarning("Failed to find shop: " + ShopID);
			return;
		}
		CFGStoreGood value = null;
		if (!cFGStore.m_Goods.TryGetValue(ItemID, out value))
		{
			Debug.LogWarning("Shop: " + ShopID + " has no item " + ItemID);
			return;
		}
		value.BaseBuyPrice = Mathf.Clamp(PriceBuy, -1, 999999);
		value.BaseSellPrice = Mathf.Clamp(PriceSell, -1, 999999);
		value.SellModifier = 1f;
		value.BuyModifier = 1f;
	}

	[CFGFlowCode(Category = "Economy")]
	public void Shop_ModifyItemCount(string ShopID, string ItemID, int CountDelta)
	{
		CFGStore cFGStore = CFGEconomy.RegisterShop(ShopID);
		if (cFGStore == null)
		{
			Debug.LogWarning("Failed to find shop: " + ShopID);
		}
		else
		{
			cFGStore.ModifyItemCount(ItemID, CountDelta);
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public void ModifyBackpackByLoot(string LootID)
	{
		CFGStaticDataContainer.ApplyLootNode(LootID);
	}

	[CFGFlowCode(Category = "Economy")]
	public void ModifyBackpack(string ItemID, int DeltaCount)
	{
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(ItemID);
		if (itemDefinition == null)
		{
			Debug.LogWarning("ModifyInventory(): Failed to find item definition: " + ItemID);
		}
		else if (DeltaCount > 0)
		{
			CFGInventory.AddItem(ItemID, DeltaCount, SetAsNew: true, bUseCallBack: true, shownotification: true);
		}
		else if (DeltaCount < 0)
		{
			CFGInventory.RemoveItem(ItemID, -DeltaCount, SetAsNew: true, shownotification: true);
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public int GetItemCountInBackpack(string ItemID)
	{
		for (int i = 0; i < CFGInventory.BackpackItems.Count; i++)
		{
			if (string.Compare(ItemID, CFGInventory.BackpackItems[i].ItemID, ignoreCase: true) == 0)
			{
				return CFGInventory.BackpackItems[i].Count;
			}
		}
		return 0;
	}

	[CFGFlowCode(Category = "Economy")]
	public int GetItemCount(string ItemID)
	{
		int num = GetItemCountInBackpack(ItemID);
		for (int i = 0; i < 4; i++)
		{
			CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(i);
			if (teamCharacter != null)
			{
				if (string.Compare(ItemID, teamCharacter.Item1, ignoreCase: true) == 0)
				{
					num++;
				}
				if (string.Compare(ItemID, teamCharacter.Item2, ignoreCase: true) == 0)
				{
					num++;
				}
				if (string.Compare(ItemID, teamCharacter.Weapon1, ignoreCase: true) == 0)
				{
					num++;
				}
				if (string.Compare(ItemID, teamCharacter.Weapon2, ignoreCase: true) == 0)
				{
					num++;
				}
				if (string.Compare(ItemID, teamCharacter.Talisman, ignoreCase: true) == 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	[CFGFlowCode(Category = "Economy")]
	public void PlayerCash_Modify(int DeltaValue)
	{
		if (DeltaValue > 0)
		{
			CFGInventory.Cash_Add(DeltaValue, SetAsNew: true);
		}
		else if (DeltaValue < 0)
		{
			CFGInventory.Cash_Remove(-DeltaValue, SetAsNew: true);
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public void PlayerCash_Set(int NewValue)
	{
		CFGInventory.Cash_Set(NewValue);
	}

	[CFGFlowCode(Category = "Economy")]
	public int PlayerCash_Get()
	{
		return CFGInventory.Cash_Get();
	}

	[CFGFlowCode(Category = "Economy", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "False", "True" })]
	public bool PlayerCash_Pay(int Amount)
	{
		if (Amount < 1)
		{
			return true;
		}
		if (CFGInventory.Cash_Get() < Amount)
		{
			return false;
		}
		CFGInventory.Cash_Remove(Amount, SetAsNew: true);
		return true;
	}

	[CFGFlowCode(Category = "Economy")]
	public void ResetCards()
	{
		for (int i = 0; i < 4; i++)
		{
			CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(i);
			if (teamCharacter != null)
			{
				for (int j = 0; j < 5; j++)
				{
					CFGInventory.MoveCardFromCharacter(teamCharacter.Definition.NameID, j);
				}
			}
		}
	}

	[CFGFlowCode(Category = "Economy")]
	public void AddCardsToTheDeck(List<string> SpecificCards, int RandomCardCount)
	{
		int num = RandomCardCount;
		if (SpecificCards != null && SpecificCards.Count > 0)
		{
			foreach (string SpecificCard in SpecificCards)
			{
				if (CFGInventory.IsCardCollected(SpecificCard))
				{
					num++;
				}
				else
				{
					CFGInventory.CollectCard(SpecificCard);
				}
			}
		}
		CFGInventory.AddRandomCards(RandomCardCount);
	}

	[CFGFlowCode(Category = "Object")]
	public void ShowVisObject(CFGVisObject Target, bool Visible)
	{
		if (!(Target == null))
		{
			Target.ChangeVisiblity(Visible);
		}
	}

	[CFGFlowCode(Category = "Object")]
	public void ActivateUsable(CFGUsableObject Target, CFGCharacter Character)
	{
		if (!(Target == null))
		{
			if ((bool)Character)
			{
				Character.MakeAction(ETurnAction.Use, Target);
			}
			else
			{
				Target.DoUse(null);
			}
		}
	}

	[CFGFlowCode(Category = "Object")]
	public void EnableUsable(CFGUsableObject Target, bool Enabled)
	{
		if (!(Target == null))
		{
			Target.SetEnabled(Enabled);
		}
	}

	[CFGFlowCode(Category = "Object")]
	public void EnableUsableAndCollider(CFGUsableObject Target, bool Enabled)
	{
		if (!(Target == null))
		{
			Target.SetEnabledWithCollider(Enabled);
		}
	}

	[CFGFlowCode(Category = "Object")]
	public void LockDoor(CFGDoorObject Target)
	{
		if (!(Target == null))
		{
			Target.SetLocked(NewState: true);
		}
	}

	[CFGFlowCode(Category = "Object")]
	public void UnlockDoor(CFGDoorObject Target)
	{
		if (!(Target == null))
		{
			Target.SetLocked(NewState: false);
		}
	}

	[CFGFlowCode(Category = "Object")]
	public void OpenDoor(CFGDoorObject Target)
	{
		if (!(Target == null))
		{
			CFGCharacter user = null;
			Target.Open(user);
		}
	}

	[CFGFlowCode(Category = "Object", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "False", "True" })]
	public bool CanLockDoor(CFGDoorObject Target)
	{
		if (Target == null)
		{
			return false;
		}
		return Target.CanLock;
	}

	[CFGFlowCode(Category = "Object", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "False", "True" })]
	public bool CanOpenDoor(CFGDoorObject Target)
	{
		if (Target == null)
		{
			return false;
		}
		return Target.CanOpen;
	}

	[CFGFlowCode(Category = "Game")]
	public void LoadTacticalMap(string SceneName)
	{
		if (!CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			Debug.LogWarning("LoadTacticalMap() can be called only from strategic map");
		}
		else if (string.IsNullOrEmpty(SceneName))
		{
			Debug.LogWarning("LoadTacticalMap(): Scene name must be a valid string!");
		}
		else
		{
			m_TacticalToLoad = SceneName;
		}
	}

	[CFGFlowCode(Category = "Game")]
	public void MissionComplete()
	{
		CFGSingleton<CFGGame>.Instance.MissionComplete();
	}

	[CFGFlowCode(Category = "Game")]
	public void MissionFail(string reason_id)
	{
		CFGSingleton<CFGGame>.Instance.MissionFail(reason_id);
	}

	[CFGFlowCode(Category = "Game")]
	public void ScenarioComplete()
	{
		CFGSingleton<CFGGame>.Instance.ScenarioComplete();
	}

	[CFGFlowCode(Category = "Game")]
	public void CampaignComplete(int video_idx)
	{
		CFGSingleton<CFGGame>.Instance.CampaignComplete(video_idx);
	}

	[CFGFlowCode(Category = "Game")]
	public void SetNightmareEnabled(bool enabled)
	{
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null && componentInChildren.m_IsFading)
		{
			Debug.LogError("ERROR! SetNightmareEnabled called while camera fade is in progress. This is not supported!");
		}
		else
		{
			CFGGame.SetNightmareMode(enabled, onLevelStart: false);
		}
	}

	[CFGFlowCode(Category = "Game")]
	public void SetNightmareSilent(bool enabled)
	{
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null && componentInChildren.m_IsFading)
		{
			Debug.LogError("ERROR! SetNightmareEnabled called while camera fade is in progress. This is not supported!");
		}
		else
		{
			CFGGame.SetNightmareMode(enabled, onLevelStart: true);
		}
	}

	[Obsolete("Decyzja Czaka checkpointy zostaly usuniete z gry")]
	[CFGFlowCode(Category = "Game")]
	public void AutosaveCreate()
	{
		Debug.Log("AutosaveCreate");
		m_bCreateAutoSave = true;
	}

	[CFGFlowCode(Category = "Game")]
	public void AchievementUnlock(EAchievement achievement_id)
	{
		CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(achievement_id);
	}

	[CFGFlowCode(Category = "Audio")]
	public void PlayMusic(AudioClip audio_clip)
	{
		if (audio_clip != null)
		{
			CFGAudioManager.Instance.PlayBackgroundMusic(audio_clip);
		}
		else
		{
			CFGAudioManager.Instance.StopBackgroundMusic();
		}
	}

	[CFGFlowCode(Category = "Audio")]
	public void PlaySound3D(CFGSoundDef sound_def, Transform marker)
	{
		if (marker != null)
		{
			CFGSoundDef.Play(sound_def, marker.position);
		}
	}

	[CFGFlowCode(Category = "Audio")]
	public void PlaySound2D(AudioClip audio_clip)
	{
		if (audio_clip != null)
		{
			GameObject gameObject = new GameObject();
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.spatialBlend = 0f;
			audioSource.clip = audio_clip;
			audioSource.Play();
			UnityEngine.Object.Destroy(gameObject, audio_clip.length);
		}
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraFocus(CFGGameObject target)
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.ChangeFocus(target, force: true);
		}
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraFocusOnTransform(Transform target)
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.ChangeFocus(target.position, force: true);
		}
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraSetRotation(int Angle, float Time)
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (!(component == null))
		{
			if (Angle != 40 && Angle != 130 && Angle != 220 && Angle != 310)
			{
				Debug.LogWarning("Uncommon camera rotation value: " + Angle);
			}
			component.SetRotation(Angle, Time);
		}
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraRotateRight()
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.RotateRight(0.5f);
		}
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraRotateLeft()
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.RotateLeft(0.5f);
		}
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraFadeOut()
	{
		CameraFadeOutSpeed(1f);
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraFadeOutSpeed(float speed)
	{
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null)
		{
			componentInChildren.SetFade(CFGFadeToColor.FadeType.fadeOut, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FadingColor, speed, null);
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.EnableCameraControlLock(ELockReason.FadeOut, Enable: true);
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.SetLock(ELockReason.FadeOut, bLock: true);
		}
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraFadeIn()
	{
		CameraFadeInSpeed(1f);
	}

	[CFGFlowCode(Category = "Camera")]
	public void CameraFadeInSpeed(float speed)
	{
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null)
		{
			componentInChildren.SetFade(CFGFadeToColor.FadeType.fadeIn, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FadingColor, speed, OnCameraFadeEnd);
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.EnableCameraControlLock(ELockReason.FadeOut, Enable: true);
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.SetLock(ELockReason.FadeOut, bLock: true);
		}
	}

	private void OnCameraFadeEnd()
	{
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null)
		{
			componentInChildren.enabled = false;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.EnableCameraControlLock(ELockReason.FadeOut, Enable: false);
		}
		else
		{
			Debug.LogError("NO CAMERA!");
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.SetLock(ELockReason.FadeOut, bLock: false);
		}
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveStart(string objective_id)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.ObjectiveStart(objective_id);
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveComplete(string objective_id)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.ObjectiveComplete(objective_id);
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveFail(string objective_id)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.ObjectiveFail(objective_id);
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveCompleteSilent(string objective_id)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.ObjectiveComplete(objective_id, bSilent: true);
	}

	[CFGFlowCode(Category = "Objectives", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "Inactive", "Active" })]
	public bool ObjectiveIsActive(string objective_id)
	{
		return CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.IsObjectiveActive(objective_id);
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveSetSpecialText(string objective_id, string special_text)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.SetObjectiveSpecialText(objective_id, special_text);
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveSetLocations(string objective_id, List<Transform> locations)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.SetObjectiveLocations(objective_id, new List<Transform>(locations));
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveAddLocation(string objective_id, Transform transform)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.AddObjectiveLocation(objective_id, transform);
	}

	[CFGFlowCode(Category = "Objectives")]
	public void ObjectiveRemoveLocation(string objective_id, Transform transform)
	{
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.RemoveObjectiveLocation(objective_id, transform);
	}

	[CFGFlowCode(Category = "Dialogs")]
	public void PlayDialog(string dialog_id)
	{
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayDialog(dialog_id);
	}

	[CFGFlowCode(Category = "Dialogs", IsLatent = true)]
	public bool PlayDialogAndWait(string dialog_id)
	{
		if (!m_DialogStarted)
		{
			m_DialogStarted = true;
			PlayDialog(dialog_id);
		}
		if (CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.IsPlayingDialog() || CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.AreDialogsInQueue())
		{
			return false;
		}
		m_DialogStarted = false;
		return true;
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterMoveToTopSpot(string CharacterID)
	{
		CFGCharacterList.MoveCharToTeamTop(CharacterID);
	}

	[CFGFlowCode(Category = "Characters")]
	public void SetActivePosse(int PosseID)
	{
		if (CFGSingleton<CFGGame>.Instance == null || !CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			Debug.LogError("SetActivePosse() can be only used on strategic map!");
		}
		else
		{
			CFGCharacterList.SetActiveTeam(PosseID);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public int GetActivePosse()
	{
		return CFGCharacterList.CurrentTeamID;
	}

	[CFGFlowCode(Category = "Characters")]
	public void MarkCharacterForAchievement06(CFGCharacter Character)
	{
		if (!(Character == null) && Character.CharacterData != null)
		{
			Character.CharacterData.SetState(ECharacterStateFlag.MarkedForAchiev_06, Value: true);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void ReplaceCharacterID(string CurrentCharacterID, string NewCharacterID)
	{
		if (!CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			Debug.LogError("This function works only on strategic. Please kill J.R.");
			return;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CurrentCharacterID);
		if (characterData == null)
		{
			Debug.LogError("Failed to find current character !");
		}
		else
		{
			characterData.ReplaceDefinition(NewCharacterID);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void SaveCharState(string CharacterID, string VariableName, string ScopeName)
	{
		if (string.IsNullOrEmpty(CharacterID))
		{
			Debug.LogError("Character name is empty!");
			return;
		}
		if (string.IsNullOrEmpty(VariableName))
		{
			Debug.LogError("Invalid variable name!");
			return;
		}
		if (string.IsNullOrEmpty(ScopeName))
		{
			Debug.LogError("Invalid scope name!");
			return;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.SaveCharState() - Failed to find character: " + CharacterID);
			return;
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(VariableName, ScopeName);
		if (variable == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.SaveCharState() - Failed to find variable: " + VariableName + " within scope: " + ScopeName);
			return;
		}
		string scarsAndInjuries = characterData.GetScarsAndInjuries();
		variable.Value = scarsAndInjuries;
		CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
		string campaignID = null;
		if (sessionSingle != null)
		{
			string campaignName = sessionSingle.CampaignName;
			if (!string.IsNullOrEmpty(campaignName))
			{
				campaignID = campaignName;
			}
		}
		CFGVariableContainer.Instance.SaveValuesGlobal(campaignID);
	}

	[CFGFlowCode(Category = "Characters")]
	public void LoadCharState(string CharacterID, string VariableName, string ScopeName)
	{
		if (string.IsNullOrEmpty(CharacterID) || string.IsNullOrEmpty(VariableName) || string.IsNullOrEmpty(ScopeName))
		{
			return;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.LoadCharState() - Failed to find character: " + CharacterID);
			return;
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(VariableName, ScopeName);
		if (variable == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.LoadCharState() - Failed to find variable: " + VariableName + " within scope: " + ScopeName);
		}
		else
		{
			characterData.ApplyScars((string)variable.Value);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSetCardSlots(string CharacterID, int OpenSlots)
	{
		int num = Mathf.Clamp(OpenSlots, 0, 5);
		if (string.IsNullOrEmpty(CharacterID))
		{
			return;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSetCardSlots() - Failed to find character: " + CharacterID);
		}
		else
		{
			if (num == characterData.UnlockedCardSlots)
			{
				return;
			}
			if (num < characterData.UnlockedCardSlots)
			{
				int num2 = Mathf.Max(0, num - 1);
				for (int i = num2; i < 5; i++)
				{
					CFGDef_Card card = characterData.GetCard(i);
					if (card != null)
					{
						CFGInventory.MoveCardFromCharacter(CharacterID, i);
					}
				}
			}
			characterData.UnlockedCardSlots = num;
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void SetDecayLevel(string CharacterID, int DecayLevel)
	{
		if (!string.IsNullOrEmpty(CharacterID))
		{
			CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
			if (characterData == null)
			{
				Debug.LogWarning("WARNING! CFGMissionFlow.SetDecayLevel() - Failed to find character: " + CharacterID);
			}
			else
			{
				characterData.SetDecayLevel(DecayLevel);
			}
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public int GetDecayLevel(string CharacterID)
	{
		if (string.IsNullOrEmpty(CharacterID))
		{
			return -2;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.GetDecayLevel() - Failed to find character: " + CharacterID);
			return -2;
		}
		return characterData.DecayLevel;
	}

	[CFGFlowCode(Category = "Characters")]
	public void SetDecayImmunity(string CharacterID, bool SetImmune)
	{
		if (!string.IsNullOrEmpty(CharacterID))
		{
			CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
			if (characterData == null)
			{
				Debug.LogWarning("WARNING! CFGMissionFlow.SetDecayImmunity() - Failed to find character: " + CharacterID);
			}
			else
			{
				characterData.SetState(ECharacterStateFlag.ImmuneToDecay, SetImmune);
			}
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public bool GetDecayImmunity(string CharacterID)
	{
		if (string.IsNullOrEmpty(CharacterID))
		{
			return false;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.SetDecayImmunity() - Failed to find character: " + CharacterID);
			return false;
		}
		return characterData.IsStateSet(ECharacterStateFlag.ImmuneToDecay);
	}

	[CFGFlowCode(Category = "Characters")]
	public void HealBuffs(string CharacterID)
	{
		if (string.IsNullOrEmpty(CharacterID))
		{
			return;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.HealBuffs() - Failed to find character: " + CharacterID);
			return;
		}
		characterData.HealBuffs();
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && characterData.Definition != null && characterData.PositionInTeam >= 0)
		{
			string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("strategic_popup_injurieshealed", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(characterData.Definition.NameID));
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(localizedText, 1, characterData.Definition.ImageID);
		}
	}

	[CFGFlowCode(Category = "Characters", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "False", "True" })]
	public bool CharacterCheckIfHasHealableBuffs(string CharacterID)
	{
		if (string.IsNullOrEmpty(CharacterID))
		{
			return false;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterCheckIfHasHealableBuffs() - Failed to find character: " + CharacterID);
			return false;
		}
		return characterData.HasHealableBuffs();
	}

	[CFGFlowCode(Category = "Characters")]
	public bool CharacterHasHealableBuffs(string CharacterID)
	{
		if (string.IsNullOrEmpty(CharacterID))
		{
			return false;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterHasHealableBuffs() - Failed to find character: " + CharacterID);
			return false;
		}
		return characterData.HasHealableBuffs();
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSelectionChange(CFGCharacter Character, bool ChangeFocus)
	{
		if (Character == null)
		{
			return;
		}
		if (!CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			Debug.LogError("ERROR! CharacterSelectionChange can be only called on tactical");
			return;
		}
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.SelectCharacter(Character, ChangeFocus);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSelectionChangeByID(string CharacterID, bool ChangeFocus)
	{
		int num = CFGCharacterList.CharacterOrderInTeam(CharacterID);
		if (num < 0)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSelectionChangeByID() - Failed to find character: " + CharacterID);
			return;
		}
		CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(num);
		if (teamCharacter != null && !(teamCharacter.CurrentModel == null))
		{
			CharacterSelectionChange(teamCharacter.CurrentModel, ChangeFocus);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterChangeOwner(CFGCharacter Character, CFGOwner NewOwner)
	{
		if (!(Character == null) && !(NewOwner == null) && !(Character.Owner == NewOwner))
		{
			Character.ChangeOwner(NewOwner);
		}
	}

	[Obsolete("Nowy bloczek: CharacterHire")]
	[CFGFlowCode(Category = "Characters")]
	public void HireCharacter(int place, string character_id)
	{
		CFGCharacterList.RegisterNewCharacter(character_id, CanBeHired: true, TempTactical: false, null);
		CFGCharacterList.AssignToTeam(character_id);
	}

	[Obsolete("Nowy bloczek: CharacterFire")]
	[CFGFlowCode(Category = "Characters")]
	public void FireCharacter(int place)
	{
		CFGCharacterList.RemoveFromTeam(place, bRemoveEquipment: true);
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterHire(string character_id)
	{
		CFGCharacterList.RegisterNewCharacter(character_id, CanBeHired: true, TempTactical: false, null);
		CFGCharacterList.AssignToTeam(character_id);
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterFire(string character_id)
	{
		CFGCharacterList.RemoveFromTeam(character_id, bRemoveEquipment: true);
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterFireWithItemsAndCards(string character_id)
	{
		CFGCharacterList.RemoveFromTeam(character_id, bRemoveEquipment: false);
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterHireSilent(string CharacterID)
	{
		CFGCharacterList.RegisterNewCharacter(CharacterID, CanBeHired: true, TempTactical: false, null);
		CFGCharacterList.AssignToTeam(CharacterID, bHidePopup: true);
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterFireSilent(string CharacterID, bool ReturnStuffToParty)
	{
		CFGCharacterList.RemoveFromTeam(CharacterID, ReturnStuffToParty, bUsePopup: false);
	}

	[CFGFlowCode(Category = "Characters")]
	public List<string> GetHiredChars()
	{
		return CFGCharacterList.GetTeamMembersAsIDs();
	}

	[CFGFlowCode(Category = "Characters", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "False", "True" })]
	public bool IsCharacterHired(string character_id)
	{
		return CFGCharacterList.GetPositionInTeam(character_id) != -1;
	}

	private bool IsCodeOnlyAbility(ETurnAction ta, bool bShowWarning)
	{
		switch (ta)
		{
		case ETurnAction.Use_Item1:
		case ETurnAction.Use_Item2:
		case ETurnAction.Use_Talisman:
			if (bShowWarning)
			{
				Debug.LogWarning(string.Concat("Cannot add ability: ", ta, " from flow code!"));
			}
			return true;
		default:
			return false;
		}
	}

	[CFGFlowCode(Category = "Characters")]
	[Obsolete]
	public void CharacterAddAbility(string character_id, EAbility ability)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData != null)
		{
			ETurnAction eTurnAction = ETurnAction.None;
			switch (ability)
			{
			case EAbility.Disguise:
				eTurnAction = ETurnAction.Disguise;
				break;
			case EAbility.Gunpoint:
				eTurnAction = ETurnAction.Gunpoint;
				break;
			case EAbility.RicochetShoot:
				eTurnAction = ETurnAction.Ricochet;
				break;
			case EAbility.ShadowCloak:
				eTurnAction = ETurnAction.ShadowCloak;
				break;
			}
			if (!IsCodeOnlyAbility(eTurnAction, bShowWarning: true))
			{
				characterData.AddAbilityWithCard(eTurnAction, EAbilitySource.FlowCode);
			}
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterAddInjury(string CharacterID)
	{
		if (!string.IsNullOrEmpty(CharacterID))
		{
			CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
			if (characterData == null)
			{
				Debug.LogWarning("WARNING! CFGMissionFlow.CharacterAddInjury() - Failed to find character: " + CharacterID);
			}
			else
			{
				characterData.AddInjury(bCombatInjury: false);
			}
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterGiveAbility(string character_id, ETurnAction ability)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGiveAbility() - Failed to find character: " + character_id);
		}
		else if (!IsCodeOnlyAbility(ability, bShowWarning: true))
		{
			characterData.AddAbilityWithCard(ability, EAbilitySource.FlowCode);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterRemoveAbility(string character_id, ETurnAction ability)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterRemoveAbility() - Failed to find character: " + character_id);
		}
		else if (!IsCodeOnlyAbility(ability, bShowWarning: true))
		{
			characterData.RemoveAbility(ability, EAbilitySource.FlowCode);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public bool CharacterHasBuff(string character_id, string buff_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterHasBuff() - Failed to find character: " + character_id);
			return false;
		}
		return characterData.HasBuff(buff_id);
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterChangeBuffs(string character_id, List<string> buffs_to_add, List<string> buffs_to_remove)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterChangeBuffs() - Failed to find character: " + character_id);
		}
		else
		{
			INTERNAL_CharacterDataChangeBuffs(characterData, buffs_to_add, buffs_to_remove);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterObjectChangeBuffs(CFGCharacter Character, List<string> buffs_to_add, List<string> buffs_to_remove)
	{
		if (!(Character == null) && Character.CharacterData != null)
		{
			INTERNAL_CharacterDataChangeBuffs(Character.CharacterData, buffs_to_add, buffs_to_remove);
		}
	}

	private void INTERNAL_CharacterDataChangeBuffs(CFGCharacterData chd, List<string> buffs_to_add, List<string> buffs_to_remove)
	{
		if (chd == null)
		{
			return;
		}
		if (buffs_to_add != null)
		{
			for (int i = 0; i < buffs_to_add.Count; i++)
			{
				EBuffSource source = EBuffSource.Script;
				if (string.Compare(buffs_to_add[i], "critical_buff", ignoreCase: true) == 0)
				{
					source = EBuffSource.Permanent;
				}
				chd.AddBuff(buffs_to_add[i], source);
			}
		}
		if (buffs_to_remove != null)
		{
			for (int j = 0; j < buffs_to_remove.Count; j++)
			{
				chd.RemBuff(buffs_to_remove[j]);
			}
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public List<string> CharacterGetBuffs(string character_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGetBuffs() - Failed to find character: " + character_id);
			return null;
		}
		return new List<string>(characterData.Buffs.Keys);
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSetImprisoned(string character_id, bool enabled)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSetImprisoned() - Failed to find character: " + character_id);
		}
		else
		{
			characterData.SetState(ECharacterStateFlag.Imprisoned, enabled);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSetWeaponFirst(string character_id, string weapon_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSetWeaponFirst() - Failed to find character: " + character_id);
			return;
		}
		characterData.EquipItem(EItemSlot.Weapon1, weapon_id);
		if ((bool)characterData.CurrentModel)
		{
			characterData.CurrentModel.EquipWeapon();
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSetWeaponSecond(string character_id, string weapon_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSetWeaponSecond() - Failed to find character: " + character_id);
		}
		else
		{
			characterData.EquipItem(EItemSlot.Weapon2, weapon_id);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSetEquipmentFirst(string character_id, string equipment_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSetEquipmentFirst() - Failed to find character: " + character_id);
		}
		else
		{
			characterData.EquipItem(EItemSlot.Item1, equipment_id);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSetEquipmentSecond(string character_id, string equipment_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSetEquipmentSecond() - Failed to find character: " + character_id);
		}
		else
		{
			characterData.EquipItem(EItemSlot.Item2, equipment_id);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterSetTalisman(string character_id, string talisman_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterSetTalisman() - Failed to find character: " + character_id);
		}
		else
		{
			characterData.EquipItem(EItemSlot.Talisman, talisman_id);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public string CharacterGetTalisman(string character_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGetTalisman() - Failed to find character: " + character_id);
			return string.Empty;
		}
		return characterData.Talisman;
	}

	[CFGFlowCode(Category = "Characters")]
	public string CharacterGetWeaponFirst(string character_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGetWeaponFirst() - Failed to find character: " + character_id);
			return string.Empty;
		}
		return characterData.Weapon1;
	}

	[CFGFlowCode(Category = "Characters")]
	public string CharacterGetWeaponSecond(string character_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGetWeaponSecond() - Failed to find character: " + character_id);
			return string.Empty;
		}
		return characterData.Weapon2;
	}

	[CFGFlowCode(Category = "Characters")]
	public string CharacterGetEquipmentFirst(string character_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGetEquipmentFirst() - Failed to find character: " + character_id);
			return string.Empty;
		}
		return characterData.Item1;
	}

	[CFGFlowCode(Category = "Characters")]
	public string CharacterGetEquipmentSecond(string character_id)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(character_id);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGetEquipmentSecond() - Failed to find character: " + character_id);
			return string.Empty;
		}
		return characterData.Item2;
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterEquipWeapon(CFGCharacter character)
	{
		if ((bool)character)
		{
			character.EquipWeapon();
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void SetHp(CFGCharacter character, int hp)
	{
		if ((bool)character)
		{
			character.SetHP(hp, null, bSilent: false);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void CharacterChangeLuck(string CharacterID, int LuckMod)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterChangeLuck() - Failed to find character: " + CharacterID);
			return;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			float num = Mathf.Clamp(characterData.Luck + LuckMod, 0f, characterData.MaxLuck) - (float)characterData.Luck;
			string text = string.Format("{0} {1}: {2}", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(CharacterID), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("strategic_popup_luckchange"), num);
			if (num != 0f && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
			{
				CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text, 1, characterData.ImageIDX);
			}
		}
		characterData.SetLuck(characterData.Luck + LuckMod, bAllowSplash: false);
	}

	[CFGFlowCode(Category = "Characters")]
	public int CharacterGetLuck(string CharacterID)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.CharacterGetLuck() - Failed to find character: " + CharacterID);
			return 0;
		}
		return characterData.Luck;
	}

	[CFGFlowCode(Category = "Characters")]
	public void SilentKill(CFGCharacter character)
	{
		if ((bool)character)
		{
			character.Eradicate();
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void KillMultiple(List<CFGCharacter> CharacterList)
	{
		if (CharacterList == null || CharacterList.Count == 0)
		{
			return;
		}
		foreach (CFGCharacter Character in CharacterList)
		{
			if (!(Character == null))
			{
				Character.Eradicate();
			}
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void Resurrect(CFGCharacter character, int hp)
	{
		if ((bool)character)
		{
			if (hp < 1)
			{
				hp = 1;
			}
			character.IsAlive = true;
			character.SetHP(hp, null, bSilent: false);
			CFGCharacterList.AssignToTeam(character.NameId);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void ResurrectByName(string name, int hp)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(name);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.ResurrectByName() - Failed to find character: " + name);
			return;
		}
		if (hp < 1)
		{
			hp = 1;
		}
		characterData.IsDead = false;
		characterData.Hp = hp;
		CFGCharacterList.AssignToTeam(name);
	}

	[CFGFlowCode(Category = "Characters")]
	public void RestoreBaseStats(string name)
	{
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(name);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.RestoreBaseStats() - Failed to find character: " + name);
		}
		else
		{
			characterData.RestoreBaseStats();
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public CFGCharacter SpawnCharacter(string character_id, CFGOwner owner, Transform t)
	{
		CFGCharacter cFGCharacter = null;
		CFGCell characterCell = CFGCellMap.GetCharacterCell((!t) ? Vector3.zero : t.position);
		Vector3 pos = ((!characterCell) ? Vector3.zero : characterCell.WorldPosition);
		if (t == null)
		{
			Debug.LogError("CFGMissionFlow::SpawnCharacter: transform is NULL!");
		}
		Quaternion rot = ((!t) ? Quaternion.identity : t.rotation);
		cFGCharacter = CFGCharacterList.SpawnCharacter(character_id, owner, pos, rot, bSingleInstance: false);
		if ((bool)cFGCharacter)
		{
			if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner == owner)
			{
				cFGCharacter.StartTurn(owner);
			}
			if ((bool)owner && owner.IsPlayer && CFGSelectionManager.Instance.SelectedCharacter == null)
			{
				CFGSelectionManager.Instance.SelectCharacter(cFGCharacter, focus: true);
			}
		}
		return cFGCharacter;
	}

	[CFGFlowCode(Category = "Characters")]
	public List<CFGCharacter> SpawnCharactersInRandomArea(CFGRandomSpawnArea Area, List<string> CharacterIDList, CFGOwner Owner)
	{
		if (Area == null || CharacterIDList == null || CharacterIDList.Count == 0 || Owner == null)
		{
			Debug.LogError("Fail");
			return new List<CFGCharacter>();
		}
		return Area.SpawnCharacters(CharacterIDList, Owner);
	}

	[CFGFlowCode(Category = "Characters")]
	public CFGCharacter SpawnCharacterTemp(string character_id, CFGOwner owner, Transform t)
	{
		CFGCharacter cFGCharacter = SpawnCharacter(character_id, owner, t);
		if (cFGCharacter != null)
		{
			cFGCharacter.gameObject.AddComponent<CFGSpawnDemonFxOnStart>();
		}
		return cFGCharacter;
	}

	[CFGFlowCode(Category = "Characters")]
	public List<CFGCharacter> SpawnHiredChars(List<Transform> spawn_points)
	{
		List<CFGCharacter> list = new List<CFGCharacter>();
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner == null)
		{
			Debug.LogError("CFGMissionFlow.SpawnHiredChars() - no player owner found!");
			return list;
		}
		if (spawn_points.Count != 4)
		{
			Debug.LogError("CFGMissionFlow.SpawnHiredChars() - spawn_points list must contain 4 spawn points!");
			return list;
		}
		List<string> teamMembersAsIDs = CFGCharacterList.GetTeamMembersAsIDs();
		for (int i = 0; i < teamMembersAsIDs.Count; i++)
		{
			list.Add(SpawnCharacter(teamMembersAsIDs[i], playerOwner, spawn_points[i]));
		}
		return list;
	}

	[CFGFlowCode(Category = "Characters")]
	public void SetCharacterOwner(CFGCharacter character, CFGOwner owner)
	{
		if ((bool)character)
		{
			character.SetOwner(owner);
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public void SetCharacterInvulnerable(CFGCharacter character, bool invulnerable)
	{
		if ((bool)character && character.CharacterData != null)
		{
			character.CharacterData.Invulnerable = invulnerable;
		}
	}

	[CFGFlowCode(Category = "Characters")]
	public bool GetCharacterInvulnerable(CFGCharacter character)
	{
		if (character == null || character.CharacterData == null)
		{
			return false;
		}
		return character.CharacterData.Invulnerable;
	}

	[CFGFlowCode(Category = "Characters")]
	public void TranslateCharacter(CFGCharacter Character, Transform NewLoc)
	{
		if (!(Character == null) && !(NewLoc == null))
		{
			Character.Translate(NewLoc);
		}
	}

	[CFGFlowCode(Category = "Characters", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "False", "True" })]
	public bool IsCharacterAtLocation(string CharID, CFGCellObject Location)
	{
		if (string.IsNullOrEmpty(CharID) || Location == null)
		{
			return false;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharID);
		if (characterData == null)
		{
			Debug.LogWarning("WARNING! CFGMissionFlow.IsCharacterAtLocation() - Failed to find character: " + CharID);
			return false;
		}
		if (characterData.CurrentModel == null || characterData.CurrentModel.CurrentCell == null)
		{
			return false;
		}
		if (characterData.CurrentModel.CurrentCell.OwnerObject == Location && characterData.CurrentModel.CurrentCell.CheckFlag(0, 64))
		{
			return true;
		}
		return false;
	}

	[CFGFlowCode(Category = "Characters")]
	public List<CFGCharacter> GetOwnerCharactersInArea(CFGOwner owner, CFGCellObject location)
	{
		if (owner == null || location == null)
		{
			return new List<CFGCharacter>();
		}
		return owner.Characters.Where((CFGCharacter character) => character.CurrentCell.OwnerObject == location && character.CurrentCell.CheckFlag(0, 64)).ToList();
	}

	[CFGFlowCode(Category = "Characters")]
	public void ReloadPosseWeapons()
	{
		List<CFGCharacterData> teamCharactersList = CFGCharacterList.GetTeamCharactersList();
		foreach (CFGCharacterData item in teamCharactersList)
		{
			if (item.IsAlive)
			{
				if (item.CurrentModel.CurrentWeapon != null)
				{
					item.CurrentModel.FirstWeapon.CurrentAmmo = item.CurrentModel.FirstWeapon.AmmoCapacity;
				}
				if (item.CurrentModel.SecondWeapon != null)
				{
					item.CurrentModel.SecondWeapon.CurrentAmmo = item.CurrentModel.SecondWeapon.AmmoCapacity;
				}
			}
		}
	}

	[CFGFlowCode(Category = "AI", Title = "AI Team Set Preset")]
	public void AiTeamSetPreset(int ai_team, CFGAiPreset ai_preset)
	{
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.AiTeamSetPreset(ai_team, ai_preset);
		}
	}

	[CFGFlowCode(Category = "AI", Title = "AI Team Set Objective")]
	public void AiTeamSetObjective(int ai_team, Transform objective)
	{
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.AiTeamSetObjective(ai_team, objective);
		}
	}

	[CFGFlowCode(Category = "AI", Title = "AI Team Activate Roaming")]
	public void AiTeamActivateRoaming(int ai_team)
	{
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.AiTeamActivateRoaming(ai_team);
		}
	}

	[CFGFlowCode(Category = "AI", Title = "AI Team Set Priority Target")]
	public void AiTeamSetPriorityTarget(int ai_team, CFGCharacter target)
	{
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.AiTeamSetPriorityTarget(ai_team, target);
		}
	}

	[CFGFlowCode(Category = "AI", Title = "AI Team Get Characters")]
	public List<CFGCharacter> AiTeamGetCharacters(int ai_team)
	{
		List<CFGCharacter> list = new List<CFGCharacter>();
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			foreach (CFGCharacter character in aiOwner.Characters)
			{
				if (character.AiTeam == ai_team)
				{
					list.Add(character);
				}
			}
		}
		return list;
	}

	[CFGFlowCode(Category = "AI", Title = "AI Team Get Characters Count")]
	public int AiTeamGetCharactersCount(int ai_team)
	{
		int num = 0;
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			foreach (CFGCharacter character in aiOwner.Characters)
			{
				if (character.AiTeam == ai_team)
				{
					num++;
				}
			}
		}
		return num;
	}

	[CFGFlowCode(Category = "Owners")]
	public List<CFGCharacter> GetOwnerCharacters(CFGOwner owner)
	{
		return new List<CFGCharacter>((!owner) ? new HashSet<CFGCharacter>() : owner.Characters);
	}

	[CFGFlowCode(Category = "Locations")]
	public void SetObjectLocationState(CFGLocationObject location, ELocationState state)
	{
		if (location != null)
		{
			location.SetState(state);
			if (state == ELocationState.OPEN)
			{
				CFGLocationObject.m_LocationsForPopup.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(location.NameId));
			}
		}
	}

	[CFGFlowCode(Category = "Locations")]
	public ELocationState GetObjectLocationState(CFGLocationObject location)
	{
		return (!(location != null)) ? ELocationState.LOCKED : location.State;
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void PreLocationPanel()
	{
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_StrategicExplorator, bEnable: true);
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void HideLocationPanel()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance != null && instance.m_StrategicExplorator != null)
		{
			instance.m_StrategicExplorator.HideExplorationWindow();
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void FloatingText(Transform transform, string text_id)
	{
		CFGFloatingText.SpawnText(transform.position, CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id), Color.white);
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void StrategicEventPopup(string Variable, int Icon)
	{
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null)
		{
			string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(Variable);
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(localizedText, 2, Icon);
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void StrategicEventPopupTrinket(string item_id)
	{
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(item_id);
		if (itemDefinition == null)
		{
			Debug.LogWarning("StrategicEventPopupTrinket(): Failed to find item definition: " + item_id);
		}
		else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null)
		{
			string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(item_id + "_name");
			string localizedText2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_trinket_unlocked", localizedText);
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(localizedText2, 2, 0);
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void StrategicEventPopupBodyPart(string item_id)
	{
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(item_id);
		if (itemDefinition == null)
		{
			Debug.LogWarning("StrategicEventPopupBodyPart(): Failed to find item definition: " + item_id);
		}
		else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null)
		{
			string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(item_id + "_name");
			string localizedText2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s9_eventpopup_newcard", localizedText);
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(localizedText2, 3, itemDefinition.ShopIcon);
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void EnableTutorialHUDLimiter(EHudLimiterMode Mode)
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			switch (Mode)
			{
			case EHudLimiterMode.Nothing:
				component.AllowedMode = EPlayerHudLimiterMode.Nothing;
				break;
			case EHudLimiterMode.Attack:
				component.AllowedMode = EPlayerHudLimiterMode.SpecificTurnActionOnly;
				component.AllowedAction = ETurnAction.Shoot;
				break;
			case EHudLimiterMode.Fanning:
				component.AllowedMode = EPlayerHudLimiterMode.SpecificTurnActionOnly;
				component.AllowedAction = ETurnAction.AltFire_Fanning;
				break;
			case EHudLimiterMode.Reload:
				component.AllowedMode = EPlayerHudLimiterMode.SpecificTurnActionOnly;
				component.AllowedAction = ETurnAction.Reload;
				break;
			case EHudLimiterMode.Confirm:
				component.AllowedMode = EPlayerHudLimiterMode.Confirm;
				break;
			}
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void DisableCameraRotationAndEndTurn()
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.RotationDisabled = true;
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.EndTurnDisabled = true;
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void EnableCameraRotationAndEndTurn()
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.RotationDisabled = false;
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.EndTurnDisabled = false;
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void DisableTutorialHUDLimiter()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.AllowedMode = EPlayerHudLimiterMode.Default;
			component.AllowedAction = ETurnAction.None;
		}
	}

	[CFGFlowCode(Category = "GUI and HUD", IsLatent = true)]
	public bool ShowTutorialPopup(string title, string text, int img_nr)
	{
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && m_ShowTutorialReady)
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadTutorialPopup(title, text, img_nr, out m_ShowTutorialReady);
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && !m_ShowTutorialReady && CFGSingleton<CFGWindowMgr>.Instance.m_TutorialPopup == null)
		{
			m_ShowTutorialReady = true;
			return true;
		}
		return false;
	}

	[CFGFlowCode(Category = "GUI and HUD", IsLatent = true)]
	public bool ShowTutorialPanelLatent(string title)
	{
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && m_ShowTutorialPanelReady)
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadTutorialPanel(title, is_with_button: true, out m_ShowTutorialPanelReady);
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && !m_ShowTutorialPanelReady && CFGSingleton<CFGWindowMgr>.Instance.m_TutorialPanel == null)
		{
			m_ShowTutorialPanelReady = true;
			return true;
		}
		return false;
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void ShowTutorialPanel(string title)
	{
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadTutorialPanel(title, is_with_button: false, out var _);
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void HideTutorialPanel()
	{
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadTutorialPanel();
		}
		m_ShowTutorialPanelReady = true;
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void ShowTutorialMarker(ETutorialMarkerPlaces place)
	{
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadTutorialMarker(place);
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void HideTutorialMarker()
	{
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadTutorialMarker();
		}
	}

	[CFGFlowCode(Category = "Game", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "Easy", "Normal", "Hard" })]
	public int DifficultyLevel()
	{
		return CFGGame.Difficulty switch
		{
			EDifficulty.Easy => 0, 
			EDifficulty.Hard => 2, 
			_ => 1, 
		};
	}

	[CFGFlowCode(Category = "Tutorial")]
	public void ShowTutorialSceneMarker(Transform _transform)
	{
		CFGSingleton<CFGSceneMarker>.Instance.Show(_transform);
	}

	[CFGFlowCode(Category = "Tutorial")]
	public void HideTutorialSceneMarker()
	{
		CFGSingleton<CFGSceneMarker>.Instance.Hide();
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void SceneLimiter_TurnOff()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.PlayerActionLimiter_Reset();
			ClearInputActions();
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void SceneLimiter_DisableAllActions()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.PlayerActionLimiter_DisableAll();
			ClearInputActions();
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void SceneLimiter_AllowMoveToTileOnly(Transform TilePosition)
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.PlayerActionLimiter_SetToMove(TilePosition);
			ClearInputActions();
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void SceneLimiter_AllowEnemyClickOnly(CFGCharacter Enemy)
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.PlayerActionLimiter_EnemyCharacter(Enemy);
			ClearInputActions();
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void SceneLimiter_AllowUsableClickOnly(CFGUsableObject Usable)
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.PlayerActionLimiter_Usable(Usable);
			ClearInputActions();
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void SceneLimiter_AllowReloadClickOnly()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.PlayerActionLimiter_Reload();
			ClearInputActions();
		}
	}

	[CFGFlowCode(Category = "GUI and HUD")]
	public void SceneLimiter_AllowFanningClickOnly()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			component.PlayerActionLimiter_Fanning();
			ClearInputActions();
		}
	}

	private void ClearInputActions()
	{
		CFGInput.ClearActions(0.5f);
		CFGJoyManager.ClearJoyActions(0.5f);
	}

	[CFGFlowCode(Category = "FX")]
	public void Spawn_DLC1Fx(CFGSpawnableVFX Prefab, Transform Destination)
	{
		if (!(Prefab == null))
		{
			Transform component = UnityEngine.Object.Instantiate(Prefab).GetComponent<Transform>();
			if (component != null)
			{
				component.SetParent(Destination, worldPositionStays: true);
				component.position = Destination.position;
				component.rotation = Destination.rotation;
			}
		}
	}

	[CFGFlowCode(Category = "FX")]
	public void Stop_DLC1Fx(CFGSpawnableVFX VFXObject)
	{
		if (!(VFXObject == null))
		{
			VFXObject.StopVFX();
		}
	}

	[CFGFlowCode(Category = "Game", CodeType = FlowCodeType.CT_Condition, OutputNames = new string[] { "False", "True" })]
	public bool IsTacticalRestarted()
	{
		if (CFGSingleton<CFGGame>.Instance.SessionSingle == null)
		{
			return false;
		}
		return CFGSingleton<CFGGame>.Instance.SessionSingle.WasTacticalRestarted;
	}
}
