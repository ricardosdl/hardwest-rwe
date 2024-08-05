public class CFGDef_Card
{
	[CFGTableField(ColumnName = "card_id", DefaultValue = "")]
	public string CardID = string.Empty;

	[CFGTableField(ColumnName = "rank", DefaultValue = "", Optional = true)]
	public string UnparsedRank = string.Empty;

	[CFGTableField(ColumnName = "compatibility", DefaultValue = "", Optional = true)]
	public string Compatibility = string.Empty;

	public ECardRank CardRank;

	[CFGTableField(ColumnName = "color", DefaultValue = ECardColor.Unknown, Optional = true)]
	public ECardColor CardColor;

	[CFGTableField(ColumnName = "image_id", DefaultValue = 0)]
	public int ImageID;

	[CFGTableField(ColumnName = "ability_id", DefaultValue = ETurnAction.None)]
	public ETurnAction AbilityID;

	[CFGTableField(ColumnName = "max_health", DefaultValue = 0)]
	public int MaxHealth;

	[CFGTableField(ColumnName = "aim", DefaultValue = 0)]
	public int Aim;

	[CFGTableField(ColumnName = "defense", DefaultValue = 0)]
	public int Defense;

	[CFGTableField(ColumnName = "movement", DefaultValue = 0)]
	public int Movement;

	[CFGTableField(ColumnName = "sight", DefaultValue = 0)]
	public int Sight;

	[CFGTableField(ColumnName = "max_luck", DefaultValue = 0)]
	public int MaxLuck;

	private CFGCharacterStats m_RT_Stats = new CFGCharacterStats();

	public string TextID => CardID;

	public string DescriptionID => CardID + "_desc";

	public CFGCharacterStats CharStats => m_RT_Stats;

	public void UpdateHand_DLC1(ref byte[] SumTable)
	{
		if (string.IsNullOrEmpty(Compatibility))
		{
			return;
		}
		for (int i = 0; i < Compatibility.Length; i++)
		{
			if (Compatibility[i] >= 'a' && Compatibility[i] <= 'z')
			{
				SumTable[Compatibility[i] - 97]++;
			}
		}
	}

	public void GenerateStats()
	{
		if (Compatibility == null)
		{
			Compatibility = string.Empty;
		}
		else
		{
			string text = Compatibility.ToLower();
			Compatibility = string.Empty;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] >= 'a' && text[i] <= 'z')
				{
					Compatibility += text[i];
				}
			}
		}
		if (string.Compare(UnparsedRank, "9", ignoreCase: true) == 0)
		{
			CardRank = ECardRank.Rank_9;
		}
		else if (string.Compare(UnparsedRank, "10", ignoreCase: true) == 0)
		{
			CardRank = ECardRank.Rank_10;
		}
		else if (string.Compare(UnparsedRank, "j", ignoreCase: true) == 0)
		{
			CardRank = ECardRank.Rank_J;
		}
		else if (string.Compare(UnparsedRank, "q", ignoreCase: true) == 0)
		{
			CardRank = ECardRank.Rank_Q;
		}
		else if (string.Compare(UnparsedRank, "k", ignoreCase: true) == 0)
		{
			CardRank = ECardRank.Rank_K;
		}
		else if (string.Compare(UnparsedRank, "a", ignoreCase: true) == 0)
		{
			CardRank = ECardRank.Rank_A;
		}
		else if (string.Compare(UnparsedRank, "joker", ignoreCase: true) == 0)
		{
			CardRank = ECardRank.Rank_Joker;
		}
		if (m_RT_Stats == null)
		{
			m_RT_Stats = new CFGCharacterStats();
		}
		if (m_RT_Stats != null)
		{
			m_RT_Stats.m_MaxHealth = MaxHealth;
			m_RT_Stats.m_Aim = Aim;
			m_RT_Stats.m_Defense = Defense;
			m_RT_Stats.m_Movement = Movement;
			m_RT_Stats.m_Sight = Sight;
			m_RT_Stats.m_MaxLuck = MaxLuck;
			m_RT_Stats.m_LuckReplenishTurn = 0;
			m_RT_Stats.m_LuckReplenishHit = 0;
		}
	}
}
