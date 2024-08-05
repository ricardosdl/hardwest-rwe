public static class ConnectorDirectionMethods
{
	public static ConnectorDirection GetReversed(this ConnectorDirection dir)
	{
		return (dir == ConnectorDirection.CD_Input) ? ConnectorDirection.CD_Output : ConnectorDirection.CD_Input;
	}
}
