using UnityEngine;

namespace Core;

public interface INode
{
	Vector2 NodePos { get; set; }

	Color NodeColor { get; }

	void Initialize();

	void OnLink(IPin from, IPin to);

	void OnBreak(IPin from, IPin to);

	string GetDisplayName();
}
