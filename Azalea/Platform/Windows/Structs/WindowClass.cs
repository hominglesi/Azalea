using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

//Mapping of WNDCLASSEXW
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WindowClass
{
	private readonly uint cbSize;

	public ClassStyles Style = 0;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	private readonly WindowProcedure lpfnWndProc;

	private readonly int cbClsExtra = 0;

	private readonly int cbWndExtra = 0;

	private readonly IntPtr hInstance;

	private readonly IntPtr hIcon = IntPtr.Zero;

	public IntPtr Cursor = IntPtr.Zero;

	private readonly IntPtr hbrBackground = IntPtr.Zero;

	[MarshalAs(UnmanagedType.LPTStr)]
	private readonly string? lpszMenuName = null;

	[MarshalAs(UnmanagedType.LPTStr)]
	private readonly string lpszClassName;

	private readonly IntPtr hIconSm = IntPtr.Zero;

	public WindowClass(string className, IntPtr instance, WindowProcedure procedure)
	{
		cbSize = (uint)Marshal.SizeOf<WindowClass>();
		lpszClassName = className;
		hInstance = instance;
		lpfnWndProc = procedure;
	}
}

public delegate IntPtr WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
