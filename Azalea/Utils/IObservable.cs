using System;

namespace Azalea.Utils;
public interface IObservable<T>
{
	public T Value { get; }
	public event Action<T>? OnValueChanged;
}
