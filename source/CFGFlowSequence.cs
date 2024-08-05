#define USE_ERROR_REPORTING
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CFGFlowSequence : CFGFlowNode
{
	public struct EventInQueue
	{
		public float m_RetriggerDelay;

		public CFGFlowEventBase m_Event;
	}

	public delegate void FlowProgressEvent(CFGFlowNode node);

	public const int MAX_FLOW_STEPS = 1000;

	[HideInInspector]
	[Instanced]
	public List<CFGFlowObject> m_FlowObjects;

	[HideInInspector]
	public List<CFGFlowNode> m_ActiveFlow;

	[HideInInspector]
	public List<CFGFlowEventBase> m_RegisteredEvents;

	[SerializeField]
	private UnityEngine.Object _gameContext;

	private bool _stopExecution;

	[HideInInspector]
	public Vector2 m_Origin;

	[HideInInspector]
	public List<EventInQueue> m_EventQueue = new List<EventInQueue>();

	[HideInInspector]
	public List<CFGFlowEventBase> m_AwaitEvents = new List<CFGFlowEventBase>();

	public static Type GameType => typeof(CFGMissionFlow);

	public UnityEngine.Object GameContext
	{
		get
		{
			if (_gameContext == null)
			{
				_gameContext = UnityEngine.Object.FindObjectOfType(GameType);
			}
			return _gameContext;
		}
	}

	public IEnumerable<CFGFlowSequence> Sequences
	{
		get
		{
			Stack<CFGFlowSequence> sequences = new Stack<CFGFlowSequence>(new CFGFlowSequence[1] { this });
			while (sequences.Any())
			{
				CFGFlowSequence seq = sequences.Pop();
				yield return seq;
				foreach (CFGFlowSequence s in seq.m_FlowObjects.Where((CFGFlowObject o) => o is CFGFlowSequence))
				{
					sequences.Push(s);
				}
			}
		}
	}

	public event FlowProgressEvent OnActivateNode;

	public event FlowProgressEvent OnDeactivateNode;

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.Sequence;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = BaseSerialization(Parent, "Sequence");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		if (!base.OnSerialize(cFG_SG_Node))
		{
			return false;
		}
		CFG_SG_Node parent = cFG_SG_Node.AddSubNode("FlowObjects");
		for (int i = 0; i < m_FlowObjects.Count; i++)
		{
			if (!(m_FlowObjects[i] is CFGFlowSequence))
			{
				if (m_FlowObjects[i].GetFOS_Type() != 0)
				{
					m_FlowObjects[i].OnSerialize(parent);
				}
			}
			else if (!m_FlowObjects[i].OnSerialize(parent))
			{
				return false;
			}
		}
		CFG_SG_Node cFG_SG_Node2 = cFG_SG_Node.AddSubNode("EventQueue");
		for (int j = 0; j < m_EventQueue.Count; j++)
		{
			if (!(m_EventQueue[j].m_Event == null) && m_EventQueue[j].m_Event.m_GUID != null)
			{
				CFG_SG_Node cFG_SG_Node3 = cFG_SG_Node2.AddSubNode("Item");
				if (cFG_SG_Node3 == null)
				{
					return false;
				}
				cFG_SG_Node3.Attrib_Set("UUID", m_EventQueue[j].m_Event.m_GUID);
				cFG_SG_Node3.Attrib_Set("Delay", m_EventQueue[j].m_RetriggerDelay);
			}
		}
		CFG_SG_Node cFG_SG_Node4 = cFG_SG_Node.AddSubNode("AwaitEvent");
		for (int k = 0; k < m_AwaitEvents.Count; k++)
		{
			if (!(m_AwaitEvents[k] == null) && m_AwaitEvents[k].m_GUID != null)
			{
				CFG_SG_Node cFG_SG_Node5 = cFG_SG_Node4.AddSubNode("Item");
				if (cFG_SG_Node5 == null)
				{
					return false;
				}
				cFG_SG_Node5.Attrib_Set("UUID", m_AwaitEvents[k].m_GUID);
			}
		}
		CFG_SG_Node cFG_SG_Node6 = cFG_SG_Node.AddSubNode("ActiveFlow");
		for (int l = 0; l < m_ActiveFlow.Count; l++)
		{
			if (!(m_ActiveFlow[l] == null) && m_ActiveFlow[l].m_GUID != null)
			{
				CFG_SG_Node cFG_SG_Node7 = cFG_SG_Node6.AddSubNode("Item");
				if (cFG_SG_Node7 == null)
				{
					return false;
				}
				cFG_SG_Node7.Attrib_Set("UUID", m_ActiveFlow[l].m_GUID);
			}
		}
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		if (!OnDeserialize_FlowObjects(_FlowObject))
		{
			LogError("Failed to deserialize flow objects!");
			return false;
		}
		if (!OnDeserialize_EventQueue(_FlowObject))
		{
			LogError("Failed to deserialize event queue!");
			return false;
		}
		if (!OnDeserialize_AwaitEvents(_FlowObject))
		{
			LogError("Failed to deserialize await events!");
			return false;
		}
		if (!OnDeserialize_ActiveNodes(_FlowObject))
		{
			LogError("Failed to deserialize active nodes!");
			return false;
		}
		return true;
	}

	private bool OnDeserialize_ActiveNodes(CFG_SG_Node _FlowObject)
	{
		m_ActiveFlow.Clear();
		CFG_SG_Node cFG_SG_Node = _FlowObject.FindSubNode("ActiveFlow");
		if (cFG_SG_Node == null)
		{
			LogError("Failed to find active flow node list!");
			return false;
		}
		for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
			if (subNode != null && string.Compare("Item", subNode.Name, ignoreCase: true) == 0)
			{
				QueueFlowNode(ReadFlowObject(subNode) as CFGFlowNode);
			}
		}
		return true;
	}

	private bool OnDeserialize_AwaitEvents(CFG_SG_Node _FlowObject)
	{
		m_AwaitEvents.Clear();
		CFG_SG_Node cFG_SG_Node = _FlowObject.FindSubNode("AwaitEvent");
		if (cFG_SG_Node == null)
		{
			LogError("Failed to find await event list!");
			return false;
		}
		for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
			if (subNode != null && string.Compare("Item", subNode.Name, ignoreCase: true) == 0)
			{
				m_AwaitEvents.Add(ReadFlowObject(subNode) as CFGFlowEventBase);
			}
		}
		return true;
	}

	private CFGFlowObject ReadFlowObject(CFG_SG_Node Node)
	{
		CFGGUID cFGGUID = Node.Attrib_Get<CFGGUID>("UUID", null);
		if (cFGGUID == null)
		{
			LogError("Failed to read event's guid!");
			return null;
		}
		CFGFlowObject flowObjectByGUID = GetFlowObjectByGUID(cFGGUID);
		if (flowObjectByGUID == null)
		{
			LogError("Failed to find event object: " + cFGGUID.ToString());
		}
		return flowObjectByGUID;
	}

	private bool OnDeserialize_EventQueue(CFG_SG_Node _FlowObject)
	{
		m_EventQueue.Clear();
		CFG_SG_Node cFG_SG_Node = _FlowObject.FindSubNode("EventQueue");
		if (cFG_SG_Node == null)
		{
			LogError("Failed to find events list!");
			return false;
		}
		for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
			if (subNode != null && string.Compare("Item", subNode.Name, ignoreCase: true) == 0)
			{
				EventInQueue item = default(EventInQueue);
				item.m_Event = ReadFlowObject(subNode) as CFGFlowEventBase;
				item.m_RetriggerDelay = subNode.Attrib_Get("Delay", 0f);
				m_EventQueue.Add(item);
			}
		}
		return true;
	}

	private bool OnDeserialize_FlowObjects(CFG_SG_Node nd)
	{
		CFG_SG_Node cFG_SG_Node = nd.FindSubNode("FlowObjects");
		if (cFG_SG_Node == null)
		{
			CFGError.ReportError("Failed to locate FlowObjects node - cannot deserialize objects", CFGError.ErrorCode.FileIsCorrupted);
			return false;
		}
		for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
			if (subNode == null)
			{
				return false;
			}
			if (subNode.Attrib_Get("Type", EFOSType.Unknown) == EFOSType.Unknown)
			{
				LogWarning("Failed to read flow object's type " + nd.Name + " subnode: " + subNode.Name);
				return false;
			}
			CFGGUID cFGGUID = subNode.Attrib_Get<CFGGUID>("UUID", null);
			if (cFGGUID == null)
			{
				LogWarning("Failed to read flow object's guid ");
				return false;
			}
			CFGFlowObject flowObjectByGUID = GetFlowObjectByGUID(cFGGUID);
			if (flowObjectByGUID == null)
			{
				LogWarning("Failed to find flow object " + cFGGUID);
			}
			else if (!flowObjectByGUID.OnDeSerialize(subNode))
			{
				return false;
			}
		}
		return true;
	}

	private CFGFlowObject GetFlowObjectByGUID(CFGGUID guid)
	{
		for (int i = 0; i < m_FlowObjects.Count; i++)
		{
			if (m_FlowObjects[i].m_GUID != null && m_FlowObjects[i].m_GUID.IsEqualTo(guid))
			{
				return m_FlowObjects[i];
			}
		}
		return null;
	}

	public override bool OnPostLoad()
	{
		foreach (CFGFlowObject flowObject in m_FlowObjects)
		{
			if (flowObject != null)
			{
				flowObject.OnPostLoad();
			}
		}
		if (m_ActiveFlow != null)
		{
			foreach (CFGFlowNode item in m_ActiveFlow)
			{
				item.PopulateVariableValues();
			}
		}
		return true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_FlowObjects == null)
		{
			m_FlowObjects = new List<CFGFlowObject>();
		}
	}

	public void StopExecution()
	{
		_stopExecution = true;
	}

	public override string GetDisplayName()
	{
		return (!string.IsNullOrEmpty(m_NodeName)) ? m_NodeName : "Sequence";
	}

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Activate", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Abort", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Finished", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Aborted", null, null, string.Empty);
	}

	public List<CFGFlowSequence> GetSequences()
	{
		return Sequences.ToList();
	}

	public List<T> GetNodesOfType<T>() where T : CFGFlowObject
	{
		List<T> list = new List<T>();
		foreach (CFGFlowSequence sequence in Sequences)
		{
			list.AddRange(sequence.m_FlowObjects.FindAll((CFGFlowObject o) => o is T).Cast<T>());
		}
		return list;
	}

	protected void RegisterAutoEvents()
	{
		for (int i = 0; i < m_FlowObjects.Count; i++)
		{
			CFGFlowEvent_SequenceActivated cFGFlowEvent_SequenceActivated = m_FlowObjects[i] as CFGFlowEvent_SequenceActivated;
			if (cFGFlowEvent_SequenceActivated != null)
			{
				cFGFlowEvent_SequenceActivated.RegisterEvent(this);
			}
		}
	}

	public override void Activated()
	{
		if (m_Inputs.Count >= 2 && m_Inputs[1].m_HasImpulse)
		{
			StopExecution();
			return;
		}
		RegisterAutoEvents();
		foreach (CFGFlowObject flowObject in m_FlowObjects)
		{
			CFGFlowEvent_SequenceActivated cFGFlowEvent_SequenceActivated = flowObject as CFGFlowEvent_SequenceActivated;
			if (cFGFlowEvent_SequenceActivated != null)
			{
				QueueFlowNode(cFGFlowEvent_SequenceActivated, bPushTop: true);
			}
		}
		m_ActivateCount++;
		m_Active = true;
	}

	public override void DeActivated()
	{
		bool flag = m_ActiveFlow.Count > 0 || _stopExecution;
		if (m_Inputs.Count >= 2 && m_ActiveFlow.Count <= 0 && m_Inputs[1].m_HasImpulse)
		{
			return;
		}
		if (m_Outputs.Count >= 2)
		{
			if (flag)
			{
				m_Outputs[1].m_HasImpulse = true;
			}
			else
			{
				m_Outputs[0].m_HasImpulse = true;
			}
		}
		m_ActiveFlow.Clear();
		m_EventQueue.Clear();
		m_AwaitEvents.Clear();
		_stopExecution = false;
		base.DeActivated();
	}

	public override void ReActivated()
	{
		if (m_Inputs.Count >= 2 && m_Inputs[1].m_HasImpulse)
		{
			StopExecution();
		}
	}

	public override bool UpdateFlow(float deltaTime)
	{
		if (_stopExecution)
		{
			return true;
		}
		bool flag = false;
		flag = ((m_Inputs.Count < 2) ? ExecuteActiveFlow(deltaTime) : (m_Outputs[1].m_HasImpulse || ExecuteActiveFlow(deltaTime)));
		return m_ParentSequence != null && flag;
	}

	private bool ExecuteActiveFlow(float deltaTime, int maxSteps = 0)
	{
		List<CFGFlowNode> list = new List<CFGFlowNode>();
		List<CFGFlowNode> list2 = new List<CFGFlowNode>();
		for (int i = 0; i < m_EventQueue.Count; i++)
		{
			EventInQueue inq = m_EventQueue[i];
			inq.m_RetriggerDelay -= deltaTime;
			if (inq.m_RetriggerDelay <= 0f)
			{
				m_EventQueue.RemoveAt(i--);
				int num = m_AwaitEvents.FindIndex((CFGFlowEventBase p) => p == inq.m_Event);
				if (num != -1)
				{
					QueueFlowNode(m_AwaitEvents[num], bPushTop: true, bAllowSame: true);
					m_AwaitEvents.RemoveAt(num);
				}
			}
			else
			{
				m_EventQueue[i] = inq;
			}
		}
		int num2 = 0;
		if (m_ActiveFlow == null)
		{
			return false;
		}
		int num3 = m_ActiveFlow.Count;
		while (num3 > 0 && (num2++ < maxSteps || maxSteps == 0))
		{
			if (num2 >= 1000)
			{
				Log("Max execution steps exceeded, aborting!");
				break;
			}
			CFGFlowNode cFGFlowNode = m_ActiveFlow[num3 - 1];
			m_ActiveFlow.RemoveAt(num3 - 1);
			if (!(cFGFlowNode != null))
			{
				continue;
			}
			if (!cFGFlowNode.m_Active && !cFGFlowNode.m_Disabled)
			{
				cFGFlowNode.PopulateVariableValues();
				if (this.OnActivateNode != null)
				{
					this.OnActivateNode(cFGFlowNode);
				}
				cFGFlowNode.m_Active = true;
				cFGFlowNode.m_ActivateCount++;
				cFGFlowNode.Activated();
			}
			bool flag = false;
			if (cFGFlowNode.m_Active || cFGFlowNode.m_Disabled)
			{
				if (cFGFlowNode.m_Disabled)
				{
					cFGFlowNode.DeActivated();
					list.AddRange(cFGFlowNode.GetImpulsedFlow());
				}
				else
				{
					cFGFlowNode.m_Active = !cFGFlowNode.UpdateFlow(deltaTime);
					if (!cFGFlowNode.m_Active)
					{
						flag = true;
						cFGFlowNode.DeActivated();
						cFGFlowNode.PublishVariableValues();
						list.AddRange(cFGFlowNode.GetImpulsedFlow());
					}
					else
					{
						list2.Add(cFGFlowNode);
					}
				}
			}
			if (!m_ActiveFlow.Contains(cFGFlowNode))
			{
				cFGFlowNode.ClearInputs();
			}
			cFGFlowNode.ClearOutputs();
			num3--;
			while (list.Count > 0)
			{
				int index = list.Count - 1;
				CFGFlowNode cFGFlowNode2 = list[index];
				if (cFGFlowNode2.m_Active)
				{
					cFGFlowNode2.ReActivated();
				}
				else
				{
					QueueFlowNode(cFGFlowNode2, bPushTop: true, cFGFlowNode2.AllowMultiInFrame);
				}
				list.RemoveAt(index);
			}
			if (flag)
			{
				cFGFlowNode.PostDeActivated();
				if (this.OnDeactivateNode != null)
				{
					this.OnDeactivateNode(cFGFlowNode);
				}
			}
		}
		while (list2.Count > 0)
		{
			CFGFlowNode cFGFlowNode3 = list2[list2.Count - 1];
			list2.Remove(cFGFlowNode3);
			if (cFGFlowNode3 != null && cFGFlowNode3.m_Active)
			{
				QueueFlowNode(cFGFlowNode3, bPushTop: true);
			}
		}
		return m_ActiveFlow.Count == 0 && m_RegisteredEvents.Count == 0;
	}

	public bool QueueFlowNode(CFGFlowNode newFlowNode, bool bPushTop = false, bool bAllowSame = false)
	{
		bool result = false;
		if (newFlowNode != null)
		{
			if (newFlowNode is CFGFlowCustomEvent)
			{
				CFGFlowCustomEvent cFGFlowCustomEvent = newFlowNode as CFGFlowCustomEvent;
				if (cFGFlowCustomEvent.m_MaxActivationCount > 0 && cFGFlowCustomEvent.m_ActivateCount >= cFGFlowCustomEvent.m_MaxActivationCount)
				{
					return false;
				}
				bool flag = false;
				foreach (EventInQueue item2 in m_EventQueue)
				{
					if (item2.m_Event == cFGFlowCustomEvent)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					m_AwaitEvents.Add(cFGFlowCustomEvent);
					return true;
				}
				EventInQueue item = default(EventInQueue);
				item.m_Event = cFGFlowCustomEvent;
				item.m_RetriggerDelay = cFGFlowCustomEvent.m_ReActivationDelay;
				m_EventQueue.Add(item);
			}
			if (bAllowSame || !m_ActiveFlow.Contains(newFlowNode))
			{
				int index = (bPushTop ? m_ActiveFlow.Count : 0);
				m_ActiveFlow.Insert(index, newFlowNode);
			}
			result = true;
		}
		return result;
	}

	public void EventUnregistered(CFGFlowEventBase e)
	{
		for (int i = 0; i < m_AwaitEvents.Count; i++)
		{
			if (m_AwaitEvents[i] == e)
			{
				m_AwaitEvents.RemoveAt(i--);
			}
		}
	}

	public bool ActivateRemoteEvent(string eventName)
	{
		List<CFGFlowEvent_RemoteEvent> list = GetNodesOfType<CFGFlowEvent_RemoteEvent>().FindAll((CFGFlowEvent_RemoteEvent p) => p.m_RemoteName == eventName);
		foreach (CFGFlowEvent_RemoteEvent item in list)
		{
			if (item.m_ParentSequence != null && item.m_ParentSequence.m_Active)
			{
				item.m_ParentSequence.QueueFlowNode(item, bPushTop: true, bAllowSame: true);
			}
		}
		return true;
	}

	public bool ActivateSubsequencesByName(string sequenceName)
	{
		foreach (CFGFlowSequence item in from s in Sequences
			where s.m_NodeName.Equals(sequenceName)
			where !s.m_Active
			select s)
		{
			QueueFlowNode(item);
		}
		return true;
	}
}
