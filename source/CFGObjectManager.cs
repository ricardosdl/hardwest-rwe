using System.Collections.Generic;
using UnityEngine;

public class CFGObjectManager : CFGSingletonResourcePrefab<CFGObjectManager>
{
	private HashSet<CFGGameObject> m_GameObjects = new HashSet<CFGGameObject>();

	private HashSet<CFGGameObject> m_GameObjectsToAdd = new HashSet<CFGGameObject>();

	private HashSet<CFGGameObject> m_GameObjectsToRem = new HashSet<CFGGameObject>();

	private HashSet<CFGCharacter> m_Characters = new HashSet<CFGCharacter>();

	private HashSet<CFGOwner> m_Owners = new HashSet<CFGOwner>();

	private HashSet<CFGRicochetObject> m_RicochetObjects = new HashSet<CFGRicochetObject>();

	private HashSet<CFGIAttackable> m_OtherAttackable = new HashSet<CFGIAttackable>();

	private CFGPlayerOwner m_PlayerOwner;

	private CFGAiOwner m_AiOwner;

	private Dictionary<int, CFGSerializableObject> m_Serializables = new Dictionary<int, CFGSerializableObject>();

	public Dictionary<int, CFGSerializableObject> Serializables => m_Serializables;

	public HashSet<CFGGameObject> GameObjects => m_GameObjects;

	public HashSet<CFGCharacter> Characters => m_Characters;

	public HashSet<CFGOwner> Owners => m_Owners;

	public HashSet<CFGRicochetObject> RicochetObjects => m_RicochetObjects;

	public HashSet<CFGIAttackable> OtherAttackableObjects => m_OtherAttackable;

	public CFGPlayerOwner PlayerOwner => m_PlayerOwner;

	public CFGAiOwner AiOwner => m_AiOwner;

	public void RegisterSerializable(CFGSerializableObject sobj)
	{
		if (sobj.UniqueID == 0)
		{
			Debug.LogWarning("Cannot register serializable with clear uuid!" + sobj.name);
		}
		else if (!m_Serializables.ContainsKey(sobj.UniqueID))
		{
			m_Serializables.Add(sobj.UniqueID, sobj);
		}
	}

	public void UnRegisterSerializable(CFGSerializableObject sobj)
	{
		if (sobj.UniqueID != 0 && m_Serializables.ContainsKey(sobj.UniqueID))
		{
			m_Serializables.Remove(sobj.UniqueID);
		}
	}

	public T FindSerializableGO<T>(int UUID, ESerializableType stype) where T : CFGSerializableObject
	{
		if (UUID == 0)
		{
			return (T)null;
		}
		CFGSerializableObject value = null;
		if (m_Serializables.TryGetValue(UUID, out value))
		{
			if (stype != 0 && value.SerializableType != stype)
			{
				return (T)null;
			}
			return value as T;
		}
		return (T)null;
	}

	public void RegisterPlayerOwner(CFGPlayerOwner player_owner)
	{
		m_PlayerOwner = player_owner;
		m_Owners.Add(m_PlayerOwner);
	}

	public void RegisterAiOwner(CFGAiOwner ai_owner)
	{
		m_AiOwner = ai_owner;
		m_Owners.Add(m_AiOwner);
	}

	public void RegisterImmediatly(CFGGameObject go)
	{
		AddToSet(go, GameObjects);
		AddToSet(go, Characters);
		AddToSet(go, Owners);
	}

	public void Register(CFGGameObject go)
	{
		m_GameObjectsToAdd.Add(go);
		if (!(go is CFGCharacter) && go is CFGIAttackable)
		{
			m_OtherAttackable.Add(go as CFGIAttackable);
		}
	}

	public void UnregisterImmediatly(CFGGameObject go)
	{
		RemFromSet(go, GameObjects);
		RemFromSet(go, Characters);
		RemFromSet(go, Owners);
	}

	public void Unregister(CFGGameObject go)
	{
		m_GameObjectsToRem.Add(go);
		if (!(go is CFGCharacter) && go is CFGIAttackable item && m_OtherAttackable.Contains(item))
		{
			m_OtherAttackable.Remove(item);
		}
	}

