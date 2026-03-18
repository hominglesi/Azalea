using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

	public ValueAwaiter GetAwaiter() => ValueAwaiter.Create(_value, _promise);

	public readonly struct ValueAwaiter(T? value, Promise<T>? promise) : INotifyCompletion
	{
		private static readonly ValueAwaiter _completedDefault = new(default, null);

		public static ValueAwaiter Create(T? value, Promise<T>? promise)
		{
			if (promise is null && EqualityComparer<T>.Default.Equals(value, default))
				return _completedDefault;

			return new ValueAwaiter(value, promise);
		}

		private readonly T? _value = value;
		private readonly Promise<T>? _promise = promise;

		public readonly bool IsCompleted => _promise is null || _promise.GetAwaiter().IsCompleted;

		public readonly T GetResult() => _promise is not null ? _promise.GetAwaiter().GetResult() : _value!;

		public readonly void OnCompleted(Action onCompleted)
			=> _promise!.GetAwaiter().OnCompleted(onCompleted);
	}
}
