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
		if (_pool.TryTake(out var existing))
		{
			existing._commands.Clear();
			existing._currentProgram = null;
			existing._currentVertexArray = null;
			return existing;
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

	private Program? _currentProgram;
	private VertexArray? _currentVertexArray;

	internal RenderCommand? Dequeue()
	{
		if (_commands.TryDequeue(out var command))
			return command;

		return null;
	}

	internal void Enqueue(RenderCommand command)
	{
		// We avoid enqueuing commands that don't change state 
		switch (command)
		{
			case BindVertexArrayCommand(var vertexArray):
				if (_currentVertexArray == vertexArray) return;

				_currentVertexArray = vertexArray;
				break;
			case UseProgramCommand(var program):
				if (_currentProgram == program) return;

				_currentProgram = program;
				break;

		}

		_commands.Enqueue(command);
	}

	void IRenderCommandConsumer.Enqueue(RenderCommand command) => Enqueue(command);

	internal void ReturnAllCommands()
	{
		foreach (var command in _commands)
			command.Return();
	}
}
