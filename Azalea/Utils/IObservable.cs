using System;

namespace Azalea.Utils;
public interface IObservable<T>
	where T : unmanaged
{
	public T Value { get; }
	public event Action<T>? OnValueChanged;
}
