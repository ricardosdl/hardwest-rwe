namespace BehaviorPattern;

public interface ITaskBody
{
	T GetParam<T>(BehaviorComponent agent, string argName);

	T GetLocal<T>();

	T GetValue<T>(BehaviorComponent agent, string argName);

	BehaviorExec GetTaskInstance(BehaviorComponent agent);
}
