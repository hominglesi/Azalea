using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Azalea.Utils.Proxies;

public class Proxy<T> : IProxy<T>
{
	private readonly object _original;
	private readonly PropertyInfo _property;
	private readonly Action<T>? _indirectSet;
	private T _value;

	public Proxy(object original, string propertyName, Action<T>? indirectSet = null)
	{
		_original = original;
		_indirectSet = indirectSet;

		var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
		_property = original.GetType().GetProperty(propertyName, bindingFlags)!;

		_value = getValue();
	}

	private T getValue() => (T)_property.GetValue(_original)!;

	public bool HasNewValue(out T value)
	{
		value = getValue();

		if (Equals(_value, value))
			return false;

		_value = value;
		return true;
	}

	public void SetValue(T value)
	{
		if (_indirectSet is not null)
		{
			_indirectSet(value);
			return;
		}

		_property.SetValue(_original, value);
	}
}
