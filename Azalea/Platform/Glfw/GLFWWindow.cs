
/*
namespace Azalea.Platform.Glfw;
internal class GLFWWindow : PlatformWindow
{
	public Window Handle { get; init; }

	private Monitor? _primaryMonitor;
	protected Monitor PrimaryMonitor => _primaryMonitor ??= GLFW.GetPrimaryMonitor();

	private VideoMode? _primaryMode;
	protected VideoMode PrimaryMode => _primaryMode ??= GLFW.GetVideoMode(PrimaryMonitor);

	public GLFWWindow(
		string title,
		int width,
		int height,
		WindowState state,
		bool visible,
		bool resizable = DEFAULT_RESIZABLE,
		bool decorated = DEFAULT_DECORATED,
		bool transparentFramebuffer = DEFAULT_TRANSPARENT_FRAMEBUFFER,
		bool vSync = DEFAULT_VSYNC) :
		base(title, new(width, height), state)
	{
		if (GLFW.Initialized == false && GLFW.Init() == false)
			Console.WriteLine("Could not initialize GLFW");

		_state = state;
		_visible = visible;
		//_resizable = resizable;
		//_decorated = decorated;

		GLFW.WindowHint(WindowHint.ContextVersionMajor, 3);
		GLFW.WindowHint(WindowHint.ContextVersionMinor, 3);
		GLFW.WindowHint(WindowHint.OpenGLProfile, (int)OpenGLProfile.Core);
		GLFW.WindowHint(WindowHint.CenterCursor, false);
		GLFW.WindowHint(WindowHint.Visible, _visible);
		//GLFW.WindowHint(WindowHint.Resizable, _resizable);
		//GLFW.WindowHint(WindowHint.Decorated, _decorated);
		GLFW.WindowHint(WindowHint.TransparentFramebuffer, transparentFramebuffer);


		//Setup Windowed Borderless
		GLFW.WindowHint(WindowHint.RedBits, PrimaryMode.RedBits);
		GLFW.WindowHint(WindowHint.GreenBits, PrimaryMode.GreenBits);
		GLFW.WindowHint(WindowHint.BlueBits, PrimaryMode.BlueBits);
		GLFW.WindowHint(WindowHint.RefreshRate, PrimaryMode.RefreshRate);

		if (_state == WindowState.Fullscreen)
			Handle = GLFW.CreateWindow(PrimaryMode.Width, PrimaryMode.Height, title, PrimaryMonitor, null);
		else
			Handle = GLFW.CreateWindow(width, height, title, null, null);

		GLFW.MakeContextCurrent(Handle);
		//We need to call this after context is made current
		VSync = vSync;

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
	}

	#region Callbacks

	private readonly GLFW.FramebufferSizeCallback _onResizeCallback;
private void onResize(Window _, int width, int height)
{
	//If we are in the process of going full screen or minimizing
	//we should not save the clientSize
	//if (_targetState == WindowState.BorderlessFullscreen ||
	//width == 0 || height == 0) return;

	UpdateSize(new(width, height), new(width, height));
}

private readonly GLFW.WindowPositionCallback _onMoveCallback;
private void onMove(Window _, int x, int y)
{
	//If we are in the process of going full screen we should not save the position
	//if (_targetState == WindowState.BorderlessFullscreen) return;

	UpdatePosition(new(x, y), new(x, y));
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
protected void SetOpacityImplementation(float opacity)
	=> GLFW.SetWindowOpacity(Handle, opacity);
protected override void SetPositionImplementation(Vector2Int position)
	=> GLFW.SetWindowPos(Handle, position.X, position.Y);
protected override void MaximizeImplementation()
{

}
protected override void RestoreImplementation()
{

}
protected override void SetClientPositionImplementation(Vector2Int clientPosition)
	=> SetPositionImplementation(clientPosition);
protected override void SetClientSizeImplementation(Vector2Int clientSize)
	=> GLFW.SetWindowSize(Handle, clientSize.X, clientSize.Y);
protected override void SetSizeImplementation(Vector2Int size)
	=> SetClientSizeImplementation(size);
public override void Center()
{

}
public override void RequestAttention()
	=> GLFW.RequestWindowAttention(Handle);
public override void Focus()
	=> GLFW.FocusWindow(Handle);
protected override void SetIconImplementation(Graphics.Image? data)
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
protected override void RestoreFullscreenImplementation(Vector2Int lastPosition, Vector2Int lastSize)
{
	var mode = GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());

	GLFW.SetWindowMonitor(Handle, IntPtr.Zero, lastPosition.X, lastPosition.Y, lastSize.X, lastSize.Y, mode.RefreshRate);
}

protected override void MinimizeImplementation()
	=> GLFW.IconifyWindow(Handle);

public override void ProcessEvents()
	=> GLFW.PollEvents();

#endregion

protected override void OnDispose()
{
	//We dont need to destroy the window because GLFW.Terminate() will do it automatically
	GLFW.Terminate();
}
}
*/
