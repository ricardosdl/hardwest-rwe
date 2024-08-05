using System;
using System.Collections.Generic;
using UnityEngine;

public class CFG_SG_SaveGame : IComparable
{
	public enum eSG_Type
	{
		Unknown,
		SinglePlayer
	}

	public enum eSG_Mode
	{
		Unknown,
		Strategic,
		Tactical
	}

	public enum eSG_Source
	{
		Unknown,
		StandardSave,
		AutoSave,
		QuickSave,
		AutoSaveOnLoad
	}

	public const int CurrentVersion = 1;

	public const string SID_SAVEGAMEID = "HardWestSaveGame";

	public const string SID_ND_SAVEGAMEINFO = "SaveGameInfo";

	public const string SID_ND_STRATEGIC = "Strategic";

	public const string SID_ND_TACTICAL = "Tactical";

	public const string SID_ND_ECONOMY = "Economy";

	public const string SID_ND_SHOPLIST = "ShopList";

	public const string SID_ND_SHOP = "Shop";

	public const string SID_ND_OBJECTIVELIST = "Objectives";

	public const string SID_ND_LOCATION = "Location";

	public const string SID_ND_VARIABLES = "Variables";

	public const string SID_ND_INVENTORY = "Inventory";

	public const string SID_ND_INVENTORY2 = "Inventory2";

	public const string SID_ND_CHARACTERS = "Characters";

	public const string SID_ND_CHARACTERDATA = "Char";

	public const string SID_ND_ABILITIES = "Abilities";

	public const string SID_ND_STATS = "Stats";

	public const string SID_ND_BUFFS = "Buffs";

	public const string SID_ND_BUFFDATA = "Buff";

	public const string SID_ND_GAMEFLOW = "GameFlow";

	public const string SID_ND_SEQUENCE = "Sequence";

	public const string SID_ND_FLOWOBJECTS = "FlowObjects";

	public const string SID_ND_EVENTQUEUE = "EventQueue";

	public const string SID_ND_AWAITEVENT = "AwaitEvent";

	public const string SID_ND_ACTIVEFLOW = "ActiveFlow";

	public const string SID_ND_GAMEOBJECTS = "GameObjects";

	public const string SID_ND_GAME_OBJECT = "Object";

	public const string SID_ND_CAMERA = "Camera";

	public const string SID_ND_CARDS = "Cards";

	public const string SID_ND_CARD = "Card";

	public const string SID_ND_SELMGR = "SelectionMgr";

	public const string SID_ND_AUDIO = "Audio";

	public const string SID_DESCRIPTION = "Desc";

	public const string SID_ACTIVE = "Active";

	public const string SID_ACTIVATECOUNT = "ActivateCount";

	public const string SID_VERSION = "Version";

	public const string SID_GAMETYPE = "GameType";

	public const string SID_GAMEMODE = "GameMode";

	public const string SID_GAMESOURCE = "GameSrc";

	public const string SID_NGAMEP = "NGameP";

	public const string SID_TYPE = "Type";

	public const string SID_UUID = "UUID";

	public const string SID_DLC = "Ext";

	public const string SID_ROTATION = "Rot";

	public const string SID_OWNERUUID = "Owner";

	public const string SID_TARGETUUID = "TargetUUID";

	public const string SID_SELECTUUID = "SelectionUUID";

	public const string SID_ITEMS = "Items";

	public const string SID_ITEM_INVENTORY = "Item";

	public const string SID_COUNT = "Count";

	public const string SID_NAME = "Name";

	public const string SID_TEMPORARY = "Temp";

	public const string SID_POSITION = "Pos";

	public const string SID_HIREABLE = "Hire";

	public const string SID_ABILITY = "Ability";

	public const string SID_TURN = "Turn";

	public const string SID_STARTEDTURN = "StartedTurn";

	public const string SID_APLEFT = "APLeft";

	public const string SID_TEAMPOS = "TeamPos";

	public const string SID_TEAMID = "TeamID";

	public const string SID_WEAPON1 = "Weapon1";

	public const string SID_WEAPON2 = "Weapon2";

	public const string SID_ITEM1 = "Item1";

	public const string SID_ITEM2 = "Item2";

	public const string SID_TALISMAN = "Talisman";

	public const string SID_DEAD = "Dead";

