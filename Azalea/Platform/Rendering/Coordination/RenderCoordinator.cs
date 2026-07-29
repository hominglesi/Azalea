using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azalea.Platform.Rendering.Coordination;
public class RenderCoordinator
{
	public PlatformRenderer Renderer { get; }

	public DefaultQuadBatch DefaultQuadBatch { get; }

	public RenderCoordinator(PlatformRenderer renderer)
	{
		Renderer = renderer;

		DefaultQuadBatch = new DefaultQuadBatch(this);
	}

	#region CommandQueue

	public RenderCommandQueue? _commandQueue;
	[MemberNotNull(nameof(_commandQueue))]
	private void assertQueueExists() => Debug.Assert(_commandQueue is not null,
		"Command queue does not exist!");
	private void assertQueueDoesNotExist() => Debug.Assert(_commandQueue is null,
		"Command queue already exists!");

	public RenderCommandQueue CommandQueue =>
		_commandQueue is not null ? _commandQueue
			: throw new Exception("A command queue has not been started!");

	public RenderCommandQueue BeginCommandQueue()
	{
		assertQueueDoesNotExist();

		return _commandQueue = RenderCommandQueue.Borrow();
	}

	public RenderCommandQueue EndCommandQueue()
	{
		assertQueueExists();

		var commandQueue = _commandQueue;
		_commandQueue = null;
		return commandQueue;
	}

	#endregion
	#region ScissorState

	private readonly Stack<RectangleInt> _scissorStack = [];
	private void assertScissorExists() => Debug.Assert(_scissorStack.Count > 0,
		"Scissor state does not exist!");

	public void PushScissor(RectangleInt scissorRect)
	{
		assertQueueExists();

		FlushRenderBatch();

		_scissorStack.Push(scissorRect);
		_commandQueue.Scissor(scissorRect);
	}

	public void PopScissor()
	{
		assertQueueExists();
		assertScissorExists();

		FlushRenderBatch();

		_scissorStack.Pop();

		if (_scissorStack.Count == 0)
			_commandQueue.Scissor(null);
		else
			_commandQueue.Scissor(_scissorStack.Peek());
	}

	#endregion
	#region RenderBatch

	private IRenderBatch? _activeRenderBatch;
	internal void SelectRenderBatch(IRenderBatch renderBatch)
	{
		if (_activeRenderBatch == renderBatch)
			return;

		assertQueueExists();

		FlushRenderBatch();

		_activeRenderBatch = renderBatch;
	}

	internal void FlushRenderBatch()
	{
		if (_activeRenderBatch is null)
			return;

		assertQueueExists();

		_activeRenderBatch.Draw(_commandQueue);
		_activeRenderBatch = null;
	}

	#endregion

	public Program CreateStandardProgram(string vertexShaderSource, string fragmentShaderSource, ICommandGroup? commandGroup = null)
	{
		var vertexShader = Renderer.GenerateShader(GL.VERTEX_SHADER, commandGroup);
		Renderer.ShaderSource(vertexShader, vertexShaderSource, commandGroup);
		Renderer.CompileShader(vertexShader, commandGroup);
		Renderer.PrintShaderCompileStatus(vertexShader, commandGroup);

		var fragmentShader = Renderer.GenerateShader(GL.FRAGMENT_SHADER, commandGroup);
		Renderer.ShaderSource(fragmentShader, fragmentShaderSource, commandGroup);
		Renderer.CompileShader(fragmentShader, commandGroup);
		Renderer.PrintShaderCompileStatus(fragmentShader, commandGroup);

		var program = Renderer.GenerateProgram(commandGroup);
		Renderer.AttachShader(program, vertexShader, commandGroup);
		Renderer.AttachShader(program, fragmentShader, commandGroup);
		Renderer.LinkProgram(program, commandGroup);
		Renderer.PrintProgramCompileStatus(program, commandGroup);

		Renderer.DeleteShader(vertexShader, commandGroup);
		Renderer.DeleteShader(fragmentShader, commandGroup);

		return program;
	}
}
