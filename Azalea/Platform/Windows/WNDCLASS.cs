using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
[StructLayout(LayoutKind.Sequential)]
internal struct WNDCLASS
{
	public ClassStyles style;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public WndProc lpfnWndProc;

	public int cbClsExtra;

	public int cbWndExtra;

	public IntPtr hInstance;

	public IntPtr hIcon;

	public IntPtr hCursor;

	public IntPtr hbrBackground;

	[MarshalAs(UnmanagedType.LPTStr)]
	public string lpszMenuName;

	[MarshalAs(UnmanagedType.LPTStr)]
	public string lpszClassName;
}

public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
