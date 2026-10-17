using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
	private RenderCommandGroup getCommandQueue()
	{
		Debug.Assert(_commandQueue is not null, "Command queue does not exist!");
		return _commandQueue;
	}

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
		var commandQueue = getCommandQueue();
		_commandQueue = null;

		Debug.Assert(_scissorStack.Count == 0);
		_boundShader = null;
		_boundTexture = null;
		_activeRenderBatch = null;

		return commandQueue;
	}

	#endregion
	#region ScissorState

	private readonly Stack<RectangleInt> _scissorStack = [];
	private void assertScissorExists() => Debug.Assert(_scissorStack.Count > 0,
		"Scissor state does not exist!");

	public void PushScissor(RectangleInt scissorRect)
	{
		FlushRenderBatch();

		_scissorStack.Push(scissorRect);
		getCommandQueue().Scissor(scissorRect);
	}

	public void PopScissor()
	{
		var commandQueue = getCommandQueue();
		assertScissorExists();

		FlushRenderBatch();

		_scissorStack.Pop();

		if (_scissorStack.Count == 0)
			commandQueue.Scissor(null);
		else
			commandQueue.Scissor(_scissorStack.Peek());
	}

	#endregion
	#region Shader
	private IShader? _boundShader;
	public void BindShader(IShader shader)
	{
		Debug.Assert(shader is not null);
		if (_boundShader == shader)
			return;

		FlushRenderBatch();

		getCommandQueue().UseProgram((Program)shader);
		_boundShader = shader;
	}

	private Program getBoundProgram()
	{
		Debug.Assert(_boundShader is not null);
		return (Program)_boundShader;
	}

	public void Uniform(string uniformName, float v) => getCommandQueue().Uniform1f(getBoundProgram(), uniformName, v);
	public void Uniform(string uniformName, float v1, float v2) => getCommandQueue().Uniform2f(getBoundProgram(), uniformName, v1, v2);

	#endregion
	#region Texture
	private NativeTexture? _boundTexture;
	internal void BindTexture(NativeTexture texture)
	{
		Debug.Assert(texture is not null);
		if (_boundTexture == texture)
			return;

		FlushRenderBatch();

		getCommandQueue().BindTexture(GL.TEXTURE_2D, texture);
		_boundTexture = texture;
	}

	#endregion
	#region RenderBatch

	private IRenderBatch? _activeRenderBatch;
	internal void SelectRenderBatch(IRenderBatch renderBatch)
	{
		if (_activeRenderBatch == renderBatch)
			return;

		FlushRenderBatch();

		_activeRenderBatch = renderBatch;
	}

	internal void FlushRenderBatch()
	{
		if (_activeRenderBatch is null)
			return;

		var commandQueue = getCommandQueue();

		_activeRenderBatch.Draw(commandQueue);
		_activeRenderBatch = null;
	}

	#endregion
}
