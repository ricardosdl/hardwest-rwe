using System.Text.RegularExpressions;

public static class StringExtensions
{
	public static string[] ToLines(this string input)
	{
		return Regex.Split(input, "\r\n|\r|\n");
	}

	public static string SplitCamelCase(this string input)
	{
		return Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1");
	}
}
