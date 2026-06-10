using Azalea.Platform.Windows;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Azalea.Threading;
internal abstract class GameThread
{
	public static readonly List<GameThread> ActiveThreads = [];
	public static event Action<GameThread>? OnThreadStarted;
	public static event Action<GameThread>? OnThreadStopped;

	private volatile bool _running;
	private readonly Thread _thread;

	private WindowsWaitableTimer? _timer;

	public virtual string DisplayName => "Game Thread";
	public int TargetInterval { get; }
	public int ManagedThreadId { get; }

	public GameThread(int targetInterval)
	{
		TargetInterval = targetInterval;

		_thread = new Thread(threadLoop)
		{
			IsBackground = true,
		};

		ManagedThreadId = _thread.ManagedThreadId;
	}

	public virtual void Start()
	{
		if (_running)
			return;

		_running = true;
		_thread.Start();

		ActiveThreads.Add(this);
		OnThreadStarted?.Invoke(this);
	}

	public void Stop()
	{
		if (_running == false)
			return;

		_running = false;
		_thread.Join();

		ActiveThreads.Remove(this);
		OnThreadStopped?.Invoke(this);
	}

	private void threadLoop()
	{
		Thread.BeginThreadAffinity();

		Initialize();

		_timer = new WindowsWaitableTimer(TargetInterval);
		_timer.Start();

		try
		{
			while (_running)
			{
				Update();

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
}
