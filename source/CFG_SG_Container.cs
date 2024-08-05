using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ComponentAce.Compression.Libs.zlib;
using UnityEngine;

public class CFG_SG_Container
{
	public const int INVALIDSTRING = -1;

	public const byte SAVEVERSION = 7;

	public const byte DELETEVERSION = 6;

	private CFG_SG_Node m_MainNode;

	private CFG_SG_Node m_Types;

	private Dictionary<int, string> m_Strings = new Dictionary<int, string>();

	private int m_NextNum = 1;

	private Dictionary<Type, int> m_TypeDic = new Dictionary<Type, int>();

	private Dictionary<int, Type> m_TypeDicInv = new Dictionary<int, Type>();

	private int m_TypeDicInt;

	public CFG_SG_Node MainNode => m_MainNode;

	public CFG_SG_Node Types => m_Types;

	public bool IsReset
	{
		get
		{
			if (m_MainNode == null)
			{
				return true;
			}
			return false;
		}
	}

	public Dictionary<Type, int> TypeDict => m_TypeDic;

	public Dictionary<int, Type> InvTypeDict => m_TypeDicInv;

	public void Reset()
	{
		m_MainNode = null;
		m_Types = null;
		m_Strings.Clear();
		m_NextNum = 1;
	}

	public string GetString(int Idx)
	{
		string value = null;
		if (m_Strings.TryGetValue(Idx, out value))
		{
			return value;
		}
		return null;
	}

	public int GetID(string Str)
	{
		if (string.IsNullOrEmpty(Str))
		{
			return -1;
		}
		string text = Str.ToLower();
		foreach (KeyValuePair<int, string> @string in m_Strings)
		{
			if (@string.Value == text)
			{
				return @string.Key;
			}
		}
		return -1;
	}

	public int RegisterString(string Str)
	{
		if (string.IsNullOrEmpty(Str))
		{
			return -1;
		}
		string text = Str.ToLower();
		foreach (KeyValuePair<int, string> @string in m_Strings)
		{
			if (@string.Value == text)
			{
				return @string.Key;
			}
		}
		while (m_Strings.ContainsKey(m_NextNum))
		{
			m_NextNum++;
		}
		m_Strings.Add(m_NextNum, text);
		return m_NextNum;
	}

	public bool CreateContainer(string MainNodeName)
	{
		Reset();
		m_MainNode = new CFG_SG_Node(MainNodeName);
		return true;
	}

	public bool LoadData(string FileName)
	{
		if (string.IsNullOrEmpty(FileName))
		{
			return false;
		}
		if (!File.Exists(FileName))
		{
			return false;
		}
		byte[] array = File.ReadAllBytes(FileName);
		if (array == null || array.Length < 6)
		{
			Debug.LogError("Failed to open file: " + FileName);
			return false;
		}
		EContainerFormat eContainerFormat = EContainerFormat.Xml;
		if (array[1] == 164 && array[2] == 184 && array[3] == 34 && array[0] >= 4)
		{
			eContainerFormat = EContainerFormat.Packed;
		}
		if (array[1] == 82 && array[2] == 31 && array[3] == 195 && array[0] >= 4)
		{
			eContainerFormat = EContainerFormat.Encoded;
		}
		Debug.Log(string.Concat("Preparing saved data: Size: ", array.Length, "; Format: ", eContainerFormat, " Version: ", array[0]));
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		switch (eContainerFormat)
		{
		case EContainerFormat.Encoded:
			array = EncodeArray(array, bSkipFirst: true);
			break;
		case EContainerFormat.Packed:
			array = UnpackArray(array);
			break;
		}
		float realtimeSinceStartup2 = Time.realtimeSinceStartup;
		Debug.Log("Data prepared in " + (realtimeSinceStartup2 - realtimeSinceStartup).ToString() + " s. Size: " + array.Length);
		MemoryStream datastream = new MemoryStream(array, 0, array.Length, writable: false, publiclyVisible: true);
		return ParseXML(datastream, FileName);
	}

	private bool ParseXML(Stream datastream, string FileName)
	{
		m_TypeDicInv = new Dictionary<int, Type>();
		XmlDocument xmlDocument = new XmlDocument();
		if (xmlDocument == null)
		{
			Debug.Log("failed allocate memory");
			return false;
		}
		try
		{
			xmlDocument.Load(datastream);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception: " + ex);
		}
		datastream.Close();
		XmlNode xmlNode = xmlDocument.FirstChild;
		if (xmlNode == null)
		{
			Debug.LogWarning("SaveGame [" + FileName + "] has no nodes");
			return false;
		}
		if (xmlNode.NodeType == XmlNodeType.XmlDeclaration)
		{
			xmlNode = xmlDocument.FirstChild.NextSibling;
		}
		if (xmlNode == null)
		{
			Debug.LogWarning("SaveGame [" + FileName + "] has no nodes");
			return false;
		}
		if (CreateContainer(xmlNode.Name))
		{
			return m_MainNode.Import_XML(xmlNode, bMain: true, this);
		}
		Debug.Log("failed to create main node");
		return false;
	}

