using System;

namespace BehaviorPattern;

public interface IOperation
{
	Type OpType { get; }

	int OpIndex { get; set; }

	bool Evaluate<T>(params T[] args);
}
