using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azalea.Threading;
public static class Scheduler
{
	private readonly static Channel<Action> _commandChannel;

	static Scheduler()
	{
		_commandChannel = Channel.CreateUnbounded<Action>(new()
		{
			SingleReader = true
		});
	}

	public static void Schedule(Action command)
	{
		if (_commandChannel.Writer.TryWrite(command) == false)
			Console.WriteLine("Could not write command");
	}

	internal static void InvokeScheduled()
	{
		while (_commandChannel.Reader.TryRead(out var command))
			command.Invoke();
	}

	public static void Run(Action action)
	{
		Task.Run(action);
	}
}
