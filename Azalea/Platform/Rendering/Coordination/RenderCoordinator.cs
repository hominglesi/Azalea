using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Threading;
using Azalea.Utils;
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

	public RenderCommandGroup? _commandQueue;
	[MemberNotNull(nameof(_commandQueue))]
	private void assertQueueExists() => Debug.Assert(_commandQueue is not null,
		"Command queue does not exist!");
	private void assertQueueDoesNotExist() => Debug.Assert(_commandQueue is null,
		"Command queue already exists!");

	public RenderCommandGroup CommandQueue =>
		_commandQueue is not null ? _commandQueue
			: throw new Exception("A command queue has not been started!");

	public RenderCommandGroup BeginCommandQueue()
	{
		assertQueueDoesNotExist();

		return _commandQueue = ObjectPool<RenderCommandGroup>.Borrow();
	}

	public RenderCommandGroup EndCommandQueue()
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

	public static Program CreateStandardProgram(ICommandHandler<RenderCommand> handler, string vertexShaderSource, string fragmentShaderSource)
	{
		var vertexShader = handler.GenerateShader(GL.VERTEX_SHADER);
		handler.ShaderSource(vertexShader, vertexShaderSource);
		handler.CompileShader(vertexShader);
		handler.PrintShaderCompileStatus(vertexShader);

		var fragmentShader = handler.GenerateShader(GL.FRAGMENT_SHADER);
		handler.ShaderSource(fragmentShader, fragmentShaderSource);
		handler.CompileShader(fragmentShader);
		handler.PrintShaderCompileStatus(fragmentShader);

		var program = handler.GenerateProgram();
		handler.AttachShader(program, vertexShader);
		handler.AttachShader(program, fragmentShader);
		handler.LinkProgram(program);
		handler.PrintProgramCompileStatus(program);

		handler.DeleteShader(vertexShader);
		handler.DeleteShader(fragmentShader);

		return program;
	}
}
