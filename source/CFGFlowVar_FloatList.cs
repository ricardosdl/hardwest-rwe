using System.Collections.Generic;

public class CFGFlowVar_FloatList : CFGFlowVar_Typed<List<float>>
{
	public override string GetTypeName()
	{
		return "FloatList";
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarFloatList;
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
			CFG_SG_Node cFG_SG_Node2 = cFG_SG_Node.AddSubNode("Item");
			if (cFG_SG_Node2 == null)
			{
				return false;
			}
			cFG_SG_Node2.Attrib_Set("ID", i);
			cFG_SG_Node2.Attrib_Set("Value", m_Value[i]);
		}
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		int num = _FlowObject.Attrib_Get("Count", -1);
		if (num < 1)
		{
			return true;
		}
		m_Value.Clear();
		for (int i = 0; i < num; i++)
		{
			m_Value.Add(0f);
		}
		for (int j = 0; j < _FlowObject.SubNodeCount; j++)
		{
			CFG_SG_Node subNode = _FlowObject.GetSubNode(j);
			if (subNode != null && string.Compare(subNode.Name, "Item", ignoreCase: true) == 0)
			{
				int num2 = subNode.Attrib_Get("ID", -1);
				if (num2 < 0 || num2 >= m_Value.Count)
				{
					LogWarning("Float List: Wrong index: " + num2);
					continue;
				}
				float value = subNode.Attrib_Get("Value", 0f);
				m_Value[num2] = value;
			}
		}
		return true;
	}
}
