using Azalea.Graphics;
using Azalea.Graphics.OpenGL;
using Azalea.Inputs;
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

	public Win32Window(string title, Vector2Int clientSize, WindowState state, bool visible)
		: base(title, clientSize, state)
	{
		_initialShowState = state;
		var processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		_windowProcedure = windowProcedure;
		var wndClass = new WindowClass("Azalea Window", processHandle, _windowProcedure)
		{
			Style = ClassStyles.OwnDC,
			Cursor = WinAPI.LoadCursor(IntPtr.Zero, 32512)
		};

		WinRectangle windowRect = new(100, 100, clientSize.X, clientSize.Y);
		var style = WindowStyles.Caption | WindowStyles.SysMenu
			| WindowStyles.MinimizeBox | WindowStyles.MaximizeBox | WindowStyles.SizeBox;

		if (visible) style |= WindowStyles.Visible;

		var styleEx = WindowStylesEx.AppWindow;

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

		//Setup Raw Input
		var rawInputDevice = new RawInputDevice(1, 5, 0, _window);
		WinAPI.RegisterRawInputDevices(ref rawInputDevice, 1, (uint)Marshal.SizeOf<RawInputDevice>());

		//Setup OpenGL
		_deviceContext = WinAPI.GetDC(_window);

		var pfDescriptor = new PixelFormatDescriptor();
		var format = WinAPI.ChoosePixelFormat(_deviceContext, ref pfDescriptor);

		WinAPI.SetPixelFormat(_deviceContext, format, ref pfDescriptor);

		var glContext = GL.CreateContext(_deviceContext);
		GL.MakeCurrent(_deviceContext, glContext);
		GL.ImportFunctions();

		//Sync values with PlatformWindow
		var windowSize = WinAPI.GetWindowRect(_window).Size;
		UpdateSize(windowSize, clientSize);
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
				Input.HandleMouseButtonStateChange(MouseButton.Left, true);
				WinAPI.SetCapture(_window);
				break;
			case WindowMessage.LeftButtonUp:
				Input.HandleMouseButtonStateChange(MouseButton.Left, false);
				WinAPI.ReleaseCapture();
				break;
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

			//Raw Input
			case WindowMessage.Input:

				uint bufferSize = 0;
				RawInput rawInput = new();

				var rawInputBytes = new byte[2000];

				WinAPI.GetRawInputData(lParam, 0x10000003, IntPtr.Zero, ref bufferSize, (uint)Marshal.SizeOf<RawInputHeader>());
				WinAPI.GetRawInputData(lParam, 0x10000003, ref rawInput, ref bufferSize, (uint)Marshal.SizeOf<RawInputHeader>());

				WinAPI.GetRawInputDeviceInfo(rawInput.Header.Device, 0x20000005, IntPtr.Zero, ref bufferSize);

				var data = new byte[bufferSize];
				WinAPI.GetRawInputDeviceInfo(rawInput.Header.Device, 0x20000005, ref data[0], ref bufferSize);

				WinAPI.HidP_GetCaps(ref data[0], out HidPCaps capabilities);

				var buttonCaps = new HidPButtonCaps[capabilities.NumberInputButtonCaps];
				var capsLength = capabilities.NumberInputButtonCaps;

				WinAPI.HidP_GetButtonCaps(HidPReportType.Input, ref buttonCaps[0], ref capsLength, ref data[0]);

				var numberOfButtons = buttonCaps[0].Range.UsageMax - buttonCaps[0].Range.UsageMin + 1;

				var valueCaps = new HidPValueCaps[capabilities.NumberInputValueCaps];
				capsLength = capabilities.NumberInputValueCaps;

				WinAPI.HidP_GetValueCaps(HidPReportType.Input, valueCaps, ref capsLength, ref data[0]);

				var x = new ushort[1000];
				uint length = 8;

				WinAPI.HidP_GetUsages(HidPReportType.Input, capabilities.UsagePage, 0, x, ref length, ref data[0],
					ref rawInput.HID.RawData, rawInput.HID.SizeHid);

				string str = "";

				for (int i = 0; i < capabilities.NumberInputValueCaps; i++)
				{
					WinAPI.HidP_GetUsageValue(HidPReportType.Input, valueCaps[i].UsagePage, 0,
					valueCaps[i].Range.UsageMin, out var usageValue, ref data[0],
					ref rawInput.HID.RawData, rawInput.HID.SizeHid);

					str += usageValue;
					str += " : ";
				}

				Console.WriteLine(str);

				//Console.WriteLine(length);

				//Console.WriteLine("adwdaw");
				break;
		}

		return WinAPI.DefWindowProc(window, message, wParam, lParam);
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

		//Update mouse position
		WinAPI.GetCursorPos(out _mousePosition);
		WinAPI.ScreenToClient(_window, ref _mousePosition);
		Input.HandleMousePositionChange(_mousePosition);
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