	public const string SID_INVUL = "Invul";

	public const string SID_INJURED = "Injured";

	public const string SID_HP = "HP";

	public const string SID_ID = "ID";

	public const string SID_LUCK = "Luck";

	public const string SID_CASH = "Cash";

	public const string SID_DELAY = "Delay";

	public const string SID_IMPRISONED = "Imprisoned";

	public const string SID_MAXHEALTH = "MaxHealth";

	public const string SID_AIM = "Aim";

	public const string SID_DEFENSE = "Def";

	public const string SID_DAMAGE = "Damage";

	public const string SID_MOVEMENT = "Movement";

	public const string SID_SIGHT = "Sight";

	public const string SID_SCOPE = "Scope";

	public const string SID_MAXLUCK = "MaxLuck";

	public const string SID_LUCKTURN = "LuckTurn";

	public const string SID_LUCKHIT = "LuckHit";

	public const string SID_MAIN = "Main";

	public const string SID_BACK = "Back";

	public const string SID_SOURCE = "Source";

	public const string SID_TIME = "Time";

	public const string SID_BUY = "Buy";

	public const string SID_SELL = "Sell";

	public const string SID_BUYMOD = "BuyMod";

	public const string SID_SELLMOD = "SellMod";

	public const string SID_SLOTS = "Slots";

	public const string SID_TEMPW1 = "TempW1";

	public const string SID_TEMPW2 = "TempW2";

	public const string SID_SCENE = "Scene";

	public const string SID_SCENARIO = "Scenario";

	public const string SID_CAMPAIGN = "Campaign";

	public const string SID_TACTICAL = "Tactical";

	public const string SID_CURRENT = "Current";

	public const string SID_DISABLED = "Disabled";

	public const string SID_LOCKED = "Locked";

	public const string SID_BLOCKED = "Blocked";

	public const string SID_OPENED = "Opened";

	public const string SID_NEW = "New";

	public const string SID_READED = "Readed";

	public const string SID_REMOVED = "Removed";

	public const string SID_ANGLEV = "AngleV";

	public const string SID_ANGLEH = "AngleH";

	public const string SID_USED = "Used";

	public const string SID_APPLIED = "Applied";

	public const string SID_STATE = "State";

	public const string SID_LOCATION_SHOW_SCENE_MARKER = "ShowNewMarker";

	public const string SID_PROGRESS = "Progress";

	public const string SID_START = "Start";

	public const string SID_END = "End";

	public const string SID_DIRTY = "Dirty";

	public const string SID_VALUE = "Value";

	public const string SID_INT = "Integer";

	public const string SID_STRING = "String";

	public const string SID_NIGHTMARE = "Nightmare";

	public const string SID_COOLDOWN = "Cooldown";

	public const string SID_TURNLEFT = "TurnLeft";

	public const string SID_SETUPSTAGE = "SetupStage";

	public const string SID_SUSPICIOUSLEVEL = "SuspiciousLevel";

	public const string SID_SUBDUEDCOUNT = "SubduedCnt";

	public const string SID_AISTATE = "AIState";

	public const string SID_DIFFICULTY = "Difficulty";

	public const string SID_DIFFICULTYLOWEST = "LowestDifficulty";

	public const string SID_DIFFICULTYCHANGED = "DifficultyChanged";

	public const string SID_INJURIES = "Injuries";

	public const string SID_PERMADEATH = "Permadeath";

	public const string SID_ARTERYDISTANCE = "ArteryDist";

	public const string SID_NEMESIS = "Nemesis";

	public const string SID_DEATHTIME = "Death";

	public const string SID_AITEAM = "AITeam";

	public const string SID_ND_MODEL = "Model";

	public const string SID_DECAYLEVEL = "Decay";

	public const string SID_GUNPOINTSTATE = "GunpointState";

	public const string SID_ACH_EQUALIZATION = "AEU";

	public const string SID_ACH_TOTALAP = "ATAP";

	public const string SID_ACH_TOTALSHOTS = "ATS";

	public const string SID_ACH_RUSTYSHOTS = "ARS";

	public const string SID_ACH_100SHOTS = "ASH";

	public const string SID_FLOATLIST = "FLTLST";

	public const string SID_INTLIST = "INTLST";

	public const string SID_STRINGLIST = "STRLST";

