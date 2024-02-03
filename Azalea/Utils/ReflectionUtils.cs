using System;
using System.Collections.Generic;
using System.Linq;

namespace Azalea.Utils;
public static class ReflectionUtils
{
	public static IEnumerable<Type> GetAllTypesWithInterface(Type interfaceType)
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(t => t.GetInterfaces()
			.Contains(interfaceType));
	}

	public static IEnumerable<string> GetAllTypeWithInterfaceAsStrings(Type interfaceType)
	{
		foreach (var validType in GetAllTypesWithInterface(interfaceType))
			yield return validType.Name;
	}

	public static IEnumerable<Type> GetAllChildrenOf(Type parentType)
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(t => t.IsSubclassOf(parentType));
	}

	public static IEnumerable<string> GetAllChildrenOfAsStrings(Type parentType)
	{
		foreach (var validType in GetAllChildrenOf(parentType))
			yield return validType.Name;
	}
}
