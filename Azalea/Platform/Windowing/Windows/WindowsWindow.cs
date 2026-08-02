using Azalea.Native.Windows;
using Azalea.Platform.Windows;
using Azalea.Utils;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windowing.Windows;
internal class WindowsWindow(Vector2Int clientSize, bool initiallyVisible)
	: PlatformWindow(clientSize, initiallyVisible)
{
	private static int _nextClassId = 0;
	private Win32.WNDPROC? _windowProcedure;

	private Win32.WindowStyles _windowStyles;
	private Win32.WindowStylesExtended _windowExtendedStyles;

	public override string PlatformType => "Windows";

	internal ushort ClassAtom { get; private set; }
	internal nint Handle { get; private set; }

	protected override void InitializationLogic()
	{
		var processHandle = Process.GetCurrentProcess().Handle;
		_windowProcedure = windowProcedure;

		var classNamePtr = Marshal.StringToHGlobalUni("Azalea Window " + _nextClassId++);
		var winProcPtr = Marshal.GetFunctionPointerForDelegate(_windowProcedure);

		var wndClass = new Win32.WNDCLASSEXW
		{
			lpszClassName = classNamePtr,
			hInstance = processHandle,
			lpfnWndProc = winProcPtr,
			style = Win32.WNDCLASSEXW.ClassStyles.OWNDC,
			hCursor = WinAPI.LoadCursor(nint.Zero, 32512)
		};

		ClassAtom = Win32.RegisterClassExW(ref wndClass);

		_windowStyles = Win32.WindowStyles.OVERLAPPEDWINDOW;
		if (Shown) _windowStyles |= Win32.WindowStyles.VISIBLE;

		_windowExtendedStyles = Win32.WindowStylesExtended.APPWINDOW;

		Win32.RECT windowRect = new(ClientPosition.Value.X, ClientPosition.Value.Y, ClientSize.Value.X, ClientSize.Value.Y);
		Win32.AdjustWindowRectEx(ref windowRect, _windowStyles, false, _windowExtendedStyles);

		Handle = Win32.CreateWindowExWDLL(
			_windowExtendedStyles,
			ClassAtom,
			Title,
			_windowStyles,
			windowRect.left,
			windowRect.top,
			windowRect.Width,
			windowRect.Height,
			IntPtr.Zero,
			IntPtr.Zero,
			processHandle,
			IntPtr.Zero);

		if (Handle == IntPtr.Zero)
			throw new Exception($"Could not create Window. (Error {Marshal.GetLastWin32Error()})");

		Marshal.FreeHGlobal(classNamePtr);

		// Set actual window position
		Win32.GetWindowRect(Handle, out windowRect);
		Position.Value = new Vector2Int(windowRect.X, windowRect.Y);
		Size.Value = new Vector2Int(windowRect.Width, windowRect.Height);
	}

	protected override void UpdateLogic()
	{
		while (Win32.PeekMessageW(out Win32.MSG message, Handle, 0, 0, 0x0001) != 0)
		{
			Win32.TranslateMessage(in message);
			Win32.DispatchMessageW(in message);
		}
	}

	protected override void HandleCommandLogic(WindowCommand command)
	{
		switch (command)
		{
			case CenterCommand:
				var monitor = Win32.MonitorFromWindow(Handle, Win32.MonitorFromWindowFlags.DEFAULTTONEAREST);
				var monitorInfo = new Win32.MonitorInfo();
				Win32.GetMonitorInfoW(monitor, ref monitorInfo);

				var workArea = monitorInfo.rcWork;
				var centerPosition = new Vector2Int(workArea.X, workArea.Y)
					+ (new Vector2Int(workArea.Width, workArea.Height) / 2 - Size.Value / 2);
				Win32.SetWindowPos(Handle, IntPtr.Zero, centerPosition.X, centerPosition.Y, 0, 0, Win32.SetWindowPosFlags.NOSIZE);
				break;
			case FocusCommand:
				Win32.BringWindowToTop(Handle);
				Win32.SetForegroundWindow(Handle);
				Win32.SetFocus(Handle);
				break;
			case HideCommand:
				Win32.ShowWindow(Handle, Win32.ShowWindowCommand.HIDE);
				break;
			case MaximizeCommand:
				Win32.ShowWindow(Handle, Win32.ShowWindowCommand.SHOWMAXIMIZED);
				break;
			case MinimizeCommand:
				Win32.ShowWindow(Handle, Win32.ShowWindowCommand.SHOWMINIMIZED);
				break;
			case RequestAttentionCommand:
				Win32.FlashWindow(Handle, true);
				break;
			case RestoreCommand:
				Win32.ShowWindow(Handle, Win32.ShowWindowCommand.SHOWNORMAL);
				break;
			case SetClientPositionCommand(var clientPosition):
				var newPosition = new Win32.RECT(clientPosition.X, clientPosition.Y, 0, 0);
				Win32.AdjustWindowRectEx(ref newPosition, _windowStyles, false, _windowExtendedStyles);
				Win32.SetWindowPos(Handle, IntPtr.Zero, newPosition.X, newPosition.Y, 0, 0, Win32.SetWindowPosFlags.NOSIZE);
				break;
			case SetClientSizeCommand(var clientSize):
				var newSize = new Win32.RECT(0, 0, clientSize.X, clientSize.Y);
				Win32.AdjustWindowRectEx(ref newSize, _windowStyles, false, _windowExtendedStyles);
				Win32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, newSize.Width, newSize.Height, Win32.SetWindowPosFlags.NOMOVE);
				break;
			case SetCursorVisibleCommand(var isVisible):
				Win32.ShowCursor(isVisible);
				CursorVisible.Value = isVisible;
				break;
			case SetIconCommand(var image):
				IntPtr icon = IntPtr.Zero;
				var deviceContext = Win32.GetDC(Handle);

				if (image is not null)
					icon = Win32.CreateIconFromPixelArray(deviceContext, image.Width, image.Height, image.Data);

				Win32.SendMessageW(Handle, Win32.WindowMessage.SETICON, nint.Zero, icon);
				Win32.SendMessageW(Handle, Win32.WindowMessage.SETICON, 1, icon);

				if (icon != nint.Zero)
					Win32.DeleteObject(icon);

				break;
			case SetPositionCommand(var position):
				Win32.SetWindowPos(Handle, IntPtr.Zero, position.X, position.Y, 0, 0, Win32.SetWindowPosFlags.NOSIZE);
				break;
			case SetSizeCommand(var size):
				Win32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, size.X, size.Y, Win32.SetWindowPosFlags.NOMOVE);
				break;
			case SetTitleCommand(var title):
				Win32.SetWindowTextW(Handle, title);
				Title.Value = title;
				break;
			case ShowCommand:
				Win32.ShowWindow(Handle, Win32.ShowWindowCommand.SHOW);
				break;
			default:
				throw new NotImplementedException("Command handling hasn't been implemented");
		}

		command.Return();
	}

	private nint windowProcedure(nint hWnd, uint uMsg, nint wParam, nint lParam)
	{
		switch ((Win32.WindowMessage)uMsg)
		{
			case Win32.WindowMessage.MOVE:
				Win32.GetWindowRect(Handle, out var rect);
				Position.Value = new Vector2Int(rect.X, rect.Y);
				ClientPosition.Value = BitwiseUtils.SplitValue(lParam);
				break;
			case Win32.WindowMessage.SIZE:
				Win32.GetWindowRect(Handle, out rect);
				Size.Value = new Vector2Int(rect.Width, rect.Height);
				ClientSize.Value = BitwiseUtils.SplitValue(lParam);
				break;
			case Win32.WindowMessage.SHOWWINDOW:
				Shown.Value = wParam != nint.Zero;
				break;
			case Win32.WindowMessage.CLOSE:
				Close();
				return nint.Zero;
			case Win32.WindowMessage.ERASEBKGND:
				return 1;
		}

		return Win32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
	}

	protected override IPlatformDeviceContext GetDeviceContext()
	{
		var deviceContext = Win32.GetDC(Handle);
		return new WindowsDeviceContext(deviceContext, ClientSize);
	}
}
