using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CFGWindowObject : CFGSerializableObject
{
	public Transform m_FxPivot;

	[Range(0f, 1f)]
	public int m_WindowSizeIndex;

	private bool m_isBroken;

	private int m_MaterialNumber;

	private bool m_isMaterialFound;

	private CFGGameplaySettings m_WindowElements;

	private Renderer m_WindowRenderer;

	public override ESerializableType SerializableType => ESerializableType.Window;

	public override bool NeedsSaving => true;

	private void OnTriggerEnter(Collider bullet)
	{
		if (!(bullet.tag == "Bullet"))
		{
		}
	}

	private void Start()
	{
		m_WindowElements = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		m_WindowRenderer = GetComponentInChildren<Renderer>();
		if (m_FxPivot == null)
		{
			m_FxPivot = base.transform.Find("fx_pivot");
			if (m_FxPivot == null)
			{
				Debug.Log("CFGWindowObject: No Fx Pivot assigned! You should assign Fx Pivot for proper effect position. Default transform used.", base.gameObject);
				m_FxPivot = base.transform;
			}
		}
		for (m_MaterialNumber = 0; m_MaterialNumber < m_WindowRenderer.sharedMaterials.Length; m_MaterialNumber++)
		{
			if (m_WindowRenderer.sharedMaterials[m_MaterialNumber].shader == Shader.Find("CFG/Base/Glass"))
			{
				m_isMaterialFound = true;
				break;
			}
			if (m_MaterialNumber == m_WindowRenderer.sharedMaterials.Length && !m_isMaterialFound)
			{
				Debug.Log("CFGWindowObject: No glass material can be found! Assign glass material for broken glass effect!", base.gameObject);
			}
		}
	}

	public void BreakWindow(Vector3 breakingSource, bool silent = false)
	{
		if (m_isBroken)
		{
			return;
		}
		float num = Vector3.Angle(new Vector3(breakingSource.x, 0f, breakingSource.z) - new Vector3(m_FxPivot.transform.position.x, 0f, m_FxPivot.transform.position.z), m_FxPivot.transform.forward);
		Vector3 localEulerAngles = ((!(num <= 90f)) ? Vector3.zero : new Vector3(0f, -180f, 0f));
		ParticleSystem particleSystem = null;
		switch (m_WindowSizeIndex)
		{
		case 0:
			if (m_WindowElements.m_BrokenWindowElements.m_GlassParticle0 == null)
			{
				Debug.LogWarning("CFGWindowObject: There is no glass-break particle system attached in the GameplaySettings");
			}
			else if (!silent)
			{
				particleSystem = Object.Instantiate(m_WindowElements.m_BrokenWindowElements.m_GlassParticle0, m_FxPivot.transform.position, m_FxPivot.transform.rotation) as ParticleSystem;
			}
			break;
		case 1:
			if (m_WindowElements.m_BrokenWindowElements.m_GlassParticle1 == null)
			{
				Debug.LogWarning("CFGWindowObject: There is no glass-break particle system attached in the GameplaySettings");
			}
			else if (!silent)
			{
				particleSystem = Object.Instantiate(m_WindowElements.m_BrokenWindowElements.m_GlassParticle1, m_FxPivot.transform.position, m_FxPivot.transform.rotation) as ParticleSystem;
			}
			break;
		}
		if ((bool)particleSystem)
		{
			particleSystem.transform.SetParent(m_FxPivot);
			particleSystem.transform.localEulerAngles = localEulerAngles;
		}
		if (!silent)
		{
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_WindowBreak, m_FxPivot);
		}
		if (m_isMaterialFound)
		{
			Material[] sharedMaterials = m_WindowRenderer.sharedMaterials;
			sharedMaterials[m_MaterialNumber] = m_WindowElements.m_BrokenWindowElements.m_GlassMaterial0;
			m_WindowRenderer.sharedMaterials = sharedMaterials;
			CFGCellObjectPart component = GetComponent<CFGCellObjectPart>();
			if (component != null)
			{
				component.GenerateMaterialPacks();
			}
		}
		m_isBroken = true;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("State", m_isBroken);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node _Node)
	{
		if (_Node.Attrib_Get("State", m_isBroken))
		{
			BreakWindow(Vector3.zero, silent: true);
		}
		return true;
	}
}
