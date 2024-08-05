public class CFGPair<T, U>
{
	public T First { get; set; }

	public U Second { get; set; }

	public CFGPair()
	{
	}

	public CFGPair(CFGPair<T, U> pair)
	{
		if (pair != null)
		{
			First = pair.First;
			Second = pair.Second;
		}
	}

	public CFGPair(T first, U second)
	{
		First = first;
		Second = second;
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is CFGPair<T, U> cFGPair))
		{
			return false;
		}
		return ((First == null && cFGPair.First == null) || (First != null && First.Equals(cFGPair.First))) && ((Second == null && cFGPair.Second == null) || (Second != null && Second.Equals(cFGPair.Second)));
	}

	public override int GetHashCode()
	{
		int num = 0;
		if (First == null && Second == null)
		{
			num = base.GetHashCode();
		}
		if (First != null)
		{
			num ^= First.GetHashCode();
		}
		if (Second != null)
		{
			num ^= Second.GetHashCode();
		}
		return num;
	}
}
