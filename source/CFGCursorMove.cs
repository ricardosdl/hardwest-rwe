using UnityEngine;

public class CFGCursorMove : MonoBehaviour
{
	private enum ECursorMoveType
	{
		AP_1,
		AP_1_ORANGE,
		AP_2
	}

	private Animator m_Animator;

	[SerializeField]
	private float m_1APSpeed = 1f;

	[SerializeField]
	private float m_2APSpeed = 0.7f;

	[SerializeField]
	private GameObject m_1APActionPoints;

	[SerializeField]
	private GameObject m_2APActionPoints;

	[SerializeField]
	private MeshRenderer m_GridHighlight;

	[SerializeField]
	private MeshRenderer m_BoxGlow;

	[Space(20f)]
	[SerializeField]
	private Material m_1APGrid;

	[SerializeField]
	private Material m_1APBox;

	[SerializeField]
	[Space(10f)]
	private Material m_2APGrid;

	[SerializeField]
	private Material m_2APBox;

	private ECursorMoveType m_CurrentState;

	private void Awake()
	{
		m_Animator = base.gameObject.GetComponentInChildren<Animator>();
		UpdateState(_force: true);
	}

	private void Update()
	{
		UpdateState();
	}

	private void UpdateState(bool _force = false)
	{
		CFGCharacter selectedCharacter = CFGSelectionManager.Instance.SelectedCharacter;
		ECursorMoveType newState = ((CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage || (selectedCharacter != null && selectedCharacter.ActionPoints == 1)) ? ECursorMoveType.AP_1_ORANGE : (CFGRangeBorders.s_ShowAp2Region ? ECursorMoveType.AP_2 : ECursorMoveType.AP_1));
		ManageVisualziation(newState, _force);
	}

	private void ManageVisualziation(ECursorMoveType _newState, bool _force)
	{
		if (m_CurrentState != _newState || _force)
		{
			m_CurrentState = _newState;
			switch (m_CurrentState)
			{
			case ECursorMoveType.AP_1:
				Show1AP();
				break;
			case ECursorMoveType.AP_1_ORANGE:
				Show1APOrange();
				break;
			case ECursorMoveType.AP_2:
				Show2AP();
				break;
			}
		}
	}

	private void Show1AP()
	{
		if ((bool)m_GridHighlight && (bool)m_BoxGlow && (bool)m_1APGrid && (bool)m_1APBox)
		{
			m_GridHighlight.material = m_1APGrid;
			m_BoxGlow.material = m_1APBox;
			m_Animator.speed = m_1APSpeed;
			m_1APActionPoints.SetActive(value: true);
			m_2APActionPoints.SetActive(value: false);
		}
		else
		{
			Debug.LogWarning("Something is wrong with 1AP cursor", base.gameObject);
		}
	}

	private void Show1APOrange()
	{
		if ((bool)m_GridHighlight && (bool)m_BoxGlow && (bool)m_2APGrid && (bool)m_2APBox)
		{
			m_GridHighlight.material = m_2APGrid;
			m_BoxGlow.material = m_2APBox;
			m_Animator.speed = m_1APSpeed;
			m_1APActionPoints.SetActive(value: true);
			m_2APActionPoints.SetActive(value: false);
		}
		else
		{
			Debug.LogWarning("Something is wrong with 1AP orange cursor", base.gameObject);
		}
	}

	private void Show2AP()
	{
		if ((bool)m_GridHighlight && (bool)m_BoxGlow && (bool)m_2APGrid && (bool)m_2APBox)
		{
			m_GridHighlight.material = m_2APGrid;
			m_BoxGlow.material = m_2APBox;
			m_Animator.speed = m_2APSpeed;
			m_1APActionPoints.SetActive(value: false);
			m_2APActionPoints.SetActive(value: true);
		}
		else
		{
			Debug.LogWarning("Something is wrong with 2AP cursor", base.gameObject);
		}
	}
}
