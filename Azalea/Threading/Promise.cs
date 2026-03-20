using System;
using System.Runtime.CompilerServices;

namespace Azalea.Threading;

public class Promise : Promise<bool>
{
	public void Resolve() => Resolve(true);
}

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

		_onCompleted?.Invoke();
		if (_onCompletedScheduled is not null)
			Scheduler.Schedule(_onCompletedScheduled);

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

	private Action? _onCompleted;
	private readonly object _onCompletedLock = new();

	public void ThenRun(Action action)
	{
		if (IsResolved)
			action.Invoke();
		else
			lock (_onCompletedLock)
				_onCompleted += action;
	}

	private Action? _onCompletedScheduled;
	private readonly object _onCompletedScheduledLock = new();
	public void ThenSchedule(Action action)
	{
		if (IsResolved)
			Scheduler.Schedule(action);
		else
			lock (_onCompletedScheduledLock)
				_onCompletedScheduled += action;
	}

	#region Awaiting

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

	#endregion
}
