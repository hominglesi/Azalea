using Azalea.Graphics.GLFW;
using Azalea.Graphics.GLFW.Enums;
using System;
using System.Diagnostics;
using System.IO;

namespace Azalea.Platform.Desktop;
public class GLFWWindow : IWindow
{
	public GLFW_Window Handle { get; init; }

	public Action<Vector2Int>? Resized;
	public Action? Initialized;
	public Action? Input;
	public Action? Update;
	public Action? Render;

	private Stopwatch _stopwatch = new();

	#region Callbacks

	private GLFW.FramebufferSizeCallback _onResizeCallback;
	private void onResize(GLFW_Window _, int width, int height) => Resized?.Invoke(new Vector2Int(width, height));

	#endregion

	public GLFWWindow(Vector2Int preferredClientSize)
	{
		if (GLFW.Init() == false) throw new Exception("GLFW could not be initialized.");


		GLFW.WindowHint(GLFWWindowHint.ContextVersionMajor, 3);
		GLFW.WindowHint(GLFWWindowHint.ContextVersionMinor, 3);
		GLFW.OpenGLProfileHint(GLFWOpenGLProfile.Core);

		Handle = GLFW.CreateWindow(preferredClientSize.X, preferredClientSize.Y, IWindow.DefaultTitle, null, null);
		GLFW.MakeContextCurrent(Handle);

		_onResizeCallback = onResize;
		GLFW.SetFramebufferSizeCallback(Handle, _onResizeCallback);
	}

	public void Run()
	{
		Initialized?.Invoke();
		while (GLFW.WindowShouldClose(Handle) == false)
		{

			/*
			//Get all inputs first
			Input?.Invoke();*/

			//Invoke all updates and calculate delta time
			if (_stopwatch.IsRunning == false) _stopwatch.Start();

			Time._deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
			_stopwatch.Restart();

			Update?.Invoke();

			//Render to screen
			Render?.Invoke();

			GLFW.PollEvents();
		}
	}

	public void SwapBuffers() => GLFW.SwapBuffers(Handle);

	public string Title { get => ""; set { } }
	public Vector2Int ClientSize { get => new(1280, 720); set { } }
	public WindowState State { get => WindowState.Normal; set { } }
	public bool Resizable { get => true; set { } }
	public bool CursorVisible { get => true; set { } }
	public Vector2Int Position { get => Vector2Int.Zero; set { } }

	public void Center()
	{

	}

	public void Close()
	{

	}

	public void SetIconFromStream(Stream imageStream)
	{

	}
}
