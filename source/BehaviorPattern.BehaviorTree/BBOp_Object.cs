using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

[Node("BB Object")]
public class BBOp_Object : BBOpUnary<GameObject, GameObjectOperation>
{
}
