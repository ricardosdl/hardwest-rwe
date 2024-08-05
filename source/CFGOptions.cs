using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class CFGOptions
{
	[INISection(Name = "Graphics")]
	public static class Graphics
	{
		[INIValue(Name = "VSync", DefaultVal = false)]
		public static bool VSync;

		[INIValue(Name = "Aniso", DefaultVal = true)]
		public static bool Aniso = true;

		[INIValue(Name = "TexturesQuality", DefaultVal = 0, MinVal = 0, MaxVal = 2)]
		public static int TexturesQuality;

		[INIValue(Name = "ShadersQuality", DefaultVal = 0, MinVal = 0, MaxVal = 2)]
		public static int ShadersQuality;

		[INIValue(Name = "PostProcessing", DefaultVal = 0, MinVal = 0, MaxVal = 2)]
		public static int PostProcessing;

		[INIValue(Name = "WorldDetails", DefaultVal = 0, MinVal = 0, MaxVal = 2)]
		public static int WorldDetails;

		[INIValue(Name = "WaterReflections", DefaultVal = 0, MinVal = 0, MaxVal = 3)]
		public static int WaterReflections;

		public static void ApplyToUnity()
		{
			QualitySettings.vSyncCount = (VSync ? 1 : 0);
			QualitySettings.masterTextureLimit = TexturesQuality;
			QualitySettings.anisotropicFiltering = (Aniso ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable);
			switch (ShadersQuality)
			{
			case 0:
				Shader.globalMaximumLOD = 1000;
				break;
			case 1:
				Shader.globalMaximumLOD = 510;
				break;
			case 2:
				Shader.globalMaximumLOD = 200;
				break;
			}
		}
	}

	[INISection(Name = "Input")]
	public static class Input
	{
		[INIValue(Name = "GamepadProfile", DefaultVal = 1, MinVal = 1, MaxVal = 4)]
		public static int GamepadProfile = 1;

		[INIValue(Name = "GamepadDeadZone", DefaultVal = 0.2f, MinVal = 0f, MaxVal = 1f)]
		public static float GamepadDZ = 0.2f;

		[INIValue(Name = "GamepadCursorSpeed", DefaultVal = 20f, MinVal = 1f, MaxVal = 20f)]
		public static float GamepadCursorSpeed = 20f;

		[INIValue(Name = "GamepadCameraMoveSpeed", DefaultVal = 20f, MinVal = 1f, MaxVal = 20f)]
		public static float GamepadCameraMoveSpeed = 20f;

		[INIValue(Name = "GamepadCameraRotateSpeed", DefaultVal = 50f, MinVal = 1f, MaxVal = 100f)]
		public static float GamepadCameraRotateSpeed = 50f;

		[INIValue(Name = "AllowLMBOnStrategic", DefaultVal = true)]
		public static bool AllowLMBOnStrategic = true;

		[INIValue(Name = "CenterCameraOnCharSwitch", DefaultVal = true)]
		public static bool CenterCameraOnCharSwitch = true;
	}

	[INISection(Name = "DevOptions")]
	public static class DevOptions
	{
		[INIValue(Name = "RAWPath", DefaultVal = false)]
		public static bool RAWPath;

		[INIValue(Name = "SmoothPathVisualization", DefaultVal = true)]
		public static bool SmoothPathVisualization = true;

		[INIValue(Name = "SuspicionNotifications", DefaultVal = false)]
		public static bool SuspicionNotifications;

		[INIValue(Name = "SetupStateMoveSpeedMul", DefaultVal = 1f, Hidded = true)]
		public static float SetupStateMoveSpeedMul = 1f;

		[INIValue(Name = "EnableDevPanel", DefaultVal = true)]
		public static bool EnableDevPanel = true;

		[INIValue(Name = "AllowCheats", DefaultVal = false)]
		public static bool AllowCheats;

		[INIValue(Name = "TimeScale", DefaultVal = 10f, MinVal = 0.5f, MaxVal = 8f, Hidded = true)]
		public static float TimeScale = 1f;

		[INIValue(Name = "AllowTimeScale", DefaultVal = false, Hidded = true)]
		public static bool AllowTimeScale;
	}

	[INISection(Name = "Audio")]
	public static class Audio
	{
		[INIValue(Name = "Master", DefaultVal = 0f, MinVal = -80f, MaxVal = 0f)]
		public static float Vol_Master;

		[INIValue(Name = "Music", DefaultVal = -15f, MinVal = -80f, MaxVal = 0f)]
		public static float Vol_Music = -15f;

		[INIValue(Name = "Voice", DefaultVal = 0f, MinVal = -80f, MaxVal = 0f)]
		public static float Vol_Voice;

		[INIValue(Name = "Enviro", DefaultVal = 0f, MinVal = -80f, MaxVal = 0f)]
		public static float Vol_Enviro;

		[INIValue(Name = "SFX", DefaultVal = 0f, MinVal = -80f, MaxVal = 0f)]
		public static float Vol_SFX;

		[INIValue(Name = "UserInterface", DefaultVal = -4f, MinVal = -80f, MaxVal = 0f)]
		public static float Vol_UI = -4f;

		[INIValue(Name = "Cinematic", DefaultVal = -6f, MinVal = -80f, MaxVal = 0f)]
		public static float Vol_Cinematic = -6f;

		[INIValue(Name = "MuteMusic", DefaultVal = false)]
		public static bool MuteMusic;

		[INIValue(Name = "MuteSound", DefaultVal = false)]
		public static bool MuteSound;

		public static void ApplyToUnity()
		{
			CFGAudioManager.Instance.MasterVolume = Vol_Master;
			CFGAudioManager.Instance.MusicVolume = Vol_Music;
			CFGAudioManager.Instance.DialogsVolume = Vol_Voice;
			CFGAudioManager.Instance.EnviroVolume = Vol_Enviro;
			CFGAudioManager.Instance.SFXVolume = Vol_SFX;
			CFGAudioManager.Instance.InterfaceVolume = Vol_UI;
			CFGAudioManager.Instance.CinematicVolume = Vol_Cinematic;
		}
	}

	[INISection(Name = "Gameplay")]
	public static class Gameplay
	{
		[INIValue(Name = "Language", DefaultVal = ELanguage.English)]
		public static ELanguage Language;

		[INIValue(Name = "CustomLanguage", DefaultVal = "")]
		public static string CustomLanguage = string.Empty;

		[INIValue(Name = "DisplaySubtitles", DefaultVal = true)]
		public static bool DisplaySubtitles = true;

		[INIValue(Name = "AlwaysRunOnTactical", DefaultVal = false)]
		public static bool AlwaysRunOnTactical;

		[INIValue(Name = "BarterAutoAdjust", DefaultVal = true)]
		public static bool BarterAutoAdjust = true;

		[INIValue(Name = "PawnSuperSpeed", DefaultVal = false)]
		public static bool PawnSuperSpeed;

		[INIValue(Name = "ShowConeOfView", DefaultVal = true)]
		public static bool ShowConeOfView = true;

		[INIValue(Name = "ShowReactionRange", DefaultVal = true)]
		public static bool ShowReactionRange = true;

		[INIValue(Name = "ShowCoverIcons", DefaultVal = true)]
		public static bool ShowCoverIcons = true;

		[INIValue(Name = "ShowFloatingTexts", DefaultVal = true)]
		public static bool ShowFloatingTexts = true;

		[INIValue(Name = "ShowWeaponRange", DefaultVal = true)]
		public static bool ShowWeaponRange = true;

		[INIValue(Name = "InteractiveObjectsGlow", DefaultVal = 3, MinVal = 0, MaxVal = 3)]
		public static int InteractiveObjectsGlow = 3;

		[INIValue(Name = "ExplorationTextSpeed", DefaultVal = 0, MinVal = 0, MaxVal = 2)]
		public static int ExplorationTextSpeed;

		[INIValue(Name = "CameraPanningSpeed", DefaultVal = 1f, MinVal = 0f, MaxVal = 2f)]
		public static float CameraPanningSpeed = 1f;

		[INIValue(Name = "DisableController", DefaultVal = false)]
		public static bool DisableController;

		[INIValue(Name = "QuickSaveEnabled", DefaultVal = false)]
		public static bool QuickSaveEnabled;

		[INIValue(Name = "PathShowDelay", DefaultVal = 0.25f)]
		public static float PathShowDelay = 0.25f;

		[INIValue(Name = "InstantTexts", DefaultVal = false)]
		public static bool InstantTexts;

		[INIValue(Name = "ResetCharQueueOnNewTurn", DefaultVal = true)]
		public static bool ResetCharQueueOnNewTurn = true;

		public static void SelectLanguage(ELanguage NewLanguage, bool bForce)
		{
			if (bForce || NewLanguage != Language)
			{
				Debug.Log("Selecting language: " + NewLanguage);
				Language = NewLanguage;
				CFGSingletonResourcePrefab<CFGTextManager>.Instance.LoadLanguageFiles();
				CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.LoadLanguageFiles();
				CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnLanguageChange(bForce);
			}
		}

		public static void SelectCustomLanguage(string newCustomLanguage, bool bForce)
		{
			if (bForce || !(newCustomLanguage == CustomLanguage))
			{
				Debug.Log("Selecting language: " + newCustomLanguage);
				Language = ELanguage.Custom;
				CustomLanguage = newCustomLanguage;
				CFGSingletonResourcePrefab<CFGTextManager>.Instance.LoadLanguageFiles();
				CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.LoadLanguageFiles();
				CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnLanguageChange(bForce);
			}
		}
	}

	[INISection(Name = "Game")]
	public static class Game
	{
		[INIValue(Name = "DeleteBadSaveGames", DefaultVal = true)]
		public static bool DeleteBadSaveGames = true;

		[INIValue(Name = "SkipDialogs", DefaultVal = false, Hidded = true)]
		public static bool SkipDialogs;
	}

	private class INISection : Attribute
	{
		public string Name = string.Empty;
	}

	private class INIValue : Attribute
	{
		public string Name = string.Empty;

		public object DefaultVal;

		public object MinVal;

		public object MaxVal;

		public bool Hidded;

		public void Clamp(ref object Val)
		{
			IComparable comparable = MinVal as IComparable;
			IComparable comparable2 = MaxVal as IComparable;
			if (Val is IComparable comparable3)
			{
				if (comparable != null && comparable3.CompareTo(comparable) < 0)
				{
					Val = MinVal;
				}
				if (comparable2 != null && comparable3.CompareTo(comparable2) > 0)
				{
					Val = MaxVal;
				}
			}
		}

		public bool IsDefault(ref object Val)
		{
			IComparable comparable = Val as IComparable;
			IComparable comparable2 = DefaultVal as IComparable;
			if (comparable == null || comparable2 == null)
			{
				return false;
			}
			return comparable.CompareTo(comparable2) == 0;
		}
	}

	public const int VERSION_CURRENT = 13;

	private static string m_FileName = string.Empty;

	private static bool m_bDirty;

	public static string FileName
	{
		get
		{
			return m_FileName;
		}
		set
		{
			if (!(m_FileName == value))
			{
				m_FileName = value;
				m_bDirty = true;
			}
		}
	}

	public static bool IsDirty
	{
		get
		{
			return m_bDirty;
		}
		set
		{
			m_bDirty = value;
		}
	}

	public static bool Reset(Type SectionType)
	{
		if (SectionType != null)
		{
			return ResetSection(SectionType);
		}
		Type[] types = Assembly.GetAssembly(typeof(CFGOptions)).GetTypes();
		Type[] array = types;
		foreach (Type sectionType in array)
		{
			ResetSection(sectionType);
		}
		return true;
	}

	private static bool ResetSection(Type SectionType)
	{
		object[] customAttributes = SectionType.GetCustomAttributes(typeof(INISection), inherit: false);
		if (customAttributes == null)
		{
			return false;
		}
		object[] array = customAttributes;
		foreach (object obj in array)
		{
			if (!(obj is INISection))
			{
				continue;
			}
			FieldInfo[] fields = SectionType.GetFields();
			FieldInfo[] array2 = fields;
			foreach (FieldInfo fieldInfo in array2)
			{
				object[] customAttributes2 = fieldInfo.GetCustomAttributes(typeof(INIValue), inherit: false);
				object[] array3 = customAttributes2;
				foreach (object obj2 in array3)
				{
					if (obj2 is INIValue { DefaultVal: var Val } iNIValue)
					{
						iNIValue.Clamp(ref Val);
						SectionType.InvokeMember(fieldInfo.Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.SetField, null, null, new object[1] { Val });
					}
				}
			}
			return true;
		}
		return false;
	}

	public static bool MakeProfileDir()
	{
		try
		{
			Directory.CreateDirectory(CFGApplication.ProfileDir);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to create document folder: " + ex);
			return false;
		}
		return true;
	}

	public static bool Load()
	{
		if (string.IsNullOrEmpty(FileName))
		{
			Debug.LogWarning("CFGOptions: set filename before calling Load()");
			return false;
		}
		if (!File.Exists(FileName))
		{
			Debug.Log("Player Options File does not exists. Creating default");
			Reset(null);
			Save();
			return true;
		}
		Type[] types = Assembly.GetAssembly(typeof(CFGOptions)).GetTypes();
		Type type = null;
		FieldInfo[] array = null;
		int result = 0;
		try
		{
			TextReader textReader = new StreamReader(FileName);
			for (string text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
			{
				text = text.Trim();
				if (text.StartsWith(";v."))
				{
					string s = text.Substring(3, text.Length - 3);
					if (!int.TryParse(s, out result))
					{
					}
				}
				else if (text != string.Empty && !text.StartsWith(";"))
				{
					if (text.StartsWith("[") && text.EndsWith("]"))
					{
						string strB = text.Substring(1, text.Length - 2);
						type = null;
						array = null;
						Type[] array2 = types;
						foreach (Type type2 in array2)
						{
							object[] customAttributes = type2.GetCustomAttributes(typeof(INISection), inherit: false);
							if (customAttributes == null)
							{
								continue;
							}
							object[] array3 = customAttributes;
							foreach (object obj in array3)
							{
								if (obj is INISection iNISection && string.Compare(iNISection.Name, strB, ignoreCase: true) == 0)
								{
									type = type2;
									array = type2.GetFields();
									break;
								}
							}
							if (type != null)
							{
								break;
							}
						}
					}
					else if (type != null && array != null && array.Length > 0)
					{
						string[] array4 = text.Split(new char[1] { '=' }, 2);
						if (array4.Length == 2)
						{
							string text2 = array4[0];
							text2 = text2.Trim();
							FieldInfo[] array5 = array;
							foreach (FieldInfo fieldInfo in array5)
							{
								object[] customAttributes2 = fieldInfo.GetCustomAttributes(typeof(INIValue), inherit: false);
								object[] array6 = customAttributes2;
								foreach (object obj2 in array6)
								{
									if (obj2 is INIValue iNIValue && string.Compare(iNIValue.Name, text2, ignoreCase: true) == 0)
									{
										object Val = ParseObject(array4[1], fieldInfo.FieldType, iNIValue.DefaultVal);
										iNIValue.Clamp(ref Val);
										type.InvokeMember(fieldInfo.Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.SetField, null, null, new object[1] { Val });
									}
								}
							}
						}
						else
						{
							Debug.LogWarning("KeyPair: invalid length: " + array4.Length);
						}
					}
				}
			}
			textReader.Close();
		}
		catch
		{
		}
		Debug.Log("Loaded options file: version: " + result);
		if (result < 13)
		{
			Debug.LogWarning("Player Options File is out of date. Creating default");
			Reset(null);
			Save();
			return true;
		}
		return true;
	}

	public static bool Save()
	{
		if (string.IsNullOrEmpty(FileName))
		{
			Debug.LogWarning("CFGSetup: set filename before calling Save()");
			return false;
		}
		List<string> list = new List<string>();
		list.Add("; Hard West Configuration File");
		list.Add(";v." + 13);
		Type[] types = Assembly.GetAssembly(typeof(CFGOptions)).GetTypes();
		Type[] array = types;
		foreach (Type type in array)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(INISection), inherit: false);
			if (customAttributes == null)
			{
				continue;
			}
			object[] array2 = customAttributes;
			foreach (object obj in array2)
			{
				if (obj is INISection iNISection)
				{
					list.Add("[" + iNISection.Name + "]");
					SaveSection(type, ref list);
					list.Add(string.Empty);
				}
			}
		}
		try
		{
			File.WriteAllLines(FileName, list.ToArray());
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to write options file: " + FileName);
			Debug.LogError("Reason: " + ex);
		}
		return true;
	}

	private static bool SaveSection(Type SectionType, ref List<string> list)
	{
		FieldInfo[] fields = SectionType.GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(INIValue), inherit: false);
			object[] array2 = customAttributes;
			foreach (object obj in array2)
			{
				if (obj is INIValue iNIValue)
				{
					object Val = SectionType.InvokeMember(fieldInfo.Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField, null, null, null);
					iNIValue.Clamp(ref Val);
					if (!iNIValue.Hidded || !iNIValue.IsDefault(ref Val))
					{
						list.Add(iNIValue.Name + " = " + Val);
					}
				}
			}
		}
		return true;
	}

	private static object ParseObject(string Text, Type Tp, object defval)
	{
		if (Tp.IsGenericType)
		{
			Debug.LogError("ParseObject: Generic types are not allowed!");
			return null;
		}
		Text = Text.Trim();
		if (Tp == typeof(string))
		{
			if (Text == null || Text == string.Empty)
			{
				return defval;
			}
			return Text;
		}
		object result = defval;
		try
		{
			if (Tp == typeof(int))
			{
				result = int.Parse(Text);
			}
			else if (Tp == typeof(bool))
			{
				result = bool.Parse(Text);
			}
			else if (Tp == typeof(byte))
			{
				result = byte.Parse(Text);
			}
			else if (Tp == typeof(ushort))
			{
				result = ushort.Parse(Text);
			}
			else if (Tp == typeof(uint))
			{
				result = uint.Parse(Text);
			}
			else if (Tp == typeof(float))
			{
				result = float.Parse(Text);
			}
			else if (Tp.IsEnum)
			{
				result = Enum.Parse(Tp, Text, ignoreCase: true);
			}
		}
		catch
		{
			return defval;
		}
		return result;
	}
}