	public const string SID_SCENEVER = "SCENEVER";

	public const string SID_AITEAMSDATA = "AiTeamsData";

	public const string SID_AITEAMDATA = "AiTeamData";

	public const string SID_ROAMING = "Roaming";

	public const string SID_AIPRESET = "AiPreset";

	public const string SID_AI_MOVEMENTMODE = "MovementMode";

	public const string SID_AI_APRESERVEDFORMOVE = "APReservedForMove";

	public const string SID_AI_TARGETINGMODE = "TargetingMode";

	public const string SID_AI_OBJRADIUS = "ObjectiveRadius";

	public const string SID_AI_OBJINLOSREQ = "ObjectiveInLOSRequired";

	public const string SID_AI_DEFRATE = "DefenseRate";

	public const string SID_AI_OFFRATE = "OffenseRate";

	public const string SID_AI_PRESSRATE = "PressingRate";

	public const string SID_AI_MOBRATE = "MobilityRate";

	public const string SID_AI_HIDERATE = "HidingRate";

	public const string SID_AI_SCOUTRATE = "ScoutingRate";

	public const string SID_PLAYERACTIONLIMITER = "PlayerActionLimiter";

	public const string SID_ACTIONLIMITCHARACTER = "ActionLimitTargetCharacter";

	public const string SID_ACTIONLIMITUSABLE = "ActionLimitTargetUsable";

	public const string SID_ACTIONLIMITERCELL = "ActionLimitTargetCell";

	public const string SID_ALLOWEDMODE = "AllowedMode";

	public const string SID_ALLOWEDACTION = "AllowedAction";

	public int Version = 1;

	private int UUIDToSet = 100000;

	private EDifficulty m_Current = EDifficulty.Normal;

	private EDifficulty m_Lowest = EDifficulty.Normal;

	private bool m_bPermadeath;

	private bool m_bInjuries = true;

	private bool m_bDiffChanged;

	public float m_MissionTime;

	public int m_NewGamePlusFlags;

	public string SceneName = string.Empty;

	public string CampaignName = string.Empty;

	public string ScenarioName = string.Empty;

	public string TacticalID = string.Empty;

	public string Description = string.Empty;

	public DateTime tileLastSave = DateTime.MinValue;

	private string _FName = string.Empty;

	private bool _Loaded;

	private bool _IsContentValid;

	private eSG_Mode _Mode;

	private eSG_Source _Source;

	private eSG_Type _Type;

	private EDLC _DLC;

	private int _SceneVersion;

	public CFG_SG_Container ContentContainer = new CFG_SG_Container();

	public int FirstFreeUID => UUIDToSet;

	public string FileName
	{
		get
		{
			return _FName;
		}
		set
		{
			_FName = value;
		}
	}

	public bool IsLoaded => _Loaded;

	public bool IsContentValid => _IsContentValid;

	public eSG_Mode SaveGameMode => _Mode;

	public eSG_Source SaveGameSource => _Source;

	public eSG_Type SaveGameType => _Type;

	public int SceneVersion => _SceneVersion;

	public CFG_SG_SaveGame(string _FileName)
	{
		_FName = _FileName;
	}

	public void LoadData()
	{
		ContentContainer.Reset();
		if (!string.IsNullOrEmpty(_FName))
		{
			_IsContentValid = ContentContainer.LoadData(_FName);
			if (_IsContentValid)
			{
				_IsContentValid = LoadMainNode();
				_Loaded = true;
			}
		}
	}

