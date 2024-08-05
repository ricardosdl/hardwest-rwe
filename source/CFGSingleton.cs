using UnityEngine;

public class CFGSingleton<T> : CFGSingletonBase where T : MonoBehaviour
{
	private const string CLASS_START = "CFG";

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
		if (s_Instance == null)
		{
			s_Instance = Object.FindObjectOfType(typeof(T)) as T;
		}
		if (s_Instance == null)
		{
			string text = typeof(T).Name;
			if (text.StartsWith("CFG"))
			{
				text = text.Substring("CFG".Length);
			}
			GameObject gameObject = new GameObject(text);
			s_Instance = gameObject.AddComponent(typeof(T)) as T;
		}
	}

	public static bool IsInstanceInitialized()
	{
		return s_Instance != null;
	}
}
