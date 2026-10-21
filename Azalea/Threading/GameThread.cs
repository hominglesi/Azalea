using Azalea.Platform.Windows;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;

namespace Azalea.Threading;

internal abstract partial class GameThread(int targetInterval)
	: GameThread<ThreadCommand>(targetInterval)
{ }

internal abstract partial class GameThread<T> : ICommandHandler<T>
	where T : ThreadCommand
{
	public readonly Thread NativeThread;

	private volatile bool _running;
	public bool Running { get; private set; }

	private WindowsWaitableTimer? _timer;

	public virtual string DisplayName => "Game Thread";
	public int TargetInterval { get; }
	public float TargetFrequency => 1000 / TargetInterval;

	public GameThread(int targetInterval)
	{
		TargetInterval = targetInterval;

		NativeThread = new Thread(threadLoop)
		{
			Name = DisplayName,
			IsBackground = true
		};

		if (OperatingSystem.IsWindows())
			NativeThread.SetApartmentState(ApartmentState.STA);
	}

	public virtual void Start()
	{
		if (_running)
			return;

		_running = true;
		NativeThread.Start();
		_stopwatch.Start();
		Running = true;
	}

	public void Stop()
	{
		if (_running == false)
			return;

		_running = false;

		if (Environment.CurrentManagedThreadId != NativeThread.ManagedThreadId)
			NativeThread.Join();
		else
			Console.WriteLine($"{DisplayName} closed itself!");

		Running = false;
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
				var workBegan = GetRunningTime();
				HandleCommands();
				Update();

				updateTime(workBegan);
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

	#region Thread Commands

	private readonly Channel<T> _pendingCommands = Channel.CreateUnbounded<T>(
		new() { SingleReader = true }
	);
	private readonly Lock _pendingCommandsLock = new();

	public virtual ICommandAwaitable? Enqueue(T command)
	{
		if (_pendingCommands.Writer.TryWrite(command) == false)
			throw new Exception("Could not write command!");

		if (command is ICommandAwaitable awaitable)
			return awaitable;

		return null;
	}

	protected virtual void HandleCommand(T command) { }

	protected virtual void HandleCommands()
	{
		lock (_pendingCommandsLock)
		{
			while (_pendingCommands.Reader.TryRead(out var command))
				HandleCommand(command);
		}
	}

	public void SubmitCommandGroup(CommandGroup<T> commandGroup)
	{
		lock (_pendingCommandsLock)
		{
			T? nextCommand;
			while ((nextCommand = commandGroup.Dequeue()) is not null)
				Enqueue(nextCommand);
		}
	}

	#endregion

	#region Time

	private readonly Stopwatch _stopwatch = new();
	public DateTime GetRunningTime() => new(_stopwatch.ElapsedTicks);
	public float GetMilisecondsSince(DateTime time) => (float)GetRunningTime().Subtract(time).TotalMilliseconds;

	public float DeltaTime { get; private set; }
	private DateTime _lastTickTime;

	private int _tickIndex = 0;
	private double _tickSum = 0;
	private readonly double[] _tickList = new double[100];

	private double _workSum = 0;
	private readonly double[] _workDurationList = new double[100];

	public double AverageInterval { get; private set; }
	public double AverageFrequency { get; private set; }
	public double AverageWorkDuration { get; private set; }

	private void updateTime(DateTime workBegan)
	{
		var tickTime = GetRunningTime();
		DeltaTime = (float)tickTime.Subtract(_lastTickTime).TotalSeconds;

		_tickSum -= _tickList[_tickIndex];
		_tickSum += (DeltaTime * 1000);
		_tickList[_tickIndex] = (DeltaTime * 1000);

		var workDuration = tickTime.Subtract(workBegan).TotalMilliseconds;

		_workSum -= _workDurationList[_tickIndex];
		_workSum += workDuration;
		_workDurationList[_tickIndex] = workDuration;

		_tickIndex = (_tickIndex + 1) % _tickList.Length;
		_lastTickTime = tickTime;

		if (_tickIndex % 20 == 0)
		{
			AverageInterval = _tickSum / _tickList.Length;
			AverageFrequency = 1000 / AverageInterval;

			AverageWorkDuration = _workSum / _tickList.Length;
		}
	}

	#endregion
}