	public void OnTacticalStart()
	{
		foreach (CFGGameObject gameObject in GameObjects)
		{
			gameObject.OnTacticalStart();
		}
	}

	public void OnTacticalEnd()
	{
		foreach (CFGGameObject gameObject in GameObjects)
		{
			gameObject.OnTacticalEnd();
		}
		Clear();
	}

	public void OnStrategicStart()
	{
		foreach (CFGGameObject gameObject in GameObjects)
		{
			gameObject.OnStrategicStart();
		}
	}

	public void OnStrategicEnd()
	{
		foreach (CFGGameObject gameObject in GameObjects)
		{
			gameObject.OnStrategicEnd();
		}
		Clear();
	}

	public void Clear()
	{
		GameObjects.Clear();
		Characters.Clear();
		Owners.Clear();
		m_RicochetObjects.Clear();
		m_Serializables.Clear();
		m_OtherAttackable.Clear();
	}

	private static void AddToSet<T>(CFGGameObject obj, HashSet<T> hash_set) where T : CFGGameObject
	{
		T val = obj as T;
		if (val != null)
		{
			hash_set.Add(val);
		}
	}

	private static void RemFromSet<T>(CFGGameObject obj, HashSet<T> hash_set) where T : CFGGameObject
	{
		T val = obj as T;
		if (val != null)
		{
			hash_set.Remove(val);
		}
	}

