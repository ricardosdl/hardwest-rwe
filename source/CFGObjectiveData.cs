public class CFGObjectiveData
{
	public string m_Id = string.Empty;

	public string m_DescShort = string.Empty;

	public string m_DescLong = string.Empty;

	public EObjectiveScene m_ObjectiveScene;

	public EObjectiveImportance m_Importance;

	public EObjectiveType m_Type;

	public CFGObjectiveData(string id, EObjectiveImportance importance, EObjectiveType type, EObjectiveScene OScene)
	{
		m_Id = id;
		m_Importance = importance;
		m_Type = type;
		m_ObjectiveScene = OScene;
		UpdateLocalizedTexts();
	}

	public void UpdateLocalizedTexts()
	{
		m_DescShort = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Id);
		m_DescLong = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Id + "_desc_long");
	}
}
