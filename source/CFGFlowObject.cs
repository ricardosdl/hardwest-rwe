using UnityEngine;

public abstract class CFGFlowObject : ScriptableObject
{
	public enum EFOSType
	{
		Unknown,
		Sequence,
		Event,
		ANDGate,
		Delay,
		Gate,
		GenericNode,
		VarFloat,
		VarInt,
		VarBool,
		VarFloatList,
		VarIntList,
		VarObject,
		VarObjectList,
		VarString,
		VarStringList,
		VarTyped,
		VarVector,
		VarEnum
	}

	public delegate void OnChangedCallback();

	public CFGGUID m_GUID = new CFGGUID();

	[SerializeField]
	[HideInInspector]
	public CFGFlowSequence m_ParentSequence;

	[HideInInspector]
	public Vector2 m_Position;

	[HideInInspector]
	public string m_DisplayName;

	[HideInInspector]
	public bool m_Deprecated;

	[HideInInspector]
	public bool m_NeedAttention;

	[HideInInspector]
	public event OnChangedCallback OnChangedEvent;

	protected virtual void OnEnable()
	{
		if (m_ParentSequence == null && !m_GUID.IsClear() && !(this is CFGFlowSequence))
		{
			LogError("NULL PARENT SEQUENCE");
			return;
		}
		if (m_GUID.IsClear() ? true : false)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("GUID of flow object in game is clear -> please save scene before playing");
			}
			else
			{
				m_GUID.GenerateNew();
			}
		}
		m_Deprecated = GetType().IsObsolete();
	}

	public virtual void OnRemove()
	{
	}

	public virtual void Draw()
	{
	}

	public virtual bool IsOK()
	{
		return true;
	}

	public virtual void Check()
	{
		m_NeedAttention = !IsOK();
	}

	public virtual void UnLink(CFGFlowConn_Var connVar, bool bReversed = false)
	{
	}

	public virtual void OnLinked(CFGFlowConnector fromConnector, CFGFlowObject toObject, CFGFlowConnector toConnector)
	{
	}

	public virtual void OnUnLinked(CFGFlowConnector fromConnector, CFGFlowObject toObject, CFGFlowConnector toConnector)
	{
	}

	public virtual void OnReload()
	{
	}

	public virtual string GetDisplayName()
	{
		return m_DisplayName;
	}

	public void OnChanged()
	{
		if (this.OnChangedEvent != null)
		{
			this.OnChangedEvent();
		}
	}

	public virtual bool OnSerialize(CFG_SG_Node Parent)
	{
		return true;
	}

	public virtual EFOSType GetFOS_Type()
	{
		return EFOSType.Unknown;
	}

	protected CFG_SG_Node BaseSerialization(CFG_SG_Node Parent, string StrName = "Object")
	{
		if (m_GUID == null || m_GUID.IsClear())
		{
			Debug.LogError("GameFlow has uninitialized guids");
			return null;
		}
		CFG_SG_Node cFG_SG_Node = Parent.AddSubNode(StrName);
		if (cFG_SG_Node == null)
		{
			return null;
		}
		cFG_SG_Node.Attrib_Set("UUID", m_GUID);
		cFG_SG_Node.Attrib_Set("Type", GetFOS_Type());
		return cFG_SG_Node;
	}

	public virtual bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		return true;
	}

	public virtual bool OnPostLoad()
	{
		return true;
	}

	private string LogTextWithID(object message)
	{
		string text = ((message != null) ? message.ToString() : "Null");
		return m_GUID.ToString() + " " + text;
	}

	public void Log(object message)
	{
		Debug.Log(LogTextWithID(message), this);
	}

	public void LogWarning(object message)
	{
		Debug.LogWarning(LogTextWithID(message), this);
	}

	public void LogWarning(object message, Object context)
	{
		Debug.LogWarning(LogTextWithID(message), context);
	}

	public void LogError(object message)
	{
		Debug.LogError(LogTextWithID(message), this);
	}

	public void LogError(object message, Object context)
	{
		Debug.LogError(LogTextWithID(message), context);
	}
}
