using System.IO;
using UnityEngine;

public class CFGStoryMovie : MonoBehaviour
{
	public Camera m_Camera;

	public MoviePlayer m_MoviePlayer;

	private CFGSubtitles m_Subtitles = new CFGSubtitles();

	private bool m_IsPlaying;

	private void CancelMovie()
	{
		if (m_MoviePlayer != null)
		{
			m_MoviePlayer.play = false;
		}
		m_Subtitles.Finish(bUnloadScaleform: false);
		m_IsPlaying = false;
		Finish();
	}

	private void Start()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadStrategicExplorator();
		if (m_MoviePlayer == null)
		{
			Finish();
			return;
		}
		if (m_Camera != null)
		{
			float num = (float)Screen.width / (float)Screen.height;
			m_Camera.orthographicSize = 1.7777778f / num * 5f;
		}
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
		string text = ((!cFGSessionSingle.GetMissionStats().scenario_end) ? cFGSessionSingle.ScenarioName : ("outro_0" + cFGSessionSingle.GetMissionStats().video_idx));
		string text2 = "Data/Movies/" + text + ".bytes";
		Debug.Log("Loading movie: " + text2);
		if (!File.Exists(text2))
		{
			Debug.LogWarning("Movie " + text + " not found.");
			Finish();
			return;
		}
		m_MoviePlayer.Load(text2);
		m_Subtitles.Init(text, CFGData.GetDataPathFor("sm_subtitles.tsv", CFGGame.DlcType), UseScaleform: true);
		m_MoviePlayer.play = true;
		m_Subtitles.Begin();
		m_IsPlaying = true;
	}

	private void Update()
	{
		if (m_IsPlaying)
		{
			m_Subtitles.Update();
			if (!m_MoviePlayer.play || Input.GetKeyUp(KeyCode.Escape) || CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
			{
				CancelMovie();
			}
		}
	}

	private void Finish()
	{
		if (m_MoviePlayer != null)
		{
			m_MoviePlayer.Unload();
		}
		CFGSingleton<CFGWindowMgr>.Instance.UnloadSubtitles();
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
		if (cFGSessionSingle != null)
		{
			if (cFGSessionSingle.GetMissionStats().scenario_end)
			{
				CFGGame.s_LoadCreditsFirst = true;
				CFGSingleton<CFGGame>.Instance.DestroySession();
				CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.UnloadingMission);
				Debug.Log("Loading MainMenu Scene");
				Application.LoadLevel("MainMenu");
			}
			else
			{
				cFGSessionSingle.LoadLevel();
			}
		}
	}
}
