using System;

namespace BehaviorPattern;

[Serializable]
public class EmptyOperation : IOperation
{
	public Type OpType => null;

	public int OpIndex
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public virtual bool Evaluate<T>(params T[] args)
	{
		return true;
	}
}