	private bool LoadMainNode()
	{
		CFG_SG_Node mainNode = ContentContainer.MainNode;
		if (mainNode == null)
		{
			Debug.LogError("Failed to find main node");
			return false;
		}
		Version = mainNode.Attrib_Get("Version", 0);
		_Type = mainNode.Attrib_Get("GameType", eSG_Type.Unknown);
		if (_Type == eSG_Type.Unknown)
		{
			_IsContentValid = false;
			Debug.LogError("Unknown type");
		}
		Description = mainNode.Attrib_Get("Desc", string.Empty);
		_Mode = ContentContainer.MainNode.Attrib_Get("GameMode", eSG_Mode.Unknown);
		if (_Mode == eSG_Mode.Unknown)
		{
			Debug.LogError("Unknown mode");
			_IsContentValid = false;
		}
		_Source = ContentContainer.MainNode.Attrib_Get("GameSrc", eSG_Source.Unknown);
		if (_Source == eSG_Source.Unknown)
		{
			_Source = eSG_Source.StandardSave;
			Debug.LogWarning("Unknown Source");
		}
		_DLC = ContentContainer.MainNode.Attrib_Get("Ext", EDLC.None, bReport: false);
		SceneName = mainNode.Attrib_Get("Scene", string.Empty);
		if (string.IsNullOrEmpty(SceneName))
		{
			Debug.LogError("Missing Scene name");
			_IsContentValid = false;
		}
		TacticalID = mainNode.Attrib_Get("Tactical", string.Empty);
		ScenarioName = mainNode.Attrib_Get("Scenario", string.Empty);
		if (string.IsNullOrEmpty(SceneName))
		{
			Debug.LogError("Missing Scenario name");
			_IsContentValid = false;
		}
		CampaignName = mainNode.Attrib_Get("Campaign", string.Empty);
		if (string.IsNullOrEmpty(CampaignName))
		{
			_IsContentValid = false;
			Debug.LogError("Missing Campaign name");
		}
		UUIDToSet = mainNode.Attrib_Get("UUID", 100000);
		m_Current = mainNode.Attrib_Get("Difficulty", EDifficulty.Normal);
		m_Lowest = mainNode.Attrib_Get("LowestDifficulty", EDifficulty.Normal);
		m_bDiffChanged = mainNode.Attrib_Get("DifficultyChanged", DefVal: false);
		m_bInjuries = mainNode.Attrib_Get("Injuries", DefVal: true);
		m_bPermadeath = mainNode.Attrib_Get("Permadeath", DefVal: false);
		_SceneVersion = mainNode.Attrib_Get("SCENEVER", 0);
		_DLC = mainNode.Attrib_Get("Ext", EDLC.None, bReport: false);
		m_NewGamePlusFlags = mainNode.Attrib_Get("NGameP", 0, bReport: false);
		if (!CFGApplication.IsDLCInstalled(_DLC))
		{
			_IsContentValid = false;
			return false;
		}
		return true;
	}

	public void ApplyDifficulty()
	{
		LoadMainNode();
		CFGGame.OnDeserializeMain(m_bInjuries, m_bPermadeath, m_bDiffChanged, m_Current, m_Lowest, m_NewGamePlusFlags);
	}

	public void Reset()
	{
		Debug.Log("Reset SaveGame Content");
		ContentContainer.Reset();
		_IsContentValid = false;
		_Type = eSG_Type.Unknown;
		_Mode = eSG_Mode.Unknown;
		_Source = eSG_Source.Unknown;
		_DLC = EDLC.None;
		_SceneVersion = 0;
	}

	public void InitData(eSG_Type SaveGameType)
	{
		Reset();
		_Type = SaveGameType;
		if (ContentContainer.MainNode != null || ContentContainer.CreateContainer("HardWestSaveGame"))
		{
			_IsContentValid = true;
			_Loaded = false;
		}
	}

	public void UpdateData_ModeAndSource(eSG_Mode Mode, eSG_Source Source)
	{
		_Mode = Mode;
		_Source = Source;
		if (CreateHierarchy())
		{
		}
	}

