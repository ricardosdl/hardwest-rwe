using System;

namespace BehaviorPattern;

[Serializable]
public class ObjectOperation : Operation<EObjectOperatorType>
{
	public override bool Evaluate<T>(params T[] args)
	{
		return (EObjectOperatorType)OpIndex switch
		{
			EObjectOperatorType.Set => args[0] != null, 
			EObjectOperatorType.NotSet => args[0] == null, 
			_ => false, 
		};
	}
}
