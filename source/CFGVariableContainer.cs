using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGVariableContainer
{
	public const string VAR_PATH = "Variables/";

	public const string SCOPES_PATH = "Variables/scopes.tsv";

	public const string SCOPE_PROFILE = "profile";

	public const string SCOPE_CAMPAIGN = "campaign";

	public const string SCOPE_SCENARIO = "scenario";

	public const string SCOPE_LOCALS = "local";

	public const string PROFILE_FILE = "profile.dat";

	private const string NID_MAIN = "variables";

	private const EContainerFormat PROFILE_FORMAT = EContainerFormat.Packed;

	private static CFGVariableContainer _instance;

	private Dictionary<string, CFGScopedVariables> svs = new Dictionary<string, CFGScopedVariables>();

	public List<CFGScopedVariables> locals = new List<CFGScopedVariables>();

	public static CFGVariableContainer Instance => _instance ?? (_instance = new CFGVariableContainer());

	public List<CFGVariableScope> Scopes => svs.Values.Select((CFGScopedVariables x) => x.scope).ToList();

	public List<CFGScopedVariables> ScopedVariables => svs.Values.ToList();

	private CFGVariableContainer()
	{
		_instance = this;
	}

	public bool ContainsDefinition(string variableName, string scopeName)
	{
		if (ContainsScope(scopeName))
		{
			return GetScoped(scopeName).ContainsDefinition(variableName);
		}
		return false;
	}

	public bool ContainsVariable(string variableName, string scopeName)
	{
		if (ContainsScope(scopeName))
		{
			return GetScoped(scopeName).ContainsVariable(variableName);
		}
		return false;
	}

	public bool ContainsScope(string scopeName)
	{
		return svs.ContainsKey(CFGVariableScope.GetID(scopeName));
	}

	public CFGVar GetVariable(string variableName, string scopeName)
	{
		return GetScoped(scopeName)?.GetVariable(variableName);
	}

	public CFGVarDef GetDefiniton(string variableName, string scopeName)
	{
		return GetScoped(scopeName)?.GetDefiniton(variableName);
	}

	public CFGVariableScope GetScope(string scopeName)
	{
		return GetScoped(scopeName)?.scope;
	}

	public CFGScopedVariables GetScoped(string scopeName)
	{
		if (string.IsNullOrEmpty(scopeName))
		{
			Debug.LogError("Scope name can not be null");
			return null;
		}
		svs.TryGetValue(CFGVariableScope.GetID(scopeName), out var value);
		if (value == null)
		{
			Debug.LogError("Could not find scoped variables for: " + scopeName);
		}
		return value;
	}

	public void ClearScope(string scopeID)
	{
		GetScoped(scopeID).Clear();
	}

	public bool RemoveScoped(CFGVariableScope scope)
	{
		if (scope == null || svs == null)
		{
			return false;
		}
		return svs.Remove(scope.ID);
	}

	private void AddScoped(CFGVariableScope scope)
	{
		CFGScopedVariables value = new CFGScopedVariables(scope);
		if (svs.ContainsKey(scope.ID))
		{
			Debug.LogError("ScopedVariablesContainer already contains scope: " + scope.name);
		}
		else
		{
			svs.Add(scope.ID, value);
		}
	}

	private void Clear(bool withoutLocals = true)
	{
		foreach (CFGScopedVariables value in svs.Values)
		{
			bool flag = true;
			if (withoutLocals && locals.Contains(value))
			{
				flag = false;
			}
			if (flag)
			{
				value.Clear();
			}
		}
		svs.Clear();
	}

	public bool LoadLocals()
	{
		CFGVariablesSubContainer[] array = UnityEngine.Object.FindObjectsOfType<CFGVariablesSubContainer>();
		CFGVariablesSubContainer[] array2 = array;
		foreach (CFGVariablesSubContainer cFGVariablesSubContainer in array2)
		{
			cFGVariablesSubContainer.RemoveInvalidDefinitions();
			CFGScopedVariables scoped = cFGVariablesSubContainer.scoped;
			if (scoped != null && !string.IsNullOrEmpty(scoped.scope.name))
			{
				if (!svs.ContainsKey(scoped.scope.ID))
				{
					svs.Add(scoped.scope.ID, scoped);
					locals.Add(scoped);
				}
			}
			else
			{
				Debug.LogError("CFGVariableContainer:LoadLocals() -> container was invalid");
			}
		}
		return true;
	}

	public void UnloadLocals()
	{
		foreach (CFGScopedVariables local in locals)
		{
			svs.Remove(local.scope.ID);
		}
		locals.Clear();
	}

	public void Init()
	{
		Debug.Log("Initializing variable container");
		if (!ReadScopeDefinitions(CFGData.GetDataPathFor("Variables/scopes.tsv")))
		{
			return;
		}
		ReadVariableDefinitions(CFGData.GetDataPathFor("Variables/vars_profile.tsv"));
		foreach (CFGScopedVariables value in svs.Values)
		{
			value.ResetVariables();
		}
	}

	private bool ReadScopeDefinitions(string path)
	{
		Clear();
		string text = CFGData.ReadAllText(path);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - {path} does not exist or is empty");
			return false;
		}
		string[] array = text.ToLines();
		for (int i = 1; i < array.Length; i++)
		{
			string text2 = array[i];
			string[] array2 = text2.Split('\t');
			if (array2.Length < 2)
			{
				Debug.LogWarning("CFGVariableContainer::ReadScopeDefinitions() -> [Row: " + i + "] inusufficent data -> ignoring");
				continue;
			}
			string text3 = array2[0];
			if (string.IsNullOrEmpty(text3))
			{
				Debug.LogWarning("CFGVariableContainer::ReadScopeDefinitions() -> [Row: " + i + "] empty scope name");
				continue;
			}
			int result = 0;
			if (!int.TryParse(array2[1], out result))
			{
				Debug.LogWarning("CFGVariableContainer::ReadScopeDefinitions() -> [Row: " + i + "] error while parsing scope level from: " + array2[1]);
			}
			CFGVariableScope scope = new CFGVariableScope(text3, result);
			AddScoped(scope);
		}
		return true;
	}

	public bool ReadVariableDefinitions(string path)
	{
		string text = CFGData.ReadAllText(path);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		string[] array = text.ToLines();
		for (int i = 1; i < array.Length; i++)
		{
			string text2 = array[i];
			string[] array2 = text2.Split('\t');
			if (array2.Length < 4)
			{
				Debug.LogWarning("CFGVariableContainer::ReadVariableDefinitions() ->[" + path + "]:[Row: " + i + "] inusufficent data -> ignoring");
				continue;
			}
			string text3 = array2[0];
			string text4 = array2[1];
			if (string.IsNullOrEmpty(text4))
			{
				Debug.LogWarning("CFGVariableContainer::ReadVariableDefinitions() ->[" + path + "]:[Row: " + i + "] variable name is empty -> ignoring");
				continue;
			}
			string text5 = array2[2];
			if (text5 == "float")
			{
				array2[3] = array2[3].Replace(',', '.');
			}
			object value = array2[3];
			float result2;
			bool result3;
			if (int.TryParse(array2[3], out var result))
			{
				value = result;
			}
			else if (float.TryParse(array2[3], out result2))
			{
				value = result2;
			}
			else if (bool.TryParse(array2[3], out result3))
			{
				value = result3;
			}
			if (ContainsScope(text3))
			{
				CFGScopedVariables scoped = GetScoped(text3);
				CFGVarDef cFGVarDef = CFGVarDef.Create(text4, text5, value);
				if (array2.Length >= 5)
				{
					bool result4 = false;
					if (bool.TryParse(array2[4], out result4))
					{
						cFGVarDef.readOnly = result4;
					}
				}
				scoped.AddDefinition(cFGVarDef);
			}
			else
			{
				Debug.LogWarning("CFGVariableContainer::ReadVariableDefinitions() -> [Row: " + i + "] scope does not exist: " + text3);
			}
		}
		return true;
	}

	public bool SetAndSaveProfileValue(string variableId, object value)
	{
		CFGVar variable = GetVariable(variableId, "profile");
		if (variable == null)
		{
			return false;
		}
		variable.Value = value;
		SaveValuesGlobal(null);
		return true;
	}

	private bool SerializeScope(string ScopeName, CFG_SG_Node SG)
	{
		if (SG == null || string.IsNullOrEmpty(ScopeName))
		{
			return false;
		}
		CFGScopedVariables scoped = GetScoped(ScopeName);
		if (scoped == null)
		{
			return true;
		}
		CFG_SG_Node cFG_SG_Node = SG.AddSubNode("Scope");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Name", ScopeName);
		if (!scoped.OnSerialize(cFG_SG_Node))
		{
			return false;
		}
		return true;
	}

	private bool DeserializeScope(string ScopeName, CFG_SG_Node SG)
	{
		if (SG == null || string.IsNullOrEmpty(ScopeName))
		{
			return false;
		}
		CFGScopedVariables scoped = GetScoped(ScopeName);
		if (scoped == null)
		{
			Debug.LogWarning("Failed to find scope: " + ScopeName);
			return false;
		}
		for (int i = 0; i < SG.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = SG.GetSubNode(i);
			if (subNode != null && string.Compare(subNode.Name, "Scope", ignoreCase: true) == 0)
			{
				string text = subNode.Attrib_Get<string>("Name", null);
				if (text != null && string.Compare(text, ScopeName, ignoreCase: true) == 0)
				{
					return scoped.OnDeserialize(subNode);
				}
			}
		}
		Debug.LogWarning("Failed to find scope: " + ScopeName);
		return false;
	}

	public void SaveValuesGlobal(string CampaignID)
	{
		CFG_SG_Container cFG_SG_Container = new CFG_SG_Container();
		if (cFG_SG_Container == null)
		{
			return;
		}
		if (!cFG_SG_Container.LoadData(CFGApplication.ProfileDir + "profile.dat"))
		{
			Debug.Log("Failed to load profile data");
		}
		if (cFG_SG_Container.MainNode == null || !string.Equals(cFG_SG_Container.MainNode.Name, "variables", StringComparison.OrdinalIgnoreCase))
		{
			Debug.Log("No variables node in profile file");
			if (!cFG_SG_Container.CreateContainer("variables"))
			{
				return;
			}
		}
		CFG_SG_Node mainNode = cFG_SG_Container.MainNode;
		if (mainNode == null)
		{
			return;
		}
		cFG_SG_Container.MainNode.DeleteSubNode("profile");
		CFG_SG_Node cFG_SG_Node = cFG_SG_Container.MainNode.AddSubNode("profile");
		if (cFG_SG_Node == null || !SerializeScope("profile", cFG_SG_Node))
		{
			return;
		}
		if (CampaignID != null)
		{
			cFG_SG_Container.MainNode.DeleteSubNode(CampaignID);
			CFG_SG_Node cFG_SG_Node2 = cFG_SG_Container.MainNode.AddSubNode(CampaignID);
			if (cFG_SG_Node2 == null || !SerializeScope("campaign", cFG_SG_Node2))
			{
				return;
			}
		}
		cFG_SG_Container.SaveData(CFGApplication.ProfileDir + "profile.dat", EContainerFormat.Packed);
	}

	public void LoadValuesGlobal(string CampaignID, bool bCampaign, bool bProfile)
	{
		Debug.Log("Loading global values");
		CFG_SG_Container cFG_SG_Container = new CFG_SG_Container();
		if (cFG_SG_Container == null)
		{
			return;
		}
		if (!cFG_SG_Container.LoadData(CFGApplication.ProfileDir + "profile.dat"))
		{
			Debug.Log("No profile file.");
		}
		else
		{
			if (cFG_SG_Container.MainNode == null)
			{
				return;
			}
			if (bProfile)
			{
				Debug.Log("Loading profile scope: ");
				CFG_SG_Node cFG_SG_Node = cFG_SG_Container.MainNode.FindSubNode("profile");
				if (cFG_SG_Node != null)
				{
					DeserializeScope("profile", cFG_SG_Node);
				}
				else
				{
					Debug.LogWarning("Failed to find node: profile");
				}
			}
			if (bCampaign)
			{
				Debug.Log("Loading campaign scope: " + CampaignID);
				CFG_SG_Node cFG_SG_Node2 = cFG_SG_Container.MainNode.FindSubNode(CampaignID);
				if (cFG_SG_Node2 != null)
				{
					DeserializeScope("campaign", cFG_SG_Node2);
				}
			}
		}
	}

	public bool OnSerialize(CFG_SG_Node SG)
	{
		if (svs == null)
		{
			return true;
		}
		SerializeScope("scenario", SG);
		return SerializeScope("local", SG);
	}

	public bool OnDeserialize(CFG_SG_Node SG)
	{
		if (SG == null || svs == null)
		{
			Debug.LogWarning("variables has not been deserialized");
			return true;
		}
		return DeserializeScope("scenario", SG);
	}

	public bool OnDeserializeLocals(CFG_SG_Node SG)
	{
		if (SG == null || svs == null)
		{
			Debug.LogWarning("variables has not been deserialized");
			return true;
		}
		return DeserializeScope("local", SG);
	}
}
