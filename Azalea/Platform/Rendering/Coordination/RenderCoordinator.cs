using Azalea.Native.OpenGL;
using Azalea.Numerics;
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
	#region Shader
	private IShader? _boundShader;
	public void BindShader(IShader shader)
	{
		Debug.Assert(shader is not null);
		if (_boundShader == shader)
			return;

		assertQueueExists();
		FlushRenderBatch();

		_commandQueue.UseProgram((Program)shader);
		_boundShader = shader;
	}

	public void Uniform(string uniformName, float v) => _commandQueue.Uniform1f((Program)_boundShader, uniformName, v);
	public void Uniform(string uniformName, float v1, float v2) => _commandQueue.Uniform2f((Program)_boundShader, uniformName, v1, v2);

	#endregion
	#region Texture
	private NativeTexture? _boundTexture;
	internal void BindTexture(NativeTexture texture)
	{
		Debug.Assert(texture is not null);
		if (_boundTexture == texture)
			return;

		assertQueueExists();
		FlushRenderBatch();

		_commandQueue.BindTexture(GL.TEXTURE_2D, texture);
		_boundTexture = texture;
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
}
