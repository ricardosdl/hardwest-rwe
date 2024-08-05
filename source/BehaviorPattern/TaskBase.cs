namespace BehaviorPattern;

public abstract class TaskBase<T>
{
	protected T m_Current;

	public virtual T Current => m_Current;

	public TaskBase()
	{
		m_Current = default(T);
	}

	public virtual bool Update()
	{
		return false;
	}

	public virtual void Abort()
	{
	}
}
