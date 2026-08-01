using Azalea.Graphics;
using Azalea.Graphics.OpenGL;
using Azalea.Inputs;
using Azalea.Native.Windows;
using Azalea.Platform.Windows.Com;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Azalea.Platform.Windows;
internal class Win32Window : PlatformWindow
{
	public nint Handle { get; }
	public nint DeviceContext { get; }

	private readonly Win32.WNDPROC _windowProcedure;
	private readonly WindowState _initialShowState;

	private readonly XInputManager _xInputManager;
	private readonly Dictionary<uint, WindowsTrayIcon> _trayIcons = [];

	public Win32Window(string title, Vector2Int clientSize, WindowState state, bool visible)
		: base(title, clientSize, state)
	{
		_initialShowState = state;
		var processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		_windowProcedure = windowProcedure;
		var winProcPtr = Marshal.GetFunctionPointerForDelegate(_windowProcedure);
		var classNamePtr = Marshal.StringToHGlobalUni("Azalea Window");
		var wndClass = new Win32.WNDCLASSEXW
		{
			lpszClassName = classNamePtr,
			hInstance = processHandle,
			lpfnWndProc = winProcPtr,
			style = Win32.WNDCLASSEXW.ClassStyles.OWNDC,
			hCursor = WinAPI.LoadCursor(nint.Zero, 32512)
		};

		Win32.RECT windowRect = new(100, 100, clientSize.X, clientSize.Y);
		var style = Win32.WindowStyles.CAPTION | Win32.WindowStyles.SYSMENU
			| Win32.WindowStyles.MINIMIZEBOX | Win32.WindowStyles.MAXIMIZEBOX
			| Win32.WindowStyles.SIZEBOX;

		if (visible) style |= Win32.WindowStyles.VISIBLE;

		var styleEx = Win32.WindowStylesExtended.APPWINDOW;

		Win32.AdjustWindowRectEx(ref windowRect, style, false, styleEx);

		var atom = Win32.RegisterClassExW(ref wndClass);
		Marshal.FreeHGlobal(classNamePtr);

		Handle = Win32.CreateWindowExW(
			styleEx,
			atom,
			title,
			style,
			windowRect.left,
			windowRect.top,
			windowRect.Width,
			windowRect.Height,
			IntPtr.Zero,
			IntPtr.Zero,
			processHandle,
			IntPtr.Zero);

		if (Handle == IntPtr.Zero)
		{
			Console.WriteLine($"Could not create Window. (Error {Marshal.GetLastWin32Error()})");
			return;
		}

		_xInputManager = new XInputManager();
		Input.SetGamepadManager(_xInputManager);

		if (WinAPI.OleInitialize(0) == 0)
			_ = WinAPI.RegisterDragDrop(Handle, new DropTarget(this));
		else
			Console.WriteLine("The Main method has not been marked with an [STAThread] attribute. You may experience some strange behaviours.");

		//Setup OpenGL
		DeviceContext = Win32.GetDC(Handle);

		initializeOpenGL();

		var pixelFormatAttribs = new int[]
		{
			Native.OpenGL.GL.WGL_DRAW_TO_WINDOW_ARB, 1,
			Native.OpenGL.GL.WGL_SUPPORT_OPENGL_ARB, 1,
			Native.OpenGL.GL.WGL_DOUBLE_BUFFER_ARB, 1,
			Native.OpenGL.GL.WGL_ACCELERATION_ARB, Native.OpenGL.GL.WGL_FULL_ACCELERATION_ARB,
			Native.OpenGL.GL.WGL_PIXEL_TYPE_ARB, Native.OpenGL.GL.WGL_TYPE_RGBA_ARB,
			Native.OpenGL.GL.WGL_COLOR_BITS_ARB, 32,
			Native.OpenGL.GL.WGL_DEPTH_BITS_ARB, 24,
			Native.OpenGL.GL.WGL_STENCIL_BITS_ARB, 8,
			0
		};

		int pixelFormat = 0;
		uint formatCount = 0;
		GL.ChoosePixelFormatARB(DeviceContext, ref pixelFormatAttribs[0], IntPtr.Zero, 1, ref pixelFormat, ref formatCount);

		Win32.PIXELFORMATDESCRIPTOR pixelFormatDescriptor = new();
		_ = Win32.DescribePixelFormat(DeviceContext, pixelFormat, pixelFormatDescriptor.nSize, ref pixelFormatDescriptor);
		Win32.SetPixelFormat(DeviceContext, pixelFormat, in pixelFormatDescriptor);

		var openGLAttribs = new int[]
		{
			Native.OpenGL.GL.WGL_CONTEXT_MAJOR_VERSION_ARB, 3,
			Native.OpenGL.GL.WGL_CONTEXT_MINOR_VERSION_ARB, 3,
			Native.OpenGL.GL.WGL_CONTEXT_PROFILE_MASK_ARB, Native.OpenGL.GL.WGL_CONTEXT_CORE_PROFILE_BIT_ARB,
			0
		};

		var glContext = GL.CreateContextAttribsARB(DeviceContext, false, ref openGLAttribs[0]);
		Win32.wglMakeCurrent(DeviceContext, glContext);

		//Sync values with PlatformWindow
		Win32.GetWindowRect(Handle, out windowRect);
		var windowSize = new Vector2Int(windowRect.Width, windowRect.Height);
		UpdateSize(windowSize, clientSize);
	}

