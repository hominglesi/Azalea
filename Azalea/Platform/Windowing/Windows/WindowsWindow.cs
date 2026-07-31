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

		var styles = Win32.WindowStyles.OVERLAPPEDWINDOW;
		if (Shown) styles |= Win32.WindowStyles.VISIBLE;

		var extendedStyles = Win32.WindowStylesExtended.APPWINDOW;


		Win32.RECT windowSize = new(100, 100, ClientSize.Value.X, ClientSize.Value.Y);
		Win32.AdjustWindowRectEx(ref windowSize, styles, false, extendedStyles);

		Handle = Win32.CreateWindowExWDLL(
			extendedStyles,
			ClassAtom,
			Title,
			styles,
			windowSize.left,
			windowSize.top,
			windowSize.Width,
			windowSize.Height,
			IntPtr.Zero,
			IntPtr.Zero,
			processHandle,
			IntPtr.Zero);

		if (Handle == IntPtr.Zero)
			throw new Exception($"Could not create Window. (Error {Marshal.GetLastWin32Error()})");

		Marshal.FreeHGlobal(classNamePtr);
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
		switch (uMsg)
		{
			case 5 /* WM_SIZE */:
				var clientSize = BitwiseUtils.SplitValue(lParam);
				ClientSize.Value = clientSize;
				break;
			case 15 /* WM_PAINT */:
				Console.WriteLine("PAINT: " + Time.TimeSinceStart);
				break;
			case 16 /* WM_CLOSE */:
				Scheduler.Schedule(Close);
				return 0;
		}

		return Win32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
	}

	protected override IPlatformDeviceContext GetDeviceContext()
	{
		var deviceContext = Win32.GetDC(Handle);
		return new WindowsDeviceContext(deviceContext, ClientSize);
	}
}
