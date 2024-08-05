using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("BB Float")]
public class BBOp_Float : BBOpBinary<BBFloat, float, ArithmeticOperation>
{
}
