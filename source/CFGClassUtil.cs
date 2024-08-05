using System;
using System.Collections.Generic;
using System.Linq;

public abstract class CFGClassUtil
{
	public static IEnumerable<Type> AllClasses()
	{
		return typeof(CFGClassUtil).Assembly.GetTypes();
	}

	public static IEnumerable<Type> AllClasses(Type ofType)
	{
		return from p in typeof(CFGClassUtil).Assembly.GetTypes()
			where p.GetType() == ofType || p.IsSubclassOf(ofType)
			select p;
	}

	public static IEnumerable<Type> AllSubClasses(Type ofType)
	{
		return from p in typeof(CFGClassUtil).Assembly.GetTypes()
			where p.GetType() == ofType || p.IsSubclassOf(ofType)
			select p;
	}
}
