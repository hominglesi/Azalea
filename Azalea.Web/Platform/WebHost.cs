using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Web.Rendering;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Web.Platform;

public class WebHost : GameHost
{
	public override IWindow Window => _window ?? throw new Exception("Cannot use Window before it is initialized");
	private WebWindow _window;

	public override IRenderer Renderer => _renderer ?? throw new Exception("Cannot use Renderer before it is initialized");
	private WebGLRenderer? _renderer;

	public WebHost()
	{
		_window = new WebWindow() { };
	}

	public override void CallInitialized()
	{
		WebGL.Enable(GLCapability.Blend);
		WebGL.BlendFuncSeparate(GLBlendFunction.SrcAlpha, GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		_renderer = new WebGLRenderer(_window);

		base.CallInitialized();
	}

	private readonly float[] _vertices =
		[0.0f, 0.5f,
		-0.5f, -0.5f,
		0.5f, -0.5f];

	public override void Run(AzaleaGame game)
	{
		WebGL.ClearColor(0.5f, 0.5f, 1f, 1f);
		WebGL.Clear(GLBufferBit.Color);

		var vertexBuffer = WebGL.CreateBuffer();
		WebGL.BindBuffer(GLBufferType.Array, vertexBuffer);
		WebGL.BufferData(GLBufferType.Array, MemoryMarshal.AsBytes(_vertices.AsSpan()), GLUsageHint.DynamicDraw);

		var vertexShader = WebGL.CreateShader(GLShaderType.Vertex);
		WebGL.ShaderSource(vertexShader, Assets.GetText("Shaders/webgl_vertex_shader.glsl")!);
		WebGL.CompileShader(vertexShader);

		if (WebGL.GetShaderParameter(vertexShader, GLParameterName.CompileStatus) == false)
		{
			Console.WriteLine("Shader compile error: " + WebGL.GetShaderInfoLog(vertexShader));
		}

		var fragmentShader = WebGL.CreateShader(GLShaderType.Fragment);
		WebGL.ShaderSource(fragmentShader, Assets.GetText("Shaders/webgl_fragment_shader.glsl")!);
		WebGL.CompileShader(fragmentShader);

		if (WebGL.GetShaderParameter(fragmentShader, GLParameterName.CompileStatus) == false)
		{
			Console.WriteLine("Shader compile error: " + WebGL.GetShaderInfoLog(fragmentShader));
		}

		var program = WebGL.CreateProgram();
		WebGL.AttachShader(program, vertexShader);
		WebGL.AttachShader(program, fragmentShader);
		WebGL.LinkProgram(program);
		WebGL.UseProgram(program);

		var vPosLocation = WebGL.GetAttribLocation(program, "vertexPosition");
		WebGL.EnableVertexAttribArray(vPosLocation);
		WebGL.VertexAttribPointer(vPosLocation, 2, GLDataType.Float, false, 8, 0);

		WebGL.DrawArrays(GLBeginMode.Triangles, 0, 3);

		WebEvents.OnAnimationFrameRequested += runGameLoop;
		WebEvents.RequestAnimationFrame();
	}

	private WebGLIndexBuffer _indexBuffer;
	private WebGLVertexBuffer _vertexBuffer;
	private WebGLVertexArray _vertexArray;
	private WebGLShader _shader;
	private int[] _indices = [0, 1, 2, 1, 3, 2];

	private void runGameLoop()
	{
		Console.WriteLine(Assets.GetText("Text/test.txt"));
		WebEvents.RequestAnimationFrame();
	}
}
