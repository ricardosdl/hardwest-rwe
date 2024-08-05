using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Accumulate")]
public class BTAccumulate : BTDecorator
{
	[Body("Before")]
	public AccumulatorAccessType m_AccessBeforeThreshold;

	[Body("After")]
	public AccumulatorAccessType m_AccessAfterThreshold = AccumulatorAccessType.Deny;
}
