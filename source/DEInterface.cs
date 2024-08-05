using System;
using UnityEngine;

public class DEInterface<T> : DEInterface where T : class
{
	public T IObject => m_Object as T;

	public DEInterface()
		: base(typeof(T))
	{
	}

	public DEInterface(T interfacedObject)
		: base(typeof(T), interfacedObject as UnityEngine.Object)
	{
	}
}
[Serializable]
public class DEInterface
{
	[SerializeField]
	protected UnityEngine.Object m_Object;

	[HideInInspector]
	[SerializeField]
	protected SerializedType m_InterfaceType;

	public UnityEngine.Object Object => m_Object;

	protected DEInterface(Type interfaceType)
	{
		m_InterfaceType = new SerializedType(interfaceType);
	}

	protected DEInterface(Type interfaceType, UnityEngine.Object interfacedObject)
		: this(interfaceType)
	{
		m_Object = interfacedObject;
	}
}