	private void Restore_CharList(CFG_SG_Node Parent, bool bListOnly)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Characters");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find characters node!");
		}
		else if (bListOnly)
		{
			CFGCharacterList.OnDeserialize_ListOnly(cFG_SG_Node);
		}
		else
		{
			CFGCharacterList.OnDeserialize(cFG_SG_Node);
		}
	}

	private void Restore_CharList_UpdatePostionsOnly(CFG_SG_Node Parent)
	{
		Debug.Log("Updating positions of characters");
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Characters");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find characters node!");
		}
		else
		{
			CFGCharacterList.OnDeserialize_UpdatePositionsOnly(cFG_SG_Node);
		}
	}

	private void Restore_Objects(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("GameObjects");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find gameobjects node!");
			return;
		}
		HashSet<CFGSerializableObject> hashSet = new HashSet<CFGSerializableObject>();
		foreach (CFGSerializableObject value in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Serializables.Values)
		{
			if (value.SerializableType != 0 && value.NeedsSaving && !hashSet.Contains(value))
			{
				hashSet.Add(value);
			}
		}
		bool flag = false;
		for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Object", ignoreCase: true) != 0)
			{
				continue;
			}
			ESerializableType eSerializableType = subNode.Attrib_Get("Type", ESerializableType.NotSerializable);
			if (eSerializableType == ESerializableType.NotSerializable)
			{
				Debug.LogWarning("Serializable object with invalid type detected");
				continue;
			}
			int num = subNode.Attrib_Get("UUID", 0);
			if (num == 0)
			{
				Debug.LogWarning("Serializable object with null ID detected! " + eSerializableType);
				continue;
			}
			CFGSerializableObject cFGSerializableObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGSerializableObject>(num, eSerializableType);
			if (cFGSerializableObject == null)
			{
				Debug.LogWarning("There is no scene object with uuid: " + num + " and type: " + eSerializableType);
				continue;
			}
			switch (eSerializableType)
			{
			case ESerializableType.Door:
			case ESerializableType.Usable:
			case ESerializableType.Location:
				flag = true;
				break;
			}
			cFGSerializableObject.OnDeserialize(subNode);
			hashSet.Remove(cFGSerializableObject);
		}
		Debug.Log("Objects to destroy: " + hashSet.Count);
		foreach (CFGSerializableObject item in hashSet)
		{
			Debug.Log("Destroy: " + item.UniqueID);
			CFGGameObject cFGGameObject = item as CFGGameObject;
			if (!(cFGGameObject == null))
			{
				Debug.Log("Object: " + cFGGameObject.name);
				UnityEngine.Object.Destroy(cFGGameObject);
			}
		}
		if (flag)
		{
			CFGCellMap.CreateMap(CFGCellMap.FindSceneMainObject());
		}
	}

	private void ActivateMainSeq()
	{
	}

	private bool Restore_Camera(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Camera");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find camera node!");
			return false;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.OnDeserialize(cFG_SG_Node);
		}
		CFG_SG_Node cFG_SG_Node2 = cFG_SG_Node.FindSubNode("SelectionMgr");
		if (cFG_SG_Node2 == null)
		{
			Debug.LogError("Failed to find selection manager node!");
			return false;
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.OnDeserialize(cFG_SG_Node2);
		}
		return true;
	}

	private bool Restore_Audio(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Audio");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find camera node!");
			return false;
		}
		return CFGAudioManager.Instance.OnDeserialize(cFG_SG_Node);
	}

	private bool Restore_Inventory(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Inventory");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find inventory node!");
			return false;
		}
		if (!CFGInventory.OnDeserialize(cFG_SG_Node, 0))
		{
			return false;
		}
		cFG_SG_Node = Parent.FindSubNode("Inventory2");
		if (cFG_SG_Node == null)
		{
			return true;
		}
		return CFGInventory.OnDeserialize(cFG_SG_Node, 1);
	}

	private bool Restore_Economy(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Economy");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find economy node!");
			return false;
		}
		return CFGEconomy.OnDeserialize(cFG_SG_Node);
	}

	private bool Restore_Objectives(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Objectives");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find camera node!");
			return false;
		}
		return CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnDeserialize(cFG_SG_Node);
	}

	private bool Restore_Variables(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Variables");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find variables node!");
			return false;
		}
		return CFGVariableContainer.Instance.OnDeserialize(cFG_SG_Node);
	}

	private bool OnPostLoad_Seq()
	{
		bool result = false;
		CFGSequencer cFGSequencer = UnityEngine.Object.FindObjectOfType<CFGSequencer>();
		if ((bool)cFGSequencer)
		{
			result = cFGSequencer.OnPostLoad();
		}
		return result;
	}

	public bool OnReturnToStrategic_Sync()
	{
		Debug.Log("Scene Sync after tactical");
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode("Strategic");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find strategic node");
			return false;
		}
		m_MissionTime = 0f;
		if (cFG_SG_Node.SubNodeCount > 0)
		{
			m_MissionTime = cFG_SG_Node.Attrib_Get("Time", 0f);
			Restore_Objects(cFG_SG_Node);
			FlowSave.RestoreFlow(cFG_SG_Node);
			Restore_Audio(cFG_SG_Node);
			Restore_CharList_UpdatePostionsOnly(cFG_SG_Node);
			Restore_Camera(cFG_SG_Node);
			ApplyDifficulty();
		}
		else
		{
			Debug.Log("Strategic node is empty (probably because this is after first tactical from savegame)");
		}
		CFGTimer.MissionTime = m_MissionTime;
		return true;
	}

	public bool OnRestartTactical()
	{
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode("Strategic");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find strategic node");
			return false;
		}
		if (cFG_SG_Node.SubNodeCount > 0)
		{
			Restore_CharList(cFG_SG_Node, bListOnly: true);
			Restore_Objectives(cFG_SG_Node);
			ApplyDifficulty();
		}
		m_MissionTime = 0f;
		return true;
	}

	public bool OnReturnToStrategic_PostSync()
	{
		Debug.Log("Scene Post Sync after tactical");
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode("Strategic");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find strategic node");
			return false;
		}
		if (cFG_SG_Node.SubNodeCount > 0)
		{
			OnPostLoad_Seq();
			RestoreLocals(cFG_SG_Node);
		}
		CFGTimer.MissionTime = m_MissionTime;
		return true;
	}

	public bool OnRestoreStrategicDataOnly()
	{
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode("Strategic");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find node Strategic");
			return false;
		}
		if (cFG_SG_Node.SubNodeCount == 0)
		{
			return true;
		}
		Restore_Inventory(cFG_SG_Node);
		Restore_Economy(cFG_SG_Node);
		Restore_CharList(cFG_SG_Node, bListOnly: false);
		Restore_Objectives(cFG_SG_Node);
		ApplyDifficulty();
		return true;
	}

	public bool OnFullGameLoad(bool bStrategic)
	{
		string text = ((!bStrategic) ? "Tactical" : "Strategic");
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode(text);
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find node " + text);
			return false;
		}
		m_MissionTime = 0f;
		Restore_Inventory(cFG_SG_Node);
		Restore_Economy(cFG_SG_Node);
		Restore_CharList(cFG_SG_Node, bListOnly: false);
		Restore_Objects(cFG_SG_Node);
		Restore_Camera(cFG_SG_Node);
		Restore_Audio(cFG_SG_Node);
		Restore_Objectives(cFG_SG_Node);
		FlowSave.RestoreFlow(cFG_SG_Node);
		Restore_Variables(cFG_SG_Node);
		m_MissionTime = cFG_SG_Node.Attrib_Get("Time", 0f);
		if (!bStrategic)
		{
			uint turn = cFG_SG_Node.Attrib_Get("Turn", 0u);
			int startedTurn = cFG_SG_Node.Attrib_Get("StartedTurn", -1);
			bool enabled = cFG_SG_Node.Attrib_Get("Nightmare", DefVal: false, bReport: false);
			bool inSetupStage = cFG_SG_Node.Attrib_Get("SetupStage", DefVal: true);
			CFGGame.SetNightmareMode(enabled, onLevelStart: true);
			CFGSingletonResourcePrefab<CFGTurnManager>.Instance.OnDeserialize(turn, startedTurn);
			CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage = inSetupStage;
			CFGAchievmentTracker.OnDeserialize(cFG_SG_Node);
		}
		ApplyDifficulty();
		return true;
	}

	public bool OnFullGameLoad_PostSync(bool bStrategic)
	{
		Debug.Log("OnFullGameLoad_PostSync : Strategic ?" + bStrategic);
		string text = ((!bStrategic) ? "Tactical" : "Strategic");
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode(text);
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find node " + text);
			return false;
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if ((bool)character)
			{
				character.SetCurrentCell();
			}
		}
		OnPostLoad_Seq();
		RestoreLocals(cFG_SG_Node);
		CFGTimer.MissionTime = m_MissionTime;
		return true;
	}

	private bool RestoreLocals(CFG_SG_Node Parent)
	{
		CFGVariableContainer.Instance.GetScoped("local")?.ResetVariables();
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Variables");
		if (cFG_SG_Node == null)
		{
			Debug.LogWarning("Failed to find local variable node!");
			return false;
		}
		return CFGVariableContainer.Instance.OnDeserializeLocals(cFG_SG_Node);
	}

	public bool OnResetTactical_Sync()
	{
		if (ContentContainer.MainNode == null)
		{
			CFGInventory.Reset();
			CFGEconomy.Reset();
			CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnNewCampaign();
			CFGCharacterList.OnCampaignStart();
			return true;
		}
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode("Strategic");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find node Strategic");
			return false;
		}
		if (cFG_SG_Node.SubNodeCount > 0)
		{
			Restore_Inventory(cFG_SG_Node);
			Restore_Economy(cFG_SG_Node);
			Restore_CharList(cFG_SG_Node, bListOnly: true);
			Restore_Objectives(cFG_SG_Node);
		}
		return true;
	}

	public bool OnResetTactical_PostSync()
	{
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindSubNode("Strategic");
		if (cFG_SG_Node == null)
		{
			Debug.LogError("Failed to find node Strategic");
			return false;
		}
		if (cFG_SG_Node.SubNodeCount > 0)
		{
			Restore_Variables(cFG_SG_Node);
			Restore_Economy(cFG_SG_Node);
			Restore_Inventory(cFG_SG_Node);
			Restore_Variables(cFG_SG_Node);
		}
		CFGTimer.MissionTime = 0f;
		return true;
	}

	public void OnStrategic_StoreData()
	{
		if (CreateHierarchy())
		{
			CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindOrCreateSubNode("Strategic");
			if (cFG_SG_Node == null)
			{
				Debug.LogError("Serialization Failed. Could not create node for strategic data");
				return;
			}
			cFG_SG_Node.Attrib_Set("Time", CFGTimer.MissionTime);
			WriteInventory(cFG_SG_Node);
			WriteEconomy(cFG_SG_Node);
			WriteChars(cFG_SG_Node);
			WriteGameObjects(cFG_SG_Node);
			WriteCamera(cFG_SG_Node);
			WriteAudio(cFG_SG_Node);
			WriteObjectives(cFG_SG_Node);
			FlowSave.WriteGameFlow(cFG_SG_Node);
			WriteVariables(cFG_SG_Node);
		}
	}

	public void OnTactical_StoreData()
	{
		if (CreateHierarchy())
		{
			CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindOrCreateSubNode("Tactical");
			if (cFG_SG_Node == null)
			{
				Debug.LogError("Serialization Failed. Could not create node for tactical data");
				return;
			}
			cFG_SG_Node.Attrib_Set("Time", CFGTimer.MissionTime);
			cFG_SG_Node.Attrib_Set("Nightmare", CFGGame.Nightmare);
			cFG_SG_Node.Attrib_Set("Turn", CFGSingletonResourcePrefab<CFGTurnManager>.Instance.Turn);
			cFG_SG_Node.Attrib_Set("StartedTurn", CFGSingletonResourcePrefab<CFGTurnManager>.Instance.StartedTurn);
			cFG_SG_Node.Attrib_Set("SetupStage", CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage);
			CFGAchievmentTracker.OnSerialize(cFG_SG_Node);
			WriteInventory(cFG_SG_Node);
			WriteEconomy(cFG_SG_Node);
			WriteChars(cFG_SG_Node);
			WriteGameObjects(cFG_SG_Node);
			WriteCamera(cFG_SG_Node);
			WriteAudio(cFG_SG_Node);
			WriteObjectives(cFG_SG_Node);
			FlowSave.WriteGameFlow(cFG_SG_Node);
			WriteVariables(cFG_SG_Node);
		}
	}

	private void WriteGameObjects(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("GameObjects");
		if (cFG_SG_Node != null)
		{
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnSerialize(cFG_SG_Node);
		}
	}

	private void WriteCamera(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("Camera");
		if (cFG_SG_Node == null)
		{
			return;
		}
		CFG_SG_Node cFG_SG_Node2 = cFG_SG_Node.FindOrCreateSubNode("SelectionMgr");
		if (cFG_SG_Node2 != null)
		{
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if ((bool)component)
			{
				component.OnSerialize(cFG_SG_Node);
			}
			CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
			if ((bool)component2)
			{
				component2.OnSerialize(cFG_SG_Node2);
			}
		}
	}

	private void WriteAudio(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("Audio");
		if (cFG_SG_Node != null)
		{
			CFGAudioManager.Instance.OnSerialize(cFG_SG_Node);
		}
	}

	private void WriteChars(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("Characters");
		if (cFG_SG_Node != null)
		{
			CFGCharacterList.OnSerialize(cFG_SG_Node);
		}
	}

	private void WriteInventory(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("Inventory");
		if (cFG_SG_Node != null)
		{
			CFGInventory.OnSerialize(cFG_SG_Node, 0);
			cFG_SG_Node = Parent.FindOrCreateSubNode("Inventory2");
			if (cFG_SG_Node != null)
			{
				CFGInventory.OnSerialize(cFG_SG_Node, 1);
			}
		}
	}

	private void WriteEconomy(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("Economy");
		if (cFG_SG_Node != null)
		{
			CFGEconomy.OnSerialize(cFG_SG_Node);
		}
	}

	public void WriteHeaderContent()
	{
		if (!CreateHierarchy())
		{
			return;
		}
		CFG_SG_Node mainNode = ContentContainer.MainNode;
		if (mainNode == null)
		{
			return;
		}
		CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
		if (sessionSingle != null)
		{
			SceneName = sessionSingle.SceneName;
			CampaignName = sessionSingle.CampaignName;
			ScenarioName = sessionSingle.ScenarioName;
			if (sessionSingle.CurrentTactical != null)
			{
				TacticalID = sessionSingle.CurrentTactical.TacticalID;
			}
			else
			{
				TacticalID = string.Empty;
			}
			CFGVariableContainer.Instance.SaveValuesGlobal(sessionSingle.CampaignName);
		}
		_SceneVersion = 0;
		if (CFGSingleton<CFGGame>.Instance.LevelSettings != null)
		{
			_SceneVersion = CFGSingleton<CFGGame>.Instance.LevelSettings.UID_Version;
		}
		switch (CFGSingleton<CFGGame>.Instance.GetGameState())
		{
		case EGameState.InGame:
			_Mode = eSG_Mode.Tactical;
			break;
		case EGameState.Strategic:
			_Mode = eSG_Mode.Strategic;
			break;
		default:
			_Mode = eSG_Mode.Unknown;
			break;
		}
		_DLC = CFGGame.DlcType;
		Version = 1;
		mainNode.Attrib_Set("Version", Version);
		mainNode.Attrib_Set("GameType", _Type);
		mainNode.Attrib_Set("Desc", Description);
		mainNode.Attrib_Set("GameMode", _Mode);
		mainNode.Attrib_Set("GameSrc", _Source);
		mainNode.Attrib_Set("Scene", SceneName);
		mainNode.Attrib_Set("Ext", _DLC);
		mainNode.Attrib_Set("Tactical", TacticalID);
		mainNode.Attrib_Set("Scenario", ScenarioName);
		mainNode.Attrib_Set("Campaign", CampaignName);
		mainNode.Attrib_Set("UUID", CFGSerializableObject.FirstFreeID);
		mainNode.Attrib_Set("Difficulty", CFGGame.Difficulty);
		mainNode.Attrib_Set("LowestDifficulty", CFGGame.ScenarioLowestDifficulty);
		mainNode.Attrib_Set("DifficultyChanged", CFGGame.DifficultyChanged);
		mainNode.Attrib_Set("Injuries", CFGGame.InjuriesEnabled);
		mainNode.Attrib_Set("Permadeath", CFGGame.Permadeath);
		mainNode.Attrib_Set("SCENEVER", _SceneVersion);
		mainNode.Attrib_Set("NGameP", CFGGame.NewGamePlusFlags);
	}

	private bool WriteObjectives(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("Objectives");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		return CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnSerialize(cFG_SG_Node);
	}

	private bool WriteVariables(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindOrCreateSubNode("Variables");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		return CFGVariableContainer.Instance.OnSerialize(cFG_SG_Node);
	}

	private bool CreateHierarchy()
	{
		if (ContentContainer.MainNode == null && !ContentContainer.CreateContainer("HardWestSaveGame"))
		{
			return false;
		}
		CFG_SG_Node cFG_SG_Node = ContentContainer.MainNode.FindOrCreateSubNode("Strategic");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node = ContentContainer.MainNode.FindOrCreateSubNode("Tactical");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		return true;
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is CFG_SG_SaveGame cFG_SG_SaveGame))
		{
			return 1;
		}
		return tileLastSave.CompareTo(cFG_SG_SaveGame.tileLastSave);
	}
}
