using Azalea.Graphics.GLFW;
using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using System;

namespace Azalea;
public static class AzProg
{
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
		GL.ClearColor(0, 1, 1, 1);

		while (GLFW.WindowShouldClose(window) == false)
		{
			GL.Clear(GLBufferBits.Color);

			GL.Begin(GLBeginMode.Triangles);

			GL.Vertex2f(-0.5f, -0.5f);
			GL.Vertex2f(0.0f, 0.5f);
			GL.Vertex2f(0.5f, -0.5f);

			GL.End();

			GLFW.SwapBuffers(window);

			GLFW.PollEvents();
		}

		GLFW.Terminate();
	}







}
