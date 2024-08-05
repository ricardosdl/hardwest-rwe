using UnityEngine;

public class CFGAiPreset : ScriptableObject
{
	public enum EMovementMode
	{
		Defensive,
		Offensive,
		Hiding,
		Custom
	}

	public enum EAPReservedForMove
	{
		None,
		One,
		Both
	}

	public enum ETargetingMode
	{
		Highest_CTH,
		Highest_Dmg,
		Optimum
	}

	public EMovementMode m_MovementMode = EMovementMode.Offensive;

	public EAPReservedForMove m_ApReservedForMove;

	public ETargetingMode m_TargetingMode = ETargetingMode.Optimum;

	public int m_ObjectiveRadius = 5;

	public bool m_ObjectiveInLoSRequired = true;

	public int m_DefenseRate = 1;

	public int m_OffenseRate = 1;

	public int m_PressingRate = 1;

	public int m_MobilityRate = 1;

	public int m_HidingRate = 1;

	public int m_ScoutingRate = 1;

	public void OnSerialize(CFG_SG_Node ParentNode)
	{
		CFG_SG_Node cFG_SG_Node = ParentNode.AddSubNode("AiPreset");
		if (cFG_SG_Node != null)
		{
			cFG_SG_Node.Attrib_Set("MovementMode", m_MovementMode);
			cFG_SG_Node.Attrib_Set("APReservedForMove", m_ApReservedForMove);
			cFG_SG_Node.Attrib_Set("TargetingMode", m_TargetingMode);
			cFG_SG_Node.Attrib_Set("ObjectiveRadius", m_ObjectiveRadius);
			cFG_SG_Node.Attrib_Set("ObjectiveInLOSRequired", m_ObjectiveInLoSRequired);
			cFG_SG_Node.Attrib_Set("DefenseRate", m_DefenseRate);
			cFG_SG_Node.Attrib_Set("OffenseRate", m_OffenseRate);
			cFG_SG_Node.Attrib_Set("PressingRate", m_PressingRate);
			cFG_SG_Node.Attrib_Set("MobilityRate", m_MobilityRate);
			cFG_SG_Node.Attrib_Set("HidingRate", m_HidingRate);
			cFG_SG_Node.Attrib_Set("ScoutingRate", m_ScoutingRate);
		}
	}

	public void OnDeserialize(CFG_SG_Node node)
	{
		m_MovementMode = node.Attrib_Get("MovementMode", EMovementMode.Offensive);
		m_ApReservedForMove = node.Attrib_Get("APReservedForMove", EAPReservedForMove.None);
		m_TargetingMode = node.Attrib_Get("TargetingMode", ETargetingMode.Optimum);
		m_ObjectiveRadius = node.Attrib_Get("ObjectiveRadius", 5);
		m_ObjectiveInLoSRequired = node.Attrib_Get("ObjectiveInLOSRequired", DefVal: true);
		m_DefenseRate = node.Attrib_Get("DefenseRate", 1);
		m_OffenseRate = node.Attrib_Get("OffenseRate", 1);
		m_PressingRate = node.Attrib_Get("PressingRate", 1);
		m_MobilityRate = node.Attrib_Get("MobilityRate", 1);
		m_HidingRate = node.Attrib_Get("HidingRate", 1);
		m_ScoutingRate = node.Attrib_Get("ScoutingRate", 1);
	}
}
