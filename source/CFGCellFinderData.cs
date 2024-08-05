using UnityEngine;

public class CFGCellFinderData
{
	public CFGCellFinderData m_Prev;

	public CFGCell m_Cell;

	public int m_GCost;

	public int m_HCost;

	public int m_FCost;

	public CFGCellFinderData(CFGCell tile, int gcost)
	{
		m_Cell = tile;
		m_HCost = 0;
		m_FCost = 0;
		m_GCost = gcost;
		CalcF();
	}

	public void CalcF()
	{
		m_FCost = m_GCost + m_HCost;
	}

	public override bool Equals(object obj)
	{
		CFGCellFinderData cFGCellFinderData = (CFGCellFinderData)obj;
		return m_Cell == cFGCellFinderData.m_Cell;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static int CalcH(CFGCell start, CFGCell end)
	{
		return Mathf.Abs(end.PositionX - start.PositionX) * 10 + Mathf.Abs(end.PositionZ - start.PositionZ) * 10 + Mathf.Abs(end.Floor - start.Floor) * 25;
	}
}