	private void Update()
	{
		foreach (CFGGameObject item in m_GameObjectsToAdd)
		{
			AddToSet(item, GameObjects);
			AddToSet(item, Characters);
			AddToSet(item, Owners);
			AddToSet(item, m_RicochetObjects);
		}
		m_GameObjectsToAdd.Clear();
		foreach (CFGGameObject item2 in m_GameObjectsToRem)
		{
			RemFromSet(item2, GameObjects);
			RemFromSet(item2, Characters);
			RemFromSet(item2, Owners);
			RemFromSet(item2, m_RicochetObjects);
		}
		m_GameObjectsToRem.Clear();
		foreach (CFGGameObject gameObject in GameObjects)
		{
			if (gameObject != null)
			{
				gameObject.PrePreUpdateLogic();
			}
		}
		foreach (CFGGameObject gameObject2 in GameObjects)
		{
			if (gameObject2 != null)
			{
				gameObject2.PreUpdateLogic();
			}
		}
		CFGSelectionManager cFGSelectionManager = ((!CFGSelectionManager.IsInstanceInitialized()) ? null : CFGSelectionManager.Instance);
		if (cFGSelectionManager != null)
		{
			cFGSelectionManager.ClearCharacterVisibilityNeedUpdate();
		}
		if ((bool)PlayerOwner && PlayerOwner.Characters != null && PlayerOwner.Characters.Count > 0 && (bool)AiOwner && AiOwner.Characters != null && !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn && !CFGTimer.IsPaused_Gameplay)
		{
			float fEDRange = 5f;
			int eDChance = 30;
			if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData != null)
			{
				fEDRange = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_Eavesdrop_Range;
				eDChance = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_Eavesdrop_Chance;
			}
			HashSet<CFGCharacter> characters = AiOwner.Characters;
			foreach (CFGCharacter character in PlayerOwner.Characters)
			{
				if ((bool)character && character.IsAlive)
				{
					character.UpdateEavesdrop(characters, fEDRange, eDChance);
				}
			}
		}
		foreach (CFGGameObject gameObject3 in GameObjects)
		{
			if (gameObject3 != null)
			{
				gameObject3.UpdateLogic();
			}
		}
	}

	public void StartTurn(CFGOwner owner)
	{
		foreach (CFGGameObject gameObject in GameObjects)
		{
			gameObject.StartTurn(owner);
		}
	}

	public void EndTurn(CFGOwner owner)
	{
		foreach (CFGGameObject gameObject in GameObjects)
		{
			gameObject.EndTurn(owner);
		}
	}

	public bool IsVisibleByPlayer(CFGIAttackable iatt)
	{
		if (iatt == null || !iatt.IsAlive)
		{
			return false;
		}
		CFGCharacter cFGCharacter = iatt as CFGCharacter;
		if (cFGCharacter != null)
		{
			if (!cFGCharacter.IsAlive)
			{
				return false;
			}
			foreach (CFGCharacter character in PlayerOwner.Characters)
			{
				if (character.VisibleEnemies.Contains(cFGCharacter))
				{
					return true;
				}
			}
			return false;
		}
		foreach (CFGCharacter character2 in PlayerOwner.Characters)
		{
			if (character2.IsOtherTargetVisible(iatt))
			{
				return true;
			}
		}
		return false;
	}

	public static CFGGameObject Instantiate(CFGGameObject prefab)
	{
		return Instantiate(prefab, Vector3.zero, Quaternion.identity);
	}

	public static CFGGameObject Instantiate(CFGGameObject prefab, Vector3 position, Quaternion rotation)
	{
		CFGGameObject result = null;
		if ((bool)prefab)
		{
			result = Object.Instantiate(prefab, position, rotation) as CFGGameObject;
		}
		return result;
	}

	private int CompareBody(CFGCharacter ch1, CFGCharacter ch2)
	{
		if (ch1.IsCorpseLooted)
		{
			return -1;
		}
		if (ch2.IsCorpseLooted)
		{
			return 1;
		}
		return ch1.DeathTime.CompareTo(ch2.DeathTime);
	}

	public void DisintegrateNearBodies(CFGCharacter DyingCharacter)
	{
		if (!DyingCharacter)
		{
			return;
		}
		foreach (CFGCharacter character in m_Characters)
		{
			if (!character.IsAlive && !(character == DyingCharacter) && character.CanDisintegrateBody)
			{
				float num = Vector3.Distance(DyingCharacter.Position, character.Position);
				if (num < 0.5f)
				{
					character.DisintegrateBody();
				}
			}
		}
	}

	public void UpdateBodyCount()
	{
		int num = 1;
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo != null)
		{
			num = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo.m_MaxBodyCount;
		}
		if (num < 0)
		{
			return;
		}
		List<CFGCharacter> list = new List<CFGCharacter>();
		foreach (CFGCharacter character in m_Characters)
		{
			if (character.CanDisintegrateBody)
			{
				list.Add(character);
			}
		}
		list.Sort(CompareBody);
		if (list.Count > num)
		{
			for (int i = 0; i < num; i++)
			{
				list[i].DisintegrateBody();
			}
		}
	}

	public bool OnSerialize(CFG_SG_Node nd)
	{
		foreach (KeyValuePair<int, CFGSerializableObject> serializable in m_Serializables)
		{
			CFGSerializableObject value = serializable.Value;
			if (value == null || value.SerializableType == ESerializableType.NotSerializable || !value.NeedsSaving || value.OnSerialize(nd))
			{
				continue;
			}
			return false;
		}
		return true;
	}

	public bool OnDeserialize(CFG_SG_Node ObjectList)
	{
		for (int i = 0; i < ObjectList.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = ObjectList.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Object", ignoreCase: true) != 0)
			{
				continue;
			}
			int num = subNode.Attrib_Get("UUID", 0);
			if (num == 0)
			{
				Debug.LogWarning("SaveGame contains object with clear uuid. it's state cannot be restored");
				continue;
			}
			ESerializableType eSerializableType = subNode.Attrib_Get("Type", ESerializableType.NotSerializable);
			CFGSerializableObject cFGSerializableObject = FindSerializableGO<CFGSerializableObject>(num, eSerializableType);
			if (cFGSerializableObject == null)
			{
				Debug.LogWarning("Failed to find serializable object id: " + num + " type = " + eSerializableType);
			}
			else if (!cFGSerializableObject.OnDeserialize(subNode))
			{
				return false;
			}
		}
		return true;
	}

	public static void ProcessFacingToCoverAllCharacters()
	{
		HashSet<CFGCharacter> characters = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters;
		if (characters == null || characters.Count == 0)
		{
			return;
		}
		foreach (CFGCharacter item in characters)
		{
			if (!(item == null) && !item.IsDead && item.CurrentAction == ETurnAction.None)
			{
				item.ProcessFacingToCover();
			}
		}
	}
}
