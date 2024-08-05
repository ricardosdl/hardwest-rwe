public class CFGStrategicManager : CFGSingletonResourcePrefab<CFGStrategicManager>
{
	private CFGVariableTable m_VT_Global = new CFGVariableTable();

	public void Clear()
	{
		m_VT_Global.Clear();
	}

	public void Set_Int(string Name, int Value)
	{
		m_VT_Global.Set_Int(Name, Value);
	}

	public int Get_Int(string Name)
	{
		return m_VT_Global.Get_Int(Name);
	}

	public void Set_String(string Name, string Value)
	{
		m_VT_Global.Set_String(Name, Value);
	}

	public string Get_String(string Name)
	{
		return m_VT_Global.Get_String(Name);
	}
}
