using Azalea.Editor.Design.Gui;
using Azalea.Graphics.Colors;
using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using Azalea.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azalea.Editor.DebugWindows;
internal class WindowingWindow
{
	private static GUIWindow? _window;
	private static bool _shown = false;

	public static void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private static readonly Dictionary<PlatformWindow, GUIGroup> _windowGroups = [];

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Windows", new(400, 400));

			PlatformWindow.OnWindowCreated += window =>
			{
				var group = _window.AddGroup(window.Title);
				_window.AddLabel("Window Type: " + window.PlatformType);

				if (window is WindowsWindow win)
				{
					// A bit hacky but it's for debugging purposes so it should be fine
					while (window.Initialized == false)
						Thread.Sleep(1);

					_window.AddLabel("Class Atom: " + win.ClassAtom);
					_window.AddLabel("Handle: " + win.Handle);
				}

				_window.AddButton("Close", () => window.Close());
				_window.FinishGroup();
				_windowGroups.Add(window, group);
			};
			PlatformWindow.OnWindowClosed += window =>
			{
				_window.RemoveElement(_windowGroups[window]);
				_windowGroups.Remove(window);
			};

			_window.AddLabel("Process Architecture: " + RuntimeInformation.ProcessArchitecture);
			_window.AddButton("Create new Window", () => PlatformWindow.Create());
			_window.AddButton("Create renderable new Window", () =>
			{
				var window = PlatformWindow.Create();
				var renderer = PlatformRenderer.AttachRenderer(window);
				var scheduler = PlatformScheduler.AttachScheduler(window);

				uint[] indices =
				[
					0, 1, 3,
					1, 2, 3
				];

				string vertexShaderSource = """
					#version 330 core
					layout (location = 0) in vec3 aPos;

					void main()
					{
						gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
					}
				""";

				string fragmentShaderSource = """
					#version 330 core
					out vec4 FragColor;

					uniform vec4 outColor;

					void main()
					{
						FragColor = outColor;
					} 
				""";

				renderer.BeginCommandGroup();

				renderer.Disable(GL.CULL_FACE);
				renderer.Disable(GL.DEPTH_TEST);

				var vertexShader = renderer.GenerateShader(GL.VERTEX_SHADER);
				renderer.ShaderSource(vertexShader, vertexShaderSource);
				renderer.CompileShader(vertexShader);
				renderer.PrintShaderCompileStatus(vertexShader);

				var fragmentShader = renderer.GenerateShader(GL.FRAGMENT_SHADER);
				renderer.ShaderSource(fragmentShader, fragmentShaderSource);
				renderer.CompileShader(fragmentShader);
				renderer.PrintShaderCompileStatus(fragmentShader);

				var program = renderer.GenerateProgram();
				renderer.AttachShader(program, vertexShader);
				renderer.AttachShader(program, fragmentShader);
				renderer.LinkProgram(program);
				renderer.PrintProgramCompileStatus(program);

				renderer.DeleteShader(vertexShader);
				renderer.DeleteShader(fragmentShader);

				var outColorUniform = renderer.GetUniformLocation(program, "outColor");

				var vertexArray = renderer.GenerateVertexArray();
				renderer.BindVertexArray(vertexArray);

				var vertexBuffer = renderer.GenerateBuffer();
				renderer.BindBuffer(GL.ARRAY_BUFFER, vertexBuffer);

				var indexArray = renderer.GenerateBuffer();
				renderer.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, indexArray);
				renderer.BufferData(GL.ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(uint), indices, GL.STATIC_DRAW, false);

				renderer.VertexAttribPointer(0, 3, GL.FLOAT, false, 3 * sizeof(float), 0);
				renderer.EnableVertexAttribArray(0);

				renderer.BindVertexArray(null);

				renderer.PrintErrors();

				renderer.SubmitCommandGroup();

				scheduler.InjectProtocol((win, rend) =>
				{
					var renderQueue = RenderCommandQueue.Borrow();
					renderQueue.Clear(Palette.Beige);

					renderQueue.UseProgram(program);
					var blueValue = MathUtils.Map(MathF.Sin(Platform.Time.TimeSinceStart), -1, 1, 0, 1);
					renderQueue.Uniform4f(outColorUniform, 0, blueValue, 1 - blueValue, 1);

					renderQueue.BindVertexArray(vertexArray);

					var dynamicVertices = ArrayPool<float>.Shared.Rent(12);

					if (Rng.Int(2) == 0)
					{
						dynamicVertices[0] = 0.5f;
						dynamicVertices[1] = 0.5f;
						dynamicVertices[2] = 0.0f;
						dynamicVertices[3] = 0.5f;
						dynamicVertices[4] = -0.5f;
						dynamicVertices[5] = 0.0f;
						dynamicVertices[6] = -0.5f;
						dynamicVertices[7] = -0.5f;
						dynamicVertices[8] = 0.0f;
						dynamicVertices[9] = -0.5f;
						dynamicVertices[10] = 0.5f;
						dynamicVertices[11] = 0.0f;
					}
					else
					{
						dynamicVertices[0] = 0.5f;
						dynamicVertices[1] = 0.5f;
						dynamicVertices[2] = 0.0f;
						dynamicVertices[3] = 0.5f;
						dynamicVertices[4] = -0.5f;
						dynamicVertices[5] = 0.0f;
						dynamicVertices[6] = -0.6f;
						dynamicVertices[7] = -0.6f;
						dynamicVertices[8] = 0.0f;
						dynamicVertices[9] = -0.5f;
						dynamicVertices[10] = 0.5f;
						dynamicVertices[11] = 0.0f;
					}

					renderQueue.BufferData(GL.ARRAY_BUFFER, 12 * sizeof(float), dynamicVertices, GL.STATIC_DRAW, true);

					renderQueue.DrawElements(GL.TRIANGLES, 6, GL.UNSIGNED_INT, 0);

					renderQueue.PrintErrors();

					renderQueue.SwapBuffers();

					renderer.StageQueue(renderQueue);
				});
			});
		}

		_window.Show();
	}

	private static void hide()
	{
		if (_window is null)
			return;

		_window.Hide();
	}
}
