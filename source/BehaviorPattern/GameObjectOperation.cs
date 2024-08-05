using System;

namespace BehaviorPattern;

[Serializable]
public class GameObjectOperation : Operation<EObjectOperatorType>
{
	public override bool Evaluate<T>(params T[] args)
	{
		return (EObjectOperatorType)OpIndex switch
		{
			EObjectOperatorType.Set => args[0] != null && !args[0].Equals(null), 
			EObjectOperatorType.NotSet => args[0] == null || args[0].Equals(null), 
			_ => false, 
		};
	}
}
