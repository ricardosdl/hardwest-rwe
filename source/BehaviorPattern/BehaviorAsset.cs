using System;

namespace BehaviorPattern;

[Serializable]
public class BehaviorAsset : DEInterface<IBehaviorAsset>
{
	public BehaviorAsset(IBehaviorAsset asset)
		: base(asset)
	{
	}
}
