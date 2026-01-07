using Azalea.Graphics;
using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Inputs;
using Azalea.Platform.Windows.ComInterfaces;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal class Win32Window : PlatformWindow
{
	private readonly IntPtr _window;
	private readonly IntPtr _deviceContext;
	private readonly WindowProcedure _windowProcedure;
	private readonly WindowState _initialShowState;

	private readonly XInputManager _xInputManager;
	private readonly DropTarget _dropTarget;

	private readonly nint _normalPointer;
	private readonly nint _blockedPointer;
	private readonly nint _handPointer;

	public Win32Window(string title, Vector2Int clientSize, WindowState state, bool visible)
		: base(title, clientSize, state)
	{
		_initialShowState = state;
		var processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		_normalPointer = WinAPI.LoadCursor(IntPtr.Zero, 32512);
		_blockedPointer = WinAPI.LoadCursor(IntPtr.Zero, 32648);
		_handPointer = WinAPI.LoadCursor(IntPtr.Zero, 32649);

		_windowProcedure = windowProcedure;
		var wndClass = new WindowClass("Azalea Window", processHandle, _windowProcedure)
		{
			Style = ClassStyles.OwnDC,
			Cursor = _normalPointer
		};

		WinRectangle windowRect = new(100, 100, clientSize.X, clientSize.Y);
		var style = WindowStyles.Caption | WindowStyles.SysMenu
			| WindowStyles.MinimizeBox | WindowStyles.MaximizeBox | WindowStyles.SizeBox;

		if (visible) style |= WindowStyles.Visible;

		var styleEx = WindowStylesEx.AppWindow | WindowStylesEx.AcceptFiles;

		WinAPI.AdjustWindowRect(ref windowRect, style, false, styleEx);

		var atom = WinAPI.RegisterClass(ref wndClass);
		_window = WinAPI.CreateWindow(
			styleEx,
			atom,
			title,
			style,
			windowRect.X,
			windowRect.Y,
			windowRect.Width,
			windowRect.Height,
			IntPtr.Zero,
			IntPtr.Zero,
			processHandle,
			IntPtr.Zero);

		if (_window == IntPtr.Zero)
		{
			Console.WriteLine($"Could not create Window. (Error {Marshal.GetLastWin32Error()})");
			return;
		}

		_xInputManager = new XInputManager();
		Input.SetGamepadManager(_xInputManager);

		if (WinAPI.OleInitialize(0) == 0)
			_ = WinAPI.RegisterDragDrop(_window, _dropTarget = new DropTarget(this));
		else
			Console.WriteLine("The Main method has not been marked with an [STAThread] attribute. You may experience some strange behaviours.");

		//Setup OpenGL
		_deviceContext = WinAPI.GetDC(_window);

		initializeOpenGL();

		var pixelFormatAttribs = new int[]
		{
			(int)WGLAttribute.DrawToWindow, 1,
			(int)WGLAttribute.SupportOpenGL, 1,
			(int)WGLAttribute.DoubleBuffer, 1,
			(int)WGLAttribute.Acceleration, (int)WGLAttribute.FullAcceleration,
			(int)WGLAttribute.PixelType, (int)WGLAttribute.TypeRGBA,
			(int)WGLAttribute.ColorBits, 32,
			(int)WGLAttribute.DepthBits, 24,
			(int)WGLAttribute.StencilBits, 8,
			0
		};

		int pixelFormat = 0;
		uint formatCount = 0;
		GL.ChoosePixelFormatARB(_deviceContext, ref pixelFormatAttribs[0], IntPtr.Zero, 1, ref pixelFormat, ref formatCount);

		PixelFormatDescriptor pixelFormatDescriptor = default;
		_ = WinAPI.DescribePixelFormat(_deviceContext, pixelFormat, (uint)Marshal.SizeOf<PixelFormatDescriptor>(), ref pixelFormatDescriptor);
		WinAPI.SetPixelFormat(_deviceContext, pixelFormat, ref pixelFormatDescriptor);

		var openGLAttribs = new int[]
		{
			(int)WGLAttribute.ContextMajorVersion, 3,
			(int)WGLAttribute.ContextMinorVersion, 3,
			(int)WGLAttribute.ContextProfileMask, (int)WGLAttribute.ContextCoreProfileBit,
			0
		};

		var glContext = GL.CreateContextAttribsARB(_deviceContext, false, ref openGLAttribs[0]);
		GL.MakeCurrent(_deviceContext, glContext);

		//Sync values with PlatformWindow
		var windowSize = WinAPI.GetWindowRect(_window).Size;
		UpdateSize(windowSize, clientSize);
		SetAcceptFiles(AcceptFiles);
	}

	private void initializeOpenGL()
	{
		// We need to create a dummy window to be able to load its drawing context functions
		// because we need them when creating the real drawing context

		var processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		var dummywindowClass = new WindowClass("Dummy Window", processHandle, _windowProcedure)
		{
			Style = ClassStyles.HorizontalReDraw | ClassStyles.VerticalReDraw | ClassStyles.OwnDC
		};

		var atom = WinAPI.RegisterClass(ref dummywindowClass);
		var dummyWindow = WinAPI.CreateWindow(
			0,
			atom,
			"Dummy Window",
			0,
			(unchecked((int)0x80000000)), //CW_USEDEFAULT
			(unchecked((int)0x80000000)), //CW_USEDEFAULT
			(unchecked((int)0x80000000)), //CW_USEDEFAULT
			(unchecked((int)0x80000000)), //CW_USEDEFAULT
			IntPtr.Zero,
			IntPtr.Zero,
			processHandle,
			IntPtr.Zero);

		if (_window == IntPtr.Zero)
		{
			Console.WriteLine($"Could not create dummy window. (Error {Marshal.GetLastWin32Error()})");
			return;
		}

		var dummyDC = WinAPI.GetDC(dummyWindow);
		var pfDescriptor = new PixelFormatDescriptor();
		var pixelFormat = WinAPI.ChoosePixelFormat(dummyDC, ref pfDescriptor);

		WinAPI.SetPixelFormat(dummyDC, pixelFormat, ref pfDescriptor);

		var dummyContext = GL.CreateContext(dummyDC);
		GL.MakeCurrent(dummyDC, dummyContext);

		GL.ImportFunctions();

		GL.MakeCurrent(dummyDC, IntPtr.Zero);
		GL.DeleteContext(dummyContext);
		WinAPI.ReleaseDC(dummyWindow, dummyDC);
		WinAPI.DestroyWindow(dummyWindow);
	}

	private IntPtr windowProcedure(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
	{
		switch ((WindowMessage)message)
		{
			case WindowMessage.Move:
				var windowPosition = WinAPI.GetWindowRect(_window).Position;
				var clientPosition = BitwiseUtils.SplitValue(lParam);
				UpdatePosition(windowPosition, clientPosition);
				break;
			case WindowMessage.Size:
				var windowSize = WinAPI.GetWindowRect(_window).Size;
				var clientSize = BitwiseUtils.SplitValue(lParam);
				UpdateSize(windowSize, clientSize);

				var monitorSize = getCurrentMonitorInfo().Monitor.Size;
				var resizeReason = (ResizeReason)wParam;
				if (monitorSize == windowSize && (getCurrentStyle() & WindowStyles.Caption) == 0)
				{
					UpdateState(WindowState.Fullscreen);
					break;
				}

				UpdateState(resizeReason switch
				{
					ResizeReason.Minimized => WindowState.Minimized,
					ResizeReason.Maximized => WindowState.Maximized,
					_ => WindowState.Normal,
				});
				break;
			case WindowMessage.Close:
				Close();
				return IntPtr.Zero;

			//Mouse Input
			case WindowMessage.LeftButtonDown:
				Input.ExecuteMouseButtonStateChange(MouseButton.Left, true);
				WinAPI.SetCapture(_window);
				break;
			case WindowMessage.LeftButtonUp:
				Input.ExecuteMouseButtonStateChange(MouseButton.Left, false);
				WinAPI.ReleaseCapture();
				break;
			case WindowMessage.RightButtonDown:
				Input.ExecuteMouseButtonStateChange(MouseButton.Right, true); break;
			case WindowMessage.RightButtonUp:
				Input.ExecuteMouseButtonStateChange(MouseButton.Right, false); break;
			case WindowMessage.MiddleButtonDown:
				Input.ExecuteMouseButtonStateChange(MouseButton.Middle, true); break;
			case WindowMessage.MiddleButtonUp:
				Input.ExecuteMouseButtonStateChange(MouseButton.Middle, false); break;
			case WindowMessage.XButtonDown:
				var xButtonDown = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.ExecuteMouseButtonStateChange(xButtonDown, true); break;
			case WindowMessage.XButtonUp:
				var xButtonUp = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.ExecuteMouseButtonStateChange(xButtonUp, false); break;
			case WindowMessage.MouseWheel:
				var delta = BitwiseUtils.GetHighOrderValue(wParam) / 120;
				Input.ExecuteScroll(delta); break;

			//Keyboad Input
			case WindowMessage.Char:
				Input.ExecuteTextInput((char)wParam); break;
			case WindowMessage.KeyDown:
				var isRepeat = BitwiseUtils.GetSpecificBit(lParam, 31);
				var downKey = WindowsExtentions.KeycodeToKey((int)wParam);
				handleKeyDown(downKey, isRepeat);
				break;
			case WindowMessage.KeyUp:
				Input.ExecuteKeyboardKeyStateChange(WindowsExtentions.KeycodeToKey((int)wParam), false); break;
			case WindowMessage.SysKeyDown:
				var downSysKey = WindowsExtentions.KeycodeToKey((int)wParam);

				if (downSysKey == Keys.F10)
				{
					var downSysKeyIsRepeat = BitwiseUtils.GetSpecificBit(lParam, 31);
					handleKeyDown(downSysKey, downSysKeyIsRepeat);
					return IntPtr.Zero;
				}

				break;
		}

		return WinAPI.DefWindowProc(window, message, wParam, lParam);
	}

	[ComVisible(true)]
	[Guid("00000122-0000-0000-C000-000000000046")]
	private class DropTarget(Win32Window window) : IDropTarget
	{
		private readonly Win32Window _window = window;

		public int DragEnter(nint dataObject, uint keyState, Vector2Int point, ref uint effect)
		{
			return 0;
		}

		public int DragLeave()
		{
			return 0;
		}

		public int DragOver(uint keyState, Vector2Int point, ref uint effect)
		{
			effect = _window.AcceptFiles ? 1u : 0u;

			return 0;
		}

		public int Drop(nint dataObject, uint keyState, Vector2Int point, ref uint effect)
		{
			return 0;
		}
	}

	private void handleKeyDown(Keys key, bool isRepeat)
	{
		if (isRepeat)
			Input.ExecuteKeyboardKeyRepeat(key);
		else
			Input.ExecuteKeyboardKeyStateChange(key, true);
	}

	#region Implementations

	protected override void SetSizeImplementation(Vector2Int size)
		=> WinAPI.SetWindowPos(_window, IntPtr.Zero, 0, 0, size.X, size.Y, SetWindowPosFlags.NoMove);

	protected override void SetClientSizeImplementation(Vector2Int clientSize)
	{
		var newSize = new WinRectangle(Vector2Int.Zero, clientSize);
		WinAPI.AdjustWindowRect(ref newSize, getCurrentStyle(), false, getCurrentStyleEx());
		WinAPI.SetWindowPos(_window, IntPtr.Zero, 0, 0, newSize.Width, newSize.Height, SetWindowPosFlags.NoMove);
	}

	protected override void SetPositionImplementation(Vector2Int position)
		=> WinAPI.SetWindowPos(_window, IntPtr.Zero, position.X, position.Y, 0, 0, SetWindowPosFlags.NoSize);

	protected override void SetClientPositionImplementation(Vector2Int clientPosition)
	{
		var newPosition = new WinRectangle(clientPosition, Vector2Int.Zero);
		WinAPI.AdjustWindowRect(ref newPosition, getCurrentStyle(), false, getCurrentStyleEx());
		WinAPI.SetWindowPos(_window, IntPtr.Zero, newPosition.X, newPosition.Y, 0, 0, SetWindowPosFlags.NoSize);
	}

	protected override void MinimizeImplementation()
		=> WinAPI.ShowWindow(_window, ShowWindowCommand.Minimize);

	protected override void MaximizeImplementation()
		=> WinAPI.ShowWindow(_window, ShowWindowCommand.Maximize);

	protected override void RestoreImplementation()
		=> WinAPI.ShowWindow(_window, ShowWindowCommand.Restore);

	protected override void FullscreenImplementation()
	{
		var monitor = getCurrentMonitorInfo().Monitor;
		var newStyle = getCurrentStyle() & ~(WindowStyles.Caption | WindowStyles.SizeBox);
		WinAPI.SetWindowStyle(_window, newStyle);
		WinAPI.SetWindowPos(_window, IntPtr.Zero, monitor.X, monitor.Y, monitor.Width, monitor.Height,
			SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoActivate | SetWindowPosFlags.FrameChanged);
	}

	protected override void RestoreFullscreenImplementation(Vector2Int lastPosition, Vector2Int lastSize)
	{
		var newStyle = getCurrentStyle() | WindowStyles.Caption | WindowStyles.SizeBox;
		WinAPI.SetWindowStyle(_window, newStyle);
		WinAPI.SetWindowPos(_window, IntPtr.Zero, lastPosition.X, lastPosition.Y, lastSize.X, lastSize.Y,
			SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoActivate | SetWindowPosFlags.FrameChanged);
	}

	protected override void SetTitleImplementation(string title)
		=> WinAPI.SetWindowText(_window, title);

	protected override void SetResizableImplementation(bool enabled)
	{
		var newStyle = getCurrentStyle();
		if (enabled)
			newStyle |= WindowStyles.SizeBox | WindowStyles.MaximizeBox;
		else
			newStyle &= ~(WindowStyles.SizeBox | WindowStyles.MaximizeBox);

		WinAPI.SetWindowStyle(_window, newStyle);
	}

	protected override void SetVSyncImplementation(bool enabled)
		=> GL.SwapInterval(enabled ? 1 : 0);

	protected override bool GetVSyncImplementation()
		=> GL.GetSwapInterval() == 1;

	protected override bool GetCanChangeVSyncImplementation()
	{
		var current = GetVSyncImplementation();

		SetVSyncImplementation(!current);
		var changed = current != GetVSyncImplementation();

		SetVSyncImplementation(current);
		return changed;
	}

	protected override void SetCursorVisible(bool show) => WinAPI.ShowCursor(show);
	protected override void SetAcceptFiles(bool show) => WinAPI.DragAcceptFiles(_window, show);

	public override void Center()
	{
		var workArea = getCurrentMonitorInfo().WorkArea;
		Position = workArea.Position + (workArea.Size / 2 - Size / 2);
	}

	public override void Focus()
	{
		WinAPI.BringWindowToTop(_window);
		WinAPI.SetForegroundWindow(_window);
		WinAPI.SetFocus(_window);
	}

	public override void RequestAttention()
		=> WinAPI.FlashWindow(_window, true);

	protected override void SetIconImplementation(Image? data)
	{
		IntPtr icon = IntPtr.Zero;

		if (data is not null)
			icon = WinAPI.CreateIconFromImage(_deviceContext, data);

		WinAPI.SendMessage(_window, WindowMessage.SetIcon, IntPtr.Zero, icon);
		WinAPI.SendMessage(_window, WindowMessage.SetIcon, IntPtr.Zero + 1, icon);

		if (icon != IntPtr.Zero)
			WinAPI.DeleteObject(icon);
	}

	public override void SwapBuffers()
		=> WinAPI.SwapBuffers(_deviceContext);

	public override void Show(bool firstTime)
	{
		if (firstTime == false)
		{
			WinAPI.ShowWindow(_window, ShowWindowCommand.Show);
			return;
		}

		switch (_initialShowState)
		{
			case WindowState.Normal:
				WinAPI.ShowWindow(_window, ShowWindowCommand.ShowNormal); return;
			case WindowState.Maximized:
				WinAPI.ShowWindow(_window, ShowWindowCommand.ShowMaximized); return;
			case WindowState.Minimized:
				WinAPI.ShowWindow(_window, ShowWindowCommand.ShowMinimized); return;
			case WindowState.Fullscreen:
				WinAPI.ShowWindow(_window, ShowWindowCommand.ShowNormal);
				State = WindowState.Fullscreen; return;
		}
	}

	public override void Hide()
		=> WinAPI.ShowWindow(_window, ShowWindowCommand.Hide);

	private Vector2Int _mousePosition;
	public override void ProcessEvents()
	{
		while (WinAPI.PeekMessage(out Message message, IntPtr.Zero) != 0)
		{
			WinAPI.TranslateMessage(ref message);
			WinAPI.DispatchMessage(ref message);
		}

		// Update mouse position
		WinAPI.GetCursorPos(out _mousePosition);
		WinAPI.ScreenToClient(_window, ref _mousePosition);
		Input.ExecuteMousePositionChange(_mousePosition);

		// Update (gamepads)
		_xInputManager.Update();
	}

	#endregion

	private WindowStyles getCurrentStyle() => (WindowStyles)WinAPI.GetWindowLong(_window, (int)WindowLongValue.Style);
	private WindowStylesEx getCurrentStyleEx() => (WindowStylesEx)WinAPI.GetWindowLong(_window, (int)WindowLongValue.ExStyle);
	private IntPtr getCurrentMonitor() => WinAPI.MonitorFromWindow(_window, MonitorFromFlags.DefaultToNearest);
	private MonitorInfo getCurrentMonitorInfo() => WinAPI.GetMonitorInfo(getCurrentMonitor());

	protected override void OnDispose()
	{

	}
}
