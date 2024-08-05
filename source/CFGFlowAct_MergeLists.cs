using System.Collections.Generic;
using System.Linq;

public abstract class CFGFlowAct_MergeLists<T> : CFGFlowGameAction
{
	public List<T> m_List;

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Lists", CFGFlowVariable.GetFlowVarType(typeof(List<T>)), typeof(List<T>), string.Empty, byte.MaxValue);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, "Merged", CFGFlowVariable.GetFlowVarType(typeof(List<T>)), typeof(List<T>), "m_List");
	}

	public override void Activated()
	{
		m_List = new List<T>();
		foreach (CFGFlowVar_Typed<List<T>> item in m_Vars.Where((CFGFlowConn_Var v) => v.m_ConnDir == ConnectorDirection.CD_Input).SelectMany((CFGFlowConn_Var v) => v.m_VarLinks).ToList())
		{
			m_List.AddRange(item.m_Value);
		}
	}
}
