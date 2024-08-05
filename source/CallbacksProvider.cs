using System;
using UnityEngine;

[ExecuteInEditMode]
public class CallbacksProvider : MonoBehaviour
{
	public delegate void UpdateDelegate();

	private static CallbacksProvider instance;

	public static CallbacksProvider Instance
	{
		get
		{
			if (instance == null)
			{
				InitInstance();
			}
			return instance;
		}
	}

	public event UpdateDelegate OnUpdate;

	private static void InitInstance()
	{
		if (instance == null)
		{
			instance = UnityEngine.Object.FindObjectOfType<CallbacksProvider>();
		}
		if (instance == null)
		{
			CFGLevelSettings cFGLevelSettings = UnityEngine.Object.FindObjectOfType<CFGLevelSettings>();
			if (cFGLevelSettings != null)
			{
				GameObject gameObject = cFGLevelSettings.gameObject;
				instance = gameObject.AddComponent<CallbacksProvider>();
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.OnUpdate != null)
		{
			this.OnUpdate();
		}
	}

	public bool IsOnUpdateRegistered(UpdateDelegate prospectiveHandler)
	{
		if (this.OnUpdate != null)
		{
			Delegate[] invocationList = this.OnUpdate.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				UpdateDelegate updateDelegate = (UpdateDelegate)invocationList[i];
				if (updateDelegate == prospectiveHandler)
				{
					return true;
				}
			}
		}
		return false;
	}
}
