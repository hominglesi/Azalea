using Azalea.Utils;
using System.Collections.Generic;
using System.Threading;

namespace Azalea.Threading;

public class CommandGroup<T> : ICommandHandler<T>, IPoolable
	where T : ThreadCommand
{
	private readonly Lock _commandsLock = new();
	private readonly Queue<T> _commands = [];

	public virtual ICommandAwaitable? Enqueue(T command)
	{
		lock (_commandsLock)
			_commands.Enqueue(command);

		if (command is ICommandAwaitable awaitable)
			return awaitable;

		return null;
	}

	internal T? Dequeue()
	{
		if (_commands.TryDequeue(out var command))
			return command;

		return null;
	}

	public virtual void Reset()
	{
		while (_commands.TryDequeue(out var unusedCommand))
			unusedCommand.Return();

		_commands.Clear();
	}
}
