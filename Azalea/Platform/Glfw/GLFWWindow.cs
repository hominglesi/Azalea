using Azalea.Graphics.Textures;
using System;

namespace Azalea.Platform.Glfw;
internal class GLFWWindow : PlatformWindow
{
	public Window Handle { get; init; }

	private Monitor? _primaryMonitor;
	protected Monitor PrimaryMonitor => _primaryMonitor ??= GLFW.GetPrimaryMonitor();

	private VideoMode? _primaryMode;
	protected VideoMode PrimaryMode => _primaryMode ??= GLFW.GetVideoMode(PrimaryMonitor);

	public GLFWWindow(
		string title = DEFAULT_TITLE,
		int width = DEFAULT_CLIENT_WIDTH,
		int height = DEFAULT_CLIENT_HEIGHT,
		WindowState state = DEFAULT_STATE,
		bool visible = DEFAULT_VISIBLE,
		bool resizable = DEFAULT_RESIZABLE,
		bool decorated = DEFAULT_DECORATED,
		bool transparentFramebuffer = DEFAULT_TRANSPARENT_FRAMEBUFFER,
		bool vSync = DEFAULT_VSYNC)
	{
		if (GLFW.Initialized == false && GLFW.Init() == false)
			Console.WriteLine("Could not initialize GLFW");

		_title = title;
		_clientWidth = width;
		_clientHeight = height;
		_state = state;
		_visible = visible;
		_resizable = resizable;
		_decorated = decorated;

		GLFW.WindowHint(WindowHint.ContextVersionMajor, 3);
		GLFW.WindowHint(WindowHint.ContextVersionMinor, 3);
		GLFW.WindowHint(WindowHint.OpenGLProfile, (int)OpenGLProfile.Core);
		GLFW.WindowHint(WindowHint.CenterCursor, false);
		GLFW.WindowHint(WindowHint.Visible, _visible);
		GLFW.WindowHint(WindowHint.Resizable, _resizable);
		GLFW.WindowHint(WindowHint.Decorated, _decorated);
		GLFW.WindowHint(WindowHint.TransparentFramebuffer, transparentFramebuffer);


		//Setup Windowed Borderless
		GLFW.WindowHint(WindowHint.RedBits, PrimaryMode.RedBits);
		GLFW.WindowHint(WindowHint.GreenBits, PrimaryMode.GreenBits);
		GLFW.WindowHint(WindowHint.BlueBits, PrimaryMode.BlueBits);
		GLFW.WindowHint(WindowHint.RefreshRate, PrimaryMode.RefreshRate);

		if (_state == WindowState.BorderlessFullscreen)
			Handle = GLFW.CreateWindow(PrimaryMode.Width, PrimaryMode.Height, _title, PrimaryMonitor, null);
		else
			Handle = GLFW.CreateWindow(_clientWidth, _clientHeight, _title, null, null);

		GLFW.MakeContextCurrent(Handle);
		//We need to call this after context is made current
		VSync = vSync;

		_onResizeCallback = onResize;
		GLFW.SetFramebufferSizeCallback(Handle, _onResizeCallback);

		_onMoveCallback = onMove;
		GLFW.SetWindowPosCallback(Handle, _onMoveCallback);

		_onInconifyCallback = onIconify;
		GLFW.SetWindowIconifyCallback(Handle, _onInconifyCallback);

		/*
		_onMaximizeCallback = onMaximize;
		GLFW.SetWindowMaximizeCallback(Handle, _onMaximizeCallback); */

		_onCloseCallback = onClose;
		GLFW.SetWindowCloseCallback(Handle, _onCloseCallback);
	}

	#region Callbacks

	private readonly GLFW.FramebufferSizeCallback _onResizeCallback;
	private void onResize(Window _, int width, int height)
	{
		//If we are in the process of going full screen or minimizing
		//we should not save the clientSize
		if (_targetState == WindowState.BorderlessFullscreen ||
			width == 0 || height == 0) return;

		_clientSize = new(width, height);
	}

	private readonly GLFW.WindowPositionCallback _onMoveCallback;
	private void onMove(Window _, int x, int y)
	{
		//If we are in the process of going full screen we should not save the position
		if (_targetState == WindowState.BorderlessFullscreen) return;

		_position = new(x, y);
	}

	private readonly GLFW.WindowCloseCallback _onCloseCallback;
	private void onClose(Window window)
		=> Close();

	private readonly GLFW.WindowIconifyCallback _onInconifyCallback;
	private void onIconify(Window window, int iconified)
	{
		//if (iconified == 1)
		//{
		//	setWindowState(WindowState.Minimized);
		//}
		//else
		//{
		//Resized?.Invoke(ClientSize);
		//}
	}

	#endregion

	#region Implementations

	protected override void SetTitleImplementation(string title)
		=> GLFW.SetWindowTitle(Handle, title);
	protected override void SetVSyncImplementation(bool enabled)
		=> GLFW.SwapInterval(enabled ? 1 : 0);
	protected override void SetResizableImplementation(bool enabled)
		=> GLFW.SetWindowAttribute(Handle, WindowAttribute.Resizable, enabled);
	protected override void SetDecoratedImplementation(bool enabled)
		=> GLFW.SetWindowAttribute(Handle, WindowAttribute.Decorated, enabled);
	protected override void SetOpacityImplementation(float opacity)
		=> GLFW.SetWindowOpacity(Handle, opacity);
	protected override void SetPositionImplementation(int x, int y)
		=> GLFW.SetWindowPos(Handle, x, y);
	protected override void SetClientSizeImplementation(int width, int height)
		=> GLFW.SetWindowSize(Handle, width, height);
	protected override Vector2Int GetWorkareaSizeImplementation()
		=> GLFW.GetMonitorWorkarea(GLFW.GetPrimaryMonitor());
	protected override void RequestAttentionImplementation()
		=> GLFW.RequestWindowAttention(Handle);
	protected override void FocusImplementation()
		=> GLFW.FocusWindow(Handle);
	protected override void SetIconImplementation(ITextureData? data)
		=> GLFW.SetWindowIcon(Handle, data);
	protected override void SetShouldCloseImplementation(bool shouldClose)
		=> GLFW.SetWindowShouldClose(Handle, shouldClose);
	protected override void SwapBuffersImplementation()
		=> GLFW.SwapBuffers(Handle);
	protected override Vector2Int GetFullscreenSize()
		=> new(PrimaryMode.Width, PrimaryMode.Height);
	protected override void SetVisibleImplementation(bool visible)
	{
		if (visible) GLFW.ShowWindow(Handle);
		else GLFW.HideWindow(Handle);
	}
	protected override void FullscreenImplementation()
	{
		GLFW.SetWindowMonitor(Handle, PrimaryMonitor, 0, 0,
			PrimaryMode.Width, PrimaryMode.Height, PrimaryMode.RefreshRate);
	}
	protected override void RestoreFullscreenImplementation(int lastX, int lastY, int lastWidth, int lastHeight)
	{
		var mode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());

		GLFW.SetWindowMonitor(Handle, IntPtr.Zero, lastX, lastY, lastWidth, lastHeight, mode.RefreshRate);
	}

	protected override void MinimizeImplementation()
		=> GLFW.IconifyWindow(Handle);

	#endregion

	protected override void OnDispose()
	{
		//We dont need to destroy the window because GLFW.Terminate() will do it automatically
		GLFW.Terminate();
	}
}
