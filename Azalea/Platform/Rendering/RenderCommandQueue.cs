using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Azalea.Platform.Rendering;
public partial class RenderCommandQueue : IRenderCommandConsumer
{
	#region Pooling

	private RenderCommandQueue() { }

	private static readonly ConcurrentBag<RenderCommandQueue> _pool = [];
	internal static RenderCommandQueue Borrow()
	{
		if (_pool.TryTake(out var pool))
		{
			pool._commands.Clear();
			return pool;
		}

		return new RenderCommandQueue();
	}

	internal void Return()
	{
		while (_commands.TryDequeue(out var unusedCommand))
			unusedCommand.Return();

		_pool.Add(this);
	}

	#endregion


	private readonly Queue<RenderCommand> _commands = [];

	internal RenderCommand? Dequeue()
	{
		if (_commands.TryDequeue(out var command))
			return command;

		return null;
	}

	internal void Enqueue(RenderCommand command) => _commands.Enqueue(command);
	void IRenderCommandConsumer.Enqueue(RenderCommand command) => _commands.Enqueue(command);

	internal void ReturnAllCommands()
	{
		foreach (var command in _commands)
			command.Return();
	}
}
