public class CFGFlowAct_ShowLocationPanel : CFGFlowGameAction
{
	public const int BUTTON_USER_DATA_DECAY_ID = 100;

	[CFGFlowProperty(displayName = "Title", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Title;

	[CFGFlowProperty(displayName = "Description", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Description;

	[CFGFlowProperty(displayName = "Image", expectedType = typeof(CFGFlowVar_Int), bWritable = false)]
	public int m_Image;

	[CFGFlowProperty(displayName = "Btn1 text", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Btn1Text;

	[CFGFlowProperty(displayName = "Btn2 text", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Btn2Text;

	[CFGFlowProperty(displayName = "Btn3 text", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Btn3Text;

	[CFGFlowProperty(displayName = "Btn4 text", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Btn4Text;

	[CFGFlowProperty(displayName = "Btn5 text", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Btn5Text;

	[CFGFlowProperty(displayName = "Btn6 text", expectedType = typeof(CFGFlowVar_String), bWritable = false)]
	public string m_Btn6Text;

	private bool m_Finished;

	public override bool DefaultOutput => false;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		m_Outputs[0].m_ConnName = "Button1";
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Button2", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Button3", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Button4", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Button5", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Button6", null, null, string.Empty);
	}

	public override void Activated()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		m_Finished = false;
		if (!(instance != null) || !(instance.m_StrategicExplorator != null))
		{
			return;
		}
		string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Title);
		string localizedText2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Description);
		instance.m_StrategicExplorator.ShowExplorationWindow();
		instance.m_StrategicExplorator.m_ExplorationWindowTitle.text = localizedText;
		instance.m_StrategicExplorator.m_ExplorationWindowText.WordAfterWordText(localizedText2);
		instance.m_StrategicExplorator.m_OnLocationPanelButtonClickCallback = OnLocationPanelButtonClick;
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			string text = null;
			switch (i)
			{
			case 0:
				text = m_Btn1Text;
				break;
			case 1:
				text = m_Btn2Text;
				break;
			case 2:
				text = m_Btn3Text;
				break;
			case 3:
				text = m_Btn4Text;
				break;
			case 4:
				text = m_Btn5Text;
				break;
			case 5:
				text = m_Btn6Text;
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				int req_count = -1;
				string item_id = string.Empty;
				bool flag = ShouldOptionBeEnabled(text, ref req_count, ref item_id);
				string text2 = string.Empty;
				if (req_count != -1)
				{
					text2 = ((!(item_id != string.Empty)) ? (((!flag) ? "<color=#af0000ff>" : "<color=#63860fff>") + req_count + "</color>") : ((item_id == "cash") ? (((!flag) ? "<color=#af0000ff>" : "<color=#63860fff>") + "$" + req_count + "</color>") : ((req_count != 1) ? (((!flag) ? "<color=#af0000ff>" : "<color=#63860fff>") + req_count + "x " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(item_id + "_name") + "</color>") : (((!flag) ? "<color=#af0000ff>" : "<color=#63860fff>") + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(item_id + "_name") + "</color>"))));
				}
				int Data = -1;
				int Data2 = -1;
				int sPIconNumber = GetSPIconNumber(text, out Data, out Data2);
				string text3 = num + 1 + ". " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text, text2);
				instance.m_StrategicExplorator.m_ExplorationButtons[num].m_Label.text = text3;
				instance.m_StrategicExplorator.m_ExplorationButtons[num].IconNumber = GetIconNumber(text);
				instance.m_StrategicExplorator.m_ExplorationButtons[num].m_AdditionalIcon.gameObject.SetActive(ShouldSPIconBeVisible(text));
				instance.m_StrategicExplorator.m_ExplorationButtons[num].m_AdditionalIcon.IconNumber = sPIconNumber;
				instance.m_StrategicExplorator.m_ExplorationButtons[num].m_UserData1 = Data;
				instance.m_StrategicExplorator.m_ExplorationButtons[num].m_UserData2 = Data2;
				instance.m_StrategicExplorator.m_ExplorationButtons[num].gameObject.SetActive(value: true);
				instance.m_StrategicExplorator.m_ExplorationButtons[num].m_Data = i;
				instance.m_StrategicExplorator.m_ExplorationButtons[num].SetVisualsEnabled(flag);
				instance.m_StrategicExplorator.m_ExplorationButtons[num].UpdateLabelTextColor();
				num++;
			}
		}
		for (int j = num; j < 6; j++)
		{
			instance.m_StrategicExplorator.m_ExplorationButtons[j].m_Data = -1;
			instance.m_StrategicExplorator.m_ExplorationButtons[j].m_Label.text = string.Empty;
			instance.m_StrategicExplorator.m_ExplorationButtons[j].IconNumber = GetIconNumber(string.Empty);
			instance.m_StrategicExplorator.m_ExplorationButtons[j].gameObject.SetActive(value: false);
		}
		if (instance.m_StrategicExplorator.m_Image != null)
		{
			if (m_Image >= 0)
			{
				instance.m_StrategicExplorator.m_Image.gameObject.SetActive(m_Image > 0);
				instance.m_StrategicExplorator.m_Image.IconNumber = m_Image;
			}
			else
			{
				instance.m_StrategicExplorator.m_Image.SetRandomImage();
			}
		}
	}

	private static int GetIconNumber(string text_id)
	{
		string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetTextFormat(text_id);
		if (text.StartsWith("m"))
		{
			text = text.Substring(2);
		}
		if (text.StartsWith("g"))
		{
			return 3;
		}
		if (text.StartsWith("i"))
		{
			return 1;
		}
		if (text.StartsWith("p"))
		{
			return 2;
		}
		if (text.StartsWith("b"))
		{
			return 0;
		}
		if (text.StartsWith("c"))
		{
			return 5;
		}
		if (text.StartsWith("x"))
		{
			return 4;
		}
		if (text.StartsWith("t"))
		{
			return 6;
		}
		if (text.StartsWith("n"))
		{
			return 7;
		}
		return 1;
	}

	private static int GetSPIconNumber(string text_id, out int Data1, out int Data2)
	{
		Data1 = -1;
		Data2 = -1;
		string textFormat = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetTextFormat(text_id);
		if (textFormat.Contains("-"))
		{
			return 0;
		}
		if (textFormat.Contains("+"))
		{
			return 1;
		}
		if (textFormat.StartsWith("m1"))
		{
			return 2;
		}
		if (textFormat.StartsWith("m2"))
		{
			return 3;
		}
		if (textFormat.StartsWith("m3"))
		{
			return 4;
		}
		if (textFormat.StartsWith("m4"))
		{
			return 5;
		}
		if (textFormat.StartsWith("m5"))
		{
			return 6;
		}
		if (textFormat.StartsWith("m6"))
		{
			return 7;
		}
		if (textFormat.StartsWith("m7"))
		{
			return 8;
		}
		if (textFormat.StartsWith("m8"))
		{
			return 9;
		}
		if (textFormat.StartsWith("m9"))
		{
			return 10;
		}
		if (textFormat.StartsWith("gd") || textFormat.StartsWith("bd") || textFormat.StartsWith("xd"))
		{
			return GetDecayIcon(textFormat, 2, out Data1, out Data2);
		}
		if (textFormat.StartsWith("d"))
		{
			return GetDecayIcon(textFormat, 1, out Data1, out Data2);
		}
		return 0;
	}

	private static int GetDecayIcon(string DecayStr, int toskip, out int data1, out int data2)
	{
		data1 = 100;
		data2 = 0;
		string text = DecayStr.Substring(toskip);
		int length = IndexOfNum(text);
		string s = text.Substring(0, length);
		int.TryParse(s, out data2);
		if (data2 < 1)
		{
			return 10;
		}
		switch (data2)
		{
		case 1:
		case 2:
		case 3:
			return 20;
		case 4:
		case 5:
		case 6:
			return 21;
		case 7:
		case 8:
		case 9:
			return 22;
		default:
			if (data2 > 9)
			{
				return 23;
			}
			return data2 + 10;
		}
	}

	private static bool ShouldSPIconBeVisible(string text_id)
	{
		string textFormat = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetTextFormat(text_id);
		if (textFormat.StartsWith("d") || textFormat.StartsWith("gd") || textFormat.StartsWith("xd") || textFormat.StartsWith("bd"))
		{
			return true;
		}
		return textFormat.Contains("-") || textFormat.Contains("+") || textFormat.StartsWith("m");
	}

	private static int IndexOfNum(string StrInput)
	{
		if (string.IsNullOrEmpty(StrInput))
		{
			return 0;
		}
		int num = 0;
		do
		{
			switch (StrInput[num])
			{
			default:
				return num;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				break;
			}
			num++;
		}
		while (num < StrInput.Length);
		return num;
	}

	private static bool ShouldOptionBeEnabled(string text_id, ref int req_count, ref string item_id)
	{
		string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetTextFormat(text_id);
		if (text.StartsWith("m"))
		{
			text = text.Substring(2);
		}
		if (text.StartsWith("p"))
		{
			text = text.Substring(1);
			text = text.Trim('+', '-');
			int num = IndexOfNum(text);
			string s = text.Substring(0, num);
			int result = 0;
			int.TryParse(s, out result);
			req_count = result;
			item_id = text.Substring(num);
			return result <= CFGInventory.Item_GetCount(item_id);
		}
		if (text.StartsWith("v"))
		{
			text = text.Substring(1);
			text = text.Trim('+', '-');
			int num2 = IndexOfNum(text);
			string s2 = text.Substring(0, num2);
			int result2 = 0;
			int.TryParse(s2, out result2);
			req_count = result2;
			string variableName = text.Substring(num2);
			CFGVar variable = CFGVariableContainer.Instance.GetVariable(variableName, "scenario");
			if (variable != null)
			{
				return result2 <= (int)variable.Value;
			}
			return true;
		}
		if (text.StartsWith("d"))
		{
			text = text.Substring(1);
			text = text.Trim('+', '-');
			int length = IndexOfNum(text);
			string s3 = text.Substring(0, length);
			int result3 = 0;
			int.TryParse(s3, out result3);
			req_count = result3;
			return true;
		}
		if (text.StartsWith("gd") || text.StartsWith("bd") || text.StartsWith("xd") || text.StartsWith("nd") || text.StartsWith("id"))
		{
			text = text.Substring(2);
			text = text.Trim('+', '-');
			int length2 = IndexOfNum(text);
			string s4 = text.Substring(0, length2);
			int result4 = 0;
			int.TryParse(s4, out result4);
			req_count = result4;
			return true;
		}
		return true;
	}

	public override bool UpdateFlow(float deltaTime)
	{
		return m_Finished;
	}

	public void OnLocationPanelButtonClick(int button_id)
	{
		m_Outputs[button_id].m_HasImpulse = true;
		m_Finished = true;
		if (m_Outputs[button_id].m_Links.Count == 0)
		{
			CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
			if (instance != null && instance.m_StrategicExplorator != null)
			{
				instance.m_StrategicExplorator.HideExplorationWindow();
			}
		}
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (base.OnDeSerialize(_FlowObject))
		{
			m_Active = false;
			return true;
		}
		return false;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ShowLocationPanel);
		flowActionInfo.DisplayName = "Location Panel";
		flowActionInfo.CategoryName = "GUI and HUD";
		return flowActionInfo;
	}
}
