using Azalea.Platform;
using Azalea.Platform.Windows;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Azalea.Threading;
internal abstract class GameThread
{
	public static readonly List<GameThread> ActiveThreads = [];
	public static event Action<GameThread>? OnThreadStarted;
	public static event Action<GameThread>? OnThreadStopped;

	public readonly Thread NativeThread;

	private volatile bool _running;
	public readonly ReadOnlyObservable<bool> Running = new(false);

	private WindowsWaitableTimer? _timer;

	public virtual string DisplayName => "Game Thread";
	public int TargetInterval { get; }
	public float TargetFrequency => 1000 / TargetInterval;

	public GameThread(int targetInterval)
	{
		TargetInterval = targetInterval;

		NativeThread = new Thread(threadLoop)
		{
			IsBackground = true,
		};
	}

	public virtual void Start()
	{
		if (_running)
			return;

		_running = true;
		NativeThread.Start();
		Running.Value = true;

		ActiveThreads.Add(this);
		OnThreadStarted?.Invoke(this);
	}

	public void Stop()
	{
		if (_running == false)
			return;

		_running = false;
		NativeThread.Join();
		Running.Value = false;

		ActiveThreads.Remove(this);
		OnThreadStopped?.Invoke(this);
	}

	private void threadLoop()
	{
		Thread.BeginThreadAffinity();

		Initialize();

		_timer = new WindowsWaitableTimer(TargetInterval);
		_timer.Start();

		_lastTickTime = Time.GetCurrentPreciseTime();

		try
		{
			while (_running)
			{
				_workBeginTime = Time.GetCurrentPreciseTime();
				Update();

				updateActualInterval();
				_timer.Wait();
			}
		}
		finally
		{
			Thread.EndThreadAffinity();
			_timer.Dispose();
		}
	}

	protected abstract void Initialize();
	protected abstract void Update();

	#region ActualInterval

	private int _tickIndex = 0;
	private double _tickSum = 0;
	private double[] _tickList = new double[100];
	private DateTime _lastTickTime;

	private double _workSum = 0;
	private double[] _workDurationList = new double[100];
	private DateTime _workBeginTime;

	public readonly ReadOnlyObservable<double> AverageInterval = new(0);
	public readonly ReadOnlyObservable<double> AverageFrequency = new(0);
	public readonly ReadOnlyObservable<double> AverageWorkDuration = new(0);

	private void updateActualInterval()
	{
		var tickTime = Time.GetCurrentPreciseTime();
		var deltaTime = tickTime.Subtract(_lastTickTime).TotalMilliseconds;
		_lastTickTime = tickTime;

		_tickSum -= _tickList[_tickIndex];
		_tickSum += deltaTime;
		_tickList[_tickIndex] = deltaTime;

		var workDuration = tickTime.Subtract(_workBeginTime).TotalMilliseconds;

		_workSum -= _workDurationList[_tickIndex];
		_workSum += workDuration;
		_workDurationList[_tickIndex] = workDuration;

		_tickIndex = (_tickIndex + 1) % _tickList.Length;

		if (_tickIndex % 20 == 0)
		{
			AverageInterval.Value = _tickSum / _tickList.Length;
			AverageFrequency.Value = 1000 / AverageInterval;

			AverageWorkDuration.Value = _workSum / _tickList.Length;
		}
	}

	#endregion
}
