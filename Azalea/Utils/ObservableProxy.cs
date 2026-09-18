using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Azalea.Utils;

public class ObservableProxy<T>
{
	private bool _invalidated = true;
	public bool Invalidated => _invalidated;

	private T _value;
	private readonly Action<T> _setAction;

	public ObservableProxy(object obj, string propertyName, Action<T>? setAction = null)
	{
		var changedEvent = obj.GetType().GetEvent($"On{propertyName}Changed");
		Debug.Assert(changedEvent is not null);

		var changedHandler = GetType().GetMethod(nameof(onValueChanged), BindingFlags.NonPublic | BindingFlags.Instance)!;

		changedEvent.AddEventHandler(obj, Delegate.CreateDelegate(changedEvent.EventHandlerType!, this, changedHandler));

		var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
		var property = obj.GetType().GetProperty(propertyName, bindingFlags)!;
		Debug.Assert(property is not null);

		var currentValue = property.GetValue(obj)!;
		Debug.Assert(currentValue is not null);

		_value = (T)currentValue;

		_setAction = setAction ?? (value => property.SetValue(obj, value)); 
	}

	private readonly Lock _lock = new();

	private void onValueChanged(T value)
	{
		lock (_lock)
		{
			if (Equals(_value, value))
				return;

			_value = value;
			_invalidated = true;
		}
	}

	public bool TryGetInvalid(out T invalid)
	{
		lock (_lock)
		{
			if (_invalidated == false)
			{
				invalid = default!;
				return false;
			}

			invalid = _value;
			_invalidated = false;
			return true;
		}
	}

	public void SetValue(T value)
	{
		if (Equals(_value, value))
			return;

		_setAction(value);
	}
}
