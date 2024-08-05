using UnityEngine;

public abstract class CFGSerializableObject : MonoBehaviour
{
	public const int FIRST_DYNAMIC_ID = 100000;

	public const int UNUSED_ID = 0;

	[HideInInspector]
	[SerializeField]
	protected int m_UniqueID;

	private static int m_FIRSTFREEID = 100000;

	public static int FirstFreeID
	{
		get
		{
			return m_FIRSTFREEID;
		}
		set
		{
			m_FIRSTFREEID = value;
			if (m_FIRSTFREEID < 100000)
			{
				Debug.LogWarning("First Free ID has been set to a value lower than allowed" + m_FIRSTFREEID);
				m_FIRSTFREEID = 100000;
			}
		}
	}

	public int UniqueID
	{
		get
		{
			return m_UniqueID;
		}
		set
		{
			m_UniqueID = value;
		}
	}

	public bool IsUniqueID_OK => m_UniqueID != 0;

	public bool IsDynamicObject => m_UniqueID >= 100000;

	public bool IsEditorObject => m_UniqueID < 100000;

	public virtual ESerializableType SerializableType => ESerializableType.NotSerializable;

	public virtual bool NeedsSaving => false;

	public static void ResetFreeID()
	{
		m_FIRSTFREEID = 100000;
	}

	public virtual bool OnSerialize(CFG_SG_Node ParentNode)
	{
		return false;
	}

	public virtual bool OnDeserialize(CFG_SG_Node Node)
	{
		return false;
	}

	public void AssignUUID(int uuid)
	{
		if (m_UniqueID != uuid || uuid == 0)
		{
			if (m_UniqueID != 0)
			{
				CFGSingletonResourcePrefab<CFGObjectManager>.Instance.UnRegisterSerializable(this);
			}
			m_UniqueID = uuid;
			if (m_UniqueID == 0)
			{
				m_UniqueID = m_FIRSTFREEID;
				m_FIRSTFREEID++;
			}
			if (m_UniqueID != 0)
			{
				CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RegisterSerializable(this);
			}
		}
	}

	public void ClearUUID()
	{
		m_UniqueID = 0;
	}

	protected CFG_SG_Node OnBeginSerialization(CFG_SG_Node ParentNode)
	{
		if (ParentNode == null || !NeedsSaving)
		{
			return null;
		}
		CFG_SG_Node cFG_SG_Node = ParentNode.AddSubNode("Object");
		if (cFG_SG_Node == null)
		{
			return null;
		}
		cFG_SG_Node.Attrib_Set("Type", SerializableType);
		cFG_SG_Node.Attrib_Set("UUID", m_UniqueID);
		return cFG_SG_Node;
	}

	protected virtual void OnEnable()
	{
		if (m_UniqueID != 0)
		{
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RegisterSerializable(this);
		}
	}

	protected virtual void OnDisable()
	{
	}
}
