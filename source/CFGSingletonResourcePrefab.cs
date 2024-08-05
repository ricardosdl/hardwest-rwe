using UnityEngine;

public class CFGSingletonResourcePrefab<T> : CFGSingletonBase where T : MonoBehaviour
{
	private const string CLASS_START = "CFG";

	private const string FOLDER = "Prefabs/";

	private static T s_Instance;

	public static T Instance
	{
		get
		{
			if (s_Instance == null)
			{
				InitInstance();
			}
			return s_Instance;
		}
	}

	public static void InitInstance()
	{
		if (!(s_Instance == null))
		{
			return;
		}
		s_Instance = Object.FindObjectOfType(typeof(T)) as T;
		if (s_Instance == null)
		{
			string text = typeof(T).Name;
			if (text.StartsWith("CFG"))
			{
				text = text.Substring("CFG".Length);
			}
			s_Instance = Object.Instantiate(Resources.Load("Prefabs/" + text, typeof(T))) as T;
			if ((bool)s_Instance)
			{
				Object.DontDestroyOnLoad(s_Instance);
				(s_Instance as CFGSingletonBase).Init();
			}
		}
	}
}
