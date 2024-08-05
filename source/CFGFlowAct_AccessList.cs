using System;
using System.Collections.Generic;

public abstract class CFGFlowAct_AccessList<T> : CFGFlowGameAction
{
	public List<T> m_List;

	public int m_Idx;

	public T m_Object;

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "List", CFGFlowVariable.GetFlowVarType(typeof(List<T>)), typeof(List<T>), "m_List");
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Index", typeof(CFGFlowVar_Int), typeof(int), "m_Idx");
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, "Output item", CFGFlowVariable.GetFlowVarType(typeof(T)), typeof(T), "m_Object");
		m_Inputs[0].m_ConnName = "Random";
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "First", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Last", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "At Index", null, null, string.Empty);
	}

	public override void Activated()
	{
		int impulsedInputIndex = GetImpulsedInputIndex();
		int num = -1;
		if (m_List == null || m_List.Count <= 0)
		{
			LogError("Flow_AccessList -> List is null or empty");
			m_Object = default(T);
			return;
		}
		switch (impulsedInputIndex)
		{
		case 0:
		{
			Random random = new Random();
			num = random.Next(m_List.Count);
			break;
		}
		case 1:
			num = 0;
			break;
		case 2:
			num = m_List.Count - 1;
			break;
		case 3:
			if (m_Idx < 0)
			{
				return;
			}
			num = m_Idx;
			break;
		default:
			num = -1;
			break;
		}
		if (num > m_List.Count - 1)
		{
			LogError("Flow_AccessList -> Argument is out of range(" + num + " out of " + (m_List.Count - 1) + ")");
			m_Object = default(T);
		}
		else
		{
			m_Object = m_List[num];
		}
	}
}
