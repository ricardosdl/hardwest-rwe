using UnityEngine;

public class CFGBuff
{
	public const string BUFF_SHADOWCLOAK = "shadowcloak";

	public const string BUFF_COURAGE_2 = "courage2";

	public const string BUFF_COURAGE_3 = "courage3";

	public const string BUFF_COURAGE_4 = "courage4";

	public const string BUFF_COURAGE_5 = "courage5";

	public const string BUFF_DODGE = "dodge";

	public const string BUFF_SMELLONTARGET = "revealed";

	public const string BUFF_CRIPPLE_POINTBLANK = "crippledaim";

	public const string BUFF_CRIPPLE_NOCOVER = "crippledmovement";

	public const string BUFF_CRIPPLE_HALFCOVER = "crippledsight";

	public const string BUFF_CRIPPLE_FULLCOVER = "crippleddefense";

	public const string BUFF_CANNIBAL = "wellfed";

	public const string BUFF_VAMPIRE_INSHADOW = "shadowregen";

	public const string BUFF_VAMPIRE_NIGHTMARE = "nightmareregen";

	public const string BUFF_JINXED = "jinxed";

	public const string BUFF_INTIMIDATE = "intimidated";

	public const string BUFF_ARTERYSHOT = "arteryshot";

	public const string BUFF_NEMESIS = "nemesis";

	public const string BUFF_HALFDEAD = "halfdead";

	public const string BUFF_DEMONPOWER = "demonpower";

	public const string BUFF_PRAYER1 = "prayeraim";

	public const string BUFF_PRAYER2 = "prayerinvul";

	public const string BUFF_PRAYER3 = "prayersight";

	public const string BUFF_PRAYER4 = "prayermovement";

	public const string BUFF_PRAYER5 = "prayerheal";

	public const string BUFF_REWARDEDKILL = "rewardedkill";

	public const string BUFF_CRITICALCHARACTER = "critical_buff";

	public const string BUFF_REVEALED = "revealed";

	public CFGDef_Buff m_Def;

	public CFGCharacterStats m_Stats = new CFGCharacterStats();

	public EBuffSource m_Source;

	public int m_Duration = -1;

	public int m_StartTurn = -1;

	private int m_TotalDuration;

	public EBuffAutoEndType AutoEndType
	{
		get
		{
			if (m_Def != null)
			{
				return m_Def.AutoEnd;
			}
			return EBuffAutoEndType.Never;
		}
	}

	public int TotalDuration => m_TotalDuration;

	public int DurationLeft => Mathf.Max(0, m_TotalDuration - m_Duration);

	public CFGBuff(CFGDef_Buff definition, EBuffSource Source)
	{
		m_Stats.Clear();
		m_Source = Source;
		m_TotalDuration = 0;
		m_Duration = 0;
		if (definition != null)
		{
			m_Def = definition;
			m_TotalDuration = m_Def.Duration * 2;
			if (m_Def.AutoEnd == EBuffAutoEndType.AfterDuration && m_TotalDuration == 0)
			{
				m_TotalDuration = 1;
			}
			m_StartTurn = (int)CFGSingletonResourcePrefab<CFGTurnManager>.Instance.Turn;
			m_Stats = new CFGCharacterStats(definition);
		}
	}

	public CFGBuff(CFG_SG_Node BuffNode)
	{
		m_Stats.Clear();
		m_Def = null;
		OnDeserialize(BuffNode);
	}

	public string ToStrigName()
	{
		string empty = string.Empty;
		empty = ((m_Def.Color_Font == "positive") ? "<color=#96be46>" : ((!(m_Def.Color_Font == "negative")) ? "<color=#fadc5a>" : "<color=#ff4646>"));
		string empty2 = string.Empty;
		empty2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Def.BuffID);
		return empty + empty2 + "</color>";
	}

	public string ToStringDesc(bool tactical)
	{
		string empty = string.Empty;
		empty = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Def.BuffID + "_desc");
		string text = string.Empty;
		if (m_Def.HPChange != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.HPChange <= 0) ? m_Def.HPChange.ToString() : ("+" + m_Def.HPChange)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_hpchange") + "; ";
		}
		if (m_Def.LuckChange != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.LuckChange <= 0) ? m_Def.LuckChange.ToString() : ("+" + m_Def.LuckChange)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_luckchange") + "; ";
		}
		if (m_Def.Mod_Aim != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.Mod_Aim <= 0) ? m_Def.Mod_Aim.ToString() : ("+" + m_Def.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
		}
		if (m_Def.Mod_Defense != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.Mod_Defense <= 0) ? m_Def.Mod_Defense.ToString() : ("+" + m_Def.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
		}
		if (m_Def.Mod_Sight != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.Mod_Sight <= 0) ? m_Def.Mod_Sight.ToString() : ("+" + m_Def.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
		}
		if (m_Def.Mod_Damage != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.Mod_Damage <= 0) ? m_Def.Mod_Damage.ToString() : ("+" + m_Def.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
		}
		if (m_Def.Mod_Movement != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.Mod_Movement <= 0) ? m_Def.Mod_Movement.ToString() : ("+" + m_Def.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
		}
		if (m_Def.Mod_MaxHP != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.Mod_MaxHP <= 0) ? m_Def.Mod_MaxHP.ToString() : ("+" + m_Def.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
		}
		if (m_Def.Mod_MaxLuck != 0)
		{
			string text2 = text;
			text = text2 + ((m_Def.Mod_MaxLuck <= 0) ? m_Def.Mod_MaxLuck.ToString() : ("+" + m_Def.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
		}
		string text3 = string.Empty;
		if (AutoEndType == EBuffAutoEndType.AfterDuration)
		{
			text3 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_duration", ((DurationLeft + 1) / 2).ToString());
		}
		if (tactical)
		{
			return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_buff_desc_tactical", empty, text, text3);
		}
		return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_buff_desc_strategic", empty, text);
	}

	public void OnSerialize(CFG_SG_Node ParentNode)
	{
		if (m_Def != null)
		{
			CFG_SG_Node cFG_SG_Node = ParentNode.AddSubNode("Buff");
			if (cFG_SG_Node != null)
			{
				cFG_SG_Node.Attrib_Set("Name", m_Def.BuffID);
				cFG_SG_Node.Attrib_Set("Source", m_Source);
				cFG_SG_Node.Attrib_Set("Time", m_Duration);
				cFG_SG_Node.Attrib_Set("Start", m_StartTurn);
			}
		}
	}

	public void OnDeserialize(CFG_SG_Node BuffNode)
	{
		string text = BuffNode.Attrib_Get<string>("Name", null);
		if (text == null)
		{
			return;
		}
		m_Source = BuffNode.Attrib_Get("Source", EBuffSource.Unknown);
		m_Duration = BuffNode.Attrib_Get("Time", -1);
		m_StartTurn = BuffNode.Attrib_Get("Start", -1);
		m_Def = CFGStaticDataContainer.GetBuff(text);
		if (m_Def != null)
		{
			m_TotalDuration = m_Def.Duration * 2;
			if (m_Def.AutoEnd == EBuffAutoEndType.AfterDuration && m_TotalDuration == 0)
			{
				m_TotalDuration = 1;
			}
			m_Stats = new CFGCharacterStats(m_Def);
		}
	}
}