	public void SaveData(string FileName, EContainerFormat DataFormat)
	{
		if (m_MainNode == null)
		{
			return;
		}
		if (DataFormat == EContainerFormat.Auto)
		{
			DataFormat = EContainerFormat.Xml;
		}
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
			if (fileStream == null)
			{
				Debug.LogError("Failed to create file: " + FileName);
				return;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to create data document: " + FileName);
			Debug.LogError("Reason: " + ex);
			return;
		}
		MemoryStream memoryStream = new MemoryStream();
		if (memoryStream == null)
		{
			Debug.LogError("Failed to create memory stream for file: " + FileName);
			return;
		}
		WriteXML(memoryStream);
		if (memoryStream.Length != 0L)
		{
			byte[] array = memoryStream.ToArray();
			if (array == null)
			{
				Debug.LogError("Failed to allocate temp array");
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Debug.Log(string.Concat("Processing save data. Format: ", DataFormat, "; Size: (", array.Length, " bytes)"));
			switch (DataFormat)
			{
			case EContainerFormat.Encoded:
				array = EncodeArray(array, bSkipFirst: false);
				break;
			case EContainerFormat.Packed:
				array = PackArray(array);
				break;
			}
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			Debug.Log("Save data prepared in " + (realtimeSinceStartup2 - realtimeSinceStartup).ToString() + "s; Final size is " + array.Length);
			switch (DataFormat)
			{
			case EContainerFormat.Packed:
				fileStream.WriteByte(7);
				fileStream.WriteByte(164);
				fileStream.WriteByte(184);
				fileStream.WriteByte(34);
				break;
			case EContainerFormat.Encoded:
				fileStream.WriteByte(7);
				fileStream.WriteByte(82);
				fileStream.WriteByte(31);
				fileStream.WriteByte(195);
				break;
			}
			fileStream.Write(array, 0, array.Length);
			fileStream.Flush();
			fileStream.Close();
		}
	}

	public static byte[] UnpackArray(byte[] arr)
	{
		if (arr == null || arr.Length == 0)
		{
			return null;
		}
		byte[] array = null;
		byte[] array2 = new byte[2000];
		MemoryStream memoryStream = new MemoryStream();
		ZOutputStream zOutputStream = new ZOutputStream(memoryStream);
		MemoryStream memoryStream2 = new MemoryStream(arr, writable: false);
		try
		{
			memoryStream2.ReadByte();
			memoryStream2.ReadByte();
			memoryStream2.ReadByte();
			memoryStream2.ReadByte();
			int len;
			while ((len = memoryStream2.Read(array2, 0, 2000)) > 0)
			{
				zOutputStream.Write(array2, 0, len);
			}
		}
		finally
		{
			memoryStream.Flush();
			zOutputStream.Close();
			memoryStream2.Close();
			memoryStream.Close();
		}
		return memoryStream.ToArray();
	}

	private byte[] PackArray(byte[] arr)
	{
		if (arr == null || arr.Length == 0)
		{
			return null;
		}
		byte[] array = null;
		byte[] array2 = new byte[2000];
		MemoryStream memoryStream = new MemoryStream();
		ZOutputStream zOutputStream = new ZOutputStream(memoryStream, -1);
		MemoryStream memoryStream2 = new MemoryStream(arr, writable: false);
		try
		{
			int len;
			while ((len = memoryStream2.Read(array2, 0, 2000)) > 0)
			{
				zOutputStream.Write(array2, 0, len);
			}
		}
		finally
		{
			memoryStream.Flush();
			zOutputStream.Close();
			memoryStream2.Close();
			memoryStream.Close();
		}
		return memoryStream.ToArray();
	}

	public static byte[] EncodeArray(byte[] arr, bool bSkipFirst)
	{
		if (arr == null || arr.Length == 0)
		{
			return null;
		}
		int num = arr.Length;
		int num2 = 0;
		if (bSkipFirst)
		{
			num2 = 4;
			num -= 4;
		}
		byte[] array = new byte[num];
		int num3 = 33;
		int num4 = 0;
		for (int i = num2; i < arr.Length; i++)
		{
			array[num4++] = (byte)(arr[i] ^ num3);
			num3++;
			if (num3 > 255)
			{
				num3 = 0;
			}
		}
		return array;
	}

	private void WriteXML(Stream stream)
	{
		m_TypeDicInt = 0;
		m_TypeDic = new Dictionary<Type, int>();
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Indent = true;
		xmlWriterSettings.Encoding = Encoding.UTF8;
		XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);
		if (xmlWriter != null)
		{
			m_MainNode.RegisterTypes(this);
			m_MainNode.Export_XML(string.Empty, xmlWriter, this, bMain: true);
			xmlWriter.Flush();
			xmlWriter.Close();
		}
	}

	public void RegisterTypeID(Type _type)
	{
		if (!m_TypeDic.ContainsKey(_type))
		{
			m_TypeDic.Add(_type, m_TypeDicInt);
			m_TypeDicInt++;
		}
	}

	public void RegisterLoadedType(int ID, string type)
	{
		Type type2 = null;
		type2 = Type.GetType(type);
		if (type2 == null)
		{
			if (string.Compare(type, typeof(Vector3).ToString(), ignoreCase: true) == 0)
			{
				type2 = typeof(Vector3);
			}
			else if (string.Compare(type, typeof(Quaternion).ToString(), ignoreCase: true) == 0)
			{
				type2 = typeof(Quaternion);
			}
		}
		if (type2 == null)
		{
			Debug.Log("Could not parse type: " + type);
		}
		else if (!InvTypeDict.ContainsKey(ID) && type2 != null)
		{
			InvTypeDict.Add(ID, type2);
		}
	}

	public Type GetLoadedTypeID(int ID)
	{
		Type value = null;
		m_TypeDicInv.TryGetValue(ID, out value);
		return value;
	}

	public Type GetTypeID(int ID)
	{
		foreach (KeyValuePair<Type, int> item in m_TypeDic)
		{
			if (item.Value == ID)
			{
				return item.Key;
			}
		}
		return null;
	}

	public int GetTypeIDX(Type _type)
	{
		int value = 0;
		if (m_TypeDic.TryGetValue(_type, out value))
		{
			return value;
		}
		return -1;
	}
}
