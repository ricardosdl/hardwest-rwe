using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGScenarioElement : UIBehaviour
{
	public CFGButtonExtension m_Button;

	public GameObject m_BigVersion;

	public GameObject m_SmallVersion;

	public CFGImageExtension m_LevelImageS;

	public CFGImageExtension m_EasyModeS;

	public CFGImageExtension m_MediumModeS;

	public CFGImageExtension m_HardModeS;

	public CFGImageExtension m_IronmanS;

	public CFGImageExtension m_InjuriesS;

	public CFGImageExtension m_UnlockableImg1aS;

	public CFGImageExtension m_UnlockableImg2aS;

	public CFGImageExtension m_UnlockableImg3aS;

	public CFGImageExtension m_UnlockableImg1bS;

	public CFGImageExtension m_UnlockableImg2bS;

	public GameObject m_Star2S;

	public GameObject m_Star3S;

	public Text m_PercentS;

	public CFGImageExtension m_LevelImageB;

	public CFGImageExtension m_EasyModeB;

	public CFGImageExtension m_MediumModeB;

	public CFGImageExtension m_HardModeB;

	public CFGImageExtension m_IronmanB;

	public CFGImageExtension m_InjuriesB;

	public CFGImageExtension m_UnlockableImg1aB;

	public CFGImageExtension m_UnlockableImg2aB;

	public CFGImageExtension m_UnlockableImg3aB;

	public CFGImageExtension m_UnlockableImg1bB;

	public CFGImageExtension m_UnlockableImg2bB;

	public GameObject m_Star2B;

	public GameObject m_Star3B;

	public Text m_PercentB;

	public CFGImageExtension m_UnlockedFrame;

	public List<CFGImageExtension> m_Connectors = new List<CFGImageExtension>();

	private bool m_Selected;

	private bool m_Completed;

	private bool m_Unlocked;

	private int m_CompletedPercent;

	private string m_ScenarioId = string.Empty;

	private string m_ScenarioNameID;

	public bool Selected
	{
		get
		{
			return m_Selected;
		}
		set
		{
			m_Selected = value;
			m_BigVersion.gameObject.SetActive(m_Selected);
			m_SmallVersion.gameObject.SetActive(!m_Selected);
		}
	}

	public bool Completed
	{
		get
		{
			return m_Completed;
		}
		set
		{
			m_Completed = value;
		}
	}

	public bool Unlocked
	{
		get
		{
			return m_Unlocked;
		}
		set
		{
			m_Unlocked = value;
			m_EasyModeS.gameObject.SetActive(m_Unlocked);
			m_MediumModeS.gameObject.SetActive(m_Unlocked);
			m_HardModeS.gameObject.SetActive(m_Unlocked);
			m_IronmanS.gameObject.SetActive(m_Unlocked);
			m_InjuriesS.gameObject.SetActive(m_Unlocked);
			m_UnlockableImg1aB.gameObject.SetActive(m_Unlocked);
			m_UnlockableImg2aB.gameObject.SetActive(m_Unlocked);
			m_UnlockableImg3aB.gameObject.SetActive(m_Unlocked);
			m_UnlockableImg1bB.gameObject.SetActive(m_Unlocked);
			m_UnlockableImg2bB.gameObject.SetActive(m_Unlocked);
			m_Button.enabled = m_Unlocked;
			m_UnlockedFrame.IconNumber = ((!m_Unlocked) ? 1 : 0);
			foreach (CFGImageExtension connector in m_Connectors)
			{
				connector.IconNumber = (m_Unlocked ? 1 : 0);
			}
			SetLabel();
		}
	}

	public int CompletedPercent
	{
		get
		{
			return m_CompletedPercent;
		}
		set
		{
			m_CompletedPercent = value;
			SetLabel();
		}
	}

	public string ScenarioId
	{
		get
		{
			return m_ScenarioId;
		}
		set
		{
			m_ScenarioId = value;
		}
	}

	public string ScenarioNameID
	{
		get
		{
			return m_ScenarioNameID;
		}
		set
		{
			m_ScenarioNameID = value;
		}
	}

	public void SetLabel()
	{
		if (Unlocked)
		{
			m_PercentS.text = m_CompletedPercent + "%";
			m_PercentB.text = m_CompletedPercent + "%";
		}
		else
		{
			m_PercentS.text = string.Empty;
			m_PercentB.text = string.Empty;
		}
	}
}
