public class CCardStatus
{
	private bool m_bIsNew;

	private ECardStatus m_Status;

	public bool IsNew
	{
		get
		{
			return m_bIsNew;
		}
		set
		{
			m_bIsNew = value;
		}
	}

	public ECardStatus Status
	{
		get
		{
			return m_Status;
		}
		set
		{
			m_Status = value;
		}
	}
}
