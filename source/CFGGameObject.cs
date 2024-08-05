using UnityEngine;

public class CFGGameObject : CFGSerializableObject
{
	private Transform m_Transform;

	[SerializeField]
	private string m_NameId = string.Empty;

	[CFGFlowCode(HideSetter = true)]
	public string NameId
	{
		get
		{
			return m_NameId;
		}
		set
		{
			m_NameId = value;
		}
	}

	public Transform Transform
	{
		get
		{
			if (m_Transform == null)
			{
				m_Transform = GetComponent<Transform>();
			}
			return m_Transform;
		}
		private set
		{
			m_Transform = value;
		}
	}

	public bool IsUnderCursor { get; set; }

	public virtual void PrePreUpdateLogic()
	{
	}

	public virtual void PreUpdateLogic()
	{
	}

	public virtual void UpdateLogic()
	{
	}

	protected virtual void Awake()
	{
		Transform = GetComponent<Transform>();
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}

	public virtual void OnCursorEnter()
	{
	}

	public virtual void OnCursorLeave()
	{
	}

	public virtual void StartTurn(CFGOwner owner)
	{
	}

	public virtual void EndTurn(CFGOwner owner)
	{
	}

	public virtual void OnStrategicStart()
	{
	}

	public virtual void OnStrategicEnd()
	{
	}

	public virtual void OnTacticalStart()
	{
	}

	public virtual void OnTacticalEnd()
	{
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Transform = GetComponent<Transform>();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Register(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	private void OnDestroy()
	{
	}
}
