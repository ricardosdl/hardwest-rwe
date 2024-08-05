using System.Collections.Generic;

namespace BehaviorPattern;

public delegate IEnumerator<TaskResult> TaskSignature(BehaviorComponent agent, ITaskBody body);
