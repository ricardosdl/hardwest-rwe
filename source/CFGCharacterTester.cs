using UnityEngine;

public class CFGCharacterTester : MonoBehaviour
{
	public CFGCharacter m_CharacterPrefab;

	public CFGWeaponVisualisation m_OneHandWeaponPrefab;

	public CFGWeaponVisualisation m_TwoHandWeaponPrefab;

	private CFGCharacter m_Character;

	private Animator m_Animator;

	private CFGCharacterAnimator m_CharacterAnimator;

	private CFGWeaponVisualisation m_CurrentWeapon;

	private string[] m_CoverTypes = new string[4] { "None", "Half", "Full L", "Full R" };

	private int m_WeaponType;

	private int m_CurrentCover;

	private float m_Speed;

	private float m_DirectionDiff;

	private bool m_GunpointUser;

	private bool m_GunpointTarget;

	private bool m_Imprisoned;

	private bool m_Woman;

	private string m_PresetIdx = "0";

	private void Awake()
	{
		CFGGame.s_DontInitApp = true;
		if (m_CharacterPrefab != null)
		{
			m_Character = Object.Instantiate(m_CharacterPrefab, Vector3.zero, Quaternion.identity) as CFGCharacter;
			m_Character.enabled = false;
			m_Animator = m_Character.GetComponent<Animator>();
			m_CharacterAnimator = m_Character.GetComponent<CFGCharacterAnimator>();
			Camera.main.transform.parent = m_Character.Transform;
		}
	}

	private void Start()
	{
		if (m_Character != null)
		{
			m_Character.RandomizePartsVisibility();
			m_Character.RandomizePartsColors();
		}
	}

	private void OnGUI()
	{
		GUILayout.Window(0, new Rect((float)Screen.width * 0.01f, (float)Screen.height * 0.01f, 300f, (float)Screen.height - (float)Screen.height * 0.02f), MakeWindow, "Character tester");
	}

