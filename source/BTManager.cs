public class BTManager
{
	private static object _lock = new object();

	protected BTManager m_Instance;

	public BTManager Instance
	{
		get
		{
			lock (_lock)
			{
				return m_Instance ?? (m_Instance = new BTManager());
			}
		}
	}

	private BTManager()
	{
	}
}
