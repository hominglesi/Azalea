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

	public static IEnumerable<Type> GetAllChildrenOf(Type parentType)
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(t => t.IsSubclassOf(parentType));
	}

	public static T InstantiateType<T>(string typeName, Type assemblyType)
		=> (T)Activator.CreateInstance(assemblyType.Assembly.FullName!, typeName)!.Unwrap()!;

	public static T InstantiateType<T>(Type type)
		=> (T)Activator.CreateInstance(type)!;
}
