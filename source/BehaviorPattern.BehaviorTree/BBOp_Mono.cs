using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

[Node("BB Mono")]
public class BBOp_Mono : BBOpUnary<MonoBehaviour, ObjectOperation>
{
}
