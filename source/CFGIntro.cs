using UnityEngine;

public class CFGIntro : MonoBehaviour
{
	public Camera m_Camera;

	public MoviePlayer m_MoviePlayer;

	private bool m_IsPlaying;

	private void Start()
	{
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
		string dataPathFor = CFGData.GetDataPathFor("Movies/intro.bytes");
		if (!CFGData.Exists(dataPathFor))
		{
			Debug.LogWarning("Intro movie not found.");
			Finish();
		}
		else
		{
			m_MoviePlayer.Load(dataPathFor);
			Invoke("Intro_Delayed", 1f);
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape) || (m_IsPlaying && !m_MoviePlayer.play) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick1Button8) || Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetKeyDown(KeyCode.Joystick1Button10))
		{
			Finish();
		}
	}

	private void Intro_Delayed()
	{
		m_MoviePlayer.play = true;
		m_IsPlaying = true;
	}

	private void Finish()
	{
		Application.LoadLevel("Loader");
	}
}
