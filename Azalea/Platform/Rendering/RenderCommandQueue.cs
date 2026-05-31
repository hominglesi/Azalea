using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Azalea.Platform.Rendering;
public partial class RenderCommandQueue
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

	internal static void Return(RenderCommandQueue queue)
		=> _pool.Add(queue);

	#endregion


	private readonly Queue<RenderCommand> _commands = [];

	internal RenderCommand? Dequeue()
	{
		if (_commands.TryDequeue(out var command))
			return command;

		return null;
	}

	internal void Enqueue(RenderCommand command)
		=> _commands.Enqueue(command);
}
