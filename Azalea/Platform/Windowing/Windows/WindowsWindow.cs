using Azalea.Native.Windows;
using Azalea.Platform.Windows;
using Azalea.Threading;
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

	protected override void Initialize()
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
	}

	protected override void Update()
	{
		while (Win32.PeekMessageW(out Win32.MSG message, Handle, 0, 0, 0x0001) != 0)
		{
			Win32.TranslateMessage(in message);
			Win32.DispatchMessageW(in message);
		}
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
				var clientSize = BitwiseUtils.SplitValue(lParam);
				ClientSize.Value = clientSize;
				break;
			case Win32.WindowMessage.CLOSE:
				Scheduler.Schedule(Close);
				return 0;
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
