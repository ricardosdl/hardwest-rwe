using UnityEngine;

public abstract class CFGWindow : MonoBehaviour
{
	private bool m_IsActive;

	private bool m_IsEnabled = true;

	private bool m_JustActivated;

	protected virtual void Update()
	{
		if (IsActive())
		{
			OnUpdate();
		}
	}

	private void OnGUI()
	{
		if (IsActive())
		{
			bool flag = GUI.enabled;
			GUI.enabled = IsEnabled();
			DrawWindow();
			if (m_JustActivated)
			{
				GUI.FocusWindow((int)GetWindowID());
				m_JustActivated = false;
			}
			GUI.enabled = flag;
		}
	}

	public abstract EWindowID GetWindowID();

	public void Activate()
	{
		SetActive(active: true);
	}

	public void Deactivate()
	{
		SetActive(active: false);
	}

	public void ToggleActive()
	{
		SetActive(!IsActive());
	}

	public void SetActive(bool active)
	{
		m_IsActive = active;
		if (active)
		{
			m_JustActivated = true;
			OnActivate();
		}
		else
		{
			OnDeactivate();
		}
	}

	public bool IsActive()
	{
		return m_IsActive;
	}

	public void Enable()
	{
		SetEnabled(enabled: true);
	}

	public void Disable()
	{
		SetEnabled(enabled: false);
	}

	public void SetEnabled(bool enabled)
	{
		m_IsEnabled = enabled;
	}

	public bool IsEnabled()
	{
		return m_IsEnabled;
	}

	protected abstract void OnActivate();

	protected abstract void OnDeactivate();

	protected abstract void OnUpdate();

	protected abstract void DrawWindow();
}
