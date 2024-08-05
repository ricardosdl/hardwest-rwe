public static class CFGPlayerProgress
{
	private static CFGValueTable m_Default_Trinkets = new CFGValueTable();

	private static CFGValueTable m_Default_Globals = new CFGValueTable();

	private static CFGValueTable m_Starter_Globals = new CFGValueTable();

	private static CFGValueTable m_Current_Globals = new CFGValueTable();

	private static CFGValueTable m_Current_Trinkets = new CFGValueTable();

	public static bool Init()
	{
		if (!m_Default_Trinkets.LoadDefinition(CFGData.GetDataPathFor("trinkets.tsv")))
		{
			return false;
		}
		if (!m_Default_Globals.LoadDefinition(CFGData.GetDataPathFor("universals.tsv")))
		{
			return false;
		}
		m_Default_Globals.CloneTo(ref m_Starter_Globals);
		m_Default_Globals.CloneTo(ref m_Current_Globals);
		m_Default_Trinkets.CloneTo(ref m_Current_Trinkets);
		return true;
	}

	public static void Reset(bool bResetTrinkets, bool bResetGlobals)
	{
		if (bResetGlobals)
		{
			m_Default_Globals.CloneTo(ref m_Starter_Globals);
			m_Default_Globals.CloneTo(ref m_Current_Globals);
		}
		if (bResetTrinkets)
		{
			m_Default_Trinkets.CloneTo(ref m_Current_Trinkets);
		}
	}

	public static void OnNewCampaign(CFGValueTable LoadedGlobals)
	{
		if (LoadedGlobals == null)
		{
			m_Starter_Globals.CloneTo(ref m_Current_Globals);
		}
		else
		{
			LoadedGlobals.CloneTo(ref m_Current_Globals);
		}
	}

	public static void Set_Int(bool bGlobal, string Name, int NewValue)
	{
		if (bGlobal)
		{
			m_Current_Globals.Set_Int(Name, NewValue);
		}
		else
		{
			m_Current_Trinkets.Set_Int(Name, NewValue);
		}
	}

	public static int Get_Int(bool bGlobal, string Name)
	{
		if (bGlobal)
		{
			return m_Current_Globals.Get_Int(Name);
		}
		return m_Current_Trinkets.Get_Int(Name);
	}

	public static void Set_Float(bool bGlobal, string Name, float NewValue)
	{
		if (bGlobal)
		{
			m_Current_Globals.Set_Float(Name, NewValue);
		}
		else
		{
			m_Current_Trinkets.Set_Float(Name, NewValue);
		}
	}

	public static float Get_Float(bool bGlobal, string Name)
	{
		if (bGlobal)
		{
			return m_Current_Globals.Get_Float(Name);
		}
		return m_Current_Trinkets.Get_Float(Name);
	}

	public static void Set_String(bool bGlobal, string Name, string NewValue)
	{
		if (bGlobal)
		{
			m_Current_Globals.Set_String(Name, NewValue);
		}
		else
		{
			m_Current_Trinkets.Set_String(Name, NewValue);
		}
	}

	public static string Get_String(bool bGlobal, string Name)
	{
		if (bGlobal)
		{
			return m_Current_Globals.Get_String(Name);
		}
		return m_Current_Trinkets.Get_String(Name);
	}

	public static void ApplyCurrentCampaignGlobals()
	{
		m_Current_Globals.ApplyModified(ref m_Starter_Globals);
	}
}
