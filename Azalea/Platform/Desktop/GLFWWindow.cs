using Azalea.Platform.Desktop.Glfw;
using Azalea.Platform.Desktop.Glfw.Enums;
using Azalea.Platform.Desktop.Glfw.Native;
using Azalea.Utils;
using System;
using System.IO;

namespace Azalea.Platform.Desktop;
public class GLFWWindow : Disposable, IWindow
{
	public GLFW_Window Handle { get; init; }

	public Action<Vector2Int>? Resized { get; set; }

	#region Callbacks

	private readonly GLFW.FramebufferSizeCallback _onResizeCallback;
	private void onResize(GLFW_Window _, int width, int height)
	{
		Resized?.Invoke(new Vector2Int(width, height));
		_clientSize = new Vector2Int(width, height);
	}

	private readonly GLFW.WindowPositionCallback _onMoveCallback;
	private void onMove(GLFW_Window _, int x, int y)
	{
		_position = new Vector2Int(x, y);
	}

	private readonly GLFW.WindowIconifyCallback _onInconifyCallback;
	private void onIconify(GLFW_Window window, int iconified)
	{
		if (iconified == 1)
		{
			setWindowState(WindowState.Minimized);
		}
		else
		{
			unminimize();
		}
	}

	private readonly GLFW.WindowMaximizeCallback _onMaximizeCallback;
	private void onMaximize(GLFW_Window window, int maximized)
	{
		if (maximized == 1)
		{
			setWindowState(WindowState.Maximized);
		}
		else
		{
			unmaximize();
		}
	}

	#endregion

	private int _titleBarHeight;

	public GLFWWindow(HostPreferences prefs)
	{
		if (GLFW.Init() == false) throw new Exception("GLFW could not be initialized.");

		_title = prefs.WindowTitle;
		_clientSize = prefs.PreferredClientSize;
		_resizable = prefs.WindowResizable;
		_state = WindowState.Normal;

		GLFW.WindowHint(GLFWWindowHint.ContextVersionMajor, 3);
		GLFW.WindowHint(GLFWWindowHint.ContextVersionMinor, 3);
		GLFW.WindowHint(GLFWWindowHint.OpenGLProfile, (int)GLFWOpenGLProfile.Core);
		GLFW.WindowHint(GLFWWindowHint.Resizable, _resizable);

		Handle = GLFW.CreateWindow(_clientSize.X, _clientSize.Y, _title, null, null);
		GLFW.MakeContextCurrent(Handle);

		_onResizeCallback = onResize;
		GLFW.SetFramebufferSizeCallback(Handle, _onResizeCallback);

		_onMoveCallback = onMove;
		GLFW.SetWindowPosCallback(Handle, _onMoveCallback);

		_onInconifyCallback = onIconify;
		GLFW.SetWindowIconifyCallback(Handle, _onInconifyCallback);

		_onMaximizeCallback = onMaximize;
		GLFW.SetWindowMaximizeCallback(Handle, _onMaximizeCallback);

		_titleBarHeight = (int)GLFW.GetWindowFrameSize(Handle).Top;
	}

	#region Title

	private string _title;

	public string Title { get => _title; set => setTitle(value); }

	private void setTitle(string title)
	{
		GLFW.SetWindowTitle(Handle, title);
		_title = title;
	}

	#endregion

	#region Resizable

	private bool _resizable;
	public bool Resizable { get => _resizable; set => setResizable(value); }

	private void setResizable(bool value)
	{
		if (_resizable == value) return;

		_resizable = value;
		GLFW.SetWindowAttribute(Handle, GLFWAttribute.Resizable, value);
	}

	#endregion

	public bool ShouldClose => GLFW.WindowShouldClose(Handle);

	public void SwapBuffers() => GLFW.SwapBuffers(Handle);

	private Vector2Int _position;
	public Vector2Int Position { get => _position; set => setWindowPosition(value); }

	private Vector2Int _clientSize;
	public Vector2Int ClientSize { get => _clientSize; set => GLFW.SetWindowSize(Handle, value.X, value.Y); }

	private bool _minimized;
	private bool _maximized;
	private WindowState _state;
	public WindowState State { get => _state; set => setWindowState(value); }


	public bool CursorVisible { get => true; set { } }


	public void Center()
	{

	}

	public void Close()
	{

	}

	public void SetIconFromStream(Stream imageStream)
	{

	}

	private void setWindowPosition(Vector2Int position)
	{
		if (State == WindowState.Normal)
		{
			GLFW.SetWindowPos(Handle, position.X, position.Y + _titleBarHeight);
		}
	}

	private WindowProperties? _lastStateProperties;

	private void unminimize()
	{
		WindowProperties props = _lastStateProperties ?? throw new Exception("When setting a window to minimized we need to save its properties");

		if (props.Maximized)
		{
			setWindowState(WindowState.Maximized);
			return;
		}

		setWindowState(WindowState.Normal);
	}

	private void unmaximize()
	{
		_maximized = false;
		if (_state == WindowState.Maximized)
			_state = WindowState.Normal;
	}

	private void setWindowState(WindowState state)
	{
		if (state == WindowState.Minimized)
		{
			if (_minimized == true) return;

			_lastStateProperties = new()
			{
				Minimized = false,
				Maximized = _maximized
			};

			_minimized = true;
			_maximized = false;

			GLFW.IconifyWindow(Handle);
			_state = WindowState.Minimized;
		}
		else if (state == WindowState.Maximized)
		{
			if (_maximized == true) return;
			_maximized = true;
			_minimized = false;

			GLFW.MaximizeWindow(Handle);
			_state = WindowState.Maximized;

		}
		else if (state == WindowState.Normal)
		{
			if (_minimized || _maximized)
			{
				_minimized = false;
				_maximized = false;

				GLFW.RestoreWindow(Handle);
			}
			_state = WindowState.Normal;
		}
	}

	protected override void OnDispose()
	{
		//We dont need to destroy the window because GLFW.Terminate() will do it automatically
		GLFW.Terminate();
	}

	private struct WindowProperties
	{
		public bool Minimized;
		public bool Maximized;
	}
}
