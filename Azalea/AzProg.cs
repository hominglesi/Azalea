/*
using Azalea.Graphics.Colors;
using Azalea.Graphics.GLFW;
using Azalea.Graphics.GLFW.Enums;
using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using System;
using System.IO;
using System.Numerics;


namespace Azalea;
public static unsafe class AzProg
{
	private static Vector2Int _viewportSize;

	public static void Main()
	{
		if (GLFW.Init() == false)
		{
			Console.WriteLine("GLFW could not be initialized.");
			return;
		}


		GLFW.WindowHint(GLFWWindowHint.ContextVersionMajor, 3);
		GLFW.WindowHint(GLFWWindowHint.ContextVersionMinor, 3);
		GLFW.OpenGLProfileHint(GLFWOpenGLProfile.Core);

		var window = GLFW.CreateWindow(1080, 720, "Ide gas", null, null);
		_viewportSize = new(1080, 720);

		if (window == IntPtr.Zero)
		{
			Console.WriteLine("Window could not be created.");
			GLFW.Terminate();
			return;
		}

		GLFW.MakeContextCurrent(window);
		GL.Import();
		Console.WriteLine(GL.GetString(GLStringName.Version));

		GLFW.SetFramebufferSizeCallback(window, (_, width, height) =>
		{
			_viewportSize = new Vector2Int(width, height);
			GL.Viewport(0, 0, width, height);
		});

		GL.ClearColor(Palette.Aqua);

		float[] positions =
		{
			50, 250, 0.0f, 0.0f,
			350, 250, 1.0f, 0.0f,
			350, 50, 1.0f, 1.0f,
			50, 50, 0.0f, 1.0f
		};

		uint[] indices =
		{
			0, 1, 2,
			2, 3, 0
		};

		var vertexArray = new GLVertexArray();

		var vertexBuffer = new GLVertexBuffer();
		vertexBuffer.SetData(positions);

		var vbLayout = new GLVertexBufferLayout();
		vbLayout.AddElement<float>(2);
		vbLayout.AddElement<float>(2);

		vertexArray.AddBuffer(vertexBuffer, vbLayout);

		var indexBuffer = new GLIndexBuffer();
		indexBuffer.SetData(indices);

		var vertexShaderSource = File.ReadAllText("D:\\Programming\\Azalea\\Azalea\\Resources\\Shaders\\vertex_shader.glsl");
		var fragmentShaderSource = File.ReadAllText("D:\\Programming\\Azalea\\Azalea\\Resources\\Shaders\\fragment_shader.glsl");

		GL.Enable(GLCapability.Blend);
		GL.BlendFunc(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);

		var texture = new GLTexture("D:\\Programming\\Azalea\\Azalea\\Resources\\Textures\\azalea-icon.png");
		texture.Bind();

		var shader = new GLShader(vertexShaderSource, fragmentShaderSource);
		shader.SetUniform("u_Texture", 0);

		shader.Unbind();
		vertexArray.Unbind();
		vertexBuffer.Unbind();
		indexBuffer.Unbind();

		while (GLFW.WindowShouldClose(window) == false)
		{
			GL.Clear(GLBufferBit.Color);

			shader.Bind();
			vertexArray.Bind();
			indexBuffer.Bind();

			var projection = Matrix4x4.CreateOrthographicOffCenter(0, _viewportSize.X, _viewportSize.Y, 0, 0.1f, 100);
			shader.SetUniform("u_Projection", projection);

			GL.DrawElements(GLBeginMode.Triangles, 6, GLDataType.UnsignedInt, 0);

			GLFW.SwapBuffers(window);

			GLFW.PollEvents();

			GL.PrintErrors();
		}

		vertexArray.Dispose();
		vertexBuffer.Dispose();
		indexBuffer.Dispose();
		shader.Dispose();

		GLFW.Terminate();
	}
}


*/
