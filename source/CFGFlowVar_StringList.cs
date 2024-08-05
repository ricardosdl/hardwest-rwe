using System.Collections.Generic;

public class CFGFlowVar_StringList : CFGFlowVar_Typed<List<string>>
{
	public override string GetTypeName()
	{
		return "StringList";
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarStringList;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = BaseSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		if (!base.OnSerialize(cFG_SG_Node))
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Count", m_Value.Count);
		for (int i = 0; i < m_Value.Count; i++)
		{
			string attName = "Value" + i;
			if (m_Value[i] == null)
			{
				cFG_SG_Node.Attrib_Set(attName, string.Empty);
			}
			else
			{
				cFG_SG_Node.Attrib_Set(attName, m_Value[i]);
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
		m_Value.Clear();
		int num = _FlowObject.Attrib_Get("Count", 0);
		if (num == 0)
		{
			return true;
		}
		for (int i = 0; i < num; i++)
		{
			string attName = "Value" + i;
			string item = _FlowObject.Attrib_Get<string>(attName, null);
			m_Value.Add(item);
		}
		return true;
	}
}
