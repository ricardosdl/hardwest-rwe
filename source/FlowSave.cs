using UnityEngine;

public class FlowSave
{
	public const string SID_ND_GAMEFLOW = "GameFlow";

	public const string SID_ACTIVATECOUNT = "ActivateCount";

	public const string SID_ND_SEQUENCE = "Sequence";

	public const string SID_ND_FLOWOBJECTS = "FlowObjects";

	public const string SID_ND_EVENTQUEUE = "EventQueue";

	public const string SID_DELAY = "Delay";

	public const string SID_ND_AWAITEVENT = "AwaitEvent";

	public const string SID_ND_ACTIVEFLOW = "ActiveFlow";

	public const string SID_ACTIVE = "Active";

	public const string SID_ND_GAME_OBJECT = "Object";

	public const string SID_UUID = "UUID";

	public const string SID_TYPE = "Type";

	public const string SID_DISABLED = "Disabled";

	public const string SID_COUNT = "Count";

	public const string SID_ID = "ID";

	public const string SID_TIME = "Time";

	public const string SID_ITEM1 = "Item1";

	public const string SID_ITEM2 = "Item2";

	public const string SID_VALUE = "Value";

	public const string SID_ITEM_INVENTORY = "Item";

	public const string SID_TARGETUUID = "TargetUUID";

	public const string SID_STATE = "State";

	public static bool WriteGameFlow(CFG_SG_Node parent)
	{
		CFGSequencerBase cFGSequencerBase = Object.FindObjectOfType<CFGSequencerBase>();
		if (cFGSequencerBase == null)
		{
			Debug.LogWarning("No sequencer on scene -> skipping Flow save");
			return false;
		}
		CFG_SG_Node cFG_SG_Node = parent.FindOrCreateSubNode("GameFlow");
		if (cFG_SG_Node != null)
		{
			return cFGSequencerBase.OnSerialize(cFG_SG_Node);
		}
		Debug.LogError("Could not find or create node for Flow -> skipping Flow save");
		return false;
	}

	public static bool RestoreFlow(CFG_SG_Node parent)
	{
		CFGSequencerBase cFGSequencerBase = Object.FindObjectOfType<CFGSequencerBase>();
		if (cFGSequencerBase == null)
		{
			Debug.LogWarning("No sequencer on scene -> skipping Flow load");
			return false;
		}
		CFG_SG_Node cFG_SG_Node = parent.FindSubNode("GameFlow");
		if (cFG_SG_Node != null)
		{
			return cFGSequencerBase.OnDeserialize(cFG_SG_Node);
		}
		Debug.LogError("Could not find Flow node -> skipping Flow load");
		return false;
	}
}
