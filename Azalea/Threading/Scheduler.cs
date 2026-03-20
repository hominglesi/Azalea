using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azalea.Threading;
public static class Scheduler
{
	private readonly static Channel<(Action Action, Promise Promise)> _commandChannel;

	static Scheduler()
	{
		_commandChannel = Channel.CreateUnbounded<(Action, Promise)>(new()
		{
			SingleReader = true
		});
	}

	public static Promise Schedule(Action command)
	{
		var promise = new Promise();
		if (_commandChannel.Writer.TryWrite((command, promise)) == false)
			Console.WriteLine("Could not write command");

		return promise;
	}

	internal static void InvokeScheduled()
	{
		while (_commandChannel.Reader.TryRead(out var command))
		{
			command.Action.Invoke();
			command.Promise.Resolve();
		}
	}

	public static Promise Run(Func<Task> action)
	{
		var promise = new Promise();
		Task.Run(async () =>
		{
			await action.Invoke();
			promise.Resolve();
		});

		return promise;
	}
}
