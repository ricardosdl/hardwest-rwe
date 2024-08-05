using UnityEngine;

[ExecuteInEditMode]
public class CFGWind : MonoBehaviour
{
	[SerializeField]
	public Vector2 m_windVector = new Vector2(1f, 1f);

	public bool m_UseWindCurve;

	public AnimationCurve m_windCurve = AnimationCurve.EaseInOut(0f, 1f, 50f, 1f);

	[HideInInspector]
	public Vector2 m_WindForce = Vector2.zero;

	private GameObject CFGWindController;

	public static void GlobalLevelCFGWind()
	{
		if (GameObject.Find("CFGWindController") == null)
		{
			GameObject gameObject = new GameObject("CFGWindController");
			gameObject.AddComponent<CFGWind>();
		}
		else
		{
			Debug.Log("Only one CFGWindController is allower per level!");
		}
	}

	private void Start()
	{
		m_windCurve.postWrapMode = WrapMode.Loop;
		m_windCurve.preWrapMode = WrapMode.Loop;
	}

	private void Update()
	{
		SetWind();
	}

	public void SetWind()
	{
		float value = ((!m_UseWindCurve) ? 1f : m_windCurve.Evaluate(Time.time));
		value = Mathf.Clamp(value, 0f, 2f);
		m_windVector.x = Mathf.Clamp(m_windVector.x, -4f, 4f);
		m_windVector.y = Mathf.Clamp(m_windVector.y, -4f, 4f);
		m_WindForce = m_windVector * value;
		Vector3 vector = new Vector3(m_windVector.x, m_windVector.y, value);
		Shader.SetGlobalVector("_WindSpeed", vector);
	}
}
