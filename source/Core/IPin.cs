using System.Collections.Generic;
using UnityEngine;

namespace Core;

public interface IPin
{
	PinDirection Dir { get; }

	PinMode PinMode { get; }

	INode Owner { get; set; }

	Color PinColor { get; }

	T GetOwner<T>() where T : class, INode;

	IEnumerable<IPin> AllLinks();

	T GetLink<T>() where T : class, IPin;

	bool CanConnect(IPin pin);

	bool Connect(IPin pin);

	void BreakTo(IPin pin);

	void Break();

	string GetDisplayName();
}
