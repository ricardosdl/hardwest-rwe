using System.Collections;
using UnityEngine;

public class CFGSpawnableVFX : CFGSerializableObject
{
	public bool m_PureParticleSystem;

	private bool m_Stopped;

	[Header("Used only if VFX is not PureParticleSystem")]
	[Space(10f)]
	public bool m_DestroyAfterDelay;

	public float m_DestroyDelay = 10f;

	public override ESerializableType SerializableType => ESerializableType.DLC1_DynamicFX;

	public override bool NeedsSaving => true;

	private void Start()
	{
		if (!m_PureParticleSystem)
		{
			StartCoroutine(AutoDestroy());
		}
	}

	private IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(m_DestroyDelay);
		Object.Destroy(base.gameObject);
	}

	public void StopVFX()
	{
		m_Stopped = true;
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enableEmission = false;
			componentsInChildren[i].loop = false;
		}
	}

	public override bool OnSerialize(CFG_SG_Node ParentNode)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(ParentNode);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Disabled", m_Stopped);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node Node)
	{
		if (Node == null)
		{
			return false;
		}
		m_Stopped = Node.Attrib_Get("Disabled", DefVal: false);
		if (m_Stopped)
		{
			StopVFX();
		}
		return true;
	}
}
