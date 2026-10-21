using Azalea.Threading;

namespace Azalea.Platform.Rendering;

/// <summary>
/// Wrapper class that discards some commands if they wouldn't change render state.
/// </summary>
public class RenderCommandGroup : CommandGroup<RenderCommand>
{
	private Program? _currentProgram;
	private VertexArray? _currentVertexArray;

	public override ICommandAwaitable? Enqueue(RenderCommand command)
	{
		switch (command)
		{
			case BindVertexArrayCommand(var vertexArray) bindVertexArrayCommand:
				if (_currentVertexArray == vertexArray)
				{
					bindVertexArrayCommand.Return();
					return null;
				}

				_currentVertexArray = vertexArray;
				break;
			case UseProgramCommand(var program) useProgramCommand:
				if (_currentProgram == program)
				{
					useProgramCommand.Return();
					return null;
				}

				_currentProgram = program;
				break;
		}

		return base.Enqueue(command);
	}

	public override void Reset()
	{
		_currentProgram = null;
		_currentVertexArray = null;

		base.Reset();
	}
}
