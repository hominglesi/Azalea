using System;
using System.Runtime.CompilerServices;

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

		_awaiter?.Complete(_value);
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

	private PromiseAwaiter? _awaiter;
	public PromiseAwaiter GetAwaiter() => _awaiter ??= new();

	public class PromiseAwaiter : INotifyCompletion
	{
		public bool IsCompleted { get; private set; }
		private Action? _onCompleted;
		private T? _result;

		public void Complete(T result)
		{
			_result = result;
			IsCompleted = true;
			_onCompleted?.Invoke();
		}

		public T GetResult() => _result!;

		public void OnCompleted(Action onCompleted)
			=> _onCompleted = onCompleted;
	}
}
