using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CFGSequencerBase : MonoBehaviour, ICFGSequencer
{
	public List<CFGFlowSequence> m_Sequences = new List<CFGFlowSequence>();

	public virtual CFGFlowSequence MainSequence
	{
		get
		{
			return m_Sequences.FirstOrDefault();
		}
		set
		{
			if (m_Sequences == null)
			{
				m_Sequences = new List<CFGFlowSequence>();
			}
			if (m_Sequences.Any())
			{
				m_Sequences[0] = value;
			}
			else
			{
				m_Sequences.Add(value);
			}
		}
	}

	protected virtual void Update()
	{
		CFGFlowSequence mainSequence = MainSequence;
		if (mainSequence != null && mainSequence.m_Active)
		{
			mainSequence.UpdateFlow(Time.deltaTime);
		}
	}

	public virtual bool OnSerialize(CFG_SG_Node Node)
	{
		Debug.Log("Serializing Sequencer with " + m_Sequences.Count + " sequences");
		foreach (CFGFlowSequence sequence in m_Sequences)
		{
			if (!(sequence == null))
			{
				sequence.OnSerialize(Node);
			}
		}
		return true;
	}

	public virtual bool OnDeserialize(CFG_SG_Node Node)
	{
		CFGGUID cFGGUID = new CFGGUID();
		for (int i = 0; i < Node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = Node.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Sequence", ignoreCase: true) != 0)
			{
				continue;
			}
			cFGGUID = subNode.Attrib_Get<CFGGUID>("UUID", null);
			if (cFGGUID == null)
			{
				Debug.LogWarning("Serialized sequence has NULL GUID and cannot be used!");
				continue;
			}
			if (cFGGUID.IsClear())
			{
				Debug.LogWarning("Serialized sequence has CLEAR GUID and cannot be used!");
				continue;
			}
			bool flag = false;
			foreach (CFGFlowSequence sequence in m_Sequences)
			{
				if (sequence.m_GUID != null && sequence.m_GUID.IsEqualTo(cFGGUID))
				{
					flag = true;
					if (!sequence.OnDeSerialize(subNode))
					{
						return false;
					}
				}
			}
			if (!flag)
			{
				Debug.LogWarning("Failed to find sequence: " + cFGGUID.ToString());
			}
		}
		MainSequence.m_Active = false;
		return true;
	}

	public static CFGFlowSequence GetMainSequence()
	{
		CFGSequencerBase cFGSequencerBase = Object.FindObjectOfType<CFGSequencerBase>();
		return (!(cFGSequencerBase != null)) ? null : cFGSequencerBase.MainSequence;
	}
}
