public static class ELanguageExtensions
{
	public static string GetLanguageCode(this ELanguage val)
	{
		return val switch
		{
			ELanguage.English => "en", 
			ELanguage.French => "fr", 
			ELanguage.German => "de", 
			ELanguage.Russian => "ru", 
			ELanguage.Polish => "pl", 
			ELanguage.Custom => "custom", 
			ELanguage.None => "test", 
			_ => "en", 
		};
	}
}
