using UnityEngine;

public class CFGVisObject : CFGSerializableObject
{
	[SerializeField]
	private bool m_Visible;

	public override ESerializableType SerializableType => ESerializableType.VisObject;

	public override bool NeedsSaving => true;

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("State", m_Visible);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node _Node)
	{
		m_Visible = _Node.Attrib_Get("State", m_Visible);
		ToggleVisible();
		return true;
	}

	private void ToggleVisible()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (!(child == null))
			{
				child.gameObject.SetActive(m_Visible);
			}
		}
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.enabled = m_Visible;
		}
	}

	private void Start()
	{
		ToggleVisible();
	}

	public void ChangeVisiblity(bool Visible)
	{
		if (m_Visible != Visible)
		{
			m_Visible = Visible;
			ToggleVisible();
		}
	}
}
