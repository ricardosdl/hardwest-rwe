public class CFGJoyAction
{
	public struct ActionDef
	{
		public EJoyButton Button1;

		public EJoyButton Button2;

		private bool bUseUp;

		public bool Check()
		{
			bUseUp = true;
			bool bContinous = false;
			if (Button1 != 0 && Button2 != 0)
			{
				bContinous = true;
			}
			float num = CFGJoyManager.ReadAsButton(Button1, bContinous, bUseUp);
			if (Button2 == EJoyButton.Unknown && num >= 0.5f)
			{
				return true;
			}
			if (num < 0.5f)
			{
				return false;
			}
			float num2 = CFGJoyManager.ReadAsButton(Button2, bContinous: false, bUseUp);
			if (num2 < 0.5f)
			{
				return false;
			}
			return true;
		}
	}

	private bool bActive;

	private ActionDef[] Actions = new ActionDef[4];

	public bool IsActive
	{
		get
		{
			return bActive;
		}
		set
		{
			bActive = value;
		}
	}

	public CFGJoyAction(EJoyButton P1B1, EJoyButton P1B2, EJoyButton P2B1, EJoyButton P2B2, EJoyButton P3B1, EJoyButton P3B2, EJoyButton P4B1, EJoyButton P4B2)
	{
		Actions[0].Button1 = P1B1;
		Actions[0].Button2 = P1B2;
		Actions[1].Button1 = P2B1;
		Actions[1].Button2 = P2B2;
		Actions[2].Button1 = P3B1;
		Actions[2].Button2 = P3B2;
		Actions[3].Button1 = P4B1;
		Actions[3].Button2 = P4B2;
		bActive = false;
	}

	public void Read()
	{
		bActive = false;
		int num = 0;
		do
		{
			bActive |= Actions[num].Check();
			if (bActive)
			{
				break;
			}
			num++;
		}
		while (num <= 3);
	}

	public float GetComposedValue()
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			if (Actions[i].Button1 != 0)
			{
				float num2 = CFGJoyManager.ReadAsButton(Actions[i].Button1, bContinous: false, bUseUp: true);
				if (!(num2 < 0.2f))
				{
					num += num2;
				}
			}
		}
		return num;
	}
}
