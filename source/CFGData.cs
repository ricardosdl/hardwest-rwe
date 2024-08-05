using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CFGData
{
	protected static string WithoutExtension(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return string.Empty;
		}
		string extension = Path.GetExtension(path);
		return path.Substring(0, path.Length - extension.Length).Replace("\\", "/");
	}

	protected static string LoadTextFromResource(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			Debug.LogError("Path cannot be null or empty");
			return string.Empty;
		}
		path = WithoutExtension(path);
		TextAsset textAsset = Resources.Load<TextAsset>(path);
		if (textAsset != null)
		{
			string text = textAsset.text;
			Resources.UnloadAsset(textAsset);
			return text;
		}
		Debug.LogWarning(path + " was not found");
		return string.Empty;
	}

	public static string GetDataPathFor(string fileName, EDLC DlcType = EDLC.None)
	{
		string empty = string.Empty;
		return empty + "Data" + DlcType.GetDataDirExt() + "/" + fileName;
	}

	public static bool Exists(string path)
	{
		return File.Exists(path);
	}

	public static string ReadAllText(string path)
	{
		return (!File.Exists(path)) ? string.Empty : File.ReadAllText(path);
	}

	public static string[] ReadAllTextsFromDirectory(string path, string searchPattern = "")
	{
		string[] array = ((!string.IsNullOrEmpty(searchPattern)) ? Directory.GetFiles(path, searchPattern) : Directory.GetFiles(path));
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string path2 in array2)
		{
			list.Add(File.ReadAllText(path2));
		}
		return list.ToArray();
	}
}
