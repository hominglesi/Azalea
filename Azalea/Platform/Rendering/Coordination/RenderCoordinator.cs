using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Platform.Rendering.OpenGL.LoadingContext;
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
	public Program DefaultQuadProgram { get; }

	public Program DefaultTextProgram { get; }

	public RenderCoordinator(PlatformRenderer renderer)
	{
		Renderer = renderer;

		DefaultQuadBatch = new DefaultQuadBatch(this);

		DefaultQuadProgram = new Program();
		GLRenderer.LoadingContext.GenerateProgram(DefaultQuadProgram,
			_quadVertexShaderSource, _quadFragmentShaderSource);

		DefaultTextProgram = new Program();
		GLRenderer.LoadingContext.GenerateProgram(DefaultTextProgram,
			_quadVertexShaderSource, _textFragmentShaderSource);

		var shaderGroup = ObjectPool<RenderCommandGroup>.Borrow();

		shaderGroup.UseProgram(DefaultQuadProgram);

		Renderer.Thread.SubmitCommandGroup(shaderGroup);
		ObjectPool<RenderCommandGroup>.Return(shaderGroup);

		_boundProgram = DefaultQuadProgram;
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
	#region Program
	private Program? _boundProgram;
	internal void BindProgram(Program program)
	{
		if (_boundProgram == program)
			return;

		assertQueueExists();
		FlushRenderBatch();

		_commandQueue.UseProgram(program);
		_boundProgram = program;
	}
	#endregion
	#region Texture
	private Texture? _boundTexture;
	internal void BindTexture(Texture texture)
	{
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

	private const string _quadVertexShaderSource = """
		#version 330 core
		#extension GL_ARB_shading_language_420pack : enable

		layout (location = 0) in vec2 vPos;
		layout (location = 1) in vec4 vCol;
		layout (location = 2) in vec2 vTex;

		layout (std140, binding = 0) uniform Matrices
		{
			mat4 projection;
		};

		out vec4 oCol;
		out vec2 oTex;

		void main()
		{
			gl_Position = projection * vec4(vPos.x, vPos.y, 1.0, 1.0);
			oCol = vCol;
			oTex = vTex;
		}
	""";

	private const string _quadFragmentShaderSource = """
		#version 330 core
		in vec4 oCol;
		in vec2 oTex;

		uniform sampler2D u_Texture;

		out vec4 FragColor;

		void main()
		{
			FragColor = texture(u_Texture, oTex) * vec4(oCol.x, oCol.y, oCol.z, oCol.w);
		}
	""";

	private const string _textFragmentShaderSource = """
		#version 330 core
		in vec4 oCol;
		in vec2 oTex;

		uniform sampler2D u_Texture;

		out vec4 FragColor;

		float median(float r, float g, float b) {
		    return max(min(r, g), min(max(r, g), b));
		}

		float screenPxRange() {
		    vec2 unitRange = vec2(2.0) / vec2(textureSize(u_Texture, 0));
		    vec2 screenTexSize = vec2(1.0) / fwidth(oTex);
		    return max(0.5 * dot(unitRange, screenTexSize), 1.0);
		}

		void main()
		{
		    vec3 msd = texture(u_Texture, oTex).rgb;
		    float sd = median(msd.r, msd.g, msd.b);
		    float screenPxDistance = screenPxRange()*(sd - 0.5);
		    float opacity = clamp(screenPxDistance + 0.5, 0.0, 1.0);
		    FragColor = vec4(oCol.x, oCol.y, oCol.z, oCol.w * opacity);
		}
	""";
}
