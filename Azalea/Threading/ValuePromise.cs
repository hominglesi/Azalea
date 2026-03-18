using System;

namespace Azalea.Threading;
public readonly struct ValuePromise<T>
{
	private readonly T? _value;
	private readonly Promise<T>? _promise;

	public ValuePromise(T value)
	{
		_value = value;
	}

	public ValuePromise(Promise<T> promise)
	{
		_promise = promise;
	}

	public ValuePromise()
	{
		throw new Exception($"{nameof(ValuePromise<T>)} must be created with either a value or a promise.");
	}

	public bool IsResolved => _promise is null || _promise.IsResolved;
	public T Value => _promise is not null ? _promise.Value : _value!;

	public void AssertResolved()
	{
		if (IsResolved == false)
			throw new Exception($"{nameof(ValuePromise<T>)} wasn't resolved!");
	}
}
