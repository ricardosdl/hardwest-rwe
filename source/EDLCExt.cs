public static class EDLCExt
{
	public static string GetDataDirExt(this EDLC Dlc)
	{
		return Dlc switch
		{
			EDLC.DLC1 => "_dlc1", 
			EDLC.DLC2 => "_dlc2", 
			_ => string.Empty, 
		};
	}

	public static bool ShouldBeChecked(this EDLC Dlc)
	{
		return Dlc switch
		{
			EDLC.DLC1 => true, 
			EDLC.DLC2 => false, 
			_ => true, 
		};
	}
}
