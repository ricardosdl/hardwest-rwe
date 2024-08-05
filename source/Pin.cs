using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public abstract class Pin : ScriptableObject, IPin
{
	public virtual PinMode PinMode => PinMode.Single;

	public abstract PinDirection Dir { get; }

	public abstract INode Owner { get; set; }

	public abstract Color PinColor { get; }

	public virtual IList Links => null;

	public virtual IPin Link
	{
		get
		{
			if (PinMode == PinMode.Single && Links.Count > 0)
			{
				return Links[0] as IPin;
			}
			return null;
		}
		set
		{
			if (PinMode == PinMode.Single && Links.Count > 0)
			{
				Links[0] = value;
			}
		}
	}

	public virtual string GetDisplayName()
	{
		return base.name;
	}

	public virtual IEnumerable<IPin> AllLinks()
	{
		if (Links != null)
		{
			foreach (object link in Links)
			{
				if (link is IPin)
				{
					yield return link as IPin;
				}
			}
			yield break;
		}
		if (Link != null)
		{
			yield return Link;
		}
	}

	public virtual T GetLink<T>() where T : class, IPin
	{
		return Link as T;
	}

	public virtual T GetOwner<T>() where T : class, INode
	{
		return Owner as T;
	}

	protected virtual void OnEnable()
	{
		base.hideFlags = HideFlags.HideInHierarchy;
	}

	public virtual bool CanConnect(IPin pin)
	{
		return Dir != pin.Dir;
	}

	public virtual bool Connect(IPin conn)
	{
		if (PinMode == PinMode.Single)
		{
			if (Links.Count == 0)
			{
				Links.Add(conn);
			}
			else
			{
				Links[0] = conn;
			}
		}
		else if (!Links.Contains(conn))
		{
			Links.Add(conn);
		}
		return true;
	}

	public virtual void BreakTo(IPin pin)
	{
		if (Links != null)
		{
			Links.Remove(pin);
		}
		else if (Link == pin)
		{
			Link = null;
		}
	}

	public virtual void Break()
	{
		foreach (IPin item in AllLinks())
		{
			item.BreakTo(this);
		}
		if (Links != null)
		{
			Links.Clear();
		}
		else if (Link != null)
		{
			Link = null;
		}
	}
}
