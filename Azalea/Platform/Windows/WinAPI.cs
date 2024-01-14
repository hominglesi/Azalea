using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal static class WinAPI
{
	private const string User32Path = "user32.dll";

	[DllImport(User32Path, SetLastError = true)]
	public static extern ushort RegisterClass([In] ref WNDCLASS lpWndClass);

	[DllImport(User32Path, SetLastError = true)]
	public static extern IntPtr CreateWindowEx(
		WindowStylesEx dwExStyle,
		ushort lpClassName,
		string lpWindowName,
		WindowStyles dwStyle,
		int x,
		int y,
		int nWidth,
		int nHeight,
		IntPtr hWndParent,
		IntPtr hMenu,
		IntPtr hInstance,
		IntPtr lpParam);

	[DllImport(User32Path)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
	[DllImport(User32Path)]
	public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
	[DllImport(User32Path)]
	public static extern bool UpdateWindow(IntPtr hWnd);
	[DllImport(User32Path)]
	public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);
	[DllImport(User32Path)]
	public static extern bool TranslateMessage([In] ref MSG lpMsg);
	[DllImport(User32Path)]
	public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
	   uint wMsgFilterMax);
}
