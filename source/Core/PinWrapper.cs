using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core;

[Serializable]
public abstract class PinWrapper<NODE, TYPE> where NODE : UnityEngine.Object, INode where TYPE : struct
{
	[Serializable]
	public abstract class PinBase : Pin
	{
		[SerializeField]
		protected NODE m_Owner;

		[SerializeField]
		protected List<Pin> m_Links = new List<Pin>();

		public override INode Owner
		{
			get
			{
				return m_Owner;
			}
			set
			{
				m_Owner = value as NODE;
			}
		}

		public virtual TYPE PinType => default(TYPE);

		public override IList Links => m_Links;

		public override Color PinColor => Color.white;

		public override bool CanConnect(IPin pin)
		{
			return base.CanConnect(pin) && pin is PinBase && (pin as PinBase).PinType.Equals(PinType);
		}
	}
}
