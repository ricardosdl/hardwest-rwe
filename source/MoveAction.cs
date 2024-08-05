using UnityEngine;

public class MoveAction
{
	public int CellEP;

	public Vector3 Position;

	public CFGCharacter Source;

	public EMOVEACTION Action;

	public int INTVAL1;

	public int INTVAL2;

	public bool Played;

	public MoveAction(int Cell, CFGCharacter _Source, EMOVEACTION MoveAction, int IV1, int IV2, Vector3 _Position)
	{
		CellEP = Cell;
		Source = _Source;
		Action = MoveAction;
		INTVAL1 = IV1;
		INTVAL2 = IV2;
		Position = _Position;
		Played = false;
	}
}
