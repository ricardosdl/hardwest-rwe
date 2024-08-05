using System;
using System.Collections.Generic;
using System.Linq;

namespace Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class CategoryAttribute : Attribute
{
	public string Name;

	public CategoryAttribute(string name)
	{
		Name = name ?? "Other";
	}

	public List<string> GetCategoryChain()
	{
		return Name.Split('/').ToList();
	}
}
