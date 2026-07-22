using System;

namespace Azalea.Utils;
public class Observable<T>(T initialValue)
	where T : unmanaged
{
	private T _value = initialValue;

	public T Value
	{
		get => _value;
		set
		{
			if (_value.Equals(value))
				return;

			_value = value;
			OnValueChanged?.Invoke(_value);
		}
	}

	public Action<T>? OnValueChanged;

	public static implicit operator T(Observable<T> observable) => observable.Value;
}
