using System;
using System.Collections.Generic;

namespace Azalea.Utils;
public class ReadOnlyObservable<T>(T initialValue) : IFormattable, IObservable<T>
{
	private T _value = initialValue;

	public T Value
	{
		get => _value;
		internal set
		{
			if (EqualityComparer<T>.Default.Equals(value, _value))
				return;

			_value = value;
			OnValueChanged?.Invoke(_value);
		}
	}

	public event Action<T>? OnValueChanged;

	public static implicit operator T(ReadOnlyObservable<T> observable) => observable.Value;
	public override string ToString() => _value is null ? "Null" : _value.ToString()!;
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		if (format is null) return ToString();

		if (_value is IFormattable formattable)
			return formattable.ToString(format, formatProvider);

		return ToString();
	}
}
