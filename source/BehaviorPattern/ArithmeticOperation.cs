using System;
using System.Collections.Generic;

namespace BehaviorPattern;

[Serializable]
public class ArithmeticOperation : Operation<EArithmeticOperatorType>
{
	public override bool Evaluate<T>(params T[] args)
	{
		IComparer<T> @default = Comparer<T>.Default;
		bool result = false;
		if (@default != null)
		{
			int num = @default.Compare(args[0], args[1]);
			switch ((EArithmeticOperatorType)OpIndex)
			{
			case EArithmeticOperatorType.Equal:
				result = num == 0;
				break;
			case EArithmeticOperatorType.NotEqual:
				result = num != 0;
				break;
			case EArithmeticOperatorType.Less:
				result = num < 0;
				break;
			case EArithmeticOperatorType.LessOrEqual:
				result = num <= 0;
				break;
			case EArithmeticOperatorType.Greater:
				result = num > 0;
				break;
			case EArithmeticOperatorType.GreaterOrEqual:
				result = num >= 0;
				break;
			}
		}
		return result;
	}
}
