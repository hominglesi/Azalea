using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

internal static class WinAPI
{
	private const string User32Path = "user32.dll";
	private const string Gdi32Path = "gdi32.dll";

	[DllImport(Gdi32Path, EntryPoint = "ChoosePixelFormat")]
	public static extern int ChoosePixelFormat(IntPtr deviceContext, [In] ref PixelFormatDescriptor descriptor);

	[DllImport(User32Path, EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode)]
	public static extern IntPtr CreateWindow(
		WindowStylesEx stylesEx,
		ushort classAtom,
		string name,
		WindowStyles styles,
		int x,
		int y,
		int width,
		int height,
		IntPtr parent,
		IntPtr menu,
		IntPtr instance,
		IntPtr param);

	[DllImport(User32Path, EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
	public static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

	[DllImport(User32Path, EntryPoint = "DispatchMessageW", CharSet = CharSet.Unicode)]
	public static extern IntPtr DispatchMessage([In] ref Message message);

	[DllImport(User32Path, EntryPoint = "EnableWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnableWindow(IntPtr window, bool enable);

	[DllImport(User32Path, EntryPoint = "GetDC")]
	public static extern IntPtr GetDC(IntPtr window);

	[DllImport(User32Path, EntryPoint = "GetMessageW", CharSet = CharSet.Unicode)]
	private static extern sbyte getMessage(out Message message, IntPtr window, uint wMsgFilterMin, uint wMsgFilterMax);
	public static sbyte GetMessage(out Message message, IntPtr window) => getMessage(out message, window, 0, 0);

	[DllImport(User32Path, EntryPoint = "IsDialogMessageW", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool IsDialogMessage(IntPtr window, [In] ref Message message);

	[DllImport(User32Path, EntryPoint = "LoadCursorW", CharSet = CharSet.Unicode)]
	public static extern IntPtr LoadCursor(IntPtr instance, uint cursorValue);

	[DllImport(User32Path, EntryPoint = "PostQuitMessage")]
	public static extern void PostQuitMessage(int exitCode);

	[DllImport(User32Path, EntryPoint = "RedrawWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RedrawWindow(IntPtr window, WinRectangle? rectangle, IntPtr region, uint flags);

	[DllImport(User32Path, EntryPoint = "RegisterClassExW", CharSet = CharSet.Unicode)]
	public static extern ushort RegisterClass([In] ref WindowClass windowClass);

	[DllImport(Gdi32Path, EntryPoint = "SetPixelFormat")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetPixelFormat(IntPtr deviceContext, int format, [In] ref PixelFormatDescriptor descriptor);

	[DllImport(User32Path, EntryPoint = "SetWindowLongW", CharSet = CharSet.Unicode)]
	public static extern uint SetWindowLong(IntPtr window, int index, uint newValue);

	[DllImport(User32Path, EntryPoint = "SetWindowPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(
		IntPtr window,
		IntPtr insertAfter,
		int x,
		int y,
		int width,
		int height,
		uint flags);

	[DllImport(User32Path, EntryPoint = "ShowWindow")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr window, ShowWindowCommand showCommand);

	[DllImport(Gdi32Path, EntryPoint = "SwapBuffers")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SwapBuffers(IntPtr deviceContext);

	[DllImport(User32Path, EntryPoint = "TranslateMessage")]
	public static extern bool TranslateMessage([In] ref Message message);

	[DllImport(User32Path, EntryPoint = "UpdateWindow")]
	public static extern bool UpdateWindow(IntPtr window);
}