	private void initializeOpenGL()
	{
		// We need to create a dummy window to be able to load its drawing context functions
		// because we need them when creating the real drawing context

		var processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		var classNamePtr = Marshal.StringToHGlobalUni("Dummy Window");
		var winProcPtr = Marshal.GetFunctionPointerForDelegate(_windowProcedure);
		var dummywindowClass = new Win32.WNDCLASSEXW()
		{
			lpszClassName = classNamePtr,
			hInstance = processHandle,
			lpfnWndProc = winProcPtr,
			style = Win32.WNDCLASSEXW.ClassStyles.HREDRAW
			| Win32.WNDCLASSEXW.ClassStyles.VREDRAW | Win32.WNDCLASSEXW.ClassStyles.OWNDC
		};

		var atom = Win32.RegisterClassExW(ref dummywindowClass);
		Marshal.FreeHGlobal(classNamePtr);

		var dummyWindow = Win32.CreateWindowExW(
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

		if (Handle == IntPtr.Zero)
		{
			Console.WriteLine($"Could not create dummy window. (Error {Marshal.GetLastWin32Error()})");
			return;
		}

		var dummyDC = Win32.GetDC(dummyWindow);
		var pfDescriptor = new Win32.PIXELFORMATDESCRIPTOR();
		var pixelFormat = Win32.ChoosePixelFormat(dummyDC, in pfDescriptor);

		Win32.SetPixelFormat(dummyDC, pixelFormat, in pfDescriptor);

		var dummyContext = Win32.wglCreateContext(dummyDC);
		Win32.wglMakeCurrent(dummyDC, dummyContext);

		GL.ImportFunctions();

		Win32.wglMakeCurrent(dummyDC, IntPtr.Zero);
		GL.DeleteContext(dummyContext);
		WinAPI.ReleaseDC(dummyWindow, dummyDC);
		WinAPI.DestroyWindow(dummyWindow);
	}

	private IntPtr windowProcedure(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
	{
		switch ((Win32.WindowMessage)message)
		{
			case Win32.WindowMessage.MOVE:
				Win32.GetWindowRect(Handle, out var rect);
				var windowPosition = new Vector2Int(rect.X, rect.Y);
				var clientPosition = BitwiseUtils.SplitValue(lParam);
				UpdatePosition(windowPosition, clientPosition);
				break;
			case Win32.WindowMessage.SIZE:
				Win32.GetWindowRect(Handle, out rect);
				var windowSize = new Vector2Int(rect.Width, rect.Height);
				var clientSize = BitwiseUtils.SplitValue(lParam);
				UpdateSize(windowSize, clientSize);

				var monitor = getCurrentMonitorInfo().Monitor;
				var monitorSize = new Vector2Int(monitor.Width, monitor.Height);
				var resizeReason = (ResizeReason)wParam;
				if (monitorSize == windowSize && (getCurrentStyle() & Win32.WindowStyles.CAPTION) == 0)
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
			case Win32.WindowMessage.CLOSE:
				Close();
				return IntPtr.Zero;

			//Mouse Input
			case Win32.WindowMessage.LBUTTONDOWN:
				Input.ExecuteMouseButtonStateChange(MouseButton.Left, true);
				WinAPI.SetCapture(Handle);
				break;
			case Win32.WindowMessage.LBUTTONUP:
				Input.ExecuteMouseButtonStateChange(MouseButton.Left, false);
				WinAPI.ReleaseCapture();
				break;
			case Win32.WindowMessage.RBUTTONDOWN:
				Input.ExecuteMouseButtonStateChange(MouseButton.Right, true); break;
			case Win32.WindowMessage.RBUTTONUP:
				Input.ExecuteMouseButtonStateChange(MouseButton.Right, false); break;
			case Win32.WindowMessage.MBUTTONDOWN:
				Input.ExecuteMouseButtonStateChange(MouseButton.Middle, true); break;
			case Win32.WindowMessage.MBUTTONUP:
				Input.ExecuteMouseButtonStateChange(MouseButton.Middle, false); break;
			case Win32.WindowMessage.XBUTTONDOWN:
				var xButtonDown = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.ExecuteMouseButtonStateChange(xButtonDown, true); break;
			case Win32.WindowMessage.XBUTTONUP:
				var xButtonUp = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.ExecuteMouseButtonStateChange(xButtonUp, false); break;
			case Win32.WindowMessage.MOUSEWHEEL:
				var delta = BitwiseUtils.GetHighOrderValue(wParam) / 120;
				Input.ExecuteScroll(delta); break;

			//Keyboad Input
			case Win32.WindowMessage.CHAR:
				Input.ExecuteTextInput((char)wParam); break;
			case Win32.WindowMessage.KEYDOWN:
				var isRepeat = BitwiseUtils.GetSpecificBit(lParam, 31);
				var downKey = WindowsExtentions.KeycodeToKey((int)wParam);
				handleKeyDown(downKey, isRepeat);
				break;
			case Win32.WindowMessage.KEYUP:
				Input.ExecuteKeyboardKeyStateChange(WindowsExtentions.KeycodeToKey((int)wParam), false); break;
			case Win32.WindowMessage.SYSKEYDOWN:
				var downSysKey = WindowsExtentions.KeycodeToKey((int)wParam);

				if (downSysKey == Keys.F10 || downSysKey == Keys.AltLeft)
				{
					var downSysKeyIsRepeat = BitwiseUtils.GetSpecificBit(lParam, 31);
					handleKeyDown(downSysKey, downSysKeyIsRepeat);
					return IntPtr.Zero;
				}

				break;
			case Win32.WindowMessage.AZ_TRAYICON:
				var iconId = (uint)wParam;
				var iconEvent = (Win32.WindowMessage)BitwiseUtils.GetLowOrderValue(lParam);

				if (_trayIcons.TryGetValue(iconId, out var icon))
				{
					switch (iconEvent)
					{
						case Win32.WindowMessage.MOUSEMOVE:
						case Win32.WindowMessage.LBUTTONDOWN:
						case Win32.WindowMessage.RBUTTONDOWN:
						case Win32.WindowMessage.MBUTTONDOWN:
							break;
						case Win32.WindowMessage.LBUTTONUP:
							icon.InvokeClick(MouseButton.Left);
							break;
						case Win32.WindowMessage.RBUTTONUP:
							icon.InvokeClick(MouseButton.Right);
							break;
						case Win32.WindowMessage.MBUTTONUP:
							icon.InvokeClick(MouseButton.Middle);
							break;
						case Win32.WindowMessage.LBUTTONDBLCLK:
							icon.InvokeDoubleClick(MouseButton.Left);
							break;
						case Win32.WindowMessage.RBUTTONDBLCLK:
							icon.InvokeDoubleClick(MouseButton.Right);
							break;
						case Win32.WindowMessage.MBUTTONDBLCLK:
							icon.InvokeDoubleClick(MouseButton.Middle);
							break;
					}
				}
				break;
		}

		return Win32.DefWindowProcW(window, message, wParam, lParam);
	}

	private class DropTarget(Win32Window window) : IDropTarget
	{
		public int DragEnter(nint dataObject, uint keyState, Vector2Int point, ref uint effect) => 0;
		public int DragLeave() => 0;

		public int DragOver(uint keyState, Vector2Int point, ref uint effect)
		{
			effect = Input.OverDroppableFile ? 1u : 0u;

			return 0;
		}

		public int Drop(IDataObject dataObject, uint keyState, Vector2Int point, ref uint effect)
		{
			var format = new FORMATETC()
			{
				cfFormat = 15, // CF_HDROP
				dwAspect = DVASPECT.DVASPECT_CONTENT,
				tymed = TYMED.TYMED_HGLOBAL
			};

			string[] files;
			dataObject.GetData(ref format, out STGMEDIUM medium);

			try
			{
				IntPtr dropHandle = medium.unionmember;
				int fileCount = WinAPI.DragQueryFile(dropHandle, uint.MaxValue, null, 0);
				files = new string[fileCount];
				for (uint x = 0; x < fileCount; ++x)
				{
					int size = WinAPI.DragQueryFile(dropHandle, x, null, 0);
					if (size > 0)
					{
						StringBuilder fileName = new StringBuilder(size + 1);
						if (WinAPI.DragQueryFile(dropHandle, x, fileName, (uint)fileName.Capacity) > 0)
							files[x] = fileName.ToString();
					}
				}
			}
			finally { WinAPI.ReleaseStgMedium(ref medium); }

			Input.ExecuteFileDropped(files);

			return 0;
		}
	}

	internal void AddTrayIcon(WindowsTrayIcon trayIcon)
		=> _trayIcons.Add(trayIcon.Handle, trayIcon);

	internal void RemoveTrayIcon(WindowsTrayIcon trayIcon)
		=> _trayIcons.Remove(trayIcon.Handle);

	private void handleKeyDown(Keys key, bool isRepeat)
	{
		if (isRepeat)
			Input.ExecuteKeyboardKeyRepeat(key);
		else
			Input.ExecuteKeyboardKeyStateChange(key, true);
	}

	#region Implementations

	protected override void SetSizeImplementation(Vector2Int size)
		=> WinAPI.SetWindowPos(Handle, IntPtr.Zero, 0, 0, size.X, size.Y, SetWindowPosFlags.NoMove);

	protected override void SetClientSizeImplementation(Vector2Int clientSize)
	{
		var newSize = new Win32.RECT(0, 0, clientSize.X, clientSize.Y);
		Win32.AdjustWindowRectEx(ref newSize, getCurrentStyle(), false, getCurrentStyleEx());
		WinAPI.SetWindowPos(Handle, IntPtr.Zero, 0, 0, newSize.Width, newSize.Height, SetWindowPosFlags.NoMove);
	}

	protected override void SetPositionImplementation(Vector2Int position)
		=> WinAPI.SetWindowPos(Handle, IntPtr.Zero, position.X, position.Y, 0, 0, SetWindowPosFlags.NoSize);

	protected override void SetClientPositionImplementation(Vector2Int clientPosition)
	{
		var newPosition = new Win32.RECT(clientPosition.X, clientPosition.Y, 0, 0);
		Win32.AdjustWindowRectEx(ref newPosition, getCurrentStyle(), false, getCurrentStyleEx());
		WinAPI.SetWindowPos(Handle, IntPtr.Zero, newPosition.X, newPosition.Y, 0, 0, SetWindowPosFlags.NoSize);
	}

	protected override void MinimizeImplementation()
		=> WinAPI.ShowWindow(Handle, ShowWindowCommand.Minimize);

	protected override void MaximizeImplementation()
		=> WinAPI.ShowWindow(Handle, ShowWindowCommand.Maximize);

	protected override void RestoreImplementation()
		=> WinAPI.ShowWindow(Handle, ShowWindowCommand.Restore);

	protected override void FullscreenImplementation()
	{
		var monitor = getCurrentMonitorInfo().Monitor;
		var newStyle = getCurrentStyle() & ~(Win32.WindowStyles.CAPTION | Win32.WindowStyles.SIZEBOX);
		WinAPI.SetWindowStyle(Handle, newStyle);
		WinAPI.SetWindowPos(Handle, IntPtr.Zero, monitor.X, monitor.Y, monitor.Width, monitor.Height,
			SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoActivate | SetWindowPosFlags.FrameChanged);
	}

	protected override void RestoreFullscreenImplementation(Vector2Int lastPosition, Vector2Int lastSize)
	{
		var newStyle = getCurrentStyle() | Win32.WindowStyles.CAPTION | Win32.WindowStyles.SIZEBOX;
		WinAPI.SetWindowStyle(Handle, newStyle);
		WinAPI.SetWindowPos(Handle, IntPtr.Zero, lastPosition.X, lastPosition.Y, lastSize.X, lastSize.Y,
			SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoActivate | SetWindowPosFlags.FrameChanged);
	}

	protected override void SetTitleImplementation(string title)
		=> WinAPI.SetWindowText(Handle, title);

	protected override void SetResizableImplementation(bool enabled)
	{
		var newStyle = getCurrentStyle();
		if (enabled)
			newStyle |= Win32.WindowStyles.SIZEBOX | Win32.WindowStyles.MAXIMIZEBOX;
		else
			newStyle &= ~(Win32.WindowStyles.SIZEBOX | Win32.WindowStyles.MAXIMIZEBOX);

		WinAPI.SetWindowStyle(Handle, newStyle);
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

	public override void Center()
	{
		var workArea = getCurrentMonitorInfo().WorkArea;
		Position = new Vector2Int(workArea.X, workArea.Y)
			+ (new Vector2Int(workArea.Width, workArea.Height) / 2 - Size / 2);
	}

	public override void Focus()
	{
		WinAPI.BringWindowToTop(Handle);
		WinAPI.SetForegroundWindow(Handle);
		WinAPI.SetFocus(Handle);
	}

	public override void RequestAttention()
		=> WinAPI.FlashWindow(Handle, true);

	protected override void SetIconImplementation(Image? data)
	{
		IntPtr icon = IntPtr.Zero;

		if (data is not null)
			icon = WinAPI.CreateIconFromImage(DeviceContext, data);

		WinAPI.SendMessage(Handle, Win32.WindowMessage.SETICON, IntPtr.Zero, icon);
		WinAPI.SendMessage(Handle, Win32.WindowMessage.SETICON, IntPtr.Zero + 1, icon);

		if (icon != IntPtr.Zero)
			WinAPI.DeleteObject(icon);
	}

	public override void SwapBuffers()
		=> Win32.SwapBuffers(DeviceContext);

	public override void Show(bool firstTime)
	{
		if (firstTime == false)
		{
			WinAPI.ShowWindow(Handle, ShowWindowCommand.Show);
			return;
		}

		switch (_initialShowState)
		{
			case WindowState.Normal:
				WinAPI.ShowWindow(Handle, ShowWindowCommand.ShowNormal); return;
			case WindowState.Maximized:
				WinAPI.ShowWindow(Handle, ShowWindowCommand.ShowMaximized); return;
			case WindowState.Minimized:
				WinAPI.ShowWindow(Handle, ShowWindowCommand.ShowMinimized); return;
			case WindowState.Fullscreen:
				WinAPI.ShowWindow(Handle, ShowWindowCommand.ShowNormal);
				State = WindowState.Fullscreen; return;
		}
	}

	public override void Hide()
		=> WinAPI.ShowWindow(Handle, ShowWindowCommand.Hide);

	private Vector2Int _mousePosition;
	public override void ProcessEvents()
	{
		while (Win32.PeekMessageW(out Win32.MSG message, Handle, 0, 0, 0x0001) != 0)
		{
			Win32.TranslateMessage(in message);
			Win32.DispatchMessageW(in message);
		}

		// Update mouse position
		WinAPI.GetCursorPos(out _mousePosition);
		WinAPI.ScreenToClient(Handle, ref _mousePosition);
		Input.ExecuteMousePositionChange(_mousePosition);

		// Update (gamepads)
		_xInputManager.Update();
	}

	#endregion

	private Win32.WindowStyles getCurrentStyle() => (Win32.WindowStyles)WinAPI.GetWindowLong(Handle, (int)WindowLongValue.Style);
	private Win32.WindowStylesExtended getCurrentStyleEx() => (Win32.WindowStylesExtended)WinAPI.GetWindowLong(Handle, (int)WindowLongValue.ExStyle);
	private IntPtr getCurrentMonitor() => WinAPI.MonitorFromWindow(Handle, MonitorFromFlags.DefaultToNearest);
	private MonitorInfo getCurrentMonitorInfo() => WinAPI.GetMonitorInfo(getCurrentMonitor());

	protected override void OnDispose()
	{

	}
}
