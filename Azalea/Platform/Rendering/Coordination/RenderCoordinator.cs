using Azalea.Native.OpenGL;
using System;

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

	#region Queues

	public RenderCommandQueue CommandQueue =>
		_commandQueue is not null ? _commandQueue
			: throw new Exception("A command queue has not been started!");

	public RenderCommandQueue? _commandQueue;

	public RenderCommandQueue BeginCommandQueue()
	{
		if (_commandQueue is not null)
			throw new Exception("Only one command queue can be begun!");

		return _commandQueue = RenderCommandQueue.Borrow();
	}

	public RenderCommandQueue EndCommandQueue()
	{
		if (_commandQueue is null)
			throw new Exception("A command queue has not been started!");

		var commandQueue = _commandQueue;
		_commandQueue = null;
		return commandQueue;
	}
	#endregion

	public Program CreateStandardProgram(string vertexShaderSource, string fragmentShaderSource)
	{
		var vertexShader = Renderer.GenerateShader(GL.VERTEX_SHADER);
		Renderer.ShaderSource(vertexShader, vertexShaderSource);
		Renderer.CompileShader(vertexShader);
		Renderer.PrintShaderCompileStatus(vertexShader);

		var fragmentShader = Renderer.GenerateShader(GL.FRAGMENT_SHADER);
		Renderer.ShaderSource(fragmentShader, fragmentShaderSource);
		Renderer.CompileShader(fragmentShader);
		Renderer.PrintShaderCompileStatus(fragmentShader);

		var program = Renderer.GenerateProgram();
		Renderer.AttachShader(program, vertexShader);
		Renderer.AttachShader(program, fragmentShader);
		Renderer.LinkProgram(program);
		Renderer.PrintProgramCompileStatus(program);

		Renderer.DeleteShader(vertexShader);
		Renderer.DeleteShader(fragmentShader);

		return program;
	}
}
