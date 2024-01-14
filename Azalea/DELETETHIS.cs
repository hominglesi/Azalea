using Azalea.Platform.Windows;
using System;
using System.Runtime.InteropServices;

namespace Azalea;
public static class DELETETHIS
{
	public const string WindowClassName = "Azalea Window";

	public static void Run()
	{
		var programHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

		var wndClass = new WNDCLASS()
		{
			lpszClassName = WindowClassName,
			hInstance = programHandle,
			lpfnWndProc = wndProc
		};
		var id = WinAPI.RegisterClass(ref wndClass);

		var hwnd = WinAPI.CreateWindowEx(
			0,
			id,
			"Azalea Window",
			WindowStyles.WS_OVERLAPPED,
			100,
			100,
			1280,
			720,
			IntPtr.Zero,
			IntPtr.Zero,
			programHandle,
			IntPtr.Zero);

		if (hwnd == IntPtr.Zero)
		{
			Console.WriteLine(Marshal.GetLastWin32Error());
			return;
		}

		WinAPI.ShowWindow(hwnd, ShowWindowCommands.Show);

		while (WinAPI.GetMessage(out MSG msg, hwnd, 0, 0) > 0)
		{
			WinAPI.TranslateMessage(ref msg);
			WinAPI.DispatchMessage(ref msg);
		}
	}

	private static IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		return WinAPI.DefWindowProc(hWnd, msg, wParam, lParam);
	}
}
