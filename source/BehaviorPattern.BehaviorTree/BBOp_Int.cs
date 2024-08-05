using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("BB Int")]
public class BBOp_Int : BBOpBinary<BBInt, int, ArithmeticOperation>
{
}
