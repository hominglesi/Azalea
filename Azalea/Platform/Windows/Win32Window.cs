using Azalea.Graphics;
using Azalea.Graphics.OpenGL;
using Azalea.Inputs;
using Azalea.Utils;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal class Win32Window : PlatformWindowOld
{
	private readonly IntPtr _handle;
	private readonly IntPtr _deviceContext;

	private WindowStyles _style;
	private WindowStylesEx _styleEx;

	public Win32Window(string title, Vector2Int clientSize, WindowState state, bool visible)
		: base(title, clientSize, state)
	{
		var programHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		var cursor = WinAPI.LoadCursor(IntPtr.Zero, 32512);
		_windowProcedure = windowProcedure;
		var wndClass = new WindowClass("Azalea Window", programHandle, _windowProcedure)
		{
			Style = ClassStyles.OwnDC,
			Cursor = cursor
		};

		WinRectangle windowRect = new(100, 100, clientSize.X, clientSize.Y);
		_style = WindowStyles.Caption | WindowStyles.SysMenu
			| WindowStyles.MinimizeBox | WindowStyles.MaximizeBox | WindowStyles.SizeBox;
		_styleEx = WindowStylesEx.AppWindow;

		if (visible) _style |= WindowStyles.Visible;

		WinAPI.AdjustWindowRect(ref windowRect, _style, false, 0);

		var id = WinAPI.RegisterClass(ref wndClass);
		_handle = WinAPI.CreateWindow(
			_styleEx,
			id,
			title,
			_style,
			windowRect.X,
			windowRect.Y,
			windowRect.Width,
			windowRect.Height,
			IntPtr.Zero,
			IntPtr.Zero,
			programHandle,
			IntPtr.Zero);

		if (_handle == IntPtr.Zero)
		{
			Console.WriteLine($"Could not create Window. (Error {Marshal.GetLastWin32Error()})");
			return;
		}

		//Setup OpenGL
		_deviceContext = WinAPI.GetDC(_handle);

		var pfDescriptor = new PixelFormatDescriptor();
		var format = WinAPI.ChoosePixelFormat(_deviceContext, ref pfDescriptor);

		WinAPI.SetPixelFormat(_deviceContext, format, ref pfDescriptor);

		var glContext = GL.CreateContext(_deviceContext);
		GL.MakeCurrent(_deviceContext, glContext);
		GL.Import();

		//Sync values with PlatformWindow
		var windowSize = WinAPI.GetWindowRect(_handle).Size;
		UpdateSize(windowSize, clientSize);

		_visible = visible;
	}

	private readonly WindowProcedure _windowProcedure;
	private IntPtr windowProcedure(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
	{
		switch ((WindowMessage)message)
		{
			case WindowMessage.Close:
				Close();
				return IntPtr.Zero;
			case WindowMessage.Move:
				var windowPosition = WinAPI.GetWindowRect(_handle).Position;
				var clientPosition = BitwiseUtils.SplitValue(lParam);
				UpdatePosition(windowPosition, clientPosition);
				break;
			case WindowMessage.Size:
				var windowSize = WinAPI.GetWindowRect(_handle).Size;
				var clientSize = BitwiseUtils.SplitValue(lParam);

				switch ((ResizeReason)wParam)
				{
					case ResizeReason.Minimized:
						UpdateState(WindowState.Minimized); break;
					case ResizeReason.Maximized:
						UpdateState(WindowState.Maximized); break;
					default:
						var monitorSize = getCurrentMonitorInfo().Monitor.Size;
						if (monitorSize == windowSize)
							UpdateState(WindowState.Fullscreen);
						else
							UpdateState(WindowState.Normal); break;
				}

				UpdateSize(windowSize, clientSize);
				break;
			//Mouse Input
			case WindowMessage.LeftButtonDown:
				Input.HandleMouseButtonStateChange(MouseButton.Left, true); break;
			case WindowMessage.LeftButtonUp:
				Input.HandleMouseButtonStateChange(MouseButton.Left, false); break;
			case WindowMessage.RightButtonDown:
				Input.HandleMouseButtonStateChange(MouseButton.Right, true); break;
			case WindowMessage.RightButtonUp:
				Input.HandleMouseButtonStateChange(MouseButton.Right, false); break;
			case WindowMessage.MiddleButtonDown:
				Input.HandleMouseButtonStateChange(MouseButton.Middle, true); break;
			case WindowMessage.MiddleButtonUp:
				Input.HandleMouseButtonStateChange(MouseButton.Middle, false); break;
			case WindowMessage.XButtonDown:
				var xButtonDown = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.HandleMouseButtonStateChange(xButtonDown, true); break;
			case WindowMessage.XButtonUp:
				var xButtonUp = MouseButton.Middle + BitwiseUtils.GetHighOrderValue(wParam);
				Input.HandleMouseButtonStateChange(xButtonUp, false); break;
			case WindowMessage.MouseWheel:
				var delta = BitwiseUtils.GetHighOrderValue(wParam) / 120;
				Input.HandleScroll(delta); break;

			//Keyboad Input
			case WindowMessage.Char:
				Input.HandleTextInput((char)wParam); break;
			case WindowMessage.KeyDown:
				var isRepeat = BitwiseUtils.GetSpecificBit(lParam, 31);
				var key = WindowsExtentions.KeycodeToKey((int)wParam);
				if (isRepeat)
					Input.HandleKeyboardKeyRepeat(key);
				else
					Input.HandleKeyboardKeyStateChange(key, true);
				break;
			case WindowMessage.KeyUp:
				Input.HandleKeyboardKeyStateChange(WindowsExtentions.KeycodeToKey((int)wParam), false); break;
		}

		return WinAPI.DefWindowProc(window, message, wParam, lParam);
	}

	private Vector2Int _mousePosition;

	public override void ProcessEvents()
	{
		while (WinAPI.PeekMessage(out Message message, IntPtr.Zero) != 0)
		{
			_ = WinAPI.TranslateMessage(ref message);
			WinAPI.DispatchMessage(ref message);
		}

		//Update mouse position
		WinAPI.GetCursorPos(out _mousePosition);
		WinAPI.ScreenToClient(_handle, ref _mousePosition);
		Input.HandleMousePositionChange(_mousePosition);
	}

	#region Implementations

	protected override void SetSizeImplementation(Vector2Int size)
		=> WinAPI.SetWindowPos(_handle, IntPtr.Zero, 0, 0, size.X, size.Y, SetWindowPosFlags.NoMove);

	protected override void SetClientSizeImplementation(Vector2Int clientSize)
	{
		var newSize = new WinRectangle(Vector2Int.Zero, clientSize);
		WinAPI.AdjustWindowRect(ref newSize, _style, false, _styleEx);
		WinAPI.SetWindowPos(_handle, IntPtr.Zero, 0, 0, newSize.Width, newSize.Height, SetWindowPosFlags.NoMove);
	}

	protected override void SetPositionImplementation(Vector2Int position)
	{
		WinAPI.SetWindowPos(_handle, IntPtr.Zero, position.X, position.Y, 0, 0, SetWindowPosFlags.NoSize);
	}

	protected override void SetClientPositionImplementation(Vector2Int clientPosition)
	{
		var newPosition = new WinRectangle(clientPosition, Vector2Int.Zero);
		WinAPI.AdjustWindowRect(ref newPosition, _style, false, _styleEx);
		WinAPI.SetWindowPos(_handle, IntPtr.Zero, newPosition.X, newPosition.Y, 0, 0, SetWindowPosFlags.NoSize);
	}

	protected override void MinimizeImplementation()
		=> WinAPI.ShowWindow(_handle, ShowWindowCommand.Minimize);

	protected override void MaximizeImplementation()
		=> WinAPI.ShowWindow(_handle, ShowWindowCommand.Maximize);

	protected override void RestoreImplementation()
		=> WinAPI.ShowWindow(_handle, ShowWindowCommand.Restore);

	protected override void FullscreenImplementation()
	{
		var info = getCurrentMonitorInfo().Monitor;
		_style = getCurrentStyle() & ~(WindowStyles.Caption | WindowStyles.SizeBox);
		WinAPI.SetWindowStyle(_handle, _style);
		WinAPI.SetWindowPos(_handle, IntPtr.Zero, info.X, info.Y, info.Width, info.Height,
			SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoActivate | SetWindowPosFlags.FrameChanged);
	}

	protected override void RestoreFullscreenImplementation(Vector2Int lastPosition, Vector2Int lastSize)
	{
		_style = getCurrentStyle() | WindowStyles.Caption | WindowStyles.SizeBox;
		WinAPI.SetWindowStyle(_handle, _style);
		WinAPI.SetWindowPos(_handle, IntPtr.Zero, lastPosition.X, lastPosition.Y, lastSize.X, lastSize.Y,
			SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoActivate | SetWindowPosFlags.FrameChanged);
	}

	public override void Center()
	{
		var workAreaInfo = getCurrentMonitorInfo().WorkArea;
		Position = workAreaInfo.Position + (workAreaInfo.Size / 2 - Size / 2);
	}

	public override void Focus()
	{
		WinAPI.BringWindowToTop(_handle);
		WinAPI.SetForegroundWindow(_handle);
		WinAPI.SetFocus(_handle);
	}

	public override void RequestAttention()
		=> WinAPI.FlashWindow(_handle, true);



	protected override Vector2Int GetFullscreenSize() => getCurrentMonitorInfo().Monitor.Size;

	protected override void SetIconImplementation(Image? data)
	{
		IntPtr icon = IntPtr.Zero;

		if (data is not null)
		{
			icon = WinAPI.CreateIconFromImage(_deviceContext, data);
		}

		WinAPI.SendMessage(_handle, WindowMessage.SetIcon, (IntPtr)0, icon);
		WinAPI.SendMessage(_handle, WindowMessage.SetIcon, (IntPtr)1, icon);

		WinAPI.DeleteObject(icon);
	}


	protected override void SetResizableImplementation(bool enabled)
	{
		var style = getCurrentStyle();
		if (enabled)
			style |= WindowStyles.SizeBox | WindowStyles.MaximizeBox;
		else
			style &= ~(WindowStyles.SizeBox | WindowStyles.MaximizeBox);

		WinAPI.SetWindowStyle(_handle, style);
	}
	protected override void SetShouldCloseImplementation(bool shouldClose) { }
	protected override void SetTitleImplementation(string title)
		=> WinAPI.SetWindowText(_handle, title);
	protected override void SetVisibleImplementation(bool visible)
		=> WinAPI.ShowWindow(_handle, visible ? ShowWindowCommand.Show : ShowWindowCommand.Hide);
	protected override void SetVSyncImplementation(bool enabled)
		=> GL.SwapInterval(enabled ? 1 : 0);
	protected override void SwapBuffersImplementation() => WinAPI.SwapBuffers(_deviceContext);

	#endregion

	private WindowStyles getCurrentStyle() => (WindowStyles)WinAPI.GetWindowLong(_handle, (int)WindowLongValue.Style);
	private IntPtr getCurrentMonitor() => WinAPI.MonitorFromWindow(_handle, MonitorFromFlags.DefaultToNearest);
	private MonitorInfo getCurrentMonitorInfo() => WinAPI.GetMonitorInfo(getCurrentMonitor());

	protected override void OnDispose()
	{

	}
}
