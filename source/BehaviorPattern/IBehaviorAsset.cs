using System;

namespace BehaviorPattern;

public interface IBehaviorAsset
{
	Type CustomClass { get; }

	BehaviorInstance GetInstance(BehaviorComponent agent);

	void GetInstance(BehaviorComponent agent, ref BehaviorInstance instance);

	BehaviorExec StartRoot(BehaviorComponent agent);

	BehaviorExec GetRoot(BehaviorComponent agent);
}
