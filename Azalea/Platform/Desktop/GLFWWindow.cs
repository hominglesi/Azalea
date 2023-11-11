using Azalea.Graphics.Textures;
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
		GLFW.WindowHint(GLFWWindowHint.Visible, false);

		//Windowed borderless
		var mode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());
		GLFW.WindowHint(GLFWWindowHint.RedBits, mode.RedBits);
		GLFW.WindowHint(GLFWWindowHint.GreenBits, mode.GreenBits);
		GLFW.WindowHint(GLFWWindowHint.BlueBits, mode.BlueBits);
		GLFW.WindowHint(GLFWWindowHint.RefreshRate, mode.RefreshRate);

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

		_onCloseCallback = onClose;
		GLFW.SetWindowCloseCallback(Handle, _onCloseCallback);

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
	#region Position

	private Vector2Int _position;
	public Vector2Int Position { get => _position; set => setPosition(value); }

	private void setPosition(Vector2Int position)
	{
		if (State == WindowState.Normal)
		{
			GLFW.SetWindowPos(Handle, position.X, position.Y + _titleBarHeight);
		}
	}

	private readonly GLFW.WindowPositionCallback _onMoveCallback;
	private void onMove(GLFW_Window _, int x, int y)
	{
		_position = new Vector2Int(x, y);
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
	#region Visibility

	public void Show() => GLFW.ShowWindow(Handle);

	public void Hide() => GLFW.HideWindow(Handle);

	#endregion
	#region Closing

	private readonly GLFW.WindowCloseCallback _onCloseCallback;
	private void onClose(GLFW_Window window)
	{
		ShouldClose = true;
		Closing?.Invoke();
	}

	public Action? Closing { get; set; }

	public void Close()
	{
		GLFW.SetWindowShouldClose(Handle, true);
		onClose(Handle);
	}

	public void PreventClosure()
	{
		ShouldClose = false;
	}

	public bool ShouldClose { get; set; }

	#endregion

	public void SwapBuffers() => GLFW.SwapBuffers(Handle);

	private Vector2Int _clientSize;
	public Vector2Int ClientSize { get => _clientSize; set => GLFW.SetWindowSize(Handle, value.X, value.Y); }

	private bool _minimized;
	private bool _maximized;
	private bool _fullscreen;
	private WindowState _state;
	public WindowState State { get => _state; set => setWindowState(value); }


	public bool CursorVisible { get => true; set { } }


	public void Center()
	{
		var primaryMonitor = GLFW.GetPrimaryMonitor();
		var workareaSize = GLFW.GetMonitorWorkarea(primaryMonitor);

		Position = workareaSize / 2 - ClientSize / 2;
	}

	public void RequestAttention()
	{
		GLFW.RequestWindowAttention(Handle);
	}

	public void SetIconFromStream(Stream? imageStream)
	{
		if (imageStream is null)
		{
			GLFW.SetWindowIcon(Handle, null);
			return;
		}

		var data = new TextureData(TextureData.LoadFromStream(imageStream));
		GLFW.SetWindowIcon(Handle, data);
	}

	private WindowProperties _lastStateProperties;

	private void unminimize()
	{
		if (_lastStateProperties.Maximized)
		{
			setWindowState(WindowState.Maximized);
			return;
		}

		if (_lastStateProperties.Fullscreen)
		{
			setWindowState(WindowState.Fullscreen);
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

	private void fullscreen()
	{
		if (_fullscreen) return;

		_lastStateProperties.Maximized = _maximized;
		_lastStateProperties.Minimized = _minimized;
		_lastStateProperties.Size = ClientSize;
		_lastStateProperties.Position = Position;

		var monitor = GLFW.GetPrimaryMonitor();
		var mode = GLFW.GetVideoMode(monitor);

		GLFW.SetWindowMonitor(Handle, monitor, 0, 0, mode.Width, mode.Height, mode.RefreshRate);

		_clientSize = new Vector2Int(mode.Width, mode.Height);
		_position = Vector2Int.Zero;

		_fullscreen = true;
		_maximized = false;
		_minimized = false;
		_state = WindowState.BorderlessFullscreen;
	}

	private void unfullscreen()
	{
		var monitor = GLFW.GetPrimaryMonitor();
		var mode = GLFW.GetVideoMode(monitor);

		WindowProperties restored = _lastStateProperties;
		GLFW.SetWindowMonitor(Handle, IntPtr.Zero, restored.Position.Y, restored.Position.X, restored.Size.X, restored.Size.Y, mode.RefreshRate);

		_fullscreen = false;
		_clientSize = restored.Size;
		_position = restored.Position;

		if (restored.Maximized) maximize();
		if (restored.Minimized) minimize();
	}

	private void maximize()
	{
		if (_maximized) return;
		if (_fullscreen) return;

		_maximized = true;
		_minimized = false;

		GLFW.MaximizeWindow(Handle);
		_state = WindowState.Maximized;
	}

	private void minimize()
	{
		if (_minimized) return;

		_lastStateProperties.Minimized = false;
		_lastStateProperties.Maximized = _maximized;
		_lastStateProperties.Fullscreen = _fullscreen;

		_minimized = true;
		_maximized = false;
		_fullscreen = false;

		GLFW.IconifyWindow(Handle);
		_state = WindowState.Minimized;
	}

	private void restore()
	{
		if (_minimized || _maximized)
		{
			_minimized = false;
			_maximized = false;

			GLFW.RestoreWindow(Handle);
		}

		if (_fullscreen)
		{
			unfullscreen();
		}

		_state = WindowState.Normal;
	}

	private void setWindowState(WindowState state)
	{
		if (state == WindowState.Minimized)
		{
			minimize();
		}
		else if (state == WindowState.Maximized)
		{
			maximize();
		}
		else if (state == WindowState.Normal)
		{
			restore();
		}
		else if (state == WindowState.Fullscreen || state == WindowState.BorderlessFullscreen)
		{
			fullscreen();
		}

		Resized?.Invoke(ClientSize);
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
		public bool Fullscreen;
		public Vector2Int Position;
		public Vector2Int Size;
	}
}
