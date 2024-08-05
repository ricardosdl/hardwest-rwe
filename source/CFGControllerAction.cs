public class CFGControllerAction
{
	[CFGTableField(ColumnName = "Action", DefaultValue = EJoyAction.None)]
	public EJoyAction Action;

	[CFGTableField(ColumnName = "JoyP1_B1", DefaultValue = EJoyButton.Unknown)]
	public EJoyButton Key1_B1;

	[CFGTableField(ColumnName = "JoyP1_B2", DefaultValue = EJoyButton.Unknown)]
	public EJoyButton Key1_B2;

	[CFGTableField(ColumnName = "JoyP2_B1", DefaultValue = EJoyButton.Unknown)]
	public EJoyButton Key2_B1;

	[CFGTableField(ColumnName = "JoyP2_B2", DefaultValue = EJoyButton.Unknown)]
	public EJoyButton Key2_B2;

	[CFGTableField(ColumnName = "JoyP3_B1", DefaultValue = EJoyButton.Unknown)]
	public EJoyButton Key3_B1;

	[CFGTableField(ColumnName = "JoyP3_B2", DefaultValue = EJoyButton.Unknown)]
	public EJoyButton Key3_B2;
}