	private void MakeWindow(int id)
	{
		if (m_Character == null || m_Animator == null || m_CharacterAnimator == null)
		{
			return;
		}
		if (GUILayout.Button("Reset"))
		{
			m_Character.Transform.position = Vector3.zero;
			m_WeaponType = 0;
			m_CurrentCover = 0;
			m_Speed = 0f;
			m_DirectionDiff = 0f;
			m_Animator.Play("0H_Idle");
			if (m_CurrentWeapon != null)
			{
				m_CurrentWeapon.transform.parent = null;
				Object.Destroy(m_CurrentWeapon.gameObject);
				m_CurrentWeapon = null;
			}
		}
		GUI.enabled = m_Speed == 0f;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Weapon:");
		if (GUILayout.Button("None"))
		{
			m_WeaponType = 0;
			m_CharacterAnimator.PlayWeaponChange0(OnAnimHolster);
		}
		if (GUILayout.Button("1H"))
		{
			m_WeaponType = 1;
			m_CharacterAnimator.PlayWeaponChange1(OnAnimHolster, OnAnimUnholster);
		}
		if (GUILayout.Button("2H"))
		{
			m_WeaponType = 2;
			m_CharacterAnimator.PlayWeaponChange2(OnAnimHolster, OnAnimUnholster);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Cover:");
		m_CurrentCover = GUILayout.Toolbar(m_CurrentCover, m_CoverTypes);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		m_GunpointUser = GUILayout.Toggle(m_GunpointUser, "Gunpoint user");
		m_GunpointTarget = GUILayout.Toggle(m_GunpointTarget, "Gunpoint target");
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		m_Imprisoned = GUILayout.Toggle(m_Imprisoned, "Imprisoned");
		m_Woman = GUILayout.Toggle(m_Woman, "Woman");
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Label("Actions:");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Shoot") && m_WeaponType > 0)
		{
			m_Animator.SetTrigger("Shoot");
		}
		if (GUILayout.Button("Shoot fanning") && m_WeaponType > 0)
		{
			m_Animator.SetTrigger("ShootFanning");
		}
		if (GUILayout.Button("Shoot cone") && m_WeaponType > 0)
		{
			m_Animator.SetTrigger("ShootCone");
		}
		if (GUILayout.Button("Shoot scoped") && m_WeaponType > 0)
		{
			m_Animator.SetTrigger("ShootScoped");
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Reload") && m_WeaponType > 0)
		{
			m_Animator.SetTrigger("Reload");
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Use"))
		{
			m_Animator.SetTrigger("Use");
		}
		if (GUILayout.Button("CreateCover"))
		{
			m_Animator.SetTrigger("CreateCover");
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Consume"))
		{
			m_Animator.SetTrigger("Consume");
		}
		if (GUILayout.Button("Throw1"))
		{
			m_Animator.SetTrigger("Throw1");
		}
		if (GUILayout.Button("Throw2"))
		{
			m_Animator.SetTrigger("Throw2");
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Hit"))
		{
			m_Animator.SetTrigger("Hit");
		}
		if (GUILayout.Button("Death"))
		{
			m_Animator.SetTrigger("Death");
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Ladder up"))
		{
			m_Character.m_Steering.m_SteerData.m_IsOnLadder = true;
			m_CharacterAnimator.PlayLadderUp(OnLadderEnd);
		}
		if (GUILayout.Button("Ladder down"))
		{
			m_Character.m_Steering.m_SteerData.m_IsOnLadder = true;
			m_CharacterAnimator.PlayLadderDown(OnLadderEnd);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Ladder2 up"))
		{
			m_Character.m_Steering.m_SteerData.m_IsOnLadder = true;
			m_CharacterAnimator.PlayLadder2Up(OnLadderEnd);
		}
		if (GUILayout.Button("Ladder2 down"))
		{
			m_Character.m_Steering.m_SteerData.m_IsOnLadder = true;
			m_CharacterAnimator.PlayLadder2Down(OnLadderEnd);
		}
		GUILayout.EndHorizontal();
		GUI.enabled = true;
		GUILayout.Label("Speed: " + m_Speed);
		m_Speed = GUILayout.HorizontalSlider(m_Speed, 0f, 4.2f);
		GUILayout.Label("Direction diff: " + m_DirectionDiff);
		m_DirectionDiff = GUILayout.HorizontalSlider(m_DirectionDiff, -180f, 180f);
		GUILayout.Label("forward: " + m_Character.Transform.forward);
		GUILayout.Label("wanted dir: " + m_CharacterAnimator.WantedDirection);
		GUILayout.Label("angular speed: " + m_CharacterAnimator.AngularSpeed);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("LF"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(-1f, 0f, 1f).normalized;
		}
		if (GUILayout.Button("F"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(0f, 0f, 1f);
		}
		if (GUILayout.Button("RF"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(1f, 0f, 1f).normalized;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("L"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(-1f, 0f, 0f);
		}
		GUILayout.Button(string.Empty);
		if (GUILayout.Button("R"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(1f, 0f, 0f);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("LB"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(-1f, 0f, -1f).normalized;
		}
		if (GUILayout.Button("B"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(0f, 0f, -1f);
		}
		if (GUILayout.Button("RB"))
		{
			m_CharacterAnimator.WantedDirection = new Vector3(1f, 0f, -1f).normalized;
		}
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Randomize parts"))
		{
			m_Character.RandomizePartsVisibility();
		}
		if (GUILayout.Button("Randomize colors"))
		{
			m_Character.RandomizePartsColors();
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Load preset");
		m_PresetIdx = GUILayout.TextField(m_PresetIdx, 2);
		GUI.enabled = int.TryParse(m_PresetIdx, out var result) && m_Character.m_CustomPresets != null && result >= 0 && result < m_Character.m_CustomPresets.Length;
		if (GUILayout.Button("LOAD"))
		{
			m_Character.SetupCustomization(result);
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
	}

	private void Update()
	{
		if (m_Animator != null && m_Character != null)
		{
			m_Animator.SetInteger("Cover", m_CurrentCover);
			m_Character.m_Steering.m_SteerData.m_Speed = m_Speed / 4.2f;
			m_Character.m_Steering.m_SteerData.m_IsMoving = m_Speed > 0f;
			m_Animator.SetFloat("DirectionDiff", m_DirectionDiff);
			if (m_GunpointUser)
			{
				m_Animator.SetInteger("GunpointState", 1);
			}
			else if (m_GunpointTarget)
			{
				m_Animator.SetInteger("GunpointState", 2);
			}
			else
			{
				m_Animator.SetInteger("GunpointState", 0);
			}
			m_Animator.SetBool("Imprisoned", m_Imprisoned);
			m_Animator.SetFloat("Woman", (!m_Woman) ? 0f : 1f);
		}
	}

	private void OnDrawGizmos()
	{
		if (m_Character != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(m_Character.Transform.position, m_CharacterAnimator.WantedDirection);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(m_Character.Transform.position, m_Character.Transform.forward);
		}
	}

	private void OnLadderEnd()
	{
		m_Character.m_Steering.m_SteerData.m_IsOnLadder = false;
	}

	private void OnAnimWeaponChange()
	{
		Debug.Log("OnAnimWeaponChange");
		if (m_CurrentWeapon != null)
		{
			m_CurrentWeapon.transform.parent = null;
			Object.Destroy(m_CurrentWeapon.gameObject);
			m_CurrentWeapon = null;
		}
		if (m_WeaponType > 0)
		{
			CFGWeaponVisualisation cFGWeaponVisualisation = ((m_WeaponType != 1) ? m_TwoHandWeaponPrefab : m_OneHandWeaponPrefab);
			if (cFGWeaponVisualisation != null)
			{
				m_CurrentWeapon = Object.Instantiate(cFGWeaponVisualisation, m_Character.RightHand.position, m_Character.RightHand.rotation) as CFGWeaponVisualisation;
				m_CurrentWeapon.transform.parent = m_Character.RightHand;
			}
		}
	}

	private void OnAnimHolster()
	{
		Debug.Log("OnAnimHolster");
		if (m_CurrentWeapon != null)
		{
			m_CurrentWeapon.transform.parent = null;
			Object.Destroy(m_CurrentWeapon.gameObject);
			m_CurrentWeapon = null;
		}
	}

	private void OnAnimUnholster()
	{
		Debug.Log("OnAnimUnholster");
		if (m_WeaponType > 0)
		{
			CFGWeaponVisualisation cFGWeaponVisualisation = ((m_WeaponType != 1) ? m_TwoHandWeaponPrefab : m_OneHandWeaponPrefab);
			if (cFGWeaponVisualisation != null)
			{
				m_CurrentWeapon = Object.Instantiate(cFGWeaponVisualisation, m_Character.RightHand.position, m_Character.RightHand.rotation) as CFGWeaponVisualisation;
				m_CurrentWeapon.transform.parent = m_Character.RightHand;
			}
		}
	}
}
