using Azalea.Native.Windows;
using Azalea.Platform.Windows;
using Azalea.Threading;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windowing.Windows;
internal class WindowsWindow(bool initiallyVisible) : PlatformWindow(initiallyVisible)
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

		var styles = Win32.WindowStyles.OVERLAPPEDWINDOW | Win32.WindowStyles.VISIBLE;

		Handle = Win32.CreateWindowExWDLL(
			Win32.WindowStylesExtended.APPWINDOW,
			ClassAtom,
			Title,
			styles,
			100,
			100,
			800,
			600,
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
			case 16 /* WM_CLOSE */:
				Scheduler.Schedule(Close);
				return IntPtr.Zero;
		}

		return Win32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
	}

	protected override PlatformDeviceContext GetDeviceContext()
	{
		var deviceContext = Win32.GetDC(Handle);
		return new WindowsDeviceContext(deviceContext);
	}
}
