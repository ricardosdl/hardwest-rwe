namespace Core;

public class OutputPinAttribute : PinMarkAttribute
{
	public OutputPinAttribute()
	{
	}

	public OutputPinAttribute(string name)
	{
		Name = name;
	}
}
