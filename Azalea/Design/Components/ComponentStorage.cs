using System;
using System.Collections.Generic;
using System.Reflection;

namespace Azalea.Design.Components;
public static class ComponentStorage<T>
	where T : Component, new()
{
	private static readonly List<T> _list = new();
	public static IReadOnlyList<T> GetComponents() => _list;

	public static void Add(T component)
	{
		_list.Add(component);
	}

	public static void Remove(T component)
	{
		if (_list.Contains(component) == false)
			throw new Exception("List does not contain component");

		_list.Remove(component);
	}
}

public static class ComponentStorage
{
	private static Dictionary<Type, (Type, MethodInfo, MethodInfo)> _cache = new();

	private static void cacheType(Type type)
	{
		if (_cache.ContainsKey(type)) return;

		var storage = typeof(ComponentStorage<>);
		storage = storage.MakeGenericType(type);

		var addMethod = storage.GetMethod("Add",
			BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod)!;

		var removeMethod = storage.GetMethod("Remove",
			BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod)!;

		_cache.Add(type, (storage, addMethod, removeMethod));
	}

	public static void Add(Component component)
	{
		var type = component.GetType();

		cacheType(type);

		var (componentType, addMethod, _) = _cache[type];
		addMethod.Invoke(componentType, new object[] { component });
	}

	public static void Remove(Component component)
	{
		var type = component.GetType();

		cacheType(type);

		var (componentType, _, removeMethod) = _cache[type];
		removeMethod.Invoke(componentType, new object[] { component });
	}
}
