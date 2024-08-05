using System.Runtime.InteropServices;
using UnityEngine;

public class CFGShootableObject : CFGGameObject, CFGIAttackable
{
	public delegate void OnShootableKilled(CFGCharacter Character, CFGShootableObject Shootable);

	[CFGFlowCode(Category = "Shootable", Title = "On Shootable Killed")]
	public OnShootableKilled m_OnKilledCallback;

	private int m_Hp = 1;

	private CFGCell m_Cell;

	[SerializeField]
	private int m_Luck = 70;

	[SerializeField]
	private bool m_Invulnerable;

	[SerializeField]
	private Transform m_Visualization;

	[SerializeField]
	private Transform m_DestroyedVisualization;

	private Transform m_DestroyedVisualizationInstance;

	[SerializeField]
	private float m_SpawnDelay;

	private float m_DestroyTime = -1f;

	[SerializeField]
	private CFGSoundDef m_OnHitSoundDef;

	[SerializeField]
	private Transform m_FXPrefab_OnDestroy;

	[CFGFlowCode]
	public bool IsInvulnerable
	{
		get
		{
			return m_Invulnerable;
		}
		set
		{
			m_Invulnerable = value;
		}
	}

	[CFGFlowCode]
	public int Luck
	{
		get
		{
			return m_Luck;
		}
		set
		{
			m_Luck = value;
		}
	}

	public int MaxLuck => m_Luck;

	public CFGCell CurrentCell
	{
		get
		{
			if (m_Cell == null)
			{
				m_Cell = CFGCellMap.GetCell(base.transform.position);
			}
			return m_Cell;
		}
	}

	public bool IsDodging => false;

	public bool IsCorpseLooted => true;

	public int Hp => m_Hp;

	public int MaxHp => 1;

	public bool IsAlive => m_Hp > 0;

	public int BuffedDefense => 0;

	public bool IsInShadow => false;

	public Vector3 Position => base.transform.position;

	public Quaternion Rotation => base.transform.rotation;

	public override ESerializableType SerializableType => ESerializableType.Destroyable;

	public override bool NeedsSaving => true;

	public void SetHp(int hp)
	{
	}

	public int GetCoverMult()
	{
		return 0;
	}

	public int GetAimBonus()
	{
		return 20;
	}

	protected override void Start()
	{
		base.Start();
		m_Cell = CFGCellMap.GetCell(base.transform.position);
		if (CFGOptions.Gameplay.InteractiveObjectsGlow >= 3)
		{
			base.gameObject.AddComponent<CFGInteractiveObjVis>();
		}
	}

	public ECoverType GetCoverState()
	{
		return ECoverType.NONE;
	}

	public bool TakeDamage(int dmg, CFGCharacter damage_giver, bool bSilent, [Optional] Vector3 recoilDir)
	{
		if (!IsAlive || m_Invulnerable)
		{
			return true;
		}
		if (dmg < 1)
		{
			return false;
		}
		m_Hp = 0;
		if (!bSilent)
		{
			CFGSoundDef.Play(m_OnHitSoundDef, base.Transform.position);
			if (m_FXPrefab_OnDestroy != null)
			{
				Object.Instantiate(m_FXPrefab_OnDestroy, base.transform.position, base.transform.rotation);
			}
		}
		if (m_OnKilledCallback != null)
		{
			m_OnKilledCallback(damage_giver, this);
		}
		m_DestroyTime = Time.timeSinceLevelLoad;
		return true;
	}

	protected override void Update()
	{
		if (!IsAlive && m_DestroyTime > 0f && m_DestroyTime + m_SpawnDelay < Time.timeSinceLevelLoad)
		{
			SwitchVisualizationToDestroyed();
		}
	}

	private void SwitchVisualizationToDestroyed()
	{
		if (m_Visualization != null)
		{
			m_Visualization.gameObject.SetActive(value: false);
		}
		if (m_DestroyedVisualization != null)
		{
			m_DestroyedVisualizationInstance = Object.Instantiate(m_DestroyedVisualization, Vector3.zero, Quaternion.identity) as Transform;
			if (m_DestroyedVisualizationInstance != null)
			{
				m_DestroyedVisualizationInstance.parent = base.transform;
				m_DestroyedVisualizationInstance.transform.localPosition = Vector3.zero;
				m_DestroyedVisualizationInstance.transform.localRotation = Quaternion.identity;
			}
		}
		m_DestroyTime = -1f;
	}

	public Transform GetDamagePivot()
	{
		return base.transform;
	}

	public CFGOwner GetOwner()
	{
		return null;
	}

	public void ApplyBuffAction(CFGDef_UsableItem.eActionType Action, string Param, CFGCharacter caster)
	{
		int num = 0;
		if (Param != null)
		{
			try
			{
				num = int.Parse(Param);
			}
			catch
			{
			}
		}
		switch (Action)
		{
		case CFGDef_UsableItem.eActionType.HP_Mod:
			if (num < 0)
			{
				TakeDamage(-num, null, bSilent: false);
			}
			break;
		case CFGDef_UsableItem.eActionType.AddBuff:
		{
			CFGDef_Buff buff = CFGStaticDataContainer.GetBuff(Param);
			if (buff != null && buff.HPChange <= -1)
			{
				TakeDamage(-buff.HPChange, null, bSilent: false);
			}
			break;
		}
		case CFGDef_UsableItem.eActionType.RemoveBuff:
		case CFGDef_UsableItem.eActionType.RemoveAllBuffs:
			break;
		}
	}

	public override bool OnSerialize(CFG_SG_Node ParentNode)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(ParentNode);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("HP", m_Hp);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node Node)
	{
		if (Node == null)
		{
			return false;
		}
		m_Hp = Node.Attrib_Get("HP", 1);
		if (m_Hp == 0)
		{
			SwitchVisualizationToDestroyed();
		}
		return false;
	}

	virtual string CFGIAttackable.get_NameId()
	{
		return base.NameId;
	}
}
