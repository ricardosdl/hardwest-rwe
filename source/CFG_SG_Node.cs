using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CFG_SG_Node
{
	private Dictionary<string, CFG_SG_Value> m_Attribs = new Dictionary<string, CFG_SG_Value>();

	private List<CFG_SG_Node> m_Children = new List<CFG_SG_Node>();

	private string m_NameID = string.Empty;

	public string Name => m_NameID;

	public int SubNodeCount => m_Children.Count;

	public CFG_SG_Node(string NodeID)
	{
		m_NameID = NodeID.ToLower();
	}

	public CFG_SG_Node GetSubNode(int IDX)
	{
		if (IDX >= m_Children.Count)
		{
			return null;
		}
		return m_Children[IDX];
	}

	public CFG_SG_Node AddSubNode(string NodeName)
	{
		if (string.IsNullOrEmpty(NodeName))
		{
			return null;
		}
		CFG_SG_Node cFG_SG_Node = null;
		string nodeID = NodeName.ToLower();
		cFG_SG_Node = new CFG_SG_Node(nodeID);
		if (cFG_SG_Node == null)
		{
			return null;
		}
		m_Children.Add(cFG_SG_Node);
		return cFG_SG_Node;
	}

	public CFG_SG_Node FindSubNode(string NodeName, int FirstItem = 0)
	{
		if (FirstItem >= m_Children.Count || string.IsNullOrEmpty(NodeName))
		{
			return null;
		}
		for (int i = FirstItem; i < m_Children.Count; i++)
		{
			if (string.Compare(m_Children[i].Name, NodeName, ignoreCase: true) == 0)
			{
				return m_Children[i];
			}
		}
		return null;
	}

	public CFG_SG_Node FindOrCreateSubNode(string NodeName)
	{
		string text = NodeName.ToLower();
		for (int i = 0; i < m_Children.Count; i++)
		{
			if (m_Children[i].Name == text)
			{
				return m_Children[i];
			}
		}
		return AddSubNode(NodeName);
	}

	public bool CreateCloneNode(CFG_SG_Node SrcNode, string NodeName)
	{
		if (SrcNode == null || string.IsNullOrEmpty(NodeName))
		{
			return false;
		}
		DeleteSubNode(NodeName);
		m_Children.Add(SrcNode);
		return true;
	}

	private bool CopyNode(CFG_SG_Node SrcNode, CFG_SG_Node DestNode)
	{
		for (int i = 0; i < SrcNode.m_Children.Count; i++)
		{
			CFG_SG_Node cFG_SG_Node = DestNode.AddSubNode(SrcNode.m_Children[i].Name);
			if (cFG_SG_Node == null)
			{
				return false;
			}
			CopyNode(SrcNode.m_Children[i], cFG_SG_Node);
		}
		return true;
	}

	public int DeleteSubNode(string NodeName)
	{
		int num = 0;
		int num2 = 0;
		string text = NodeName.ToLower();
		while (num2 < m_Children.Count)
		{
			if (m_Children[num2] == null || m_Children[num2].m_NameID == text)
			{
				m_Children.RemoveAt(num2);
				num++;
			}
			else
			{
				num2++;
			}
		}
		return num;
	}

	public bool AttribExists(string AttName)
	{
		if (m_Attribs == null || string.IsNullOrEmpty(AttName))
		{
			return false;
		}
		string key = AttName.ToLower();
		return m_Attribs.ContainsKey(key);
	}

	public void Attrib_Set<T>(string AttName, T Value)
	{
		string key = AttName.ToLower();
		CFG_SG_Value.eSG_ValueType type = CFG_SG_Value.GetType<T>(bShowMsg: true);
		if (type == CFG_SG_Value.eSG_ValueType.Unknown)
		{
			return;
		}
		CFG_SG_Value value = null;
		if (m_Attribs.TryGetValue(key, out value))
		{
			if (value.ValueType != type)
			{
				Debug.LogWarning("Attrib with name " + AttName + " is already present, but with different type!");
				return;
			}
		}
		else
		{
			value = CFG_SG_Value.Create(type);
		}
		if (value != null)
		{
			value.Set<T>(Value);
			m_Attribs.Add(key, value);
		}
	}

	public void Attrib_Add(string AttName, Type _Type, string val)
	{
		if (_Type == null)
		{
			Debug.Log("Null type: " + AttName);
			return;
		}
		string key = AttName.ToLower();
		if (m_Attribs.ContainsKey(key))
		{
			Debug.LogWarning("Attrib with name " + AttName + " is already set!");
			return;
		}
		CFG_SG_Value cFG_SG_Value = CFG_SG_Value.Create(_Type);
		if (cFG_SG_Value != null)
		{
			cFG_SG_Value.FromString(val, _Type);
			m_Attribs.Add(key, cFG_SG_Value);
		}
	}

	public void Serialize<T>(bool IsWriting, string AttName, ref T Value)
	{
		string key = AttName.ToLower();
		CFG_SG_Value value = null;
		CFG_SG_Value.eSG_ValueType type = CFG_SG_Value.GetType<T>(bShowMsg: true);
		if (type == CFG_SG_Value.eSG_ValueType.Unknown)
		{
			return;
		}
		if (IsWriting)
		{
			if (m_Attribs.TryGetValue(key, out value))
			{
				if (value.ValueType == type)
				{
					value.Set<T>(Value);
				}
				else
				{
					Debug.LogWarning("Value : " + AttName + " is already present but with different type!");
				}
				return;
			}
			value = CFG_SG_Value.Create(type);
			if (value != null)
			{
				value.Set<T>(Value);
				m_Attribs.Add(key, value);
			}
		}
		else if (m_Attribs.TryGetValue(key, out value))
		{
			Value = (T)value.GenericValue;
		}
	}

	public T Attrib_Get<T>(string AttName, T DefVal, bool bReport = true)
	{
		string text = AttName.ToLower();
		CFG_SG_Value value = null;
		CFG_SG_Value.eSG_ValueType type = CFG_SG_Value.GetType<T>();
		if (type == CFG_SG_Value.eSG_ValueType.Unknown)
		{
			Debug.LogWarning("Failed to determine attrib type: " + text);
			return DefVal;
		}
		if (!m_Attribs.TryGetValue(text, out value))
		{
			if (bReport)
			{
				Debug.LogWarning("Failed to find attrib: " + text);
			}
			return DefVal;
		}
		if (value != null && value.ValueType == type)
		{
			return (T)value.GenericValue;
		}
		Debug.LogWarning("wrong type : " + text + " " + value.ValueType);
		return DefVal;
	}

	public bool Attrib_Get<T>(string AttName, T DefVal, out T Val)
	{
		Val = DefVal;
		string text = AttName.ToLower();
		CFG_SG_Value value = null;
		CFG_SG_Value.eSG_ValueType type = CFG_SG_Value.GetType<T>();
		if (type == CFG_SG_Value.eSG_ValueType.Unknown)
		{
			return false;
		}
		if (!m_Attribs.TryGetValue(text, out value))
		{
			return false;
		}
		if (value != null && value.ValueType == type)
		{
			Val = (T)value.GenericValue;
			return true;
		}
		Debug.LogWarning("wrong type : " + text + " " + value.ValueType);
		return false;
	}

	public bool Attrib_Get<T>(string AttName, ref T _Value, bool bReportMissing = true)
	{
		string key = AttName.ToLower();
		CFG_SG_Value value = null;
		CFG_SG_Value.eSG_ValueType type = CFG_SG_Value.GetType<T>();
		if (type == CFG_SG_Value.eSG_ValueType.Unknown)
		{
			if (bReportMissing)
			{
				Debug.LogWarning("Undefined attribute's type: " + AttName + " in node: " + Name);
			}
			return false;
		}
		if (!m_Attribs.TryGetValue(key, out value) || value == null)
		{
			if (bReportMissing)
			{
				Debug.LogWarning("Missing attribute: " + AttName + " in node: " + Name);
			}
			return false;
		}
		if (value.ValueType != type)
		{
			if (bReportMissing)
			{
				Debug.LogWarning(string.Concat("Value type missmatch for attribute: ", AttName, " in node: ", Name, " Type: ", value.ValueType, " requested type: ", type));
			}
			return false;
		}
		_Value = (T)value.GenericValue;
		return true;
	}

	public bool Attrib_GetWithType(string AttName, Type _Type, ref object _Value, bool bReportMissing = true)
	{
		string key = AttName.ToLower();
		CFG_SG_Value value = null;
		CFG_SG_Value.eSG_ValueType type = CFG_SG_Value.GetType(_Type);
		if (type == CFG_SG_Value.eSG_ValueType.Unknown)
		{
			if (bReportMissing)
			{
				Debug.LogWarning("Undefined attribute's type: " + AttName + " in node: " + Name);
			}
			return false;
		}
		if (!m_Attribs.TryGetValue(key, out value) || value == null)
		{
			if (bReportMissing)
			{
				Debug.LogWarning("Missing attribute: " + AttName + " in node: " + Name);
			}
			return false;
		}
		if (value.ValueType != type)
		{
			if (bReportMissing)
			{
				Debug.LogWarning(string.Concat("Value type missmatch for attribute: ", AttName, " in node: ", Name, " Type: ", value.ValueType, " requested type: ", type));
			}
			return false;
		}
		_Value = value.GenericValue;
		return true;
	}

	public void Export_XML(string spaces, XmlWriter sw, CFG_SG_Container cont, bool bMain)
	{
		if (bMain)
		{
		}
		sw.WriteStartElement(m_NameID);
		if (bMain)
		{
			foreach (KeyValuePair<Type, int> item in cont.TypeDict)
			{
				sw.WriteAttributeString("_" + item.Value, item.Key.ToString());
			}
		}
		if (m_Attribs.Count > 0)
		{
			foreach (KeyValuePair<string, CFG_SG_Value> attrib in m_Attribs)
			{
				sw.WriteAttributeString(attrib.Key.ToString() + "_" + cont.GetTypeIDX(attrib.Value.ObjectType), attrib.Value.ToString());
			}
		}
		if (m_Children.Count == 0)
		{
			if (m_Attribs.Count < 18)
			{
				sw.WriteEndElement();
			}
			else
			{
				sw.WriteFullEndElement();
			}
			return;
		}
		foreach (CFG_SG_Node child in m_Children)
		{
			child.Export_XML(spaces + " ", sw, cont, bMain: false);
		}
		sw.WriteFullEndElement();
	}

	public void RegisterTypes(CFG_SG_Container cont)
	{
		foreach (KeyValuePair<string, CFG_SG_Value> attrib in m_Attribs)
		{
			cont.RegisterTypeID(attrib.Value.ObjectType);
		}
		foreach (CFG_SG_Node child in m_Children)
		{
			child.RegisterTypes(cont);
		}
	}

	public bool Import_XML(XmlNode node, bool bMain, CFG_SG_Container cont)
	{
		if (node.Attributes != null)
		{
			foreach (XmlAttribute attribute in node.Attributes)
			{
				if (bMain && attribute.Name[0] == '_')
				{
					string s = attribute.Name.Remove(0, 1);
					int iD = int.Parse(s);
					cont.RegisterLoadedType(iD, attribute.Value);
					continue;
				}
				int num = attribute.Name.LastIndexOf('_');
				if (num > 0)
				{
					string text = attribute.Name.Substring(0, num);
					string s2 = attribute.Name.Substring(num + 1, attribute.Name.Length - num - 1);
					int iD2 = int.Parse(s2);
					Type loadedTypeID = cont.GetLoadedTypeID(iD2);
					if (loadedTypeID == null)
					{
						Debug.LogError("Unknown type for: " + text);
					}
					else
					{
						Attrib_Add(text, loadedTypeID, attribute.Value);
					}
				}
			}
		}
		if (node.ChildNodes != null)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				AddSubNode(childNode.Name)?.Import_XML(childNode, bMain: false, cont);
			}
		}
		return true;
	}
}
