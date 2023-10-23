using Azalea.Graphics.Colors;
using Azalea.Graphics.GLFW;
using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using System;

namespace Azalea;
public static unsafe class AzProg
{
	private static readonly string VertexShaderSource = @"
        #version 330

		layout(location = 0) in vec4 position;

        void main()
        {
            gl_Position = position;
        }
        ";

	//Fragment shaders are run on each fragment/pixel of the geometry.
	private static readonly string FragmentShaderSource = @"
        #version 330

        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0, 0.0, 0.0, 1.0);
        }
        ";

	public static void Main()
	{
		if (GLFW.Init() == false)
		{
			Console.WriteLine("GLFW could not be initialized.");
			return;
		}

		var window = GLFW.CreateWindow(1080, 720, "Ide gas", null, null);

		if (window == IntPtr.Zero)
		{
			Console.WriteLine("Window could not be created.");
			GLFW.Terminate();
			return;
		}

		GLFW.MakeContextCurrent(window);
		GL.Import();
		Console.WriteLine(GL.GetString(GLStringName.Version));

		GL.ClearColor(Palette.Aqua);

		float[] positions =
		{
			-0.5f, -0.5f,
			0.0f, 0.5f,
			0.5f, -0.5f,
		};

		var buffer = GL.GenBuffer();
		GL.BindBuffer(GLBufferType.Array, buffer);
		GL.BufferData(GLBufferType.Array, positions, GLUsageHint.StaticDraw);

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, GLDataType.Float, GLBool.False, sizeof(float) * 2, 0);

		uint shader = createShader(VertexShaderSource, FragmentShaderSource);
		GL.UseProgram(shader);

		while (GLFW.WindowShouldClose(window) == false)
		{
			GL.Clear(GLBufferBit.Color);

			GL.DrawArrays(GLBeginMode.Triangles, 0, 3);

			GLFW.SwapBuffers(window);

			GLFW.PollEvents();
		}

		GL.DeleteProgram(shader);

		GLFW.Terminate();
	}

	static uint createShader(string vertexShaderSource, string fragmentShaderSource)
	{
		var program = GL.CreateProgram();
		var vertexShader = compileShader(GLShaderType.Vertex, vertexShaderSource);
		var fragmentShader = compileShader(GLShaderType.Fragment, fragmentShaderSource);

		GL.AttachShader(program, vertexShader);
		GL.AttachShader(program, fragmentShader);

		GL.LinkProgram(program);
		GL.ValidateProgram(program);

		GL.DeleteShader(vertexShader);
		GL.DeleteShader(fragmentShader);

		return program;
	}

	static uint compileShader(GLShaderType type, string source)
	{
		var id = GL.CreateShader(type);
		GL.ShaderSource(id, source);
		GL.CompileShader(id);

		int result;
		GL.GetShaderiv(id, GLParameterName.CompileStatus, &result);
		if (result == 0)
		{

			int length;
			GL.GetShaderiv(id, GLParameterName.InfoLogLength, &length);
			Console.WriteLine(length);
			/*
			var str = new StringBuilder(length);
			for(int i = 0; i < length; i++)
			{
				str.Append((char)Marshal.ReadByte(result, i));
			}	*/
			Console.WriteLine("THERE IS AN ERROR!!!!");
			GL.DeleteShader(id);
			return 0;
		}

		return id;
	}
}


