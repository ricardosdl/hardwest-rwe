internal static class CFGObjectiveImportanceExtention
{
	public static EObjectiveImportance ToObjectiveImportance(this string text)
	{
		return text.ToLower().Trim() switch
		{
			"optional" => EObjectiveImportance.Optional, 
			_ => EObjectiveImportance.Main, 
		};
	}

	public static EObjectiveType ToObjectiveType(this string text)
	{
		return text.ToLower().Trim() switch
		{
			"generic" => EObjectiveType.Generic, 
			"kill" => EObjectiveType.Kill, 
			"take" => EObjectiveType.Take, 
			"go" => EObjectiveType.Go, 
			"find" => EObjectiveType.Find, 
			"use" => EObjectiveType.Use, 
			"safecrack" => EObjectiveType.Safecrack, 
			"knockout" => EObjectiveType.Knockout, 
			_ => EObjectiveType.Generic, 
		};
	}
}
