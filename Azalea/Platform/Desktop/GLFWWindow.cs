/*

using Azalea.Graphics.Textures;
using Azalea.Platform.Glfw;
using System;
using System.IO;

namespace Azalea.Platform.Desktop;
internal class GLFWWindow : PlatformWindow, IWindow
{ 
	#region Callbacks

	private readonly GLFW.WindowIconifyCallback _onInconifyCallback;
	private void onIconify(PlatformWindow window, int iconified)
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
	private void onMaximize(PlatformWindow window, int maximized)
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
		_state = WindowState.Normal;

		_onInconifyCallback = onIconify;
		GLFW.SetWindowIconifyCallback(Handle, _onInconifyCallback);

		_onMaximizeCallback = onMaximize;
		GLFW.SetWindowMaximizeCallback(Handle, _onMaximizeCallback);

		VSync = prefs.VSync;
		Title = prefs.WindowTitle;
		Resizable = prefs.WindowResizable;
	}


	private bool _minimized;
	private bool _maximized;
	private bool _fullscreen;
	private WindowState _state;
	public WindowState State { get => _state; set => setWindowState(value); }

	private void unminimize()
	{
		_minimized = false;

		if (_preMinimizedMaximized)
		{
			_maximized = true;
			_state = WindowState.Maximized;
			return;
		}

		if (_preMinimizedFullscreen)
		{
			_fullscreen = true;
			_state = WindowState.Fullscreen;
			return;
		}

		_state = WindowState.Normal;
	}

	private void unmaximize()
	{
		if (_maximized)
		{
			_maximized = false;

			_state = WindowState.Normal;
		}
	}

	private bool _preMinimizedMaximized;
	private bool _preMinimizedFullscreen;
	private Vector2Int _lastPosition;
	private Vector2Int _lastSize;

	private void setWindowState(WindowState state)
	{
		if (_state == state) return;

		if (state == WindowState.Normal)
		{
			if (_fullscreen)
			{
				var refresh = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor()).RefreshRate;
				GLFW.SetWindowMonitor(Handle, IntPtr.Zero, _lastPosition.X, _lastPosition.Y, _lastSize.X, _lastSize.Y, refresh);

				_fullscreen = false;
			}

			if (_minimized)
			{
				GLFW.RestoreWindow(Handle);

				_minimized = false;
			}

			if (_maximized)
			{
				GLFW.RestoreWindow(Handle);

				_maximized = false;
			}
		}
		else if (state == WindowState.Minimized)
		{
			_preMinimizedMaximized = _maximized;
			_preMinimizedFullscreen = _fullscreen;

			GLFW.IconifyWindow(Handle);

			_maximized = false;
			_fullscreen = false;

			_minimized = true;
		}
		else if (state == WindowState.Maximized)
		{
			if (_fullscreen)
			{
				var refresh = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor()).RefreshRate;
				GLFW.SetWindowMonitor(Handle, IntPtr.Zero, _lastPosition.X, _lastPosition.Y, _lastSize.X, _lastSize.Y, refresh);

				_fullscreen = false;
			}

			GLFW.MaximizeWindow(Handle);

			_minimized = false;

			_maximized = true;
		}
		else if (state == WindowState.Fullscreen)
		{
			_lastPosition = _position;
			_lastSize = _clientSize;

			var monitor = GLFW.GetPrimaryMonitor();
			var mode = GLFW.GetVideoMode(monitor);
			GLFW.SetWindowMonitor(Handle, monitor, 0, 0, mode.Width, mode.Height, mode.RefreshRate);

			_minimized = false;
			_maximized = false;

			_position = Vector2Int.Zero;
			_clientSize = new Vector2Int(mode.Width, mode.Height);

			_fullscreen = true;
		}

		_state = state;
	}

	
}
*/
