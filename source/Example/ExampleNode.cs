using Core;
using UnityEngine;

namespace Example;

public class ExampleNode : ScriptableObject, INode
{
	[SerializeField]
	protected Vector2 m_NodePos;

	public Vector2 NodePos
	{
		get
		{
			return m_NodePos;
		}
		set
		{
			m_NodePos = value;
		}
	}

	public Color NodeColor => Color.green;

	public virtual void Initialize()
	{
	}

	public virtual string GetDisplayName()
	{
		NodeAttribute attribute = GetType().GetAttribute<NodeAttribute>(inherit: false);
		if (attribute != null)
		{
			return attribute.Name;
		}
		return GetType().Name;
	}

	public virtual void OnLink(IPin from, IPin to)
	{
	}

	public virtual void OnBreak(IPin from, IPin to)
	{
	}
}
