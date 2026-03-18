using System;

namespace Azalea.Threading;
public class Promise<T>
{
	private T? _value;

	public bool IsResolved { get; private set; }

	public Promise() { }

	public Promise(T value)
	{
		_value = value;
		IsResolved = true;
	}

	public void Resolve(T value)
	{
		if (IsResolved)
			throw new InvalidOperationException("Promise cannot be resolved twice!");

		_value = value;
		IsResolved = true;
	}

	public T Value
	{
		get
		{
			if (IsResolved == false)
				throw new InvalidOperationException("Cannot get promise value before it's resolved");

			return _value!;
		}
	}
}
